using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    //######################################################################################################################################
    //######################################################################################################################################
    //######################################################################################################################################
    //######################################################################################################################################
    //��{���
    private const Camera DefaultMainCamera = null;
    private const bool DefaultIsDead = false;
    //NetworkObject�̏����Ǘ�����
    private static List<CharacterNetworkObjectInfo> DefaultCharacterNetworkObjectsList = new List<CharacterNetworkObjectInfo>();
    private static List<AttackNetworkObjectInfo> DefaultAttackNetworkObjectsList = new List<AttackNetworkObjectInfo>();
    private static CharacterNetworkObjectInfo DefaultMyCharacterNetworkObjectInfo = null;
    //�o�O����
    private const bool DefaultIsOwnerForResetTransform = false;
    private static readonly Vector3 DefaultGlobalVector3ForReset = new Vector3(100, 100, 100);
    private static readonly Quaternion DefaultGlobalQuaternionForReset = new Quaternion(-100, -100, -100, 1);
    //�X�}�z�Ή�
    private const bool DefaultIsDragging = false;
    private const bool DefaultIsDash = false;
    private const bool DefaultIsJump = false;
    private const bool DefaultIsAttack1 = false;
    private const bool DefaultIsAttack2 = false;
    private const bool DefaultIsAttack3 = false;
    private const bool DefaultIsAttack4 = false;
    private const bool DefaultIsAttack5 = false;
    //################################################################################################
    //################################################################################################
    //�v���p�e�B�ƃv���C�x�[�g�ϐ��̒�`
    private static Camera _mainCamera = DefaultMainCamera;
    public static Camera MainCamera { get => _mainCamera; private set => _mainCamera = value; }
    private static bool _isDead = DefaultIsDead;
    public static bool IsDead { get => _isDead; private set => _isDead = value; }
    private static List<CharacterNetworkObjectInfo> _characterNetworkObjectsList = DefaultCharacterNetworkObjectsList.Select(item => item.Clone()).ToList();
    public static List<CharacterNetworkObjectInfo> CharacterNetworkObjectsList { get => _characterNetworkObjectsList.Select(item => item.Clone()).ToList(); private set => _characterNetworkObjectsList = value.Select(item => item.Clone()).ToList(); }
    private static List<AttackNetworkObjectInfo> _attackNetworkObjectsList = DefaultAttackNetworkObjectsList.Select(item => item.Clone()).ToList();
    public static List<AttackNetworkObjectInfo> AttackNetworkObjectsList { get => _attackNetworkObjectsList.Select(item => item.Clone()).ToList(); private set => _attackNetworkObjectsList = value.Select(item => item.Clone()).ToList(); }
    private static CharacterNetworkObjectInfo _myCharacterNetworkObjectInfo = DefaultMyCharacterNetworkObjectInfo?.Clone();
    public static CharacterNetworkObjectInfo MyCharacterNetworkObjectInfo { get => _myCharacterNetworkObjectInfo?.Clone(); private set => _myCharacterNetworkObjectInfo = value?.Clone(); }
    private static bool _isOwnerForResetTransform = DefaultIsOwnerForResetTransform;
    public static bool IsOwnerForResetTransform { get => _isOwnerForResetTransform; private set => _isOwnerForResetTransform = value; }
    private static Vector3 _globalVector3ForReset = DefaultGlobalVector3ForReset;
    public static Vector3 GlobalVector3ForReset { get => _globalVector3ForReset; private set => _globalVector3ForReset = value; }
    private static Quaternion _globalQuaternionForReset = DefaultGlobalQuaternionForReset;
    public static Quaternion GlobalQuaternionForReset { get => _globalQuaternionForReset; private set => _globalQuaternionForReset = value; }
    private static bool _isDragging = DefaultIsDragging;
    public static bool IsDragging { get => _isDragging; private set => _isDragging = value; }
    private static bool _isDash = DefaultIsDash;
    public static bool IsDash { get => _isDash; private set => _isDash = value; }
    private static bool _isJump = DefaultIsJump;
    public static bool IsJump { get => _isJump; private set => _isJump = value; }
    private static bool _isAttack1 = DefaultIsAttack1;
    public static bool IsAttack1 { get => _isAttack1; private set => _isAttack1 = value; }
    private static bool _isAttack2 = DefaultIsAttack2;
    public static bool IsAttack2 { get => _isAttack2; private set => _isAttack2 = value; }
    private static bool _isAttack3 = DefaultIsAttack3;
    public static bool IsAttack3 { get => _isAttack3; private set => _isAttack3 = value; }
    private static bool _isAttack4 = DefaultIsAttack4;
    public static bool IsAttack4 { get => _isAttack4; private set => _isAttack4 = value; }
    private static bool _isAttack5 = DefaultIsAttack5;
    public static bool IsAttack5 { get => _isAttack5; private set => _isAttack5 = value; }
    //################################################################################################
    //################################################################################################
    public static void ResetGameInfo()
    {
        MainCamera = DefaultMainCamera;
        IsDead = DefaultIsDead;
        CharacterNetworkObjectsList = DefaultCharacterNetworkObjectsList; // Deep copy in property set
        AttackNetworkObjectsList = DefaultAttackNetworkObjectsList; // Deep copy in property set
        MyCharacterNetworkObjectInfo = DefaultMyCharacterNetworkObjectInfo; // Deep copy in property set
        IsOwnerForResetTransform = DefaultIsOwnerForResetTransform;
        GlobalVector3ForReset = DefaultGlobalVector3ForReset;
        GlobalQuaternionForReset = DefaultGlobalQuaternionForReset;
        IsDragging = DefaultIsDragging;
        IsDash = DefaultIsDash;
        IsJump = DefaultIsJump;
        IsAttack1 = DefaultIsAttack1;
        IsAttack2 = DefaultIsAttack2;
        IsAttack3 = DefaultIsAttack3;
        IsAttack4 = DefaultIsAttack4;
        IsAttack5 = DefaultIsAttack5;
    }

    //######################################################################################################################################
    //######################################################################################################################################
    //######################################################################################################################################
    //######################################################################################################################################
    //################################################################################################
    //################################################################################################
    //�p�t�H�[�}���X�̂��߂Ƀv���p�e�B������ɒ��ڕύX��������
    //_attackNetworkObjectsList�̑���
    //�܂Ƃ߂čX�V
    public static void UpdateAllAttackNetworkObjectsList(List<AttackNetworkObjectInfo> updatedList)
    {
        AttackNetworkObjectsList = updatedList;
    }

    //1�v�f�����X�V
    public static void UpdateAttackNetworkObjectsList(AttackNetworkObjectInfo updatedInfo)
    {
        var targetObject = _attackNetworkObjectsList.FirstOrDefault(obj => obj.AttackNetworkObjectID == updatedInfo.AttackNetworkObjectID);
        if (targetObject != null)
        {
            int index = _attackNetworkObjectsList.IndexOf(targetObject);
            _attackNetworkObjectsList[index] = updatedInfo.Clone();
        }
        else
        {
            UnityEngine.Debug.Log("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE Error. There is no AttackNetworkObjectInfo you want to update.");
        }
    }

    //1�v�f���폜
    public static void RemoveAttackNetworkObject(AttackNetworkObjectInfo updatedInfo)
    {
        var targetObject = _attackNetworkObjectsList.FirstOrDefault(obj => obj.AttackNetworkObjectID == updatedInfo.AttackNetworkObjectID);
        if (targetObject != null)
        {
            _attackNetworkObjectsList.Remove(targetObject);
        }
        else
        {
            UnityEngine.Debug.Log("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE Error. There is no AttackNetworkObjectInfo you want to remove.");
        }
    }

    //1�v�f��ǉ�
    public static void AddAttackNetworkObject(AttackNetworkObjectInfo newInfo)
    {
        var existingObject = _attackNetworkObjectsList.FirstOrDefault(obj => obj.AttackNetworkObjectID == newInfo.AttackNetworkObjectID);
        if (existingObject == null)
        {
            _attackNetworkObjectsList.Add(newInfo.Clone());
        }
        else
        {
            UnityEngine.Debug.Log("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE Error. AttackNetworkObjectInfo you want to add already exists.");
        }
    }

    //##########################################################
    //##########################################################
    //_characterNetworkObjectsList�̑���
    //�܂Ƃ߂čX�V
    public static void UpdateAllCharacterNetworkObjectsList(List<CharacterNetworkObjectInfo> updatedList)
    {
        CharacterNetworkObjectsList = updatedList;
    }

    //1�v�f�����X�V
    public static void UpdateCharacterNetworkObjectsList(CharacterNetworkObjectInfo updatedInfo)
    {
        var targetObject = _characterNetworkObjectsList.FirstOrDefault(obj => obj.CharacterNetworkObjectID == updatedInfo.CharacterNetworkObjectID);
        if (targetObject != null)
        {
            int index = _characterNetworkObjectsList.IndexOf(targetObject);
            _characterNetworkObjectsList[index] = updatedInfo.Clone();
        }
        else
        {
            UnityEngine.Debug.Log("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE Error. There is no CharacterNetworkObjectInfo you want to update.");
        }
    }

    //1�v�f���폜
    public static void RemoveCharacterNetworkObject(CharacterNetworkObjectInfo updatedInfo)
    {
        var targetObject = _characterNetworkObjectsList.FirstOrDefault(obj => obj.CharacterNetworkObjectID == updatedInfo.CharacterNetworkObjectID);
        if (targetObject != null)
        {
            _characterNetworkObjectsList.Remove(targetObject);
        }
        else
        {
            UnityEngine.Debug.Log("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE Error. There is no CharacterNetworkObjectInfo you want to remove.");
        }
    }

    //1�v�f��ǉ�
    public static void AddCharacterNetworkObject(CharacterNetworkObjectInfo newInfo)
    {
        var existingObject = _characterNetworkObjectsList.FirstOrDefault(obj => obj.CharacterNetworkObjectID == newInfo.CharacterNetworkObjectID);
        if (existingObject == null)
        {
            _characterNetworkObjectsList.Add(newInfo.Clone());
        }
        else
        {
            UnityEngine.Debug.Log("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE Error. CharacterNetworkObjectInfo you want to add already exists.");
        }
    }

    //################################################################################################
    //################################################################################################
    //CharacterNetworkObjectInfo�̑���
    //�X�V
    public static void UpdateMyCharacterNetworkObjectInfo(CharacterNetworkObjectInfo newInfo)
    {
        MyCharacterNetworkObjectInfo = newInfo;
    }

    //################################################################################################
    //################################################################################################
    //�ȒP�ȑ��
    //
    public static void SetIsDead(bool isDead)
    {
        IsDead = isDead;
    }
    public static void SetIsDragging(bool isDragging)
    {
        IsDragging = isDragging;
    }
    public static void SetIsDash(bool isDash)
    {
        IsDash = isDash;
    }
    public static void SetIsJump(bool isJump)
    {
        IsJump = isJump;
    }
    public static void SetIsAttack1(bool isAttack1)
    {
        IsAttack1 = isAttack1;
    }
    public static void SetIsAttack2(bool isAttack2)
    {
        IsAttack2 = isAttack2;
    }
    public static void SetIsAttack3(bool isAttack3)
    {
        IsAttack3 = isAttack3;
    }
    public static void SetIsAttack4(bool isAttack4)
    {
        IsAttack4 = isAttack4;
    }
    public static void SetIsAttack5(bool isAttack5)
    {
        IsAttack5 = isAttack5;
    }
    //�J�����؂�ւ�
    public static void SetMainCamera(Camera newMainCamera)
    {
        MainCamera = newMainCamera;
    }

    //################################################################################################
    //################################################################################################
    //�o�O�����̂��߂�
    //
    public static void SetIsOwnerForResetTransform(bool isOwnerForResetTransform)
    {
        IsOwnerForResetTransform = isOwnerForResetTransform;
    }
    public static void SetGlobalVector3ForReset(Vector3 globalVector3ForReset)
    {
        GlobalVector3ForReset = globalVector3ForReset;
    }
    public static void SetGlobalQuaternionForReset(Quaternion globalQuaternionForReset)
    {
        GlobalQuaternionForReset = globalQuaternionForReset;
    }
    //################################################################################################
    //################################################################################################
}

