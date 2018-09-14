using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSpawnManager : SpawnManager {

    [Space(10)]
    [SerializeField] protected int SpawnNumber;

    public override void OnStartServer()
    {
        base.Start();

        for (int i = 0; i < SpawnNumber; i++)
            SpawnObject();

    }
}
