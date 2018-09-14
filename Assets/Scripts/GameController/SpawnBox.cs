using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class SpawnBox : NetworkBehaviour
{
    public float respawnTime;

    [Serializable]
    public struct itemAndRatio  //아이템과 아이템 생성 비율을 저장하는 구조체
    {
        public GameObject item;
        public int ratio;
    }
    public itemAndRatio[] item;

    Transform itemObjects;
    Bounds boxBounds;   //BoxCollider 범위
    int nextItemNumber;
    int count = 0;
    float MinX, MaxX, MinZ, MaxZ;

    //확률을 결정하는 배열선언
    float[] itemPercentage = new float[3];

    public override void OnStartServer()
    {
        boxBounds = GetComponent<BoxCollider>().bounds;  //BoxCollider 범위로 아이템 생성 범위 결정
        MinX = boxBounds.min.x;
        MaxX = boxBounds.max.x;
        MinZ = boxBounds.min.z;
        MaxZ = boxBounds.max.z;

        if (transform.parent.Find("ItemObjects") != null)
            itemObjects = transform.parent.Find("ItemObjects");    //생성된 아이템들을 itemObjects라는 오브젝트의 자식으로 하기 위함

        nextItemNumber = Choose();
        StartCoroutine(RandomSpawn());
    }

    int Choose()
    {
        float total = 0f;
        for (int i = 0; i < item.Length; i++)
            total += item[i].ratio;

        float randomPoint = UnityEngine.Random.value * total;
        for (int i = 0; i < item.Length; i++)
        {
            if (randomPoint < item[i].ratio)
                return i;
            else
                randomPoint -= item[i].ratio;
        }
        return item.Length - 1;
    }

    Vector3 SetPos()
    {
        float PosX = UnityEngine.Random.Range(MinX, MaxX);
        float PosZ = UnityEngine.Random.Range(MinZ, MaxZ);

        return new Vector3(PosX, boxBounds.center.y, PosZ);
    }

    IEnumerator RandomSpawn()
    {
        while (true)
        {
            GameObject newItem = Instantiate(item[nextItemNumber].item, SetPos(), Quaternion.identity);
            NetworkServer.Spawn(newItem);

            count++;
            newItem.GetComponent<ItemNetworkSync>().ItemID = "Item" + count;

            if (itemObjects != null)    //새로 생성된 아이템을 itemObjects 오브젝트의 자식으로
                newItem.transform.SetParent(itemObjects);

            yield return new WaitForSeconds(respawnTime);
            nextItemNumber = Choose();
        }
    }
}
