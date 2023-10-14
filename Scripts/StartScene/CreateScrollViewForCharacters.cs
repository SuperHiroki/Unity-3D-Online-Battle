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

    //�O���[�o���ϐ�������
    private CharacterData[] characterDataArrayForShow =>  UserDataManager.UserData.Characters;

    private Transform contentTransform;

    //�V�[���Ɉ�����Ȃ����Ƃ�ۏ؂���
    public static CreateScrollViewForCharacters Instance { get; private set; }

    private void Awake()
    {
        //�V�[���Ɉ�����Ȃ����Ƃ�ۏ؂���
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
                //�����_���̂��߂�For���[�v���Ő錾����B
                string imagePath = "Photo/button_background";
                Vector2 desiredSize = new Vector2(110f, 30f);
                Image buttonImage;
                TextMeshProUGUI buttonText;
                Shadow buttonShadow;
                Color defaultColor = Color.white;
                Color hoverColor = Color.red;
                Vector3 defaultLocalScale = Vector3.one;
                //�{�^���̃C���X�^���X�𐶐�
                GameObject buttonObj = Instantiate(buttonPrefab, contentTransform);
                //RectTransform��ݒ肷��
                RectTransform buttonRectTransform = buttonObj.GetComponent<RectTransform>();
                buttonRectTransform.sizeDelta = new Vector2(270, 100);
                //�e�L�X�g�Ɋւ���ݒ�
                buttonText = buttonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                buttonText.text = GlobalDefine.CharaNamesList[characterDataArrayForShow[i].CharacterId] + "   " + characterDataArrayForShow[i].CharacterLevel.ToString() + " Level" + "\n" + "<size=60%>Awakening: " + characterDataArrayForShow[i].Awakening.ToString()  +"\n" + "Reliability: " + characterDataArrayForShow[i].Reliability.ToString() + "\n" + "Experience: " + characterDataArrayForShow[i].Experience.ToString() + " / " + GlobalDefine.CharaLevelUpExperienceList[characterDataArrayForShow[i].CharacterLevel - 1] + "</size>";
                buttonText.fontSize = 20;
                //�{�^���������Ĕ��΂���֐���ݒ肷��(�����_���ɒ���)
                Button button = buttonObj.GetComponent<Button>();
                int characterId = characterDataArrayForShow[i].CharacterId;
                button.onClick.AddListener(() => OnButtonClicked(characterId));
                // �ʐ^�����[�h���ă{�^���ɐݒ�
                buttonImage = buttonObj.GetComponent<Image>();
                Sprite sprite = Resources.Load<Sprite>(imagePath);
                if (sprite != null)
                {
                    buttonImage.sprite = sprite;
                }
                //�e�̐ݒ�
                buttonShadow = buttonObj.GetComponent<Shadow>() ?? buttonObj.AddComponent<Shadow>();
                buttonShadow.effectDistance = new Vector2(-3, -3);
                buttonShadow.enabled = true;
                // �{�^����PointerEnter��PointerExit�C�x���g�Ƀ��X�i��ǉ�
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

    //�ߋ���ScrollView��j�󂷂�
    private void DeleteScrollView()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    //�{�^�����������Ƃ��ɔ��΂���֐�
    void OnButtonClicked(int characterId)
    {
        UserDataManager.SetCharacterIdInCharaScene(characterId);
        SceneManager.LoadScene("CharaScene");
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
