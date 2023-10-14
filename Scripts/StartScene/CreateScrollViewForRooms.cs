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
        //�ߋ���ScrollView���폜����
        DeleteScrollView();
        //ScrollView��\������
        if (roomDataArrayForShow != null)
        {
            //���݂̕����̌������\������
            int roomNumber = roomDataArrayForShow.Length;
            for (int i = 0; i < roomNumber; i++)
            {
                //�����_���̂��߂�For���[�v���Ő錾����B
                string imagePath = "Photo/button_background";
                Vector2 desiredSize = new Vector2(160f, 35f);
                Image buttonImage;
                TextMeshProUGUI buttonText;
                Shadow buttonShadow;
                Color defaultColor = Color.white;
                Color hoverColor = Color.red;
                Vector3 defaultLocalScale = Vector3.one;
                //�{�^���̃C���X�^���X�𐶐�����
                GameObject buttonObj = Instantiate(buttonPrefab, contentTransform);
                //�{�^���̃e�L�X�g��ݒ肷��
                buttonText = buttonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                buttonText.text = roomDataArrayForShow[i].RoomId + "   " + roomDataArrayForShow[i].Host;
                buttonText.fontSize = 12;
                // �ʐ^�����[�h���ă{�^���ɐݒ�
                buttonImage = buttonObj.GetComponent<Image>();
                Sprite sprite = Resources.Load<Sprite>(imagePath);
                if (sprite != null)
                {
                    buttonImage.sprite = sprite;
                }
                // �{�^���̃T�C�Y��ݒ�
                RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = desiredSize;
                //�e�̐ݒ�
                buttonShadow = buttonObj.GetComponent<Shadow>() ?? buttonObj.AddComponent<Shadow>();
                buttonShadow.effectDistance = new Vector2(-3, -3);
                buttonShadow.enabled = true;
                //�{�^�����������Ƃ��ɔ��΂���֐���ݒ�
                Button button = buttonObj.GetComponent<Button>();
                string roomId = roomDataArrayForShow[i].RoomId;
                button.onClick.AddListener(() => OnButtonClicked(roomId));
                // �{�^����PointerEnter��PointerExit�C�x���g�Ƀ��X�i��ǉ�
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

�@�@//�ߋ���ScrollView��j�󂷂�
    public void DeleteScrollView()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    //�N���b�N�Ŕ��΂���֐�
    void OnButtonClicked(string roomId)
    {
        UserDataManager.SetRoomId(roomId);
        relayManager.JoinRelayButton(roomId);
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



