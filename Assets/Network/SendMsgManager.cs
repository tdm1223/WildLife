using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using Prototype.NetworkLobby;

public class SendMsgManager : NetworkBehaviour
{

    const short PickUpItemMsg = 1001;
    const short ThrowItemMsg = 1002;
    const short SurvivorUIMsg = 1003;
    const short SurvivorDeadMsg = 1004;
    const short SendPepperSprayMsgIndex = 1005;

    const short SendEquipItemMsg = 1007;
    const short SendUnEquipItemMsg = 1008;

    const short StartBigWildSpawnMsg = 1009;
    const short SendBearAttentionMsg = 1011;
    const short MessageMsg = 1014;
    const short ActionMessageMsg = 1016;
    const short StartClockMsg = 1015;

    const short AudioPlayMsg = 1012;
    const short AudioStopMsg = 1013;

    const short TrapMsg = 1018;
    const short UmbrellaAnimSyncMsg = 1019;

    const short BeeDestroyMsg = 1020;
    const short SetParnetMsg = 1021;
    const short SpawnParticleMsg = 1022;
    const short SetNetworkTransformMsg = 1023;
    const short TFTruemsg = 1024;

    NetworkClient m_client;
    //SendToServerMsg NetworkSingleton;
    NetworkClient client;

    private static SendMsgManager instance;

