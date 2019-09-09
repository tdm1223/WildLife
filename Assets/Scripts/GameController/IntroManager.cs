using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour {

    void Update()
    {
        if (Input.GetKeyDown("space") || Input.GetKeyDown("escape"))
            LoadLobbyScene();
    }

    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }
}
