using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Specialized;
using System.Collections;


public class TextManager : MonoBehaviour
{
    //外部参照
    [SerializeField] private TMP_Text textPrefab;
    [SerializeField] private Transform textParent;
    [SerializeField] private Image backgroundImagePrefab;

    //グローバル変数
    private UserData userData => UserDataManager.UserData;
    private string username => UserDataManager.UserData.Username;
    private int userLevel => UserDataManager.UserData.UserLevel;
    private int experience => UserDataManager.UserData.Experience;
    private int magicStone => UserDataManager.UserData.MagicStone;

    private void Awake()
    {
        Sprite backgroundSprite = Resources.Load<Sprite>("Photo/noise");

        foreach (var key in GlobalDefine.TextsDefineDict.Keys)
        {
            //////////////////////////////////////////////////////////////////////////////////////
            //背景はtextParentの子
            Image imageObject = Instantiate(backgroundImagePrefab, textParent.transform);
            RectTransform backgroundRectTransform = imageObject.GetComponent<RectTransform>();
            backgroundRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            backgroundRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRectTransform.anchoredPosition = GlobalDefine.TextsDefineDict[key].position;
            backgroundRectTransform.sizeDelta = GlobalDefine.TextsDefineDict[key].sizeDelta;
            //背景を設定
            imageObject.sprite = backgroundSprite;
            //影をつける
            var shadow = imageObject.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(-3.5f, -3.5f);
            //////////////////////////////////////////////////////////////////////////////////////
            //テキストは背景の子
            TMP_Text instanceText = Instantiate(textPrefab, imageObject.transform);
            instanceText.text = GlobalDefine.TextsDefineDict[key].text;
            instanceText.color = GlobalDefine.TextsDefineDict[key].color;
            instanceText.fontSize = GlobalDefine.TextsDefineDict[key].fontSize;
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
            RoomPlayerInfo.texts[key] = (imageObject, instanceText);
        }
        RoomPlayerInfo.texts["UserName"].keyText.text = username;
        RoomPlayerInfo.texts["UserLevel"].keyText.text = userLevel.ToString() + " Level";
        RoomPlayerInfo.texts["Experience"].keyText.text = "Experience: " + experience.ToString() + " / " + GlobalDefine.UserLevelUpExperienceList[userLevel-1];
        RoomPlayerInfo.texts["MagicStone"].keyText.text = "MagicStone: " + magicStone.ToString();
    }
}
