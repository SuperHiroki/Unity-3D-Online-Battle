using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.Networking;
using System.Diagnostics;
using System.Linq;

public class UserManager : MonoBehaviour
{
    //外部参照
    [SerializeField] private CreateScrollViewForCharacters instanceCreateScrollViewForCharacters;
    [SerializeField] private CreateScrollViewForRooms instanceCreateScrollViewForRooms;
    [SerializeField] private DropdownManager instanceDropdownManager;

    //グローバル変数
    private bool isLogin => UserDataManager.IsLogin;
    private string playerName => UserDataManager.UserData.Username;
    private int playerLevel => UserDataManager.UserData.UserLevel;
    private int experience => UserDataManager.UserData.Experience;
    private int magicStone => UserDataManager.UserData.MagicStone;

    void Start()
    {
        instanceCreateScrollViewForCharacters.CreateScrollView();
    }

    public void myLogin()
    {
        if (string.IsNullOrEmpty(RoomPlayerInfo.inputFields["Username"].text) || string.IsNullOrEmpty(RoomPlayerInfo.inputFields["Password"].text))
        {
            TextManagerAllScene.MakeAlertText("There is no input.");
            return;
        }
        if (RoomPlayerInfo.inputFields["Username"].text == "No Name")
        {
            TextManagerAllScene.MakeAlertText("[No Name] means You are not logged in.");
            return;
        }
        if (isLogin == true)
        {
            TextManagerAllScene.MakeAlertText("You have already logged in.");
            return;
        }
        //ユーザデータを更新する前にリセット
        UserDataManager.ResetPlayerInfo();
        StartCoroutine(GetUserData(RoomPlayerInfo.inputFields["Username"].text, RoomPlayerInfo.inputFields["Password"].text));
    }

    public void myLogout()
    {
        UserDataManager.ResetPlayerInfo();
        RoomPlayerInfo.texts["UserName"].keyText.text = playerName;
        RoomPlayerInfo.texts["UserLevel"].keyText.text = playerLevel.ToString() + " Level";
        RoomPlayerInfo.texts["Experience"].keyText.text = "Experience: " + experience.ToString();
        RoomPlayerInfo.texts["MagicStone"].keyText.text = "Magic Stone: " + magicStone.ToString();
        instanceCreateScrollViewForCharacters.CreateScrollView();
    }

    public void mySignup()
    {
        if (string.IsNullOrEmpty(RoomPlayerInfo.inputFields["Username"].text) || string.IsNullOrEmpty(RoomPlayerInfo.inputFields["Password"].text))
        {
            TextManagerAllScene.MakeAlertText("Enter some text.");
            return;
        }
        if (RoomPlayerInfo.inputFields["Username"].text == "No Name")
        {
            TextManagerAllScene.MakeAlertText("[No Name] means you are not login.");
            return;
        }
        if (isLogin == true)
        {
            TextManagerAllScene.MakeAlertText("You have already loged in.");
            return;
        }
        StartCoroutine(PostUserData(RoomPlayerInfo.inputFields["Username"].text, RoomPlayerInfo.inputFields["Password"].text));
    }

    IEnumerator GetUserData(string username, string password)
    {
        UnityWebRequest request = UnityWebRequest.Get(GlobalDefine.BaseUrl + "user/" + username);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            UserData userData = JsonUtility.FromJson<UserData>(response);//取得したデータ
            if (userData.Password != password)
            {
                TextManagerAllScene.MakeAlertText("Password or Name is wrong...");
                yield break;
            }
            //グローバルデータを変更
            UserDataManager.SetIsLogin(true);
            UserDataManager.UpdateAllUserData(userData);
            //表示
            RoomPlayerInfo.texts["UserName"].keyText.text = userData.Username;
            RoomPlayerInfo.texts["UserLevel"].keyText.text = userData.UserLevel.ToString() + " Level";
            RoomPlayerInfo.texts["Experience"].keyText.text = "Experience: " + userData.Experience + " / " + GlobalDefine.UserLevelUpExperienceList[userData.UserLevel-1];
            RoomPlayerInfo.texts["MagicStone"].keyText.text = "Magic Stone: " + userData.MagicStone;
            instanceCreateScrollViewForCharacters.CreateScrollView();
            TextManagerAllScene.MakeAlertText("You loged in.");
        }
        else 
        {
            if (request.responseCode == 404)
            {
                TextManagerAllScene.MakeAlertText("Password or Name is wrong.");
                UnityEngine.Debug.Log(request.error);
            }
            else
            {
                TextManagerAllScene.MakeAlertText("Something went wrong...");
                UnityEngine.Debug.Log(request.error);
            }
        }
        //Dropdownに所持キャラリストを適用する
        instanceDropdownManager.HavingCharacterNamesFromIdArrayToDropdown();
        //キャラレベルの選択肢をDropdownsに入れる
        instanceDropdownManager.SelectedCharaAllowedLevelListToDropdown();
    }

    IEnumerator PostUserData(string username, string password)
    {
        string url = GlobalDefine.BaseUrl + "user";
        string json = JsonUtility.ToJson(new UserInput { Username = username, Password = password });
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, json);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.responseCode == 201)
            {
                string response = request.downloadHandler.text;
                Message message = JsonUtility.FromJson<Message>(response);
                //表示を変更する
                RoomPlayerInfo.texts["UserName"].keyText.text = username;
                TextManagerAllScene.MakeAlertText("You signed up.");
                //グローバル変数を変更
                UserDataManager.UpdateUsername(username);
                UserDataManager.SetIsLogin(true);
            }
        }
        else
        {
            if (request.responseCode == 400)
            {
                TextManagerAllScene.MakeAlertText("That username already exists.");
            }
            else if (request.responseCode == 404)
            {
                TextManagerAllScene.MakeAlertText("Something went wrong at server");
                UnityEngine.Debug.Log(request.error);
            }
            else
            {
                TextManagerAllScene.MakeAlertText("Something went wrong......");
                UnityEngine.Debug.Log(request.error);
            }
        }
    }
}

