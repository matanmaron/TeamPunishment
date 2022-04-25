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

        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if (!Debug.isDebugBuild)
            {
                Destroy(DebugConsole);
                hostButton.gameObject.SetActive(false);
            }
        }
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        // Called by UI element UsernameInput.OnValueChanged
        public void ToggleButtons(string username)
        {
            hostButton.interactable = !string.IsNullOrWhiteSpace(username);
            clientButton.interactable = !string.IsNullOrWhiteSpace(username);
        }
    }
}
