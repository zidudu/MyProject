using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{


    //const int countOfDamageAnimations = 3;
    //int lastDamageAnimation = -1;


    // Start is called before the first frame update
    // 컴포넌트를 캐시 처리할 변수
    [SerializeField]
    private Transform tr;

    //Animator 컴포넌트
    private Animation anim;

    //이동 속도 변수(public으로 선언되어 인스펙터 뷰에 노출됨
    public float moveSpeed = 10.0f;
    public float runSpeed = 20.0f;  // 달리기 속도
    //회전 속도 변수
    public float turnSpeed = 80.0f;

    // 점프 힘 변수
    public float jumpForce = 10.0f;

    //초기 생명 값
    private readonly float initHp = 100.0f;
    //현재 생명 값
    public float currHp;

    //HPbar 연결할 변수
    private Image hpBar;
    //Dashbar 연결할 변수
    private Image DashBar;

    //델리게이트 선언 
    public delegate void PlayerDieHandler();


    //이벤트 선언 //이벤트가 언제 호출될지 모르기 때문에 정적 변수로 선언해야함.
    //onPlayerDie는 이벤트명이라고 하지만 변수의 일종이며 playerdiehandler는 onplayerdie 변수의 타입일 뿐이다. 
    public static event PlayerDieHandler OnPlayerDie;

    //점프 여부 확인을 위한 변수
    private bool isGrounded = true;
    // Rigidbody 컴포넌트를 저장할 변수 
    private Rigidbody rb;

    //초기 대쉬 값
    private readonly float initDash = 100.0f;
    //현재 대쉬 값
    public float currDash;

    //기본 이동 속도
    float currentSpeed;


    //private bool isDashCooldown = false; // 대시 쿨다운 상태
    //public float dashCooldownTime = 1.0f; // 대시 재충전 대기 시간 (초)

    //슬라이딩 속도
    public float SlidingSpeed = 6.0f;

    //대쉬할때 대쉬 게이지 소모 값 정의
    public float consume_Dash = 10.0f;
    //슬라이딩할때 대쉬 게이지 소모 값 정의
    public float consume_Sliding = 35.0f;



    // 추가: Collider를 저장할 변수
    private CapsuleCollider playerCollider;

    // 슬라이딩 상태에서 사용할 콜라이더 크기와 중심점
    private Vector3 originalColliderCenter;
    private float originalColliderHeight;
    private float originalColliderRadius;

    private Vector3 slidingColliderCenter = new Vector3(0, 0.5f, 0);
    private float slidingColliderHeight = 1.0f;
    private float slidingColliderRadius = 0.4f;


    public float h, v, r;

    // 현재 상태를 나타내는 bool 변수 (이 변수들은 다른 로직에서 세팅한다고 가정)
    //bool isAiming = false;
    //bool isShooting = false;
    //bool isReloading = false;


    /* bool isSit = false*/

    bool isReloading = false; // 재장전 상태 확인 변수

    IEnumerator Start()
    {


        //HPbar 연결
        hpBar = GameObject.FindGameObjectWithTag("HP_BAR")?.GetComponent<Image>(); //hpbar 태그를 찾았니? 찾았으면 image컴포넌트를 hpbar에 넘김
        //Dashbar 연결
        DashBar = GameObject.FindGameObjectWithTag("DASH_BAR")?.GetComponent<Image>(); //hpbar 태그를 찾았니? 찾았으면 image컴포넌트를 hpbar에 넘김

        //HP 초기화
        currHp = initHp;

        //Dash 초기화
        currDash = initDash;

        ////현재 속도 초기화
        currentSpeed = moveSpeed;

        //체력 바 보여주는 함수 호출
        DisplayHealth();

        //체력바 로그 출력
        Debug.Log($"Player hp = {currHp / initHp}");
        // Debug.Log($"Player hp : {currHp} / {initHp}={currHp/initHp}");


        // transform 컴포넌트를 추출해 변수에 대입
        //스크립트가 실행되면 제일 먼저 호출되는 start 함수에서 해당 컴포넌트 추출해 저장함
        tr = GetComponent<Transform>();
        //Unity에서 특정 오브젝트에 연결된 Animation 컴포넌트를 가져와,
        //스크립트에서 사용할 수 있도록 변수에 할당하는 코드입니다. 이를 통해 애니메이션을 코드로 제어할 수 있게 됩니다.
        // Start 함수 내

        // ...
        anim = GetComponent<Animation>();
        //애니메이션 초기화
        //애니메이션 실행
        //anim.Play("Idle_Rifile");
        PlayerAnim(0.0f, 0.0f, "IDLE");


        // Animator Controller에서 초기 상태가 Idle이라고 가정
        // 별도로 animator.Play("Idle")를 호출할 필요는 없을 수 있음
        // 애니메이터 파라미터 초기화 등 필요하다면 여기서 처리
        //yield return null;

        // Rigidbody 컴포넌트 추출
        rb = GetComponent<Rigidbody>();
        // 회전 고정 (X, Z 축)
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        ////애니메이션 실행
        //anim.Play("Idle");
        //tr=GetComponent("Transform") as Transform;
        //tr =(Transform)GetComponent(typeof(Transform));


        // CapsuleCollider 컴포넌트 저장
        playerCollider = GetComponent<CapsuleCollider>();

        // 원래 콜라이더 크기 저장
        originalColliderCenter = playerCollider.center;
        originalColliderHeight = playerCollider.height;
        originalColliderRadius = playerCollider.radius;
        yield return null;

        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.5f);
        turnSpeed = 280.0f;



    }
    // 재장전 코루틴
    IEnumerator Reloading(float h, float v)
    {
        // 재장전 시간 대기 (예: 2초)
        yield return new WaitForSeconds(2.0f);

        // 재장전 완료 처리 (GameManager에서 재장전 로직 마무리)
        //GameManager.instance.FinishReload();

        // 재장전 끝나면 다시 이동 가능 상태로 복원
        isReloading = false;
        PlayerAnim(h, v, "WALK"); // 재장전 완료 후 걷기 상태로 복귀
    }    // Update is called once per frame
    void Update()
    {

        //키보드 A,D나 left,right 키 눌렀을 때 -1.0f부터 +1.0f까지의 값을 반환함
        //키보드 누르지 않을 때는 둘다 0.0 값이 반환됨
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        //GetAxis("Mouse X"): 이 함수는 **마우스가 X축(왼쪽/오른쪽)**으로 얼마나 움직였는지를 숫자로 반환합니다.
        //마우스의 좌우 이동 값을 받음
        r = Input.GetAxis("Mouse X");



        ////점프
        //if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        //{
        //    Jump();
        //}
        ////재장전
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    if (GameManager.instance != null && GameManager.instance.CurrentAmmo <= 0)
        //    {
        //        // 장전을 위한 트리거 설정 (Animator Controller에서 Reload 트리거 전환)
        //        animator.SetTrigger("Reload");
        //        GameManager.instance.StartReload();
        //    }
        //}
        // 재장전 키를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (GameManager.instance != null && GameManager.instance.CurrentAmmo <= 0)
            {
                isReloading = true;
                PlayerAnim(h, v, "RELOAD");   // 재장전 애니메이션 시작
                GameManager.instance.StartReload(); // GameManager에서 재장전 로직 시작
                StartCoroutine(Reloading(h, v));    // 플레이어 측에서 2초 대기 후 WALK로 복귀
                    }
        }

        // 재장전 중에는 Update에서 다른 애니메이션으로 덮어쓰지 않도록 처리
        if (isReloading)
        {
            // 재장전 중이므로 다른 이동, 걷기 모션 호출 X
            return;
        }


        // 기본 이동 속도를 초기 설정
        currentSpeed = moveSpeed;

        // 대시 조건 확인
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (currDash > 0) // 대시 값이 남아있는 경우
            {

                currentSpeed = runSpeed; // 대시 속도
                PlayingDash(10.0f); // 대시 값 감소
                PlayerAnim(h, v, "DASH"); // 대쉬 모션
            }
            //쉬프트 키를 때거나 대시 값이 없는 경우
            else if (Input.GetKeyUp(KeyCode.LeftShift) || currDash <= 0)
            {
                Debug.Log("Dash is not available.");
                currentSpeed = moveSpeed; //기본 속도로 돌아옴
                PlayerAnim(h, v, "WALK"); // 걷기 모션
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                Dash_Sliding();
            }
        }
        //}
        //// 대쉬 시작 감지
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    if (currDash > 0)
        //    {
        //        currentSpeed = runSpeed; // 대쉬 속도 고정
        //        PlayingDash(consume_Dash);
        //        //PlayerAnim(h, v, "DASH");
        //    }
        //    else
        //    {
        //        Debug.Log("Dash is not available.");
        //    }
        //}
        //// 대쉬 유지 (Shift 누르고 있는 동안)
        //else if (Input.GetKey(KeyCode.LeftShift))
        //{
        //    // 대쉬 게이지 남아있다면 계속 runSpeed 유지
        //    if (currDash > 0)
        //    {
        //        currentSpeed = runSpeed;
        //        PlayingDash(consume_Dash);
        //    }
        //    else
        //    {
        //        Debug.Log("No dash left.");
        //    }
        //}
        //// 대쉬 종료 감지
        //else if (Input.GetKeyUp(KeyCode.LeftShift))
        //{
        //    // 키를 떼는 순간 기본 속도로 복구
        //    currentSpeed = moveSpeed;
        //    PlayerAnim(h, v, "WALK");
        //}
        //슬라이딩 키 ctrl을 눌렀을때 슬라이딩 모션이 나오고 속도 빨라짐.
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            if (currDash > 0) // 대시 값이 남아있는 경우
            {

                currentSpeed += SlidingSpeed; //기존 속도에 추가
                PlayingDash(consume_Sliding); //슬라이딩 값 감소
                PlayerAnim(h, v, "SLIDE");
                AdjustColliderForSliding(true); //콜라이더 값 줄임
                isGrounded = true;
            }
            //컨트롤 키를 때거나 대시 값이 없는 경우
            else if (Input.GetKeyUp(KeyCode.LeftControl) || currDash <= 0)
            {
                Debug.Log("Dash is not available.");
                currentSpeed = moveSpeed; //속도 기본값
                PlayerAnim(h, v, "WALK"); // 걷기 모션
                AdjustColliderForSliding(false); //콜라이더 값 원래대로 복구
            }
            else
            {
                // 기본 상태
                RecoverDash(15.0f); //대쉬 게이지 충전
                // 원래 상태로 복원
                playerCollider.center = originalColliderCenter;
                playerCollider.height = originalColliderHeight;
                playerCollider.radius = originalColliderRadius;


            }
        }
        //키를 누르지 않을때, 즉 기본 상태
        else
        {
            currentSpeed = moveSpeed; //속도
            PlayerAnim(h, v, "WALK"); // 걷기 모션
            RecoverDash(15.0f); // 대쉬 값 회복

            // 원래 상태로 복원
            playerCollider.center = originalColliderCenter;
            playerCollider.height = originalColliderHeight;
            playerCollider.radius = originalColliderRadius;

        }


        //전후좌우 이동방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //Translate(이동방향*속력 * Time.deltaTime)
        // 대각선 이동할때 피타고라스 법칙에 의해 1.4~ 뭐시기로 나와서 좀더 빨라지는데 그걸 정규화 벡터인 normalized를 사용해 길이가 1인 벡터를 써서 속도를 맞춘다.
        tr.Translate(moveDir.normalized * currentSpeed * Time.deltaTime);

        //vector3.up 축을 기준으로 turnSpeed만큼의 속도로 회전
        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);


        // 왼쪽 마우스를 꾹 누르고 있는지 확인
        if (Input.GetKey(KeyCode.Tab))
        {
            // 왼쪽 마우스를 누른 상태에서 오른쪽 마우스를 새로 클릭했을 때
            if (Input.GetMouseButtonDown(0))
            {
                PlayerAnim(h, v, "SHOOT");
                // 총알 발사 로직(Fire 함수나 게임매니저 ShootBullet 메서드) 호출
                // 예: GameManager.instance.ShootBullet();
            }
            else
            {
                PlayerAnim(h, v, "AIMING");

            }
        }
        else
        {
            // 왼쪽 마우스를 꾹 누르고 있지 않다면, 오른쪽 마우스 클릭 시 에임 모드
            if (Input.GetMouseButtonDown(1))
            {
                PlayerAnim(h, v, "AIMING");
            }
        }

        // 점프 입력 처리
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
            PlayerAnim(h, v, "JUMP"); //점프 모션

        }


        //=>  firepos 코드에서 완료함
        //if (Input.GetMouseButton(1))
        //{

        //    if (GameManager.instance != null && GameManager.instance.CurrentAmmo > 0)
        //    {
        //        PlayerAnim(h, v, "SHOOT");
        //    }


        //}

        //PlayerAnim(h, v, Input.GetKey(KeyCode.LeftShift)); //v로 전진 후진, h로 좌우 이동 감지해서 애니메이션 변경함

        ////앉기
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    isSit = true;
        //    PlayerAnim(h, v, "SIT");
        //}
        //else if (Input.GetKeyUp(KeyCode.C))
        //{
        //    isSit = false;
        //    PlayerAnim(h, v, "WALK");

        //}
        // 특정 상태일 때 캐릭터가 움직이지 못하도록 조건 추가


    }


    // 슬라이딩 시 콜라이더 크기 조정 함수
    void AdjustColliderForSliding(bool isSliding)
    {
        if (isSliding)
        {
            // 슬라이딩 상태: 콜라이더 크기와 위치 조정
            playerCollider.center = slidingColliderCenter;
            playerCollider.height = slidingColliderHeight;
            playerCollider.radius = slidingColliderRadius;
        }
        else
        {
            // 원래 상태로 복원
            playerCollider.center = originalColliderCenter;
            playerCollider.height = originalColliderHeight;
            playerCollider.radius = originalColliderRadius;

        }
    }

    void PlayingDash(float Dash_Value)
    {
        if (currDash > 0) // 대쉬 값이 0보다 클때
        {
            currDash -= Dash_Value * Time.deltaTime; // 대시 값 감소ㄴㅇ
            if (currDash < 0) currDash = 0; // 음수 방지
        }

        DisplayDash(); // UI 업데이트
    }
    void RecoverDash(float Dash_Value)
    {
        // 대시 값이 최대치보다 작을 때 서서히 회복
        if (currDash < initDash)
        {
            currDash += Dash_Value * Time.deltaTime; // 대시 값 회복 (초당 10씩 증가)ㅁ
            if (currDash > initDash) currDash = initDash; // 대시 값이 최대치를 초과하지 않도록 제한
        }

        // 대시 화면 UI 업데이트
        DisplayDash();
    }



    void LateUpdate()
    {
        // 플레이어의 회전값 고정
        Quaternion currentRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0, currentRotation.eulerAngles.y, 0);
    }

    void Dash_Sliding()
    {
        if (currDash > 0) // 대시 값이 남아있는 경우
        {

            currentSpeed += SlidingSpeed; //기존 속도에 추가
            PlayingDash(consume_Sliding); //슬라이딩 값 감소
            PlayerAnim(h, v, "SLIDE");
            AdjustColliderForSliding(true); //콜라이더 값 줄임
            isGrounded = true;
        }
        //컨트롤 키를 때거나 대시 값이 없는 경우
        else if (Input.GetKeyUp(KeyCode.LeftControl) || currDash <= 0)
        {
            Debug.Log("Dash is not available.");
            currentSpeed = moveSpeed; //속도 기본값
            PlayerAnim(h, v, "WALK"); // 걷기 모션
            AdjustColliderForSliding(false); //콜라이더 값 원래대로 복구
        }
        else
        {
            // 기본 상태
            RecoverDash(15.0f); //대쉬 게이지 충전
                                // 원래 상태로 복원
            playerCollider.center = originalColliderCenter;
            playerCollider.height = originalColliderHeight;
            playerCollider.radius = originalColliderRadius;


        }

    }

    // 점프 기능
    void Jump()
    {
        // Rigidbody를 통해 위쪽으로 힘을 가하여 점프
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // 점프 중이므로 지면에 없는 상태로 설정
        Debug.Log("Jumped");
    }
    // 지면에 닿았을 때 호출되는 함수
    void OnCollisionEnter(Collision coll)
    {

        // 바닥과 충돌 시 점프 가능 상태로 변경
        if (coll.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Grounded: True");
            // 충돌 후 다시 회전값 정규화
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }

    }
    void OnCollisionExit(Collision coll)
    {
        // 바닥에서 벗어나면 점프 불가 상태로 변경
        if (coll.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Grounded: False");
        }
    }
    private void OnTriggerEnter(Collider coll)
    {
        //충돌한 collider가 몬스터의 PUNCH면 Player의 HP 차감
        if (currHp >= 0.0f && coll.CompareTag("PUNCH"))
        {
            //체력이 최대체력 100에서 10 깎임
            currHp -= 10.0f;

            //체력 바 보여주는 함수 호출
            DisplayHealth();

            //체력 로그 띄워줌
            Debug.Log($"Player hp = {currHp / initHp}");
        }
        //Player의 생명이 0이하이면 사망 처리
        if (currHp < 0.0f)
        {
            //죽는 함수 호출
            PlayerDie();
        }

    }
    //죽는 함수
    void PlayerDie()
    {
        //죽었다고 로그 띄움
        Debug.Log("Player Die !");

        ////MONSTER 태그를 가진 모든 게임오브젝트를 찾아옴
        //GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
        ////모든 몬스터의 OnPlayerDie 함수를 순차적으로 호출
        //foreach (GameObject monster in monsters) { 
        //    monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}

        //주인공 사망 이벤트 호출
        OnPlayerDie();

        //GameManager 스크립트의 IsGameOver 프로퍼티 값을 변경
        //GameObject.Find("GameMgr").GetComponent<GameManager>().IsGameOver = true; //주인공이 사망했을때 IsGameover 속성값만 true로 변경하면 CreateMonster 함수가 종료된다. 
        GameManager.instance.IsGameOver = true; //주인공 사망하ㅐㅆ을때 더는 적캐릭 생성x. gamemanager에서 싱글턴을 이용해 instance를 받아 Isgameover를 true로 바꿈.
        //GameManager 클래스의 static 변수인 instance를 거친후 public으로 선언된 변수 또는 함수에 직접 접근하는 방식으로 더욱 더 간결해짐.
        //즉 주인공 사망=>몬스터 생성안함.
        //PlayerAnim(0.0f, 0.0f, "DIE");


        //추가 부분 
        //스코어 창 나오게 하기
    }



    //플레이어 애니메이션 함수
    public void PlayerAnim(float h, float v, string KEY_STATE)
    {
        //달리기
        if (KEY_STATE == "DASH" && currDash > 0)
        {
            //// 달리기 애니메이션
            //if (v >= 0.1f || v <= -0.1f || h >= 0.1f || h <= -0.1f)
            //{
            //    anim.CrossFade("Run_Rifle", 0.25f); // 앞으로 달리기
            //}
            //else
            //{
            //    anim.CrossFade("Idle_Rifle", 0.25f); // 앞으로 달리기

            //}
            // 달리기 애니메이션
            if (v >= 0.1f)
            {
                anim.CrossFade("Run_Rifle", 0.25f); // 앞으로 달리기
            }
            else if (v <= -0.1f)
            {
                anim.CrossFade("Run_Rifle", 0.25f); // 앞으로 달리기
            }
            else if (h >= 0.1f)
            {
                anim.CrossFade("Run_Rifle", 0.25f); // 앞으로 달리기
            }
            else if (h <= -0.1f)
            {
                anim.CrossFade("Run_Rifle", 0.25f); // 앞으로 달리기
            }
            else
            {
                anim.CrossFade("Idle Rifle", 0.25f); // 앞으로 달리기
            }
        }
        //걷기
        else if (KEY_STATE == "WALK")
        {

            if (v >= 0.1f)
            {
                anim.CrossFade("Walk_Rifle", 0.25f); // 앞으로 달리기
            }
            else if (v <= -0.1f)
            {
                anim.CrossFade("Walk_Rifle", 0.25f); // 앞으로 달리기
            }
            else if (h >= 0.1f)
            {
                anim.CrossFade("Walk_Rifle", 0.25f); // 앞으로 달리기
            }
            else if (h <= -0.1f)
            {
                anim.CrossFade("Walk_Rifle", 0.25f); // 앞으로 달리기
            }
            else
            {
                anim.CrossFade("Idle Rifle", 0.25f); // 앞으로 달리기
            }


            //// 걷기 애니메이션
            //if (v >= 0.1f || h >= 0.1f || h >= 0.1f || h <= -0.1f)
            //{
            //    anim.CrossFade("Walk_Rifle", 0.25f); // 앞으로 달리기

            //}
            ////else if (v <= -0.1f)
            ////{
            ////    anim.CrossFade("WalkB", 0.25f); // 뒤로 걷기
            ////}
            ////else if (h >= 0.1f)
            ////{
            ////    anim.CrossFade("WalkR", 0.25f); // 오른쪽으로 걷기
            ////}
            ////else if (h <= -0.1f)
            ////{
            ////    anim.CrossFade("WalkL", 0.25f); // 왼쪽으로 걷기
            ////}
            //else
            //{
            //    anim.CrossFade("Idle_Rifle", 0.25f); // 앞으로 달리기
            //}
        }
        //장전
        else if (KEY_STATE == "RELOAD")
        {
            anim.CrossFade("Reloading", 3.0f); // 앞으로 달리기
        }
        //쏘기
        else if (KEY_STATE == "SHOOT")
        {
            anim.CrossFade("Fire_SniperRifle", 0.15f); // 앞으로 달리기

        }
        //에임 조준
        else if(KEY_STATE == "AIMING")
        {
            anim.CrossFade("Aiming_Rifle", 0.25f); // 앞으로 달리기

        }
        else if(KEY_STATE == "IDLE")
        {
            anim.CrossFade("Idle Rifle", 0.25f); // 앞으로 달리기

        }
        ////데미지를 입었을때
        //else if( KEY_STATE == "DAMAGE")
        //{
        //    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death")) return;
        //    int id = Random.Range(0, countOfDamageAnimations);
        //    if (countOfDamageAnimations > 1)
        //        while (id == lastDamageAnimation)
        //            id = Random.Range(0, countOfDamageAnimations);
        //    lastDamageAnimation = id;
        //    animator.SetInteger("DamageID", id);
        //    animator.SetTrigger("Damage");

            //}

            ////플레이어가 죽었을때
            //else if (KEY_STATE == "DIE")
            //{

            //}

            ////앉기(제작중)
            //else if (KEY_STATE == "SIT")
            //{
            //    animator.SetBool("Squat", !animator.GetBool("Squat"));
            //    animator.SetBool("Aiming", false);
            //}




            //슬라이딩
        else if (KEY_STATE == "SLIDE")
        {
            anim.CrossFade("Slide", 0.25f); // 앞으로 달리기

        }
        //점프하기
        else if (KEY_STATE == "JUMP")
        {
            anim.CrossFade("Jump_Rifle", 0.25f); // 앞으로 달리기

        }
        
    }

    void DisplayHealth()
    {
        hpBar.fillAmount = currHp / initHp; //inithp가 100프로인데 현재 hp가 80프로이면 0.8이 되니 체력바의 상태가 조금 줄어든 상태가 됨.
    }
    void DisplayDash()
    {
        DashBar.fillAmount = currDash / initDash; //inithp가 100프로인데 현재 hp가 80프로이면 0.8이 되니 체력바의 상태가 조금 줄어든 상태가 됨.
    }


}
