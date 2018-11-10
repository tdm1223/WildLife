using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Bear : BigWildFSM
{
    [Header("곰 변수")]
    [Tooltip("버린 아이템에 관심을 가지는 시간")]
    public float attentionTimer;
    [Tooltip("가까이 붙었을때 Look->Attack로 상태가 바뀌기 위한 시간")]
    public float stopLookTimer;
    [Tooltip("Look->Chase로 상태가 바뀌기 위한 시간")]
    public float lookToChaseTimer;

    private GameObject throwItem;   //생존자 추적중 버려진 아이템
    private string beforeState;
    private bool backMoveFlag;
    private Quaternion rotation;            //곰 avoid때 저장할 각도
    private float angle;                //look에서 chase넘어가는 잘못된 행동의 각도 판단변수
    
    public void FixedUpdate()
    {
        if (backMoveFlag)   //타겟이 우산을 펼치거나 후추스프레이 사용하면 뒤로가는 코드.
        {
            TurnAround();
        }
    }
    private void OnTriggerEnter(Collider coll)//소리가 들리면 Patrol에서 Detect로 바뀌고 소리가나는방향으로 천천히이동
    {
        if ((state == State.Patrol || state == State.Idle || state==State.Detect)
            && (coll.tag == "BearRecogRangeCollider" || coll.tag == "RecogRangeCollider")
            && (!coll.transform.root.GetComponent<SurvivorStatus>().IsDead())
            && (!backMoveFlag))
        {
            detectPoint = coll.GetComponent<Transform>();
            state = State.Detect;
        }
    }
    private void OnTriggerStay(Collider coll)
    {
        if ((state == State.Idle || state == State.Patrol || state == State.Detect)
            && (coll.tag == "BearRecogRangeCollider" || coll.tag == "RecogRangeCollider")
            && (!coll.transform.root.GetComponent<SurvivorStatus>().IsDead())
            && (!backMoveFlag))
        {
            detectPoint = coll.GetComponent<Transform>();
            state = State.Detect;
        }
    }

    IEnumerator Look()
    {
        //들어갈때
        if (Audio != null)
        {
            Audio.Play("Look", LookTarget);
            Audio.Play("Walk");
        }
        if (backMoveFlag)
        {
            state = State.Idle;
            LookTarget = null;
        }
        aiPath.speed = walkSpeed - 1f;
        aiPath.rotationSpeed = rotationDamping;
        animator.SetFloat("Speed_f", aiPath.speed / runSpeed);
        while (state == State.Look)
        {
            //업데이트할때
            if (LookTarget != null)
            {
                aiPath.target = LookTarget.transform;
                float dist = GetDistance(LookTarget);
                currentPosition = LookTarget.transform.position; //타겟의 현재 위치
                currentSpeed = LookTarget.GetComponent<CharacterController>().velocity.magnitude; //타겟의 현재 속도
                //일정거리에 가까워질때 몇초후에도 가만히있다면 공격
                if (dist < 2f)
                {
                    aiPath.speed = 0f; //이동속도 0
                    aiPath.rotationSpeed = 0f; //회전속도 0
                    animator.SetFloat("Speed_f", aiPath.speed / runSpeed); //애니메이션 속도조절
                    StartCoroutine(StopLook(stopLookTimer, currentPosition));
                }
                else if (dist > 35f)
                {
                    GameController.GetInstance().ActionMessage("Right", "곰의 시야에서 벗어났습니다.", LookTarget);
                    state = State.Idle;
                    LookTarget.GetComponent<SurvivorStatus>().SetBW(null);
                    LookTarget = null;
                }
                else
                {
                    aiPath.speed = walkSpeed - 1f;
                }

                //공격범위 안에 들어온 타겟이 LookTarget하고 다르다면 LookTarget을 들어온 타겟으로 바꿈
                if (AttackRange.InTarget != null)
                {
                    if (LookTarget != AttackRange.InTarget)
                        LookTarget = AttackRange.InTarget;
                }

                //타겟이 뛰거나 뒤로 돌면 쫒아감
                if (LookTarget != null)
                {
                    if (!backMoveFlag)   //회피때는 인식X
                    {
                        StartCoroutine(LookToChase(lookToChaseTimer, currentSpeed, LookTarget));
                    }
                    else
                    {
                        StopCoroutine(LookToChase(lookToChaseTimer, currentSpeed, LookTarget));
                    }
                }
            }
            else if (LookTarget == null)
            {
                state = State.Idle;
            }
            yield return 0;
        }
        //벗어날때
        if (Audio != null)
        {
            Audio.Stop("Walk");
        }
        GoToNextState();
    }
    IEnumerator Chase()
    {
        if (Audio != null)
            Audio.Play("Walk");

        LookTarget = null;
        aiPath.speed = runSpeed;
        aiPath.rotationSpeed = rotationDamping;

        while (state == State.Chase)
        {
            if (backMoveFlag)
            {
                state = State.Idle;
                ChaseTarget = null;
            }
            //업데이트할때
            if (ChaseTarget != null)
            {
                animator.SetFloat("Speed_f", aiPath.speed / runSpeed);
                ChaseTarget.GetComponent<SurvivorStatus>().SetUpRunSpeed();
                aiPath.target = ChaseTarget.transform;
                float dist = GetDistance(ChaseTarget);
                if (dist < 1.5f)
                {
                    aiPath.speed = 0f;      //가까이 가면 속도 줄기
                    aiPath.rotationSpeed = 360f;
                    animator.SetFloat("Speed_f", aiPath.speed / runSpeed);
                }
                else if (dist > 35f)
                {
                    GameController.GetInstance().ActionMessage("Right", "곰의 시야에서 벗어났습니다.", ChaseTarget);
                    state = State.Idle;
                    ChaseTarget.GetComponent<SurvivorStatus>().SetBW(null);
                    ChaseTarget = null;
                }
                else
                {
                    aiPath.speed = runSpeed;
                }

                //ChaseTarget이 죽었다면 Patrol
                if (ChaseTarget != null)
                {
                    if (ChaseTarget.GetComponent<SurvivorStatus>().IsDead())
                    {
                        ChaseTarget.GetComponent<SurvivorStatus>().SetBackRunSpeed();
                        state = State.Patrol;
                        ChaseTarget = null;
                    }
                }
            }
            else if (ChaseTarget == null)
            {
                state = State.Idle;
            }

            //Chase상태에서 공격범위 안에 들어오면 공격
            if (AttackRange.IsAttack)
            {
                ChaseTarget = AttackRange.InTarget;
                state = State.Attack;
            }

            //공격범위 안에 들어온 타겟이 ChaseTarget 다르다면 ChaseTarget을 들어온 타겟으로 바꿈
            if (AttackRange.InTarget != null)
            {
                if (ChaseTarget != AttackRange.InTarget)
                {
                    ChaseTarget = AttackRange.InTarget;
                }
            }

            yield return 0;
        }
        //벗어날때
        if (Audio != null)
        {
            Audio.Stop("Walk");
        }
        GoToNextState();
    }
    IEnumerator Attention()
    {

        aiPath.rotationSpeed = rotationDamping;
        while (state == State.Attention)
        {
            if (throwItem == null)
            {
                StopAttention();
            }
            else
            {
                aiPath.target = throwItem.transform;
                float dist = GetDistance(throwItem);
                if (dist < 2.5f)        //아이템과 가까워지면
                {
                    aiPath.speed = 0f;
                    animator.SetBool("Eat_b", true);
                }
                else if (dist > 30f)    //아이템과 멀어도 풀림
                {
                    BigWildState = beforeState;
                }
                else
                {
                    aiPath.rotationSpeed = rotationDamping;
                    if (beforeState.Equals("Look"))
                        aiPath.speed = walkSpeed;
                    else if (beforeState.Equals("Chase"))
                        aiPath.speed = runSpeed;
                }
            }
            yield return 0;
        }
        //벗어날때
        GoToNextState();
    }
    public void AttentionItem(GameObject throwItem)     //생존자가 아이템 던져서 관심가지게실행
    {
        Debug.Log("AttentionItem");
        if (state != State.Attention && state != State.Attack)
        {
            this.throwItem = throwItem;
            beforeState = BigWildState;
            state = State.Attention;
        }
    }
    void TurnAround()
    {
        animator.SetBool("Avoid", true);
        state = State.Idle;
        transform.Translate(Vector3.back * Time.deltaTime * 0.5f);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.5f);
    }
    void SetBackMoveFlag(bool backMoveFlag)
    {
        this.backMoveFlag = backMoveFlag;
        rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 180, 0); // avoid순간 바라볼 방향
        StartCoroutine(ResetBackMoveFlag());
    }
    IEnumerator ResetBackMoveFlag()
    {
        yield return new WaitForSeconds(4f);
        animator.SetBool("Avoid", false);
        state = State.Idle;
        LookTarget = null;
        ChaseTarget = null;
        this.backMoveFlag = false;
    }
    public void StopAttention()	//Eat애니메이션에서 실행 
    {
        animator.SetBool("Eat_b", false);
        if (state == State.Attention)
        {
            if (beforeState.Equals("Look"))
            {
                aiPath.speed = walkSpeed;
            }
            else if (beforeState.Equals("Chase"))
            {
                aiPath.speed = runSpeed;
            }
            animator.SetFloat("Speed_f", aiPath.speed / runSpeed);
            aiPath.rotationSpeed = rotationDamping;
            BigWildState = beforeState;
            EatItem();
        }
    }
    public void EatItem()
    {
        //Eat 애니메이션 중간에 먹기
        if (throwItem != null)
        {
            if (throwItem.GetComponent<Item>().Kind / 100 == 3)   // 네트워크 시 아이템 구별 되면 좀 더 구현예정 먹는 아이템이면 곰이 먹음
            {
                NetworkServer.Destroy(throwItem);
            }
        }
    }
    public void EatRightActionMessage()
    {
        if (GetOneTarget() != null)
        {
            GameController.GetInstance().ActionMessage("Right", "곰은 사람이 버린 물건에 관심을 가집니다.", GetOneTarget());
        }
    }
    IEnumerator StopLook(float stopLookTimer, Vector3 currentPosition)
    {
        yield return new WaitForSeconds(stopLookTimer);
        if (!backMoveFlag)
        {
            if (LookTarget != null)
            {
                //animator.SetBool("Eat_b", true);
                if (LookTarget.transform.position == currentPosition && state != State.Idle) //정해진시간뒤에 포지션이 저장해둔 포지션과 같다면 공격
                {
                    state = State.Attack;
                    ChaseTarget = LookTarget;
                    LookTarget = null;

                    GameController.GetInstance().ActionMessage("Wrong", "곰에게 죽은 척 했습니다.", ChaseTarget);
                }
            }
        }
    }
    IEnumerator LookToChase(float lookToChaseTimer, float currentSpeed, GameObject target)
    {
        yield return new WaitForSeconds(lookToChaseTimer);
        if (!backMoveFlag)
        {
            if (LookTarget != null && state == State.Look)
            {
                float angle = Quaternion.Angle(target.transform.rotation, transform.rotation);
                if (angle < 80f || angle > 280f)
                {
                    state = State.Chase;
                    ChaseTarget = LookTarget;
                    LookTarget = null;

                    GameController.GetInstance().ActionMessage("Wrong", "곰에게 등을 보였습니다.", ChaseTarget);
                }
                else if(currentSpeed > 5.0f)
                {
                    state = State.Chase;
                    ChaseTarget = LookTarget;
                    LookTarget = null;

                    GameController.GetInstance().ActionMessage("Wrong", "곰에게서 뛰어서 도망갔습니다.", ChaseTarget);
                }
            }
        }
    }
}