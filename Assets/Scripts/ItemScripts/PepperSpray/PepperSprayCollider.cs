using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PepperSprayCollider : MonoBehaviour {

    //private BigWildStates BWStates;

    private void OnTriggerEnter(Collider other)
    {
        //곰 도망
        if (other.CompareTag("Bear"))
        {
            GameController.GetInstance().ActionMessage("Right", "곰은 후추스프레이를 뿌렸습니다.", this.transform.parent.gameObject);
            SendMsgManager.GetInstance().PepperUse(other.gameObject);
        }
    }
}
