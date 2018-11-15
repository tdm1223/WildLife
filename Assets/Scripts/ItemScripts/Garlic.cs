using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garlic : Item
{
    public int hpRecovery;
    public int hungerRecovery;
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
		cont.CmdSetBearColliderRadius(passiveRadius);
        cont.CmdSetBearColliderEnable(true);
		cont.CmdSetSnakeColliderRadius(passiveRadius/4);
        cont.CmdSetSnakeColliderEnable(true);
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
        Owner.transform.SendMessage("GarlicDuration");
    }

    private void HPup()
    {
        Owner.GetComponent<SurvivorStatus>().addtoHP(hpRecovery, ItemName);
        Owner.GetComponent<SurvivorStatus>().addtoHunger(hungerRecovery);
    }
}
