using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PepperSpray : Item {
    public GameObject SprayCollider;
    public override void Start()
    {
        base.Start();
        ItemName = "후추 스프레이";
        EquipTag = "EquipArm";
    }
    public override void Use(int pos)
    {
        base.Use(pos);

        GameObject PepperSpray = (GameObject)Instantiate(SprayCollider) as GameObject;
        PepperSpray.transform.SetParent(Owner.transform);
        PepperSpray.transform.localPosition = new Vector3(0, 0, 0);
        PepperSpray.transform.localRotation = Quaternion.identity;
        Destroy(PepperSpray, 3.0f);
    }
}
