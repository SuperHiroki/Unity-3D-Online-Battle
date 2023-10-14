using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonEventSetCharaScene : MonoBehaviour
{
    private void Start()
    {
        SetButtonDisable();
        SetButtonActions();
    }

    //#####################################################################################
    //#####################################################################################
    //ボタンの無効化と有効化
    private void SetButtonDisable()
    {
        foreach (var pair in RoomPlayerInfo.buttonsCharaScene)
        {
            var key = pair.Key;
            var button = pair.Value;
            Button currentButton = button;
            button.onClick.AddListener(() =>
            {
                DisableButton(currentButton);
                StartCoroutine(EnableButtonWithDelay(currentButton, GlobalDefine.ButtonsDefineDictCharaScene[key].disableTime));
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

    //#####################################################################################
    //#####################################################################################
    //ボタンのメインイベント
    private void SetButtonActions()
    {
        //ホーム画面に戻る
        RoomPlayerInfo.buttonsCharaScene["BackToHome"].onClick.AddListener(() =>
        {
            /*
            // シーンの全てのオブジェクトを破壊
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                Destroy(obj);
            }
            */
            // シーンを切り替え
            SceneManager.LoadScene("StartScene");
        });
    }
}
