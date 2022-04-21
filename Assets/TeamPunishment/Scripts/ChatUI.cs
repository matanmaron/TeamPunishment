using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class ChatUI : NetworkBehaviour
    {
        [Header("Chat UI Elements")]
        public InputField chatMessage;
        public Text chatHistory;
        public Scrollbar scrollbar;

        [Header("Kick UI Elements")]
        public Transform ButtonHolder;
        public Button ButtonPrefab;

        [Header("Diagnostic - Do Not Edit")]
        public string localPlayerName;

        Dictionary<NetworkConnectionToClient, string> connNames = new Dictionary<NetworkConnectionToClient, string>();

        public static ChatUI instance;

        void Awake()
        {
            instance = this;
        }

        #region Chat
        [Command(requiresAuthority = false)]
        public void CmdSend(string message, NetworkConnectionToClient sender = null)
        {
            if (!connNames.ContainsKey(sender))
                connNames.Add(sender, sender.identity.GetComponent<Player>().playerName);

            if (!string.IsNullOrWhiteSpace(message))
                RpcReceive(connNames[sender], message.Trim());
        }

        [ClientRpc]
        public void RpcReceive(string playerName, string message)
        {
            if (HandleCommandMsg(message))
            {
                return;
            }
            string prettyMessage = playerName == localPlayerName ?
                $"<color=red>{playerName}:</color> {message}" :
                $"<color=blue>{playerName}:</color> {message}";
            AppendMessage(prettyMessage);
        }

        // Called by UI element MessageField.OnEndEdit
        public void OnEndEdit(string input)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetButtonDown("Submit"))
                SendMessage();
        }

        // Called by OnEndEdit above and UI element SendButton.OnClick
        public void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(chatMessage.text))
            {
                CmdSend(chatMessage.text.Trim());
                chatMessage.text = string.Empty;
                chatMessage.ActivateInputField();
            }
        }

        internal void AppendMessage(string message)
        {
            StartCoroutine(AppendAndScroll(message));
        }

        IEnumerator AppendAndScroll(string message)
        {
            chatHistory.text += message + "\n";

            // it takes 2 frames for the UI to update ?!?!
            yield return null;
            yield return null;

            // slam the scrollbar down
            scrollbar.value = 0;
        }
        #endregion

        #region Kick
        private void OnConnectedToServer()
        {
            Debug.Log("[player] OnConnectedToServer");
            CmdSend("@@@LOGIN");
        }

        private void FillUpAllPlayersButton()
        {
            foreach (string player in connNames.Values)
            {
                var btn = Instantiate(ButtonPrefab, ButtonHolder);
                btn.GetComponentInChildren<Text>().text = player;
                btn.onClick.AddListener(delegate { OnPlayerKickClick(player); });
            }
        }
        private void OnPlayerKickClick(string player)
        {
            Debug.Log($"[OnPlayerKickClick] - kick {player}");
        }

        #endregion

        private bool HandleCommandMsg(string msg)
        {
            if (msg == "@@@LOGIN")
            {
                Debug.Log("[HandleCommandMsg] LOGIN");
                if (connNames.Count == 2)
                {
                    FillUpAllPlayersButton();
                    return true;
                }
            }
            return false;
        }
    }
}
