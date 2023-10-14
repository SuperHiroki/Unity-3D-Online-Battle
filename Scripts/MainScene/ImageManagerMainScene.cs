using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageManagerMainScene : MonoBehaviour
{
    public Canvas canvasImage;

    private void Awake()
    {
        foreach (var key in GlobalDefine.ImagesDefineDictMainScene.Keys)
        {
            //�ϐ��̒�`
            string imagePath = GlobalDefine.ImagesDefineDictMainScene[key].photoPath;
            Vector2 desiredSize = GlobalDefine.ImagesDefineDictMainScene[key].sizeDelta;
            Color defaultColor = GlobalDefine.ImagesDefineDictMainScene[key].color;
            Color hoverColor = Color.red;
            // �摜�̃��[�h�Ɛݒ�
            Sprite sprite = Resources.Load<Sprite>(imagePath);
            GameObject imageObj = new GameObject(key + "_Image");
            Image image = imageObj.AddComponent<Image>();
            image.sprite = sprite;
            // RectTransform�̐ݒ�
            RectTransform imageRectTransform = imageObj.GetComponent<RectTransform>();
            imageRectTransform.SetParent(canvasImage.transform); 
            imageRectTransform.anchoredPosition = GlobalDefine.ImagesDefineDictMainScene[key].position;
            imageRectTransform.sizeDelta = desiredSize;
            // �e�̐ݒ�
            Shadow imageShadow = imageObj.AddComponent<Shadow>();
            imageShadow.effectDistance = new Vector2(-3, -3);
            // �C�x���g�g���K�[�̐ݒ�
            EventTrigger eventTrigger = imageObj.AddComponent<EventTrigger>();
            AddEventTriggerEntry(eventTrigger, EventTriggerType.PointerEnter, (eventData) => {
                OnImagePointerEnter(image, hoverColor);
            });
            AddEventTriggerEntry(eventTrigger, EventTriggerType.PointerExit, (eventData) => {
                OnImagePointerExit(image, defaultColor, Vector3.one);
            });
            // �O���[�o���Ȏ����Ɋi�[
            RoomPlayerInfo.imagesMainScene[key] = image;
        }
    }

    //#######################################################################################
    // �C�x���g�g���K�[�̐ݒ�
    private void AddEventTriggerEntry(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((data) => { callback((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    private void OnImagePointerEnter(Image image, Color hoverColor)
    {
        image.color = hoverColor;
        image.transform.localScale *= 1.1f;
    }

    private void OnImagePointerExit(Image image, Color defaultColor, Vector3 defaultLocalScale)
    {
        image.color = defaultColor;
        image.transform.localScale = defaultLocalScale;
    }
}
