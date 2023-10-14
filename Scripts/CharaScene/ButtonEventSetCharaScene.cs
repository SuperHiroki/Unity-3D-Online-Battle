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
    //�{�^���̖������ƗL����
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
    //�{�^���̃��C���C�x���g
    private void SetButtonActions()
    {
        //�z�[����ʂɖ߂�
        RoomPlayerInfo.buttonsCharaScene["BackToHome"].onClick.AddListener(() =>
        {
            /*
            // �V�[���̑S�ẴI�u�W�F�N�g��j��
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                Destroy(obj);
            }
            */
            // �V�[����؂�ւ�
            SceneManager.LoadScene("StartScene");
        });
    }
}
