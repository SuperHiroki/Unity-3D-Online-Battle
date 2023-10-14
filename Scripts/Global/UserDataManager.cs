using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    //############################################################################################################
    //############################################################################################################
    //############################################################################################################
    //############################################################################################################
    //�v���[���[���
    //################################################################
    //################################################################
    private const int DefaultLifeStock = 2;
    private const ulong DefaultClientId = ulong.MaxValue;
    private const string DefaultRoomId = null;
    private const string DefaultRole = null;
    private const bool DefaultIsLogin = false;
    private const int DefaultCharacterId = 0;
    private const int DefaultCharacterLevel = 6;
    private const int DefaultCharacterIdInCharaScene = 0;
    private static UserData DefaultUserData => new UserData
    {
        Username = "No Name",
        Password = "aaa",
        UserLevel = 1,
        Experience = 0,
        MagicStone = 0,
        Characters = new CharacterData[]
        {
            new CharacterData { CharacterId = DefaultCharacterId, CharacterLevel = DefaultCharacterLevel, Awakening = 0, Reliability = 0, Experience = 0 },
            new CharacterData { CharacterId = 1, CharacterLevel = 2, Awakening = 0, Reliability = 0, Experience = 0 },
            new CharacterData { CharacterId = 2, CharacterLevel = 3, Awakening = 0, Reliability = 0, Experience = 0 }
        }
    };
    private static readonly Dictionary<int, int> DefaultKillCountEachCharaDict = new Dictionary<int, int>();
    private static readonly Dictionary<int, int> DefaultKilledCountEachCharaDict = new Dictionary<int, int>();
    //################################################################
    //################################################################
    // �ȉ��A�O������Q�Ƃ͂ł��邪���ڂ̕ύX�͂ł��Ȃ��悤�ɂ��Ă��܂��B
    private static int _lifeStock = DefaultLifeStock;
    public static int LifeStock { get => _lifeStock; private set => _lifeStock = value; }
    private static ulong _clientId = DefaultClientId;
    public static ulong ClientId { get => _clientId; private set => _clientId = value; }
    private static string _roomId = DefaultRoomId;
    public static string RoomId { get => _roomId; private set => _roomId = value; }
    private static string _role = DefaultRole;
    public static string Role { get => _role; private set => _role = value; }
    private static bool _isLogin = DefaultIsLogin;
    public static bool IsLogin { get => _isLogin; private set => _isLogin = value; }
    private static int _characterId = DefaultCharacterId;
    public static int CharacterId { get => _characterId; private set => _characterId = value; }
    private static int _characterLevel = DefaultCharacterLevel;
    public static int CharacterLevel { get => _characterLevel; private set => _characterLevel = value; }
    private static int _characterIdInCharaScene = DefaultCharacterIdInCharaScene;
    public static int CharacterIdInCharaScene { get => _characterIdInCharaScene; private set => _characterIdInCharaScene = value; }
    // �������userData���ύX�\�ł��邽��private�Ƃ��ăJ�v�Z�������܂��B
    private static UserData _userData = DefaultUserData.Clone();
    public static UserData UserData { get => _userData.Clone(); set => _userData = value.Clone(); }  
    private static Dictionary<int, int> _killCountEachCharaDict = new Dictionary<int, int>(DefaultKillCountEachCharaDict);
    public static Dictionary<int, int> KillCountEachCharaDict { get => new Dictionary<int, int>(_killCountEachCharaDict); set => _killCountEachCharaDict = new Dictionary<int, int>(value); }
    private static Dictionary<int, int> _killedCountEachCharaDict = new Dictionary<int, int>(DefaultKilledCountEachCharaDict);
    public static Dictionary<int, int> KilledCountEachCharaDict { get => new Dictionary<int, int>(_killedCountEachCharaDict); set => _killedCountEachCharaDict = new Dictionary<int, int>(value); } 
    //################################################################
    //################################################################
    // ���O�A�E�g�����Ƃ�
    public static void ResetPlayerInfo()
    {
        IsLogin = DefaultIsLogin;
        UserData = DefaultUserData;
        KillCountEachCharaDict = DefaultKillCountEachCharaDict;
        KilledCountEachCharaDict = DefaultKillCountEachCharaDict;
    }
    //############################################################################################################
    //############################################################################################################
    //############################################################################################################
    //############################################################################################################
    //Userdata�̒��g��ύX
    //���[�U�̑S�Ă̏����X�V
    public static void UpdateAllUserData(UserData newUserData)
    {
        UserData = newUserData;
    }

    //�L�����N�^�[���ꂼ��̃f�[�^���X�V����
    public static void UpdateCharacters(CharacterData[] newCharacterDataArray)
    {
        UserData newUserData = UserData;
        newUserData.Characters = newCharacterDataArray.Select(character => character.Clone()).ToArray();
        UserData = newUserData;
    }

    //���O�ύX
    public static void UpdateUsername(string username)
    {
        UserData newUserData = UserData;
        newUserData.Username = username;
        UserData = newUserData;
    }

    //���[�U���x���ȂǕύX
    public static void UpdateUserData(int newUserLevel, int newExperience, int newMagicStone)
    {
        UserData newUserData = UserData;
        newUserData.Experience = newExperience;
        newUserData.MagicStone = newMagicStone;
        newUserData.UserLevel = newUserLevel;
        UserData = newUserData;
    }

    //#########################################################################################
    //#########################################################################################
    //�ȒP�ȑ��
    //
    public static void SetCharacterIdInCharaScene(int characterId)
    {
        CharacterIdInCharaScene = characterId;
    }

    public static void SetCharacterId(int characterId)
    {
        CharacterId = characterId;
    }

    public static void SetCharacterLevel(int characterLevel)
    {
        CharacterLevel = characterLevel;
    }

    public static void SetLifeStock(int lifeStock)
    {
        LifeStock = lifeStock;
    }

    public static void SetRoomId(string roomId)
    {
        RoomId = roomId;
    }

    public static void SetRole(string role)
    {
        Role = role;
    }

    public static void SetClientId(ulong clientId)
    {
        ClientId = clientId;
    }

    public static void SetIsLogin(bool isLogin)
    {
        IsLogin = isLogin;
    }

    //#########################################################################################
    //#########################################################################################
    //�L�����̃J�E���g
    //�N���A
    public static void ClearKillCountEachCharaDict()
    {
        var dict = KillCountEachCharaDict;
        dict.Clear();
        KillCountEachCharaDict = dict;
    }

    //�J�E���g���₷
    public static void AddkillCountEachCharaDict(int characterID)
    {
        var dict = KillCountEachCharaDict;
        if (dict.ContainsKey(characterID))
        {
            dict[characterID]++;
        }
        else
        {
            dict[characterID] = 1;
        }
        KillCountEachCharaDict = dict;
    }

    //####################################################################
    //��L�����̃J�E���g
    //�N���A
    public static void ClearKilledCountEachCharaDict()
    {
        var dict = KilledCountEachCharaDict;
        dict.Clear();
        KilledCountEachCharaDict = dict;
    }

    //�J�E���g���₷
    public static void AddkilledCountEachCharaDict(int characterID)
    {
        var dict = KilledCountEachCharaDict;
        if (dict.ContainsKey(characterID))
        {
            dict[characterID]++;
        }
        else
        {
            dict[characterID] = 1;
        }
        KilledCountEachCharaDict = dict;
    }

    //#########################################################################################
    //#########################################################################################
}

