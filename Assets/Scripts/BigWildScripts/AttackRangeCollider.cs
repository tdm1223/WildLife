using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeCollider : MonoBehaviour
{
    bool isAttack = false;
    public int targetObjectID = 0;
    GameObject inTarget;

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            isAttack = true;
            inTarget = other.gameObject;
            targetObjectID = other.gameObject.GetComponent<SurvivorStatus>().PlayerID; //PlayerID 생존자 속성
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isAttack = false;
            inTarget = null;
            targetObjectID = 0;
        }
    }
    public bool IsAttack
    {
        get
        {
            return isAttack;
        }

    }
    public GameObject InTarget
    {
        get
        {
            return inTarget;
        }
        set
        {
            inTarget = value;
        }
    }
}
