using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button btnStart;
    [SerializeField] Button btnOption;
    [SerializeField] Button btnExit;
    [SerializeField] Button btnBack;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject optionPanel;

    private void Start()
    {
        btnStart.onClick.AddListener(OnMenuStart);
        btnOption.onClick.AddListener(OnMenuOption);
        btnExit.onClick.AddListener(OnMenuExit);
        btnBack.onClick.AddListener(OnOptionBack);
    }

    private void OnOptionBack()
    {
        menuPanel.SetActive(true);
        optionPanel.SetActive(false);
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
        btnBack.onClick.RemoveAllListeners();
    }

}
