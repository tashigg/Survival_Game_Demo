using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using SurvivalEngine;

public class PlayerCharacterVariable : NetworkBehaviour
{
    PlayerCharacter character;

    public NetworkVariable<Vector3> SyncedMove = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        character = GetComponent<PlayerCharacter>();
        SyncedMove.OnValueChanged += SyncedMoveChanged;
    }

    PlayerData pdata;

    private void Start()
    {
        pdata = PlayerData.Get();
    }

    private void Update()
    {
        if (IsLocalPlayer)
        {
            SyncedMove.Value = character.move;
        }
    }

    void SyncedMoveChanged(Vector3 oldValue, Vector3 newValue)
    {
        if (!IsLocalPlayer)
        {
            character.move = newValue;
        }
    }
}
