using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class BigWildSpawnManager : SpawnManager
{
    [Space(10)]
    [SerializeField] protected int[] spawnTimes;

    public AstarPath aStar;

    private GameAudio Audio;

    public void StartBigWildSpawn()
    {
        if(GetComponent<GameAudio>() != null)
            Audio = GetComponent<GameAudio>();
        StartCoroutine(BigWildSpawn());
    }

    IEnumerator BigWildSpawn()
    {
        int spawnTime;

        for (int i = 0; i < spawnTimes.Length; i++)
        {
            if (i > 0)
                spawnTime = spawnTimes[i] - spawnTimes[i - 1];
            else
                spawnTime = spawnTimes[0];

            if (spawnTime <= 10)
            {
                GameController.GetInstance().UpdateBigWildSpawnText(spawnTime);
                if (spawnTime <= 5 && spawnTime > 0)
                    Audio.Play("Tick");
            }

            while (true)
            {
                yield return new WaitForSeconds(1);

                spawnTime--;

                if (spawnTime <= 10)
                {
                    GameController.GetInstance().UpdateBigWildSpawnText(spawnTime);
                    if (spawnTime <= 5 && spawnTime > 0)
                        Audio.Play("Tick");
                }

                if (spawnTime == 0)
                {
                    Audio.Play("Warning");

                    if (NetworkServer.active)
                    {
                        GameObject spawnObject = Instantiate(ObjectPrefabs[Choose()].Object, SetRespawnPos(), Quaternion.identity);
                        aStar.GetComponent<AstarPath>().Scan();
                        NetworkServer.Spawn(spawnObject);

                        if (SpawnObjectsParent != null)
                            spawnObject.transform.SetParent(SpawnObjectsParent.transform);   //부모 오브젝트 설정

                        spawnObject.name = spawnObject.tag; //오브젝트 이름을 태그와 같게 설정
                    }
                }
                else if (spawnTime == -3)
                    break;
            }
        }
    }

}
