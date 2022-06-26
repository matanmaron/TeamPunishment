using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace TeamPunishment
{
    public class Player : NetworkBehaviour
    {
        public static readonly HashSet<string> playerNames = new HashSet<string>();

        [SyncVar(hook = nameof(OnPlayerNameChanged))]
        public string playerName;

        // RuntimeInitializeOnLoadMethod -> fast playmode without domain reload
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        static void ResetStatics()
        {
            playerNames.Clear();
        }

        void OnPlayerNameChanged(string _, string newName)
        {
            //ChatUI.instance.localPlayerName = playerName;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            playerName = (string)connectionToClient.authenticationData;
        }
    }
}