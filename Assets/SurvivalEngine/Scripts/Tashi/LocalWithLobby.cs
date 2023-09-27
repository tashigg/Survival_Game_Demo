using System;
using Tashi.NetworkTransport;
using System.Collections.Generic;
using UnityEngine.UI;

using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LocalWithLobby : MonoBehaviour
{
    public TMP_InputField profileName, lobbyCodeName, maxPlayerInput;
    public GameObject profileMenu, lobbyMenu, roomMenu;


    // private const string LobbyName = "example lobby";
    public int MaxConnections => 8;
    private string PlayerId => AuthenticationService.Instance.PlayerId;

    [Header("---LOBBY UI---")]
    public TMP_Text profileNameText;

    public async void SignInButtonClicked()
    {
        if (string.IsNullOrEmpty(profileName.text))
        {
            Debug.Log($"Signing in with the default profile");
            profileNameText.text = $"Name: Player";
            await UnityServices.InitializeAsync();
        }
        else
        {
            Debug.Log($"Signing in with profile '{profileName.text}'");
            var options = new InitializationOptions();
            options.SetProfile(profileName.text);
            profileNameText.text = $"Name: {profileName.text}";
            LobbyManager.Get.ProfileName = profileName.text;
            await UnityServices.InitializeAsync(options);
        }

        try
        {
            AuthenticationService.Instance.SignedIn += delegate
            {
                profileMenu.SetActive(false);
                lobbyMenu.SetActive(true);

                FilteringLobbies();
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
    }

    public async void CreateLobbyButtonClicked()
    {
        var lobbyOptions = new CreateLobbyOptions
        {
            IsPrivate = false
        };

        // lobbyOptions.Data = new Dictionary<string, DataObject>()
        // {
        //     {
        //     "PublicLobbyData", new DataObject(
        //     visibility: DataObject.VisibilityOptions.Public, // Visible publicly.
        //     value: "PublicLobbyData")
        //     },
        // };

        var lobby = await LobbyService.Instance.CreateLobbyAsync(LobbyManager.Get.ProfileName == string.Empty ? "Player's Room" : $"{LobbyManager.Get.ProfileName}'s Room", MaxConnections, lobbyOptions);

        LobbyManager.Get.Lobby = lobby;
        LobbyManager.Get._isLobbyHost = true;
        LobbyManager.Get.lobbyEvent.SubscribeToLobbyEvents();

        lobbyMenu.SetActive(false);
        roomMenu.SetActive(true);

        FilteringLobbies();
    }

    // public async void JoinLobbyButtonClicked()
    // {
    //     Lobby lobby;
    //     // if (string.IsNullOrEmpty(lobbyCodeName.text))
    //     // {
    //     //     lobby = await LobbyService.Instance.QuickJoinLobbyAsync();

    //     //     LobbyManager.Get.Lobby = new LobbyInstance(lobby);
    //     //     LobbyManager.Get._isLobbyHost = false;

    //     //     Debug.Log($"Join lobby with id: {lobby.Id}");
    //     // }
    //     // else
    //     // {
    //     lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCodeName.text);
    //     // }

    //     LobbyManager.Get.Lobby = new LobbyInstance(lobby);
    //     LobbyManager.Get._isLobbyHost = false;


    //     // SceneManager.LoadScene("BlankMap");
    // }


    // private void OnApplicationQuit()
    // {
    //     if (LobbyManager.Get._isLobbyHost)
    //     {
    //         LobbyService.Instance.DeleteLobbyAsync(LobbyManager.Get.SerializeFieldLobby.Id);
    //     }
    // }

    private void Awake()
    {
        maxPlayerInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
    }

    public RectTransform roomContent;
    [Header("---LOBBIES---")]
    public LobbyRoom room;
    List<LobbyRoom> rooms = new List<LobbyRoom>();
    public List<LobbyInstance> lobbyInstances = new List<LobbyInstance>();

    public async void FilteringLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder>()
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            foreach (var room in rooms)
            {
                Destroy(room.gameObject);
            }

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            foreach (var lobby in lobbies.Results)
            {
                var lobbyInstance = new LobbyInstance(lobby);
                lobbyInstances.Add(lobbyInstance);

                var roomInstance = Instantiate(room, roomContent);
                roomInstance.GetLobby(lobbyInstance);
                rooms.Add(roomInstance);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    static LocalWithLobby instance;
    public static LocalWithLobby Get => instance ??= FindObjectOfType<LocalWithLobby>();
}

[System.Serializable]
public class LobbyInstance //Just for debug
{
    public string HostId, Id, LobbyCode, Upid, EnvironmentId, Name;
    public int MaxPlayers, AvailableSlots;
    public bool IsPrivate, IsLocked;

    public LobbyInstance(Lobby lobby)
    {
        HostId = lobby.HostId;
        Id = lobby.Id;
        LobbyCode = lobby.LobbyCode;
        Upid = lobby.Upid;
        EnvironmentId = lobby.EnvironmentId;
        Name = lobby.Name;

        MaxPlayers = lobby.MaxPlayers;
        AvailableSlots = lobby.AvailableSlots;

        IsPrivate = lobby.IsPrivate;
        IsLocked = lobby.IsLocked;
    }
}