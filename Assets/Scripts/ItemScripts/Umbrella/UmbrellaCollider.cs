using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmbrellaCollider : MonoBehaviour
{
    //private BigWildStates BWStates;

    private void OnTriggerEnter(Collider other)
    {
        //곰,멧돼지 도망
        if (other.CompareTag("Bear"))
        {
            GameController.GetInstance().ActionMessage("Right", "곰에게 우산을 펼쳤습니다.", this.transform.parent.gameObject);
            SendMsgManager.GetInstance().PepperUse(other.gameObject);
        }
    }
}
