using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Prototype.NetworkLobby;
using UnityStandardAssets.Cameras;
using System;

public class UITitle : UIMain {

    public GameObject lobbyManager;

    void Start () {
        if (LobbyManager.s_Singleton == null || !LobbyManager.s_Singleton.mainMenuPanel.gameObject.activeSelf)
        {
            Logo.SetActive(true);
            menuPanel.SetActive(true);
        }
        Cursor.visible = true;
    }

	public void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (optionPanel.activeSelf) //옵션창이 켜져있다면 옵션창을 닫기
			{
                BackButton(optionPanel);
			}
            else if (howToPlayPanel.activeSelf) //게임방법창이 켜져있다면 닫기
            {
                BackButton(howToPlayPanel);
            }
		}
	}

    //게임 시작버튼
    public void GameStartButton()
    {
        //Camera.main.GetComponent<MoveCamera>().RotateUPCamera();

        Audio.Play("Click");

        LobbyManager.s_Singleton.mainMenuPanel.gameObject.SetActive(true);
        LobbyManager.s_Singleton.topPanel.gameObject.SetActive(true);
        LobbyManager.s_Singleton.topPanel.transform.Find("BackButton").gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    //게임방법버튼, 설정 버튼
    public void OpenMenu(GameObject panel)
    {
        Audio.Play("Click");

        panel.SetActive(true);
        menuPanel.SetActive(false);
        if (Logo != null)
            Logo.SetActive(false);
    }
}
