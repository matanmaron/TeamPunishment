using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TeamPunishment
{
    public class TPPlayerController : NetworkBehaviour
    {
        [SerializeField] TextMeshProUGUI stateTMP;

        public ConnectionState connectionState = ConnectionState.Offline;

        public override void OnStartLocalPlayer()
        {
            Debug.Log($"player {netId} joined game");
            base.OnStartLocalPlayer();
            ToggleConnection(ConnectionState.Online);
        }

        private void ToggleConnection(ConnectionState state)
        {
            connectionState = state;
            stateTMP.text = state.ToString();
            Debug.Log($"user is now - {state}");
        }
    }
}