using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayCollider: MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //살충제 범위 안으로 벌이 들어오면 벌 제거
        if (other.tag == "BeeClone")
        {
            Debug.Log(other.tag);
            GameController.GetInstance().ActionMessage("Right", "벌에게 스프레이를 뿌렸습니다.", this.transform.parent.gameObject);
            SendMsgManager.GetInstance().DestroyBee(other.gameObject.transform.parent.gameObject);
        }
    }
}
