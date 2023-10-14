using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Transform buttonParent;

    private void Awake()
    {
        foreach (var key in GlobalDefine.ButtonsDefineDict.Keys)
        {
            //�����_�����g���Ă���̂ŁAFor���[�v�̒��Ő錾����K�v������B
            string imagePath = GlobalDefine.ButtonsDefineDict[key].photoPath;
            Vector2 desiredSize = GlobalDefine.ButtonsDefineDict[key].size;
            Image buttonImage;
            TMP_Text buttonText;
            Shadow buttonShadow;
            Color defaultColor = Color.white;
            Color hoverColor = Color.red;
            Vector3 defaultLocalScale = Vector3.one;
            //�����_�����ϐ��̎Q�Ƃ��L���v�`�����邩�疾���I�ɐ錾����K�v������B
            var currentKey = key;
            //�{�^����Button�^�Ƃ��ăC���X�^���X��
            Button button = Instantiate(buttonPrefab, buttonParent);
            //�ʐ^�����[�h���ă{�^���ɐݒ�
            buttonImage = button.gameObject.GetComponent<Image>();
            Sprite sprite = Resources.Load<Sprite>(imagePath);
            if (sprite != null)
            {
                buttonImage.sprite = sprite;
            }
            //�{�^����RectTransform���w��
            RectTransform buttonRectTransform = button.gameObject.GetComponent<RectTransform>();
            buttonRectTransform.anchoredPosition = GlobalDefine.ButtonsDefineDict[currentKey].position;
            buttonRectTransform.sizeDelta = desiredSize;
            //�{�^���ɕ�����ݒ肷��
            buttonText = button.gameObject.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = GlobalDefine.ButtonsDefineDict[currentKey].label;
                buttonText.fontSize = GlobalDefine.ButtonsDefineDict[currentKey].textSize;
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
            RoomPlayerInfo.buttons[key] = button;
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

