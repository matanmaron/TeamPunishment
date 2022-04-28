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
        public Button ToggleChatButton;

        [Header("Kick UI Elements")]
        public Transform ButtonHolder;
        public Button ButtonPrefab;
        public Transform Marker;
        public Transform Star1;
        public Transform Star2;
        public Transform Star3;
        public Transform Star4;

        [Header("Finish Elements")]
        public GameObject finishPanel;
        public Image finishStar;

        [Header("Diagnostic - Do Not Edit")]
        public string localPlayerName;

        Dictionary<NetworkConnectionToClient, string> connNames = new Dictionary<NetworkConnectionToClient, string>();

        public static ChatUI instance;
        private bool gamestarted = false;
        private bool chatWindowHidden = true;
        private int starToKick = 0;

#if UNITY_EDITOR
        const int MAX_PLAYERS = 1;
#else
        const int MAX_PLAYERS = 4; //NEVER CHANGE!
#endif

        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            Debug.Log("[Start]");
            gamestarted = false;
            if (GameObject.FindGameObjectsWithTag("Player").Length > MAX_PLAYERS)
            {
                Debug.LogWarning("[HandleCommandMsg] - over 4 players. bye bye!");
                Application.Quit();
                return;
            }
            StartCoroutine(Login(0.1f));

            ToggleChatButton.onClick.AddListener(ToggleChat);
        }

        IEnumerator Login(float time)
        {
            yield return new WaitForSeconds(time);
            CmdSend("@@@LOGIN");
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Quit();
            }
            if (gamestarted && Input.GetKeyUp(KeyCode.Return))
            {
                OnEnterClick();
            }
        }

        public void Quit()
        {
            Debug.Log("[Quit]");
            Application.Quit();
        }

        public void OnEnterClick()
        {
            //CmdSend($"@@@{starToKick}");
            finishPanel.SetActive(true);
            if (starToKick == 0)
            {
                finishStar.gameObject.SetActive(false);
            }
            else
            {
                finishStar.sprite = GetStar(starToKick).GetComponent<Image>().sprite;
                finishStar.SetNativeSize();
            }
            Debug.Log($"[*******] - player {localPlayerName} choose to kick {starToKick}");
        }

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
            if (HandleCommandMsg(message)) return;
            if (message.StartsWith("@@@")) return;

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
            if (chatWindowHidden)
            {
                ToggleChatButton.GetComponent<Image>().color = Color.yellow;
            }

            // it takes 2 frames for the UI to update ?!?!
            yield return null;
            yield return null;

            // slam the scrollbar down
            scrollbar.value = 0;
        }

        private void ToggleChat()
        {
            ToggleChatButton.GetComponent<Image>().color = Color.white;
            if (chatWindowHidden)
            {
                GetComponent<Animator>().Play("ChatAnimationShow");
            }
            else
            {
                GetComponent<Animator>().Play("ChatAnimationHide");

            }
            chatWindowHidden = !chatWindowHidden;
        }

        public void OnPlayerStarClick(int star)
        {
            if (star == starToKick)
            {
                Marker.gameObject.SetActive(false);
                starToKick = 0;
            }
            else
            {
                Marker.gameObject.SetActive(true);
                var starObject = GetStar(star);
                Marker.SetParent(starObject);
                Marker.gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                starToKick = star;
            }
            Debug.Log($"[OnPlayerKickClick] - player {localPlayerName} kick {starToKick}");
        }

        private Transform GetStar(int index)
        {
            switch (index)
            {
                case 1: return Star1;
                case 2: return Star2;
                case 3: return Star3;
                case 4: return Star4;
                default:
                    return null;
            }
        }

        private bool HandleCommandMsg(string msg)
        {
            Debug.Log("[HandleCommandMsg]");
            if (msg == "@@@LOGIN")
            {
                if (!gamestarted && GameObject.FindGameObjectsWithTag("Player").Length == MAX_PLAYERS)
                {
                    Debug.Log("[HandleCommandMsg] - gamestarted");
                    gamestarted = true;
                    PlayIntro();
                    return true;
                }
            }
            return false;
        }

        private void PlayIntro()
        {
            VideoManager.instance.PlayIntro(()=> ButtonHolder.gameObject.SetActive(true));
        }
    }
}
