using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class SpawnManager : NetworkBehaviour {

    [SerializeField] protected BoxCollider SpawnBoxCollider;
    [SerializeField] protected GameObject SpawnObjectsParent;

    [Space(10)]
    [SerializeField] protected ObjectAndRatio[] ObjectPrefabs;

    [Serializable]
    protected struct ObjectAndRatio
    {
        public GameObject Object;
        public int ratio;
    }

    protected RaycastHit rayHit;
    protected float MinX, MaxX, MinZ, MaxZ;

    protected virtual void Start()
    {
        Bounds spawnBounds = SpawnBoxCollider.bounds;

        MinX = spawnBounds.min.x;
        MaxX = spawnBounds.max.x;
        MinZ = spawnBounds.min.z;
        MaxZ = spawnBounds.max.z;
    }

    protected Vector3 SetRespawnPos()
    {
        Vector3 position = new Vector3(UnityEngine.Random.Range(MinX, MaxX), 50, UnityEngine.Random.Range(MinZ, MaxZ));
        
        if (Physics.Raycast(position, Vector3.down, out rayHit, 100))
        {
            if (rayHit.collider.gameObject.layer == 9)
            {
                position = rayHit.point;
            }
            else
            {
                position = SetRespawnPos();
            }
        }

        return position;
    }

    protected int Choose()
    {
        float total = 0f;
        for (int i = 0; i < ObjectPrefabs.Length; i++)
            total += ObjectPrefabs[i].ratio;

        float randomPoint = UnityEngine.Random.value * total;
        for (int i = 0; i < ObjectPrefabs.Length; i++)
        {
            if (randomPoint < ObjectPrefabs[i].ratio)
                return i;
            else
                randomPoint -= ObjectPrefabs[i].ratio;
        }
        return ObjectPrefabs.Length - 1;
    }

    protected virtual GameObject SpawnObject()
    {
        GameObject spawnObject = Instantiate(ObjectPrefabs[Choose()].Object, SetRespawnPos(), Quaternion.identity);
        NetworkServer.Spawn(spawnObject);

        if (SpawnObjectsParent != null)
            spawnObject.transform.parent = SpawnObjectsParent.transform;

        spawnObject.name = spawnObject.tag;

        return spawnObject;
    }
}
