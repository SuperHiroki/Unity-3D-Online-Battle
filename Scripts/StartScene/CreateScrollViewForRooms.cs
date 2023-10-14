using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CreateScrollViewForRooms : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private RelayManager relayManager;

    private RoomInfo[] roomDataArrayForShow => RoomDataManager.RoomDataArrayForShow;

    private Transform contentTransform;

    void Awake()
    {
        contentTransform = transform.Find("Viewport/Content");
    }

    public void CreateScrollView()
    {
        //過去のScrollViewを削除する
        DeleteScrollView();
        //ScrollViewを表示する
        if (roomDataArrayForShow != null)
        {
            //現在の部屋の個数だけ表示する
            int roomNumber = roomDataArrayForShow.Length;
            for (int i = 0; i < roomNumber; i++)
            {
                //ラムダ式のためにForループ内で宣言する。
                string imagePath = "Photo/button_background";
                Vector2 desiredSize = new Vector2(160f, 35f);
                Image buttonImage;
                TextMeshProUGUI buttonText;
                Shadow buttonShadow;
                Color defaultColor = Color.white;
                Color hoverColor = Color.red;
                Vector3 defaultLocalScale = Vector3.one;
                //ボタンのインスタンスを生成する
                GameObject buttonObj = Instantiate(buttonPrefab, contentTransform);
                //ボタンのテキストを設定する
                buttonText = buttonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                buttonText.text = roomDataArrayForShow[i].RoomId + "   " + roomDataArrayForShow[i].Host;
                buttonText.fontSize = 12;
                // 写真をロードしてボタンに設定
                buttonImage = buttonObj.GetComponent<Image>();
                Sprite sprite = Resources.Load<Sprite>(imagePath);
                if (sprite != null)
                {
                    buttonImage.sprite = sprite;
                }
                // ボタンのサイズを設定
                RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = desiredSize;
                //影の設定
                buttonShadow = buttonObj.GetComponent<Shadow>() ?? buttonObj.AddComponent<Shadow>();
                buttonShadow.effectDistance = new Vector2(-3, -3);
                buttonShadow.enabled = true;
                //ボタンを押したときに発火する関数を設定
                Button button = buttonObj.GetComponent<Button>();
                string roomId = roomDataArrayForShow[i].RoomId;
                button.onClick.AddListener(() => OnButtonClicked(roomId));
                // ボタンのPointerEnterとPointerExitイベントにリスナを追加
                EventTrigger eventTrigger = buttonObj.gameObject.AddComponent<EventTrigger>();
                AddEventTriggerEntry(eventTrigger, EventTriggerType.PointerEnter, (eventData) => {
                    OnButtonPointerEnter(buttonImage, hoverColor);
                });
                AddEventTriggerEntry(eventTrigger, EventTriggerType.PointerExit, (eventData) => {
                    OnButtonPointerExit(buttonImage, defaultColor, defaultLocalScale);
                });
            }
        }
    }

　　//過去のScrollViewを破壊する
    public void DeleteScrollView()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    //クリックで発火する関数
    void OnButtonClicked(string roomId)
    {
        UserDataManager.SetRoomId(roomId);
        relayManager.JoinRelayButton(roomId);
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



