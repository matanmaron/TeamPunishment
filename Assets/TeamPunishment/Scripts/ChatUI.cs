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
        public Transform StarNone;
        public Text Textbox;
        public GameObject EndPanel;
        Stars starToKick = Stars.None;
        GameState gameState = GameState.None;

        [Header("Diagnostic - Do Not Edit")]
        public string localPlayerName;
        public string localStarName;

        Dictionary<NetworkConnectionToClient, string> connNames = new Dictionary<NetworkConnectionToClient, string>();
        List<Player> allPlayers = new List<Player>();
        List<Stars> dilemaResults = new List<Stars>();
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
            starToKick = (Stars)star;
            ButtonHolder.gameObject.SetActive(false);
            if (starToKick == Stars.None)
            {
                CmdSend("@@@DILEMA10");
            }
            else
            {
                CmdSend($"@@@DILEMA1{(int)starToKick}");
            }
            CheckIfSelectionEnded();
        }

        private void CheckIfSelectionEnded()
        {
            int ps = GameObject.FindGameObjectsWithTag(PLAYER_TAG).Length;
            if (dilemaResults.Count == ps)
            {
                WaitingText.text = string.Empty;
                var votes = CalcStar();
                ShowEnd(votes);
            }
            else
            {
                WaitingText.text = $"Waiting for {ps - dilemaResults.Count} more players to choose...";
            }
        }

        private int CalcStar()
        {
            Dictionary<Stars, float> votes = new Dictionary<Stars, float>
            {       
                {Stars.None, 0f },
                {Stars.Ferrum, 0f },
                {Stars.Cibus, 0f },
                {Stars.Ordo, 0f },
                { Stars.Artem, 0f }
            };
            for (int i = 0; i < dilemaResults.Count; i++)
            {
                if (i==0)
                {
                    votes[dilemaResults[i]] += 1;
                }
                else
                {
                    votes[dilemaResults[i]] += (1 - ((float)i / 100));
                }
            }
            Debug.Log($"VOTES:");
            foreach (var v in votes)
            {
                Debug.Log($"{v.Key} -> {v.Value}");
            }
            starToKick = votes.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            return Mathf.RoundToInt(votes.Aggregate((x, y) => x.Value > y.Value ? x : y).Value);
        }

        private void EndDilema2()
        {
            gameState = GameState.End;
        }

        public void OnEndClick()
        {
            EndPanel.SetActive(false);
            switch (gameState)
            {
                case GameState.Dilema_A:
                    EndDilema1();
                    break;
                case GameState.Dilema_NoKicked:
                case GameState.Dilema_Kicked:
                    EndDilema2();
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

        private void ShowEnd(int votes)
        {
            dilemaResults = new List<Stars>();
            ButtonHolder.gameObject.SetActive(true);
            EndPanel.SetActive(true);
            EndPanel.GetComponentInChildren<Text>().text = $"{starToKick} has been voted with {votes} Votes!";
        }

        private void EndDilema1()
        {
            if (starToKick == Stars.None)
            {
                MoveToDilema2a();
            }
            else
            {
                MoveToDilema2b();
            }
        }

        /// <summary>
        /// if choose none
        /// </summary>
        private void MoveToDilema2a()
        {
            SetupSecondDilemaNoKicked();
        }

        /// <summary>
        /// if choose some star
        /// </summary>
        private void MoveToDilema2b()
        {
            SetupSecondDilemaKicked();
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
                dilemaResults.Add((Stars)selection);
                CheckIfSelectionEnded();
            }
            return false;
        }

        private void StartGame()
        {
            Debug.Log("[StartGame]");
            gameState = GameState.Dilema_A;
            localPlayerName = LoginUI.localPlayerName;
            WaitingText.text = string.Empty;
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
            SetupFirstDilema();
            PlayIntro();
        }

        private void SetupSecondDilemaKicked()
        {
            gameState = GameState.Dilema_Kicked;
            Stars _starToKick = starToKick;
            InitDilema();
            Textbox.text = @"Yonos pose an imminent threat. There have been many deaths and planets lost already. The vaccine supply is depleted, and we don't have enough space to bury all of our loved ones. Is it better to dismiss one planet or to fight this through together?";
            GetStar((int)_starToKick).gameObject.SetActive(false);
        }

        private void SetupSecondDilemaNoKicked()
        {
            gameState = GameState.Dilema_NoKicked;
            InitDilema();
            Textbox.text = @"You will have to decide together on the number of residents you are willing to relinquish, and every single of the planets will have to decide on the minimal number of residents they wish to eliminate.";
        }

        private void SetupFirstDilema()
        {
            gameState = GameState.Dilema_A;
            InitDilema();
            Textbox.text = @"The FDA is ready for the 1st trial of the vaccine (raven’s blood and an owl’s feather). There are not enough vaccines for all the residents. Eliminating one of the planets will be sufficient for surviving this trial, however all of the planet’s resources will be forever lost and the ability to face the additional trials. What will you choose to do ?";
        }

        private void InitDilema()
        {
            StarArtem.gameObject.SetActive(true);
            StarCibus.gameObject.SetActive(true);
            StarFerrum.gameObject.SetActive(true);
            StarOrdo.gameObject.SetActive(true);
            StarNone.gameObject.SetActive(true);
            StarArtem.GetComponent<OnStarClick>().Init();
            StarCibus.GetComponent<OnStarClick>().Init();
            StarFerrum.GetComponent<OnStarClick>().Init();
            StarOrdo.GetComponent<OnStarClick>().Init();
            StarNone.GetComponent<OnStarClick>().Init();
            starToKick = Stars.None;
            Enum.TryParse(localStarName, out Stars localStar);
            GetStar((int)localStar).GetComponent<Button>().interactable = false;
            StarNone.gameObject.SetActive(true);
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
