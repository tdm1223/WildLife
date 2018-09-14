using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    private bool isEmpty;
    private Item item;
    // Use this for initialization
    void Start()
    {
        IsEmpty = true; //초기에는 슬롯이 비어있음
    }
    public void SetItemNull()
    {
        item = null;
    }
    public bool IsEmpty
    {
        get
        {
            return isEmpty;
        }
        set
        {
            isEmpty = value;
        }
    }
    public void AddItem(Item item)
    {
        this.item = item;
        isEmpty = false;
    }
    public void RemoveItem()
    {
        this.item = null;
        isEmpty = true;
    }
    public Item Item
    {
        get
        {
            return item;
        }
    }
    public bool IsBundleFull()
    {
        if (item.maxCount == item.useCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
