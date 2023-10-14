using System.Diagnostics;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;
using System.Collections;

[RequireComponent(typeof(ClientNetworkTransform))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //外部参照
    public GameObject cameraPrefab;
    public NetworkObject attack0Part0;
    public NetworkObject attack1Part0;
    public NetworkObject attack2Part0;
    public NetworkObject attack3Part0;
    public NetworkObject attack4Part0;

    //グローバル変数
    private string playerName => UserDataManager.UserData.Username;
    private int playerLevel => UserDataManager.UserData.UserLevel;
    private int usingCharacterId => UserDataManager.CharacterId;
    private int usingCharacterLevel => UserDataManager.CharacterLevel;
    private ulong clientId => UserDataManager.ClientId;
    private ulong charaNetObjId => GameManager.MyCharacterNetworkObjectInfo?.CharacterNetworkObjectID ?? ulong.MaxValue;//ここは参照にしないとダメ。キャラがスポーンされた直後はネットワークオブジェクトIDを持っていないから。
    //毎回読み込む
    private bool isDead => GameManager.IsDead;
    private bool isDash => GameManager.IsDash;
    private bool isJump => GameManager.IsJump;
    private bool isDragging => GameManager.IsDragging;
    private bool isOwnerForResetTransform => GameManager.IsOwnerForResetTransform;
    private bool isAttack1 => GameManager.IsAttack1;
    private bool isAttack2 => GameManager.IsAttack2;
    private bool isAttack3 => GameManager.IsAttack3;
    private bool isAttack4 => GameManager.IsAttack4;
    private bool isAttack5 => GameManager.IsAttack5;

    //外部クラス
    private NetworkObjectStatusShow networkObjectStatusShow;

    //ローカル変数
    private Camera newMainCamera;
    private float jumpForce = 13.0f;
    private float gravity = 9.81f;
    private float speed;
    private float normalSpeed = 9.0f;
    private float dashMultiply = 1.8f;
    private float rotationSpeed = 10f;
    private Vector3 velocity = Vector3.zero;
    private CharacterController controller;
    private Animator animator;
    private NetworkObject netObj;
    float rayLength = 2.4f;
    RaycastHit hit;
    private ulong charaNetbjectID;
    private List<List<NetworkObject>> AttackPrefabsList;
    private Vector3 startDragPosition;
    private float vertical;
    private float horizontal;
    private Image DragArrowImage;
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    void Awake()
    {
        //スピード
        speed = normalSpeed * (1.0f + 0.1f * usingCharacterLevel);
        //プレハブをリスト化する
        AttackPrefabsList = new List<List<NetworkObject>>
        {
                new List<NetworkObject> { attack0Part0 },
                new List<NetworkObject> { attack1Part0 },
                new List<NetworkObject> { attack2Part0 },
                new List<NetworkObject> { attack3Part0 },
                new List<NetworkObject> { attack4Part0 }
        };
    }
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    private void Start()
    {
        //コンポーネントの取得
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if (IsServer)
        {
            //ステータス(キル数など)を表示させるために
            networkObjectStatusShow = NetworkObjectStatusShow.MyInstance;
        }
        //#######################################################
        //ドラッグ操作のためにCanvasのRectを取得
        canvasRectTransform = CanvasForDragSingleton.Instance.CanvasRectTransform;
        //ドラッグ操作のために矢印画像を取得
        DragArrowImage = RoomPlayerInfo.imagesMainScene["DragArrow"];
    }

    //#####################################################################################################################
    //#####################################################################################################################
    //カメラをローカルにスポーンさせる
    private bool _isChangedOwnership = false;
    public bool IsChangedOwnership
    {
        get
        {
            return _isChangedOwnership;
        }
        set
        {
            _isChangedOwnership = value;
            if (_isChangedOwnership && IsOwner)
            {
                SetupCamera();
            }
        }
    }
    private void SetupCamera()
    {
        // デフォルトのカメラを削除
        Camera defaultCamera = Camera.main;
        AudioListener audioListener = defaultCamera.GetComponent<AudioListener>();
        if (audioListener != null)
        {
            Destroy(audioListener);
        }
        // カメラのインスタンス化と設定
        GameObject cameraInstance = Instantiate(cameraPrefab, transform.position + new Vector3(0, 10, -19), Quaternion.Euler(0, 0, 0));
        cameraInstance.GetComponent<CameraController>().target = transform;
        newMainCamera = cameraInstance.GetComponent<Camera>();
        newMainCamera.depth = 10;
        //グローバル変数に格納する(BillBoardで使う)
        GameManager.SetMainCamera(newMainCamera);
    }

    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    void Update()
    {
        //#####################################################################################################################
        //#####################################################################################################################
        //当然ownerしか操作は許されない
        if (!IsOwner)
        {
            return;
        }

        //mainCameraがまだ、SetupCamera()によってスポーンしていない場合
        if (newMainCamera == null)
        {
            return;
        }

        //死んでいたら操作不可にする
        if (isDead == true)
        {
            return;
        }
        //#####################################################################################################################
        //#####################################################################################################################
        //矢印キーのインプットを取得
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        //ドラッグでのインプットを取得
        // PCのマウス操作
        if (Input.GetMouseButtonDown(0) && IsWithinStartDragRange(Input.mousePosition))
        {
            StartDragging(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            PerformDragging(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
        // スマホのタッチ操作
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (IsWithinStartDragRange(Input.mousePosition))
                    {
                        StartDragging(touch.position);
                    }
                    break;
                case TouchPhase.Moved:
                    PerformDragging(touch.position);
                    break;
                case TouchPhase.Ended:
                    StopDragging();
                    break;
            }
        }
        //動かす距離を求める
        Vector3 forwardMovement = newMainCamera.transform.forward * vertical;
        Vector3 rightMovement = newMainCamera.transform.right * horizontal;
        Vector3 movement = (forwardMovement + rightMovement) * speed * Time.deltaTime;
        movement.y = 0;

        //体の向きを変更する
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        //地面に接している場合
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            //velocity.y = 0; 
            if (Input.GetKeyDown(KeyCode.J) || isJump == true)
            {
                GameManager.SetIsJump(false);
                velocity.y = 0;//こっちにおけばうまくいく。つまり、上の位置に入れるとうまくいかない理由は、地面から離れる前に次のUpdate()が来てしまい、velocity.y = 0;が実行されるから。
                velocity.y += jumpForce;
            }
        }

        //重力が常に働く
        velocity.y -= gravity * Time.deltaTime;
        //空気抵抗があるから無限大に加速しない。
        if (velocity.y < -50.0f)
        {
            velocity.y = -50.0f;
        }

        //基本的な移動
        if(Input.GetKey(KeyCode.K) || isDash)
        {
            movement *= dashMultiply;
        }
        controller.Move(movement + velocity * Time.deltaTime);
        //なぜか、controller.Move(movement + velocity);によって、初期位置がプレハブが持つ位置になってしまうので、一回だけここで初期位置を目的の位置に移動する。
        if (IsOwner && isOwnerForResetTransform)
        {
            GameManager.SetIsOwnerForResetTransform(false);
            this.transform.position = GameManager.GlobalVector3ForReset;
            this.transform.rotation = GameManager.GlobalQuaternionForReset;
        }

        //#####################################################################################################################
        //#####################################################################################################################
        //デバッグのために
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnityEngine.Debug.Log("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF Input.GetKeyDown(KeyCode.Q)");
        }
        UnityEngine.Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.red);

        //#####################################################################################################################
        //#####################################################################################################################
        //Attacks
        //Attack1
        if (Input.GetKeyDown(KeyCode.M) || isAttack1 == true)
        {
            GameManager.SetIsAttack1(false);
            AttackNetworkObjectInfo attackNetworkObjectInfo = new AttackNetworkObjectInfo(0, charaNetObjId, usingCharacterId, usingCharacterLevel, clientId, playerName, playerLevel);
            RequestThrowCubeServerRpc(attackNetworkObjectInfo);
        }

        //Attack2
        if (Input.GetKeyDown(KeyCode.N) || isAttack2 == true)
        {
            GameManager.SetIsAttack2(false);
            AttackNetworkObjectInfo attackNetworkObjectInfo = new AttackNetworkObjectInfo(1, charaNetObjId, usingCharacterId, usingCharacterLevel, clientId, playerName, playerLevel);
            RequestThrowCubeServerRpc(attackNetworkObjectInfo);
        }

        //Attack3
        if (Input.GetKeyDown(KeyCode.B) || isAttack3 == true)
        {
            GameManager.SetIsAttack3(false);
            AttackNetworkObjectInfo attackNetworkObjectInfo = new AttackNetworkObjectInfo(2, charaNetObjId, usingCharacterId, usingCharacterLevel, clientId, playerName, playerLevel);
            RequestThrowCubeServerRpc(attackNetworkObjectInfo);
        }

        //Attack4
        if (Input.GetKeyDown(KeyCode.V) || isAttack4 == true)
        {
            GameManager.SetIsAttack4(false);
            AttackNetworkObjectInfo attackNetworkObjectInfo = new AttackNetworkObjectInfo(3, charaNetObjId, usingCharacterId, usingCharacterLevel, clientId, playerName, playerLevel);
            RequestThrowCubeServerRpc(attackNetworkObjectInfo);
        }

        //Attack5
        if (Input.GetKeyDown(KeyCode.C) || isAttack5 == true)
        {
            GameManager.SetIsAttack5(false);
            AttackNetworkObjectInfo attackNetworkObjectInfo = new AttackNetworkObjectInfo(4, charaNetObjId, usingCharacterId, usingCharacterLevel, clientId, playerName, playerLevel);
            RequestThrowCubeServerRpc(attackNetworkObjectInfo);
        }

        //#####################################################################################################################
        //#####################################################################################################################
        //Animator
        //walking
        if (horizontal != 0 || vertical != 0)
        {
            animator.SetBool("walk_bool", true);
        }
        else
        {
            if (animator.GetBool("walk_bool") && animator.GetCurrentAnimatorStateInfo(0).IsName("walking") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.01)
            {
                animator.SetBool("walk_bool", false);
            }
        }
        //ball_throw
        if (Input.GetKeyDown(KeyCode.P))
        {
            animator.SetBool("ball_throw_bool", true);
        }
        else
        {
            if (animator.GetBool("ball_throw_bool") && animator.GetCurrentAnimatorStateInfo(1).IsName("ball_throw") && animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1.0)
            {
                animator.SetBool("ball_throw_bool", false);
            }
        }
    }

    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //#####################################################################################################################################################
    //攻撃
    [ServerRpc]
    void RequestThrowCubeServerRpc(AttackNetworkObjectInfo attackNetworkObjectInfo)
    {
        //サーバのローカルでインスタンス化する
        if (attackNetworkObjectInfo.AttackID == 0)
        {
            //真っ直ぐな攻撃
            AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject.transform.localScale = Vector3.one * (1 + 0.1f * usingCharacterLevel);
            GameObject thrownCubeGameObject = Instantiate(AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject, transform.position + transform.forward * 2.4f + transform.up * 0.05f * usingCharacterLevel, Quaternion.identity);
            BulletController bulletController = NetSpawnCommon(thrownCubeGameObject, attackNetworkObjectInfo);
            bulletController.Launch(transform.forward, attackNetworkObjectInfo);
        }
        else if (attackNetworkObjectInfo.AttackID == 1)
        {
            //右に曲がりながら飛んでいく
            AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject.transform.localScale = Vector3.one * (1 + 0.1f * usingCharacterLevel);
            GameObject thrownCubeGameObject = Instantiate(AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject, transform.position + transform.forward * 2.4f + transform.up * 0.05f * usingCharacterLevel, Quaternion.identity);
            BulletController bulletController = NetSpawnCommon(thrownCubeGameObject, attackNetworkObjectInfo);
            bulletController.Launch2(transform.forward, attackNetworkObjectInfo);
        }
        else if(attackNetworkObjectInfo.AttackID == 2)
        {
            //左に曲がりながら飛んでいく
            AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject.transform.localScale = Vector3.one * (1 + 0.1f * usingCharacterLevel);
            GameObject thrownCubeGameObject = Instantiate(AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject, transform.position + transform.forward * 2.4f + transform.up * 0.05f * usingCharacterLevel, Quaternion.identity);
            BulletController bulletController = NetSpawnCommon(thrownCubeGameObject, attackNetworkObjectInfo);
            bulletController.Launch3(transform.forward, attackNetworkObjectInfo);
        }
        else if(attackNetworkObjectInfo.AttackID == 3)
        {
            //ノイズありで飛んでいく
            AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject.transform.localScale = Vector3.one * (1 + 0.1f * usingCharacterLevel);
            GameObject thrownCubeGameObject = Instantiate(AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject, transform.position + transform.forward * 2.4f + transform.up * 0.05f * usingCharacterLevel, Quaternion.identity);
            BulletController bulletController = NetSpawnCommon(thrownCubeGameObject, attackNetworkObjectInfo);
            bulletController.Launch4(transform.forward, attackNetworkObjectInfo);
        }
        else if (attackNetworkObjectInfo.AttackID == 4)
        {
            //トルネード。少し高い位置からLaunchNoiseで発射する。
            AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject.transform.localScale = Vector3.zero;
            GameObject thrownCubeGameObject = Instantiate(AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject, transform.position + transform.forward * 2.4f + new Vector3(0, 1.5f, 0), Quaternion.identity);
            BulletController bulletController = NetSpawnCommon(thrownCubeGameObject, attackNetworkObjectInfo);
            bulletController.LaunchNoise(transform.forward, attackNetworkObjectInfo);
        }
    }

    //ネットワーク上にスポーンする（共通部分）
    private BulletController NetSpawnCommon(GameObject thrownCubeGameObject, AttackNetworkObjectInfo attackNetworkObjectInfo)
    {
        //スポーンする
        NetworkObject thrownCubeNetworkObject = thrownCubeGameObject.GetComponent<NetworkObject>();
        thrownCubeNetworkObject.Spawn();
        //サーバのステータスを更新するために(直後にNetwrokObjectIdが存在することは保証されないので非同期処理を行う)
        attackNetworkObjectInfo.AttackNetworkObjectID = thrownCubeNetworkObject.NetworkObjectId;
        GameManager.AddAttackNetworkObject(attackNetworkObjectInfo);
        //弾丸がまっすぐ移動するように
        return thrownCubeNetworkObject.GetComponent<BulletController>();
    }

    //#####################################################################################################################
    //#####################################################################################################################
    //クライアントが登場
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            //NetworkObjectIDを取得
            charaNetbjectID = GetComponent<NetworkObject>().NetworkObjectId;
        }
    }

    //#########################################################################
    //キャラのDespawnの時にサーバ上でリストから削除する
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsServer)
        {
            UnityEngine.Debug.Log("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB OnNetworkDespawn()");
            RemoveCharacterFromList();
        }
    }

    private void RemoveCharacterFromList()
    {
        if (IsServer)
        {
            UnityEngine.Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAA RemoveCharacterFromList()");
            //このキャラのステータス表示のキャンバスも削除する
            CharacterNetworkObjectInfo foundCharacter = GameManager.CharacterNetworkObjectsList.Find(info => info.CharacterNetworkObjectID == charaNetbjectID);
            UnityEngine.Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAA CharacterNetworkObjectID you will remove from list.: " + foundCharacter.CharacterNetworkObjectID);
            networkObjectStatusShow.DeleteCharacterStatus(foundCharacter);
        }
    }

    //#####################################################################################################################
    //#####################################################################################################################
    //ドラッグ操作で移動
    private void StartDragging(Vector3 startPosition)
    {
        GameManager.SetIsDragging(true);
        startDragPosition = startPosition;
    }

    private void PerformDragging(Vector3 currentPosition)
    {
        if (isDragging)
        {
            float referenceDistance = Screen.width * 0.1f;
            Vector3 movementNormalized = (currentPosition - startDragPosition) / referenceDistance;
            movementNormalized = Vector3.ClampMagnitude(movementNormalized, 1f);
            vertical = movementNormalized.y;
            horizontal = movementNormalized.x;
            //矢印の画像を表示する
            UpdateArrow(startDragPosition, currentPosition);
        }
    }

    private void StopDragging()
    {
        GameManager.SetIsDragging(false);
        //矢印を画面に表示しない。
        DeleteArrow();
    }

    //ドラッグ操作が指定の場所からスタートしたかを確認する
    private RectTransform canvasRectTransform;

    private bool IsWithinStartDragRange(Vector3 dragStartPosScreen)
    {
        //Canvas基準のローカル座標に変換する
        Vector2 dragStartPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, dragStartPosScreen, null, out dragStartPos);
        //判定する
        Vector2 dragAreaPos = GlobalDefine.ImagesDefineDictMainScene["DragBackground"].position;
        Vector2 dragAreaSize = GlobalDefine.ImagesDefineDictMainScene["DragBackground"].sizeDelta;
        Vector2 halfSize = dragAreaSize * 0.5f;
        Vector2 lowerLeft = dragAreaPos - halfSize;
        Vector2 upperRight = dragAreaPos + halfSize;
        return dragStartPos.x >= lowerLeft.x && dragStartPos.x <= upperRight.x && dragStartPos.y >= lowerLeft.y && dragStartPos.y <= upperRight.y;
    }

    private void UpdateArrow(Vector3 startPosition, Vector3 endPosition)
    {
        // 矢印がドラッグの開始地点から始まるようにpivotを設定
        DragArrowImage.rectTransform.pivot = new Vector2(0, 0.5f);
        //位置と角度を調整
        Vector3 direction = endPosition - startPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        DragArrowImage.transform.position = startPosition;
        DragArrowImage.transform.eulerAngles = new Vector3(0, 0, angle);
        // サイズを調整
        float distance = Vector3.Distance(startPosition, endPosition);
        float maxArrowSize = 100.0f;
        float adjustedDistance = Mathf.Min(distance, maxArrowSize);
        DragArrowImage.rectTransform.sizeDelta = new Vector2(adjustedDistance, DragArrowImage.rectTransform.sizeDelta.y);
    }

    private void DeleteArrow()
    {
        DragArrowImage.rectTransform.anchoredPosition = GlobalDefine.ImagesDefineDictMainScene["DragArrow"].position;
    }
    //#####################################################################################################################
    //#####################################################################################################################
}
