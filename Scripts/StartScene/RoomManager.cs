using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class RoomManager : MonoBehaviour
{
    //################################################################################################################
    //################################################################################################################

    [SerializeField] private CreateScrollViewForRooms instanceCreateScrollViewForRooms;

    public void RunningRoomsGet()
    {
        StartCoroutine(GetActiveRooms());
    }

    IEnumerator GetActiveRooms()
    {
        UnityWebRequest request = UnityWebRequest.Get(GlobalDefine.BaseUrl + "room");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            RoomInfo[] rooms = MyJsonHelper.MyFromJson<RoomInfo>(response);
            RoomDataManager.UpdateRoomDataArray(rooms);
        }
        else
        {
            RoomDataManager.UpdateRoomDataArray(new RoomInfo[0]);
            TextManagerAllScene.MakeAlertText("There may be no room.");
            UnityEngine.Debug.Log(request.error);
        }
        instanceCreateScrollViewForRooms.CreateScrollView();
    }
}


