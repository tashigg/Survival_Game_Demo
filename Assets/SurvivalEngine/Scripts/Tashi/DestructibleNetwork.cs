using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SurvivalEngine;
using Unity.Netcode;

public class DestructibleNetwork : NetworkBehaviour
{
    public Destructible destructible;

    public void OnKill()
    {
        OnKillServerRpc(destructible.gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnKillServerRpc(NetworkObjectReference objectReference)
    {
        OnKillClientRpc(objectReference);
    }
    
    [ClientRpc]
    private void OnKillClientRpc(NetworkObjectReference objectReference)
    {
        // Debug.LogError("On client recieve!");

        if (objectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.GetComponent<Destructible>().ClientKill();
        }
    }
}
