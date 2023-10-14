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
    //�{�^���̖������ƗL����
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

    //�{�^�����ĂїL����
    private IEnumerator EnableButtonWithDelay(Button btn, float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableButton(btn);
    }

    private void EnableButton(Button btn)
    {
        btn.interactable = true;
    }

    //�{�^���𖳌���
    private void DisableButton(Button btn)
    {
        btn.interactable = false;
    }

    //#####################################################################################
    //#####################################################################################
    //�{�^���ɃA�N�V������ݒ�
    private void SetButtonActions()
    {
        RoomPlayerInfo.buttonsFirstScene["TapToStart"].onClick.AddListener(() =>
        {
            SceneManager.LoadScene("StartScene");
        });
    }
}
