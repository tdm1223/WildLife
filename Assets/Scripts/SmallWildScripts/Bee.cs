using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bee : SmallWildFSM
{
    float timer;
    float orginSpeed;                   //원래속도

    [Header("벌 변수")]
    [Tooltip("공격횟수")]
    public int attackCount;
    [Tooltip("idle상태에서 올바른 대처를 하는지 재는 타이머")]
    public float idleTimer;
    [Tooltip("Look상태에서 올바른 대처를 하는지 재는 타이머")]
    public float lookTimer;

    private Vector3 respawnPosition;
    public delegate void AniEventHandler();

    public event AniEventHandler AttackTrue;

    public event AniEventHandler AttackFalse;
    string beforeTarget;
    public GameObject beeRespawn;
    public void Awake()
    {
        //기본상태로 비맹수급은 가만히 있는 idle
        state = State.Idle;
        respawnPosition = transform.position;
        AttackRange = transform.Find("AttackRangeCollider").GetComponent<AttackRangeCollider>();

    }

    void OnTriggerEnter(Collider coll) //자기가 감지할수 있는 영역안에 생존자가 들어오면 천천히 다가감
    {
        //향수냄새가 나거나 생존자의 냄새가 난다면 그쪽으로 천천히 추적
        if (state == State.Idle
            && (coll.tag == "BeeRecogRangeCollider" || coll.tag == "RecogRangeCollider")
            && (!coll.transform.root.GetComponent<SurvivorStatus>().IsDead()))
        {
            state = State.Guard;
            Target = coll.transform.root.gameObject;            
        }

        if (state == State.TurnBack
            && (coll.tag == "BeeRecogRangeCollider" || coll.tag == "RecogRangeCollider")
            && (!coll.transform.root.GetComponent<SurvivorStatus>().IsDead()))
        {
            Target = coll.transform.root.gameObject;
            if (Target.name == beforeTarget) return;
            else state = State.Guard;

            if (coll.tag == "BeeRecogRangeCollider")
            {
                GameController.GetInstance().ActionMessage("Wrong", "벌의 관심을 끌었습니다.", Target);
            }
            else if (coll.tag == "RecogRangeCollider")
            {
                GameController.GetInstance().ActionMessage("Wrong", "벌을 자극했습니다.", Target);
            }
        }
    }
    IEnumerator Idle() //attack후 다음 attack까지 타겟이 올바른 대처를 하는지 안하는지 판별하는 상태
    {
        speed = 4;
        while (state == State.Idle)
        {
            //업데이트할때
            TrueAction(idleTimer, Target);
            yield return 0;
        }
        //벗어날때
        GoToNextState();
    }
    IEnumerator Guard() //천천히 다가오는 상태
    {
        speed = 6;
        while (state == State.Guard)
        {
            //업데이트할때
            CurrentPosition = Target.transform.position;
            lookAtPlayer(Target.transform.position, 2.0f); //플레이어의 머리쪽으로 이동
            transform.Translate(Vector3.forward * (speed) * Time.deltaTime);
            float dist = GetDistance(Target);
            if (dist < 2.1f)
            {
                speed = 9;
                state = State.Look;
            }
            if (dist > 15.0f)
            {
                state = State.TurnBack;
            }
            yield return 0;
        }
        //벗어날때
        if (Audio != null)
            Audio.Stop("Guard");
        GoToNextState();
    }
    IEnumerator Look()
    {
        if (Audio != null)
            Audio.Play("LookAndChase");
        speed = 8;
        while (state == State.Look)
        {
            //업데이트할때
            //transform.RotateAround(Target.transform.position, Vector3.up, 180 * Time.deltaTime); //생존자 머리위에서 빙빙돔 코드 수정 필요
            beforeTarget = Target.name;
            TrueAction(lookTimer, Target);
            yield return 0;
        }
        //벗어날때
        if (Audio != null)
            Audio.Stop("LookAndChase");
        GoToNextState();

        //transform.parent = null;
    }
    IEnumerator Chase()
    {
        if (Audio != null)
            Audio.Play("LookAndChase");
        while (state == State.Chase)
        {
            //업데이트할때
            if (AttackRange.IsAttack)
            {
                if (AttackTrue != null)
                {
                    AttackTrue();
                }
                state = State.Attack;
            }
            if (speed != 0)
            {
                lookAtPlayer(Target.transform.position, 1.3f);
                transform.Translate(Vector3.forward * (speed) * Time.deltaTime);
            }
            else
            {
                speed = 10;
            }
            yield return 0;
        }
        //벗어날때
        if (Audio != null)
            Audio.Stop("LookAndChase");
        GoToNextState();

    }
    IEnumerator Attack()
    {
        while (state == State.Attack)
        {
            //업데이트할때
            //공격범위 벗어나면 다시 추격
            if (!AttackRange.IsAttack)
            {
                if (AttackFalse != null)
                {
                    AttackFalse();
                }

                state = State.Chase;
            }

            if (Audio != null)
                Audio.Play("Attack");

            Target.GetComponent<SurvivorStatus>().addtoHP((int)-attackDamage, gameObject.tag);
            CurrentPosition = Target.transform.position;
            attackCount--;
            orginSpeed = speed;
            if (attackCount > 0)        //공격 횟수가 남아있으면 다시
            {
                speed = 0;
                timer = 0;
            }
            state = State.Idle;
            yield return 0;
        }
        //벗어날때
        GoToNextState();
    }
    IEnumerator TurnBack() //집으로 돌아가는 상태
    {
        attackCount = 3;
        Target = null;
        while (state == State.TurnBack)
        {
            //업데이트할때
            speed = 3;
            lookAtPlayer(RespawnPosition, 0);
            transform.Translate(Vector3.forward * (speed) * Time.deltaTime);
            float dist = Vector3.Distance(RespawnPosition, transform.position);
            if (dist < 2.5f)
            {
                if (Audio != null)
                    Audio.Stop("Guard");
                beeRespawn.transform.GetComponent<BeeRespawn>().beeCount++;
                NetworkServer.Destroy(gameObject);
            }

            yield return 0;
        }
        //벗어날때
        GoToNextState();
    }
    private void TrueAction(float timer, GameObject currentTarget) //벌에대한 올바른 대처를 저장한 함수
    {
        this.timer += Time.deltaTime;
        if (attackCount > 0 && this.timer > timer)      //공격횟수 남아있고 일정시간 지나면 판단
        {
            speed = orginSpeed;
            if (currentTarget != null)
            {
                if (currentTarget.GetComponent<SurvivorController>().WaterCheck == false)       //생존자 물 속 판별 물 아니면 원래꺼
                {
                    if (CurrentPosition == currentTarget.transform.position
                     && Target.transform.Find("RecogRangeColliderGroup").Find("RecogRangeCollider").GetComponent<SphereCollider>().radius < 2.5
                    && Target.GetComponent<SurvivorRecogRangeCollider>().GetSneekFlag() == true)
                    //생존자가 가만히 있고 웅크리고 있으면 도망감.
                    {
                        GameController.GetInstance().ActionMessage("Right", "가만히 있었습니다.", Target);
                        attackCount = 0;
                    }
                    else
                    {
                        state = State.Chase;
                    }
                }
                else if (currentTarget.GetComponent<SurvivorRecogRangeCollider>().GetSneekFlag() == true)       //물 속일때 앉아있냐 일어서있냐 체크
                {
                    state = State.Guard;
                }
                else if (currentTarget.GetComponent<SurvivorRecogRangeCollider>().GetSneekFlag() == false)
                {
                    state = State.Chase;
                }
            }

        }
        if (attackCount <= 0)               //공격 횟수 끝나면 돌아가기
        {
            state = State.TurnBack;
        }
    }

    public Vector3 RespawnPosition
    {
        get
        {
            return respawnPosition;
        }
        set
        {
            respawnPosition = value;
        }
    }

    public void setBeeRespawn(GameObject beeRespawn)
    {
        this.beeRespawn = beeRespawn;
    }
}
