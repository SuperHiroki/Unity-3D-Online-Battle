using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Diagnostics;
using System.Collections.Specialized;

public class NetworkObjectStatusShow : NetworkBehaviour
{
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //初期設定
    //ゲーム中に一つのインスタンスしかないことが保証される
    public static NetworkObjectStatusShow MyInstance { get; private set; }
    private void Awake()
    {
        if (MyInstance == null)
        {
            MyInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //プレハブの参照設定
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameOverManager gameOverManager;

    //グローバル変数
    private CharacterNetworkObjectInfo myCharacterNetworkObjectInfo => GameManager.MyCharacterNetworkObjectInfo;
    private int lifeStock => UserDataManager.LifeStock;

    //ネットワークオブジェクトとキャンバスを対応付ける辞書データ
    private Dictionary<ulong, GameObject> CharaNetObjIDStatusCanvasDict = new Dictionary<ulong, GameObject>();

    //##################################################################################################
    //新規オブジェクトの名前設定
    string HPBarFrameObjectName = "HPBarFrameObject";
    string HPBarObjectName = "HPBarObject";
    string starIconObjectName = "starIconObject";
    string killCountTextObjectName = "KillCountTextObject";
    string hpTextObjectName = "hpTextObject";
    string crossIconObjectName = "crossIconObject";
    string killedCountTextObjectName = "KilledCountTextObject";
    string playerNameCharaLevelTextObjectName = "nameCharaLevelTextObject";

    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //キャラのデータを追加
    public void AddCharacterStatus(CharacterNetworkObjectInfo characterNetworkObjectInfo)
    {
        if (IsServer)
        {
            //サーバは完全な情報を持つように
            GameManager.AddCharacterNetworkObject(characterNetworkObjectInfo);
            ShowAllCharacterStatus();
        }
    }

    //全キャラのキャラのデータを表示
    public void ShowAllCharacterStatus()
    {
        if (IsServer)
        {
            //クライアントに全てを同期する
            CharacterNetworkObjectInfo[] characterNetworkObjectsArray = GameManager.CharacterNetworkObjectsList.ToArray();
            ShowAllCharaStatusClientRpc(characterNetworkObjectsArray);
        }
    }

    [ClientRpc]
    private void ShowAllCharaStatusClientRpc(CharacterNetworkObjectInfo[] charaNetworkObjectsArray, ClientRpcParams rpcParams = default)
    {
        if (IsClient && !IsServer)
        {
            // 丸ごとコピーする
            GameManager.UpdateAllCharacterNetworkObjectsList(new List<CharacterNetworkObjectInfo>(charaNetworkObjectsArray));
        }
        //少し非効率だが全てのstatusのcanvasを設定しなおす
        foreach (CharacterNetworkObjectInfo info in GameManager.CharacterNetworkObjectsList)
        {
            SetupCanvasText(info);
        }
    }

    private void SetupCanvasText(CharacterNetworkObjectInfo info)
    {
        StartCoroutine(SetupCanvasTextCoroutine(info));
    }

    private IEnumerator SetupCanvasTextCoroutine(CharacterNetworkObjectInfo info)
    {
        //キャラとCanvasが確実にネットワーク上にスポーンしているかどうかの確認
        while (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(info.CharacterNetworkObjectID))
        {
            yield return null; // 1フレーム待機
        }
        //すでにそのキャラがCanvasを子に持っていた場合、処理を中止する。
        if (CharaNetObjIDStatusCanvasDict.ContainsKey(info.CharacterNetworkObjectID))
        {
            yield break;
        }
        //##################################################################################################
        //canvasをローカル上に生成する
        NetworkObject charaNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[info.CharacterNetworkObjectID];
        GameObject canvasObject = Instantiate(canvasPrefab, charaNetworkObject.transform.position + Vector3.up * 2.5f, Quaternion.identity);
        canvasObject.GetComponent<Billboard>().FollowCharacter(charaNetworkObject.gameObject);
        //辞書にキャンバスを登録する
        CharaNetObjIDStatusCanvasDict[info.CharacterNetworkObjectID] = canvasObject;
        //canvasなどの設定はローカルでそれぞれが行う
        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(4.0f, 2.25f);
        //##################################################################################################
        // 表示するデータを取得
        CharacterNetworkObjectInfo CharacterInfo = GameManager.CharacterNetworkObjectsList.Find(data => data.CharacterNetworkObjectID == info.CharacterNetworkObjectID);
        //##################################################################################################
        //HPバーフとHPバーフレームには同じ大きさと同じサイズを適用
        Vector2 hpBarSize = new Vector2(3.4f, 0.99f);
        Vector2 hpBarPosition = new Vector2(0f, 0.252f);
        //キルカウントなどに関する共通設定
        float killCountKilledCountY = -0.2f;
        Vector2 iconStarCrossSize = new Vector2(0.6f, 0.6f);
        float killKilledCountFontSize = 0.4f;
        Vector2 killKilledCountSizeDelta = new Vector2(8.0f, 0.5f);
        //##################################################################################################
        // HPバーフレームの生成と設定
        GameObject hpBarFrameObject = new GameObject(HPBarFrameObjectName);
        hpBarFrameObject.transform.SetParent(canvasObject.transform);
        hpBarFrameObject.transform.localPosition = new Vector3(0, 0, 0);
        Image hpBarFrame = hpBarFrameObject.AddComponent<Image>();
        Sprite hpBarFrameSprite = Resources.Load<Sprite>("Photo/FrameHPBar");
        hpBarFrame.sprite = hpBarFrameSprite;
        // サイズを設定する
        hpBarFrame.rectTransform.sizeDelta = hpBarSize;
        hpBarFrame.rectTransform.anchoredPosition = hpBarPosition;
        //##################################################################################################
        // HPバーの生成と設定
        GameObject hpBarObject = new GameObject(HPBarObjectName);
        hpBarObject.transform.SetParent(canvasObject.transform);
        hpBarObject.transform.localPosition = new Vector3(0, 0, 0);
        Image hpBar = hpBarObject.AddComponent<Image>();
        Sprite hpBarSprite = Resources.Load<Sprite>("Photo/HPBar");
        hpBar.sprite = hpBarSprite;
        hpBar.type = Image.Type.Filled;
        hpBar.fillMethod = Image.FillMethod.Horizontal;
        hpBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        // サイズを設定する
        hpBar.rectTransform.sizeDelta = hpBarSize;
        hpBar.rectTransform.anchoredPosition = hpBarPosition;
        // HPバーの長さを設定
        hpBar.fillAmount = (float)CharacterInfo.CharacterHP / 100f;
        //##################################################################################################
        // Star (Kill count) Iconの生成と設定
        GameObject starIconObject = new GameObject(starIconObjectName);
        starIconObject.transform.SetParent(canvasObject.transform);
        starIconObject.transform.localPosition = new Vector3(0, 0, 0);
        Image starIcon = starIconObject.AddComponent<Image>();
        Sprite starIconSprite = Resources.Load<Sprite>("Photo/StarKillCount");
        starIcon.sprite = starIconSprite;
        starIcon.rectTransform.sizeDelta = iconStarCrossSize;
        starIcon.rectTransform.anchoredPosition = new Vector2(-1.3f, killCountKilledCountY);
        //##################################################################################################
        // キルカウントの数字の表示
        GameObject killCountTextObject = new GameObject(killCountTextObjectName);
        killCountTextObject.transform.SetParent(canvasObject.transform);
        killCountTextObject.transform.localPosition = new Vector3(0, 0, 0);
        TMP_Text killCountText = killCountTextObject.AddComponent<TextMeshProUGUI>();
        killCountText.text = $"{CharacterInfo.CharacterKillCount}";
        killCountText.rectTransform.anchoredPosition = new Vector2(-0.7f, killCountKilledCountY);
        killCountText.rectTransform.sizeDelta = killKilledCountSizeDelta;
        killCountText.fontSize = killKilledCountFontSize;
        killCountText.alignment = TextAlignmentOptions.Center;
        //##################################################################################################
        // Cross (Killed count) Iconの生成と設定
        GameObject crossIconObject = new GameObject(crossIconObjectName);
        crossIconObject.transform.SetParent(canvasObject.transform);
        crossIconObject.transform.localPosition = new Vector3(0, 0, 0);
        Image crossIcon = crossIconObject.AddComponent<Image>();
        Sprite crossIconSprite = Resources.Load<Sprite>("Photo/LifeHeart");
        crossIcon.sprite = crossIconSprite;
        crossIcon.rectTransform.sizeDelta = iconStarCrossSize;
        crossIcon.rectTransform.anchoredPosition = new Vector2(-0.1f, killCountKilledCountY);
        //##################################################################################################
        // 被キルカウントの数字の表示。残存ライフに変更。
        GameObject killedCountTextObject = new GameObject(killedCountTextObjectName);
        killedCountTextObject.transform.SetParent(canvasObject.transform);
        killedCountTextObject.transform.localPosition = new Vector3(0, 0, 0);
        TMP_Text killedCountText = killedCountTextObject.AddComponent<TextMeshProUGUI>();
        killedCountText.text = $"{lifeStock - CharacterInfo.CharacterKilledCount}";
        killedCountText.rectTransform.anchoredPosition = new Vector2(0.5f, killCountKilledCountY);
        killedCountText.rectTransform.sizeDelta = killKilledCountSizeDelta;
        killedCountText.fontSize = killKilledCountFontSize;
        killedCountText.alignment = TextAlignmentOptions.Center;
        //##################################################################################################
        // Hpの数字の表示
        GameObject hpTextObject = new GameObject(hpTextObjectName);
        hpTextObject.transform.SetParent(canvasObject.transform);
        hpTextObject.transform.localPosition = new Vector3(0, 0, 0);
        TMP_Text hpText = hpTextObject.AddComponent<TextMeshProUGUI>();
        hpText.text = $"{CharacterInfo.CharacterHP}/100";
        hpText.rectTransform.anchoredPosition = hpBarFrame.rectTransform.anchoredPosition + new Vector2(1.5f, -0.25f);
        hpText.rectTransform.sizeDelta = killKilledCountSizeDelta;
        hpText.fontSize = 0.3f;
        hpText.alignment = TextAlignmentOptions.Center;
        // Setting the color of the text to white
        hpText.color = Color.white;
        // Add a shadow
        Shadow shadow = hpTextObject.AddComponent<Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(-1f, -1f); 
        // Add an outline 
        Outline outline = hpTextObject.AddComponent<Outline>();
        outline.effectColor = Color.black; 
        outline.effectDistance = new Vector2(-1f, -1f);
        //##################################################################################################
        //名前とキャラクターレベルの表示
        GameObject playerNameCharaLevelTextObject = new GameObject(playerNameCharaLevelTextObjectName);
        playerNameCharaLevelTextObject.transform.SetParent(canvasObject.transform);
        playerNameCharaLevelTextObject.transform.localPosition = new Vector3(0, 0, 0);
        TMP_Text playerNameCharaLevelText = playerNameCharaLevelTextObject.AddComponent<TextMeshProUGUI>();
        playerNameCharaLevelText.text = $"{info.OwnerClientName} / {info.CharacterLevel} Lv";
        playerNameCharaLevelText.rectTransform.anchoredPosition = new Vector2(0.0f, 0.79f);
        playerNameCharaLevelText.rectTransform.sizeDelta = killKilledCountSizeDelta;
        playerNameCharaLevelText.fontSize = 0.45f;
        playerNameCharaLevelText.alignment = TextAlignmentOptions.Center;
    }

    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //対象のキャラに対応するCanvasやテキストの中身を変更
    public void ChangeCharacterStatus(CharacterNetworkObjectInfo info)
    {
        if (IsServer)
        {
            ChangeDisplayedStatusClientRpc(info);
        }
    }

    [ClientRpc]
    private void ChangeDisplayedStatusClientRpc(CharacterNetworkObjectInfo info, ClientRpcParams rpcParams = default)
    {
        if (IsClient)
        {
            //それぞれのクライアントのリストを変更する
            GameManager.UpdateCharacterNetworkObjectsList(info);
            //##################################################################################################
            // キャンバスを取得する
            GameObject canvasObject = CharaNetObjIDStatusCanvasDict[info.CharacterNetworkObjectID];
            // キルカウントのテキストの表示を変更する
            TMP_Text killCountText = canvasObject.transform.Find(killCountTextObjectName).GetComponent<TMP_Text>();
            killCountText.text = $"{info.CharacterKillCount}";
            // 残存ライフのテキストの表示を変更する
            TMP_Text killedCountText = canvasObject.transform.Find(killedCountTextObjectName).GetComponent<TMP_Text>();
            killedCountText.text = $"{lifeStock - info.CharacterKilledCount}";
            // Hpのテキストの表示を変更する
            TMP_Text hpText = canvasObject.transform.Find(hpTextObjectName).GetComponent<TMP_Text>();
            hpText.text = $"{info.CharacterHP}/100";
            //HpBarの長さを変更する
            Image hpBar = canvasObject.transform.Find(HPBarObjectName).GetComponent<Image>();
            hpBar.fillAmount = (float)info.CharacterHP / 100f;
            //##################################################################################################
            //ステータス変更が自分についての場合、キル数と被キル数のグローバル辞書を変更する(使っているキャラのキルカウントを上げる)
            CharacterNetworkObjectInfo tempMyCharacterNetworkObjectInfo = myCharacterNetworkObjectInfo;
            if (tempMyCharacterNetworkObjectInfo.CharacterNetworkObjectID == info.CharacterNetworkObjectID)
            {
                if (tempMyCharacterNetworkObjectInfo.CharacterKillCount < info.CharacterKillCount)
                {
                    UserDataManager.AddkillCountEachCharaDict(info.CharacterID);
                }
                else if(tempMyCharacterNetworkObjectInfo.CharacterKilledCount < info.CharacterKilledCount)
                {
                    UserDataManager.AddkilledCountEachCharaDict(info.CharacterID);
                }
                //前回の時点での自分のキル数と被キル数忘れないように完全に更新する  
                GameManager.UpdateMyCharacterNetworkObjectInfo(info);
            }
        }
    }
    
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //切断した場合
    //対象のキャラに対応するCanvasを削除する
    public void DeleteCharacterStatus(CharacterNetworkObjectInfo info)
    {
        if (IsServer)
        {
            //リストから削除する
            GameManager.RemoveCharacterNetworkObject(info);
            //クライアント全員にあるクライアントが切断したことを伝える
            DeleteStatusClientRpc(info);
        }
    }

    [ClientRpc]
    private void DeleteStatusClientRpc(CharacterNetworkObjectInfo info, ClientRpcParams rpcParams = default)
    {
        if (IsClient)
        {
            //対象のキャンバスを削除する
            GameObject canvasObject = CharaNetObjIDStatusCanvasDict[info.CharacterNetworkObjectID];
            Destroy(canvasObject);
        }
    }
    //#############################################################################
    //#############################################################################
    //キル数がlifeStockに達した場合(キャラにアタッチされているスクリプトでDespawnは結局呼びだされるので、リストからの削除などを行う必要はない)
    public void KillCountEqualsLifeStock(CharacterNetworkObjectInfo info)
    {
        if (IsServer)
        {
            //キル数で死んだクライアントに通知する
            ThisClientDeadClientRpc(info, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { info.OwnerClientID } } });
            //切断ではなく、キル数でDespawnさせる必要がある場合（切断なら勝手にDespawnしてくれる）
            NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[info.CharacterNetworkObjectID];
            if (networkObject != null)
            {
                networkObject.Despawn();
            }
        }
    }

    [ClientRpc]
    private void ThisClientDeadClientRpc(CharacterNetworkObjectInfo info, ClientRpcParams rpcParams = default)
    {
        if (IsClient)
        {
            //削除対象が自分で、キル数がlifeStockに達したら場合を想定（これはキル数がlifeStockに達したときのみ有効。なぜなら切断の場合はそもそもこの関数が実行されることはない）。そもそもClientRpcの時点で送信するクライアントは絞られているので、気にする必要はない。
            if (myCharacterNetworkObjectInfo.CharacterNetworkObjectID == info.CharacterNetworkObjectID)
            {
                gameOverManager.OnPlayerDeath();
            }
        }
    }
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
}