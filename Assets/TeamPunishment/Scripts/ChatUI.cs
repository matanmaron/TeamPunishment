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
        public Text ShameAreaText;

        [Header("Dilema Elements")]
        public Transform ButtonHolder;
        public Button ButtonPrefab;
        public Transform StarArtem;
        public Transform StarCibus;
        public Transform StarFerrum;
        public Transform StarOrdo;
        public Text TextStarNone;
        public TextboxElement textboxElement;
        public GameObject EndPanel;
        public ScoresPanel ScoresPanel;
        public Text TimerText;
        Stars starToKick = Stars.None;
        private GameState _gameState;
        public GameState gameState
        {
            get { return _gameState; }
            set { _gameState = value; Debug.Log($"gameState is {_gameState}"); }
        }
        [Header("Diagnostic - Do Not Edit")]
        public string localPlayerName;
        public string localStarName;
        int needToWait = 0;
        Action waitCallback = null;
        Dictionary<NetworkConnectionToClient, string> connNames = new Dictionary<NetworkConnectionToClient, string>();
        List<Player> allPlayers = new List<Player>();
        List<Stars> dilemaResults = new List<Stars>();
        public static ChatUI instance;
        private bool gamestarted = false;
        private bool chatWindowHidden = true;
        const string ADMIN = "admin";
        const string PLAYER_TAG = "Player";
        bool canActivateTimer = false;
        Coroutine TimerCoroutine = null;
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
            TextStarNone.gameObject.SetActive(false);
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
                Debug.Log("Dilema END");
                if (TimerCoroutine != null)
                {
                    StopCoroutine(TimerCoroutine);
                    TimerCoroutine = null;
                    TimerText.text = string.Empty;
                }
                WaitingText.text = string.Empty;
                var votes = CalcStar();
                StartCoroutine(ShowScores(votes, Mathf.RoundToInt(votes.Aggregate((x, y) => x.Value > y.Value ? x : y).Value)));
            }
            else
            {
                WaitingText.text = $"Waiting for {ps - dilemaResults.Count} more players to choose...";
            }
        }

        private Dictionary<Stars, float> CalcStar()
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
                if (i == 0)
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
            return votes;
        }

        private void EndDilema2()
        {
            gameState = GameState.End;
        }

        public void OnScoresClick()
        {
            ScoresPanel.gameObject.SetActive(false);
            ButtonHolder.gameObject.SetActive(false);
            waitCallback = AfterScores;
            CmdSend("@@@WAIT");
        }

        private void AfterScores()
        {
            ButtonHolder.gameObject.SetActive(true);
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
                    Debug.LogError($"[OnScoresClick] -{gameState}- How did you get here??");
                    break;
            }
        }

        IEnumerator ShowScores(Dictionary<Stars, float> votes, int winner)
        {
            yield return new WaitForSeconds(5);
            dilemaResults = new List<Stars>();
            ButtonHolder.gameObject.SetActive(true);
            ScoresPanel.gameObject.SetActive(true);
            ScoresPanel.Init(votes, winner);
            ShameAreaText.text = string.Empty;
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
                if (TimerCoroutine == null && canActivateTimer)
                {
                    TimerCoroutine = StartCoroutine(StartTimer());
                }
                int.TryParse(msg[10].ToString(), out int selection);
                ShowShameScreen(playerName, (Stars)selection);
                dilemaResults.Add((Stars)selection);
                CheckIfSelectionEnded();
            }
            if (msg.StartsWith("@@@WAIT"))
            {
                OnWaitCMD();
                return true;
            }
            return false;
        }

        private void ShowShameScreen(string playerName, Stars selection)
        {
            GameObject[] allp = GameObject.FindGameObjectsWithTag(PLAYER_TAG);
            var planet = string.Empty;
            foreach (var p in allp)
            {
                if (p.GetComponent<Player>().playerName == playerName)
                {
                    planet = p.name;
                    break;
                }
            }
            Debug.Log($"player {playerName} ({planet}) kicked out {selection}");
            if (selection == Stars.None)
            {
                ShameAreaText.text += $"Player {playerName} chose not to eliminate any Planet\n";
            }
            else
            {
                ShameAreaText.text += $"Player {playerName} ({planet}) chose to eliminate Planet {selection}\n";
            }
        }

        IEnumerator StartTimer()
        {
            canActivateTimer = false;
            Debug.Log("Start Timer");
            for (int i = 0; i < 30; i++)
            {
                TimerText.text = (30 - i).ToString();
                yield return new WaitForSeconds(1);
            }
            Debug.Log("End Timer");
            OnPlayerStarClick(0);
        }

        private void StartGame()
        {
            Debug.Log("[StartGame]");
            AudioManager.instance.PlayMusic();
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
            canActivateTimer = true;
            gameState = GameState.Dilema_Kicked;
            Stars _starToKick = starToKick;
            InitDilema();
            StarArtem.GetComponent<OnStarClick>().Init(new List<int> { 180, -10, -20, -40 - 50, -60 });
            StarCibus.GetComponent<OnStarClick>().Init(new List<int> { 180, -10, -20, -40 - 50, -60 });
            StarFerrum.GetComponent<OnStarClick>().Init(new List<int> { 120, -10, -15, -20, -30, -45 });
            StarOrdo.GetComponent<OnStarClick>().Init(new List<int> { 70, -5, -5, -10, -20, -30 });
            textboxElement.texts = new List<string>();
            textboxElement.texts.Add(@"Yonos pose an imminent threat. There have been many deaths and planets lost already. The vaccine supply is depleted, and we don't have enough space to bury all of our loved ones. Is it better to dismiss one planet or to fight this through together?");
            textboxElement.Init();
            GetStar((int)_starToKick).gameObject.SetActive(false);
        }

        private void SetupSecondDilemaNoKicked()
        {
            canActivateTimer = true;
            gameState = GameState.Dilema_NoKicked;
            InitDilema();
            StarArtem.GetComponent<OnStarClick>().Init(new List<int> { 250, -20, -40, -60, -60, -70 });
            StarCibus.GetComponent<OnStarClick>().Init(new List<int> { 250, -20, -40, -60, -60, -70 });
            StarFerrum.GetComponent<OnStarClick>().Init(new List<int> { 200, -10, -20, -30, -50, -90 });
            StarOrdo.GetComponent<OnStarClick>().Init(new List<int> { 100, -10, -20, -25, -20, -25 });
            textboxElement.texts = new List<string>();
            textboxElement.texts.Add(@"If you decide not to eliminate one of the planets and to keep the order as it is. You will have to decide together on the number of residents you are willing to relinquish...");
            textboxElement.texts.Add(@"and every one of the planets will have to decide on the minimal number of residents they wish to eliminate from their own planet. You may converse to reach the optimal amount.");
            textboxElement.Init();
        }

        private void SetupFirstDilema()
        {
            canActivateTimer = true;
            gameState = GameState.Dilema_A;
            InitDilema();
            StarArtem.GetComponent<OnStarClick>().Init(new List<int> { 250, -20, -40, -60, -60, -70 });
            StarCibus.GetComponent<OnStarClick>().Init(new List<int> { 250, -20, -40, -60, -60, -70 });
            StarFerrum.GetComponent<OnStarClick>().Init(new List<int> { 200, -10, -20, -30, -50, -90 });
            StarOrdo.GetComponent<OnStarClick>().Init(new List<int> { 100, -10, -20, -25, -20, -25 });
            textboxElement.texts = new List<string>();
            textboxElement.texts.Add(@"The FDA is ready for the 1st trial of the vaccine (raven’s blood and an owl’s feather). There are not enough vaccines for all the residents...");
            textboxElement.texts.Add(@"Eliminating one of the planets will be sufficient for surviving this trial, however all of the planet’s resources will be forever lost and the ability to face the additional trials....");
            textboxElement.texts.Add(@"What will you choose to do ? <size=55>If you choose to dismiss one of the planets please click on it until it will explode, if you wish to stay together and fight press Enter</size>");
        }

        private void InitDilema()
        {
            StarArtem.gameObject.SetActive(true);
            StarCibus.gameObject.SetActive(true);
            StarFerrum.gameObject.SetActive(true);
            StarOrdo.gameObject.SetActive(true);
            TextStarNone.gameObject.SetActive(true);
            starToKick = Stars.None;
            Enum.TryParse(localStarName, out Stars localStar);
            GetStar((int)localStar).GetComponent<Button>().interactable = false;
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
            waitCallback = () => ButtonHolder.gameObject.SetActive(true);
            CmdSend("@@@WAIT");
        }

        private void OnWaitCMD()
        {

            needToWait++;
            int ps = GameObject.FindGameObjectsWithTag(PLAYER_TAG).Length;
            WaitingText.text = $"Waiting For {ps - needToWait} More Player...!";
            if (needToWait >= ps)
            {
                waitCallback?.Invoke();
                waitCallback = null;
                needToWait = 0;
            }
        }
    }
}
