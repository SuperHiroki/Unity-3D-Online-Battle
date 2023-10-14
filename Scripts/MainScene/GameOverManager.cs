using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    //�v���n�u�̎Q��
    [SerializeField] private ButtonManagerMainScene buttonManagerMainScene;
    [SerializeField] private TextManagerMainScene textManagerMainScene;
    [SerializeField] private Canvas canvasGameOver;

    //�V�[���ɔz�u����Ă�����̂̎Q��
    [SerializeField] private Camera watchingCamera;

    private void Start()
    {
        CreateAndSetupBackgroundImage();
    }

    private void CreateAndSetupBackgroundImage()
    {
        // �V����Image GameObject���쐬���܂�
        GameObject imageGameObject = new GameObject("BackgroundImage");
        imageGameObject.transform.SetParent(canvasGameOver.transform, false);
        Image�@background = imageGameObject.AddComponent<Image>();
        background.sprite = Resources.Load<Sprite>("Photo/noise");
        background.color = new Color(0, 0, 0, 0.7f);
        // �T�C�Y��ݒ肵�܂�
        RectTransform rectTransform = imageGameObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);
        // �����̕\���ݒ�
        canvasGameOver.sortingOrder = 100;
        canvasGameOver.gameObject.SetActive(false);
    }

    public void OnPlayerDeath()
    {
        //���S�L�����o�X���A�N�e�B�u�ɂ���
        GameManager.SetIsDead(true);
        canvasGameOver.gameObject.SetActive(true);
        //�{�^���i�z�[���ɖ߂�A�ϐ�𑱂���Ȃǁj��ݒ肷��
        buttonManagerMainScene.SetButtonBasicAndEvent(GlobalDefine.ButtonsDefineDictMainSceneIsDead, RoomPlayerInfo.buttonsMainSceneIsDead);
        textManagerMainScene.SetText(GlobalDefine.TextsDefineDictMainSceneIsDead, RoomPlayerInfo.textsMainSceneIsDead);
        //�J������؂�ւ���
        watchingCamera.depth = 20;
    }

    public void StartWatching()
    {
        canvasGameOver.gameObject.SetActive(false);
    }
}
