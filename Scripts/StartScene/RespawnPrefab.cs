using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Diagnostics;

public class RespawnPrefab : NetworkBehaviour
{
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //外部参照
    public NetworkObject Chara1;
    public NetworkObject Chara2;
    public NetworkObject Chara3;
    public NetworkObject Chara4;
    public NetworkObject Chara5;
    public NetworkObject Chara6;

    //グローバル変数
    private int characterId => UserDataManager.CharacterId;
    private int characterLevel => UserDataManager.CharacterLevel;
    private string playerName => UserDataManager.UserData.Username;
    private int playerLevel => UserDataManager.UserData.UserLevel;
    private int lifeStock => UserDataManager.LifeStock;

    //外部クラス参照
    private NetworkObjectStatusShow networkObjectStatusShow;

    private List<NetworkObject> characterPrefabs;

    //シーン上に一つしかないように
    public static RespawnPrefab Instance { get; private set; }

    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //クライアントがサーバにキャラをスポーンさせるように依頼
    //Awake
    private void Awake()
    {
        //シーン上に一つしかないように
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //そのほかの設定
        characterPrefabs = new List<NetworkObject> { Chara1, Chara2, Chara3, Chara4, Chara5, Chara6 };
        NetworkManager.Singleton.OnClientConnectedCallback += MyOnClientConnectedCallback;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= MyOnClientConnectedCallback;
        }
    }

    //ネットワーク接続を確立したら、サーバにキャラをスポーンさせるように依頼する
    private void MyOnClientConnectedCallback(ulong clientId)
    {
        UserDataManager.SetClientId(clientId);
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            //サーバに指令
            SendSelectedCharacterServerRpc(characterId, characterLevel, clientId, playerName, playerLevel);
        }
    }

    //クライアントからの依頼を受け取りキャラをネットワーク上にスポーンさせる
    private static int nextSpawnPosition = 0;
    private List<Vector3> spawnPositions = new List<Vector3>
    {
        new Vector3( 12, 6,  0), new Vector3(-12, 6,   0), new Vector3(  0, 6, 12), new Vector3(  0, 6, -12),
        new Vector3( 12, 6,  6), new Vector3(-12, 6,  -6), new Vector3( 12, 6, -6), new Vector3(-12, 6,   6),
        new Vector3(  6, 6, 12), new Vector3( -6, 6, -12), new Vector3( -6, 6, 12), new Vector3(  6, 6, -12)
    };

    [ServerRpc(RequireOwnership = false)]
    private void SendSelectedCharacterServerRpc(int characterId, int characterLevel, ulong clientId, string playerName, int playerLevel)
    {
        //スポーンの場所の調整
        Vector3 spawnPos = spawnPositions[nextSpawnPosition % spawnPositions.Count];
        nextSpawnPosition++;
        //NetworkObjectをスポーンさせる
        NetworkObject charaNetObj = Instantiate(characterPrefabs[characterId], spawnPos, Quaternion.identity);
        UnityEngine.Debug.Log("KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK charaNetObj.Spawn()");
        charaNetObj.Spawn();
        charaNetObj.ChangeOwnership(clientId);
        UnityEngine.Debug.Log("KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK charaNetObj.NetworkObjectId: " + charaNetObj.NetworkObjectId);
        //カメラをスポーンするようにクライアントに依頼する
        CharacterNetworkObjectInfo characterNetworkObjectInfo = new CharacterNetworkObjectInfo(charaNetObj.NetworkObjectId, characterId, characterLevel, clientId, playerName, playerLevel);
        SetIsChangedOwnershipClientRpc(characterNetworkObjectInfo, spawnPos, Quaternion.identity, lifeStock,  new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { clientId } } });
        //ネットワークオブジェクトを管理するため。キャラの頭上にステータスを表示するため
        networkObjectStatusShow = NetworkObjectStatusShow.MyInstance;
        networkObjectStatusShow.AddCharacterStatus(characterNetworkObjectInfo);
    }

    //確実にクライアントの所有になってからカメラをスポーンさせる
    [ClientRpc]
    private void SetIsChangedOwnershipClientRpc(CharacterNetworkObjectInfo characterNetworkObjectInfo, Vector3 position, Quaternion rotation, int lifeStock, ClientRpcParams clientRpcParams = default)
    {
        //ライフストックはサーバが決めるのでサーバからもらう
        UserDataManager.SetLifeStock(lifeStock);
        //ClientRpcの時点でサーバから送信するクライアントは決まっているので、ここは本当は必要ない
        if (NetworkManager.Singleton.LocalClientId == characterNetworkObjectInfo.OwnerClientID)
        {
            //netObj.Spawn();は非同期的な処理なので、完了しているか分からないので確認する。なので、ここは本来はwhileでtargetNetObjがtrueになるのを待つ必要がある。だけど、失敗したことないからやらない。
            NetworkObject targetNetObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterNetworkObjectInfo.CharacterNetworkObjectID];
            if (targetNetObj)
            {
                // 位置と回転を再設定するためにグローバル変数に保存しておく
                GameManager.SetIsOwnerForResetTransform(true);
                GameManager.SetGlobalVector3ForReset(position);
                GameManager.SetGlobalQuaternionForReset(rotation);
                //カメラをスポーンさせるトリガーとする。
                targetNetObj.GetComponent<PlayerController>().IsChangedOwnership = true;
                //グローバル変数に格納
                GameManager.UpdateMyCharacterNetworkObjectInfo(characterNetworkObjectInfo);
            }
        }
    }
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
}
