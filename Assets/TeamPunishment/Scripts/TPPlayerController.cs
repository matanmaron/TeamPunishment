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

        private GameManager server;

        private void Awake()
        {
            server = FindObjectOfType<GameManager>();
            if (server == null)
            {
                Debug.LogError("oops, cannot find server!");
            }
        }

        public override void OnStartLocalPlayer()
        {
            if (server.IsGameStarted())
            {
                Debug.LogError($"sorry, game has started !");
                NetworkManager.singleton.StopClient();
                return;
            }
            Debug.Log($"player {netId} joined game");
            base.OnStartLocalPlayer();
            ToggleConnection(ConnectionState.Online);
            CMDAddPlayer();
        }

        private void Start()
        {
            if (!isLocalPlayer && !hasAuthority)
            {
                Debug.Log("sorry, it's not you...");
                gameObject.SetActive(false);
                return;
            }
        }

        private void ToggleConnection(ConnectionState state)
        {
            connectionState = state;
            stateTMP.text = state.ToString();
            Debug.Log($"user is now - {state}");
        }

        [Command]
        private void CMDAddPlayer()
        {
            server.RPCAddPlayer(netId, gameObject);
        }
    }
}