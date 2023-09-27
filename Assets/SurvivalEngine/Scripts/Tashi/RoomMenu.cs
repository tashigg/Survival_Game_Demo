using System.Collections;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;


using UnityEngine;
using TMPro;

public class RoomMenu : MonoBehaviour
{
    public TMP_Text introText;
    public TMP_Text roomNameText, roomIdText, roomSlotText;

    public GameObject StartButton, JoinButton;
    // public NetworkObject networkObject;
    // public LobbyNetwork lobbyNetwork;

    private void Awake()
    {
        // networkObject = GetComponent<NetworkObject>();
        // NetworkManager.Singleton.OnClientConnectedCallback += (ulong clientId) =>
        // {
        //     roomSlotText.text = $"Room Slot: {4 - lobby.AvailableSlots}/4";
        // };

        Init();
        if (LobbyManager.Get._isLobbyHost)
        {
            StartButton.SetActive(true);
        }
        else
        {
            JoinButton.SetActive(true);
        }

        InvokeRepeating(nameof(UpdateRoomSlot), 1f, 1f);
    }

    void Init()
    {
        introText.text = $"Hi {LobbyManager.Get.ProfileName}, just wait Host start..";
        roomNameText.text = $"Room Name: {LobbyManager.Get.SerializeFieldLobby.Name}";
        roomIdText.text = $"Room ID: {LobbyManager.Get.SerializeFieldLobby.Id}";
        roomSlotText.text = $"Room Slot: {LocalWithLobby.Get.MaxConnections - LobbyManager.Get.SerializeFieldLobby.AvailableSlots}/{LocalWithLobby.Get.MaxConnections}";

    }

     void UpdateRoomSlot()
    {
        roomSlotText.text = $"Room Slot: {LocalWithLobby.Get.MaxConnections - LobbyManager.Get.SerializeFieldLobby.AvailableSlots}/{LocalWithLobby.Get.MaxConnections}";
    }


    public void StartGame()
    {
        SceneManager.LoadScene("BlankMap");
    }
}
