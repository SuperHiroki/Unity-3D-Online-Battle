using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageManagerFirstScene : MonoBehaviour
{
    public Canvas canvasImage;

    private void Start()
    {
        foreach (var key in GlobalDefine.ImagesDefineDictFirstScene.Keys)
        {
            //変数の定義
            string imagePath = GlobalDefine.ImagesDefineDictFirstScene[key].photoPath;
            Vector2 desiredSize = GlobalDefine.ImagesDefineDictFirstScene[key].sizeDelta;
            Color defaultColor = GlobalDefine.ImagesDefineDictFirstScene[key].color;
            Color hoverColor = Color.red;
            // 画像のロードと設定
            Sprite sprite = Resources.Load<Sprite>(imagePath);
            GameObject imageObj = new GameObject(key + "_Image");
            Image image = imageObj.AddComponent<Image>();
            image.sprite = sprite;
            // RectTransformの設定
            RectTransform imageRectTransform = imageObj.GetComponent<RectTransform>();
            imageRectTransform.SetParent(canvasImage.transform, false);
            imageRectTransform.anchoredPosition = GlobalDefine.ImagesDefineDictFirstScene[key].position;
            imageRectTransform.sizeDelta = desiredSize;
            imageRectTransform.localScale = Vector3.one;
            // 影の設定
            Shadow imageShadow = imageObj.AddComponent<Shadow>();
            imageShadow.effectDistance = new Vector2(-3, -3);
            // グローバルな辞書に格納
            RoomPlayerInfo.imagesFirstScene[key] = image;
        }
    }
}
