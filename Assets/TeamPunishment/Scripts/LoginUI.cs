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

        public static LoginUI instance;

        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if (!Debug.isDebugBuild)
            {
                hostButton.gameObject.SetActive(false);
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
