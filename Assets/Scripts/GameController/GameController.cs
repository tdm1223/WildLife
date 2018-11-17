using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityStandardAssets.Cameras;

public class GameController : NetworkBehaviour
{
    GameObject[] survivors;

    private bool isGameOver = false;

    public FreeLookCam FreeLookCam;
    public GameObject UICanvas;
    public BigWildSpawnManager BigWildSpawnManager;
    public ItemSpawnManager ItemSpawnManager;
    public BackGroundAudio BackGroundAudio;
    private HelpMessage HelpMessage;
    private GameAudio Audio;

    private GameObject GameOverPanel;
    private Text BigWildSpawnText;
    private Text MessageText;
    private GameObject RightActionMessagePanel;
    private GameObject WrongActionMessagePanel;
    private Text ClockText;

    private int liveSurvivorNumber;
    private bool isSinglePlay = false;

    private float gameTime;

    private static GameController instance;
    public static GameController GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(GameController)) as GameController;
            if (!instance)
            {
                instance = null;
                //Debug.Log("Hierarchy에 GameController가 없음");
            }
        }
        return instance;
    }

    void Start()
    {
        GameOverPanel = UICanvas.transform.Find("GameOverPanel").gameObject;
        BigWildSpawnText = UICanvas.transform.Find("BigWildSpawnPanel/BigWildSpawnText").GetComponent<Text>();
        MessageText = UICanvas.transform.Find("MessagePanel/MessageText").GetComponent<Text>();
        HelpMessage = GetComponent<HelpMessage>();

        RightActionMessagePanel = UICanvas.transform.Find("ActionMessagePanel/RightActionMessagePanel").gameObject;
        WrongActionMessagePanel = UICanvas.transform.Find("ActionMessagePanel/WrongActionMessagePanel").gameObject;
        ClockText = UICanvas.transform.Find("ClockPanel/ClockText").GetComponent<Text>();
        Audio = GetComponent<GameAudio>();

        if (NetworkServer.connections.Count == 1)
        {
            isSinglePlay = true;
            survivors = new GameObject[1];
            survivors[0] = GameObject.FindGameObjectsWithTag("Player")[0].gameObject;

            BigWildSpawnManager.StartBigWildSpawn();
            WriteMessageText("최대한 오래 살아남으세요!");
            StartClock();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("k"))
            ActionMessage("Right", "곰은 생존자의 아이템에 관심을 보입니다.", survivors[0]);
        if (Input.GetKeyDown("l"))
            ActionMessage("Wrong", "곰에게 등을 보였습니다.", survivors[0]);
        if(Input.GetKeyDown(";"))
            WriteMessageText("최대한 오래 살아남으세요!");
    }

    public void GameControllerInit()
    {
        GameObject[] survivorObject = GameObject.FindGameObjectsWithTag("Player");
        survivors = new GameObject[survivorObject.Length];

        for (int i = 0; i < survivorObject.Length; i++)
            survivors[survivorObject[i].GetComponent<SurvivorStatus>().PlayerID] = survivorObject[i];

        liveSurvivorNumber = survivors.Length;

        if (NetworkServer.connections.Count == liveSurvivorNumber)  //NetworkServer.connections.Count는 서버에서만 실행되므로 서버에서만 실행됨
        {
            Invoke("SendStartBigWildSpawnMsg", 1);
            Invoke("SendWriteStartingMessageTextMsg", 1);
            Invoke("SendStartClockMsg", 1);
        }
    }

    void SendStartBigWildSpawnMsg()
    {
        SendMsgManager.GetInstance().SendStartBigWildSpawnMsg("null");
    }

    public void UpdateBigWildSpawnText(int remainTime)
    {
        if (remainTime > 0)
        {
            UICanvas.transform.Find("BigWildSpawnPanel").GetComponent<Animator>().SetBool("FadeOut", false);
            BigWildSpawnText.text = remainTime + "초 후에 야생동물이 출현합니다!";
        }

        if (remainTime == -3)
        {
            UICanvas.transform.Find("BigWildSpawnPanel").GetComponent<Animator>().SetBool("FadeOut", true);
            //BigWildSpawnText.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else if (remainTime <= 0)
            BigWildSpawnText.text = "야생동물이 출현했습니다!";
        else if (!BigWildSpawnText.gameObject.transform.parent.gameObject.activeSelf)
            BigWildSpawnText.gameObject.transform.parent.gameObject.SetActive(true);
    }

    public void SurvivorDead(int playerID)
    {
        liveSurvivorNumber--;

        if (isSinglePlay || liveSurvivorNumber == 1)
        {
            isGameOver = true;
            GameOver();
        }

        if (!survivors[playerID].GetComponent<SurvivorStatus>().isLocalPlayer)
            WriteMessageText(survivors[playerID].GetComponent<SurvivorStatus>().PlayerName + "가 죽었습니다!");
    }

    public void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (Transform t in UICanvas.transform)
        {
            t.gameObject.SetActive(false);
        }
        UICanvas.transform.Find("SurvivorPanel").gameObject.SetActive(true);
        UICanvas.transform.Find("VignetteImage").gameObject.SetActive(true);
        UICanvas.transform.Find("ClockPanel").gameObject.SetActive(true);

        Text GameOverText = GameOverPanel.transform.Find("GameOverText").GetComponent<Text>();
        Text WinnerText = GameOverPanel.transform.Find("WinnerText").GetComponent<Text>();

        GameObject HelpPanel = GameOverPanel.transform.Find("HelpPanel").gameObject;
        Image HelpImage = GameOverPanel.transform.Find("HelpPanel/HelpImage").GetComponent<Image>();
        Text HelpText = GameOverPanel.transform.Find("HelpPanel/HelpText").GetComponent<Text>();

        GameOverText.text = "생존 실패...";
        WinnerText.text = "";
        HelpImage.sprite = null;
        HelpText.text = "";

        if (!isSinglePlay)
        {
            int localPlayerID = 0;
            int winnerPlayerID = 0;

            if (!isGameOver)
            {
                for (int i = 0; i < survivors.Length; i++)
                {
                    if (survivors[i].GetComponent<SurvivorStatus>().isLocalPlayer)
                        localPlayerID = i;
                }

                Audio.Play("GameOver");

                WinnerText.text = liveSurvivorNumber + "등 하셨습니다!";
                //HelpText.text = HelpMessage(survivors[localPlayerID].GetComponent<SurvivorStatus>().lastestDamagedObject);
                HelpImage.sprite = HelpMessage.GetHelpMessage(survivors[localPlayerID].GetComponent<SurvivorStatus>().lastestDamagedObject).HelpSprite;
                HelpText.text = HelpMessage.GetHelpMessage(survivors[localPlayerID].GetComponent<SurvivorStatus>().lastestDamagedObject).HelpMessage;
            }
            else
            {
                for (int i = 0; i < survivors.Length; i++)
                {
                    if (survivors[i].GetComponent<SurvivorStatus>().isLocalPlayer)
                        localPlayerID = i;
                    if (!survivors[i].GetComponent<SurvivorStatus>().IsDead())
                        winnerPlayerID = i;
                }

                if (localPlayerID == winnerPlayerID)
                {
                    Audio.Play("Win");

                    GameOverPanel.transform.Find("LoserImage").gameObject.SetActive(false);
                    HelpPanel.SetActive(false);

                    GameOverPanel.transform.Find("WinnerImage").gameObject.SetActive(true);
                    GameOverText.text = "생존 성공!";
                    WinnerText.text = "당신이 최후의 생존자 입니다!";
                }
                else
                {
                    Audio.Play("GameOver");

                    WinnerText.text = survivors[winnerPlayerID].GetComponent<SurvivorStatus>().playerName + "이 최후의 생존자 입니다!";
                    //HelpText.text = HelpMessage(survivors[localPlayerID].GetComponent<SurvivorStatus>().lastestDamagedObject);
                    HelpImage.sprite = HelpMessage.GetHelpMessage(survivors[localPlayerID].GetComponent<SurvivorStatus>().lastestDamagedObject).HelpSprite;
                    HelpText.text = HelpMessage.GetHelpMessage(survivors[localPlayerID].GetComponent<SurvivorStatus>().lastestDamagedObject).HelpMessage;
                }
            }
        }
        else
        {
            Audio.Play("GameOver");
            //HelpText.text = HelpMessage(survivors[0].GetComponent<SurvivorStatus>().lastestDamagedObject);
            HelpImage.sprite = HelpMessage.GetHelpMessage(survivors[0].GetComponent<SurvivorStatus>().lastestDamagedObject).HelpSprite;
            HelpText.text = HelpMessage.GetHelpMessage(survivors[0].GetComponent<SurvivorStatus>().lastestDamagedObject).HelpMessage;
        }

        GameOverPanel.SetActive(true);
    }

    public void ObserverMode()
    {
        StartCoroutine(ObserverModeCoroutine());
    }

    IEnumerator ObserverModeCoroutine()
    {
        int currentObserverID;

        for (currentObserverID = 0; currentObserverID < survivors.Length; currentObserverID++)
        {
            if (!survivors[currentObserverID].GetComponent<SurvivorStatus>().IsDead())
            {
                FreeLookCam.SetTarget(survivors[currentObserverID].transform);
                break;
            }
        }

        Text ObservingTargetText = UICanvas.transform.Find("ObserverModePanel/ObserverModeBarImage/ObservingTargetText").GetComponent<Text>();
        ObservingTargetText.text = survivors[currentObserverID].GetComponent<SurvivorStatus>().PlayerName + " 관전 중";
        UICanvas.transform.Find("ObserverModePanel").gameObject.SetActive(true);

        while (true)
        {
            if (Input.GetMouseButtonUp(0))
            {
                for (currentObserverID = currentObserverID + 1; ; currentObserverID++)
                {
                    currentObserverID %= survivors.Length;

                    if (!survivors[currentObserverID].GetComponent<SurvivorStatus>().IsDead())
                    {
                        FreeLookCam.SetTarget(survivors[currentObserverID].transform);
                        ObservingTargetText.text = survivors[currentObserverID].GetComponent<SurvivorStatus>().PlayerName + " 관전 중";
                        break;
                    }
                }
            }

            if (isGameOver) break;

            yield return null;
        }
    }

    public bool IsGameOver
    {
        get
        {
            return isGameOver;
        }
    }

    void SendWriteStartingMessageTextMsg()
    {
        SendMsgManager.GetInstance().SendWriteMessgeMsg("최후의 1인으로 살아남으세요!");
    }

    IEnumerator MessageCoroutine;

    public void WriteMessageText(string msg)
    {
        if (MessageCoroutine == null)
        {
            MessageCoroutine = ShowMessageText(msg);
            StartCoroutine(MessageCoroutine);
        }
        else
        {
            StopCoroutine(MessageCoroutine);
            MessageCoroutine = ShowMessageText(msg);
            StartCoroutine(MessageCoroutine);
        }
    }

    IEnumerator ShowMessageText(string msg)
    {
        Audio.Play("Warning");

        UICanvas.transform.Find("MessagePanel").GetComponent<Animator>().SetBool("FadeOut", false);
        MessageText.text = msg;
        MessageText.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        UICanvas.transform.Find("MessagePanel").GetComponent<Animator>().SetBool("FadeOut", true);
    }

    IEnumerator RightActionMessageCoroutine;
    IEnumerator WrongActionMessageCoroutine;

    public void ActionMessage(string type, string message, GameObject player)
    {
        if (player.GetComponent<SurvivorStatus>().isLocalPlayer)
            StartActionMessage(type, message);
        else
            SendMsgManager.GetInstance().SendActionMessageMsg(type, message, player);
    }

    public void StartActionMessage(string type, string message)
    {
        if (type.Equals("Right"))
        {
            if (RightActionMessageCoroutine == null)
            {
                RightActionMessageCoroutine = ShowRightActionMessage(message);
                StartCoroutine(RightActionMessageCoroutine);
            }
            else
            {
                StopCoroutine(RightActionMessageCoroutine);
                RightActionMessageCoroutine = ShowRightActionMessage(message);
                StartCoroutine(RightActionMessageCoroutine);
            }
        }
        else if (type.Equals("Wrong"))
        {
            if (WrongActionMessageCoroutine == null)
            {
                WrongActionMessageCoroutine = ShowWrongActionMessage(message);
                StartCoroutine(WrongActionMessageCoroutine);
            }
            else
            {
                StopCoroutine(WrongActionMessageCoroutine);
                WrongActionMessageCoroutine = ShowWrongActionMessage(message);
                StartCoroutine(WrongActionMessageCoroutine);
            }
        }
    }

    IEnumerator ShowRightActionMessage(string message)
    {
        Audio.Play("Right");
        RightActionMessagePanel.GetComponent<Animator>().SetBool("SlideIn", true);
        RightActionMessagePanel.transform.Find("MessageText").GetComponent<Text>().text = message;

        yield return new WaitForSeconds(3);
        RightActionMessagePanel.GetComponent<Animator>().SetBool("SlideIn", false);
    }

    IEnumerator ShowWrongActionMessage (string message)
    {
        Audio.Play("Wrong");
        WrongActionMessagePanel.GetComponent<Animator>().SetBool("SlideIn", true);
        WrongActionMessagePanel.transform.Find("MessageText").GetComponent<Text>().text = message;

        yield return new WaitForSeconds(3);
        WrongActionMessagePanel.GetComponent<Animator>().SetBool("SlideIn", false);
    }

    void SendStartClockMsg()
    {
        SendMsgManager.GetInstance().SendStartClockMsg();
    }

    public void StartClock()
    {
        StartCoroutine(Clock());
    }

    IEnumerator Clock()
    {
        gameTime = 0.5f;
        float min;
        float sec;

        while (!isGameOver)
        {
            gameTime += Time.deltaTime;

            min = Mathf.Floor(gameTime / 60);
            sec = Mathf.RoundToInt(gameTime % 60);

            ClockText.text = min.ToString("00") + ":" + sec.ToString("00");

            yield return null;
        }
    }

    public float GameTime
    {
        get
        {
            return gameTime;
        }
        set
        {
            gameTime = value;
        }
    }

}

