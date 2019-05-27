using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WildNetworkSetup : NetworkBehaviour {

    //public GameObject text1;
    //public GameObject text2;

    public override void OnStartServer () {
        switch (gameObject.tag.ToString())
        {
            case "Bear":
                GetComponent<Bear>().enabled = true;
                GetComponent<AIPath>().enabled = true;
                GetComponent<Seeker>().enabled = true;

                //text1.SetActive(true);
                //text2.SetActive(true);

                break;

            case "Boar":
                GetComponent<AIPath>().enabled = true;
                GetComponent<Seeker>().enabled = true;
                GetComponent<Boar>().enabled = true;

                break;

            case "Snake":
                GetComponent<Snake>().enabled = true;
                break;

            case "Bee":

                GetComponent<Beeflock>().enabled = true;
                GetComponent<Bee>().enabled = true;
               
                break;
        }
    }


}
