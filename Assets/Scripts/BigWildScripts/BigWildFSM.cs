using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BigWildFSM : NetworkBehaviour
{
    public enum State
    {
        Idle,
        Patrol,
        Detect,
        Look,
        Chase,
        Attack,
        Attention,
    }


    [Header("야생동물 변수")]
    [Tooltip("회전 속도")]
    public float rotationDamping;
    [Tooltip("걷는속도")]
    public float walkSpeed;
    [Tooltip("뛰는변수")]
    public float runSpeed;
    [Tooltip("공격 데미지")]
    public int attackDamage;
    [Tooltip("Idle상태에서 다시 Patrol로 넘어가기위한 시간")]
    public float stopIdleTimer;


    //나머지 변수들
    protected Transform detectPoint;
    protected GameObject detect;
    protected Transform nextPoint;
    protected GameObject wayPoint;
    protected Vector3 currentPosition;
    protected float currentSpeed; //타겟의 현재속도
    protected Animator animator;
    protected AIPath aiPath;
    protected State state;
    protected ObjectAudio Audio;
    protected GameObject chaseTarget;
    protected GameObject lookTarget;
    protected SightRangeCollider SightRange;
    protected AttackRangeCollider AttackRange;

    protected RaycastHit rayHit;
    protected float MAX_RAY_DISTANCE = 30.0f; //레이 인식 거리
    protected Vector3 rayPosition;
    protected float delayTime = 1f;   //공격 딜레이
    protected float nextAttack = 0.0f;

    bool patrolFlag = false;
    bool detectFlag = false;
    int beastID;
    int targetID;
    Bounds spawnBounds;
    float MinX, MaxX, MinZ, MaxZ;

    public void Awake()
    {
        //기본상태로 야생동물이 맵 전체를 무작위로 돌아다니는 Patrol
        state = State.Patrol;
        ChaseTarget = null;
        LookTarget = null;

        SightRange = transform.Find("SightRangeCollider").GetComponent<SightRangeCollider>();
        AttackRange = transform.Find("AttackRangeCollider").GetComponent<AttackRangeCollider>();

        detect = new GameObject();
        detect.name = "DetectPoint";

        wayPoint = new GameObject();
        wayPoint.name = "WayPoint";

        animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
        aiPath.rotationSpeed = rotationDamping;

        spawnBounds = GameObject.Find("/GameController/SpawnManager/SpawnBox").GetComponent<BoxCollider>().bounds;
        MinX = spawnBounds.min.x;
        MaxX = spawnBounds.max.x;
        MinZ = spawnBounds.min.z;
        MaxZ = spawnBounds.max.z;

        if(GetComponent<ObjectAudio>() != null)
            Audio = GetComponent<ObjectAudio>();
    }
    public void Start()
    {
        GoToNextState();
    }
    protected IEnumerator Idle()
    {
        //들어올때
        aiPath.speed = 0f; //이동속도 0
        aiPath.rotationSpeed = 0f; //회전속도 0
        LookTarget = null;
        ChaseTarget = null;
        while (state == State.Idle)
        {
            //업데이트할때            
            animator.SetFloat("Speed_f", aiPath.speed / runSpeed); //애니메이션 속도조절
            //animator.SetBool("Eat_b", true); // Idle 애니메이션

            Vector3 currentPosition = transform.position;
            transform.position = currentPosition; //위치 고정
            StartCoroutine(StopIdle(stopIdleTimer, currentPosition));
            yield return 0;
        }
        //벗어날때
        GoToNextState();
    }
    protected IEnumerator Patrol()
    {
        if (Audio != null)
            Audio.Play("Walk");

        aiPath.speed = walkSpeed;
        aiPath.rotationSpeed = rotationDamping;
        animator.SetFloat("Speed_f", aiPath.speed / runSpeed);
        LookTarget = null;
        ChaseTarget = null;
        patrolFlag = false;
        while (state == State.Patrol)
        {
            //업데이트할때
            
            if (!patrolFlag)
            {
                patrolFlag = true;
                aiPath.target = SetNextPoint(wayPoint);
            }
            //두 지점사이의 거리를 구해서 거리가 어느정도 될때까지 이동
            float dist = GetDistance(aiPath.target.gameObject);

            //어느정도 가까워지면 다음 포인트를 지정하고 잠깐idle상태
            if (dist <= 2f)
            {
                state = State.Idle;
                //idle상태에서 idle상태 후에 Patrol로 자동으로 돌아와서 위치 계산
            }

            //시야안에 플레이어가 있다면 레이쏘는 함수 실행
            if (SightRange.getSurvivors() != null)
                if (!SightRange.getSurvivors().transform.GetComponent<SurvivorStatus>().IsDead())
                    InSightRange();

            yield return 0;
        }
        //벗어날때
        if (Audio != null)
            Audio.Stop("Walk");

        GoToNextState();

    }
    protected IEnumerator Detect()
    {
        if (Audio != null)
            Audio.Play("Walk");
        aiPath.speed = walkSpeed - 0.5f;
        aiPath.rotationSpeed = rotationDamping;
        animator.SetFloat("Speed_f", aiPath.speed / runSpeed);

        while (state == State.Detect)
        {
            //업데이트할때
            aiPath.target = SetDetectPoint(detect);
            //탐지된곳까지 이동하였다가 아무일도 일어나지않거나 탐지되는 소리가 없어진다면 잠시 머물다가 다시 이동
            float dist = Vector3.Distance(aiPath.target.position, transform.position);
            if (dist <= 2.0f || dist > 40.0f)
            {
                state = State.Idle;
            }

            //시야안에 플레이어가 있다면 레이쏘는 함수 실행
            if (SightRange.getSurvivors() != null)
                if (!SightRange.getSurvivors().transform.GetComponent<SurvivorStatus>().IsDead())
                    InSightRange();

            yield return 0;
        }
        //벗어날때
        if (Audio != null)
            Audio.Stop("Walk");

        GoToNextState();
    }
    protected IEnumerator Attack()
    {
        while (state == State.Attack)
        {
            if (Time.time > nextAttack)
            {
                aiPath.speed = 0f;
                animator.SetFloat("Speed_f", 0);
                aiPath.rotationSpeed = 360f;
                int ran = Random.Range(0, 2);
                if (this.transform.tag == "Boar")
                {
                    animator.Play("Attack");
                }
                else
                {
                    switch (ran)
                    {  //곰 공격 애니메이션 실행 
                        case 0:
                            animator.Play("Attack");
                            break;
                        case 1:
                            animator.Play("AttackBite");
                            break;
                    }
                }
                nextAttack = Time.time + delayTime;
            }
            else
            {
                animator.SetFloat("Speed_f", 0);
            }

            //공격 범위를 벗어나면 Chase상태로
            if (!AttackRange.IsAttack)
                state = State.Chase;

            //공격 타겟이 죽으면 Patrol로
            if (ChaseTarget != null)
            {
                if (ChaseTarget.GetComponent<SurvivorStatus>().IsDead())
                {
                    ChaseTarget.GetComponent<SurvivorStatus>().SetBW(null);
                    ChaseTarget = null;
                    state = State.Patrol;
                }
            }

            yield return 0;
        }
        //벗어날때
        GoToNextState();
    }

    //다음 웨이포인트 지정
    public Transform SetNextPoint(GameObject point)
    {
        float PosX = Random.Range(MinX, MaxX);
        float PosZ = Random.Range(MinZ, MaxZ);

        point.transform.position = new Vector3(PosX, 50, PosZ);

        if (Physics.Raycast(point.transform.position, Vector3.down, out rayHit, 100))
        {         
            if (rayHit.collider.gameObject.layer == 9)
            {
                point.transform.position = rayHit.point;
            }
            else
            {
                SetNextPoint(point);
            }
        }

        return point.transform;
    }

    //탐지 포인트 지정
    public Transform SetDetectPoint(GameObject point)
    {
        point.transform.position = detectPoint.position;
        return point.transform;
    }

    // target과 Bigwild사이의 거리를 반환
    public float GetDistance(GameObject target)
    {
        return (target.transform.position - transform.position).magnitude;
    }

    //상태를 바꾸는 함수
    protected void GoToNextState()
    {
        //호출하기 원하는 함수의 이름
        string methodName = state.ToString();

        //클래스에서 해당 상태의 함수를 검색
        System.Reflection.MethodInfo info = GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }

    //데미지 입히는 함수
    public void AttackDamaged()
    {
        if (ChaseTarget != null)
        {
            ChaseTarget.GetComponent<SurvivorStatus>().addtoHP(-attackDamage, transform.position, gameObject.tag);

            if (Audio != null)
                Audio.Play("Attack");
        }
    }

    //LookTarget 액세서
    public GameObject LookTarget
    {
        get
        {
            return lookTarget;
        }
        set
        {
            lookTarget = value;
        }
    }

    //Chase Target 액세서
    public GameObject ChaseTarget
    {
        get
        {
            return chaseTarget;
        }
        set
        {
            chaseTarget = value;
        }
    }

    //BigWild의 State 액세서
    public string BigWildState
    {
        get
        {
            return state.ToString();
        }
        set
        {
            switch (value)
            {
                case "Patrol":
                    state = State.Patrol;
                    break;
                case "Look":
                    state = State.Look;
                    break;
                case "Chase":
                    state = State.Chase;
                    break;
                case "Attack":
                    state = State.Attack;
                    break;
                case "Detect":
                    state = State.Detect;
                    break;
                case "Idle":
                    state = State.Idle;
                    break;
            }
        }
    }

    //타겟 하나만 반환해주는 함수
    public GameObject GetOneTarget()
    {
        if (LookTarget != null)
            return LookTarget;
        else if (ChaseTarget != null)
            return ChaseTarget;
        else return null;
    }

    IEnumerator StopIdle(float stopIdleTimer, Vector3 currentPosition)
    {
        //stopIdleTimer후에 상태를 Patrol로 바꿔줌
        yield return new WaitForSeconds(stopIdleTimer);
        if (state == State.Idle)
        {
            aiPath.rotationSpeed = rotationDamping;   //회전속도 복구
            aiPath.speed = walkSpeed;
            animator.SetFloat("Speed_f", aiPath.speed / runSpeed); //애니메이션 속도조절
            state = State.Patrol;
        }
        else
        {
            yield return 0;
        }
    }

    protected virtual void InSightRange()     //시야 범위 안 레이쏘기
    {
        rayPosition.x = transform.position.x;
        rayPosition.y = transform.position.y + 1.35f;
        rayPosition.z = transform.position.z;

        Vector3 survivorPosition = SightRange.getSurvivors().transform.position - this.transform.position;

        if (Physics.Raycast(rayPosition, survivorPosition, out rayHit, MAX_RAY_DISTANCE))
        {
            //Debug.DrawRay(rayPosition, survivorPosition * 30.0f, Color.red, 2);
            if (rayHit.collider.tag == "Player")
            {
                LookTarget = SightRange.getSurvivors().transform.gameObject;
                state = State.Look;
                LookTarget.GetComponent<SurvivorStatus>().SetBW(gameObject);
            }
        }
    }
}
