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
    //プレーヤー情報
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
    // 以下、外部から参照はできるが直接の変更はできないようにしています。
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
    // こちらのuserDataも変更可能であるためprivateとしてカプセル化します。
    private static UserData _userData = DefaultUserData.Clone();
    public static UserData UserData { get => _userData.Clone(); set => _userData = value.Clone(); }  
    private static Dictionary<int, int> _killCountEachCharaDict = new Dictionary<int, int>(DefaultKillCountEachCharaDict);
    public static Dictionary<int, int> KillCountEachCharaDict { get => new Dictionary<int, int>(_killCountEachCharaDict); set => _killCountEachCharaDict = new Dictionary<int, int>(value); }
    private static Dictionary<int, int> _killedCountEachCharaDict = new Dictionary<int, int>(DefaultKilledCountEachCharaDict);
    public static Dictionary<int, int> KilledCountEachCharaDict { get => new Dictionary<int, int>(_killedCountEachCharaDict); set => _killedCountEachCharaDict = new Dictionary<int, int>(value); } 
    //################################################################
    //################################################################
    // ログアウトしたとき
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
    //Userdataの中身を変更
    //ユーザの全ての情報を更新
    public static void UpdateAllUserData(UserData newUserData)
    {
        UserData = newUserData;
    }

    //キャラクターそれぞれのデータを更新する
    public static void UpdateCharacters(CharacterData[] newCharacterDataArray)
    {
        UserData newUserData = UserData;
        newUserData.Characters = newCharacterDataArray.Select(character => character.Clone()).ToArray();
        UserData = newUserData;
    }

    //名前変更
    public static void UpdateUsername(string username)
    {
        UserData newUserData = UserData;
        newUserData.Username = username;
        UserData = newUserData;
    }

    //ユーザレベルなど変更
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
    //簡単な代入
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
    //キル数のカウント
    //クリア
    public static void ClearKillCountEachCharaDict()
    {
        var dict = KillCountEachCharaDict;
        dict.Clear();
        KillCountEachCharaDict = dict;
    }

    //カウント増やす
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
    //被キル数のカウント
    //クリア
    public static void ClearKilledCountEachCharaDict()
    {
        var dict = KilledCountEachCharaDict;
        dict.Clear();
        KilledCountEachCharaDict = dict;
    }

    //カウント増やす
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
// { get; set; }をつけると、JsonUtilityが機能しなくなるので注意する
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
