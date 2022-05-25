using Mirror;
using UnityEngine;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

namespace TeamPunishment
{
    [AddComponentMenu("")]
    public class ChatNetworkManager : NetworkManager
    {
        // Called by UI element NetworkAddressInput.OnValueChanged
        public override void Start()
        {
            base.Start();
            SetHostname("localhost");
        }

        public void SetHostname(string hostname)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            networkAddress = hostname;
#else
            networkAddress = "35.216.233.202";
#endif
            Debug.Log($"[SetHostname] networkAddress is {networkAddress}");
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            // remove player name from the HashSet
            if (conn.authenticationData != null)
                Player.playerNames.Remove((string)conn.authenticationData);

            base.OnServerDisconnect(conn);
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Debug.Log("disconnected");
        }
    }
}
