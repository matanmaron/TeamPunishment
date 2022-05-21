using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        public Text WaitingText;

        [Header("Kick UI Elements")]
        public Transform ButtonHolder;
        public Button ButtonPrefab;
        public Transform StarArtem;
        public Transform StarCibus;
        public Transform StarFerrum;
        public Transform StarOrdo;

        [Header("Finish Elements")]
        public GameObject finishPanel;
        public Image finishStar;

        [Header("Diagnostic - Do Not Edit")]
        public string localPlayerName;

        Dictionary<NetworkConnectionToClient, string> connNames = new Dictionary<NetworkConnectionToClient, string>();

        public static ChatUI instance;
        private bool gamestarted = false;
        private bool chatWindowHidden = true;
        private Stars starToKick = Stars.None;
        const string ADMIN = "admin";
        const string PLAYER_TAG = "Player";
#if UNITY_EDITOR
        const int MAX_PLAYERS = 2;
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
            if (localPlayerName == ADMIN)
            {
                Debug.Log("[ADMIN JOINED]");
                CmdSend("@@@ADMIN");
                return;
            }
            WaitingText.gameObject.SetActive(true);
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
            SceneManager.LoadScene(0);
        }

        public void OnEnterClick()
        {
            //CmdSend($"@@@{starToKick}");
            finishPanel.SetActive(true);
            if (starToKick == Stars.None)
            {
                finishStar.gameObject.SetActive(false);
                CmdSend("@@@DILEMA10");
            }
            else
            {
                finishStar.sprite = GetStar((int)starToKick).GetComponent<Image>().sprite;
                finishStar.SetNativeSize();
                finishPanel.GetComponentInChildren<Text>().text = $"player {localPlayerName} choose to kick planet {starToKick}...";
                CmdSend($"@@@DILEMA1{(int)starToKick}");
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
            if (HandleCommandMsg(message, playerName)) return;
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
            resetStarColor();
            if (star == (int)starToKick)
            {
                starToKick = Stars.None;
            }
            else
            {
                Transform starObject = GetStar((int)star);
                starObject.GetComponent<Image>().color = Color.yellow;
                starToKick = (Stars)star;
            }
            Debug.Log($"[OnPlayerKickClick] - player {localPlayerName} kick {starToKick}");
        }

        private void resetStarColor()
        {
            StarArtem.GetComponent<Image>().color = Color.white;
            StarCibus.GetComponent<Image>().color = Color.white;
            StarFerrum.GetComponent<Image>().color = Color.white;
            StarOrdo.GetComponent<Image>().color = Color.white;
        }

        private Transform GetStar(int index)
        {
            switch (index)
            {
                case 1: return StarFerrum;
                case 2: return StarCibus;
                case 3: return StarOrdo;
                case 4: return StarArtem;
                default:
                    return null;
            }
        }

        private bool HandleCommandMsg(string msg, string playerName)
        {
            Debug.Log("[HandleCommandMsg]");
            if (msg == "@@@LOGIN")
            {
                Debug.Log($"{playerName} ha joined !");
                int players = GameObject.FindGameObjectsWithTag(PLAYER_TAG).Length;
                WaitingText.text = $"Waiting For {MAX_PLAYERS - players} More Player...!";
                if (!gamestarted && players == MAX_PLAYERS)
                {
                    Debug.Log("[HandleCommandMsg] - gamestarted");
                    WaitingText.gameObject.SetActive(false);
                    gamestarted = true;
                    PlayIntro();
                    return true;
                }
            }
            if (msg == "@@@ADMIN")
            {
                Player admin = FindObjectsOfType<Player>().Where(x => x.playerName == ADMIN).FirstOrDefault();
                admin.name = ADMIN;
                admin.gameObject.tag = "Untagged";
            }
            if (msg.StartsWith("@@@DILEMA1"))
            {
                int.TryParse(msg[10].ToString(), out int selection);
                Debug.Log($"player {playerName} kicked out {(Stars)selection}");
            }
            return false;
        }

        private void PlayIntro()
        {
            VideoManager.instance.PlayIntro(()=> ButtonHolder.gameObject.SetActive(true));
        }
    }
}
