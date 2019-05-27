using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SurvivorController : NetworkBehaviour
{

    SurvivorRecogRangeCollider cont;

    private CharacterController controller;
    private SurvivorStatus survivorStatus;
    private float gravity = 1f;
    private Vector3 movement = Vector3.zero;
    private Animator playerAnimator;
    private SurvivorAudio survivorAudio;

    public GameObject FreeLookCameraRig;    //캐릭터를 따라오는 카메라의 FreeLookCameraRig
    public UILookingItemText uiLookingItemText;
    private float moveSpeed;
    public float walkSpeed;      //걷는 속도
    public float runSpeed;       //뛰는 속도
    private float runSpeedBackUp;
    public float sneakSpeed;     //기는 속도
    public float jumpSpeed;      //점프량

    bool Action;    //e키
    //bool Fire1;     //왼클릭
    bool Fire2;     //우클릭
    bool Sneak;     //좌 Ctrl키
    int ItemPos;    //활성화된 아이템 
    int UsingItemPos;

    private Vector3 ScreenMidPoint;
    private SurvivorInventory Inventory;
    private Item UsingItem = null; //현재 들고있는 아이템
    private GameObject ragDoll;     //레그돌
    public GameObject EquipPoint;//이것은 자신의 아이템 equippoint 를 저장한다. 

    //동물관련 recogcollider
    private GameObject bearCollider;
    private GameObject beeCollider;
    private GameObject boarCollider;
    private GameObject snakeCollider;
    private GameObject recogRangeColliderGroup;

    private bool umbrellaState;
    private bool waterCheck = false;
    //[SyncVar]
    //public bool garlicFlag;
    //[SyncVar]
    //public bool bellFlag;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 4)
            waterCheck = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 4)
            waterCheck = false;
    }

    public void Pickup(Item item) //아이템을 줍는다. 
    {
        Inventory.AddItem(item); //인벤토리에 아이템을 넣는다. 
    }

    void SelectItem(int pos)
    {
        if (Inventory.isNotNull(pos))//해당칸이 비어있지 않으면
        {
            UsingItemPos = pos;

            if (UsingItem != null) //기존의 착용하고 있던 아이템 분리 
            {
                SendMsgManager.GetInstance().UnEquipItem(UsingItem.gameObject);
                //UsingItem.UnHold();
            }

            UsingItem = Inventory.getItem(UsingItemPos);

            SendMsgManager.GetInstance().UnEquipItem(UsingItem.gameObject);
            SendMsgManager.GetInstance().EquipItem(UsingItem.gameObject);
            //UsingItem.Hold();
            Inventory.uiInventory.SendMessage("SelectItem", pos, SendMessageOptions.DontRequireReceiver);
        }
    }
    void Start()
    {
        cont = transform.GetComponent<SurvivorRecogRangeCollider>();

        controller = GetComponent<CharacterController>();
        survivorStatus = GetComponent<SurvivorStatus>();
        playerAnimator = this.gameObject.GetComponent<Animator>();
        Inventory = GetComponent<SurvivorInventory>();
        survivorAudio = GetComponent<SurvivorAudio>();
        ragDoll = GameObject.Find("/Survivor/Bip001/Bip001 Pelvis");    //레그돌
        ScreenMidPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0f);

        BearCollider = transform.Find("RecogRangeColliderGroup").Find("BearRecogRangeCollider").gameObject;
        BeeCollider = transform.Find("RecogRangeColliderGroup").Find("BeeRecogRangeCollider").gameObject;
        BoarCollider = transform.Find("RecogRangeColliderGroup").Find("BoarRecogRangeCollider").gameObject;
        SnakeCollider = transform.Find("RecogRangeColliderGroup").Find("SnakeRecogRangeCollider").gameObject;
        recogRangeColliderGroup = transform.Find("RecogRangeColliderGroup").gameObject;

        Action = false;
        //Fire1 = false;
        Fire2 = false;
        Sneak = false;

        runSpeedBackUp = runSpeed;
    }
    void FixedUpdate()
    {
        if (survivorStatus.IsDead()) //레그돌 실행
        {
            playerAnimator.enabled = false;
        }
        else if (!survivorStatus.IsDead())
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            /* 땅에 붙어있을 때 이동 */
            if (controller.isGrounded)
            {
                moveSpeed = 0;  //가만히 있으면 스피드 0 아이들상태
                if ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
                {
                    if (waterCheck == true)
                    {
                        moveSpeed = walkSpeed - 1;
                        survivorAudio.SendMessage("StopWalkSound", null, SendMessageOptions.DontRequireReceiver);
                        survivorAudio.SendMessage("PlayWaterWalkSound", null, SendMessageOptions.DontRequireReceiver);
                        if (Input.GetButton("Run"))
                        {
                            moveSpeed = runSpeed - 2;   //걷기 뛰기 기기 키에 따른 속도 변경
                            survivorAudio.SendMessage("PlayWaterRunSound", null, SendMessageOptions.DontRequireReceiver);
                        }
                        else
                            survivorAudio.SendMessage("StopWaterRunSound", null, SendMessageOptions.DontRequireReceiver);

                        if (Sneak == true)
                        {
                            moveSpeed = sneakSpeed; //앉기 스피드 조절
                            survivorAudio.SendMessage("PlayWaterSneakSound", null, SendMessageOptions.DontRequireReceiver);
                        }
                        else
                            survivorAudio.SendMessage("StopWaterSneakSound", null, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        moveSpeed = walkSpeed;
                        survivorAudio.SendMessage("StopWaterWalkSound", null, SendMessageOptions.DontRequireReceiver);
                        survivorAudio.SendMessage("PlayWalkSound", null, SendMessageOptions.DontRequireReceiver);
                        if (Input.GetButton("Run"))
                        {
                            moveSpeed = runSpeed;   //걷기 뛰기 기기 키에 따른 속도 변경
                            survivorAudio.SendMessage("PlayRunSound", null, SendMessageOptions.DontRequireReceiver);
                        }
                        else
                            survivorAudio.SendMessage("StopRunSound", null, SendMessageOptions.DontRequireReceiver);

                        if (Sneak == true)
                        {
                            moveSpeed = sneakSpeed; //앉기 스피드 조절
                            survivorAudio.SendMessage("PlaySneakSound", null, SendMessageOptions.DontRequireReceiver);
                        }
                        else
                            survivorAudio.SendMessage("StopSneakSound", null, SendMessageOptions.DontRequireReceiver);
                    }
                }
                else
                {
                    survivorAudio.SendMessage("StopWalkSound", null, SendMessageOptions.DontRequireReceiver);
                    survivorAudio.SendMessage("StopWaterWalkSound", null, SendMessageOptions.DontRequireReceiver);
                }


                //if (moveVertical < 0) moveSpeed = sneakSpeed;
                movement = transform.forward * moveVertical + transform.right * moveHorizontal; //캐릭터가 보고있는 방향을 기준으로 이동                
                movement = movement.normalized * moveSpeed * Time.deltaTime;

                if (Input.GetButton("Jump"))
                {
                    movement.y = jumpSpeed * 0.1f;
                    playerAnimator.SetBool("Jump", true);
                    survivorAudio.SendMessage("PlayJumpSound", null, SendMessageOptions.DontRequireReceiver);
                    survivorAudio.SendMessage("StopWalkSound", null, SendMessageOptions.DontRequireReceiver);
                }
                else
                    playerAnimator.SetBool("Jump", false);
            }

            movement.y -= gravity * Time.deltaTime; //중력

            controller.Move(movement);  //움직임 벡터 적용

            if (controller.isGrounded) movement.y = 0f; //땅일때는 중력 없음

            /* 카메라가 보는 방향으로 캐릭터 회전, Fire2를 누르는 동안 캐릭터 회전 무시 */
            if (Input.GetButtonDown("Fire2")) Fire2 = true;
            else if (Input.GetButtonUp("Fire2")) Fire2 = false;

            if (!Fire2) transform.rotation = Quaternion.Slerp(transform.rotation, FreeLookCameraRig.transform.rotation, 10f * Time.deltaTime);

            playerAnimator.SetFloat("S_Speed", Mathf.Sqrt(movement.x * movement.x + movement.z * movement.z) * 50); //걷기,뛰기,가만히 상태애니메이션을 위한 파라미터 셋팅
        }
        else
            survivorAudio.SendMessage("StopAllSound", null, SendMessageOptions.DontRequireReceiver);
    }
    void Update()       // (걸으면서 앉기 등에 인식 못하는 경우가 많아 따로 뺌)
    {
        Ray ray = Camera.main.ScreenPointToRay(ScreenMidPoint);
        RaycastHit hit;
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 8f, Color.blue, Time.deltaTime);

        if (Physics.Raycast(ray, out hit, 8f, (1 << 8)))
        {
            uiLookingItemText.SendMessage("UpdateLookingItemText", hit.transform.gameObject.GetComponent<Item>(), SendMessageOptions.DontRequireReceiver);  //아이템 이름 표시 UI
            if (Input.GetButtonDown("Action"))
            {
                Action = true;

                if (hit.transform.tag.Equals("Item"))
                {
                    playerAnimator.Play("PickUp");          //픽업 애니메이션 실행
                    hit.transform.gameObject.SendMessage("GetItem", this, SendMessageOptions.DontRequireReceiver);  //아이템을 에임에 대고 Action 버튼을 누르면 SendMessage
                }
            }
            else
                Action = false;
        }
        else
            uiLookingItemText.SendMessage("UnableLookingItemText", null, SendMessageOptions.DontRequireReceiver);   //아이템 이름 표시 UI 없애기

        if (Input.GetButton("Sneek"))
        {
            if (Sneak == false)
            {
                Sneak = true;
                playerAnimator.SetBool("Sit", true);
            }
        }
        else
        {
            Sneak = false;
            playerAnimator.SetBool("Sit", false);
        }


        if (!survivorStatus.IsDead())
        {
            //아이템선택,버림
            if (Input.GetButtonDown("1"))
            {
                SelectItem(0);
                UmbrellaCheck();
            }
            else if (Input.GetButtonDown("2"))
            {
                SelectItem(1);
                UmbrellaCheck();
            }
            else if (Input.GetButtonDown("3"))
            {
                SelectItem(2);
                UmbrellaCheck();
            }
            else if (Input.GetButtonDown("4"))
            {
                SelectItem(3);
                UmbrellaCheck();
            }

            else if (Input.GetButtonDown("5"))
            {
                SelectItem(4);
                UmbrellaCheck();
            }
            else if (Input.GetButtonDown("6"))
            {
                SelectItem(5);
                UmbrellaCheck();
            }

            if (Input.GetButtonDown("Throw")) //해당 아이템을 필드에 버린다. 
            {
                if (UsingItem != null)
                {
                    if (UsingItem.Kind == 102)  //우산 버리는지 판단
                    {
                        UmbrellaThrow(UsingItem);
                    }

                    Inventory.ThrowItem(UsingItemPos);
                    //UsingItem.UnHold();

                    if (survivorStatus.bigWild != null && survivorStatus.bigWild.tag == "Bear")      //보이거나 추격당할때 아이템 버리면 곰에게 관심끌게하기
                    {
                        StartCoroutine(WaitThrowItem());
                        //bigWild.GetComponent<Bear>().SendMessage("AttentionItem", Inventory.GetThrowItem(), SendMessageOptions.DontRequireReceiver);
                    }

                    if (UsingItem.useCount < 1)
                    {
                        //UsingItem.UnHold();
                        //UsingItem.useCount = 1;//다시 초기화 시켜줌
                        //Inventory.RemoveItem(UsingItemPos);
                        UsingItem = null;
                    }
                    else
                    {
                        UsingItem = null;
                        Inventory.uiInventory.SendMessage("NotSelect", UsingItemPos, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            if (Input.GetButtonDown("Use"))
            {
                if (UsingItem != null)
                {
                    Inventory.ItemUse(UsingItemPos);
                    if (UsingItem.useCount == 0)
                    {
                        //Inventory.RemoveItem(UsingItemPos);
                        UsingItem = null;
                    }
                }
            }
            if (Input.GetButtonDown("Hi"))       //  감정표현 t버튼 Hi  Int값 1 , F버튼 Hello Int값 2 dance 버튼 3 
            {                                   // 일반 감정표현 해제 숫자 0 , 댄스 해제 -1
                playerAnimator.SetInteger("EmotionNum_Int", 1);
            }
            if (Input.GetButtonDown("Hello"))
            {
                playerAnimator.SetInteger("EmotionNum_Int", 2);
            }

            if (Input.GetButtonDown("Dance"))
            {
                playerAnimator.SetInteger("EmotionNum_Int", 3);
            }
            else if ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) && (playerAnimator.GetInteger("EmotionNum_Int") == 3))
                playerAnimator.SetInteger("EmotionNum_Int", 0);
        }
    }

    IEnumerator WaitThrowItem()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            if (Inventory.GetThrowItem() != null)
            {
                SendMsgManager.GetInstance().BearAttention(survivorStatus.bigWild, Inventory.GetThrowItem());
                break;
            }

        }
    }


    public bool GetEmptySLotBool() //인벤토리에 비어있는 슬로이 있는지 확인하는 함수
    {

        if (Inventory.GetEmptySlotCount() > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    public float getMoveSpeed()
    {
        return controller.velocity.magnitude;
    }

    public GameObject BearCollider
    {
        get
        {
            return bearCollider;
        }
        set
        {
            bearCollider = value;
        }
    }
    public GameObject BeeCollider
    {
        get
        {
            return beeCollider;
        }
        set
        {
            beeCollider = value;
        }
    }
    public GameObject BoarCollider
    {
        get
        {
            return boarCollider;
        }
        set
        {
            boarCollider = value;
        }
    }
    public GameObject SnakeCollider
    {
        get
        {
            return snakeCollider;
        }
        set
        {
            snakeCollider = value;
        }
    }
    public bool WaterCheck
    {
        get
        {
            return waterCheck;
        }
        set
        {
            waterCheck = value;
        }
    }

    public void PerfumeDuration() //향수 사용시 벌에 대한 감지범위 지속시간 설정
    {
        StartCoroutine(BeePerfumeDurationTime(15.0f));
    }

    public void SandwichDuration() //샌드위치 먹을시 곰과 멧돼지에 대한 감지범위 지속시간 설정
    {

        StartCoroutine(BearDurationTime(2.0f));
        StartCoroutine(BoarDurationTime(2.0f));
    }

    public void CanDuration() //통조림 먹을시 곰에 대한 감지범위 지속시간 설정
    {

        StartCoroutine(BearDurationTime(5.0f));
    }
    public void BreakTimeDuration() //초코바 먹을시 곰과 벌에 대한 감지범위 지속시간 설정
    {
        StartCoroutine(BeeDurationTime(2.0f));
        StartCoroutine(BearDurationTime(2.0f));
    }
    public void HoneyPotDuration() // 꿀단지 먹을시 곰과 벌에 대한 감지범위 지속시간 설정
    {
        StartCoroutine(BeeDurationTime(5.0f));
        StartCoroutine(BearDurationTime(5.0f));
    }
    public void GarlicDuration() //마늘 먹을시 곰과 뱀에 대한 감지범위 지속시간 설정
    {
        StartCoroutine(BearDurationTime(2.0f));
        StartCoroutine(SnakeDurationTime(2.0f));
    }
    public void BellDuration()
    {
        StartCoroutine(SnakeDurationTime(3.0f));
    }

    IEnumerator BeeDurationTime(float duration) //지속시간후에 벌에 대한 감지범위의 콜라이더 비활성화
    {
        yield return new WaitForSeconds(duration);
        //일정시간후에 그게 마지막 아이템이었다면 콜라이더 비활성화
        if (transform.GetComponent<SurvivorInventory>().CheckLastItemFlag)
        {
            cont.CmdSetBeeColliderEnable(false);
        }
        else //아니면 passive 상태의 콜라이더 유지
        {
            cont.CmdSetBeeColliderRadius(10);
        }
    }

    IEnumerator BeePerfumeDurationTime(float duration) //지속시간후에 벌에 대한 감지범위의 콜라이더 비활성화
    {
        cont = transform.GetComponent<SurvivorRecogRangeCollider>();
        yield return new WaitForSeconds(duration);
        cont.CmdSetBeeColliderEnable(false);
    }

    IEnumerator BearDurationTime(float duration) //지속시간후에 곰에 대한 감지범위의 콜라이더 비활성화
    {
        yield return new WaitForSeconds(duration);
        //일정시간후에 그게 마지막 아이템이었다면 콜라이더 비활성화
        if (transform.GetComponent<SurvivorInventory>().CheckLastItemFlag)
        {
            cont.CmdSetBearColliderEnable(false);
        }
        else //아니면 passive 상태의 콜라이더 유지
        {
            cont.CmdSetBearColliderRadius(10);
        }
    }

    IEnumerator BoarDurationTime(float duration) //지속시간후에 멧돼지에 대한 감지범위의 콜라이더 비활성화
    {
        yield return new WaitForSeconds(duration);
        //일정시간후에 그게 마지막 아이템이었다면 콜라이더 비활성화
        if (transform.GetComponent<SurvivorInventory>().CheckLastItemFlag)
        {
            cont.CmdSetBoarColliderEnable(false);
        }
        else //아니면 passive 상태의 콜라이더 유지
        {
            cont.CmdSetBoarColliderRadius(10);
        }
    }

    IEnumerator SnakeDurationTime(float duration) //지속시간후에 뱀에 대한 감지범위의 콜라이더 비활성화
    {
        yield return new WaitForSeconds(duration);
        //일정시간후에 그게 마지막 아이템이었다면 콜라이더 비활성화
        if (transform.GetComponent<SurvivorInventory>().CheckLastItemFlag)
        {
            cont.CmdSetSnakeColliderEnable(false);
        }
        else //아니면 passive 상태의 콜라이더 유지
        {
            cont.CmdSetSnakeColliderRadius(1.5f);
        }
    }

    public bool UmbrellaState
    {
        get
        {
            return umbrellaState;
        }
        set
        {
            umbrellaState = value;
            if (umbrellaState == false)
            {
                playerAnimator.SetBool("UmbrellaOn", false);
            }
            else if (umbrellaState == true)
            {
                playerAnimator.SetBool("UmbrellaOn", true);
            }
        }
    }

    public void UmbrellaCheck()     //우산 피고있는상태로 다른 아이템바꾸면 들고있는 애니메이션 풀리고 우산 접은상태로바꿔놓는
    {
        if (UmbrellaState == true)
        {
            playerAnimator.SetBool("UmbrellaOn", false);

            if (UsingItem != null)
            {
                if (UsingItem.Kind == 102)
                {
                    playerAnimator.SetBool("UmbrellaOn", true);
                }
            }
        }
    }

    public void UmbrellaThrow(Item Umbrella)
    {
        UmbrellaState = false;
        Umbrella.gameObject.GetComponent<Animator>().SetBool("UmbrellaOn", false);
    }

    public void Emotion_ResetInt()
    {
        playerAnimator.SetInteger("EmotionNum_Int", 0);
    }

    [Command]
    public void CmdSpawnObjcet(GameObject obj, Vector3 point, Quaternion q)
    {
        GameObject effect = Instantiate(obj, point, q);
        NetworkServer.Spawn(effect);
        DsetroyEffect(effect);
    }

    public void DestroyEffect(GameObject obj)
    {
        StartCoroutine(DsetroyEffect(obj));
    }

    IEnumerator DsetroyEffect(GameObject obj)
    {
        yield return new WaitForSeconds(2.0f);
        NetworkServer.Destroy(obj);
    }

    public void SetUpRunSpeed(float value)
    {
        if (runSpeed == runSpeedBackUp)
        {
            runSpeed *= value;
        }
    }

    public void SetBackRunSpeed()
    {
        if (runSpeed != runSpeedBackUp)
        {
            runSpeed = runSpeedBackUp;
        }
    }
}