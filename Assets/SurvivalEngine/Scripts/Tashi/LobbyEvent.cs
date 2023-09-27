using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

using UnityEngine;

public class LobbyEvent : MonoBehaviour
{
    public virtual void SubscribeToLobbyEvents()
    {
        var callbacks = new LobbyEventCallbacks();
        callbacks.LobbyChanged += OnLobbyChanged;
        Lobbies.Instance.SubscribeToLobbyEventsAsync(LobbyManager.Get.SerializeFieldLobby.Id, callbacks);
    }

    protected virtual void OnLobbyChanged(ILobbyChanges changes)
    {
        changes.ApplyToLobby(LobbyManager.Get.Lobby);
        LobbyManager.Get.SerializeFieldLobby = new LobbyInstance(LobbyManager.Get.Lobby);
    }
}
