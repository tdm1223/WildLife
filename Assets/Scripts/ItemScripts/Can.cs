using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Can : Item
{
    public int HPRecovery;
    public int HungerRecovery;

    SurvivorRecogRangeCollider cont;
    public override void Start()
    {
        base.Start();
        ItemName = "통조림";
        EquipTag = "EquipArm";
        Kind = 301;
    }
    public override void Passive()
    {
        cont = Owner.GetComponent<SurvivorRecogRangeCollider>();
		cont.CmdSetBearColliderRadius(passiveRadius);
    }

    public override void Use(int pos)
    {
        base.Use(pos);
        HPup();
        cont.CmdSetBearColliderEnable(true);
		cont.CmdSetBearColliderRadius(usedRadius);
        Owner.transform.SendMessage("CanDuration");
    }

    private void HPup()
    {
        Owner.GetComponent<SurvivorStatus>().addtoHP(HPRecovery, ItemName);
        Owner.GetComponent<SurvivorStatus>().addtoHunger(HungerRecovery);
    }
}

