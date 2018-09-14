using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneyPot : Item
{

    public int HPRecovery;
    public int HungerRecovery;
    //private GameObject BigWIldRecogCollider;
    //private GameObject SmallWildRecogCollider;
    SurvivorRecogRangeCollider cont;

    public override void Start()
    {
        base.Start();
        ItemName = "꿀단지";
        EquipTag = "EquipArm";
        Kind = 304;
    }
    public override void Passive()
    {
        Debug.Log("honeypot passive");

        cont = Owner.GetComponent<SurvivorRecogRangeCollider>();
        cont.CmdSetBearColliderEnable(true);
        cont.CmdSetBeeColliderEnable(true);

		cont.CmdSetBearColliderRadius(passiveRadius);
		cont.CmdSetBeeColliderRadius(passiveRadius/3);
    }
    public override void DePassive()
    {
        Owner.transform.SendMessage("HoneyPotDuration");
    }
    public override void Use(int pos)
    {
        base.Use(pos);
        HPup();

		cont.CmdSetBearColliderRadius(usedRadius);
		cont.CmdSetBeeColliderRadius(usedRadius/3);
        //BigWIldRecogCollider.GetComponent<SphereCollider>().radius = 30;
        //SmallWildRecogCollider.GetComponent<SphereCollider>().radius = 30;
        Owner.transform.SendMessage("HoneyPotDuration");
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
