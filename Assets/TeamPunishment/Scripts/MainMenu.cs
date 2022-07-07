using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace TeamPunishment
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Button btnStart1;
        [SerializeField] Button btnStart4;
        [SerializeField] Button btnOption;
        [SerializeField] Button btnCredits;
        [SerializeField] Button btnExit;
        [SerializeField] Button btnBackOption;
        [SerializeField] Button btnBackCredits;
        [SerializeField] GameObject menuPanel;
        [SerializeField] GameObject optionPanel;
        [SerializeField] GameObject creditsPanel;

        private void Start()
        {
            if (Application.isBatchMode)
            {
                Scenes.LoadStandartGame();
                return;
            }
            if (GameManager.instance.isAndroid)
            {
                Camera.main.GetComponent<FlareLayer>().enabled = false;
                Camera.main.GetComponent<RetroTVFX.CRTEffect>().enabled = false;
                btnStart4.gameObject.SetActive(false);
            }
            btnStart1.onClick.AddListener(OnMenu1Player);
            btnStart4.onClick.AddListener(OnMenu4Player);
            btnOption.onClick.AddListener(OnMenuOption);
            btnCredits.onClick.AddListener(OnMenuCredits);
            btnExit.onClick.AddListener(OnMenuExit);
            btnBackOption.onClick.AddListener(OnBack);
            btnBackCredits.onClick.AddListener(OnBack);
            AudioManager.instance.PlayMusic();
            if (GameManager.instance.isDemoMode)
            {
                Debug.Log("demo detected");
                ShowDemoMenu();
            }
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

        private void OnMenu1Player()
        {
            GameManager.instance.SendAnalyticsEvent("start1");
            Scenes.LoadMobileInfo();
        }

        private void OnMenu4Player()
        {
            GameManager.instance.SendAnalyticsEvent("start4");
            Scenes.LoadStandartGame();
        }

        private void OnDestroy()
        {
            btnStart1.onClick.RemoveAllListeners();
            btnStart4.onClick.RemoveAllListeners();
            btnOption.onClick.RemoveAllListeners();
            btnExit.onClick.RemoveAllListeners();
            btnBackOption.onClick.RemoveAllListeners();
            btnBackCredits.onClick.RemoveAllListeners();
            btnCredits.onClick.RemoveAllListeners();
        }

        private void ShowDemoMenu()
        {
            GameManager.instance.isDemoMode = true;
            btnExit.gameObject.SetActive(false);
        }
    }
}