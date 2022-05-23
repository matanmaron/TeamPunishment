using Mirror;
using System.Collections.Generic;

namespace TeamPunishment
{
    public class Player : NetworkBehaviour
    {
        public static readonly HashSet<string> playerNames = new HashSet<string>();

        [SyncVar(hook = nameof(OnPlayerNameChanged))]
        public string playerName;

        [SyncVar(hook = nameof(OnStarNameChanged))]
        public string startName;
        // RuntimeInitializeOnLoadMethod -> fast playmode without domain reload
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        static void ResetStatics()
        {
            playerNames.Clear();
        }

        void OnPlayerNameChanged(string _, string newName)
        {
            ChatUI.instance.localPlayerName = playerName;
        }

        void OnStarNameChanged(string _, string newName)
        {
            ChatUI.instance.localStarName = startName;
        }

        public override void OnStartServer()
        {
            playerName = (string)connectionToClient.authenticationData;
        }
    }
}
