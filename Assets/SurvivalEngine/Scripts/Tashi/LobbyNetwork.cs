using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LobbyNetwork : NetworkBehaviour
{
    public void OnStartGame(NetworkObject networkObject)
    {
        OnStartGameServerRpc(networkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnStartGameServerRpc(NetworkObjectReference objectReference)
    {
        OnStartGameClientRpc(objectReference);
    }

    [ClientRpc]
    private void OnStartGameClientRpc(NetworkObjectReference objectReference)
    {
        if (objectReference.TryGet(out NetworkObject networkObject))
        {
            // networkObject.GetComponent<RoomMenu>().ClientStartGame();
        }
    }
}
