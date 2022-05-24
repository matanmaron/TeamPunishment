using Mirror;
using System;
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

        [Header("Dilema Elements")]
        public Transform ButtonHolder;
        public Button ButtonPrefab;
        public Transform StarArtem;
        public Transform StarCibus;
        public Transform StarFerrum;
        public Transform StarOrdo;
        public Text Textbox;
        public GameObject EndPanel;
        Stars starToKick = Stars.None;
        GameState gameState = GameState.None;

        [Header("Diagnostic - Do Not Edit")]
        public string localPlayerName;
        public string localStarName;

        Dictionary<NetworkConnectionToClient, string> connNames = new Dictionary<NetworkConnectionToClient, string>();
        List<Player> allPlayers = new List<Player>();

        public static ChatUI instance;
        private bool gamestarted = false;
        private bool chatWindowHidden = true;
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
        }

        public void Quit()
        {
            Debug.Log("[Quit]");
            SceneManager.LoadScene(0);
        }

        [Command(requiresAuthority = false)]
        public void CmdSend(string message, NetworkConnectionToClient sender = null)
        {
            Debug.Log($"[CmdSend] {sender} - {message}");
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
            ShowEnd(star);
        }

        private void EndDilema2b()
        {
            gameState = GameState.End;
            if (starToKick == Stars.None)
            {
                CmdSend("@@@DILEMA10");
            }
            else
            {
                CmdSend($"@@@DILEMA1{(int)starToKick}");
            }
        }

        private void EndDilema2a()
        {
            gameState = GameState.End;
            if (starToKick == Stars.None)
            {
                CmdSend("@@@DILEMA10");
            }
            else
            {
                CmdSend($"@@@DILEMA1{(int)starToKick}");
            }
        }

        public void OnEndClick()
        {
            EndPanel.SetActive(false);
            switch (gameState)
            {
                case GameState.Dilema_A:
                    EndDilema1();
                    break;
                case GameState.Dilema_B_A:
                    EndDilema2a();
                    Quit();
                    break;
                case GameState.Dilema_B_B:
                    EndDilema2b();
                    Quit();
                    break;
                case GameState.End:
                    Quit();
                    break;
                default:
                    Debug.LogError($"[OnEndClick] -{gameState}- How did you get here??");
                    break;
            }
        }
        private void ShowEnd(int star)
        {
            starToKick = (Stars)star;
            EndPanel.SetActive(true);
            EndPanel.GetComponentInChildren<Text>().text = $"[OnPlayerKickClick] - player {localPlayerName} kick {(Stars)star}";
        }

        private void EndDilema1()
        {
            Debug.Log($"[OnPlayerKickClick] - player {localPlayerName} kick {starToKick}");
            if (starToKick == Stars.None)
            {
                CmdSend("@@@DILEMA10");
                MoveToDilema2a();
            }
            else
            {
                CmdSend($"@@@DILEMA1{(int)starToKick}");
                MoveToDilema2b();
            }
        }

        /// <summary>
        /// if choose none
        /// </summary>
        private void MoveToDilema2a()
        {
            SetupSecondDilemaA();
        }

        /// <summary>
        /// if choose some star
        /// </summary>
        private void MoveToDilema2b()
        {
            SetupSecondDilemaB();
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
            Debug.Log($"[HandleCommandMsg] {playerName} - {msg}");
            if (msg == "@@@LOGIN")
            {
                Debug.Log($"{playerName} ha joined !");
                int players = GameObject.FindGameObjectsWithTag(PLAYER_TAG).Length;
                WaitingText.text = $"Waiting For {MAX_PLAYERS - players} More Player...!";
                if (!gamestarted && players == MAX_PLAYERS)
                {
                    Debug.Log("[HandleCommandMsg] - gamestarted");
                    StartGame();
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

        private void StartGame()
        {
            Debug.Log("[StartGame]");
            gameState = GameState.Dilema_A;
            SetupFirstDilema();
            localPlayerName = LoginUI.localPlayerName;
            WaitingText.gameObject.SetActive(false);
            gamestarted = true;
            int index = 1;
            foreach (GameObject player in GameObject.FindGameObjectsWithTag(PLAYER_TAG).OrderBy(x => x.name))
            {
                Player p = player.GetComponent<Player>();
                allPlayers.Add(p);
                player.name = (Stars.None + index).ToString();
                index++;
            }
            localStarName = allPlayers.Where(x => x.isLocalPlayer).FirstOrDefault().name;
            PlayIntro();
        }

        private void SetupSecondDilemaB()
        {
            gameState = GameState.Dilema_B_B;
            Stars _starToKick = starToKick;
            InitDilema();
            GetStar((int)_starToKick).gameObject.SetActive(false);
        }

        private void SetupSecondDilemaA()
        {
            gameState = GameState.Dilema_B_A;
            InitDilema();
            Textbox.text = @"Yonos pose an imminent threat. There have been many deaths and planets lost already. The vaccine supply is depleted, and we don't have enough space to bury all of our loved ones. Is it better to dismiss one planet or to fight this through together?";
        }

        private void SetupFirstDilema()
        {
            gameState = GameState.Dilema_A;
            InitDilema();
            Textbox.text = @"The FDA is ready for the 1st trial of the vaccine (raven�s blood and an owl�s feather). There are not enough vaccines for all the residents. Eliminating one of the planets will be sufficient for surviving this trial, however all of the planet�s resources will be forever lost and the ability to face the additional trials. What will you choose to do ?";
        }

        private void InitDilema()
        {
            StarArtem.gameObject.SetActive(true);
            StarCibus.gameObject.SetActive(true);
            StarFerrum.gameObject.SetActive(true);
            StarOrdo.gameObject.SetActive(true);
            StarArtem.GetComponent<OnStarClick>().Init();
            StarCibus.GetComponent<OnStarClick>().Init();
            StarFerrum.GetComponent<OnStarClick>().Init();
            StarOrdo.GetComponent<OnStarClick>().Init();
            starToKick = Stars.None;
        }


        private void PlayIntro()
        {
            VideoManager.instance.PlayIntro(OnIntroEnd);
        }

        private void OnIntroEnd()
        {
            Enum.TryParse(localStarName, out Stars myStar);
            switch (myStar)
            {
                case Stars.Ferrum:
                    VideoManager.instance.PlayFerrum(OnStarVideoEnd);
                    break;
                case Stars.Cibus:
                    VideoManager.instance.PlayCibus(OnStarVideoEnd);
                    break;
                case Stars.Ordo:
                    VideoManager.instance.PlayOrdo(OnStarVideoEnd);
                    break;
                case Stars.Artem:
                    VideoManager.instance.PlayArtem(OnStarVideoEnd);
                    break;
                default:
                    Debug.LogError("you cannot have star NONE !");
                    break;
        }
        }

        private void OnStarVideoEnd()
        {
            ButtonHolder.gameObject.SetActive(true);
        }
    }
}
