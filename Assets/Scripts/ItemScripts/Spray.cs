using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spray : Item
{
    public Transform ParticleSystem;
    public GameObject SprayCollider;

    public override void Start()
    {
        base.Start();
        ItemName = "스프레이";
        EquipTag = "EquipArm";
    }

    public override void Use(int pos)
    {
        base.Use(pos);

        Vector3 chestPoint = new Vector3(Owner.transform.position.x, Owner.transform.position.y + 1.5f, Owner.transform.position.z);
        //스프레이 효과 생성
        Owner.GetComponent<SurvivorStatus>().CmdSpawnSprayEffect(this.gameObject, chestPoint, Owner.transform.rotation);

        //스프레이 생성
        GameObject Spray = Instantiate(SprayCollider) as GameObject;
        Spray.transform.SetParent(Owner.transform);
        Spray.transform.localPosition = new Vector3(0, 0, 0);
        Spray.transform.localRotation = Quaternion.identity;

        Destroy(Spray, 3.0f);
    }
}
