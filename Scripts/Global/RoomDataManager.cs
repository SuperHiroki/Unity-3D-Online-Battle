using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomDataManager : MonoBehaviour
{
    //############################################################################################################
    //############################################################################################################
    //############################################################################################################
    //############################################################################################################
    //�v���[���[���
    private static RoomInfo[] DefaultRoomDataArrayForShow = new RoomInfo[0];
    private static RoomInfo[] _roomDataArrayForShow = DefaultRoomDataArrayForShow.Select(item => item.CreateCopy()).ToArray();
    public static RoomInfo[] RoomDataArrayForShow
    {
        get => _roomDataArrayForShow.Select(item => item.CreateCopy()).ToArray();
        private set => _roomDataArrayForShow = value.Select(item => item.CreateCopy()).ToArray();
    }
    public static void ResetRoomInfo()
    {
        RoomDataArrayForShow = DefaultRoomDataArrayForShow;
    }
    //############################################################################################################
    //############################################################################################################
    //############################################################################################################
    //############################################################################################################
    //�����̔z����X�V
    public static void UpdateRoomDataArray(RoomInfo[] newRoomDataArray)
    {
        RoomDataArrayForShow = newRoomDataArray.Select(item => item.CreateCopy()).ToArray();
    }
}

//####################################################################################################################################################
//####################################################################################################################################################
//####################################################################################################################################################
//####################################################################################################################################################
[System.Serializable]
public class RoomInfo
{
    public string RoomId;
    public string Host;

    public RoomInfo() { }

    public RoomInfo(string roomId, string host)
    {
        this.RoomId = roomId;
        this.Host = host;
    }

    public RoomInfo CreateCopy()
    {
        return new RoomInfo(RoomId, Host);
    }
}
