using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class ItemSpawnManager : SpawnManager {

    [Space(10)]
    [Tooltip("게임 시작 시 미리 아이템을 뿌려놓음")]
    [SerializeField] protected bool PreSpawn;
    [SerializeField] protected int MaxItemNumber;
    [SerializeField] protected int SpawnPeriod;

    private int ItemNumber = 0;
    private int ItemID = 0;

    public override void OnStartServer()
    {
        base.Start();

        if (PreSpawn)
        {
            for (int i = 0; i < MaxItemNumber; i++)
                SpawnObject();
        }

        StartCoroutine(ItemSpawning());
    }

    protected override GameObject SpawnObject()
    {
        GameObject spawnObject = Instantiate(ObjectPrefabs[Choose()].Object, SetRespawnPos(), Quaternion.identity);
        NetworkServer.Spawn(spawnObject);

        if (SpawnObjectsParent != null)
            spawnObject.transform.parent = SpawnObjectsParent.transform;

        ItemNumber++;
        ItemID++;
        spawnObject.GetComponent<ItemNetworkSync>().ItemID = "Item" + ItemID;

        return spawnObject;
    }

    IEnumerator ItemSpawning()
    {
        while (true)
        {
            if (ItemNumber < MaxItemNumber)
            {
                GameObject newItem = Instantiate(ObjectPrefabs[Choose()].Object, SetRespawnPos(), Quaternion.identity);
                NetworkServer.Spawn(newItem);

                ItemNumber++;
                ItemID++;
                newItem.GetComponent<ItemNetworkSync>().ItemID = "Item" + ItemID;

                if (SpawnObjectsParent != null)
                    newItem.transform.SetParent(SpawnObjectsParent.transform);
            }

            yield return new WaitForSeconds(SpawnPeriod);
        }
    }

    public void ReduceItemNumber()
    {
        ItemNumber--;
    }
}
