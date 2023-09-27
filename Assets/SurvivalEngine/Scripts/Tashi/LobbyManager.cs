using System;
using Tashi.NetworkTransport;

using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public string ProfileName = string.Empty;
    public LobbyEvent lobbyEvent;
    public LobbyInstance SerializeFieldLobby;
    private Lobby lobby;
    public Lobby Lobby
    {
        get => lobby;
        set
        {
            lobby = value;
            LobbyManager.Get.SerializeFieldLobby = new LobbyInstance(lobby);
        }
    }

    // [Space(8)]
    // public float _nextHeartbeat;
    // public float _nextLobbyRefresh;

    public bool _isLobbyHost;

    public string PlayerId => AuthenticationService.Instance.PlayerId;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void JoinNetwork()
    {
        if (_isLobbyHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    // private async void Update()
    // {
    //     if (string.IsNullOrEmpty(SerializeFieldLobby.Id))
    //     {
    //         return;
    //     }

    //     if (Time.realtimeSinceStartup >= _nextHeartbeat && _isLobbyHost)
    //     {
    //         _nextHeartbeat = Time.realtimeSinceStartup + 15;
    //         await LobbyService.Instance.SendHeartbeatPingAsync(SerializeFieldLobby.Id);
    //     }

    //     if (Time.realtimeSinceStartup >= _nextLobbyRefresh)
    //     {
    //         _nextLobbyRefresh = Time.realtimeSinceStartup + 2;

    //         var outgoingSessionDetails = NetworkTransport.OutgoingSessionDetails;

    //         var updatePlayerOptions = new UpdatePlayerOptions();
    //         if (outgoingSessionDetails.AddTo(updatePlayerOptions))
    //         {
    //             await LobbyService.Instance.UpdatePlayerAsync(
    //                 SerializeFieldLobby.Id,
    //                 PlayerId,
    //                 updatePlayerOptions
    //             );
    //         }

    //         if (_isLobbyHost)
    //         {
    //             var updateLobbyOptions = new UpdateLobbyOptions();
    //             if (outgoingSessionDetails.AddTo(updateLobbyOptions))
    //             {
    //                 await LobbyService.Instance.UpdateLobbyAsync(SerializeFieldLobby.Id, updateLobbyOptions);
    //             }
    //         }

    //         if (!NetworkTransport.SessionHasStarted)
    //         {
    //             var lobby = await LobbyService.Instance.GetLobbyAsync(SerializeFieldLobby.Id);
    //             var incomingSessionDetails = IncomingSessionDetails.FromUnityLobby(lobby);

    //             // This should be replaced with whatever logic you use to determine when a lobby is locked in.
    //             if (incomingSessionDetails.AddressBook.Count == 2)
    //             {
    //                 NetworkTransport.UpdateSessionDetails(incomingSessionDetails);
    //             }
    //         }
    //     }
    // }

    static LobbyManager instance;
    public static LobbyManager Get => instance ??= FindObjectOfType<LobbyManager>();
}