//####################################################################################################################################################
//####################################################################################################################################################
//####################################################################################################################################################
//####################################################################################################################################################
//�L�����̃X�e�[�^�X�̕\��
[System.Serializable]
public class CharacterNetworkObjectInfo : INetworkSerializable
{
    public ulong CharacterNetworkObjectID;//���X�g�̒��ň�ӂɂ���K�v������
    public int CharacterID;
    public int CharacterLevel;
    public int CharacterHP = 100;
    public int CharacterKillCount = 0;
    public int CharacterKilledCount = 0;
    public ulong OwnerClientID;
    public string OwnerClientName;
    public int OwnerClientLevel;

    public CharacterNetworkObjectInfo(ulong CharacterNetworkObjectID, int CharacterID, int CharacterLevel, ulong OwnerClientID, string OwnerClientName, int OwnerClientLevel)
    {
        this.CharacterNetworkObjectID = CharacterNetworkObjectID;
        this.CharacterID = CharacterID;
        this.CharacterLevel = CharacterLevel;
        this.OwnerClientID = OwnerClientID;
        this.OwnerClientName = OwnerClientName;
        this.OwnerClientLevel = OwnerClientLevel;
    }

    public CharacterNetworkObjectInfo() { }

    public CharacterNetworkObjectInfo(CharacterNetworkObjectInfo original)
    {
        this.CharacterNetworkObjectID = original.CharacterNetworkObjectID;
        this.CharacterID = original.CharacterID;
        this.CharacterLevel = original.CharacterLevel;
        this.CharacterHP = original.CharacterHP;
        this.CharacterKillCount = original.CharacterKillCount;
        this.CharacterKilledCount = original.CharacterKilledCount;
        this.OwnerClientID = original.OwnerClientID;
        this.OwnerClientName = original.OwnerClientName;
        this.OwnerClientLevel = original.OwnerClientLevel;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref CharacterNetworkObjectID);
        serializer.SerializeValue(ref CharacterID);
        serializer.SerializeValue(ref CharacterLevel);
        serializer.SerializeValue(ref CharacterHP);
        serializer.SerializeValue(ref CharacterKillCount);
        serializer.SerializeValue(ref CharacterKilledCount);
        serializer.SerializeValue(ref OwnerClientID);

