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
            //�ϐ��̒�`
            string imagePath = GlobalDefine.ImagesDefineDictFirstScene[key].photoPath;
            Vector2 desiredSize = GlobalDefine.ImagesDefineDictFirstScene[key].sizeDelta;
            Color defaultColor = GlobalDefine.ImagesDefineDictFirstScene[key].color;
            Color hoverColor = Color.red;
            // �摜�̃��[�h�Ɛݒ�
            Sprite sprite = Resources.Load<Sprite>(imagePath);
            GameObject imageObj = new GameObject(key + "_Image");
            Image image = imageObj.AddComponent<Image>();
            image.sprite = sprite;
            // RectTransform�̐ݒ�
            RectTransform imageRectTransform = imageObj.GetComponent<RectTransform>();
            imageRectTransform.SetParent(canvasImage.transform, false);
            imageRectTransform.anchoredPosition = GlobalDefine.ImagesDefineDictFirstScene[key].position;
            imageRectTransform.sizeDelta = desiredSize;
            imageRectTransform.localScale = Vector3.one;
            // �e�̐ݒ�
            Shadow imageShadow = imageObj.AddComponent<Shadow>();
            imageShadow.effectDistance = new Vector2(-3, -3);
            // �O���[�o���Ȏ����Ɋi�[
            RoomPlayerInfo.imagesFirstScene[key] = image;
        }
    }
}
