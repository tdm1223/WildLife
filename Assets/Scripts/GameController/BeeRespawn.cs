using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BeeRespawn : NetworkBehaviour {
    public GameObject smallWild;
    public int beeCount=1;
    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "BeeRecogRangeCollider" || other.tag == "Player")&& beeCount>0 && NetworkServer.active)
        {
            if (transform.parent.GetComponent<ObjectAudio>() != null)
                transform.parent.GetComponent<ObjectAudio>().Play("Touch");


            var bee = Instantiate(smallWild, transform.position, Quaternion.identity);
            bee.transform.GetComponent<Bee>().setBeeRespawn(this.gameObject);
            beeCount--;

        
                NetworkServer.Spawn(bee);
            if (other.tag == "BeeRecogRangeCollider")
            {
                GameController.GetInstance().ActionMessage("Wrong", "벌의 관심을 끌었습니다.", other.transform.root.gameObject);
            }
            else if (other.tag == "Player")
            {
                GameController.GetInstance().ActionMessage("Wrong", "벌집을 건드렸습니다.", other.transform.root.gameObject);
            }
            //Debug.Log("플레이어 들어옴");
        }

        //if (other.tag == "BeeRecogRangeCollider" || other.tag == "RecogRangeCollider" || other.tag == "Player")
        //{
        //    var bee = Instantiate(smallWild, transform.position, Quaternion.identity);

        //    if (NetworkServer.active)
        //    {
        //        NetworkServer.Spawn(bee);
        //    }
        //    //Debug.Log("플레이어 들어옴");
        //}
    }
}
