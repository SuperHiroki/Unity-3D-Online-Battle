using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;

public class RoomManagerUpdateDB : MonoBehaviour
{
    //グローバル変数
    private string roomId => UserDataManager.RoomId;
    private string role => UserDataManager.Role;
    private string playerName => UserDataManager.UserData.Username;

    // Start is called before the first frame update
    void Start()
    {
        UpdateDBRoom();
    }

    //No Nameというユーザを手動でDBに追加しておかないと、DBの外部キー設定の関係で、エラーになる。
    //DBをリセットしたときは、No Nameというユーザを手動でDBに追加するのを忘れないように。
    private void UpdateDBRoom()
    {
        if (role == "Host" || role == "Server")
        {
            StartCoroutine(UpdateDBRoomAsync());
        }
    }

    IEnumerator UpdateDBRoomAsync()
    {
        string url = GlobalDefine.BaseUrl + "room";
        string json = JsonUtility.ToJson(new RoomInfo { RoomId = roomId, Host = playerName });
        //webリクエスト
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                UnityEngine.Debug.Log("QQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQ  UpdateDBRoom success");
                yield break;
            }
            else
            {
                UnityEngine.Debug.Log("QQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQ  UpdateDBRoom fail");
                yield break;
            }
        }
    }
}

