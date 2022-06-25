using UnityEngine;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class LoginUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public InputField usernameInput;
        public Button hostButton;
        public Button clientButton;
        public Text errorText;
        public GameObject DebugConsole;
        public static LoginUI instance;
        public GameObject debugButons;
        public static string localPlayerName = string.Empty;

        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
#if DEVELOPMENT_BUILD  || UNITY_EDITOR
            DebugConsole.gameObject.SetActive(true);
            hostButton.gameObject.SetActive(true);
            debugButons.gameObject.SetActive(true);
#endif
        }
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Scenes.LoadMenu();
            }
        }
         
        // Called by UI element UsernameInput.OnValueChanged
        public void ToggleButtons(string username)
        {
            hostButton.interactable = !string.IsNullOrWhiteSpace(username);
            clientButton.interactable = !string.IsNullOrWhiteSpace(username);
            localPlayerName = username;

        }
    }
}
