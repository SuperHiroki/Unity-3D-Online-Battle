using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Specialized;
using System.Collections;

public class TextManagerFirstScene : MonoBehaviour
{
    //�O���Q��
    [SerializeField] private TMP_Text textPrefab;
    [SerializeField] private Transform textParent;
    [SerializeField] private Image backgroundImagePrefab;

    private void Awake()
    {
        Sprite backgroundSprite = Resources.Load<Sprite>("Photo/noise");

        foreach (var key in GlobalDefine.TextsDefineDictFirstScene.Keys)
        {
            //////////////////////////////////////////////////////////////////////////////////////
            //�w�i��textParent�̎q
            Image imageObject = Instantiate(backgroundImagePrefab, textParent.transform);
            RectTransform backgroundRectTransform = imageObject.GetComponent<RectTransform>();
            backgroundRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            backgroundRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRectTransform.anchoredPosition = GlobalDefine.TextsDefineDictFirstScene[key].position;
            backgroundRectTransform.sizeDelta = GlobalDefine.TextsDefineDictFirstScene[key].sizeDelta;
            //�w�i��ݒ�
            imageObject.sprite = backgroundSprite;
            imageObject.color = new Color(imageObject.color.r, imageObject.color.g, imageObject.color.b, GlobalDefine.TextsDefineDictFirstScene[key].alpha);
            //�e������
            var shadow = imageObject.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(-3.5f, -3.5f);
            //////////////////////////////////////////////////////////////////////////////////////
            //�e�L�X�g�͔w�i�̎q
            TMP_Text instanceText = Instantiate(textPrefab, imageObject.transform);
            instanceText.text = GlobalDefine.TextsDefineDictFirstScene[key].text;
            instanceText.color = GlobalDefine.TextsDefineDictFirstScene[key].color;
            instanceText.fontSize = GlobalDefine.TextsDefineDictFirstScene[key].fontSize;
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
            //�e�L�X�g���O���[�o���ϐ��Ɋi�[�B
            RoomPlayerInfo.textsFirstScene[key] = (imageObject, instanceText);
        }
    }
}