        if (serializer.IsWriter)
        {
            serializer.GetFastBufferWriter().WriteValueSafe(OwnerClientName);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out OwnerClientName);
        }

        serializer.SerializeValue(ref OwnerClientLevel);
    }

    public CharacterNetworkObjectInfo Clone()
    {
        return new CharacterNetworkObjectInfo(this);
    }
}

[System.Serializable]
public class AttackNetworkObjectInfo : INetworkSerializable
{
    public ulong AttackNetworkObjectID = ulong.MaxValue;//���X�g�̒��ł͈�ӂɂ���K�v������
    public int AttackID;
    public ulong ShooterCharacterNetworkObjectID;
    public int ShooterCharacterID;
    public int ShooterCharacterLevel;
    public ulong ShooterClientID;
    public string ShooterClientName;
    public int ShooterClientLevel;

    public AttackNetworkObjectInfo(int AttackID, ulong ShooterCharacterNetworkObjectID, int ShooterCharacterID, int ShooterCharacterLevel, ulong ShooterClientID, string ShooterClientName, int ShooterClientLevel)
    {
        this.AttackID = AttackID;
        this.ShooterCharacterNetworkObjectID = ShooterCharacterNetworkObjectID;
        this.ShooterCharacterID = ShooterCharacterID;
        this.ShooterCharacterLevel = ShooterCharacterLevel;
        this.ShooterClientID = ShooterClientID;
        this.ShooterClientName = ShooterClientName;
        this.ShooterClientLevel = ShooterClientLevel;
    }

