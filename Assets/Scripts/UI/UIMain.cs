using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Prototype.NetworkLobby;
using UnityStandardAssets.Cameras;
using System;

public abstract class UIMain : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject optionPanel;
    public GameObject freelookCamera;
    public Dropdown graphicQualitySetting;
    public GameObject howToPlayPanel;
    public GameObject helpPanel;
    public Slider volumeSlider;
    public GameObject Logo;

    protected GameAudio Audio;

    void Awake ()
    {
        //그래픽 설정 초기화
        List<string> names = new List<string>(QualitySettings.names);
        graphicQualitySetting.AddOptions(names);
        UpdateQualityLabel();

        //소리 초기화
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        UpdateVolumeLabel();

        Audio = transform.parent.GetComponent<GameAudio>();

    }

    //뒤로가기 버튼 (UIMenu, UITitle에서 오버라이딩하여 구현)
    public abstract void BackButton(GameObject panel);

    //게임종료
    public void QuitGame()
    {
        Audio.Play("Click");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    //그래픽조절 관련 함수
    public void ChangeGraphicQuality(int index)
    {
        PlayerPrefs.SetInt("GraphicQuality", index);
        QualitySettings.SetQualityLevel(index);
        UpdateQualityLabel();
    }
    private void UpdateQualityLabel()
    {
        int currentQuality = PlayerPrefs.GetInt("GraphicQuality");
        string qualityName = QualitySettings.names[currentQuality];

        optionPanel.transform.Find("QualitySetting").Find("Label").GetComponent<UnityEngine.UI.Text>().text = qualityName;
    }

    //볼륨조절 관련 함수
    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
        volumeSlider.value = value;
        UpdateVolumeLabel();
    }
    private void UpdateVolumeLabel()
    {
        optionPanel.transform.Find("Master Volume").GetComponent<UnityEngine.UI.Text>().text = "Master Volume - " + (AudioListener.volume * 100).ToString("f0") + "%";
    }

}