//####################################################################################################################################################
//####################################################################################################################################################
//####################################################################################################################################################
//####################################################################################################################################################
// { get; set; }������ƁAJsonUtility���@�\���Ȃ��Ȃ�̂Œ��ӂ���
[System.Serializable]
public class CharacterData
{
    public int CharacterId;
    public int CharacterLevel;
    public int Awakening;
    public int Reliability;
    public int Experience;

    public CharacterData() { }

    public CharacterData Clone()
    {
        return new CharacterData
        {
            CharacterId = this.CharacterId,
            CharacterLevel = this.CharacterLevel,
            Awakening = this.Awakening,
            Reliability = this.Reliability,
            Experience = this.Experience
        };
    }
}

[System.Serializable]
public class UserData
{
    public string Username;
    public string Password;
    public int UserLevel;
    public int Experience;
    public int MagicStone;
    public CharacterData[] Characters;

    public UserData Clone()
    {
        return new UserData
        {
            Username = this.Username,
            Password = this.Password,
            UserLevel = this.UserLevel,
            Experience = this.Experience,
            MagicStone = this.MagicStone,
            Characters = this.Characters?.Select(character => character.Clone()).ToArray()
        };
    }
}

[System.Serializable]
public class UserInput
{
    public string Username;
    public string Password;
}

[System.Serializable]
public class Message
{
    public string Content;
}
