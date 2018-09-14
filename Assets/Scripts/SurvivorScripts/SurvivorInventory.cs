using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

public class SurvivorInventory : NetworkBehaviour
{
    public UIInventory uiInventory;
    int Max; //아이템창 개수를 제한한다. 
    int ActivationNum;
    int EmptySlotCount; //빈슬롯의 갯수 

    public GameObject Slot;//ItemSLot 스크립트를 컴포넌트로 갖는 Slot 
   
    private GameObject throwItem;   //실제 버리는 아이템 오브젝트

    List<GameObject> ItemList = new List<GameObject>();


    public List<GameObject> GetItemList()
    {
        return ItemList;
    }

    public int GetEmptySlotCount()
    {
        return EmptySlotCount;
    }

    public void ChangeEmptySlotcount(int count)
    {
        EmptySlotCount += count;
        // Debug.Log(EmptySlotCount);
    }

    void Start()
    {
        Max = 4; //초기 List크기값을 4로 초기화한다. 가방을 먹었을경우에 6으로 확장 시켜준다. 
        EmptySlotCount = 4;

        for (int i = 0; i < 6; i++)
        {
            GameObject slot = Instantiate(Slot) as GameObject;
            slot.transform.parent = transform.Find("slots");
            ItemList.Add(slot);
        }
    }

    void Update()
    {

    }
    public void ExtendMax()//가방 아이템을 먹었을경우
    {
        Max = 6;
    }
    public void ReduceMax()//가방 아이템을 버렸을경우 다시 최대창 원래대로 돌아옴 
    {
        Max = 4;
    }
    public bool GetBundleItem(Item item) //번들 체크 하는 함수
    {
        Item CurrentItem;
        ItemSlot CurrentSlot;
        for (int i = 0; i < Max; i++)
        {
            if (!ItemList[i].GetComponent<ItemSlot>().IsEmpty)
            {
                CurrentItem = getItem(i);
                CurrentSlot = getslot(i);
                if (CurrentItem.ItemName.Equals(item.ItemName) && CurrentSlot.IsBundleFull()) //번들이 꽉차면 일단 밴
                {
                    continue;
                }
                else if (CurrentItem.ItemName.Equals(item.ItemName) && !CurrentSlot.IsBundleFull()) //꽉차지 않은 자기자신의 번들이 있으면 트루반환
                {
                    return true;
                }
                else
                {
                    continue;
                }
            }
        }
        return false;
    }
    public void AddItem(Item item)
    {

        if (GetBundleItem(item)) //아이템이 번들일 경우 
        {
            //Debug.Log("인벤토리 번들");
            Item CurrentItem;
            ItemSlot CurrentItemSlot;

            for (int i = 0; i < Max; i++)
            {

                CurrentItem = getItem(i);

                CurrentItemSlot = getslot(i);

                if (CurrentItem == null)
                {
                    continue;
                }
                else if (CurrentItem.ItemName.Equals(item.ItemName) && !CurrentItemSlot.IsBundleFull())//번들이고 같은 슬롯안에 똑같은 이름이 있는 슬롯을 찾아서 use Count++
                {
                    //CurrentItem.UseCount++;//갯수 늘리고 
                    ControlUseCount(i, 1);
                    //item.SetEquip(false);
                    CmdDestroyItem(item.gameObject);
                    //item.transform.SetParent(CurrentItem.transform);//겹치는 아이템 자식 아래로 세팅

                    //item.transform.SetParent(CurrentItemSlot.transform);///////////////////////////////////////////////////////////////////

                    //Debug.Log("번들 하나 증가");

                    break;

                }
                // }
            }
        }

        else if (!GetBundleItem(item)) //아이템 번들이 꽉차있거나 처음 먹는 경우 
        {
            for (int i = 0; i < Max + 1; i++)
            {


                if (ItemList[i].GetComponent<ItemSlot>().IsEmpty) //해당 슬롯이 비어있으면
                {
                    if (item.Kind == 101) //가방이면 슬롯에 넣지않고 아이템 확장
                    {
                        ExtendInventory();

                        ChangeEmptySlotcount(2);

                        uiInventory.SendMessage("ExtendInventory", null, SendMessageOptions.DontRequireReceiver); //인벤토리 UI에 가방 먹은걸 알림

                        SendMsgManager.GetInstance().EquipItem(item.gameObject);

                        Debug.Assert(EmptySlotCount <= Max, "슬롯갯수가 Max 보다 커짐 씨발");
                        break;
                    }
                    else
                    {
                      
                        var slot = ItemList[i].GetComponent<ItemSlot>();

                        slot.AddItem(item);
                        ChangeEmptySlotcount(-1);

                        item.Passive(); //먹을때 발생하는 효과 발동 

                        object[] parameter = new object[3];
                        parameter[0] = i;
                        parameter[1] = item.ItemName;
                        parameter[2] = item.ItemIcon;
                        uiInventory.SendMessage("AddItem", parameter, SendMessageOptions.DontRequireReceiver); //인벤토리 UI에 아이템 먹은걸 알림
                        ControlUseCount(i, 0);
                        Debug.Assert(EmptySlotCount > -1, "슬롯갯수가 음수가됨 씨발");
                        //Debug.Log("남은슬롯갯수" + EmptySlotCount);
                        break;
                    }

                }
            }
        }
    }