    public static SendMsgManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(SendMsgManager)) as SendMsgManager;
            if (!instance)
            {
                Debug.LogError("Hierarchy에 SendToServerMsg가 없음");
            }
        }
        return instance;
    }

    void Start()
    {
        //NetworkSingleton = new SendToServerMsg();
        Init();
    }

    public void Init()
    {
        NetworkManager netManager = FindObjectOfType<NetworkManager>();
        m_client = netManager.client;   //Local Client

        if (isServer)
        {
            NetworkServer.RegisterHandler(PickUpItemMsg, OnServerPickUpItem);
            NetworkServer.RegisterHandler(ThrowItemMsg, OnClientThrowItem);
            NetworkServer.RegisterHandler(SurvivorUIMsg, OnServerSurvivorUI);
            NetworkServer.RegisterHandler(SurvivorDeadMsg, OnServerSurvivorDead);
            NetworkServer.RegisterHandler(SendPepperSprayMsgIndex, SendPepperSprayTarget);

            NetworkServer.RegisterHandler(SendEquipItemMsg, OnServerEquipItem);
            NetworkServer.RegisterHandler(SendUnEquipItemMsg, OnServerUnEquipItem);

            //NetworkServer.RegisterHandler(StartBigWildSpawnMsg, StartBigWildSpawn);
            NetworkServer.RegisterHandler(MessageMsg, WriteMessageMsg);
            //NetworkServer.RegisterHandler(StartClockMsg, StartClock);

            NetworkServer.RegisterHandler(SendBearAttentionMsg, OnServerSetBearAttention);

            NetworkServer.RegisterHandler(AudioPlayMsg, PlayAudio);
            NetworkServer.RegisterHandler(AudioStopMsg, StopAudio);

            NetworkServer.RegisterHandler(TrapMsg, OnServerLocateTrap);
            NetworkServer.RegisterHandler(UmbrellaAnimSyncMsg, OnServerUmbrellaAnimSync);

            NetworkServer.RegisterHandler(BeeDestroyMsg, OnServerDestroyBee);
            NetworkServer.RegisterHandler(SpawnParticleMsg, OnServerSpawnParticle);

            NetworkServer.RegisterHandler(SetNetworkTransformMsg, OnsServerSetNetworkTransformFalse);
            NetworkServer.RegisterHandler(TFTruemsg, OnServerTFTrue);
        }
        else
        {
            m_client.RegisterHandler(PickUpItemMsg, OnClientPickUpItem);
            m_client.RegisterHandler(SurvivorUIMsg, OnClientSurvivorUI);
            m_client.RegisterHandler(SurvivorDeadMsg, OnClientSurvivorDead);
            m_client.RegisterHandler(SendPepperSprayMsgIndex, OnClientPepper);

            m_client.RegisterHandler(SendEquipItemMsg, OnClientEquipItem);
            m_client.RegisterHandler(SendUnEquipItemMsg, OnClientUnEquipItem);

            m_client.RegisterHandler(StartBigWildSpawnMsg, StartBigWildSpawn);
            m_client.RegisterHandler(MessageMsg, WriteMessageMsg);
            m_client.RegisterHandler(ActionMessageMsg, ShowActionMessage);
            m_client.RegisterHandler(StartClockMsg, StartClock);

            m_client.RegisterHandler(AudioPlayMsg, PlayAudio);
            m_client.RegisterHandler(AudioStopMsg, StopAudio);

            m_client.RegisterHandler(TrapMsg, OnClientLocateTrap);
            m_client.RegisterHandler(UmbrellaAnimSyncMsg, OnClientUmbrellaAnimSync);

            m_client.RegisterHandler(SetParnetMsg, OnClientParentBeeFlock);
            m_client.RegisterHandler(SetNetworkTransformMsg, OnsClientSetNetworkTransformFalse);
            m_client.RegisterHandler(TFTruemsg, OnClientTFFalse);
        }
    }


    public void TFTrue(GameObject item)
    {
        ItemObjectMsg msg = new ItemObjectMsg();
        msg.item = item;

        m_client.Send(TFTruemsg, msg);
    }


    void OnServerTFTrue(NetworkMessage recvmsg)
    {
        var msg = recvmsg.ReadMessage<ItemObjectMsg>();
        var item = msg.item;
        item.GetComponent<Item>().TFTrue();

        NetworkServer.SendToAll(TFTruemsg, msg);
    }

    void OnClientTFFalse(NetworkMessage recvmsg)
    {
        var msg = recvmsg.ReadMessage<ItemObjectMsg>();
        var item = msg.item;


        item.GetComponent<Item>().TFTrue();
    }






    class NetworkTransformmsg : MessageBase
    {
        public GameObject target;
        public bool value;
    }





    public void SetNetworkTransformFalse(GameObject obj, bool value)

    {
        NetworkTransformmsg msg = new NetworkTransformmsg();
        msg.target = obj;
        msg.value = value;
        m_client.Send(SetNetworkTransformMsg, msg);
    }

    void OnsServerSetNetworkTransformFalse(NetworkMessage rcvmsg)
    {
        var msg = rcvmsg.ReadMessage<NetworkTransformmsg>();
        var target = msg.target;
        var value = msg.value;

        target.GetComponent<NetworkTransform>().enabled = value;    //Null 오류 라인
        NetworkServer.SendToAll(SetNetworkTransformMsg, msg);
    }
    void OnsClientSetNetworkTransformFalse(NetworkMessage rcvmsg)
    {
        var msg = rcvmsg.ReadMessage<NetworkTransformmsg>();
        var target = msg.target;
        var value = msg.value;

        target.GetComponent<NetworkTransform>().enabled = value;
    }






    class ParticleObj : MessageBase
    {
        public GameObject obj;
        public Vector3 point;
        public Quaternion q;
    }

    public void SpawnParticle(GameObject obj, Vector3 point, Quaternion q)
    {
        ParticleObj msg = new ParticleObj();
        msg.obj = obj;
        msg.point = point;
        msg.q = q;

        m_client.Send(SpawnParticleMsg, msg);
    }

    public void OnServerSpawnParticle(NetworkMessage rcvmsg)
    {
        var msg = rcvmsg.ReadMessage<ParticleObj>();
        GameObject effect = Instantiate(msg.obj, msg.point, msg.q);

        NetworkServer.Spawn(effect);
    }

    public void DestroyBee(GameObject bee)
    {
        GameObjectMsg msg = new GameObjectMsg();

        msg.TargetObject = bee;


        m_client.Send(BeeDestroyMsg, msg);
    }


    void OnServerDestroyBee(NetworkMessage rcvmsg)
    {

        var msg = rcvmsg.ReadMessage<GameObjectMsg>();
        if (msg.TargetObject != null)
        {

            msg.TargetObject.transform.GetComponent<Bee>().beeRespawn.transform.GetComponent<BeeRespawn>().beeCount++;
            NetworkServer.Destroy(msg.TargetObject);
        }
    }

    public class FamillyMsg : MessageBase
    {
        public GameObject ParentObject;
        public GameObject ChildObject;

    }


    public void SetParentBeeFlock(GameObject BeeFlock, GameObject Bee)
    {
        FamillyMsg msg = new FamillyMsg();
        msg.ParentObject = BeeFlock;
        msg.ChildObject = Bee;

        NetworkServer.SendToAll(SetParnetMsg, msg);
    }

    public void OnClientParentBeeFlock(NetworkMessage rcvmsg)
    {
        var msg = rcvmsg.ReadMessage<FamillyMsg>();
        var parent = msg.ParentObject;
        var child = msg.ChildObject;

        child.transform.SetParent(parent.transform);
    }







    public class TrapObjectMsg : MessageBase

    {
        public GameObject target;
        public Vector3 pos;
    }




    public void LocateTrap(GameObject target, Vector3 position)
    {
        TrapObjectMsg msg = new TrapObjectMsg();
        msg.target = target;
        msg.pos = position;
        m_client.Send(TrapMsg, msg);

    }


    void OnServerLocateTrap(NetworkMessage recvmsg)
    {

        TrapObjectMsg msg = recvmsg.ReadMessage<TrapObjectMsg>();
        msg.target.GetComponent<Trap>().isLocated = true;

        //msg.target.transform.localRotation.SetLookRotation(new Vector3(0,1,0),msg.pos + Vector3.up);
        msg.target.transform.SetPositionAndRotation(msg.pos, Quaternion.identity);

        var compo = msg.target.GetComponent<Item>();
        compo.SetEquip(false);

        msg.target.transform.SetParent(null);

        msg.target.GetComponent<Rigidbody>().isKinematic = true;

        NetworkServer.SendToAll(TrapMsg, msg);
    }

    void OnClientLocateTrap(NetworkMessage recvmsg)
    {
        TrapObjectMsg msg = recvmsg.ReadMessage<TrapObjectMsg>();
        msg.target.GetComponent<Trap>().isLocated = true;
        msg.target.transform.SetPositionAndRotation(msg.pos, Quaternion.identity);
        msg.target.GetComponent<Item>().SetEquip(false);
        msg.target.transform.SetParent(null);
        msg.target.GetComponent<Rigidbody>().isKinematic = true;
    }

    public class UmbrellaObjectMsg : MessageBase
    {
        public GameObject umbrella;
        public bool state;
    }

    public void SendUmbrellaAnimSyncMsg(GameObject umbrella, bool state)
    {
        UmbrellaObjectMsg msg = new UmbrellaObjectMsg();
        msg.umbrella = umbrella;
        msg.state = state;

        if (m_client.isConnected)
        {
            if (isServer)
            {
                umbrella.GetComponent<Animator>().SetBool("UmbrellaOn", state);
                NetworkServer.SendToAll(UmbrellaAnimSyncMsg, msg);
            }
            else
            {
                m_client.Send(UmbrellaAnimSyncMsg, msg);
            }
        }
    }

    void OnServerUmbrellaAnimSync(NetworkMessage recvmsg)
    {
        UmbrellaObjectMsg msg = recvmsg.ReadMessage<UmbrellaObjectMsg>();
        SendUmbrellaAnimSyncMsg(msg.umbrella, msg.state);
    }

    void OnClientUmbrellaAnimSync(NetworkMessage recvmsg)
    {
        UmbrellaObjectMsg msg = recvmsg.ReadMessage<UmbrellaObjectMsg>();
        msg.umbrella.GetComponent<Animator>().SetBool("UmbrellaOn", msg.state);
    }

    public class ItemObjectMsg : MessageBase
    {
        public GameObject item;
        public GameObject owner;
    }

    public void SendPickUpItemMsg(GameObject item, GameObject owner)
    {
        ItemObjectMsg msg = new ItemObjectMsg();
        msg.item = item;
        msg.owner = owner;

        if (m_client.isConnected)
        {
            m_client.Send(PickUpItemMsg, msg);
        }
    }

    void OnServerPickUpItem(NetworkMessage recvmsg)
    {
        ItemObjectMsg msg = recvmsg.ReadMessage<ItemObjectMsg>();
        Item item = msg.item.GetComponent<Item>();
        GameObject owner = msg.owner;

        item.Owner = owner;

        Transform[] children = owner.GetComponentsInChildren<Transform>();
        string equipTag = item.getEquipTag();
        foreach (Transform child in children)
        {
            if (child.CompareTag(equipTag))
            {
                item.setEquipPoint(child);
            }
        }

        item.OnField = false;
        item.gameObject.transform.SetPositionAndRotation(transform.position + Vector3.down * 5f, Quaternion.identity);
        NetworkServer.SendToAll(PickUpItemMsg, msg);
    }

    void OnClientPickUpItem(NetworkMessage recvmsg)
    {
        ItemObjectMsg msg = recvmsg.ReadMessage<ItemObjectMsg>();
        Item item = msg.item.GetComponent<Item>();
        GameObject owner = msg.owner;

        item.Owner = owner;

        Transform[] children = owner.GetComponentsInChildren<Transform>();
        string equipTag = item.getEquipTag();
        foreach (Transform child in children)
        {
            if (child.CompareTag(equipTag))
            {
                item.setEquipPoint(child);
            }
        }

        item.OnField = false;
        item.gameObject.transform.SetPositionAndRotation(transform.position + Vector3.down * 5f, Quaternion.identity);

    }

    public void SendThrowItemMsg(GameObject item, GameObject owner)
    {
        ItemObjectMsg msg = new ItemObjectMsg();
        msg.item = item;
        msg.owner = owner;

        if (m_client.isConnected)
        {
            m_client.Send(ThrowItemMsg, msg);
        }
    }

    void OnClientThrowItem(NetworkMessage recvmsg)
    {
        ItemObjectMsg msg = recvmsg.ReadMessage<ItemObjectMsg>();
        Item item = msg.item.GetComponent<Item>();
        GameObject owner = msg.owner;

        item.OnField = true;
        item.transform.SetPositionAndRotation(owner.transform.position + owner.transform.forward + Vector3.up, Quaternion.identity);
    }

    public void SendToServerSurvivorUIMsg(string str)
    {
        StringMessage msg = new StringMessage(str);

        if (m_client.isConnected)
            m_client.Send(SurvivorUIMsg, msg);
    }

    void OnServerSurvivorUI(NetworkMessage recvmsg)
    {
        GameObject.Find("UI/Canvas/SurvivorPanel").GetComponent<UISurvivor>().SurvivorUIUpdate();
        NetworkServer.SendToAll(SurvivorUIMsg, recvmsg.ReadMessage<StringMessage>());
    }

    void OnClientSurvivorUI(NetworkMessage recvmsg)
    {
        GameObject.Find("UI/Canvas/SurvivorPanel").GetComponent<UISurvivor>().SurvivorUIUpdate();
    }

    public void SendToServerSurvivorDeadMsg(string str)
    {
        StringMessage msg = new StringMessage(str);

        if (m_client.isConnected)
            m_client.Send(SurvivorDeadMsg, msg);
    }

    void OnServerSurvivorDead(NetworkMessage recvmsg)
    {
        StringMessage msg = recvmsg.ReadMessage<StringMessage>();
        int playerID = int.Parse(msg.value);

        GameController.GetInstance().SurvivorDead(playerID);
        NetworkServer.SendToAll(SurvivorDeadMsg, msg);
    }

    void OnClientSurvivorDead(NetworkMessage recvmsg)
    {
        int playerID = int.Parse(recvmsg.ReadMessage<StringMessage>().value);

        GameController.GetInstance().SurvivorDead(playerID);
    }


    /// <summary>
    /// ////////////////////////////////////////페퍼스프레이
    /// </summary>
    /// 

    public class GameObjectMsg : MessageBase
    {
        public GameObject TargetObject;
    }

    public void PepperUse(GameObject target)
    {
        GameObjectMsg msg = new GameObjectMsg();
        msg.TargetObject = target;

        if (m_client.isConnected)
        {
            m_client.Send(SendPepperSprayMsgIndex, msg);
        }
    }

    public void SendPepperSprayTarget(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<GameObjectMsg>();

        var BWStates = msg.TargetObject.GetComponent<BigWildFSM>();

        if (BWStates.BigWildState != "Idle")
        {
            BWStates.BigWildState = "Idle";
            if (BWStates.GetOneTarget() != null)
                BWStates.GetOneTarget().GetComponent<SurvivorStatus>().SetBW(null);
            BWStates.LookTarget = null;
            BWStates.ChaseTarget = null;
            BWStates.GetComponent<Bear>().SendMessage("SetBackMoveFlag", true);
        }


        NetworkServer.SendToAll(SendPepperSprayMsgIndex, msg);
    }

    public void OnClientPepper(NetworkMessage netMsg)
    {
        Debug.Log("페퍼스프레이 클라이언트에서 호출");
        var msg = netMsg.ReadMessage<GameObjectMsg>();

        var BWStates = msg.TargetObject.GetComponent<BigWildFSM>();

        if (BWStates.BigWildState != "Idle")
        {
            BWStates.BigWildState = "Idle";
            BWStates.LookTarget = null;
            BWStates.ChaseTarget = null;
            BWStates.GetComponent<Bear>().SendMessage("SetBackMoveFlag", true);
            if (BWStates.GetOneTarget() != null)
                BWStates.GetOneTarget().GetComponent<SurvivorStatus>().SetBW(null);
        }

        Debug.Log(msg.TargetObject.name + "페퍼 스프레이 싱크!!");
    }
    //////////////////////////////////////////////////////////////////////////////////////아이템 장착 노장착
    public void EquipItem(GameObject item)
    {
        ItemObjectMsg msg = new ItemObjectMsg();
        msg.item = item;

        m_client.Send(SendEquipItemMsg, msg);
    }


    void OnServerEquipItem(NetworkMessage recvmsg)
    {
        var msg = recvmsg.ReadMessage<ItemObjectMsg>();
        var item = msg.item;
        item.GetComponent<Item>().Hold();

        NetworkServer.SendToAll(SendEquipItemMsg, msg);
    }

    void OnClientEquipItem(NetworkMessage recvmsg)
    {
        var msg = recvmsg.ReadMessage<ItemObjectMsg>();
        var item = msg.item;
        var owner = msg.owner;

        item.GetComponent<Item>().Hold();
    }

    public void UnEquipItem(GameObject item)
    {
        ItemObjectMsg msg = new ItemObjectMsg();
        msg.item = item;

        m_client.Send(SendUnEquipItemMsg, msg);
    }


    void OnServerUnEquipItem(NetworkMessage recvmsg)
    {
        var msg = recvmsg.ReadMessage<ItemObjectMsg>();
        var item = msg.item;
        item.GetComponent<Item>().UnHold();

        NetworkServer.SendToAll(SendUnEquipItemMsg, msg);
    }

    void OnClientUnEquipItem(NetworkMessage recvmsg)
    {
        var msg = recvmsg.ReadMessage<ItemObjectMsg>();
        var item = msg.item;
        var owner = msg.owner;

        item.GetComponent<Item>().UnHold();
    }

    public void SendStartBigWildSpawnMsg(string str)
    {
        StringMessage msg = new StringMessage(str);

        if (m_client.isConnected)
            NetworkServer.SendToAll(StartBigWildSpawnMsg, msg);
        GameController.GetInstance().BigWildSpawnManager.StartBigWildSpawn();
    }

    void StartBigWildSpawn(NetworkMessage recvmsg)
    {
        GameController.GetInstance().BigWildSpawnManager.StartBigWildSpawn();
    }

    public void SendWriteMessgeMsg(string str)
    {
        StringMessage msg = new StringMessage(str);

        if (m_client.isConnected)
            NetworkServer.SendToAll(MessageMsg, msg);
        GameController.GetInstance().WriteMessageText(str);
    }

    void WriteMessageMsg(NetworkMessage recvmsg)
    {
        string str = recvmsg.ReadMessage<StringMessage>().value;

        GameController.GetInstance().WriteMessageText(str);
    }

    public class ActionMsg : MessageBase
    {
        public string type;
        public string message;
    }

    public void SendActionMessageMsg(string type, string message, GameObject player)
    {
        ActionMsg msg = new ActionMsg();
        msg.type = type;
        msg.message = message;

        if (m_client.isConnected)
            NetworkServer.SendToClient(player.GetComponent<NetworkIdentity>().connectionToClient.connectionId, ActionMessageMsg, msg);
    }

    void ShowActionMessage(NetworkMessage recvmsg)
    {
        var msg = recvmsg.ReadMessage<ActionMsg>();
        string type = msg.type;
        string message = msg.message;

        GameController.GetInstance().StartActionMessage(type, message);
    }

    public void SendStartClockMsg()
    {
        StringMessage msg = new StringMessage();

        if (m_client.isConnected)
            NetworkServer.SendToAll(StartClockMsg, msg);
        GameController.GetInstance().StartClock();
    }

    void StartClock(NetworkMessage recvmsg)
    {
        GameController.GetInstance().StartClock();
    }


    /// /////////////////////////////////////////////////
    public class GameObjectDouble : MessageBase
    {
        public GameObject target;
        public GameObject item;
    }

    public void BearAttention(GameObject target, GameObject item)
    {
        GameObjectDouble msg = new GameObjectDouble();
        msg.target = target;
        msg.item = item;

        if (m_client.isConnected)
        {
            m_client.Send(SendBearAttentionMsg, msg);
        }
    }

    public void OnServerSetBearAttention(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<GameObjectDouble>();

        var bear = msg.target;
        var item = msg.item;

        bear.GetComponent<Bear>().AttentionItem(item);
    }

    // Audio

    public class AudioMsg : MessageBase
    {
        public GameObject owner;
        public string audioName;
        public int index;
    }

    public void SendAudioPlayMsg(GameObject owner, string audioName, int index)
    {
        AudioMsg msg = new AudioMsg();
        msg.owner = owner;
        msg.audioName = audioName;
        msg.index = index;

        if (m_client.isConnected)
            NetworkServer.SendToAll(AudioPlayMsg, msg);

        msg.owner.GetComponent<ObjectAudio>().StartAudioSource(msg.audioName, msg.index);
    }

    public void SendAudioPlayMsg(GameObject owner, string audioName, int index, GameObject Player)
    {
        AudioMsg msg = new AudioMsg();
        msg.owner = owner;
        msg.audioName = audioName;
        msg.index = index;

        if (m_client.isConnected)
        {
            if (isServer)
            {
                if (Player.GetComponent<SurvivorStatus>().isLocalPlayer)
                    msg.owner.GetComponent<ObjectAudio>().StartAudioSource(msg.audioName, msg.index);
                else
                    NetworkServer.SendToClient(Player.GetComponent<NetworkIdentity>().connectionToClient.connectionId, AudioPlayMsg, msg);
            }
            else
                Debug.Log("isClient");
        }
    }

    void PlayAudio(NetworkMessage recvmsg)
    {
        AudioMsg msg = recvmsg.ReadMessage<AudioMsg>();

        msg.owner.GetComponent<ObjectAudio>().StartAudioSource(msg.audioName, msg.index);
    }

    public void SendAudioStopMsg(GameObject owner, string audioName, int index)
    {
        AudioMsg msg = new AudioMsg();
        msg.owner = owner;
        msg.audioName = audioName;
        msg.index = index;

        if (m_client.isConnected)
            NetworkServer.SendToAll(AudioStopMsg, msg);

        msg.owner.GetComponent<ObjectAudio>().EndAudioSource(msg.audioName, msg.index);
    }

    void StopAudio(NetworkMessage recvmsg)
    {
        AudioMsg msg = recvmsg.ReadMessage<AudioMsg>();

        msg.owner.GetComponent<ObjectAudio>().EndAudioSource(msg.audioName, msg.index);
    }
}
