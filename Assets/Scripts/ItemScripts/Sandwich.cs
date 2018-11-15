using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandwich : Item
{
    public int HPRecovery;
    public int HungerRecovery;
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
		cont.CmdSetBearColliderRadius(passiveRadius);
		cont.CmdSetBoarColliderRadius(passiveRadius);
        cont.CmdSetBearColliderEnable(true);
        cont.CmdSetBoarColliderEnable(true);
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
        Owner.transform.SendMessage("SandwichDuration");
    }

    private void HPup()
    {
        Owner.GetComponent<SurvivorStatus>().addtoHP(HPRecovery, ItemName);
        Owner.GetComponent<SurvivorStatus>().addtoHunger(HungerRecovery);
    }
}

