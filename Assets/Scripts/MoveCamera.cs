using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public float RotationSpeed = 2f;

    public void RotateUPCamera()
    {
        StartCoroutine(RotateUP());
    }

    IEnumerator RotateUP()
    {
        while (gameObject.transform.localRotation.eulerAngles.x > 325.1f)
        {
            gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.Euler(-35.0f, 170.0f, 0.0f), Time.deltaTime * RotationSpeed);
            yield return null;
        }
    }
}
