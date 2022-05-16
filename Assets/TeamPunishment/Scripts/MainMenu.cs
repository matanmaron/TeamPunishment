using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private void Start()
    {
        btnStart.onClick.AddListener(OnMenuStart);
        btnOption.onClick.AddListener(OnMenuOption);
        btnCredits.onClick.AddListener(OnMenuCredits);
        btnExit.onClick.AddListener(OnMenuExit);
        btnBackOption.onClick.AddListener(OnBack);
        btnBackCredits.onClick.AddListener(OnBack);
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
        SceneManager.LoadScene(1);
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

}
