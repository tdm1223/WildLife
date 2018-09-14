using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfumeCollider : MonoBehaviour {

    private void OnTriggerStay(Collider other)
    {
        //살충제 범위 안으로 벌이 들어오면 벌 제거
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.name+"향수");

            var cont = other.GetComponent<SurvivorRecogRangeCollider>();
            cont.CmdSetBeeColliderEnable(true);

            //useCollider.GetComponent<SphereCollider>().enabled = true; //사용시 벌에대한 사용자의 감지 범위를 넓힘.
            other.transform.SendMessage("PerfumeDuration");
        }
    }
}
