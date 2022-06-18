using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace TeamPunishment
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Button btnStart;
        [SerializeField] Button btnOption;
        [SerializeField] Button btnCredits;
        [SerializeField] Button btnExit;
        [SerializeField] Button btnBackOption;
        [SerializeField] Button btnBackCredits;
        [SerializeField] GameObject menuPanel;
        [SerializeField] GameObject optionPanel;
        [SerializeField] GameObject creditsPanel;

        int demoCounter = 0;

        private void Start()
        {
            btnStart.onClick.AddListener(OnMenuStart);
            btnOption.onClick.AddListener(OnMenuOption);
            btnCredits.onClick.AddListener(OnMenuCredits);
            btnExit.onClick.AddListener(OnMenuExit);
            btnBackOption.onClick.AddListener(OnBack);
            btnBackCredits.onClick.AddListener(OnBack);
            AudioManager.instance.PlayMusic();
        }

        private void OnMenuCredits()
        {
            menuPanel.SetActive(false);
            creditsPanel.SetActive(true);
        }

        private void OnBack()
        {
            creditsPanel.SetActive(false);
            optionPanel.SetActive(false);
            menuPanel.SetActive(true);
        }

        private void OnMenuExit()
        {
            Application.Quit();
        }

        private void OnMenuOption()
        {
            menuPanel.SetActive(false);
            optionPanel.SetActive(true);
        }

        private void OnMenuStart()
        {
            if (GameManager.instance.isAndroid)
            {
                Scenes.LoadMobileInfo();
            }
            else
            {
                Scenes.LoadStandartGame();
            }
        }

        private void OnDestroy()
        {
            btnStart.onClick.RemoveAllListeners();
            btnOption.onClick.RemoveAllListeners();
            btnExit.onClick.RemoveAllListeners();
            btnBackOption.onClick.RemoveAllListeners();
            btnBackCredits.onClick.RemoveAllListeners();
            btnCredits.onClick.RemoveAllListeners();
        }

        public void OnStarDemo()
        {
            if (!GameManager.instance.isDemoMode && demoCounter >= 3)
            {
                GameManager.instance.isDemoMode = true;
                btnExit.gameObject.SetActive(false);
                Debug.Log("DEMO ON");
            }
            demoCounter++;
        }
    }
}