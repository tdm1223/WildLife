using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : SmallWildFSM
{
    public enum snakeColor
    {
        Green,
        Brown,
        Yellow
    }
    public snakeColor color; //뱀의 색깔 선택
    public float DOTTimer; //지속데미지 들어가는 딜레이
    public Material[] material = new Material[3];
    private int colorNum;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    bool runawayFlag;
    bool survivorIn;
    float orginSpeed;                   //원래속도

    void OnTriggerStay(Collider coll) //자기가 감지할수 있는 영역안에 생존자가 들어오면 추적
    {
        if (state == State.Idle || state == State.Guard) //Idle 이거나 Guard상태일때
        {
            if (coll.transform.root.tag == "Player") //들어온 콜라이더의 부모가 생존자일때
            {
                if (coll.transform.root.GetComponent<SurvivorStatus>().GarlicFlag || coll.transform.root.GetComponent<SurvivorStatus>().BellFlag)
                {
                    if (coll.tag == "SnakeRecogRangeCollider")
                    {
                        survivorIn = true;
                        Target = coll.transform.root.gameObject;
                        GoToTurnBack();
                    }
                }
                else
                {
                    if (coll.tag == "RecogRangeCollider") //일반적인 인식콜라이더에 반응
                    {
                        animator.SetBool("Idle", false);
                        animator.SetBool("Guard", true);
                        state = State.Guard;
                        Target = coll.transform.root.gameObject;
                    }
                }
            }
        }
    }
    void OnTriggerExit(Collider coll)
    {
        if ((state == State.Guard || state == State.Chase) && (coll.tag == "SnakeRecogRangeCollider" || coll.tag == "RecogRangeCollider"))
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Guard", false);
            state = State.Idle;
            Target = null;
        }
        if (state == State.TurnBack && coll.tag == "SnakeRecogRangeCollider")
        {
            survivorIn = false;
        }
    }

    void Awake()
    {
        animator = transform.GetComponent<Animator>();
        animator.SetBool("Guard", false);
        animator.SetBool("Idle", true);
        animator.SetBool("Attack", false);
    }

    public override void Start()
    {
        this.transform.rotation = new Quaternion(0,0,0,0);
        colorNum = (int)(Random.value * 10) % 3;
        if (colorNum == 0) color = snakeColor.Green;
        else if (colorNum == 1) color = snakeColor.Brown;
        else if (colorNum == 2) color = snakeColor.Yellow;
        //transform.FindChild("Snake_Mesh").GetComponent<SkinnedMeshRenderer>().material = material[(int)color];
        skinnedMeshRenderer.material = material[(int)color];
        //기본상태로 비맹수급은 가만히 있는 idle
        state = State.Idle;
        AttackRange = transform.Find("AttackRangeCollider").GetComponent<AttackRangeCollider>();

        base.Start();
    }
    //뱀은 똬리를 틀고있는 Idle
    // 경계안에 들어와있을때 몸을 세우고있는 Guard
    // 공격하는 Attack 상태밖에 없다.
    void FixedUpdate()
    {
        if (runawayFlag)   //타겟에서 방울소리나 마늘냄새가 난다면 도망
        {
            BackMove();
        }
    }

    //가만히 있는 상태
    IEnumerator Idle()
    {
        while (state == State.Idle)
        {
            //업데이트할때
            Target = null;
            yield return 0;
        }
        //벗어날때
        GoToNextState();
    }
    IEnumerator Guard()
    {
        if (Audio != null)
            Audio.Play("Guard");

        while (state == State.Guard)
        {
            //업데이트할때
            if (Target.transform.root.GetComponent<SurvivorStatus>().IsDead())
            {
                state = State.Idle;
            }
            lookAtPlayer(Target.transform.position, 0);
            if (AttackRange.IsAttack)
            {
                animator.Play("Snake_Attack");
                state = State.Attack;
            }
            yield return 0;
        }
        //벗어날때
        GoToNextState();
    }
    IEnumerator Attack()
    {
        if (Audio != null)
            Audio.Play("Attack");

        while (state == State.Attack)
        {
            //업데이트 할때
            if (Target.transform.root.GetComponent<SurvivorStatus>().IsDead())
            {
                state = State.Idle;
            }
            //공격범위 벗어나면 다시 추격
            if (!AttackRange.IsAttack)
            {
                animator.SetBool("Attack", false);
                state = State.Guard;
            }

            SurvivorStatus DOTTarget = Target.GetComponent<SurvivorStatus>();
            if (Target.transform.GetComponent<SurvivorStatus>().Infection== false)
            {
                GameController.GetInstance().ActionMessage("Wrong", "뱀에게 물렸습니다.", Target);
                Target.transform.GetComponent<SurvivorStatus>().Infection = true; // 감염상태 True로
                Target.transform.GetComponent<SurvivorStatus>().InfectionColor = getSnakeColor(); //감염된 뱀의 색깔 넘기기
                StartCoroutine(DamageOverTime(DOTTarget, DOTTimer));
            }
            else
            {
                DOTTarget.addtoHP((int)(-attackDamage), transform.position, gameObject.tag); //처음 맞을때 생명력 감소 
            }

            state = State.Guard;
            yield return new WaitForSeconds(4.0f);
        }
        //벗어날때
        GoToNextState();
    }

    IEnumerator DamageOverTime(SurvivorStatus target, float timer) //지속데미지 입히는 함수
    {
        SurvivorStatus DOTTarget = target;
        DOTTarget.addtoHP((int)(-attackDamage), transform.position, gameObject.tag); //처음 맞을때 생명력 감소 
        while (!target.transform.GetComponent<SurvivorStatus>().IsDead() && target.transform.GetComponent<SurvivorStatus>().Infection == true) // 죽지않고 감염되있을때
        {
            //타이머당 지속피해
            yield return new WaitForSeconds(timer);
            if (target.transform.GetComponent<SurvivorStatus>().Infection == true)
            {
                DOTTarget.addtoHP((int)(-attackDamage * 0.2), gameObject.tag);
            }
            else if (target.transform.GetComponent<SurvivorStatus>().Infection == false)
            {
                yield break;
            }
        }
    }
    IEnumerator TurnBack()      //마늘과 종에 반응해서 도망가는 함수
    {
        while (state == State.TurnBack)
        {
            animator.SetBool("Runaway", true);
            animator.SetBool("Idle", false);
            animator.SetBool("Guard", false);
            //StartCoroutine(ResetRunawayFlag());
            yield return 0;
        }
        GoToNextState();
    }
    public string getSnakeColor() //뱀 색깔 반환
    {
        return color.ToString();
    }
    void BackMove()  //Runaway 꽈리튼 후 실제 뒤로 이동 
    {
        //Debug.Log("뱀 도망 실행여부");
        transform.Translate(Vector3.forward * Time.deltaTime * 2);
        if (survivorIn == false)
        {
            //Debug.Log("서바이버아웃");
            StartCoroutine(ResetRunawayFlag());
        }
    }
    
    void SetRunawayFlag()
    {
        if (runawayFlag == false)
        {
            GameController.GetInstance().ActionMessage("Right", "뱀이 싫어하는 물건을 가지고 있습니다.", Target);
        }
        this.runawayFlag = true;   
    }
    IEnumerator ResetRunawayFlag()
    {
        yield return new WaitForSeconds(2.7f);
        this.runawayFlag = false;
        Target = null;
        state = State.Idle;
        animator.SetBool("Runaway", false);
        animator.SetBool("Idle", true);
    }

    void GoToTurnBack()
    {
        transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(Target.transform.position - transform.position).eulerAngles.y - 180, 0);
        state = State.TurnBack;
    }

}