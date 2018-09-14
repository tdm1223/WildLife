using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class SurvivorRecogRangeCollider : NetworkBehaviour {

    private SphereCollider sphereCollider;
    private CharacterController characterController;

    private SphereCollider BearRecogCollider;
    private SphereCollider BoarRecogCollider;

    private SphereCollider BeeRecogCollider;
    private SphereCollider SnakeRecogCollider;

    float t = 0.0f;
    public float walkRecogRange;
    public float runRecogRange;
    public float sneekRecogRange;
    public float idleRecogRange;


   [SyncVar] private float  recogRange;
  
    

    [SyncVar] private bool sneekFlag; //sneek상태인지 아닌지 판별하는 플래그
    

    void Start ()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform chil in children)
        {
            if (chil.name.Equals("RecogRangeCollider"))
            {
                sphereCollider = chil.GetComponent<SphereCollider>();

            }
            else if(chil.name.Equals("BearRecogRangeCollider"))
            {
                BearRecogCollider = chil.GetComponent<SphereCollider>();


           }
            else if (chil.name.Equals("BoarRecogRangeCollider"))
            {
                BoarRecogCollider = chil.GetComponent<SphereCollider>(); 

            }
            else if (chil.name.Equals("BeeRecogRangeCollider"))
            {
                    BeeRecogCollider = chil.GetComponent<SphereCollider>(); 

            }
            else if (chil.name.Equals("SnakeRecogRangeCollider"))
            {

                     SnakeRecogCollider = chil.GetComponent<SphereCollider>(); 
            }
        }
       
    
        characterController = GetComponent<CharacterController>();
	}




    [Command] 
    public void CmdSetBearColliderEnable(bool value)
    {
        BearRecogCollider.enabled = value;
        RpcSetBearColliderEnable(value);
    }

    [ClientRpc]
    public void RpcSetBearColliderEnable(bool value)
    {
        BearRecogCollider.enabled = value;
    }


    [Command]
    public void CmdSetBearColliderRadius(float value)
    {
        BearRecogCollider.radius = value;
        RpcSetBearColliderRadius(value);
    }

    [ClientRpc]
    public void RpcSetBearColliderRadius(float value)
    {
        BearRecogCollider.radius = value;
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////
    /// </summary>
    /// <param name="value"></param>
    [Command]
    public void CmdSetBoarColliderEnable(bool value)
    {
        BoarRecogCollider.enabled = value;
        RpcSetBoarColliderEnable(value);
    }

    [ClientRpc]
    public void RpcSetBoarColliderEnable(bool value)
    {
        BoarRecogCollider.enabled = value;
    }


    [Command]
    public void CmdSetBoarColliderRadius(float value)
    {
        BoarRecogCollider.radius = value;
        RpcSetBoarColliderRadius(value);
    }

    [ClientRpc]
    public void RpcSetBoarColliderRadius(float value)
    {
        BoarRecogCollider.radius = value;
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////
    /// </summary>
    /// <param name="value"></param>
    [Command]
    public void CmdSetBeeColliderEnable(bool value)
    {
        BeeRecogCollider.enabled = value;
        RpcSetBeeColliderEnable(value);
    }

    [ClientRpc]
    public void RpcSetBeeColliderEnable(bool value)
    {
        BeeRecogCollider.enabled = value;
    }


    [Command]
    public void CmdSetBeeColliderRadius(float value)
    {
        BeeRecogCollider.radius = value;
        RpcSetBeeColliderRadius(value);
    }

    [ClientRpc]
    public void RpcSetBeeColliderRadius(float value)
    {
        BeeRecogCollider.radius = value;
    }

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////
    /// </summary>
    /// <param name="value"></param>
    [Command]
    public void CmdSetSnakeColliderEnable(bool value)
    {
        SnakeRecogCollider.enabled = value;
        RpcSetSnakeColliderEnable(value);
    }

    [ClientRpc]
    public void RpcSetSnakeColliderEnable(bool value)
    {
        SnakeRecogCollider.enabled = value;
    }


    [Command]
    public void CmdSetSnakeColliderRadius(float value)
    {
        SnakeRecogCollider.radius = value;
        RpcSetSnakeColliderRadius(value);
    }

    [ClientRpc]
    public void RpcSetSnakeColliderRadius(float value)
    {
        SnakeRecogCollider.radius = value;
    }






    void ContorllRecogCollider()
    {
        if (Input.GetButtonDown("Run") || Input.GetButtonUp("Run") || Input.GetButtonDown("Sneek") || Input.GetButtonUp("Sneek"))
            t = 0.0f;

        if (Input.GetButton("Sneek"))
        {
            CmdSetSneakflag(true);
            if (characterController.velocity.magnitude == 0) //sneek후 가만히있음
            {
                CmdRecogRange(Mathf.Lerp(recogRange, idleRecogRange, t));
                // recogRange = Mathf.Lerp(recogRange, idleRecogRange, t);
            }
            else // sneek후 걸음
            {
                CmdRecogRange(Mathf.Lerp(recogRange, sneekRecogRange, Time.deltaTime * (recogRange / sneekRecogRange)));
                //recogRange = Mathf.Lerp(recogRange, sneekRecogRange, Time.deltaTime * (recogRange / sneekRecogRange));
            }
        }
        else
        {
            CmdSetSneakflag(false);

            if (Input.GetButton("Run") && characterController.velocity.magnitude > 0)
                CmdRecogRange(Mathf.Lerp(recogRange, runRecogRange, t));
            //recogRange = Mathf.Lerp(recogRange, runRecogRange, t);
            else if ((Input.GetButton("Horizontal") || (Input.GetButton("Vertical")) && characterController.velocity.magnitude > 0))
                CmdRecogRange(Mathf.Lerp(recogRange, walkRecogRange, t));
            //recogRange = Mathf.Lerp(recogRange, walkRecogRange, t);
            else
                CmdRecogRange(Mathf.Lerp(recogRange, idleRecogRange, t));
            // recogRange = Mathf.Lerp(recogRange, idleRecogRange, t);
        }

        t += Time.deltaTime * 0.3f;


        //Debug.Log(transform.root.gameObject.GetComponent<SurvivorStatus>().PlayerName + recogRange);
        CmdSetRecogCollider(recogRange);
    }

	// Update is called once per frame
	void FixedUpdate () {

        if(isLocalPlayer)
        {
            ContorllRecogCollider();
        }
    }

    public bool GetSneekFlag()
    {
        return sneekFlag;
    }


    [Command]
    public void CmdRecogRange(float value)
    {
        recogRange = value;
    }

    [Command]
    public void CmdSetRecogCollider(float value)
    {
        sphereCollider.radius = value;
        RpcSetRecogCollider(value);
    }

    [ClientRpc]
    public void RpcSetRecogCollider(float value)
    {
        if(sphereCollider!=null)
        {
        sphereCollider.radius = value;
        }
    }

   [Command]
   void CmdSetSneakflag(bool value)
    {
        sneekFlag = value;
    }
}
