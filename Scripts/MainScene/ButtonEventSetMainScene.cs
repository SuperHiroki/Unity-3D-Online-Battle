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


public class ButtonEventSetMainScene : MonoBehaviour
{
    [SerializeField] private GameOverManager gameOverManager;

    public void SetButtonEvent(Dictionary<string, ButtonDefine> ButtonsDefineDict, Dictionary<string, Button> buttons)
    {
        SetButtonDisable(ButtonsDefineDict, buttons);
        SetButtonActions(ButtonsDefineDict, buttons);
    }

    //#####################################################################################
    //#####################################################################################
    //�{�^���̖������ƗL����
    private void SetButtonDisable(Dictionary<string, ButtonDefine> ButtonsDefineDict, Dictionary<string, Button> buttons)
    {
        foreach (var pair in buttons)
        {
            var key = pair.Key;
            var button = pair.Value;
            Button currentButton = button;
            button.onClick.AddListener(() =>
            {
                DisableButton(currentButton);
                StartCoroutine(EnableButtonWithDelay(currentButton, ButtonsDefineDict[key].disableTime));
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
    private void SetButtonActions(Dictionary<string, ButtonDefine> ButtonsDefineDict, Dictionary<string, Button> buttons)
    {
        //######################################################################
        //######################################################################
        if(object.ReferenceEquals(ButtonsDefineDict, GlobalDefine.ButtonsDefineDictMainScene))
        {
            //�z�[����ʂɖ߂�
            buttons["BackToHome"].onClick.AddListener(() =>
            {
                StartCoroutine(ShutdownAndLoad());
            });
            // Dash�{�^��
            SetUpButtonActions(buttons["Dash"], MoveUpButtonPressed, MoveUpButtonReleased);
            // Jump�{�^��
            buttons["Jump"].onClick.AddListener(() =>
            {
                GameManager.SetIsJump(true);
            });
            //Attack1
            buttons["Attack1"].onClick.AddListener(() =>
            {
                GameManager.SetIsAttack1(true);
            });
            //Attack2
            buttons["Attack2"].onClick.AddListener(() =>
            {
                GameManager.SetIsAttack2(true);
            });
            //Attack1
            buttons["Attack3"].onClick.AddListener(() =>
            {
                GameManager.SetIsAttack3(true);
            });
            //Attack1
            buttons["Attack4"].onClick.AddListener(() =>
            {
                GameManager.SetIsAttack4(true);
            });
        }
        else if(object.ReferenceEquals(ButtonsDefineDict, GlobalDefine.ButtonsDefineDictMainSceneIsDead))
        {
            //�z�[����ʂɖ߂�
            buttons["GoBackToHome"].onClick.AddListener(() =>
            {
                StartCoroutine(ShutdownAndLoad());
            });
            //�ϐ킷��
            buttons["Watch"].onClick.AddListener(() =>
            {
                gameOverManager.StartWatching();
            });
        }
    }

    //�z�[����ʂɖ߂�֐�
    private IEnumerator ShutdownAndLoad()
    {
        NetworkManager.Singleton.Shutdown(true);
        yield return new WaitUntil(() => NetworkManager.Singleton.IsListening == false);
        /*
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            Destroy(obj);
        }
        */
        SceneManager.LoadScene("StartScene");
    }

    //�������{�^���ɃC�x���g��ݒ肷��B
    private void SetUpButtonActions(Button button, UnityAction onPressed, UnityAction onReleased)
    {
        EventTrigger eventTrigger = button.gameObject.AddComponent<EventTrigger>();
        //�������Ƃ�
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { onPressed(); });
        eventTrigger.triggers.Add(pointerDownEntry);
        //�������Ƃ�
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { onReleased(); });
        eventTrigger.triggers.Add(pointerUpEntry);
    }

    //�������{�^����bool�ϐ���ύX����
    // Dash�{�^��
    private void MoveUpButtonPressed() { GameManager.SetIsDash(true); }
    private void MoveUpButtonReleased() { GameManager.SetIsDash(false); }
}