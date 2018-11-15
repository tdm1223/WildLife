using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backpack : Item
{
	public override void Start ()
    {
        base.Start();
        ItemName = "가방";
        EquipTag = "EquipBack";
        Kind = 101;
    }
    public override void Equip(Transform EquipPoint)
    {
        transform.SetParent(EquipPoint.transform);
        transform.localPosition = new Vector3(0.226f, -0.372f, 0.00f); //부모 오브젝트로부터 상대 좌표를 0으로 지정한다. 
        transform.localRotation = Quaternion.Euler(-170.514f, 89.655f, 4.256f);
    }
}
