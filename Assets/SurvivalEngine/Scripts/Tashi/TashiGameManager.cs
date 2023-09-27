using Tashi.NetworkTransport;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;

public class TashiGameManager : MonoBehaviour
{
    public TashiNetworkTransport NetworkTransport => NetworkManager.Singleton.NetworkConfig.NetworkTransport as TashiNetworkTransport;
    public float _nextHeartbeat;
    public float _nextLobbyRefresh;

    private void Start()
    {
        LobbyManager.Get.JoinNetwork();
    }

    private void Update()
    {
        UpdateNetwork();
    }

    async void UpdateNetwork()
    {
        if (Time.realtimeSinceStartup >= _nextHeartbeat && LobbyManager.Get._isLobbyHost)
        {
            _nextHeartbeat = Time.realtimeSinceStartup + 15;
            await LobbyService.Instance.SendHeartbeatPingAsync(LobbyManager.Get.SerializeFieldLobby.Id);
        }

        if (Time.realtimeSinceStartup >= _nextLobbyRefresh)
        {
            this._nextLobbyRefresh = Time.realtimeSinceStartup + 2;
            this.LobbyUpdating();
            this.ReceiveIncomingDetail();
        }
    }

    async void LobbyUpdating()
    {
        var outgoingSessionDetails = NetworkTransport.OutgoingSessionDetails;

        var updatePlayerOptions = new UpdatePlayerOptions();
        string lobbyId = LobbyManager.Get.SerializeFieldLobby.Id;

        if (outgoingSessionDetails.AddTo(updatePlayerOptions))
        {
            await LobbyService.Instance.UpdatePlayerAsync(lobbyId, LobbyManager.Get.PlayerId, updatePlayerOptions);
        }

        if (LobbyManager.Get._isLobbyHost)
        {
            var updateLobbyOptions = new UpdateLobbyOptions();
            if (outgoingSessionDetails.AddTo(updateLobbyOptions))
            {
                await LobbyService.Instance.UpdateLobbyAsync(lobbyId, updateLobbyOptions);
            }
        }
    }

    async void ReceiveIncomingDetail()
    {
        if (NetworkTransport.SessionHasStarted) return;
        Debug.LogWarning("Receive Incoming Detail");

        string lobbyId = LobbyManager.Get.SerializeFieldLobby.Id;
        var lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
        var incomingSessionDetails = IncomingSessionDetails.FromUnityLobby(lobby);

        if (incomingSessionDetails.AddressBook.Count == lobby.Players.Count)
        {
            Debug.LogWarning("Update Session Details");
            NetworkTransport.UpdateSessionDetails(incomingSessionDetails);
        }
    }
}
