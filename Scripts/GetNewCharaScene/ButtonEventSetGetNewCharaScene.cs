using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class ButtonEventSetGetNewCharaScene : MonoBehaviour
{
    //グローバル変数
    private bool isLogin => UserDataManager.IsLogin;
    private CharacterData[] charactersArray => UserDataManager.UserData.Characters;
    private UserData userData => UserDataManager.UserData;

    //外部クラス参照
    [SerializeField] private ImageManagerGetNewCharaCharaScene imageManagerGetNewCharaCharaScene;
    [SerializeField] private TextManagerGetNewCharaScene textManagerGetNewCharaScene;

    private void Start()
    {
        SetButtonDisable();
        SetButtonActions();
    }
    //###################################################################################################################
    //###################################################################################################################
    //###################################################################################################################
    //###################################################################################################################
    //ボタンの無効化と有効化
    private void SetButtonDisable()
    {
        foreach (var pair in RoomPlayerInfo.buttonsGetNewCharaScene)
        {
            var key = pair.Key;
            var button = pair.Value;
            Button currentButton = button;
            button.onClick.AddListener(() =>
            {
                DisableButton(currentButton);
                StartCoroutine(EnableButtonWithDelay(currentButton, GlobalDefine.ButtonsDefineDictGetNewCharaScene[key].disableTime));
            });
        }
    }

    //ボタンを再び有効化
    private IEnumerator EnableButtonWithDelay(Button btn, float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableButton(btn);
    }

    private void EnableButton(Button btn)
    {
        btn.interactable = true;
    }

    //ボタンを無効化
    private void DisableButton(Button btn)
    {
        btn.interactable = false;
    }

    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //ボタンにアクションを設定
    private void SetButtonActions()
    {
        //ホーム画面に戻る
        RoomPlayerInfo.buttonsGetNewCharaScene["BackToHome"].onClick.AddListener(() =>
        {
            ShutdownAndLoad();
        });

        //ガチャを引く
        RoomPlayerInfo.buttonsGetNewCharaScene["GetNewChara"].onClick.AddListener(() =>
        {
            if (!isLogin)
            {
                TextManagerAllScene.MakeAlertText("You must Login");
                return;
            }
            //MagicStoneの個数を確認
            if (userData.MagicStone < 3)
            {
                TextManagerAllScene.MakeAlertText($"{GlobalDefine.MagicStoneGetNewChara} Magic Stone Needed.");
                return;
            }
            int randomIndex = UnityEngine.Random.Range(0, GlobalDefine.CharaNamesList.Length);
            GetCharacter(randomIndex);
            imageManagerGetNewCharaCharaScene.ShowCharaImage(randomIndex);
            textManagerGetNewCharaScene.ShowCharaStatus(randomIndex);
            //リセマラを防ぐためにすぐにAPIにとばす
            SaveUserInformation.Instance.SaveUserData();
        });
    }

    //#####################################################################################
    //#####################################################################################
    //ホーム画面に戻る関数
    private void ShutdownAndLoad()
    {
        /*
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            Destroy(obj);
        }
        */
        SceneManager.LoadScene("StartScene");
    }

    //#####################################################################################
    //#####################################################################################
    //ガチャの時にキャラを追加する
    private void GetCharacter(int characterId)
    {
        var tempUserData = userData; // これを最終的に丸ごとコピーする
        //MagicStoneを消費する
        tempUserData.MagicStone -= 3;
        //キャラを追加もしくは覚醒
        CharacterData charaData = Array.Find(tempUserData.Characters, c => c.CharacterId == characterId);
        if (charaData != null)
        {
            if(charaData.Awakening == GlobalDefine.CharaMaxAwakening)
            {
                TextManagerAllScene.MakeAlertText("Already Max Awakening");
            }
            else
            {
                charaData.Awakening += 1;
            }
        }
        else
        {
            CharacterData newCharacterData = new CharacterData
            {
                CharacterId = characterId,
                CharacterLevel = 1,
                Awakening = 0,
                Reliability = 0,
                Experience = 0
            };
            List<CharacterData> characterDataList = new List<CharacterData>(tempUserData.Characters);
            characterDataList.Add(newCharacterData);
            tempUserData.Characters = characterDataList.ToArray();
        }
        UserDataManager.UpdateAllUserData(tempUserData);
    }
    //#####################################################################################
    //#####################################################################################
}