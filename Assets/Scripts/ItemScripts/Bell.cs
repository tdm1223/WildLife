using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell : Item
{

    //private BigWildStates BWStates;
    //private GameObject snakeRecogCollider;
    SurvivorRecogRangeCollider cont;
    public override void Start()
    {
        base.Start();
        ItemName = "종";
        EquipTag = "EquipArm";
        Kind = 103;
    }
    public override void Passive()
    {
        cont = Owner.GetComponent<SurvivorRecogRangeCollider>();
        Owner.GetComponent<SurvivorStatus>().CmdSetBellFlag(true);
		cont.CmdSetSnakeColliderRadius(passiveRadius);
        cont.CmdSetSnakeColliderEnable(true);
        //snakeRecogCollider = Owner.GetComponent<SurvivorController>().SnakeCollider;
        // ColliderEnable(snakeRecogCollider, 10f);
    }
    public override void DePassive()
    {
        Owner.GetComponent<SurvivorStatus>().CmdSetBellFlag(false);
        Owner.transform.SendMessage("BellDuration");
    }
    //private void ColliderEnable(GameObject collider, float radius) //획득시 콜라이더를 활성화 시키는 함수
    //{
    //    collider.GetComponent<SphereCollider>().radius = radius;
    //    collider.GetComponent<SphereCollider>().enabled = true;
    //}
}
