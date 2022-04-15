using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamPunishment
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] GameObject debugPrefab;
        public Dictionary<uint, GameObject> allplayers = new Dictionary<uint, GameObject>();
        const int MAX_PLAYERS = 1;
        [SyncVar] int currentPlayers = 0;
        [SyncVar] bool isGameStarted = false;

        private void Start()
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("[SERVER] - Instantiate debug console");
                Instantiate(debugPrefab);
            }
        }

        public bool IsGameStarted()
        {
            return isGameStarted;
        }

        [ClientRpc]
        public void RPCAddPlayer()
        {
            currentPlayers++;
            if (allplayers.Count > MAX_PLAYERS)
            {
                isGameStarted = true;
                StartGame();
            }
            Debug.Log($"[SERVER] - new client has connected ({currentPlayers})");
        }

        public void StartGame()
        { 
        }
    }
}