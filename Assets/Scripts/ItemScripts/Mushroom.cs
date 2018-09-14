using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Item {

    public int HPRecovery;
    public int HungerRecovery;

    public int RecoveryRatio;
    public int DamageRatio;
    public Material[] material = new Material[5];

    public override void Start()
    {
        GetComponent<MeshRenderer>().material= material[(int)(Random.value * 10) % 5];
        base.Start();
        ItemName = "버섯";
        EquipTag = "EquipArm";
    }
    public override void Use(int pos)
    {
        base.Use(pos);
        RecoveryOrDamaged(Choose());
    }
    private void RecoveryOrDamaged(bool flag)
    {
        if (flag == true)
        {
            Owner.GetComponent<SurvivorStatus>().addtoHP(HPRecovery, ItemName);
            Owner.GetComponent<SurvivorStatus>().addtoHunger(HungerRecovery);
        }
        else
        {
            GameController.GetInstance().ActionMessage("Wrong", "독버섯을 먹었습니다.", Owner);
            Owner.GetComponent<SurvivorStatus>().addtoHP(-HPRecovery, ItemName);
            Owner.GetComponent<SurvivorStatus>().addtoHunger(-HungerRecovery);
        }
    }
    bool randomBoolean() //반반으로 True,False 반환
    {
        if (Random.value >= 0.5)
        {
            return true;
        }
        return false;
    }

    protected bool Choose()
    {
        float total = RecoveryRatio + DamageRatio;

        float randomPoint = UnityEngine.Random.value * total;

        if (randomPoint <= RecoveryRatio)
            return true;
        else
            return false;
    }    
}
