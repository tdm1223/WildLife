using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfumeCollider : MonoBehaviour {

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var cont = other.GetComponent<SurvivorRecogRangeCollider>();
            cont.CmdSetBeeColliderEnable(true);
            other.transform.SendMessage("PerfumeDuration");
        }
    }
}
