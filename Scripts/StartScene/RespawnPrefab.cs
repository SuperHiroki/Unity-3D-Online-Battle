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
    //�O���Q��
    public NetworkObject Chara1;
    public NetworkObject Chara2;
    public NetworkObject Chara3;
    public NetworkObject Chara4;
    public NetworkObject Chara5;
    public NetworkObject Chara6;

    //�O���[�o���ϐ�
    private int characterId => UserDataManager.CharacterId;
    private int characterLevel => UserDataManager.CharacterLevel;
    private string playerName => UserDataManager.UserData.Username;
    private int playerLevel => UserDataManager.UserData.UserLevel;
    private int lifeStock => UserDataManager.LifeStock;

    //�O���N���X�Q��
    private NetworkObjectStatusShow networkObjectStatusShow;

    private List<NetworkObject> characterPrefabs;

    //�V�[����Ɉ�����Ȃ��悤��
    public static RespawnPrefab Instance { get; private set; }

    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //�N���C�A���g���T�[�o�ɃL�������X�|�[��������悤�Ɉ˗�
    //Awake
    private void Awake()
    {
        //�V�[����Ɉ�����Ȃ��悤��
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //���̂ق��̐ݒ�
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

    //�l�b�g���[�N�ڑ����m��������A�T�[�o�ɃL�������X�|�[��������悤�Ɉ˗�����
    private void MyOnClientConnectedCallback(ulong clientId)
    {
        UserDataManager.SetClientId(clientId);
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            //�T�[�o�Ɏw��
            SendSelectedCharacterServerRpc(characterId, characterLevel, clientId, playerName, playerLevel);
        }
    }

    //�N���C�A���g����̈˗����󂯎��L�������l�b�g���[�N��ɃX�|�[��������
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
        //�X�|�[���̏ꏊ�̒���
        Vector3 spawnPos = spawnPositions[nextSpawnPosition % spawnPositions.Count];
        nextSpawnPosition++;
        //NetworkObject���X�|�[��������
        NetworkObject charaNetObj = Instantiate(characterPrefabs[characterId], spawnPos, Quaternion.identity);
        UnityEngine.Debug.Log("KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK charaNetObj.Spawn()");
        charaNetObj.Spawn();
        charaNetObj.ChangeOwnership(clientId);
        UnityEngine.Debug.Log("KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK charaNetObj.NetworkObjectId: " + charaNetObj.NetworkObjectId);
        //�J�������X�|�[������悤�ɃN���C�A���g�Ɉ˗�����
        CharacterNetworkObjectInfo characterNetworkObjectInfo = new CharacterNetworkObjectInfo(charaNetObj.NetworkObjectId, characterId, characterLevel, clientId, playerName, playerLevel);
        SetIsChangedOwnershipClientRpc(characterNetworkObjectInfo, spawnPos, Quaternion.identity, lifeStock,  new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { clientId } } });
        //�l�b�g���[�N�I�u�W�F�N�g���Ǘ����邽�߁B�L�����̓���ɃX�e�[�^�X��\�����邽��
        networkObjectStatusShow = NetworkObjectStatusShow.MyInstance;
        networkObjectStatusShow.AddCharacterStatus(characterNetworkObjectInfo);
    }

    //�m���ɃN���C�A���g�̏��L�ɂȂ��Ă���J�������X�|�[��������
    [ClientRpc]
    private void SetIsChangedOwnershipClientRpc(CharacterNetworkObjectInfo characterNetworkObjectInfo, Vector3 position, Quaternion rotation, int lifeStock, ClientRpcParams clientRpcParams = default)
    {
        //���C�t�X�g�b�N�̓T�[�o�����߂�̂ŃT�[�o������炤
        UserDataManager.SetLifeStock(lifeStock);
        //ClientRpc�̎��_�ŃT�[�o���瑗�M����N���C�A���g�͌��܂��Ă���̂ŁA�����͖{���͕K�v�Ȃ�
        if (NetworkManager.Singleton.LocalClientId == characterNetworkObjectInfo.OwnerClientID)
        {
            //netObj.Spawn();�͔񓯊��I�ȏ����Ȃ̂ŁA�������Ă��邩������Ȃ��̂Ŋm�F����B�Ȃ̂ŁA�����͖{����while��targetNetObj��true�ɂȂ�̂�҂K�v������B�����ǁA���s�������ƂȂ�������Ȃ��B
            NetworkObject targetNetObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterNetworkObjectInfo.CharacterNetworkObjectID];
            if (targetNetObj)
            {
                // �ʒu�Ɖ�]���Đݒ肷�邽�߂ɃO���[�o���ϐ��ɕۑ����Ă���
                GameManager.SetIsOwnerForResetTransform(true);
                GameManager.SetGlobalVector3ForReset(position);
                GameManager.SetGlobalQuaternionForReset(rotation);
                //�J�������X�|�[��������g���K�[�Ƃ���B
                targetNetObj.GetComponent<PlayerController>().IsChangedOwnership = true;
                //�O���[�o���ϐ��Ɋi�[
                GameManager.UpdateMyCharacterNetworkObjectInfo(characterNetworkObjectInfo);
            }
        }
    }
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
}
