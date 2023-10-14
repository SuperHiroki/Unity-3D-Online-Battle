using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageManagerCharaScene : MonoBehaviour
{
    public Canvas canvasImage;

    private int characterIdInCharaScene => UserDataManager.CharacterIdInCharaScene;

    private void Start()
    {
        foreach (var key in GlobalDefine.ImagesDefineDictCharaScene.Keys)
        {
            //変数の定義
            string imagePath;
            if (GlobalDefine.ImagesDefineDictCharaScene[key].photoPath == "searchEachChara")
            {
                imagePath = "Photo/" + GlobalDefine.CharaNamesList[characterIdInCharaScene];
            }
            else
            {
                imagePath = GlobalDefine.ImagesDefineDictCharaScene[key].photoPath;
            }
            Vector2 desiredSize = GlobalDefine.ImagesDefineDictCharaScene[key].sizeDelta;
            Color defaultColor = GlobalDefine.ImagesDefineDictCharaScene[key].color;
            Color hoverColor = Color.red;
            // 画像のロードと設定
            Sprite sprite = Resources.Load<Sprite>(imagePath);
            GameObject imageObj = new GameObject(key + "_Image");
            Image image = imageObj.AddComponent<Image>();
            image.sprite = sprite;
            // RectTransformの設定
            RectTransform imageRectTransform = imageObj.GetComponent<RectTransform>();
            imageRectTransform.SetParent(canvasImage.transform);
            imageRectTransform.anchoredPosition = GlobalDefine.ImagesDefineDictCharaScene[key].position;
            imageRectTransform.sizeDelta = desiredSize;
            // 影の設定
            Shadow imageShadow = imageObj.AddComponent<Shadow>();
            imageShadow.effectDistance = new Vector2(-3, -3);
            // グローバルな辞書に格納
            RoomPlayerInfo.imagesCharaScene[key] = image;
        }
    }
}
