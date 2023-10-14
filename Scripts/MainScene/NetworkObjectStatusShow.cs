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
    //�����ݒ�
    //�Q�[�����Ɉ�̃C���X�^���X�����Ȃ����Ƃ��ۏ؂����
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

    //�v���n�u�̎Q�Ɛݒ�
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameOverManager gameOverManager;

    //�O���[�o���ϐ�
    private CharacterNetworkObjectInfo myCharacterNetworkObjectInfo => GameManager.MyCharacterNetworkObjectInfo;
    private int lifeStock => UserDataManager.LifeStock;

    //�l�b�g���[�N�I�u�W�F�N�g�ƃL�����o�X��Ή��t���鎫���f�[�^
    private Dictionary<ulong, GameObject> CharaNetObjIDStatusCanvasDict = new Dictionary<ulong, GameObject>();

    //##################################################################################################
    //�V�K�I�u�W�F�N�g�̖��O�ݒ�
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
    //�L�����̃f�[�^��ǉ�
    public void AddCharacterStatus(CharacterNetworkObjectInfo characterNetworkObjectInfo)
    {
        if (IsServer)
        {
            //�T�[�o�͊��S�ȏ������悤��
            GameManager.AddCharacterNetworkObject(characterNetworkObjectInfo);
            ShowAllCharacterStatus();
        }
    }

    //�S�L�����̃L�����̃f�[�^��\��
    public void ShowAllCharacterStatus()
    {
        if (IsServer)
        {
            //�N���C�A���g�ɑS�Ă𓯊�����
            CharacterNetworkObjectInfo[] characterNetworkObjectsArray = GameManager.CharacterNetworkObjectsList.ToArray();
            ShowAllCharaStatusClientRpc(characterNetworkObjectsArray);
        }
    }

    [ClientRpc]
    private void ShowAllCharaStatusClientRpc(CharacterNetworkObjectInfo[] charaNetworkObjectsArray, ClientRpcParams rpcParams = default)
    {
        if (IsClient && !IsServer)
        {
            // �ۂ��ƃR�s�[����
            GameManager.UpdateAllCharacterNetworkObjectsList(new List<CharacterNetworkObjectInfo>(charaNetworkObjectsArray));
        }
        //��������������S�Ă�status��canvas��ݒ肵�Ȃ���
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
        //�L������Canvas���m���Ƀl�b�g���[�N��ɃX�|�[�����Ă��邩�ǂ����̊m�F
        while (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(info.CharacterNetworkObjectID))
        {
            yield return null; // 1�t���[���ҋ@
        }
        //���łɂ��̃L������Canvas���q�Ɏ����Ă����ꍇ�A�����𒆎~����B
        if (CharaNetObjIDStatusCanvasDict.ContainsKey(info.CharacterNetworkObjectID))
        {
            yield break;
        }
        //##################################################################################################
        //canvas�����[�J����ɐ�������
        NetworkObject charaNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[info.CharacterNetworkObjectID];
        GameObject canvasObject = Instantiate(canvasPrefab, charaNetworkObject.transform.position + Vector3.up * 2.5f, Quaternion.identity);
        canvasObject.GetComponent<Billboard>().FollowCharacter(charaNetworkObject.gameObject);
        //�����ɃL�����o�X��o�^����
        CharaNetObjIDStatusCanvasDict[info.CharacterNetworkObjectID] = canvasObject;
        //canvas�Ȃǂ̐ݒ�̓��[�J���ł��ꂼ�ꂪ�s��
        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(4.0f, 2.25f);
        //##################################################################################################
        // �\������f�[�^���擾
        CharacterNetworkObjectInfo CharacterInfo = GameManager.CharacterNetworkObjectsList.Find(data => data.CharacterNetworkObjectID == info.CharacterNetworkObjectID);
        //##################################################################################################
        //HP�o�[�t��HP�o�[�t���[���ɂ͓����傫���Ɠ����T�C�Y��K�p
        Vector2 hpBarSize = new Vector2(3.4f, 0.99f);
        Vector2 hpBarPosition = new Vector2(0f, 0.252f);
        //�L���J�E���g�ȂǂɊւ��鋤�ʐݒ�
        float killCountKilledCountY = -0.2f;
        Vector2 iconStarCrossSize = new Vector2(0.6f, 0.6f);
        float killKilledCountFontSize = 0.4f;
        Vector2 killKilledCountSizeDelta = new Vector2(8.0f, 0.5f);
        //##################################################################################################
        // HP�o�[�t���[���̐����Ɛݒ�
        GameObject hpBarFrameObject = new GameObject(HPBarFrameObjectName);
        hpBarFrameObject.transform.SetParent(canvasObject.transform);
        hpBarFrameObject.transform.localPosition = new Vector3(0, 0, 0);
        Image hpBarFrame = hpBarFrameObject.AddComponent<Image>();
        Sprite hpBarFrameSprite = Resources.Load<Sprite>("Photo/FrameHPBar");
        hpBarFrame.sprite = hpBarFrameSprite;
        // �T�C�Y��ݒ肷��
        hpBarFrame.rectTransform.sizeDelta = hpBarSize;
        hpBarFrame.rectTransform.anchoredPosition = hpBarPosition;
        //##################################################################################################
        // HP�o�[�̐����Ɛݒ�
        GameObject hpBarObject = new GameObject(HPBarObjectName);
        hpBarObject.transform.SetParent(canvasObject.transform);
        hpBarObject.transform.localPosition = new Vector3(0, 0, 0);
        Image hpBar = hpBarObject.AddComponent<Image>();
        Sprite hpBarSprite = Resources.Load<Sprite>("Photo/HPBar");
        hpBar.sprite = hpBarSprite;
        hpBar.type = Image.Type.Filled;
        hpBar.fillMethod = Image.FillMethod.Horizontal;
        hpBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        // �T�C�Y��ݒ肷��
        hpBar.rectTransform.sizeDelta = hpBarSize;
        hpBar.rectTransform.anchoredPosition = hpBarPosition;
        // HP�o�[�̒�����ݒ�
        hpBar.fillAmount = (float)CharacterInfo.CharacterHP / 100f;
        //##################################################################################################
        // Star (Kill count) Icon�̐����Ɛݒ�
        GameObject starIconObject = new GameObject(starIconObjectName);
        starIconObject.transform.SetParent(canvasObject.transform);
        starIconObject.transform.localPosition = new Vector3(0, 0, 0);
        Image starIcon = starIconObject.AddComponent<Image>();
        Sprite starIconSprite = Resources.Load<Sprite>("Photo/StarKillCount");
        starIcon.sprite = starIconSprite;
        starIcon.rectTransform.sizeDelta = iconStarCrossSize;
        starIcon.rectTransform.anchoredPosition = new Vector2(-1.3f, killCountKilledCountY);
        //##################################################################################################
        // �L���J�E���g�̐����̕\��
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
        // Cross (Killed count) Icon�̐����Ɛݒ�
        GameObject crossIconObject = new GameObject(crossIconObjectName);
        crossIconObject.transform.SetParent(canvasObject.transform);
        crossIconObject.transform.localPosition = new Vector3(0, 0, 0);
        Image crossIcon = crossIconObject.AddComponent<Image>();
        Sprite crossIconSprite = Resources.Load<Sprite>("Photo/LifeHeart");
        crossIcon.sprite = crossIconSprite;
        crossIcon.rectTransform.sizeDelta = iconStarCrossSize;
        crossIcon.rectTransform.anchoredPosition = new Vector2(-0.1f, killCountKilledCountY);
        //##################################################################################################
        // ��L���J�E���g�̐����̕\���B�c�����C�t�ɕύX�B
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
        // Hp�̐����̕\��
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
        //���O�ƃL�����N�^�[���x���̕\��
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
    //�Ώۂ̃L�����ɑΉ�����Canvas��e�L�X�g�̒��g��ύX
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
            //���ꂼ��̃N���C�A���g�̃��X�g��ύX����
            GameManager.UpdateCharacterNetworkObjectsList(info);
            //##################################################################################################
            // �L�����o�X���擾����
            GameObject canvasObject = CharaNetObjIDStatusCanvasDict[info.CharacterNetworkObjectID];
            // �L���J�E���g�̃e�L�X�g�̕\����ύX����
            TMP_Text killCountText = canvasObject.transform.Find(killCountTextObjectName).GetComponent<TMP_Text>();
            killCountText.text = $"{info.CharacterKillCount}";
            // �c�����C�t�̃e�L�X�g�̕\����ύX����
            TMP_Text killedCountText = canvasObject.transform.Find(killedCountTextObjectName).GetComponent<TMP_Text>();
            killedCountText.text = $"{lifeStock - info.CharacterKilledCount}";
            // Hp�̃e�L�X�g�̕\����ύX����
            TMP_Text hpText = canvasObject.transform.Find(hpTextObjectName).GetComponent<TMP_Text>();
            hpText.text = $"{info.CharacterHP}/100";
            //HpBar�̒�����ύX����
            Image hpBar = canvasObject.transform.Find(HPBarObjectName).GetComponent<Image>();
            hpBar.fillAmount = (float)info.CharacterHP / 100f;
            //##################################################################################################
            //�X�e�[�^�X�ύX�������ɂ��Ă̏ꍇ�A�L�����Ɣ�L�����̃O���[�o��������ύX����(�g���Ă���L�����̃L���J�E���g���グ��)
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
                //�O��̎��_�ł̎����̃L�����Ɣ�L�����Y��Ȃ��悤�Ɋ��S�ɍX�V����  
                GameManager.UpdateMyCharacterNetworkObjectInfo(info);
            }
        }
    }
    
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //#############################################################################################################################
    //�ؒf�����ꍇ
    //�Ώۂ̃L�����ɑΉ�����Canvas���폜����
    public void DeleteCharacterStatus(CharacterNetworkObjectInfo info)
    {
        if (IsServer)
        {
            //���X�g����폜����
            GameManager.RemoveCharacterNetworkObject(info);
            //�N���C�A���g�S���ɂ���N���C�A���g���ؒf�������Ƃ�`����
            DeleteStatusClientRpc(info);
        }
    }

    [ClientRpc]
    private void DeleteStatusClientRpc(CharacterNetworkObjectInfo info, ClientRpcParams rpcParams = default)
    {
        if (IsClient)
        {
            //�Ώۂ̃L�����o�X���폜����
            GameObject canvasObject = CharaNetObjIDStatusCanvasDict[info.CharacterNetworkObjectID];
            Destroy(canvasObject);
        }
    }
    //#############################################################################
    //#############################################################################
    //�L������lifeStock�ɒB�����ꍇ(�L�����ɃA�^�b�`����Ă���X�N���v�g��Despawn�͌��ǌĂт������̂ŁA���X�g����̍폜�Ȃǂ��s���K�v�͂Ȃ�)
    public void KillCountEqualsLifeStock(CharacterNetworkObjectInfo info)
    {
        if (IsServer)
        {
            //�L�����Ŏ��񂾃N���C�A���g�ɒʒm����
            ThisClientDeadClientRpc(info, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { info.OwnerClientID } } });
            //�ؒf�ł͂Ȃ��A�L������Despawn������K�v������ꍇ�i�ؒf�Ȃ珟���Despawn���Ă����j
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
            //�폜�Ώۂ������ŁA�L������lifeStock�ɒB������ꍇ��z��i����̓L������lifeStock�ɒB�����Ƃ��̂ݗL���B�Ȃ��Ȃ�ؒf�̏ꍇ�͂����������̊֐������s����邱�Ƃ͂Ȃ��j�B��������ClientRpc�̎��_�ő��M����N���C�A���g�͍i���Ă���̂ŁA�C�ɂ���K�v�͂Ȃ��B
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