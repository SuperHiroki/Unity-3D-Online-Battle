using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonEventSet : MonoBehaviour
{
    [SerializeField] private RelayManager instanceRelayManager;
    [SerializeField] private UserManager instanceUserManager;
    [SerializeField] private RoomManager instanceRoomManager;

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
        foreach (var pair in RoomPlayerInfo.buttons)
        {
            var key = pair.Key;
            var button = pair.Value;
            Button currentButton = button;
            button.onClick.AddListener(() =>
            {
                DisableButton(currentButton);
                StartCoroutine(EnableButtonWithDelay(currentButton, GlobalDefine.ButtonsDefineDict[key].disableTime));
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
        RoomPlayerInfo.buttons["MakeRoom"].onClick.AddListener(() =>
        {
            instanceRelayManager.CreateRelayButtonAsHost();
        });
        RoomPlayerInfo.buttons["EnterRoom"].onClick.AddListener(() =>
        {
            instanceRelayManager.JoinRelayButton(RoomPlayerInfo.inputFields["EnterRoomId"].text);
        });
        RoomPlayerInfo.buttons["MakeRoomAsServer"].onClick.AddListener(() =>
        {
            instanceRelayManager.CreateRelayButtonAsServer();
        });
        RoomPlayerInfo.buttons["Login"].onClick.AddListener(() =>
        {
            instanceUserManager.myLogin();
        });
        RoomPlayerInfo.buttons["Logout"].onClick.AddListener(() =>
        {
            instanceUserManager.myLogout();
        });
        RoomPlayerInfo.buttons["Signup"].onClick.AddListener(() =>
        {
            instanceUserManager.mySignup();
        });
        RoomPlayerInfo.buttons["ShowRooms"].onClick.AddListener(() =>
        {
            instanceRoomManager.RunningRoomsGet();
        });
        RoomPlayerInfo.buttons["Save"].onClick.AddListener(() =>
        {
            SaveUserInformation.Instance.SaveUserData();
        });
        RoomPlayerInfo.buttons["MoveToGetNewChara"].onClick.AddListener(() =>
        {
            SceneManager.LoadScene("GetNewCharaScene");
        });
    }
}


