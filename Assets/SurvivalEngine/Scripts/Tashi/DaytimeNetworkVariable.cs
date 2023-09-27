using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using SurvivalEngine;
using UnityEngine.Events;

public class DaytimeNetworkVariable : NetworkBehaviour
{
    public UnityAction onNewDay; //Right after changing day
    [SerializeField] private NetworkVariable<float> syncedDaytime = new NetworkVariable<float>();

    PlayerData pdata;
    void Awake()
    {
        if (!LobbyManager.Get._isLobbyHost)
        {
            syncedDaytime.OnValueChanged += OnSomeValueChanged;
        }
    }

    private void Start()
    {
        pdata = PlayerData.Get();
    }

    private void Update()
    {

        if (LobbyManager.Get._isLobbyHost && pdata != null)
        {
            pdata.day_time += TheGame.Get().game_speed_per_sec * Time.deltaTime;
            if (pdata.day_time >= 24f)
            {
                pdata.day_time = 0f;
                pdata.day++; //New day
                onNewDay?.Invoke();
            }

            syncedDaytime.Value = pdata.day_time;
        }
    }

    private void OnSomeValueChanged(float previous, float current)
    {
        // Debug.LogError($"Detected NetworkVariable Change: Previous: {previous} | Current: {current}");
        if (pdata != null)
        {
            pdata.day_time = current;
        }
    }

}
