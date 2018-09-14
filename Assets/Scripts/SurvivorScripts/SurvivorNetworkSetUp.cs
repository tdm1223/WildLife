using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Cameras;


public class SurvivorNetworkSetUp : NetworkBehaviour
{
    static Color[] Colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };
    public Material[] material = new Material[6];

    [SyncVar]
    public int ID;

    [SyncVar]
    public string playerName;

    [SyncVar]
    public Color playerColor;

    void Start()
    {
        SurvivorInventory survivorInventory = GetComponent<SurvivorInventory>();
        survivorInventory.enabled = true;

        SurvivorStatus survivorStatus = GetComponent<SurvivorStatus>();
        survivorStatus.enabled = true;
        survivorStatus.PlayerID = ID;
        survivorStatus.PlayerName = playerName;
        gameObject.name = playerName;
        transform.Find("PlayerNameText").GetComponent<TextMesh>().text = playerName;

        if (GameController.GetInstance() != null)
            GameController.GetInstance().GameControllerInit();

        survivorStatus.uiSurvivor = GameObject.Find("UI/Canvas/SurvivorPanel").GetComponent<UISurvivor>();
        survivorStatus.uiSurvivor.SurvivorUIStart();

        SetClothColor(playerColor);
    }

    public override void OnStartLocalPlayer()
    {
        //transform.FindChild("PlayerNameText").gameObject.SetActive(false);    //자신의 이름은 안보이게

        GetComponent<SurvivorStatus>().uiStatus = GameObject.Find("UI/Canvas/StatusPanel").GetComponent<UIStatus>();

        SurvivorController survivorController = GetComponent<SurvivorController>();
        survivorController.enabled = true;
        survivorController.FreeLookCameraRig = GameObject.Find("Cameras/FreeLookCameraRig");
        survivorController.uiLookingItemText = GameObject.Find("UI/Canvas/LookingItemText").GetComponent<UILookingItemText>();

        GetComponent<SurvivorInventory>().enabled = true;
        GetComponent<SurvivorInventory>().uiInventory = GameObject.Find("UI/Canvas/InventoryPanel").GetComponent<UIInventory>();

        GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);

        Camera.main.transform.parent.GetComponentInParent<FreeLookCam>().SetTarget(transform);
    }

    public override void PreStartClient()
    {
        GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
    }

    public void SetClothColor(Color playerColor)
    {
        GameObject shopkeeper = transform.Find("Character_Shopkeeper").gameObject;
        Renderer rend = shopkeeper.transform.GetComponent<Renderer>();
        int idx = System.Array.IndexOf(Colors, playerColor);
        if (idx < 0) idx = 0;
        rend.material = material[idx];
    }
}