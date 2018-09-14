using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetText : MonoBehaviour {

    Bear bear;
    Vector3 rotateVector = new Vector3(0f, 180f, 0f);
    TextMesh text;

    void Start()
    {
        bear = transform.parent.GetComponent<Bear>();
        text = GetComponent<TextMesh>();
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(rotateVector);

        if (bear.LookTarget != null)
        {
            text.text = "LookTarget : " + bear.LookTarget.gameObject.name;
        }
        else
            text.text = "LookTarget : null";
    }
}
