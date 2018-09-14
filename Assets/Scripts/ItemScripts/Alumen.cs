using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alumen : Item
{
    public override void Start()
    {
        base.Start();
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        transform.rotation = rotation;
        ItemName = "백반";
        EquipTag = "EquipArm";
        Kind = 104;
    }
}
