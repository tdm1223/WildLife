using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell : Item
{
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
    }

    public override void DePassive()
    {
        Owner.GetComponent<SurvivorStatus>().CmdSetBellFlag(false);
        Owner.transform.SendMessage("BellDuration");
    }
}
