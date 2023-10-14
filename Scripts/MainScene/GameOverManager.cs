using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    //プレハブの参照
    [SerializeField] private ButtonManagerMainScene buttonManagerMainScene;
    [SerializeField] private TextManagerMainScene textManagerMainScene;
    [SerializeField] private Canvas canvasGameOver;

    //シーンに配置されているものの参照
    [SerializeField] private Camera watchingCamera;

    private void Start()
    {
        CreateAndSetupBackgroundImage();
    }

    private void CreateAndSetupBackgroundImage()
    {
        // 新しいImage GameObjectを作成します
        GameObject imageGameObject = new GameObject("BackgroundImage");
        imageGameObject.transform.SetParent(canvasGameOver.transform, false);
        Image　background = imageGameObject.AddComponent<Image>();
        background.sprite = Resources.Load<Sprite>("Photo/noise");
        background.color = new Color(0, 0, 0, 0.7f);
        // サイズを設定します
        RectTransform rectTransform = imageGameObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);
        // 初期の表示設定
        canvasGameOver.sortingOrder = 100;
        canvasGameOver.gameObject.SetActive(false);
    }

    public void OnPlayerDeath()
    {
        //死亡キャンバスをアクティブにする
        GameManager.SetIsDead(true);
        canvasGameOver.gameObject.SetActive(true);
        //ボタン（ホームに戻る、観戦を続けるなど）を設定する
        buttonManagerMainScene.SetButtonBasicAndEvent(GlobalDefine.ButtonsDefineDictMainSceneIsDead, RoomPlayerInfo.buttonsMainSceneIsDead);
        textManagerMainScene.SetText(GlobalDefine.TextsDefineDictMainSceneIsDead, RoomPlayerInfo.textsMainSceneIsDead);
        //カメラを切り替える
        watchingCamera.depth = 20;
    }

    public void StartWatching()
    {
        canvasGameOver.gameObject.SetActive(false);
    }
}
