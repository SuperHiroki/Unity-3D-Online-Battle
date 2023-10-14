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
    //�O���[�o���ϐ�
    private Dictionary<int, int> killCountEachCharaDict => UserDataManager.KillCountEachCharaDict;
    private Dictionary<int, int> killedCountEachCharaDict => UserDataManager.KilledCountEachCharaDict;
    private UserData userData => UserDataManager.UserData;
    private CharacterData[] characters => UserDataManager.UserData.Characters;
    private string username => UserDataManager.UserData.Username;
    private int userLevel => UserDataManager.UserData.UserLevel;
    private int experience => UserDataManager.UserData.Experience;
    private int magicStone => UserDataManager.UserData.MagicStone;
    private bool isLogin => UserDataManager.IsLogin;

    //�O��̏󋵂�ۑ����Ă����ꏊ(�Q�[����ʂ��ĕێ�����̂�static�ɂ���)�B����ς�Singleton�ɂ���
    private UserData oldUserData = UserDataManager.UserData;

    //�V�[����Ɉ�����Ȃ��悤��
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
        //�ۗL�L�����̏��̍X�V
        List<int> grownCharaList = new List<int>();
        //�ύX�O�̃f�[�^���i�[
        int oldUserLevel = userLevel;
        int oldExperience = experience;
        int oldMagicStone = magicStone;
        CharacterData[] oldCharacters = characters;
        CharacterData[] tempCharacters = characters;
        //�J�E���g
        int killCount =0;
        foreach (var characterId in killCountEachCharaDict.Keys)
        {
            //���̃L�����̃L����
            int killCountThisChara = killCountEachCharaDict[characterId];
            //���v�L�����̌v�Z
            killCount += killCountThisChara;
            UnityEngine.Debug.Log("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW killCount " + killCount);
            //�L�����f�[�^�̍X�V
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
            //�ő�𒴂��ĂȂ����ǂ����m�F����
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
            //�X�e�[�^�X�ύX�̂������L������ǉ�
            if (!grownCharaList.Contains(characterId))
            {
                grownCharaList.Add(characterId);
            }
        }
        //��L�����̌v�Z
        int killedCount = 0;
        foreach (var characterId in killedCountEachCharaDict.Keys)
        {
            //���̃L�����̃L����
            int killedCountThisChara = killedCountEachCharaDict[characterId];
            //���v�L�����̌v�Z
            killedCount += killedCountThisChara;
            //�L�����f�[�^�̍X�V
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
            //�ő�𒴂��ĂȂ����ǂ����m�F����
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
            //�X�e�[�^�X�ύX�̂������L������ǉ�
            if (!grownCharaList.Contains(characterId))
            {
                grownCharaList.Add(characterId);
            }
        }
        UserDataManager.UpdateCharacters(tempCharacters);
        //���[�U���g�̃f�[�^�̍X�V
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
        //���x���A�b�v�̂��߂̃L�����Ȃǂ����Z�b�g����
        UserDataManager.ClearKillCountEachCharaDict();
        UserDataManager.ClearKilledCountEachCharaDict();
    }

    //�{�Ԋ��ł͎g��Ȃ��B�X�e�[�^�X��\������(�{�Ԃł͎g��Ȃ�)
    private void ShowChangedStatus(UserData oldUserData, UserData newUserData)
    {
        //�\������
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
        //API�ɃZ�[�u�f�[�^�𑗐M����
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

    //���[�U�����X�V����
    private void ChangeShowedStatus()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name == "StartScene")
        {
            //�\��
            RoomPlayerInfo.texts["UserName"].keyText.text = username;
            RoomPlayerInfo.texts["UserLevel"].keyText.text = userLevel.ToString() + "  Level";
            RoomPlayerInfo.texts["Experience"].keyText.text = "Experience: " + experience + " / " + GlobalDefine.UserLevelUpExperienceList[userData.UserLevel - 1]; ;
            RoomPlayerInfo.texts["MagicStone"].keyText.text = "Magic Stone: " + magicStone;
            CreateScrollViewForCharacters.Instance.CreateScrollView();
            TextManagerAllScene.MakeAlertText("You Saved.");
        }
    }
}
