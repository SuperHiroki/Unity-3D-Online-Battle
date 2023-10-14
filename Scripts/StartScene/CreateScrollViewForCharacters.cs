using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
//using System.Security.Policy;
using System.Linq;

public class CreateScrollViewForCharacters : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;

    //グローバル変数から代入
    private CharacterData[] characterDataArrayForShow =>  UserDataManager.UserData.Characters;

    private Transform contentTransform;

    //シーンに一つしかないことを保証する
    public static CreateScrollViewForCharacters Instance { get; private set; }

    private void Awake()
    {
        //シーンに一つしかないことを保証する
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        contentTransform = transform.Find("Viewport/Content");
    }

    public void CreateScrollView()
    {
        DeleteScrollView();
        if (characterDataArrayForShow != null)
        {
            int charanumber = characterDataArrayForShow.Length;
            for (int i = 0; i < charanumber; i++)
            {
                //ラムダ式のためにForループ内で宣言する。
                string imagePath = "Photo/button_background";
                Vector2 desiredSize = new Vector2(110f, 30f);
                Image buttonImage;
                TextMeshProUGUI buttonText;
                Shadow buttonShadow;
                Color defaultColor = Color.white;
                Color hoverColor = Color.red;
                Vector3 defaultLocalScale = Vector3.one;
                //ボタンのインスタンスを生成
                GameObject buttonObj = Instantiate(buttonPrefab, contentTransform);
                //RectTransformを設定する
                RectTransform buttonRectTransform = buttonObj.GetComponent<RectTransform>();
                buttonRectTransform.sizeDelta = new Vector2(270, 100);
                //テキストに関する設定
                buttonText = buttonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                buttonText.text = GlobalDefine.CharaNamesList[characterDataArrayForShow[i].CharacterId] + "   " + characterDataArrayForShow[i].CharacterLevel.ToString() + " Level" + "\n" + "<size=60%>Awakening: " + characterDataArrayForShow[i].Awakening.ToString()  +"\n" + "Reliability: " + characterDataArrayForShow[i].Reliability.ToString() + "\n" + "Experience: " + characterDataArrayForShow[i].Experience.ToString() + " / " + GlobalDefine.CharaLevelUpExperienceList[characterDataArrayForShow[i].CharacterLevel - 1] + "</size>";
                buttonText.fontSize = 20;
                //ボタンを押して発火する関数を設定する(ラムダ式に注意)
                Button button = buttonObj.GetComponent<Button>();
                int characterId = characterDataArrayForShow[i].CharacterId;
                button.onClick.AddListener(() => OnButtonClicked(characterId));
                // 写真をロードしてボタンに設定
                buttonImage = buttonObj.GetComponent<Image>();
                Sprite sprite = Resources.Load<Sprite>(imagePath);
                if (sprite != null)
                {
                    buttonImage.sprite = sprite;
                }
                //影の設定
                buttonShadow = buttonObj.GetComponent<Shadow>() ?? buttonObj.AddComponent<Shadow>();
                buttonShadow.effectDistance = new Vector2(-3, -3);
                buttonShadow.enabled = true;
                // ボタンのPointerEnterとPointerExitイベントにリスナを追加
                EventTrigger eventTrigger = buttonObj.AddComponent<EventTrigger>();
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
    private void DeleteScrollView()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    //ボタンを押したときに発火する関数
    void OnButtonClicked(int characterId)
    {
        UserDataManager.SetCharacterIdInCharaScene(characterId);
        SceneManager.LoadScene("CharaScene");
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
