using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Linq;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using NetworkEvent = Unity.Networking.Transport.NetworkEvent;

public class RelayManager : MonoBehaviour
{
    private async void Start()
    {
        //初期化処理
        try
        {
            //Netcodeの初期化
            NetworkManager.Singleton.Shutdown(true);
            //Relayの初期化
            await UnityServices.InitializeAsync();
            AuthenticationService.Instance.SignedIn += () =>
            {
                UnityEngine.Debug.Log("DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD AuthenticationService.Instance.PlayerId: " + AuthenticationService.Instance.PlayerId);
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
        //コマンドラインでserverが入力されたとき
        var args = GetCommandlineArgs();
        if (args.TryGetValue("-mode", out string mode))
        {
            switch (mode)
            {
                case "server":
                    CreateRoom("Server");
                    break;
                default:
                    break;
            }
        }
    }

    private Dictionary<string, string> GetCommandlineArgs()
    {
        Dictionary<string, string> argDictionary = new Dictionary<string, string>();
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; ++i)
        {
            var arg = args[i].ToLower();
            if (arg.StartsWith("-"))
            {
                var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                value = (value?.StartsWith("-") ?? false) ? null : value;

                argDictionary.Add(arg, value);
            }
        }
        return argDictionary;
    }

    //############################################################################################################################
    //############################################################################################################################
    //Serverとして部屋を作成
    public void CreateRelayButtonAsServer()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
        {
            return;
        }
        CreateRoom("Server");
    }

    //Hostとして部屋を作成
    public void CreateRelayButtonAsHost()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
        {
            UnityEngine.Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSS if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient).");
            return;
        }
        CreateRoom("Host");
    }

    // 部屋を作成するための関数
    private UnityAction<Scene, LoadSceneMode> sceneLoadedHandler;

    private async void CreateRoom(string role)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            //websocket(暗号化ありのTCP通信)を使うのか、dtls(暗号化ありのUDP通信)を使うのか
            //RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            RelayServerData relayServerData = new RelayServerData(allocation, "wss");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            UserDataManager.SetRoomId(joinCode);
            UserDataManager.SetRole(role);
            UnityEngine.Debug.Log("VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV myOnSceneLoaded.");
            sceneLoadedHandler = (scene, mode) => myOnSceneLoaded(scene, mode, role);
            SceneManager.sceneLoaded += sceneLoadedHandler;
            SceneManager.LoadScene("MainScene");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    private void myOnSceneLoaded(Scene scene, LoadSceneMode mode, string role)
    {
        SceneManager.sceneLoaded -= sceneLoadedHandler;
        if (role == "Host")
        {
            UnityEngine.Debug.Log("VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV StartHost.");
            NetworkManager.Singleton.StartHost();
        }
        else if (role == "Server")
        {
            NetworkManager.Singleton.StartServer();
        }
    }

    //############################################################################################################################
    //############################################################################################################################
    //クライアントが部屋に入るため
    public void JoinRelayButton(string joinCode)
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
        {
            return;
        }
        StartCoroutine(JoinRelay(joinCode));
    }

    private IEnumerator JoinRelay(string joinCode)
    {
        var joinTask = JoinRelayServerFromJoinCode(joinCode);
        yield return new WaitUntil(() => joinTask.IsCompleted);
        if (joinTask.IsFaulted)
        {
            TextManagerAllScene.MakeAlertText("Failed to join room");
            UnityEngine.Debug.LogError("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX Exception thrown when attempting to connect to Relay Server. Exception: " + joinTask.Exception.Message);
            yield break;
        }
        var relayServerData = joinTask.Result;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        UserDataManager.SetRoomId(joinCode);
        UserDataManager.SetRole("Client");
        NetworkManager.Singleton.StartClient();
    }

    public static async Task<RelayServerData> JoinRelayServerFromJoinCode(string joinCode)
    {
        try
        {
            JoinAllocation allocation;
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            UnityEngine.Debug.Log($"WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
            UnityEngine.Debug.Log($"WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
            UnityEngine.Debug.Log($"WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW client: {allocation.AllocationId}");
            //websocket(暗号化ありのTCP通信)を使うのか、dtls(暗号化ありのUDP通信)を使うのか
            //return new RelayServerData(allocation, "dtls");
            return new RelayServerData(allocation, "wss");
        }
        catch
        {
            UnityEngine.Debug.LogError("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX Relay create join code request failed");
            throw;
        }
    }
}

