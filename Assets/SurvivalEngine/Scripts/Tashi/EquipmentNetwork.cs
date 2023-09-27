using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using SurvivalEngine;


public class EquipmentNetwork : NetworkBehaviour
{
    public void OnEquipItem(PlayerCharacter character, string itemId)
    {
        OnEquipItemsServerRpc(character.gameObject, itemId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnEquipItemsServerRpc(NetworkObjectReference objectReference, string itemId)
    {
        OnEquipItemsClientRpc(objectReference, itemId);
    }

    [ClientRpc]
    private void OnEquipItemsClientRpc(NetworkObjectReference objectReference, string itemId)
    {
        if (objectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.GetComponent<PlayerCharacterInventory>().ClientEquipItem(itemId);
        }
    }
}
