using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandwich : Item
{
    public int HPRecovery;
    public int HungerRecovery;
    //private GameObject BearRecogCollider;
    //private GameObject BoarRecogCollider;
    SurvivorRecogRangeCollider cont;

    public override void Start()
    {
        base.Start();
        ItemName = "샌드위치";
        EquipTag = "EquipArm";
        Kind = 305;
    }
    public override void Passive()
    {
        cont = Owner.GetComponent<SurvivorRecogRangeCollider>();

        //ont.CmdSetBearColliderEnable(true);
		cont.CmdSetBearColliderRadius(passiveRadius);
		cont.CmdSetBoarColliderRadius(passiveRadius);
        cont.CmdSetBearColliderEnable(true);
        cont.CmdSetBoarColliderEnable(true);

        //BoarRecogCollider = Owner.GetComponent<SurvivorController>().BoarCollider;
        //BearRecogCollider = Owner.GetComponent<SurvivorController>().BearCollider;

        //ColliderEnable(BearRecogCollider, 10);
        //ColliderEnable(BoarRecogCollider, 10);
    }
    public override void DePassive()
    {
        Owner.transform.SendMessage("SandwichDuration");
    }
    public override void Use(int pos)
    {
        base.Use(pos);
        HPup();
		cont.CmdSetBearColliderRadius(usedRadius);
		cont.CmdSetBoarColliderRadius(usedRadius);


        //BearRecogCollider.GetComponent<SphereCollider>().radius = 15;
        //BoarRecogCollider.GetComponent<SphereCollider>().radius = 15;
        Owner.transform.SendMessage("SandwichDuration");
    }
    private void HPup()
    {
        Owner.GetComponent<SurvivorStatus>().addtoHP(HPRecovery, ItemName);
        Owner.GetComponent<SurvivorStatus>().addtoHunger(HungerRecovery);
    }
    //private void ColliderEnable(GameObject collider, float radius) //획득시 콜라이더를 활성화 시키는 함수
    //{
    //    collider.GetComponent<SphereCollider>().radius = radius;
    //    collider.GetComponent<SphereCollider>().enabled = true;
    //}
}

