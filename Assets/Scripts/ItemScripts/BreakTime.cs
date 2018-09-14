using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakTime : Item
{

    public int HPRecovery;
    public int HungerRecovery;
    SurvivorRecogRangeCollider cont;

    public override void Start()
    {
        base.Start();
        ItemName = "초코바";
        EquipTag = "EquipArm";
        Kind = 302;
    }
    public override void Passive()
    {
        cont = Owner.GetComponent<SurvivorRecogRangeCollider>();
        cont.CmdSetBearColliderEnable(true);
		cont.CmdSetBearColliderRadius(passiveRadius);
        cont.CmdSetBeeColliderEnable(true);
		cont.CmdSetBeeColliderRadius(passiveRadius/3);

    }
    public override void DePassive()
    {
        Owner.transform.SendMessage("BreakTimeDuration");
    }
    public override void Use(int pos)
    {
        base.Use(pos);
        HPup();

		cont.CmdSetBearColliderRadius(usedRadius);
		cont.CmdSetBeeColliderRadius(usedRadius/3);

        Owner.transform.SendMessage("BreakTimeDuration");
    }

    private void HPup()
    {
        Owner.GetComponent<SurvivorStatus>().addtoHP(HPRecovery, ItemName);
        Owner.GetComponent<SurvivorStatus>().addtoHunger(HungerRecovery);
    }
}
