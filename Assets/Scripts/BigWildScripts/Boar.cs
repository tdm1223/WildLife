using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : BigWildFSM
{

    [Header("멧돼지 변수")]
    [Tooltip("멧돼지 시야")]
    public float sightDistance = 20f;
    [Tooltip("장애물로 인식하여 되돌아 가는데 걸리는 시간")]
    public float obstacleDistinctionTimer;
    [Tooltip("Look상태에서 Chase상태로 넘어는데 걸리는 위한 시간")]
    public float lookToChaseTimer = 3.0f;
    [Tooltip("우산대처후 Patrol로 전환하는데 그 후 다시 생존자를 인식하기까지 여유시간")]
    public float targetResetTimer = 10.0f;

    private float attentionTimer;
    private bool umbrellaMoveFlag = false;
    private GameObject exceptionTarget;     //우산대처후 해당 생존자 예외시키기
    private Quaternion rotation;            //멧돼지 장애물 판별후 돌때 쓰기위한 각도

    private void OnTriggerEnter(Collider coll)//소리가 들리면 Patrol에서 Detect로 바뀌고 소리가나는방향으로 천천히이동
    {
        if ((state == State.Patrol || state == State.Idle || state == State.Detect)
            && (coll.tag == "BoarRecogRangeCollider" || coll.tag == "RecogRangeCollider")
            && (!coll.transform.root.GetComponent<SurvivorStatus>().IsDead()))
        {
            if ((coll.transform.root.gameObject != exceptionTarget) || exceptionTarget == null)
            {
                detectPoint = coll.GetComponent<Transform>();
                state = State.Detect;
            }
        }
    }
    private void OnTriggerStay(Collider coll)
    {
        if ((state == State.Idle || state == State.Patrol || state == State.Detect)
            && (coll.tag == "BoarRecogRangeCollider" || coll.tag == "RecogRangeCollider")
            && (!coll.transform.root.GetComponent<SurvivorStatus>().IsDead()))
        {
            if ((coll.transform.root.gameObject != exceptionTarget) || exceptionTarget == null)
            {
                detectPoint = coll.GetComponent<Transform>();
                state = State.Detect;
            }
        }
    }

    IEnumerator Look()
    {
        if (Audio != null)
        {
            Audio.Play("Look");
            Audio.Play("Walk");
        }
        aiPath.speed = walkSpeed - 1.5f;
        aiPath.rotationSpeed = rotationDamping;
        while (state == State.Look)
        {
            //업데이트할때
            animator.SetFloat("Speed_f", aiPath.speed / runSpeed); //추가
            if (LookTarget != null)
            {
                float dist = GetDistance(LookTarget);
                aiPath.target = LookTarget.transform;
                if (LookTarget.GetComponent<SurvivorStatus>().UmbrellaState)
                {
                    if (dist < 5.0f)
                    {
                        aiPath.speed = 0f;
                        animator.SetFloat("Speed_f", 0); //추가
                        StartCoroutine(ObstacleDistinction(obstacleDistinctionTimer, LookTarget));
                    }
                }
                else if (dist < 2.0f)
                {
                    state = State.Chase;
                    ChaseTarget = LookTarget;
                    GameController.GetInstance().ActionMessage("Wrong", "멧돼지와 가깝습니다.", ChaseTarget);
                }
                else if (dist > sightDistance)
                {
                    GameController.GetInstance().ActionMessage("Right", "멧돼지의 시야에서 벗어났습니다.", LookTarget);
                    state = State.Idle;
                    LookTarget.GetComponent<SurvivorStatus>().SetBW(null);
                    LookTarget = null;
                }
                else
                {
                    aiPath.speed = walkSpeed - 1.5f;
                }

                //시야범위 안에 또 다른 타겟이 왔을때 더 가까우면 교환
                if (SightRange.getSurvivors() != null)
                {
                    if (LookTarget != SightRange.getSurvivors())
                    {
                        if (GetDistance(SightRange.getSurvivors()) < GetDistance(LookTarget))
                        {
                            GameController.GetInstance().ActionMessage("Right", "다른 상대보다 멧돼지와 더 멀어져 시야에서 벗어났습니다.", LookTarget);
                            LookTarget = SightRange.getSurvivors();
                            GameController.GetInstance().ActionMessage("Wrong", "다른 상대보다 멧돼지와 더 가까워 대상이 되었습니다.", LookTarget);
                        }
                    }
                }
                //타겟이 뛰거나 뒤로 돌면 쫒아감
                if (!umbrellaMoveFlag)
                {
                    StartCoroutine(LookToChase(lookToChaseTimer, currentSpeed, LookTarget));
                }
            }
            yield return 0;
        }
        //벗어날때
        if (Audio != null)
        {
            Audio.Stop("Walk");
            Audio.Stop("Look");

        }
        GoToNextState();
    }
    IEnumerator Chase()
    {
        if (Audio != null)
        {
            Audio.Play("Walk");
        }
        aiPath.speed = runSpeed;
        aiPath.rotationSpeed = rotationDamping;
        while (state == State.Chase)
        {
            //업데이트할때
            if (ChaseTarget != null)
            {
                float dist = GetDistance(ChaseTarget);
                animator.SetFloat("Speed_f", aiPath.speed / runSpeed);
                aiPath.target = ChaseTarget.transform;
                aiPath.rotationSpeed = rotationDamping;
                float angle = Quaternion.Angle(ChaseTarget.transform.rotation, transform.rotation);

                // 각도에 따라 추격거리 계산속도 및 이동속도 조절
                if (angle < 45)
                {
                    aiPath.repathRate = 0.1f;
                    aiPath.speed = 14;
                }
                else
                {
                    aiPath.repathRate = 1f;
                    aiPath.speed = 14;
                }

                LookTarget = null;
                if (ChaseTarget.GetComponent<SurvivorStatus>().UmbrellaState)
                {
                    if (dist < 5.0f)
                    {
                        aiPath.speed = 0;
                        animator.SetFloat("Speed_f", 0);
                        StartCoroutine(ObstacleDistinction(obstacleDistinctionTimer, ChaseTarget));
                    }
                }
                else if (dist < 2.0f)
                {
                    aiPath.speed = 0f;      //가까이 가면 속도 줄어듦
                    aiPath.rotationSpeed = 360f;
                }
                else if (dist > sightDistance)                //시야에서 벗어나면 다시 Patrol
                {
                    GameController.GetInstance().ActionMessage("Right", "멧돼지의 시야에서 벗어났습니다.", ChaseTarget);
                    state = State.Idle;
                    ChaseTarget.GetComponent<SurvivorStatus>().SetBW(null);
                    ChaseTarget = null;
                }
                else
                {
                    aiPath.speed = runSpeed;
                }


                //Chase상태에서 공격범위 안에 들어오면 공격
                if (AttackRange.IsAttack && !ChaseTarget.GetComponent<SurvivorStatus>().UmbrellaState)
                {
                    ChaseTarget = AttackRange.InTarget;
                    state = State.Attack;
                }

                //시야범위 안에 또 다른 타겟이 왔을때 더 가까우면 교환
                if (SightRange.getSurvivors() != null)
                {
                    if (ChaseTarget != SightRange.getSurvivors())
                        if (GetDistance(SightRange.getSurvivors()) < GetDistance(ChaseTarget))
                        {
                            GameController.GetInstance().ActionMessage("Right", "다른 상대보다 멧돼지와 더 멀어졌습니다.", ChaseTarget);
                            ChaseTarget = SightRange.getSurvivors();
                            GameController.GetInstance().ActionMessage("Wrong", "다른 상대보다 멧돼지와 더 가까워졌습니다.", ChaseTarget);
                        }
                }

                //ChaseTarget이 죽었다면 Patrol
                if (ChaseTarget.GetComponent<SurvivorStatus>().IsDead())
                {
                    state = State.Idle;
                    LookTarget = null;
                    ChaseTarget = null;
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

    IEnumerator Attack()
    {
        while (state == State.Attack)
        {
            if (Time.time > nextAttack)
            {
                aiPath.speed = 0f;
                animator.SetFloat("Speed_f", 0);
                aiPath.rotationSpeed = 360f;
                animator.Play("Attack");
                nextAttack = Time.time + delayTime;
            }
            else
            {
                animator.SetFloat("Speed_f", 0);
            }

            //공격 범위를 벗어나면 Chase상태로
            if (!AttackRange.IsAttack)
            {
                state = State.Chase;
            }

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

    IEnumerator ObstacleDistinction(float timer, GameObject target)
    {
        Vector3 currentPosition = target.transform.position;

        yield return new WaitForSeconds(timer);
        if (currentPosition == target.transform.position && !umbrellaMoveFlag && state != State.Attack)
        {
            GameController.GetInstance().ActionMessage("Right", "우산 뒤에 숨었습니다.", target);
            umbrellaMoveFlag = true;
            exceptionTarget = target;
            state = State.Patrol;
            StartCoroutine(ResetTarget(targetResetTimer, target));
            target = null;
        }
        else if (currentPosition != target.transform.position)
        {
            ChaseTarget = target;
            state = State.Attack;
        }
    }

    IEnumerator ResetTarget(float timer, GameObject target)
    {
        yield return new WaitForSeconds(timer);
        umbrellaMoveFlag = false;
        exceptionTarget = null;
    }

    IEnumerator LookToChase(float lookToChaseTimer, float currentSpeed, GameObject target)
    {
        yield return new WaitForSeconds(lookToChaseTimer);

        if (LookTarget != null)
        {
            float angle = Quaternion.Angle(target.transform.rotation, transform.rotation);
            if (angle < 80.0f || angle > 280.0f)
            {
                state = State.Chase;
                ChaseTarget = LookTarget;
                LookTarget = null;
                GameController.GetInstance().ActionMessage("Wrong", "멧돼지에게 등을 보였습니다.", ChaseTarget);
            }
            else if (currentSpeed > 5.0f)
            {
                state = State.Chase;
                ChaseTarget = LookTarget;
                LookTarget = null;
                GameController.GetInstance().ActionMessage("Wrong", "멧돼지에게서 뛰어서 도망갔습니다.", ChaseTarget);
            }
        }
    }

    // 시야 범위 안 레이 쏘기
    public override void InSightRange() 
    {
        rayPosition.x = transform.position.x;
        rayPosition.y = transform.position.y + 1.35f;
        rayPosition.z = transform.position.z;

        Vector3 survivorPosition = SightRange.getSurvivors().transform.position - transform.position;

        if (Physics.Raycast(rayPosition, survivorPosition, out rayHit, MAX_RAY_DISTANCE))
        {
            if (rayHit.collider.tag == "Player")
            {
                if (SightRange.getSurvivors().transform.gameObject != exceptionTarget)
                {
                    LookTarget = SightRange.getSurvivors().transform.gameObject;
                    state = State.Look;
                    LookTarget.GetComponent<SurvivorStatus>().SetBW(gameObject);
                }
            }
        }
    }
}