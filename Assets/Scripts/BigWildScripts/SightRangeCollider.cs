using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightRangeCollider : MonoBehaviour
{
    private bool isSight = false;
    public int targetObjectID = 0;
    private GameObject survivors;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" )
        {
            if (!other.GetComponent<SurvivorStatus>().IsDead())
            {
                isSight = true;
                targetObjectID = other.gameObject.GetComponent<SurvivorStatus>().PlayerID; //PlayerID 생존자 속성
                survivors = other.gameObject;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!other.GetComponent<SurvivorStatus>().IsDead())
            {
                isSight = true;
                targetObjectID = other.gameObject.GetComponent<SurvivorStatus>().PlayerID; //PlayerID 생존자 속성
                survivors = other.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!other.GetComponent<SurvivorStatus>().IsDead())
            {
                isSight = false;
                targetObjectID = 0;
                survivors = null;
            }
        }
    }

    public bool IsSight
    {
        get
        {
            return isSight;
        }
        set
        {
            isSight = value;
        }
    }
    public int IsSightTargetID()
    {
        return targetObjectID;
    }
    public GameObject getSurvivors()
    {
        return survivors;
    }
}