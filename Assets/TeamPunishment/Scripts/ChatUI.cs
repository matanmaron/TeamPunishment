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
        private bool gamestarted = false;

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
        private void Start()
        {
            Debug.Log("[Start]");
            if (GameObject.FindGameObjectsWithTag("Player").Length > 4)
            {
                Debug.LogWarning("[HandleCommandMsg] - over 4 players. bye bye!");
                Application.Quit();
                return;
            }
            CmdSend("@@@LOGIN");
        }

        private void FillUpAllPlayersButton()
        {
            Debug.Log("[FillUpAllPlayersButton]");
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                var btn = Instantiate(ButtonPrefab, ButtonHolder);
                btn.GetComponentInChildren<Text>().text = player.GetComponent<Player>().playerName;
                btn.onClick.AddListener(delegate { OnPlayerKickClick(player.GetComponent<Player>().playerName); });
            }
        }
        private void OnPlayerKickClick(string player)
        {
            Debug.Log($"[OnPlayerKickClick] - kick {player}");
        }

        #endregion

        private bool HandleCommandMsg(string msg)
        {
            Debug.Log("[HandleCommandMsg]");
            if (msg == "@@@LOGIN")
            {
                if (!gamestarted && GameObject.FindGameObjectsWithTag("Player").Length == 4)
                {
                    gamestarted = true;
                    FillUpAllPlayersButton();
                    return true;
                }
            }
            return false;
        }
    }
}
