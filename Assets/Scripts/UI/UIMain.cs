using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Prototype.NetworkLobby;
using UnityStandardAssets.Cameras;
using System;

public class UIMain : MonoBehaviour {

    public GameObject menuPanel;
    public GameObject optionPanel;
    public GameObject freelookCamera;
    public Dropdown graphicQualitySetting;
    public GameObject howToPlayPanel;
    public Slider volumeSlider;
    public GameObject Logo;

    protected GameAudio Audio;


    // Use this for initialization
    void Awake () {
        List<string> names = new List<string>(QualitySettings.names);
        graphicQualitySetting.AddOptions(names);

        //그래픽 설정 초기화
        UpdateQualityLabel();

        ///마우스 감도 초기화
        freelookCamera.GetComponent<FreeLookCam>().turnSpeed = PlayerPrefs.GetFloat("MouseSensitivity");
        UpdateMouseSensitivityLabel();

        //소리 초기화
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        UpdateVolumeLabel();

        Audio = transform.parent.GetComponent<GameAudio>();

    }

    //뒤로가기 버튼
    public virtual void BackButton(GameObject panel)
    {
        Audio.Play("Click");

        panel.SetActive(false);
        menuPanel.SetActive(true);
        if(Logo != null)
            Logo.SetActive(true);
    }

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
        optionPanel.transform.Find("Master Volume").GetComponent<UnityEngine.UI.Text>().text = "Master Volume - " + (AudioListener.volume * 100).ToString("f2") + "%";
    }

    //감도조절 관련함수
    public void SetMouseSensitivity(float value)
    {
        freelookCamera.GetComponent<FreeLookCam>().turnSpeed = value * 10;
        PlayerPrefs.SetFloat("MouseSensitivity", value * 10);
        UpdateMouseSensitivityLabel();
    }
    private void UpdateMouseSensitivityLabel()
    {
        optionPanel.transform.Find("MouseSlider").GetComponent<Slider>().value = PlayerPrefs.GetFloat("MouseSensitivity") * 0.1f;
        optionPanel.transform.Find("MouseSensitivity").GetComponent<UnityEngine.UI.Text>().text = "마우스 감도 : " + Mathf.Round(PlayerPrefs.GetFloat("MouseSensitivity") * 100) * 0.01;
        //OptionPanel.transform.FindChild("InputField/Placeholder").GetComponent<UnityEngine.UI.Text>().text = "" + freelookCamera.GetComponent<FreeLookCam>().turnSpeed;
    }
    public void SetMouseSensitivity(string value)
    {
        freelookCamera.GetComponent<FreeLookCam>().turnSpeed = (float)Convert.ToDouble(value);
        UpdateMouseSensitivityLabel();
    }
}
