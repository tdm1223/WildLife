using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Perfume : Item
{
    public Transform ParticleSystem;
    public GameObject PerfumeCollider;

    public override void Start()
    {
        base.Start();
        ItemName = "향수";
        EquipTag = "EquipArm";
    }
    public override void Use(int pos)
    {
        Vector3 headPoint = new Vector3(Owner.transform.position.x, Owner.transform.position.y + 2.0f, Owner.transform.position.z);
        base.Use(pos);
        Owner.GetComponent<SurvivorStatus>().CmdSpawnPerfumeEffect(this.gameObject, headPoint, Owner.transform.rotation);

        var cont = Owner.GetComponent<SurvivorRecogRangeCollider>();
		cont.CmdSetBeeColliderRadius (usedRadius);
        cont.CmdSetBeeColliderEnable(true);
        Owner.transform.SendMessage("PerfumeDuration");
        
        ///향수 남한테도 범위 적용 
        GameObject Perfume = (GameObject)Instantiate(PerfumeCollider) as GameObject;
        Perfume.transform.SetParent(Owner.transform);
        Perfume.transform.localPosition = new Vector3(0, 0, 0);
        Perfume.transform.localRotation = Quaternion.identity;

        Destroy(Perfume, 2.0f);
    }
}
