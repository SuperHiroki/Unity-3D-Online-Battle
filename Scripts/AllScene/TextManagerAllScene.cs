using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Specialized;
using System.Collections;

public class TextManagerAllScene : MonoBehaviour
{
    //シーン内に一個であることを保証する
    public static TextManagerAllScene Instance { get; private set; }

    //外部参照
    [SerializeField] private TMP_Text textPrefab;
    [SerializeField] private Transform textParent;
    [SerializeField] private Image backgroundImagePrefab;

    //右上のバツのクリックを管理する
    private static Dictionary<string, bool> isCrossClickedDict = new Dictionary<string, bool>();

    private void Awake()
    {
        //シーン内に一個であることを保証する
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            UnityEngine.Debug.Log("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC GameObject: " + gameObject.name + ", Instance ID: " + GetInstanceID());
            Destroy(gameObject);
            return;//これがないとなぜか破壊されているはずなのに下記が実行される
        }

        //テキストを生成する
        Instance.SetText();
    }

    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    private void SetText()
    {
        //テキストを生成する
        Sprite backgroundSprite = Resources.Load<Sprite>("Photo/noise");
        Sprite crossSprite = Resources.Load<Sprite>("Photo/cross");
        foreach (var key in GlobalDefine.TextsDefineDictAllScene.Keys)
        {
            //////////////////////////////////////////////////////////////////////////////////////
            //背景はtextParentの子
            Image image = Instantiate(backgroundImagePrefab, textParent.transform);
            RectTransform backgroundRectTransform = image.GetComponent<RectTransform>();
            backgroundRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            backgroundRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRectTransform.anchoredPosition = GlobalDefine.TextsDefineDictAllScene[key].startPos;
            backgroundRectTransform.sizeDelta = GlobalDefine.TextsDefineDictAllScene[key].sizeDelta;
            //背景を設定
            image.sprite = backgroundSprite;
            //影をつける
            var shadow = image.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(-3.5f, -3.5f);
            //////////////////////////////////////////////////////////////////////////////////////
            //テキストは背景の子
            TMP_Text instanceText = Instantiate(textPrefab, image.transform);
            instanceText.text = GlobalDefine.TextsDefineDictAllScene[key].text;
            instanceText.color = GlobalDefine.TextsDefineDictAllScene[key].color;
            instanceText.fontSize = GlobalDefine.TextsDefineDictAllScene[key].fontSize;
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
            //右上のバツ印の画像
            Image crossImage = Instantiate(backgroundImagePrefab, image.transform);
            RectTransform crossImageRectTransform = crossImage.GetComponent<RectTransform>();
            crossImageRectTransform.anchorMin = new Vector2(1.0f, 1.0f);
            crossImageRectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            crossImageRectTransform.sizeDelta = GlobalDefine.TextsDefineDictAllScene[key].crossImageSizeDelta;
            //背景を設定
            crossImage.sprite = crossSprite;
            //影をつける
            var crossImageShadow = crossImage.gameObject.AddComponent<Shadow>();
            crossImageShadow.effectColor = new Color(0, 0, 0, 0.5f);
            crossImageShadow.effectDistance = new Vector2(-3.5f, -3.5f);
            //イベントを設定する
            isCrossClickedDict[key] = false;
            var currentKey = key;
            crossImage.gameObject.AddComponent<Button>().onClick.AddListener(() => {
                isCrossClickedDict[currentKey] = true;
                UnityEngine.Debug.Log("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF crossImage clicked");
            });
            //////////////////////////////////////////////////////////////////////////////////////
            //テキストをグローバル変数に格納。
            RoomPlayerInfo.textsAllScene[key] = (image, crossImage, instanceText);
        }
        RoomPlayerInfo.textsAllScene["Alert"].keyText.text = "Some Alert Messages";
        RoomPlayerInfo.textsAllScene["SomeText"].keyText.text = "Some Text";
    }

    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    //キャンバスの子を全て破壊する
    private void DeleteCanvasChildren()
    {
        for (int i = textParent.childCount - 1; i >= 0; i--)
        {
            Destroy(textParent.GetChild(i).gameObject);
        }
    }

    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    //Alertは外部から呼びだされる
    public static void MakeAlertText(string alertMsg, string type = "Alert")
    {
        //RoomPlayerInfo.textsAllSceneはstaticだからゲームを通して破壊されない。破壊されないのは確認したが、シーンを移動して戻ってくると、下記でnullが返ってエラーが起きる。シーン遷移時にメモリの配置位置が変更になったか？
        if (RoomPlayerInfo.textsAllScene[type].keyText == null)
        {
            UnityEngine.Debug.Log("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB RoomPlayerInfo.textsAllScene not Exists");
            Instance.DeleteCanvasChildren();//もとのキャンバスの子供を破壊しないと無限にUIが増えてしまう。
            Instance.SetText();
        }
        RoomPlayerInfo.textsAllScene[type].keyText.text = alertMsg;
        MonoBehaviour mb = RoomPlayerInfo.textsAllScene[type].keyObj.GetComponent<MonoBehaviour>();
        mb.StartCoroutine(MoveAndReturn(RoomPlayerInfo.textsAllScene[type].keyObj, GlobalDefine.TextsDefineDictAllScene[type].startPos, GlobalDefine.TextsDefineDictAllScene[type].endPos, GlobalDefine.TextsDefineDictAllScene[type].moveTime, GlobalDefine.TextsDefineDictAllScene[type].stayTime, type));
    }

    static IEnumerator MoveAndReturn(Image objectToMove, Vector2 startPos, Vector2 endPos, float moveTime, float stayTime, string type)
    {
        float elapsedTime = 0;
        while (elapsedTime < moveTime)
        {
            float percentage = elapsedTime / moveTime;
            float easedPercentage = EaseOutQuad(percentage);
            objectToMove.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPos, endPos, easedPercentage);
            // Imageの透明度を調整
            var imageColor = objectToMove.color;
            imageColor.a = Mathf.Lerp(0, 1, easedPercentage);
            objectToMove.color = imageColor;
            //子であるテキストやcrossImageの透明度を調整
            foreach (var graphic in objectToMove.GetComponentsInChildren<Graphic>())
            {
                var color = graphic.color;
                color.a = Mathf.Lerp(0, 1, easedPercentage);
                graphic.color = color;
            }
            //時間進める
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objectToMove.GetComponent<RectTransform>().anchoredPosition = endPos;
        //画面内で一時停止する
        float passedTime = 0f;
        while (!isCrossClickedDict[type] && passedTime < stayTime)
        {
            passedTime += Time.deltaTime;
            yield return null;
        }
        isCrossClickedDict[type] = false;
        //画面外に移動する
        elapsedTime = 0;
        while (elapsedTime < moveTime)
        {
            float percentage = elapsedTime / moveTime;
            float easedPercentage = EaseOutQuad(percentage);
            objectToMove.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(endPos, startPos, easedPercentage);
            // Imageの透明度を調整
            var imageColor = objectToMove.color;
            imageColor.a = Mathf.Lerp(1, 0, easedPercentage);
            objectToMove.color = imageColor;
            //子であるテキストやcrossImageの透明度を調整
            foreach (var graphic in objectToMove.GetComponentsInChildren<Graphic>())
            {
                var color = graphic.color;
                color.a = Mathf.Lerp(0, 1, easedPercentage);
                graphic.color = color;
            }
            //時間進める
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objectToMove.GetComponent<RectTransform>().anchoredPosition = startPos;
    }

    private static float EaseOutQuad(float t)
    {
        return -t * (t - 2);
    }
    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
    //#############################################################################################################################################
}