    [Command]
    void CmdDestroyItem (GameObject item)
    {
        NetworkServer.Destroy(item.gameObject);
    }
    public int GetMax()
    {
        return Max;
    }
    private void ExtendInventory()
    {
        Max = 6;
    }
    private void ReducetInventory()
    {
        Max = 4;
    }

    public void ThrowItem(int pos)
    {
        Item item = getItem(pos);
        ItemSlot Slot = getslot(pos);
        var UseCount = item.useCount;
        var Owner = item.Owner;

        if ((item.Kind / 100 == 1))
        {
            //SendMsgManager.GetInstance().SetNetworkTransformFalse(item.gameObject, true);
            RemoveItem(pos);
            SendMsgManager.GetInstance().TFTrue(item.gameObject);

            //item.UnHold();
            SpawnObject(item.gameObject, Owner.transform.position + Owner.transform.forward + Vector3.up, gameObject.name);

            CmdDestroyItem(item.gameObject);
        }
        else if (UseCount > 1)
        {
            //SendMsgManager.GetInstance().SetNetworkTransformFalse(item.gameObject, true);
            //Transform child = Slot.transform.GetChild((UseCount - 2));
            ControlUseCount(pos, -1);
            SendMsgManager.GetInstance().TFTrue(item.gameObject);

            //item.UnHold();
            SpawnObject(item.gameObject, Owner.transform.position + Owner.transform.forward + Vector3.up, gameObject.name);
            SendMsgManager.GetInstance().UnEquipItem(item.gameObject);
         
        }
        else if (UseCount < 2) // 번들이 아니거나 번들의 마지막 아이템이면 버린다. 
        {
            //SendMsgManager.GetInstance().SetNetworkTransformFalse(item.gameObject, true);
            ControlUseCount(pos, -1);
            RemoveItem(pos);
           
            SendMsgManager.GetInstance().TFTrue(item.gameObject);
            //item.UnHold();
            
            SpawnObject(item.gameObject, Owner.transform.position + Owner.transform.forward + Vector3.up, gameObject.name);
            CmdDestroyItem(item.gameObject);
 
        }
    }

    //[Command]
    //void CmdSpawnItem(GameObject item)
    //{
    //    NetworkServer.Spawn(item);
    //}

    public void SpawnObject(GameObject item, Vector3 position, string name)
    {
        CmdSpawnItem(item, position, name);
    }

    [Command]
    void CmdSpawnItem(GameObject item, Vector3 position, string name)
    {
        GameObject Spawnitem = (GameObject)Instantiate(item, position, Quaternion.identity);
        NetworkServer.Spawn(Spawnitem);
        RpcSetthrowitem(Spawnitem, gameObject.name);
        throwItem = Spawnitem;
        var compo = Spawnitem.GetComponent<Item>();

        compo.SetEquip(false);
        compo.OnField = true;
        compo.useCount = Spawnitem.GetComponent<Item>().useCount;
        compo.maxCount = Spawnitem.GetComponent<Item>().maxCount;
    }

