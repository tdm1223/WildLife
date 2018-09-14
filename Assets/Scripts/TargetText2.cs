using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetText2 : MonoBehaviour {

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

        if (bear.ChaseTarget != null)
        {
            text.text = "ChaseTarget : " + bear.ChaseTarget.gameObject.name;
        }
        else
            text.text = "ChaseTarget : null";
    }
}
