using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SurvivorStatus : NetworkBehaviour
{

    public UISurvivor uiSurvivor;
    public UIStatus uiStatus;
    private Animator playerAnimator;
    private SurvivorAudio survivorAudio;
    private SurvivorController survivorController;

    public int playerID;
    public string playerName;

    public const int maxHP = 100;
    public const int maxHunger = 100;

    public float hungerReduceRate;
    public float fastHungerReduceRate;

    [SyncVar(hook = "OnHPChanged")]
    int HP = maxHP;
    [SyncVar(hook = "OnHungerChanged")]
    int hunger = maxHunger;
    [SyncVar]
    float recogRange;

    [SyncVar]
    private bool _infection; //감염여부
    [SyncVar]
    private string _infectionColor; //감염시킨 뱀의 색깔
    [SyncVar]
    private string _lastestDamagedObject = "null";

    [SyncVar]
    public bool garlicFlag;
    [SyncVar]
    public bool bellFlag;

    public bool UmbrellaState;

    public GameObject bigWild;

    bool isDead = false;

    private void Start()
    {
        if (isLocalPlayer)
        {
            if (uiStatus != null)
                uiStatus.InitStatus(maxHP, maxHunger);

            StartCoroutine(ReduceHunger()); //배고픔 감소 시작
            survivorAudio = GetComponent<SurvivorAudio>();
        }
        playerAnimator = gameObject.GetComponent<Animator>(); //플레이어 애니메이터
        survivorController = GetComponent<SurvivorController>();

        UmbrellaState = false;
    }


    [Command]
    public void CmdAddtoHP(int amount, string objectName)
    {
        if (amount < 0) //맞을때 애니메이션 발동
        {
            _lastestDamagedObject = objectName;

            if (!isDead)
            {
                RpcAttacked();
            }
        }

        HP += amount;
        if (HP >= maxHP)
            HP = maxHP;
        else if (HP <= 0)
            HP = 0;
    }

    public void addtoHP(int amount, string objectName)
    {
        CmdAddtoHP(amount, objectName);
    }

    [ClientRpc]
    void RpcAttacked()
    {
        if (isLocalPlayer)
        {
            survivorAudio.PlayBehitSound();
        }
    }

    public void addtoHP(int amount, Vector3 attackPosition, string objectName)
    {
        CmdAddtoHP(amount, objectName);

        if (amount < 0) //맞을때 애니메이션 발동
        {
            if (!isDead)
            {
                RpcAttackedByPosition(attackPosition);
            }
        }
    }

    [ClientRpc]
    void RpcAttackedByPosition(Vector3 attackPosition)
    {
        if (isLocalPlayer)
        {
            Vector3 relative = transform.InverseTransformPoint(attackPosition);
            float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

            // 4등분 해봄
            //if(angle <= 45f && angle >= -45f)
            //    Debug.Log("FRONT Attacked");
            //else if(angle > 45f && angle < 135f)
            //    Debug.Log("RIGHT Attacked");
            //else if (angle < -45f && angle > -135f)
            //    Debug.Log("LEFT Attacked");
            //else if (angle >= 135f || angle <= -135f)
            //    Debug.Log("BACK Attacked");

            if (angle <= 90f && angle >= -90)
                playerAnimator.Play("FrontAttacked");
            else if (angle > 90 || angle < -90f)
                playerAnimator.Play("BackAttacked");

            survivorAudio.PlayBehitSound();
        }
    }

    void OnHPChanged(int HP)
    {
        if (isLocalPlayer)
        {
            if (uiStatus != null)
                uiStatus.UpdateHP(HP);

            if (HP <= maxHP * 0.2f)
                survivorAudio.PlayHeartSound();
            else if (HP > maxHP * 0.2f)
                survivorAudio.StopHeartSound();

            if (((float)HP / maxHP) <= ((float)hunger / maxHunger))
                uiSurvivor.SetVignette(255f * (1f - Mathf.Clamp((((float)HP / maxHP - 0.2f) * 3.333f), 0f, 1f)));
            else
                uiSurvivor.SetVignette(255f * (1f - Mathf.Clamp((((float)hunger / maxHunger - 0.2f) * 3.333f), 0f, 1f)));

            this.HP = HP;

            if (HP <= 0)
                dead();
        }
    }

    [Command]
    public void CmdAddtoHunger(int amount)
    {
        hunger += amount;
        if (hunger >= maxHunger)
        {
            hunger = maxHunger;
        }

        if (hunger <= 0)
            hunger = 0;
    }

    public void addtoHunger(int amount)
    {
        //if (isLocalPlayer)
            CmdAddtoHunger(amount);
    }

    void OnHungerChanged(int hunger)
    {
        if (isLocalPlayer)
        {
            if (uiStatus != null)
                uiStatus.UpdateHunger(hunger);

            if (((float)HP / maxHP) <= ((float)hunger / maxHunger))
                uiSurvivor.SetVignette(255f * (1f - Mathf.Clamp((((float)HP / maxHP - 0.2f) * 3.333f), 0f, 1f)));
            else
                uiSurvivor.SetVignette(255f * (1f - Mathf.Clamp((((float)hunger / maxHunger - 0.2f) * 3.333f), 0f, 1f)));

            this.hunger = hunger;

            if (hunger <= 0)
            {
                _lastestDamagedObject = "Hunger";
                dead();
            }

        }
    }

    IEnumerator ReduceHunger()
    {
        while (true)
        {
            if (Input.GetButton("Run"))
                yield return new WaitForSeconds(fastHungerReduceRate);
            else
                yield return new WaitForSeconds(hungerReduceRate);

            addtoHunger(-1);

            if (isDead)
                break;
        }
    }

    void dead()
    {
        if (!isDead)
        {
            if (isLocalPlayer)
            {
                GameController.GetInstance().GameOver();
                GameController.GetInstance().BackGroundAudio.StopBGM();
            }

            CmdSetIsDead(true);

            SendMsgManager.GetInstance().SendToServerSurvivorUIMsg("null");
            SendMsgManager.GetInstance().SendToServerSurvivorDeadMsg(playerID.ToString());
        }
    }

    [Command]
    public void CmdSetIsDead(bool val)
    {
        isDead = val;

        gameObject.tag = "Untagged";
        transform.Find("RecogRangeColliderGroup").gameObject.SetActive(false);
        transform.GetComponent<CharacterController>().enabled = false;
        playerAnimator.enabled = false;

        RpcSetIsDead(val);
    }

    [ClientRpc]
    public void RpcSetIsDead(bool val)
    {
        isDead = val;
        playerAnimator.enabled = false;

        //gameObject.tag = "Untagged";
        //transform.FindChild("RecogRangeColliderGroup").gameObject.SetActive(false);
        //transform.GetComponent<CharacterController>().enabled = false;
        //playerAnimator.enabled = false;

        if (isLocalPlayer)
        {
            transform.Find("PlayerNameText").gameObject.SetActive(false);
            survivorAudio.PlayDeathSound();
        }
    }

    public int getHP()
    {
        return HP;
    }
    public int getHunger()
    {
        return hunger;
    }
    public bool IsDead()
    {
        return isDead;
    }
    public int PlayerID
    {
        get
        {
            return playerID;
        }
        set
        {
            playerID = value;
        }

    }
    public bool Infection
    {
        get
        {
            return _infection;
        }
        set
        {
            _infection = value;
        }
    }
    public string InfectionColor
    {
        get
        {
            return _infectionColor;
        }
        set
        {
            _infectionColor = value;
        }
    }

    public string lastestDamagedObject
    {
        get
        {
            return _lastestDamagedObject;
        }
    }

    public string PlayerName
    {
        get
        {
            return playerName;
        }
        set
        {
            playerName = value;
        }
    }

    public bool BellFlag
    {
        get
        {
            return bellFlag;
        }

    }

    public bool GarlicFlag
    {
        get
        {
            return garlicFlag;
        }

    }

    [Command]
    public void CmdSetGarlicFlag(bool value)
    {
        garlicFlag = value;
    }

    [Command]
    public void CmdSetBellFlag(bool value)
    {
        bellFlag = value;
    }


    [Command]
    public void CmdRecogRange(float value)
    {
        recogRange = value;
    }

    public float getRecogRange()
    {
        return recogRange;
    }

    [Command]
    public void CmdSetInfection(bool value)
    {
        _infection = value;
    }

    [Command]
    public void CmdSetKindAnti(string value)
    {
        _infectionColor = value;
    }




    [Command]
    public void CmdSpawnAntidoteEffect(GameObject obj, Vector3 point, Quaternion q)
    {
       
        GameObject effect;
        effect = Instantiate(obj.GetComponent<Antidote>().ParticleSystem, point, q);
       
        NetworkServer.Spawn(effect);
        Debug.Log("스폰 이펙트");
        DestroyEffect(effect);
    }

    [Command]
    public void CmdSpawnPerfumeEffect(GameObject obj, Vector3 point, Quaternion q)
    {

        GameObject effect;
        effect = ((Transform)Instantiate(obj.GetComponent<Perfume>().ParticleSystem, point, q)).gameObject;

        NetworkServer.Spawn(effect);
        Debug.Log("스폰 이펙트");
        DestroyEffect(effect);
    }

    [Command]
    public void CmdSpawnSprayEffect(GameObject obj, Vector3 point, Quaternion q)
    {

        GameObject effect;
        effect = ((Transform)Instantiate(obj.GetComponent<Spray>().ParticleSystem, point, q)).gameObject;

        NetworkServer.Spawn(effect);
        Debug.Log("스폰 이펙트");
        DestroyEffect(effect);
    }



    public void DestroyEffect(GameObject obj)
    {
        Debug.Log("디스트로이 이펙트");
        StartCoroutine(DsetroyEffect(obj));
    }

    IEnumerator DsetroyEffect(GameObject obj)
    {
        yield return new WaitForSeconds(2.0f);
        Debug.Log("코루틴 디스트로이 이펙트");
        NetworkServer.Destroy(obj);

        //CmdDestroyObject(obj);
    }

    public void SetUpRunSpeed()
    {
        if (isLocalPlayer)
            survivorController.SetUpRunSpeed(1.1f);
        else
            RpcSetUpRunSpeed(1.1f);
    }

    [ClientRpc]
    void RpcSetUpRunSpeed(float value)
    {
        survivorController.SetUpRunSpeed(value);
    }

    public void SetBackRunSpeed()
    {
        if (isLocalPlayer)
            survivorController.SetBackRunSpeed();
        else
            RpcSetBackRunSpeed();
    }

    [ClientRpc]
    void RpcSetBackRunSpeed()
    {
        survivorController.SetBackRunSpeed();
    }

    public void SetUmbrellaState(bool value)
    {
        UmbrellaState = value;
        survivorController.UmbrellaState = value;

        CmdSetUmbrellaState(value);
    }

    [Command]
    void CmdSetUmbrellaState(bool value)
    {
        UmbrellaState = value;
    }

    public void SetBW(GameObject bigWild)       //쫒는 야생동물 설정하기 (야생동물에서 실행)
    {
        this.bigWild = bigWild;

        RpcSetBW(bigWild);
    }

    [ClientRpc] 
    void RpcSetBW(GameObject bigWild)
    {
        this.bigWild = bigWild;
    }
}

