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
            //�ϐ��̒�`
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
            // �摜�̃��[�h�Ɛݒ�
            Sprite sprite = Resources.Load<Sprite>(imagePath);
            GameObject imageObj = new GameObject(key + "_Image");
            Image image = imageObj.AddComponent<Image>();
            image.sprite = sprite;
            // RectTransform�̐ݒ�
            RectTransform imageRectTransform = imageObj.GetComponent<RectTransform>();
            imageRectTransform.SetParent(canvasImage.transform);
            imageRectTransform.anchoredPosition = GlobalDefine.ImagesDefineDictCharaScene[key].position;
            imageRectTransform.sizeDelta = desiredSize;
            // �e�̐ݒ�
            Shadow imageShadow = imageObj.AddComponent<Shadow>();
            imageShadow.effectDistance = new Vector2(-3, -3);
            // �O���[�o���Ȏ����Ɋi�[
            RoomPlayerInfo.imagesCharaScene[key] = image;
        }
    }
}
