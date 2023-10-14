using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonEventSetFirstScene : MonoBehaviour
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
        foreach (var pair in RoomPlayerInfo.buttonsFirstScene)
        {
            var key = pair.Key;
            var button = pair.Value;
            Button currentButton = button;
            button.onClick.AddListener(() =>
            {
                DisableButton(currentButton);
                StartCoroutine(EnableButtonWithDelay(currentButton, GlobalDefine.ButtonsDefineDictFirstScene[key].disableTime));
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
    //ボタンにアクションを設定
    private void SetButtonActions()
    {
        RoomPlayerInfo.buttonsFirstScene["TapToStart"].onClick.AddListener(() =>
        {
            SceneManager.LoadScene("StartScene");
        });
    }
}
