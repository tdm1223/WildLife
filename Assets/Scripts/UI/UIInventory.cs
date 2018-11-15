using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{ 
    int inventoryCapacity = 6;
    Image[] SlotImage;
    Text[] SlotText;
    Text[] SlotBundleCount;
    GameObject selectImage;
    //RectTransform InventoryUIBackground;
    GameObject ExtendedInventoryUIBackground;

    void Start()
    {
        SlotImage = new Image[inventoryCapacity];
        SlotText = new Text[inventoryCapacity];
        SlotBundleCount = new Text[inventoryCapacity];

        selectImage = transform.Find("Select Image").gameObject;
        //InventoryUIBackground = transform.Find("InventoryUIBackground").GetComponent<RectTransform>();
        ExtendedInventoryUIBackground = transform.Find("ExtendedInventoryUIBackground").gameObject;

        for (int i = 0; i < inventoryCapacity ; i++)
        {
            SlotText[i] = transform.Find("Slot" + (i + 1) + " Text").GetComponent<Text>();
            SlotImage[i] = transform.Find("Slot" + (i + 1) + " Image").GetComponent<Image>();
            SlotBundleCount[i] = transform.Find("Slot" + (i + 1) + " BundleCount").GetComponent<Text>();
        }

    }

    void AddItem(object[] parameter)    //parameter[0] : slot 번호, parameter[1] : 아이템 이름, parameter[2] : 아이템 아이콘
    {
        SlotText[(int)parameter[0]].gameObject.SetActive(true);
        SlotText[(int)parameter[0]].text = (string)parameter[1];
        
        SlotImage[(int)parameter[0]].gameObject.SetActive(true);
        SlotImage[(int)parameter[0]].sprite = (Sprite)parameter[2];

        SlotBundleCount[(int)parameter[0]].gameObject.SetActive(true);
        SlotBundleCount[(int)parameter[0]].text = "1";
    }

    void RemoveItem(int index)
    {
        SlotText[index].gameObject.SetActive(false);
        SlotImage[index].gameObject.SetActive(false);
        selectImage.SetActive(false);
        SlotBundleCount[index].gameObject.SetActive(false);
    }

    void SelectItem(int index)
    {
        if(!selectImage.activeSelf) selectImage.SetActive(true);
        selectImage.transform.position = SlotImage[index].transform.position;
    }

    void NotSelect(int index)
    {
        selectImage.SetActive(false);
    }

    void ExtendInventory()
    {
        ExtendedInventoryUIBackground.SetActive(true);
    }

    void SetBundleCount(object[] parameter)   //parameter[0]는 index, parameter[1]은 count
    {
        SlotBundleCount[(int)parameter[0]].text = parameter[1].ToString();
    }
}
