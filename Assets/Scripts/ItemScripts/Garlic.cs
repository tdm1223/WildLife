using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garlic : Item
{

    public int hpRecovery;
    public int hungerRecovery;
    //private GameObject bearRecogCollider;
    //private GameObject snakeRecogCollider;


    SurvivorRecogRangeCollider cont;
    public override void Start()
    {
        base.Start();
        ItemName = "마늘";
        EquipTag = "EquipArm";
        Kind = 303;
    }
    public override void Passive()
    {
        Owner.GetComponent<SurvivorStatus>().CmdSetGarlicFlag(true);
        cont = Owner.GetComponent<SurvivorRecogRangeCollider>();
        //bearRecogCollider = Owner.GetComponent<SurvivorController>().BearCollider;
        //snakeRecogCollider = Owner.GetComponent<SurvivorController>().SnakeCollider;

		cont.CmdSetBearColliderRadius(passiveRadius);
        cont.CmdSetBearColliderEnable(true);
		cont.CmdSetSnakeColliderRadius(passiveRadius/4);

        cont.CmdSetSnakeColliderEnable(true);

        //ColliderEnable(bearRecogCollider, 10);
        //ColliderEnable(snakeRecogCollider, 5f);
    }
    public override void DePassive()
    {
        Owner.GetComponent<SurvivorStatus>().CmdSetGarlicFlag(false);
        Owner.transform.SendMessage("GarlicDuration");
    }
    public override void Use(int pos)
    {
        base.Use(pos);
        HPup();
		cont.CmdSetBearColliderRadius(usedRadius);
		cont.CmdSetSnakeColliderRadius(usedRadius/4);

        //bearRecogCollider.GetComponent<SphereCollider>().radius = 15;
        //snakeRecogCollider.GetComponent<SphereCollider>().radius = 5f;
        Owner.transform.SendMessage("GarlicDuration");
    }
    private void HPup()
    {
        Owner.GetComponent<SurvivorStatus>().addtoHP(hpRecovery, ItemName);
        Owner.GetComponent<SurvivorStatus>().addtoHunger(hungerRecovery);
    }
    //private void ColliderEnable(GameObject collider, float radius) //획득시 콜라이더를 활성화 시키는 함수
    //{
    //    collider.GetComponent<SphereCollider>().radius = radius;
    //    collider.GetComponent<SphereCollider>().enabled = true;
    //}
}
