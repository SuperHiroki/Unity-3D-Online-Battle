using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Specialized;
using System.Collections;

public class ImageManagerGetNewCharaCharaScene : MonoBehaviour
{
    public Canvas canvasImage;

    public void ShowCharaImage(int characterId)
    {
        //�������łɊi�[����Ă�����j�󂷂�
        foreach (var key in GlobalDefine.ImagesDefineDictGetNewCharaScene.Keys)
        {
            if (RoomPlayerInfo.imagesCharaScene[key] != null)
            {
                Destroy(RoomPlayerInfo.imagesCharaScene[key].gameObject);
                RoomPlayerInfo.imagesCharaScene[key] = null;
            }
        }

        //�\������
        foreach (var key in GlobalDefine.ImagesDefineDictGetNewCharaScene.Keys)
        {
            //�ϐ��̒�`
            string imagePath;
            if (GlobalDefine.ImagesDefineDictGetNewCharaScene[key].photoPath == "searchEachChara")
            {
                imagePath = "Photo/" + GlobalDefine.CharaNamesList[characterId];
            }
            else
            {
                imagePath = GlobalDefine.ImagesDefineDictGetNewCharaScene[key].photoPath;
            }
            Vector2 desiredSize = GlobalDefine.ImagesDefineDictGetNewCharaScene[key].sizeDelta;
            Color defaultColor = GlobalDefine.ImagesDefineDictGetNewCharaScene[key].color;
            Color hoverColor = Color.red;
            // �摜�̃��[�h�Ɛݒ�
            Sprite sprite = Resources.Load<Sprite>(imagePath);
            GameObject imageObj = new GameObject(key + "_Image");
            Image image = imageObj.AddComponent<Image>();
            image.sprite = sprite;
            // RectTransform�̐ݒ�
            RectTransform imageRectTransform = imageObj.GetComponent<RectTransform>();
            imageRectTransform.SetParent(canvasImage.transform);
            imageRectTransform.anchoredPosition = GlobalDefine.ImagesDefineDictGetNewCharaScene[key].position;
            imageRectTransform.sizeDelta = desiredSize;
            imageRectTransform.localScale = Vector3.one;
            // �e�̐ݒ�
            Shadow imageShadow = imageObj.AddComponent<Shadow>();
            imageShadow.effectDistance = new Vector2(-3, -3);
            // �O���[�o���Ȏ����Ɋi�[
            RoomPlayerInfo.imagesCharaScene[key] = image;
            //�A�j���[�V����������
            StartCoroutine(AnimateImageCoroutine(image, GlobalDefine.ImagesDefineDictGetNewCharaScene[key].position, 5.0f));
        }
    }

    private IEnumerator AnimateImageCoroutine(Image image, Vector2 targetAnchoredPosition, float duration)
    {
        float elapsedTime = 0f;
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        // �����ʒu�A�T�C�Y�A�����x�A�X�P�[����ۑ����܂��B
        Vector2 originalAnchoredPosition = new Vector2(100, 100);
        Vector3 originalScale = new Vector3(0.3f, 0.3f, 0.3f);
        Vector3 targetScale = new Vector3(1, 1, 1);
        Color imageColor = image.color;
        Color originalColor = new Color(imageColor.r, imageColor.g, imageColor.b, 0);
        float originalRotation = -5400f;
        float targetRotation = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(originalAnchoredPosition, targetAnchoredPosition, t);
            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            image.color = Color.Lerp(originalColor, new Color(originalColor.r, originalColor.g, originalColor.b, 1), t);
            //float currentRotation = Mathf.LerpAngle(originalRotation, targetRotation, t);
            float currentRotation = Mathf.Lerp(originalRotation, targetRotation, t);
            rectTransform.localEulerAngles = new Vector3(0, 0, currentRotation);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // �A�j���[�V�������I��������_�ōŏI�l���m���ɃZ�b�g���܂��B
        rectTransform.anchoredPosition = targetAnchoredPosition;
        rectTransform.localScale = targetScale;
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
        rectTransform.localEulerAngles = new Vector3(0, 0, targetRotation);
    }
}
