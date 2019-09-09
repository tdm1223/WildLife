using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class flock : MonoBehaviour {

    public float speed;
    public float rotationSpeed; //얼마나 발리 회전하느냐
    Vector3 averageheading; //모든그룹이 향하는 지점
    Vector3 averagePosition; //
    float neighbourDistance = 3.0f; //

    bool turning = false;
    Beeflock beeflock;

	void Start () {
        speed = Random.Range(100, speed);

        beeflock = transform.root.GetComponent<Beeflock>();
      

	}

    

    void Update () {

      

       
        if (Vector3.Distance(transform.position, beeflock.transform.position) >= beeflock.tankSize)
        {
           // Debug.Log("터닝온");
            turning = true;
        }
        else
        {
           // Debug.Log("터닝폴스");
            turning = false;
        }

        if (turning) //중심으로 모음
        {
           // Debug.Log("터닝온 상태");

            Vector3 direction = beeflock.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                                                rotationSpeed * Time.deltaTime);
            speed = Random.Range(0.5f, 1);
         
        }
        else
        {
           // Debug.Log("어플라이 룰스");
            if (Random.Range(0,5) < 1)
            {
                ApplyRules();
            }


        }
         
        transform.Translate(0, 0, Time.deltaTime * speed);
      
	}

    void ApplyRules()
    {
        GameObject[] gos;
     
        gos = beeflock.allBee;

        //gos = globalflock.allBee;
        //goalPos 

        Vector3 vcentre = Vector3.zero; //평균 중심점을 구하기 위한것 

        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.1f;


        Vector3 goalPos = beeflock.goalPos; //모든 군집 객체들을 가져온다. 
        float dist;

        int groupsize = 0;

        foreach(GameObject go in gos)
        {
            if(go == null)
            {
                Debug.Log("빔");

            }
           
                
            dist = Vector3.Distance(go.transform.position, this.transform.position);//나자신과 다른 군집개체와의 거리계산 

            if (dist <= neighbourDistance)//그거리가 내가 설정한 개체간 거리보다 작으면 
            {
                vcentre += go.transform.position; //더한다 
                groupsize++; //증가시켜줌 나중에 무언가 평균을 구할때 사용됨

                if(dist < 1.0f) // 너무 작으면 간격을 유지시켜주기위해 
                {
                     vavoid = vavoid + (this.transform.position - go.transform.position);
                }

                flock anotherFlock = go.GetComponent<flock>();
                gSpeed = gSpeed + anotherFlock.speed;


            }

        }
        if(groupsize > 0)
        {
            vcentre = vcentre / groupsize + (goalPos = this.transform.position);
            speed = gSpeed / groupsize;

            Vector3 direction = (vcentre + vavoid) - transform.position;//군집 방향으로 비틈???
   

  
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                    rotationSpeed * Time.deltaTime);

        }
    }
}
