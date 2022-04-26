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
        public void SetHostname(string hostname)
        {
            if (!Debug.isDebugBuild)
            {
                networkAddress = "35.216.233.202";
                return;
            }
            networkAddress = hostname;
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