    public AttackNetworkObjectInfo() { }

    public AttackNetworkObjectInfo(AttackNetworkObjectInfo original)
    {
        this.AttackNetworkObjectID = original.AttackNetworkObjectID;
        this.AttackID = original.AttackID;
        this.ShooterCharacterNetworkObjectID = original.ShooterCharacterNetworkObjectID;
        this.ShooterCharacterID = original.ShooterCharacterID;
        this.ShooterCharacterLevel = original.ShooterCharacterLevel;
        this.ShooterClientID = original.ShooterClientID;
        this.ShooterClientName = original.ShooterClientName;
        this.ShooterClientLevel = original.ShooterClientLevel;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref AttackNetworkObjectID);
        serializer.SerializeValue(ref AttackID);
        serializer.SerializeValue(ref ShooterCharacterNetworkObjectID);
        serializer.SerializeValue(ref ShooterCharacterID);
        serializer.SerializeValue(ref ShooterCharacterLevel);
        serializer.SerializeValue(ref ShooterClientID);

        if (serializer.IsWriter)
        {
            serializer.GetFastBufferWriter().WriteValueSafe(ShooterClientName);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out ShooterClientName);
        }

        serializer.SerializeValue(ref ShooterClientLevel);
    }

    public AttackNetworkObjectInfo Clone()
    {
        return new AttackNetworkObjectInfo(this);
    }
}
