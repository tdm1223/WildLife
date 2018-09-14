using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCloud : MonoBehaviour {

    private float speed;
    public float startZPosition;
    public float endZPosition;

    void Start()
    {
        speed = Random.Range(1.0f, 6.0f);
    }

    void Update()
    {
        transform.position += Vector3.back * speed * Time.deltaTime;
        if (transform.position.z <= endZPosition) ScrollEnd();
    }

    void ScrollEnd()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, startZPosition);
    }
}
