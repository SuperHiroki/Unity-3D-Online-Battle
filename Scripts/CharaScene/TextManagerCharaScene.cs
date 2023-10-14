using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Specialized;
using System.Collections;

public class TextManagerCharaScene : MonoBehaviour
{
    [SerializeField] private TMP_Text textPrefab;
    [SerializeField] private Transform textParent;
    [SerializeField] private Image backgroundImagePrefab;

    private UserData userData => UserDataManager.UserData;
    private int characterIdInCharaScene => UserDataManager.CharacterIdInCharaScene;
    private CharacterData characterData;


    private void Awake()
    {
        //表示するキャラのデータを取得する
        characterData = userData.Characters[characterIdInCharaScene];

        //表示する
        Sprite backgroundSprite = Resources.Load<Sprite>("Photo/noise");
        foreach (var key in GlobalDefine.TextsDefineDictCharaScene.Keys)
        {
            //////////////////////////////////////////////////////////////////////////////////////
            //背景はtextParentの子
            Image imageObject = Instantiate(backgroundImagePrefab, textParent.transform);
            RectTransform backgroundRectTransform = imageObject.GetComponent<RectTransform>();
            backgroundRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            backgroundRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRectTransform.anchoredPosition = GlobalDefine.TextsDefineDictCharaScene[key].position;
            backgroundRectTransform.sizeDelta = GlobalDefine.TextsDefineDictCharaScene[key].sizeDelta;
            //背景を設定
            imageObject.sprite = backgroundSprite;
            // 既存の背景の色を取得
            Color currentColor = imageObject.color;
            currentColor.a = 0.25f;
            imageObject.color = currentColor;
            //影をつける
            var shadow = imageObject.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(-3.5f, -3.5f);
            //////////////////////////////////////////////////////////////////////////////////////
            //テキストは背景の子
            TMP_Text instanceText = Instantiate(textPrefab, imageObject.transform);
            instanceText.text = GlobalDefine.TextsDefineDictCharaScene[key].text;
            instanceText.color = GlobalDefine.TextsDefineDictCharaScene[key].color;
            instanceText.fontSize = GlobalDefine.TextsDefineDictCharaScene[key].fontSize;
            instanceText.fontStyle = FontStyles.Italic | FontStyles.Underline;
            instanceText.transform.localScale = Vector3.one;
            //マテリアルをクローン
            Material clonedMaterial = new Material(instanceText.fontSharedMaterial);
            instanceText.fontMaterial = clonedMaterial;
            // Set outline color
            instanceText.fontMaterial.EnableKeyword("OUTLINE_ON");
            instanceText.fontMaterial.SetColor("_OutlineColor", Color.white);
            instanceText.fontMaterial.SetFloat("_OutlineWidth", 0.2f);
            //影をつける。
            var tmpText = instanceText as TextMeshProUGUI;
            tmpText.enableVertexGradient = true;
            tmpText.fontMaterial.EnableKeyword("UNDERLAY_ON");
            tmpText.fontMaterial.SetColor("_UnderlayColor", new Color(0, 0, 0, 1.0f)); // 半透明の黒色
            tmpText.fontMaterial.SetFloat("_UnderlaySoftness", 0.1f); // 軟らかさを調整
            tmpText.fontMaterial.SetFloat("_UnderlayOffsetX", -1.5f); // X方向のオフセット
            tmpText.fontMaterial.SetFloat("_UnderlayOffsetY", -1.5f);
            //位置を調整
            RectTransform textRectTransform = instanceText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.one * 0.5f;  // Set the anchor to the center
            textRectTransform.anchorMax = Vector2.one * 0.5f;  // Set the anchor to the center
            //textRectTransform.anchorMin = Vector2.zero; 
            //textRectTransform.anchorMax = Vector2.one;
            textRectTransform.pivot = Vector2.one * 0.5f;
            textRectTransform.anchoredPosition = Vector2.zero;
            textRectTransform.sizeDelta = backgroundRectTransform.sizeDelta;
            //////////////////////////////////////////////////////////////////////////////////////
            //テキストをグローバル変数に格納。
            RoomPlayerInfo.textsCharaScene[key] = (imageObject, instanceText);
        }
        RoomPlayerInfo.textsCharaScene["CharaName"].keyText.text = "Chara Name: " + GlobalDefine.CharaNamesList[characterIdInCharaScene];
        RoomPlayerInfo.textsCharaScene["CharaLevel"].keyText.text = "Chara Level: " + characterData.CharacterLevel;
        RoomPlayerInfo.textsCharaScene["CharaAwakening"].keyText.text = "Awakening: " + characterData.Awakening;
        RoomPlayerInfo.textsCharaScene["CharaReliability"].keyText.text = "Reliability: " + characterData.Reliability;
        RoomPlayerInfo.textsCharaScene["CharaExperience"].keyText.text = "Experience: " + characterData.Experience + " / " + GlobalDefine.CharaLevelUpExperienceList[characterData.CharacterLevel-1];
    }
}
