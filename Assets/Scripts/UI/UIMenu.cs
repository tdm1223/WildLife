using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Prototype.NetworkLobby;
using UnityStandardAssets.Cameras;
using System;

public class UIMenu : UIMain
{
    //public GameObject helpPanel;
    public GameObject gameOverPanel;

    void Start()
    {
        menuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void BackButton(GameObject panel)
    {
        Audio.Play("Click");

        panel.SetActive(false);
        menuPanel.SetActive(true);
        ToggleDisable(true);

        //if (panel.name.Equals("OptionPanel"))
        //    LobbyManager.s_Singleton.topPanel.gameObject.SetActive(true);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameOverPanel.activeSelf) return;
            if (optionPanel.activeSelf) //옵션창이 켜져있다면 옵션창을 닫기
            {
                BackButton(optionPanel);
            }
            else if (howToPlayPanel.activeSelf)
            {
                BackButton(howToPlayPanel);
            }
            else if (helpPanel.activeSelf)
            {
                BackButton(helpPanel);
            }
            else
            {
                if (menuPanel.activeSelf)
                {
                    menuPanel.SetActive(false);
                    ToggleDisable(false);
                }
                else
                {
                    ToggleDisable(true);
                    menuPanel.SetActive(true);
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && !optionPanel.activeSelf && !menuPanel.activeSelf)
        {
            ToggleDisable(false);
        }
        if (gameOverPanel.activeSelf)
        {
            ToggleDisable(true);
        }
    }

    public void OnMainSceneButton()
    {
        Audio.Play("Click");

        LobbyManager.s_Singleton.GoTitleButton();
        //SceneManager.LoadScene("Lobby");
    }
    public void OnObserverButton()
    {
        Audio.Play("Click");

        if (!GameController.GetInstance().IsGameOver)
        {
            transform.Find("VignetteImage").gameObject.SetActive(false);
            transform.Find("Crosshair").gameObject.SetActive(false);
            transform.Find("LookingItemText").gameObject.SetActive(false);

            gameOverPanel.SetActive(false);
            GameController.GetInstance().ObserverMode();
        }
    }

    //마우스커서 온오프
    private void ToggleDisable(bool value)
    {
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = value;
    }

    //계속하기 버튼
    public void ResumeGame()
    {
        Audio.Play("Click");
        menuPanel.SetActive(false);
        GameObject.Find("LobbyManager/TopPanel").GetComponent<LobbyTopPanel>().ToggleVisibility(false);
        ToggleDisable(false);
    }

    //게임 방법 버튼
    public void HowToPlay()
    {
        Audio.Play("Click");

        Cursor.visible = true;
        ToggleDisable(true);
        menuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
        GameObject.Find("LobbyManager/TopPanel").GetComponent<LobbyTopPanel>().ToggleVisibility(false);
    }

    //옵션창열기
    public void OpenOptionMenu()
    {
        Audio.Play("Click");

        ToggleDisable(true);
        menuPanel.SetActive(false);
        optionPanel.SetActive(true);
        //GameObject.Find("LobbyManager/TopPanel").GetComponent<LobbyTopPanel>().ToggleVisibility(false);
    }

    //게임 방법 버튼
    public void Help()
    {
        Audio.Play("Click");

        Cursor.visible = true;
        ToggleDisable(true);
        menuPanel.SetActive(false);
        helpPanel.SetActive(true);
        //GameObject.Find("LobbyManager/TopPanel").GetComponent<LobbyTopPanel>().ToggleVisibility(false);
    }
}
