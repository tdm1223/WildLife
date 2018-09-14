using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameText : MonoBehaviour {

    Vector3 rotateVector = new Vector3(0f, 180f, 0f);

	void Update ()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(rotateVector);
	}
}
