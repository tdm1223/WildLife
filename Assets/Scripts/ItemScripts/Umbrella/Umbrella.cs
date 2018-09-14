using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella : Item
{
    private GameObject instantiateUmbrellaCollider;
    public GameObject umbrellaCollider;
    private int umbrellaDurability; //우산 내구도 미구현
    private Animator animator;

    public override void Start()
    {
        Audio = GetComponent<ItemAudioPlayer>();

        OnField = true;
        ItemName = "우산";
        EquipTag = "EquipUmbrella";
        Kind = 102;
        maxCount = useCount;
        animator = transform.GetComponent<Animator>();
        animator.SetBool("UmbrellaOn", false);
    }
    public override void Use(int pos)
    {
        base.Use(pos);
        if (Owner.GetComponent<SurvivorStatus>().UmbrellaState == false)
        {
            Owner.GetComponent<SurvivorStatus>().SetUmbrellaState(true);

            SendMsgManager.GetInstance().SendUmbrellaAnimSyncMsg(gameObject, true);
            instantiateUmbrellaCollider = (GameObject)Instantiate(umbrellaCollider) as GameObject;
            instantiateUmbrellaCollider.transform.SetParent(Owner.transform);
            instantiateUmbrellaCollider.transform.localPosition = new Vector3(0, 0, 0);
            instantiateUmbrellaCollider.transform.localRotation = Quaternion.identity;
            Destroy(instantiateUmbrellaCollider,1f);
        }
        else if (Owner.GetComponent<SurvivorStatus>().UmbrellaState == true)
        {
            Owner.GetComponent<SurvivorStatus>().SetUmbrellaState(false);

            SendMsgManager.GetInstance().SendUmbrellaAnimSyncMsg(gameObject, false);
        }
    }

}
