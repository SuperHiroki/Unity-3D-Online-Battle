using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManagerFirstScene : MonoBehaviour
{
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Transform buttonParent;

    private void Awake()
    {
        foreach (var key in GlobalDefine.ButtonsDefineDictFirstScene.Keys)
        {
            //ラムダ式を使っているので、Forループの中で宣言する必要がある。
            string imagePath = GlobalDefine.ButtonsDefineDictFirstScene[key].photoPath;
            Vector2 desiredSize = GlobalDefine.ButtonsDefineDictFirstScene[key].size;
            Image buttonImage;
            TMP_Text buttonText;
            Shadow buttonShadow;
            Color defaultColor = Color.white;
            Color hoverColor = Color.red;
            Vector3 defaultLocalScale = Vector3.one;
            //ラムダ式が変数の参照をキャプチャするから明示的に宣言する必要がある。
            var currentKey = key;
            //ボタンをButton型としてインスタンス化
            Button button = Instantiate(buttonPrefab, buttonParent);
            //写真をロードしてボタンに設定
            buttonImage = button.gameObject.GetComponent<Image>();
            Sprite sprite = Resources.Load<Sprite>(imagePath);
            if (sprite != null)
            {
                buttonImage.sprite = sprite;
            }
            //ボタンのRectTransformを指定
            RectTransform buttonRectTransform = button.gameObject.GetComponent<RectTransform>();
            buttonRectTransform.anchoredPosition = GlobalDefine.ButtonsDefineDictFirstScene[currentKey].position;
            buttonRectTransform.sizeDelta = desiredSize;
            //ボタンに文字を設定する
            buttonText = button.gameObject.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = GlobalDefine.ButtonsDefineDictFirstScene[currentKey].label;
                buttonText.fontSize = GlobalDefine.ButtonsDefineDictFirstScene[currentKey].textSize;
            }
            //影の設定
            buttonShadow = button.GetComponent<Shadow>() ?? button.gameObject.AddComponent<Shadow>();
            buttonShadow.effectDistance = new Vector2(-3, -3);
            buttonShadow.enabled = true;
            // ボタンのPointerEnterとPointerExitイベントにリスナを追加
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
            //グローバルな辞書に格納する
            RoomPlayerInfo.buttonsFirstScene[key] = button;
        }
    }

    //hoverに関する関数
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
