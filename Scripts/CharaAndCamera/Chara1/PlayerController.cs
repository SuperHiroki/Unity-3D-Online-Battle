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
    //�O���Q��
    public GameObject cameraPrefab;
    public NetworkObject attack0Part0;
    public NetworkObject attack1Part0;
    public NetworkObject attack2Part0;
    public NetworkObject attack3Part0;
    public NetworkObject attack4Part0;

    //�O���[�o���ϐ�
    private string playerName => UserDataManager.UserData.Username;
    private int playerLevel => UserDataManager.UserData.UserLevel;
    private int usingCharacterId => UserDataManager.CharacterId;
    private int usingCharacterLevel => UserDataManager.CharacterLevel;
    private ulong clientId => UserDataManager.ClientId;
    private ulong charaNetObjId => GameManager.MyCharacterNetworkObjectInfo?.CharacterNetworkObjectID ?? ulong.MaxValue;//�����͎Q�Ƃɂ��Ȃ��ƃ_���B�L�������X�|�[�����ꂽ����̓l�b�g���[�N�I�u�W�F�N�gID�������Ă��Ȃ�����B
    //����ǂݍ���
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

    //�O���N���X
    private NetworkObjectStatusShow networkObjectStatusShow;

    //���[�J���ϐ�
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
        //�X�s�[�h
        speed = normalSpeed * (1.0f + 0.1f * usingCharacterLevel);
        //�v���n�u�����X�g������
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
        //�R���|�[�l���g�̎擾
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if (IsServer)
        {
            //�X�e�[�^�X(�L�����Ȃ�)��\�������邽�߂�
            networkObjectStatusShow = NetworkObjectStatusShow.MyInstance;
        }
        //#######################################################
        //�h���b�O����̂��߂�Canvas��Rect���擾
        canvasRectTransform = CanvasForDragSingleton.Instance.CanvasRectTransform;
        //�h���b�O����̂��߂ɖ��摜���擾
        DragArrowImage = RoomPlayerInfo.imagesMainScene["DragArrow"];
    }

    //#####################################################################################################################
    //#####################################################################################################################
    //�J���������[�J���ɃX�|�[��������
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
        // �f�t�H���g�̃J�������폜
        Camera defaultCamera = Camera.main;
        AudioListener audioListener = defaultCamera.GetComponent<AudioListener>();
        if (audioListener != null)
        {
            Destroy(audioListener);
        }
        // �J�����̃C���X�^���X���Ɛݒ�
        GameObject cameraInstance = Instantiate(cameraPrefab, transform.position + new Vector3(0, 10, -19), Quaternion.Euler(0, 0, 0));
        cameraInstance.GetComponent<CameraController>().target = transform;
        newMainCamera = cameraInstance.GetComponent<Camera>();
        newMainCamera.depth = 10;
        //�O���[�o���ϐ��Ɋi�[����(BillBoard�Ŏg��)
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
        //���Rowner��������͋�����Ȃ�
        if (!IsOwner)
        {
            return;
        }

        //mainCamera���܂��ASetupCamera()�ɂ���ăX�|�[�����Ă��Ȃ��ꍇ
        if (newMainCamera == null)
        {
            return;
        }

        //����ł����瑀��s�ɂ���
        if (isDead == true)
        {
            return;
        }
        //#####################################################################################################################
        //#####################################################################################################################
        //���L�[�̃C���v�b�g���擾
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        //�h���b�O�ł̃C���v�b�g���擾
        // PC�̃}�E�X����
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
        // �X�}�z�̃^�b�`����
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
        //���������������߂�
        Vector3 forwardMovement = newMainCamera.transform.forward * vertical;
        Vector3 rightMovement = newMainCamera.transform.right * horizontal;
        Vector3 movement = (forwardMovement + rightMovement) * speed * Time.deltaTime;
        movement.y = 0;

        //�̂̌�����ύX����
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        //�n�ʂɐڂ��Ă���ꍇ
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            //velocity.y = 0; 
            if (Input.GetKeyDown(KeyCode.J) || isJump == true)
            {
                GameManager.SetIsJump(false);
                velocity.y = 0;//�������ɂ����΂��܂������B�܂�A��̈ʒu�ɓ����Ƃ��܂������Ȃ����R�́A�n�ʂ��痣���O�Ɏ���Update()�����Ă��܂��Avelocity.y = 0;�����s����邩��B
                velocity.y += jumpForce;
            }
        }

        //�d�͂���ɓ���
        velocity.y -= gravity * Time.deltaTime;
        //��C��R�����邩�疳����ɉ������Ȃ��B
        if (velocity.y < -50.0f)
        {
            velocity.y = -50.0f;
        }

        //��{�I�Ȉړ�
        if(Input.GetKey(KeyCode.K) || isDash)
        {
            movement *= dashMultiply;
        }
        controller.Move(movement + velocity * Time.deltaTime);
        //�Ȃ����Acontroller.Move(movement + velocity);�ɂ���āA�����ʒu���v���n�u�����ʒu�ɂȂ��Ă��܂��̂ŁA��񂾂������ŏ����ʒu��ړI�̈ʒu�Ɉړ�����B
        if (IsOwner && isOwnerForResetTransform)
        {
            GameManager.SetIsOwnerForResetTransform(false);
            this.transform.position = GameManager.GlobalVector3ForReset;
            this.transform.rotation = GameManager.GlobalQuaternionForReset;
        }

        //#####################################################################################################################
        //#####################################################################################################################
        //�f�o�b�O�̂��߂�
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
    //�U��
    [ServerRpc]
    void RequestThrowCubeServerRpc(AttackNetworkObjectInfo attackNetworkObjectInfo)
    {
        //�T�[�o�̃��[�J���ŃC���X�^���X������
        if (attackNetworkObjectInfo.AttackID == 0)
        {
            //�^�������ȍU��
            AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject.transform.localScale = Vector3.one * (1 + 0.1f * usingCharacterLevel);
            GameObject thrownCubeGameObject = Instantiate(AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject, transform.position + transform.forward * 2.4f + transform.up * 0.05f * usingCharacterLevel, Quaternion.identity);
            BulletController bulletController = NetSpawnCommon(thrownCubeGameObject, attackNetworkObjectInfo);
            bulletController.Launch(transform.forward, attackNetworkObjectInfo);
        }
        else if (attackNetworkObjectInfo.AttackID == 1)
        {
            //�E�ɋȂ���Ȃ�����ł���
            AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject.transform.localScale = Vector3.one * (1 + 0.1f * usingCharacterLevel);
            GameObject thrownCubeGameObject = Instantiate(AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject, transform.position + transform.forward * 2.4f + transform.up * 0.05f * usingCharacterLevel, Quaternion.identity);
            BulletController bulletController = NetSpawnCommon(thrownCubeGameObject, attackNetworkObjectInfo);
            bulletController.Launch2(transform.forward, attackNetworkObjectInfo);
        }
        else if(attackNetworkObjectInfo.AttackID == 2)
        {
            //���ɋȂ���Ȃ�����ł���
            AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject.transform.localScale = Vector3.one * (1 + 0.1f * usingCharacterLevel);
            GameObject thrownCubeGameObject = Instantiate(AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject, transform.position + transform.forward * 2.4f + transform.up * 0.05f * usingCharacterLevel, Quaternion.identity);
            BulletController bulletController = NetSpawnCommon(thrownCubeGameObject, attackNetworkObjectInfo);
            bulletController.Launch3(transform.forward, attackNetworkObjectInfo);
        }
        else if(attackNetworkObjectInfo.AttackID == 3)
        {
            //�m�C�Y����Ŕ��ł���
            AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject.transform.localScale = Vector3.one * (1 + 0.1f * usingCharacterLevel);
            GameObject thrownCubeGameObject = Instantiate(AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject, transform.position + transform.forward * 2.4f + transform.up * 0.05f * usingCharacterLevel, Quaternion.identity);
            BulletController bulletController = NetSpawnCommon(thrownCubeGameObject, attackNetworkObjectInfo);
            bulletController.Launch4(transform.forward, attackNetworkObjectInfo);
        }
        else if (attackNetworkObjectInfo.AttackID == 4)
        {
            //�g���l�[�h�B���������ʒu����LaunchNoise�Ŕ��˂���B
            AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject.transform.localScale = Vector3.zero;
            GameObject thrownCubeGameObject = Instantiate(AttackPrefabsList[attackNetworkObjectInfo.AttackID][0].gameObject, transform.position + transform.forward * 2.4f + new Vector3(0, 1.5f, 0), Quaternion.identity);
            BulletController bulletController = NetSpawnCommon(thrownCubeGameObject, attackNetworkObjectInfo);
            bulletController.LaunchNoise(transform.forward, attackNetworkObjectInfo);
        }
    }

    //�l�b�g���[�N��ɃX�|�[������i���ʕ����j
    private BulletController NetSpawnCommon(GameObject thrownCubeGameObject, AttackNetworkObjectInfo attackNetworkObjectInfo)
    {
        //�X�|�[������
        NetworkObject thrownCubeNetworkObject = thrownCubeGameObject.GetComponent<NetworkObject>();
        thrownCubeNetworkObject.Spawn();
        //�T�[�o�̃X�e�[�^�X���X�V���邽�߂�(�����NetwrokObjectId�����݂��邱�Ƃ͕ۏ؂���Ȃ��̂Ŕ񓯊��������s��)
        attackNetworkObjectInfo.AttackNetworkObjectID = thrownCubeNetworkObject.NetworkObjectId;
        GameManager.AddAttackNetworkObject(attackNetworkObjectInfo);
        //�e�ۂ��܂������ړ�����悤��
        return thrownCubeNetworkObject.GetComponent<BulletController>();
    }

    //#####################################################################################################################
    //#####################################################################################################################
    //�N���C�A���g���o��
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            //NetworkObjectID���擾
            charaNetbjectID = GetComponent<NetworkObject>().NetworkObjectId;
        }
    }

    //#########################################################################
    //�L������Despawn�̎��ɃT�[�o��Ń��X�g����폜����
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
            //���̃L�����̃X�e�[�^�X�\���̃L�����o�X���폜����
            CharacterNetworkObjectInfo foundCharacter = GameManager.CharacterNetworkObjectsList.Find(info => info.CharacterNetworkObjectID == charaNetbjectID);
            UnityEngine.Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAA CharacterNetworkObjectID you will remove from list.: " + foundCharacter.CharacterNetworkObjectID);
            networkObjectStatusShow.DeleteCharacterStatus(foundCharacter);
        }
    }

    //#####################################################################################################################
    //#####################################################################################################################
    //�h���b�O����ňړ�
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
            //���̉摜��\������
            UpdateArrow(startDragPosition, currentPosition);
        }
    }

    private void StopDragging()
    {
        GameManager.SetIsDragging(false);
        //������ʂɕ\�����Ȃ��B
        DeleteArrow();
    }

    //�h���b�O���삪�w��̏ꏊ����X�^�[�g���������m�F����
    private RectTransform canvasRectTransform;

    private bool IsWithinStartDragRange(Vector3 dragStartPosScreen)
    {
        //Canvas��̃��[�J�����W�ɕϊ�����
        Vector2 dragStartPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, dragStartPosScreen, null, out dragStartPos);
        //���肷��
        Vector2 dragAreaPos = GlobalDefine.ImagesDefineDictMainScene["DragBackground"].position;
        Vector2 dragAreaSize = GlobalDefine.ImagesDefineDictMainScene["DragBackground"].sizeDelta;
        Vector2 halfSize = dragAreaSize * 0.5f;
        Vector2 lowerLeft = dragAreaPos - halfSize;
        Vector2 upperRight = dragAreaPos + halfSize;
        return dragStartPos.x >= lowerLeft.x && dragStartPos.x <= upperRight.x && dragStartPos.y >= lowerLeft.y && dragStartPos.y <= upperRight.y;
    }

    private void UpdateArrow(Vector3 startPosition, Vector3 endPosition)
    {
        // ��󂪃h���b�O�̊J�n�n�_����n�܂�悤��pivot��ݒ�
        DragArrowImage.rectTransform.pivot = new Vector2(0, 0.5f);
        //�ʒu�Ɗp�x�𒲐�
        Vector3 direction = endPosition - startPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        DragArrowImage.transform.position = startPosition;
        DragArrowImage.transform.eulerAngles = new Vector3(0, 0, angle);
        // �T�C�Y�𒲐�
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
