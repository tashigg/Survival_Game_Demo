using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using SurvivalEngine;

public class ItemNetwork : NetworkBehaviour
{
    public Item item;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public void OnUse(PlayerCharacter character)
    {
        OnTakeServerRpc(character.gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnTakeServerRpc(NetworkObjectReference objectReference)
    {
        OnTakeClientRpc(objectReference);
    }

    [ClientRpc]
    private void OnTakeClientRpc(NetworkObjectReference objectReference)
    {
        // Debug.LogError("On client recieve!");

        if (objectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.GetComponent<PlayerCharacter>().Inventory.ClientTakeItem(item);
        }
    }
}
