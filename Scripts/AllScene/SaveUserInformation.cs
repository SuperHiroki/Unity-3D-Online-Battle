using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SaveUserInformation : MonoBehaviour
{
    //グローバル変数
    private Dictionary<int, int> killCountEachCharaDict => UserDataManager.KillCountEachCharaDict;
    private Dictionary<int, int> killedCountEachCharaDict => UserDataManager.KilledCountEachCharaDict;
    private UserData userData => UserDataManager.UserData;
    private CharacterData[] characters => UserDataManager.UserData.Characters;
    private string username => UserDataManager.UserData.Username;
    private int userLevel => UserDataManager.UserData.UserLevel;
    private int experience => UserDataManager.UserData.Experience;
    private int magicStone => UserDataManager.UserData.MagicStone;
    private bool isLogin => UserDataManager.IsLogin;

    //前回の状況を保存しておく場所(ゲームを通して保持するのでstaticにする)。やっぱりSingletonにする
    private UserData oldUserData = UserDataManager.UserData;

    //シーン上に一つしかないように
    public static SaveUserInformation Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveUserData()
    {
        if (!UserDataManager.IsLogin)
        {
            TextManagerAllScene.MakeAlertText("You have to login", "Alert");
            return;
        }
        KillKilledCountToData();
        UserData newUserData = userData;
        StartCoroutine(SaveUserDataCoroutine(newUserData));
    }

    private IEnumerator SaveUserDataCoroutine(UserData newUserData)
    {
        yield return StartCoroutine(UpdateDBUserData(newUserData));
        ChangeShowedStatus();
        oldUserData = userData;
        UnityEngine.Debug.Log("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW Saved");
    }

    private void KillKilledCountToData()
    {
        if (killCountEachCharaDict == null && killCountEachCharaDict.Count == 0 && killedCountEachCharaDict == null && killedCountEachCharaDict.Count == 0)
        {
            UnityEngine.Debug.Log("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC There is No kill count to save.");
            return;
        }
        //保有キャラの情報の更新
        List<int> grownCharaList = new List<int>();
        //変更前のデータを格納
        int oldUserLevel = userLevel;
        int oldExperience = experience;
        int oldMagicStone = magicStone;
        CharacterData[] oldCharacters = characters;
        CharacterData[] tempCharacters = characters;
        //カウント
        int killCount =0;
        foreach (var characterId in killCountEachCharaDict.Keys)
        {
            //このキャラのキル数
            int killCountThisChara = killCountEachCharaDict[characterId];
            //合計キル数の計算
            killCount += killCountThisChara;
            UnityEngine.Debug.Log("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW killCount " + killCount);
            //キャラデータの更新
            CharacterData foundCharacter = Array.Find(tempCharacters, character => character.CharacterId == characterId);
            int leftExperience = foundCharacter.Experience + killCountThisChara * 123;
            while (GlobalDefine.CharaLevelUpExperienceList[foundCharacter.CharacterLevel - 1] <= leftExperience)
            {
                leftExperience -= GlobalDefine.CharaLevelUpExperienceList[foundCharacter.CharacterLevel - 1];
                foundCharacter.CharacterLevel += 1;
            }
            foundCharacter.Experience = leftExperience;
            foundCharacter.Awakening += 0;
            foundCharacter.Reliability += killCountThisChara * 1;
            //最大を超えてないかどうか確認する
            if (GlobalDefine.CharaMaxLevel <= foundCharacter.CharacterLevel)
            {
                foundCharacter.CharacterLevel = GlobalDefine.CharaMaxLevel;
                foundCharacter.Experience = 0;
            }
            if (GlobalDefine.CharaMaxAwakening <= foundCharacter.Awakening)
            {
                foundCharacter.Awakening = GlobalDefine.CharaMaxAwakening;
            }
            if (GlobalDefine.CharaMaxReliability <= foundCharacter.Reliability)
            {
                foundCharacter.Reliability = GlobalDefine.CharaMaxReliability;
            }
            //ステータス変更のあったキャラを追加
            if (!grownCharaList.Contains(characterId))
            {
                grownCharaList.Add(characterId);
            }
        }
        //被キル数の計算
        int killedCount = 0;
        foreach (var characterId in killedCountEachCharaDict.Keys)
        {
            //このキャラのキル数
            int killedCountThisChara = killedCountEachCharaDict[characterId];
            //合計キル数の計算
            killedCount += killedCountThisChara;
            //キャラデータの更新
            CharacterData foundCharacter = Array.Find(tempCharacters, character => character.CharacterId == characterId);
            int leftExperience = foundCharacter.Experience + killedCountThisChara * 120;
            while (GlobalDefine.CharaLevelUpExperienceList[foundCharacter.CharacterLevel - 1] <= leftExperience)
            {
                leftExperience -= GlobalDefine.CharaLevelUpExperienceList[foundCharacter.CharacterLevel - 1];
                foundCharacter.CharacterLevel += 1;
            }
            foundCharacter.Experience = leftExperience;
            foundCharacter.Awakening += 0;
            foundCharacter.Reliability += 0;
            //最大を超えてないかどうか確認する
            if (GlobalDefine.CharaMaxLevel <= foundCharacter.CharacterLevel)
            {
                foundCharacter.CharacterLevel = GlobalDefine.CharaMaxLevel;
                foundCharacter.Experience = 0;
            }
            if (GlobalDefine.CharaMaxAwakening <= foundCharacter.Awakening)
            {
                foundCharacter.Awakening = GlobalDefine.CharaMaxAwakening;
            }
            if (GlobalDefine.CharaMaxReliability <= foundCharacter.Reliability)
            {
                foundCharacter.Reliability = GlobalDefine.CharaMaxReliability;
            }
            //ステータス変更のあったキャラを追加
            if (!grownCharaList.Contains(characterId))
            {
                grownCharaList.Add(characterId);
            }
        }
        UserDataManager.UpdateCharacters(tempCharacters);
        //ユーザ自身のデータの更新
        int tempUserLevel = oldUserLevel;
        int tempExperience = oldExperience + killCount * 111 + killedCount * 220;
        while (GlobalDefine.UserLevelUpExperienceList[tempUserLevel - 1] <= tempExperience)
        {
            tempExperience -= GlobalDefine.UserLevelUpExperienceList[tempUserLevel - 1];
            tempUserLevel += 1;
        }
        int tempMagicStone = oldMagicStone + killCount * 1 + killedCount * 0;
        if (GlobalDefine.UserMaxLevel <= tempUserLevel)
        {
            tempUserLevel = GlobalDefine.UserMaxLevel;
            tempExperience = 0;
        }
        if (GlobalDefine.UserMaxMagicStone <= tempMagicStone)
        {
            tempMagicStone = GlobalDefine.UserMaxMagicStone;
        }
        UserDataManager.UpdateUserData(tempUserLevel, tempExperience, tempMagicStone);
        //レベルアップのためのキル数などをリセットする
        UserDataManager.ClearKillCountEachCharaDict();
        UserDataManager.ClearKilledCountEachCharaDict();
    }

    //本番環境では使わない。ステータスを表示する(本番では使わない)
    private void ShowChangedStatus(UserData oldUserData, UserData newUserData)
    {
        //表示する
        string someString = "User Level: " + oldUserData.UserLevel + " -> " + newUserData.UserLevel + "\n"
                            + "User Experience: " + oldUserData.Experience + " / " + GlobalDefine.CharaLevelUpExperienceList[oldUserData.UserLevel - 1] + " -> " + newUserData.Experience + " / " + GlobalDefine.UserLevelUpExperienceList[newUserData.UserLevel - 1] + "\n"
                            + "Magic Stone: " + oldUserData.MagicStone + " -> " + newUserData.MagicStone + "\n";
        foreach (var character in newUserData.Characters)
        {
            int characterId = character.CharacterId;
            CharacterData foundOldCharacter = Array.Find(oldUserData.Characters, character => character.CharacterId == characterId);
            CharacterData foundNewCharacter = Array.Find(newUserData.Characters, character => character.CharacterId == characterId);
            if (foundOldCharacter == null)
            {
                someString += "Chara Name: " + GlobalDefine.CharaNamesList[characterId] + "\n"
                           + "Character Level: " + "???" + " -> " + foundNewCharacter.CharacterLevel + "\n"
                             + "Character Awakening: " + "???" + " -> " + foundNewCharacter.Awakening + "\n"
                              + "Character Reliability: " + "???" + " -> " + foundNewCharacter.Reliability + "\n"
                              + "Character Experience: " + "???" + " / " + "???" + " -> " + foundNewCharacter.Experience + " / " + GlobalDefine.CharaLevelUpExperienceList[foundNewCharacter.CharacterLevel - 1];
            }
            else
            {
                someString += "Chara Name: " + GlobalDefine.CharaNamesList[characterId] + "\n"
                            + "Character Level: " + foundOldCharacter.CharacterLevel + " -> " + foundNewCharacter.CharacterLevel + "\n"
                             + "Character Awakening: " + foundOldCharacter.Awakening + " -> " + foundNewCharacter.Awakening + "\n"
                              + "Character Reliability: " + foundOldCharacter.Reliability + " -> " + foundNewCharacter.Reliability + "\n"
                               + "Character Experience: " + foundOldCharacter.Experience + " / " + GlobalDefine.CharaLevelUpExperienceList[foundOldCharacter.CharacterLevel - 1] + " -> " + foundNewCharacter.Experience + " / " + GlobalDefine.CharaLevelUpExperienceList[foundNewCharacter.CharacterLevel - 1];
            }
        }
        TextManagerAllScene.MakeAlertText(someString, "SomeText");
    }

    IEnumerator UpdateDBUserData(UserData userData)
    {
        //APIにセーブデータを送信する
        string url = GlobalDefine.BaseUrl + "user/save";
        string json = JsonUtility.ToJson(new UserData { Username = userData.Username, Password = "SomePassword", UserLevel = userData.UserLevel, Experience = userData.Experience, MagicStone = userData.MagicStone, Characters = userData.Characters });
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, json);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.responseCode == 201)
            {
                string response = request.downloadHandler.text;
                Message message = JsonUtility.FromJson<Message>(response);
            }
        }
        else
        {
            if (request.responseCode == 400)
            {
                TextManagerAllScene.MakeAlertText("Bad Request.");
            }
            else if (request.responseCode == 404)
            {
                TextManagerAllScene.MakeAlertText("Something went wrong at server");
                UnityEngine.Debug.Log(request.error);
            }
            else
            {
                TextManagerAllScene.MakeAlertText("Something went wrong......");
                UnityEngine.Debug.Log(request.error);
            }
        }
    }

    //ユーザ情報を更新する
    private void ChangeShowedStatus()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name == "StartScene")
        {
            //表示
            RoomPlayerInfo.texts["UserName"].keyText.text = username;
            RoomPlayerInfo.texts["UserLevel"].keyText.text = userLevel.ToString() + "  Level";
            RoomPlayerInfo.texts["Experience"].keyText.text = "Experience: " + experience + " / " + GlobalDefine.UserLevelUpExperienceList[userData.UserLevel - 1]; ;
            RoomPlayerInfo.texts["MagicStone"].keyText.text = "Magic Stone: " + magicStone;
            CreateScrollViewForCharacters.Instance.CreateScrollView();
            TextManagerAllScene.MakeAlertText("You Saved.");
        }
    }
}
