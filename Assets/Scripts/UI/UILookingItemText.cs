using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILookingItemText : MonoBehaviour
{

    Text LookingItemText;

    Vector3 screenPos;
    Rect rootCanvasRect;

    bool isActive = false;

    void Start()
    {
        LookingItemText = GetComponent<Text>();
        rootCanvasRect = transform.parent.GetComponent<RectTransform>().rect;
    }

    void UpdateLookingItemText(Item item)
    {
        if (!isActive)
        {
            LookingItemText.enabled = true;
            LookingItemText.text = item.ItemName + " 획득 (E)";
        }

        screenPos = Camera.main.WorldToViewportPoint(item.transform.position);
        screenPos.x = rootCanvasRect.width * screenPos.x - rootCanvasRect.width * 0.5f;
        screenPos.y = rootCanvasRect.height * screenPos.y - rootCanvasRect.height * 0.5f + 50f;

        transform.localPosition = screenPos;
    }

    void UnableLookingItemText()
    {
        LookingItemText.enabled = false;
        isActive = false;
    }
}
