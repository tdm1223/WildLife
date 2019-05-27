using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectRangeCollider : MonoBehaviour {
    bool isDetect = false;
    public int targetObjectID = 0;
    Vector3 playerPosition;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isDetect = true;
            targetObjectID = other.gameObject.GetComponent<SurvivorStatus>().PlayerID; //PlayerID 생존자 속성
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && isDetect)       //인식범위 안에 있는 플레이어 위치
        {
            playerPosition = other.transform.position;
           // Debug.Log(other.transform.position);
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isDetect = false;
            targetObjectID = 0;
        }
    }


    public bool getIsDetect()
    {
        return isDetect;
    }
    public Vector3 PlayerPostion()  //인식 범위 안에 들어와있는 플레이어 위치 반환
    {
        return playerPosition;
    }

}
