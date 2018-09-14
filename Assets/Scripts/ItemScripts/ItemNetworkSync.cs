using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemNetworkSync : NetworkBehaviour {

    [SyncVar] public string ItemID;
    string ItemName;

    private Transform myTransform;
    private NetworkIdentity myNetId;

    void Start()
    {
        myTransform = transform;
        ItemName = GetComponent<Item>().ItemName;
    }

    void Update()
    {
        if (myTransform.name == "" || myTransform.name == ItemName + "(Clone)" || myTransform.name == ItemName)
        {
            myTransform.name = ItemID;
        }
    }
}