    [ClientRpc]
    void RpcSetthrowitem(GameObject item, string name)
    {
        if (gameObject.name.Equals(name))
        {
           // Debug.Log("RpcSetthrowitem set 이야!!");
            throwItem = item;
        }
        if (throwItem == null)
        {
           // Debug.Log("RpcSetthrowitem null 이야!!");
        }
    }

    public void ItemUse(int pos)
    {
        Item CurrentItem = getItem(pos);
       
        var slot = getslot(pos);
        
        string slotName = slot.Item.ItemName;
        CheckLastItemFlag = CheckLastItem(slotName);


        CurrentItem.Use(pos);


        if ((CurrentItem.Kind == 102) && (GetComponent<SurvivorStatus>().UmbrellaState == true))
        {               //우산은 카운트 주는 케이스가 다르기 때문에 필때만 줄어든다
            if (CurrentItem.useCount > 1)
            {
                ControlUseCount(pos, -1);
                CurrentItem.maxCount = CurrentItem.useCount;
            }
            else if (CurrentItem.useCount == 1)
            {
                StartCoroutine(UmbrellaDestroyDelay(pos, CurrentItem));
            }
            //Debug.Log("현재 맥스카운트" + CurrentItem.maxCount);
        }
        else if (CurrentItem.Kind != 102 && CurrentItem.Kind != 103)
        {
            ControlUseCount(pos, -1); //여기서 갯수 하나 줄이고  
        }


        if (CurrentItem.useCount == 0)
        {
            RemoveItem(pos);

            if (CurrentItem.Kind != 105)
            {
                CmdDestroyItem(CurrentItem.gameObject);
            }
        }
    }
    private bool checkLastItemFlag;
    public bool CheckLastItemFlag
    {
        get
        {
            return checkLastItemFlag;
        }
        set
        {
            checkLastItemFlag=value;
        }
    }
    public void RemoveItem(int pos)
    {
        var item = getItem(pos);
        var slot = getslot(pos);
        ChangeEmptySlotcount(1);
        string slotName = slot.Item.ItemName;
        slot.RemoveItem();
        CheckLastItemFlag = CheckLastItem(slotName);
        if (CheckLastItemFlag)
        {
            item.DePassive();  
        }
        uiInventory.SendMessage("RemoveItem", pos, SendMessageOptions.DontRequireReceiver);

    }
    private bool CheckLastItem(string name)
    {
        for (int i = 0; i < Max; i++)
        {
            if (!ItemList[i].GetComponent<ItemSlot>().IsEmpty)
            {
                var remainsItem = getItem(i);
                if (remainsItem.ItemName.Equals(name))
                {
                    return false;
                }
                else
                {
                    return true;
                }
                    
            }
        }
        return true;
    }
    public bool isNotNull(int pos)
    {
        if (ItemList[pos].GetComponent<ItemSlot>().IsEmpty) //선택한 아이템창이 비어있지않으면 
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public Item getItem(int pos)
    {
        return ItemList[pos].GetComponent<ItemSlot>().Item;
    }
    public ItemSlot getslot(int pos)
    {
        return ItemList[pos].GetComponent<ItemSlot>();
    }

    public void ControlUseCount(int pos, int i)
    {
        Item CurrentItem = getItem(pos);
        CurrentItem.useCount += i;
        object[] parameter = new object[2];
        parameter[0] = pos;
        parameter[1] = CurrentItem.useCount;
        uiInventory.SendMessage("SetBundleCount", parameter, SendMessageOptions.DontRequireReceiver);
    }
    public GameObject GetThrowItem()
    {
        return throwItem;
    }

    IEnumerator UmbrellaDestroyDelay(int pos, Item CurrentItem) //우산 마지막 필 때 없어지는 딜레이
    {
        yield return new WaitForSeconds(1.5f);
        GetComponent<SurvivorController>().UmbrellaState = false;
        RemoveItem(pos);
        CmdDestroyItem(CurrentItem.gameObject);
    }
}
