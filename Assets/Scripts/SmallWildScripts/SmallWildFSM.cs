using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallWildFSM : MonoBehaviour
{
    public enum State
    {
        Idle,
        Chase,
        Attack,
        Guard,
        Look,
        TurnBack
    }

    [Header("야생동물 변수")]
    [Tooltip("이동 속도")]
    public float speed;
    [Tooltip("회전 속도")]
    public float rotationDamping;
    [Tooltip("공격 데미지")]
    public float attackDamage;

    protected State state;
    protected Animator animator;  //애니메이터
    protected ObjectAudio Audio;
    protected AttackRangeCollider AttackRange;

    private GameObject target;
    private Vector3 targetPosition; //타겟 머리쪽으로 추적 공격
    private Vector3 currentPosition;

    int targetID;

    public virtual void Start()
    {
        if(GetComponent<ObjectAudio>() != null)
            Audio = GetComponent<ObjectAudio>();
        GoToNextState();
    }

    //상태를 바꾸는 함수
    public void GoToNextState()
    {
        //호출하기 원하는 함수의 이름
        string methodName = state.ToString();

        //클래스에서 해당 상태의 함수를 검색
        System.Reflection.MethodInfo info = GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }

    //플레이어를 쳐다보는 함수, plusY는 쳐다볼 최종 위치의 높이
    public void lookAtPlayer(Vector3 position, float plusY) 
    {
        targetPosition = position;
        targetPosition.y += plusY;
        Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationDamping);
    }

    // target과 Smallwild사이의 거리를 반환
    public float GetDistance(GameObject target)
    {
        return (target.transform.position - transform.position).magnitude;
    }

    public string SmallWildState
    {
        get
        {
            return state.ToString();
        }
        set
        {
            switch (value)
            {
                case "Idle":
                    state = State.Idle;
                    break;
                case "Chase":
                    state = State.Chase;
                    break;
                case "Attack":
                    state = State.Attack;
                    break;
                case "Guard":
                    state = State.Guard;
                    break;
                case "Look":
                    state = State.Look;
                    break;
            }
        }
    }

    public GameObject Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }

    public Vector3 TargetPosition
    {
        get
        {
            return targetPosition;
        }
        set
        {
            targetPosition = value;
        }
    }

    public Vector3 CurrentPosition
    {
        get
        {
            return currentPosition;
        }
        set
        {
            currentPosition = value;
        }
    }

}
