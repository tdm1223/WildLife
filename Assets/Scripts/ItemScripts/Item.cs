using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Item : NetworkBehaviour
{
    protected string itemName;
    private int kind;

    public int useCount;
    public int maxCount;

    protected string EquipTag; //아이템 마다 장착 포인트 태그가 달라야 한다. 

    public Transform EquipPoint;
    public Sprite ItemIcon;

    private bool onField;
    protected GameObject owner; //아이템의 주인을 저장해놓는다. 

    public GameObject Prefab;
    protected ItemAudioPlayer Audio;

	public float passiveRadius;
	public float usedRadius;

    public virtual void Start()
    {
        OnField = true; 
        useCount = 1;

        Audio = GetComponent<ItemAudioPlayer>();
    }
    public virtual void Use(int pos)
    {
        if (Audio != null)
            Audio.Play(owner, "Use");
    }
    public virtual void Passive() { }
    public virtual void DePassive() { }

    public virtual void Hold()
    {
        GetComponent<NetworkTransform>().enabled = false;
        SetEquip(true); //모든 콜라이더 해제    
        Equip(EquipPoint);    
    }
    public virtual void UnHold()
    {
        SetEquip(false);//모든 콜라이더 활성화 
        transform.SetParent(null);
        //GetComponent<NetworkTransform>().enabled = true;
        transform.SetPositionAndRotation(transform.position + Vector3.down * 5f, Quaternion.identity); //아래에 숨기고 
    }

    public void TFTrue()
    {
        SetEquip(false);//모든 콜라이더 활성화 
        transform.SetParent(null);
        GetComponent<NetworkTransform>().enabled = true;
        transform.SetPositionAndRotation(transform.position + Vector3.down * 5f, Quaternion.identity); //아래에 숨기고 
    }

    protected virtual void GetItem(SurvivorController itemOwner)
    {
        if (OnField)
        {
            Owner = itemOwner.gameObject; // 아이템의 소유자를 지정한다. 
         
            Transform[] children = Owner.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.CompareTag(EquipTag))
                {
                    EquipPoint = child;
                }
            }

            if ((kind == 101 && Owner.GetComponent<SurvivorInventory>().GetMax() < 6))
            {
                GameController.GetInstance().ItemSpawnManager.ReduceItemNumber();
                if (Audio != null)
                    Audio.Play(owner, "PickUp");

                SendMsgManager.GetInstance().SendPickUpItemMsg(gameObject, Owner.gameObject);
                itemOwner.Pickup(this);

            }
            else if (((itemOwner.GetEmptySLotBool() || Owner.GetComponent<SurvivorInventory>().GetBundleItem(this)) && kind != 101))
            {
                GameController.GetInstance().ItemSpawnManager.ReduceItemNumber();
                if (Audio != null)
                    Audio.Play(owner, "PickUp");
                SendMsgManager.GetInstance().SetNetworkTransformFalse(this.gameObject, false);
                SendMsgManager.GetInstance().SendPickUpItemMsg(gameObject, Owner);
              

                itemOwner.Pickup(this);
                
                OnField =false;
                transform.SetPositionAndRotation(transform.position + Vector3.down * 100f, Quaternion.identity);

                
            }

        }
    }
    public virtual void Equip(Transform EquipPoint)
    {
        transform.SetParent(null);
        transform.SetParent(EquipPoint.transform);
      
        transform.rotation = new Quaternion(0,0,0,0);
        transform.localPosition = Vector3.zero; //부모 오브젝트로부터 상대 좌표를 0으로 지정한다. 

        
    }
    public virtual void SetEquip(bool isEquip)
    {
        Collider[] itemColliders = GetComponents<Collider>();
        Rigidbody itemRigidbody = GetComponent<Rigidbody>();

        foreach (Collider itemCollider in itemColliders)
        {
            itemCollider.enabled = !isEquip;
        }
        itemRigidbody.isKinematic = isEquip;
    }
    public bool OnField
    {
        get
        {
            return onField;
        }
        set
        {
            onField = value;
        }
    }
    public int Kind
    {
        get
        {
            return kind;
        }
        set
        {
            kind = value;
        }
    }
    public string ItemName
    {
        get
        {
            return itemName;
        }
        set
        {
            itemName = value;
        }
    }
    public GameObject Owner
    {
        get
        {
            return owner;
        }
        set
        {
            owner = value;
        }
    }
    public string getEquipTag()
    {
        return EquipTag;
    }

    public void setEquipPoint(Transform point)
    {
        EquipPoint = point;
    }
}
