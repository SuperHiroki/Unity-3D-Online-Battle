using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

public class HeartbeatRoom : MonoBehaviour
{
    //グローバル変数
    private string roomId => UserDataManager.RoomId;
    private string role => UserDataManager.Role;

    //ローカル変数
    private float heartbeatInterval = 6.0f;
    private string heartbeatURL;

    private void Awake()
    {
        heartbeatURL = GlobalDefine.BaseUrl + "room/" + roomId + "/heartbeat";
    }

    private void Start()
    {
        if (role == "Host" || role == "Server")
        {
            StartCoroutine(SendHeartbeat());
        }
    }

    private IEnumerator SendHeartbeat()
    {
        while (true)
        {
            using (UnityWebRequest www = UnityWebRequest.PostWwwForm(heartbeatURL, ""))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    UnityEngine.Debug.Log("LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL  Heartbeat success" + www.error);
                }
                else
                {
                    UnityEngine.Debug.Log("LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL  Heartbeat failure");
                }
            }
            yield return new WaitForSeconds(heartbeatInterval);
        }
    }
}

