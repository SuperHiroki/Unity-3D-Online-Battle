using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Specialized;
using System.Collections;

public class TextManagerAllScene : MonoBehaviour
{
    //�V�[�����Ɉ�ł��邱�Ƃ�ۏ؂���
    public static TextManagerAllScene Instance { get; private set; }

    //�O���Q��
    [SerializeField] private TMP_Text textPrefab;
    [SerializeField] private Transform textParent;
    [SerializeField] private Image backgroundImagePrefab;

    //�E��̃o�c�̃N���b�N���Ǘ�����
    private static Dictionary<string, bool> isCrossClickedDict = new Dictionary<string, bool>();

    private void Awake()
    {
        //�V�[�����Ɉ�ł��邱�Ƃ�ۏ؂���
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            UnityEngine.Debug.Log("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC GameObject: " + gameObject.name + ", Instance ID: " + GetInstanceID());
            Destroy(gameObject);
            return;//���ꂪ�Ȃ��ƂȂ����j�󂳂�Ă���͂��Ȃ̂ɉ��L�����s�����
        }

        //�e�L�X�g�𐶐�����
        Instance.SetText();
    }

    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    private void SetText()
    {
        //�e�L�X�g�𐶐�����
        Sprite backgroundSprite = Resources.Load<Sprite>("Photo/noise");
        Sprite crossSprite = Resources.Load<Sprite>("Photo/cross");
        foreach (var key in GlobalDefine.TextsDefineDictAllScene.Keys)
        {
            //////////////////////////////////////////////////////////////////////////////////////
            //�w�i��textParent�̎q
            Image image = Instantiate(backgroundImagePrefab, textParent.transform);
            RectTransform backgroundRectTransform = image.GetComponent<RectTransform>();
            backgroundRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            backgroundRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRectTransform.anchoredPosition = GlobalDefine.TextsDefineDictAllScene[key].startPos;
            backgroundRectTransform.sizeDelta = GlobalDefine.TextsDefineDictAllScene[key].sizeDelta;
            //�w�i��ݒ�
            image.sprite = backgroundSprite;
            //�e������
            var shadow = image.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(-3.5f, -3.5f);
            //////////////////////////////////////////////////////////////////////////////////////
            //�e�L�X�g�͔w�i�̎q
            TMP_Text instanceText = Instantiate(textPrefab, image.transform);
            instanceText.text = GlobalDefine.TextsDefineDictAllScene[key].text;
            instanceText.color = GlobalDefine.TextsDefineDictAllScene[key].color;
            instanceText.fontSize = GlobalDefine.TextsDefineDictAllScene[key].fontSize;
            instanceText.fontStyle = FontStyles.Italic | FontStyles.Underline;
            instanceText.transform.localScale = Vector3.one;
            //�}�e���A�����N���[��
            Material clonedMaterial = new Material(instanceText.fontSharedMaterial);
            instanceText.fontMaterial = clonedMaterial;
            // Set outline color
            instanceText.fontMaterial.EnableKeyword("OUTLINE_ON");
            instanceText.fontMaterial.SetColor("_OutlineColor", Color.white);
            instanceText.fontMaterial.SetFloat("_OutlineWidth", 0.2f);
            //�e������B
            var tmpText = instanceText as TextMeshProUGUI;
            tmpText.enableVertexGradient = true;
            tmpText.fontMaterial.EnableKeyword("UNDERLAY_ON");
            tmpText.fontMaterial.SetColor("_UnderlayColor", new Color(0, 0, 0, 1.0f)); // �������̍��F
            tmpText.fontMaterial.SetFloat("_UnderlaySoftness", 0.1f); // ��炩���𒲐�
            tmpText.fontMaterial.SetFloat("_UnderlayOffsetX", -1.5f); // X�����̃I�t�Z�b�g
            tmpText.fontMaterial.SetFloat("_UnderlayOffsetY", -1.5f);
            //�ʒu�𒲐�
            RectTransform textRectTransform = instanceText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.one * 0.5f;  // Set the anchor to the center
            textRectTransform.anchorMax = Vector2.one * 0.5f;  // Set the anchor to the center
            //textRectTransform.anchorMin = Vector2.zero; 
            //textRectTransform.anchorMax = Vector2.one;
            textRectTransform.pivot = Vector2.one * 0.5f;
            textRectTransform.anchoredPosition = Vector2.zero;
            textRectTransform.sizeDelta = backgroundRectTransform.sizeDelta;
            //////////////////////////////////////////////////////////////////////////////////////
            //�E��̃o�c��̉摜
            Image crossImage = Instantiate(backgroundImagePrefab, image.transform);
            RectTransform crossImageRectTransform = crossImage.GetComponent<RectTransform>();
            crossImageRectTransform.anchorMin = new Vector2(1.0f, 1.0f);
            crossImageRectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            crossImageRectTransform.sizeDelta = GlobalDefine.TextsDefineDictAllScene[key].crossImageSizeDelta;
            //�w�i��ݒ�
            crossImage.sprite = crossSprite;
            //�e������
            var crossImageShadow = crossImage.gameObject.AddComponent<Shadow>();
            crossImageShadow.effectColor = new Color(0, 0, 0, 0.5f);
            crossImageShadow.effectDistance = new Vector2(-3.5f, -3.5f);
            //�C�x���g��ݒ肷��
            isCrossClickedDict[key] = false;
            var currentKey = key;
            crossImage.gameObject.AddComponent<Button>().onClick.AddListener(() => {
                isCrossClickedDict[currentKey] = true;
                UnityEngine.Debug.Log("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF crossImage clicked");
            });
            //////////////////////////////////////////////////////////////////////////////////////
            //�e�L�X�g���O���[�o���ϐ��Ɋi�[�B
            RoomPlayerInfo.textsAllScene[key] = (image, crossImage, instanceText);
        }
        RoomPlayerInfo.textsAllScene["Alert"].keyText.text = "Some Alert Messages";
        RoomPlayerInfo.textsAllScene["SomeText"].keyText.text = "Some Text";
    }

    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    //�L�����o�X�̎q��S�Ĕj�󂷂�
    private void DeleteCanvasChildren()
    {
        for (int i = textParent.childCount - 1; i >= 0; i--)
        {
            Destroy(textParent.GetChild(i).gameObject);
        }
    }

    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    //Alert�͊O������Ăт������
    public static void MakeAlertText(string alertMsg, string type = "Alert")
    {
        //RoomPlayerInfo.textsAllScene��static������Q�[����ʂ��Ĕj�󂳂�Ȃ��B�j�󂳂�Ȃ��̂͊m�F�������A�V�[�����ړ����Ė߂��Ă���ƁA���L��null���Ԃ��ăG���[���N����B�V�[���J�ڎ��Ƀ������̔z�u�ʒu���ύX�ɂȂ������H
        if (RoomPlayerInfo.textsAllScene[type].keyText == null)
        {
            UnityEngine.Debug.Log("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB RoomPlayerInfo.textsAllScene not Exists");
            Instance.DeleteCanvasChildren();//���Ƃ̃L�����o�X�̎q����j�󂵂Ȃ��Ɩ�����UI�������Ă��܂��B
            Instance.SetText();
        }
        RoomPlayerInfo.textsAllScene[type].keyText.text = alertMsg;
        MonoBehaviour mb = RoomPlayerInfo.textsAllScene[type].keyObj.GetComponent<MonoBehaviour>();
        mb.StartCoroutine(MoveAndReturn(RoomPlayerInfo.textsAllScene[type].keyObj, GlobalDefine.TextsDefineDictAllScene[type].startPos, GlobalDefine.TextsDefineDictAllScene[type].endPos, GlobalDefine.TextsDefineDictAllScene[type].moveTime, GlobalDefine.TextsDefineDictAllScene[type].stayTime, type));
    }

    static IEnumerator MoveAndReturn(Image objectToMove, Vector2 startPos, Vector2 endPos, float moveTime, float stayTime, string type)
    {
        float elapsedTime = 0;
        while (elapsedTime < moveTime)
        {
            float percentage = elapsedTime / moveTime;
            float easedPercentage = EaseOutQuad(percentage);
            objectToMove.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPos, endPos, easedPercentage);
            // Image�̓����x�𒲐�
            var imageColor = objectToMove.color;
            imageColor.a = Mathf.Lerp(0, 1, easedPercentage);
            objectToMove.color = imageColor;
            //�q�ł���e�L�X�g��crossImage�̓����x�𒲐�
            foreach (var graphic in objectToMove.GetComponentsInChildren<Graphic>())
            {
                var color = graphic.color;
                color.a = Mathf.Lerp(0, 1, easedPercentage);
                graphic.color = color;
            }
            //���Ԑi�߂�
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objectToMove.GetComponent<RectTransform>().anchoredPosition = endPos;
        //��ʓ��ňꎞ��~����
        float passedTime = 0f;
        while (!isCrossClickedDict[type] && passedTime < stayTime)
        {
            passedTime += Time.deltaTime;
            yield return null;
        }
        isCrossClickedDict[type] = false;
        //��ʊO�Ɉړ�����
        elapsedTime = 0;
        while (elapsedTime < moveTime)
        {
            float percentage = elapsedTime / moveTime;
            float easedPercentage = EaseOutQuad(percentage);
            objectToMove.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(endPos, startPos, easedPercentage);
            // Image�̓����x�𒲐�
            var imageColor = objectToMove.color;
            imageColor.a = Mathf.Lerp(1, 0, easedPercentage);
            objectToMove.color = imageColor;
            //�q�ł���e�L�X�g��crossImage�̓����x�𒲐�
            foreach (var graphic in objectToMove.GetComponentsInChildren<Graphic>())
            {
                var color = graphic.color;
                color.a = Mathf.Lerp(0, 1, easedPercentage);
                graphic.color = color;
            }
            //���Ԑi�߂�
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objectToMove.GetComponent<RectTransform>().anchoredPosition = startPos;
    }

    private static float EaseOutQuad(float t)
    {
        return -t * (t - 2);
    }
    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
}
