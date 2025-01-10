using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //Animator 파라미터의 해시값 추출
    private readonly int Walk = Animator.StringToHash("Walk");
    private readonly int Jump = Animator.StringToHash("Jump");

    [Header("최대 속도 정의")]
    //최대 속도
    public float maxSpeed;

    [Header("감속 계수(클수록 빨리 멈춤)")]
    public float decelerationFactor = 5f;

    //리지드 바디 컴포넌트 변수
    Rigidbody2D rb;

    //스프라이트 랜더러 변수
    SpriteRenderer spriteRenderer;

    [Header("뱡향 정의")]
    [Tooltip("오른쪽 방향이면 참이되고, 왼쪽방향이면 거짓이 됨. 초기화는 true로 함")]
    [SerializeField]
    private bool dirRight = true; //오른쪽이면 참


    [Tooltip("좌우 값 정의")]
    [SerializeField]
    // 좌우 값 정의
    private float h;



    
    //플레이어 상태 정보

    public enum State { IDLE, WALK,JUMP,DIE }
    //애니메이션 변수
    private Animator anim;

    [Header("플레이어 상태")]
    [Tooltip("플레이어 상태(기본,걷기,죽음,점프) 등을 보여줌")]
    //플레이어의 현재 상태
    public State state = State.IDLE;

    [Header("플레이어 사망 여부")]
    [Tooltip("플레이어가 죽은 상태면 true가 되어 체크됨")]
    //플레이어의 사망 여부
    public bool isDie = false;


    [Header("점프 파워 정의")]
    [Tooltip("점프의 파워 값을 조절")]
    public float jumpPower = 50.0f;


    [Header("레이어 설정 값 정의")]
    //바닥에 세 개의 레이어를 쏨
    [Tooltip("좌우 레이어 거리값, 중심에서 좌우로 얼마만큼 떨어뜨려서 레이를 쏠지 정함")]
    public float offset = 0.3f; // 좌우로 얼마만큼 떨어뜨려서 레이를 쏠지
    [Tooltip("레이캐스트 길이")]
    public float rayLength = 1f;

    [Tooltip("넉백 함수")]
    bool isKnockback = false;

    // Start is called before the first frame update
    void Awake()
    {
        // 좌우 값 먼저 초기화
        float h = 0.0f;
        //animator 컴포넌트 할당
        anim = GetComponent<Animator>();

        // 애니메이션 상태 초기화
        anim.SetBool(Walk, false);

        //rb 변수에 이 오브젝트 rigidbody2d 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody2D>();
        //스프라이트랜더러 컴포넌트 가져옴
        spriteRenderer = GetComponent<SpriteRenderer>();

        //StartCoroutine(CheckPlayerState());
        //StartCoroutine(PlayerAction());
    }
    //void OnEnable() //OnEnable 함수는 스크립트 또는 게임오브젝트가 비활성화 된 상태에서 다시 활성화될때마다 발생하는 유니티 콜백함수이다. 따라서 코루틴 함수를 실행하는 부분을 OnEnable 함수로 옮겨 오브젝트 풀에서 재사용 하기 위해 활성화될때 CheckMonsterState와 MonsterAction 코루틴 함수가 다시 호출되게 한다. 또한 Start 함수를 Awake 함수로 변경한 이유는 OnEnable 함수가 Start 함수보다 먼저 수행되어 각종 컴포넌트가 연결되기 이전에 CheckMonsterState와 MonsterAction 코루틴 함수가 수행될 경우, 연결되지 않은 컴포넌트를 참조하는 오류가 발생하기 때문이다.
    //{
    //    //
        
    //}

    void CheckPlayerState() // 문자열 매개변수를 받아서 그 값에 따른 상태 변화를 시킴.
    {
        //플레이어가 아직 죽지 않았다면 계속 실행. 상태가 isDie가 되면 while문 탈출
       
            //0.3초 도안 중지(대기)하는 동안 제어권을 메시지 루프에 양보. 한마디로 0.3초마다 검사함. 실시간으로 검사를 하게 되면 그만큼 cpu 부하속도가 느려지기 때문에
            

            //플레이어의 상태가 DIE일때 코루틴을 종료
            if (state == State.DIE)
        {

        }
            ////걷고 있을때
            //if (h != 0 && state != State.WALK)
            //{
            //    state = State.WALK;
            //    //anim.SetBool(Walk, true); // 걷기 애니메이션 활성화
            //}
            //else if (h == 0 && state != State.IDLE)
            //{
            //    state = State.IDLE;
            //    //anim.SetBool(Walk, false); // IDLE 애니메이션 활성화
            //}

        
    }
    IEnumerator PlayerAction()
    {
        //플레이어가 죽지 않았을때 계속 호출
        while (!isDie)
        {
            switch (state)
            {
                //IDLE 상태
                case State.IDLE:
                    //걷기 false로 해 기본 상태로 돌아옴
                    anim.SetBool(Walk, false);
                    break;

                //걷기 상태    
                case State.WALK:
                    //Animator의 walk 파라미터를 true로 설정 
                    anim.SetBool(Walk, true);
                    break;
                //사망 상태
                case State.DIE:
                    //죽었다는 상태변수 true로 함
                    isDie = true;
                    break;
            }
            //0.3초마다 검사
            yield return new WaitForSeconds(0.3f);
        }
    }

    //일단 주석함. 왜냐하면 키 때어질때의 오류가 있음. 가속도가 급격하게 멈추기 때문
    private void Update()
    {
        //키보드 좌우 값 받음(-1,1) 즉각으로 받음
        h = Input.GetAxisRaw("Horizontal");

        //점프
        //스페이스바 눌렀고, 애니메이션이 점프 false 상태일때일때만 실행하면 점프중에는 다시 점프 못함.
        if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool(Jump)) 
        {
            //위로 jumpPower 만큼 올라감
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            //점프 애니메이션 나옴
            anim.SetBool(Jump, true);
        }
        //가속도가 0이면 Walk를 안함. 
        if(Mathf.Abs( rb.velocity.x) < 0.3)
        {
            anim.SetBool(Walk, false);
        }
        //걷고 있을때
        else
        {
            anim.SetBool(Walk, true);
        }
    }

    // Update is called once per frame
    void FixedUpdate() // 주기적으로 프레임마다 실행
    {
        if (isKnockback)
        {
            // 노크백 상태라면, 
            // 수평 이동 로직(velocity 세팅 등)을 스킵
            return;
        }
        //(1) 주석처리 => AddForce로 적용된 힘이 직접 세팅된 velocity에 의해 바로 덮어써져 “튕겨나가는” 효과가 무효화될 수 있습니다.
        //////물리엔진 영향으로 좌우를 움직임.
        //rb.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // (2) 키를 누르고 있을 때 → AddForce로 가속
        if (Mathf.Abs(h) > 0.01f)
        {


            // 왼쪽 or 오른쪽 누르고 있음
            // velocity로만 이동 제어
            rb.velocity = new Vector2(h * maxSpeed, rb.velocity.y);
           

            //왼쪽일땐 왼쪽으로 방향돌리고, 오른쪽일땐 오른쪽으로 방향 돌림.
            if (h > 0.0f && !dirRight) //오른쪽 키를 누를때임. 근데 왼쪽을 보고 있는 경우
                                       //이동방향이 0보다 크고, 오른쪽이 아니라면
            {
                ChangeDir();
            }
            else if (h < 0.0f && dirRight) // 이동방향이 0보다 작고, 오른쪽이라면
            {
                ChangeDir();
            }

          

            // 속도 제한 (오른쪽)
            if (rb.velocity.x > maxSpeed)
            {
                rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
            }
            // 속도 제한 (왼쪽)
            else if (rb.velocity.x < -maxSpeed)
            {
                rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
            }

           
        }
        else
        {
            // 방향키 안 누르고 있음 → 즉시 정지
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // 여러 레이어("Platforms", "Monster")를 동시에 포함하는 레이어마스크 만들기
        int layerMask = LayerMask.GetMask("Platforms", "Enemy");
        //바닥에 세 개의 레이어를 쏨
        
        // 캐릭터의 로컬 우측 방향
        Vector2 localRight = transform.right;
        Vector2 localLeft = -localRight;

        Vector2 centerPos = rb.position;
        // 그 뒤 offset만큼 더해준다
        Vector2 rightPos = rb.position + localRight * offset;
        Vector2 leftPos = rb.position + localLeft * offset;

        Debug.DrawRay(centerPos, Vector3.down * rayLength, Color.green);
        Debug.DrawRay(leftPos, Vector3.down * rayLength, Color.red);
        Debug.DrawRay(rightPos, Vector3.down * rayLength, Color.blue);
        
        RaycastHit2D hitCenter = Physics2D.Raycast(centerPos, Vector2.down, rayLength, layerMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(leftPos, Vector2.down, rayLength, layerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(rightPos, Vector2.down, rayLength, layerMask);
        RaycastHit2D[] arrayHit = { hitCenter, hitLeft, hitRight };
        //이 레이캐스트는 물리 기반임
        //이건 아래로 1만큼 빔을 쏜거에서 맞은 것을 판단
        //RaycastHit2D rayHit = Physics2D.Raycast(rb.position, Vector3.down, 1, LayerMask.GetMask("Platforms")); //1은 vector3.down 값임. GetMask() 는 아래로 1거리로 쏨. 레이어마스크로 판단  레이어 이름에 해당하는 정수값 반환

        // 아래로 1만큼 빔을 쏜 것에서 맞은 객체 판단
        //RaycastHit2D rayHit = Physics2D.Raycast(rb.position, Vector3.down, 1, layerMask); //layerMask 변수로 바닥과 몬스터의 레이어를 판단

        //올라가는 가속도가 0보다 작을때만 판별. 점프하고 내려올때만 검사함.
        if (rb.velocity.y < 0)
        {
            foreach (var rayHit in arrayHit)
            {

                if (rayHit.collider != null)
                {
                    if (rayHit.distance < 0.4f) //// 맞은 거리가 0.5f보다 작다면, 바닥과 가까워지면 실행
                    {
                        // 히트된 오브젝트의 레이어값 가져오기
                        int hitLayer = rayHit.collider.gameObject.layer;

                        // 바닥이라면
                        if (hitLayer == LayerMask.NameToLayer("Platforms"))
                        {
                            // 착지 처리 → 점프 애니메이션 해제
                            anim.SetBool(Jump, false);
                        }
                        // 몬스터라면
                        else if (hitLayer == LayerMask.NameToLayer("Enemy") && !isKnockback)
                        {
                            // 몬스터 오브젝트 제거
                            Destroy(rayHit.collider.gameObject);

                            // 플레이어를 살짝 위로 점프시킴
                            // 기존 x 속도 유지, y만 jumpPower로 설정
                            rb.velocity = new Vector2(rb.velocity.x, jumpPower);

                            // 만약 플레이어가 이미 Jump 애니메이션 중이 아니라면,
                            // 여기서 Jump 애니메이션을 true로 바꿀 수도 있음
                            anim.SetBool(Jump, true);
                        }

                    }
                }
            }
        }
        //빔을 쏠때 플랫폼 레이어만 판단할려함.


          
    }

    //방향 전환 함수
    void ChangeDir()
    {
        dirRight = !dirRight; // 현재 판정의 반대를 저장함
        spriteRenderer.flipX = !dirRight; //스프라이트를 반전시킴
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //적 오브젝트에 닿았을때
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("플레이어가 맞았습니다");
            //맞은 효과 함수
            OnDamaged(collision.transform.position);
        }

        
    }

    //데미지를 받았을때
    void OnDamaged(Vector2 targetPos)
    {
        //넉백 true
        isKnockback = true;

        //PlayerDamaged  레이어 값 3
        gameObject.layer = 3;

        //맞았을때 무적이라는 걸 보여주기 위해 스프라이트 렌더러의 색깔을 약간 투명하게 만듬
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); // 0.4f 가 투명 값

       
        //튕겨나가는 값. 방향에 따라 튕겨나가는 방향 다름
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; // 플레이어 위치 - 몬스터 위치 를 함. 플레이어가 왼쪽에 있으면 왼쪽으로 튕기게 -1로 하고, 플레이어가 오른쪽에 있으면 오른쪽으로 튕기게 1로 함
        //Vector2 DamagedAddVecter2 = new Vector2(dirc, 1); 
        //맞는 액션 표현하기 위해 뒤쪽으로 튕겨나가게 함
        // dirc 값에 따라 x 값 다르고, y축만큼 1로 튕김
        rb.AddForce(new Vector2(dirc, 1) * 7f, ForceMode2D.Impulse);


        // 0.2초 동안 이동 로직 끄기
        StartCoroutine(DisableMovement(0.5f));

        // 추가: 일정 시간 뒤 무적 해제
        StartCoroutine(EndInvincibility(2f));
    }
    IEnumerator DisableMovement(float duration)
    {
        float originalH = h;
        h = 0;  // 플레이어 입력/이동 무효화
        yield return new WaitForSeconds(duration);
        h = originalH;  // 원상 복귀 (혹은 그냥 해제)
        isKnockback = false;
    }
    IEnumerator EndInvincibility(float time)
    {
        yield return new WaitForSeconds(time);
        // 무적 해제
        gameObject.layer = LayerMask.NameToLayer("Player");
        spriteRenderer.color = new Color(1, 1, 1, 1f);
    }
}
