using System;
using System.Collections.Generic;

using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;


public class LobbyRoom : MonoBehaviour
{
    public LobbyInstance Lobby;
    public void GetLobby(LobbyInstance lobby)
    {
        Lobby = lobby;
        Init();
    }

    [Header("---UI---")]
    public TMP_Text roomNameText;
    public TMP_Text roomId, roomSlotText;
    void Init()
    {
        roomNameText.text = Lobby.Name;
        roomId.text = Lobby.Id;
        // roomSlotText.text = $"{Lobby.AvailableSlots - LobbyManager.Get.SerializeFieldLobby.MaxPlayers}/{LobbyManager.Get.SerializeFieldLobby.MaxPlayers}";
        roomSlotText.text = $"{LocalWithLobby.Get.MaxConnections - Lobby.AvailableSlots}/{LocalWithLobby.Get.MaxConnections}";
    }

    public async void JoinLobbyButtonClicked()
    {
        var lobby = await LobbyService.Instance.JoinLobbyByIdAsync(Lobby.Id);
        LobbyManager.Get.Lobby = lobby;
        LobbyManager.Get._isLobbyHost = false;

        LocalWithLobby.Get.lobbyMenu.SetActive(false);
        LocalWithLobby.Get.roomMenu.SetActive(true);
        LobbyManager.Get.lobbyEvent.SubscribeToLobbyEvents();

    }
}
