using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Beeflock : NetworkBehaviour
{

    public GameObject BeePrefab;


    public int tankSize;
    static int numBee = 10;
    public GameObject[] allBee = new GameObject[numBee];
    public  Vector3 goalPos = Vector3.zero;

    void Start()
    {
       
        
        for (int i = 0; i < numBee; i++)
        {
            Vector3 pos = new Vector3(
              Random.Range(-tankSize, tankSize)+
                transform.position.x,
                                     
              Random.Range(-tankSize, tankSize)+ 
                transform.position.y,
                                      
              Random.Range(-tankSize, tankSize)+
                transform.position.z);
           // allBee[i] = (GameObject)Instantiate(BeePrefab, pos, Quaternion.identity);

            var Bee = (GameObject)Instantiate(BeePrefab, pos, Quaternion.identity);
            Bee.transform.SetParent(transform);
            NetworkServer.Spawn(Bee);

            SendMsgManager.GetInstance().SetParentBeeFlock(this.gameObject, Bee);


            allBee[i] = Bee;
        }

    }
    void Update()
    {
     
        goalPos = transform.position;
        //if (Random.Range(0, 10000) < 50)
        //{
        //    goalPos = new Vector3(Random.Range(-tankSize, tankSize),
        //                              Random.Range(-tankSize, tankSize),
        //                               Random.Range(-tankSize, tankSize));
        //    transform.position = goalPos;
        //    //goalPos = transform.position;
        //    //goalPrefab.transform.position = goalPos;
        //}

    }
}
