using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlockSetup : NetworkBehaviour {

    Animator animator;
    public override void OnStartServer()
    {

     

        GetComponent<flock>().enabled = true;

        animator = transform.GetComponent<Animator>();
        animator.SetBool("Guard", false);
        animator.SetBool("Idle", true);
        animator.SetBool("Attack", false);

        
        var callback = transform.root.GetComponent<Bee>();
  
        
        callback.AttackTrue += new Bee.AniEventHandler(ActBeeAttackAni);
        //callback.AttackFalse += new Bee.AniEventHandler(UnActBeeAttackAni);
       
    }

   void ActBeeAttackAni()
    {
        animator.Play("Attack");
    }

   //void UnActBeeAttackAni()
   // {
   //     animator.SetBool("Attack", false);
   // }
}
