using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Trap : Item
{
    public bool isLocated;
    GameObject trappedObject;
    Animator animator;
    Vector3 CurrentPosition;
    private int TrapSeconds;

    public override void Start()
    {
        base.Start();
        animator = transform.GetComponent<Animator>();
        isLocated = false;
        TrapSeconds = 3;
        ItemName = "덫";
        Kind = 105;
        EquipTag = "EquipArm";
    }

    public override void Use(int pos)
    {
        base.Use(pos);
        Locate();
    }

    public void Locate()
    {
        RaycastHit rayHit;

        if (Physics.Raycast(owner.transform.position + owner.transform.forward * 2 + Vector3.up, Vector3.down, out rayHit, 100))
        {
            if (rayHit.collider.gameObject.layer == 9)
            {

                SendMsgManager.GetInstance().LocateTrap(this.gameObject, rayHit.point);
            }
            else
            {
                useCount++;
            }
        }

    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bear" || other.tag == "Boar" || other.tag == "Player")
        {
            if (isLocated)
            {
                Audio.Play(owner, "Active");
                CurrentPosition = other.transform.position;
                trappedObject = other.gameObject;
                animator.SetBool("TrapOn", true);

                StartCoroutine(DontMove());
            }
        }
    }

    IEnumerator DontMove()
    {
        var counter = TrapSeconds;
        float timer = 0.0f;
        // 1초씩 카운트를 진행한다
        while (counter > 0)
        {
            timer += Time.deltaTime;
            if (timer > TrapSeconds)
            {
                break;
            }
            trappedObject.transform.position = CurrentPosition;
            yield return null;
        }
        animator.SetBool("TrapOn", false);
        NetworkServer.Destroy(gameObject);
    }
}
