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
            if (Debug.isDebugBuild)
            {
                networkAddress = "localhost";
            }
            else
            {
                networkAddress = "35.216.233.202";
            }
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
            LoginUI.instance.gameObject.SetActive(true);
            LoginUI.instance.usernameInput.text = "";
            LoginUI.instance.usernameInput.ActivateInputField();
        }
    }
}
