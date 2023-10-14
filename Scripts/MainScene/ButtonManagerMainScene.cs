using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManagerMainScene : MonoBehaviour
{
    //�v���n�u�̊O���Q��
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private Transform buttonParentGameOver;

    //�N���X�̊O���Q��
    [SerializeField] private ButtonEventSetMainScene buttonEventSetMainScene;

    private void Awake()
    {
        SetButtonBasicAndEvent(GlobalDefine.ButtonsDefineDictMainScene, RoomPlayerInfo.buttonsMainScene);
    }

    public void SetButtonBasicAndEvent(Dictionary<string, ButtonDefine> ButtonsDefineDict, Dictionary<string, Button> buttons)
    {
        SetButton(ButtonsDefineDict, buttons);
        buttonEventSetMainScene.SetButtonEvent(ButtonsDefineDict, buttons);
    }

    private void SetButton(Dictionary<string, ButtonDefine> ButtonsDefineDict, Dictionary<string, Button> buttons)
    {
        foreach (var key in ButtonsDefineDict.Keys)
        {
            //�����_�����g���Ă���̂ŁAFor���[�v�̒��Ő錾����K�v������B
            string imagePath = ButtonsDefineDict[key].photoPath;
            Vector2 desiredSize = ButtonsDefineDict[key].size;
            Image buttonImage;
            TMP_Text buttonText;
            Shadow buttonShadow;
            Color defaultColor = Color.white;
            Color hoverColor = Color.red;
            Vector3 defaultLocalScale = Vector3.one;
            //�����_�����ϐ��̎Q�Ƃ��L���v�`�����邩�疾���I�ɐ錾����K�v������B
            var currentKey = key;
            //�{�^����Button�^�Ƃ��ăC���X�^���X��
            Button button;
            if (object.ReferenceEquals(ButtonsDefineDict, GlobalDefine.ButtonsDefineDictMainScene))
            {
                button = Instantiate(buttonPrefab, buttonParent);
            }
            else if (object.ReferenceEquals(ButtonsDefineDict, GlobalDefine.ButtonsDefineDictMainSceneIsDead))
            {
                button = Instantiate(buttonPrefab, buttonParentGameOver);
            }
            else
            {
                //�{���͕K�v�Ȃ��񂾂��ǁA�R���p�C���G���[��h�����߂�
                button = Instantiate(buttonPrefab, buttonParent);
            }
            //�ʐ^�����[�h���ă{�^���ɐݒ�
            buttonImage = button.gameObject.GetComponent<Image>();
            Sprite sprite = Resources.Load<Sprite>(imagePath);
            if (sprite != null)
            {
                buttonImage.sprite = sprite;
            }
            //�{�^����RectTransform���w��
            RectTransform buttonRectTransform = button.gameObject.GetComponent<RectTransform>();
            buttonRectTransform.anchoredPosition = ButtonsDefineDict[currentKey].position;
            buttonRectTransform.sizeDelta = desiredSize;
            //�{�^���ɕ�����ݒ肷��
            buttonText = button.gameObject.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = ButtonsDefineDict[currentKey].label;
                buttonText.fontSize = ButtonsDefineDict[currentKey].textSize;
            }
            //�e�̐ݒ�
            buttonShadow = button.GetComponent<Shadow>() ?? button.gameObject.AddComponent<Shadow>();
            buttonShadow.effectDistance = new Vector2(-3, -3);
            buttonShadow.enabled = true;
            // �{�^����PointerEnter��PointerExit�C�x���g�Ƀ��X�i��ǉ�
            EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            }
            AddEventTriggerEntry(eventTrigger, EventTriggerType.PointerEnter, (eventData) => {
                OnButtonPointerEnter(buttonImage, hoverColor);
            });
            AddEventTriggerEntry(eventTrigger, EventTriggerType.PointerExit, (eventData) => {
                OnButtonPointerExit(buttonImage, defaultColor, defaultLocalScale);
            });
            //�O���[�o���Ȏ����Ɋi�[����
            buttons[key] = button;
        }
    }

    //hover�Ɋւ���֐�
    private void AddEventTriggerEntry(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((data) => { callback((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }
    private void OnButtonPointerEnter(Image buttonImage, Color hoverColor)
    {
        buttonImage.color = hoverColor;
        buttonImage.transform.localScale *= 1.1f;
    }
    private void OnButtonPointerExit(Image buttonImage, Color defaultColor, Vector3 defaultLocalScale)
    {
        buttonImage.color = defaultColor;
        buttonImage.transform.localScale = defaultLocalScale;
    }
}



