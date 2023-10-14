using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InputManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputPrefab;
    [SerializeField] private Transform parentTransformCanvas;

    private void Start()
    {
        foreach (var key in GlobalDefine.InputsDefineDict.Keys)
        {
            //input���ꂼ��A�g���摜���قȂ�\��������̂ŁAFor���[�v�̒��ɓ����
            Sprite inputBackgroundSprite = Resources.Load<Sprite>("Photo/input_background");
            //instanceInput�𐶐�����
            TMP_InputField instanceInput = Instantiate(inputPrefab, parentTransformCanvas);
            RectTransform inputRectTransform = instanceInput.GetComponent<RectTransform>();
            inputRectTransform.anchoredPosition = GlobalDefine.InputsDefineDict[key].position;
            inputRectTransform.sizeDelta = GlobalDefine.InputsDefineDict[key].sizeDelta;
            instanceInput.placeholder.GetComponent<TMP_Text>().text = GlobalDefine.InputsDefineDict[key].placeholder;
            // Set sprite
            Image bgImage = instanceInput.GetComponent<Image>();
            bgImage.sprite = inputBackgroundSprite;
            // Adjust TextArea RectTransform
            RectTransform textAreaRectTransform = instanceInput.transform.Find("Text Area").GetComponent<RectTransform>();
            textAreaRectTransform.anchorMin = new Vector2(0f, 0f);
            textAreaRectTransform.anchorMax = new Vector2(1f, 1f);
            textAreaRectTransform.pivot = new Vector2(0.5f, 0.5f);
            textAreaRectTransform.offsetMin = new Vector2(15f, 5f);
            textAreaRectTransform.offsetMax = new Vector2(-10f, -6.7f);
            // Add shadow
            Shadow thisShadow = instanceInput.gameObject.AddComponent<Shadow>();
            thisShadow.effectColor = new Color(0, 0, 0, 0.7f);
            thisShadow.effectDistance = new Vector2(-3.5f, -3.5f);
            // Event for focus, hover, input
            instanceInput.onValueChanged.AddListener((value) => OnInputChanged(instanceInput, value));
            EventTrigger eventTrigger = instanceInput.gameObject.AddComponent<EventTrigger>();
            //AddEvent(eventTrigger, EventTriggerType.PointerEnter, (data) => OnInputHover(instanceInput, true));
            //AddEvent(eventTrigger, EventTriggerType.PointerExit, (data) => OnInputHover(instanceInput, false));
            AddEvent(eventTrigger, EventTriggerType.Select, (data) => OnInputFocus(instanceInput, true));
            AddEvent(eventTrigger, EventTriggerType.Deselect, (data) => OnInputFocus(instanceInput, false));
            //�O���[�o���ϐ��Ɋi�[����B
            RoomPlayerInfo.inputFields[key] = instanceInput;
        }
    }

    //######################################################################
    //Hover�ȂǂŔ��΂���֐��̐ݒ�
    private void AddEvent(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }
    // Input���ύX���ꂽ���̓���������ɏ����܂��B
    private void OnInputChanged(TMP_InputField input, string value)
    {
        // ��: input.textComponent.color = Color.red;
    }
    // Hover�̎��̓���������ɏ����܂��B
    private void OnInputHover(TMP_InputField input, bool isHovering)
    {
        if (isHovering)
        {
            input.textComponent.color = Color.green;
        }
        else
        {
            input.textComponent.color = Color.black;
        }
    }
    // Focus���ꂽ���̓���������ɏ����܂��B
    private void OnInputFocus(TMP_InputField input, bool isFocused)
    {
        if (isFocused)
        {
            input.textComponent.color = Color.green;
        }
        else
        {
            input.textComponent.color = Color.black;
        }
    }
}




