using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
//내비게이션 기능을 사용하기 위해 추가해야 하는 네임스페이스
using UnityEngine.AI;
using UnityEngine.UI; // Slider를 사용하기 위해 선언
public class MonsterCtrl : MonoBehaviour
{
    //Animator 파라미터의 해시값 추출
    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    //플레이어가 죽었을때 나오는 애니메이션
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    //속도 변수
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    
    private readonly int hashDie = Animator.StringToHash("Die");

    //몬스터 상태 정보
    public enum State { IDLE, TRACE, ATTACK, DIE }

    //컴포넌트의 캐시 처리 변수
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anim;
    //몬스터의 현재 상태
    public State state = State.IDLE;
    //추적 사정거리
    public float traceDist = 10.0f;
    //공격 사정거리
    public float attackDist = 1.8f;
    //몬스터의 사망 여부
    public bool isDie = false;


    //혈흔 효과 프리팹
    private GameObject bloodEffect;



    public Slider hpSlider; // 몬스터 HP를 표시할 Slider UI

    //몬스터 생명 변수
    private int hp = 100;
    private int maxHp = 100; // 초기 체력값

    private float originalSpeed;
    private float originalAngularSpeed;
    private float originalAcceleration;

    // 원래 애니메이터 속도를 저장할 변수
    private float originalAnimatorSpeed = 1.0f;



    private IEnumerator UpdateDestination()
    {
        while (!isDie)
        {
            agent.SetDestination(playerTr.position);
            yield return new WaitForSeconds(0.5f); // 0.5초마다 업데이트
        }
    }
    //초기화 함수
    public void InitializeMonster()
    {
        // 체력 및 상태 초기화
        hp = maxHp;
        hpSlider.value = (float)hp / 100.0f;
        isDie = false;

        // 콜라이더 다시 활성화
        GetComponent<CapsuleCollider>().enabled = true;

        // 애니메이션 상태 초기화
        anim.SetBool(hashTrace, false);
        anim.SetBool(hashAttack, false);
        anim.ResetTrigger(hashDie);

        // NavMeshAgent 초기화
        agent.isStopped = false;
        agent.ResetPath();
        agent.enabled = true;
        // 상태를 기본(IDLE)으로 설정
        state = State.IDLE;

        // NavMeshAgent 다시 활성화
        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.enabled = true;
        }
        // Rigidbody 초기화
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // mass, constraints, kinematic 상태, use gravity 상태 초기화
            rb.mass = 50.0f; // 예: 원래 몬스터의 질량 값
            rb.isKinematic = false; // 움직일 수 있도록
            rb.useGravity = true;   // 중력 다시 켜기
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            // 원하는 초기 상태에 맞게 FreezeRotation, FreezePosition 등 세팅
            rb.constraints = RigidbodyConstraints.FreezeRotationX
               | RigidbodyConstraints.FreezeRotationY
               | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    ///스크립트가 활성화될때마다 호출되는 함수
    void OnEnable() //OnEnable 함수는 스크립트 또는 게임오브젝트가 비활성화 된 상태에서 다시 활성화될때마다 발생하는 유니티 콜백함수이다. 따라서 코루틴 함수를 실행하는 부분을 OnEnable 함수로 옮겨 오브젝트 풀에서 재사용 하기 위해 활성화될때 CheckMonsterState와 MonsterAction 코루틴 함수가 다시 호출되게 한다. 또한 Start 함수를 Awake 함수로 변경한 이유는 OnEnable 함수가 Start 함수보다 먼저 수행되어 각종 컴포넌트가 연결되기 이전에 CheckMonsterState와 MonsterAction 코루틴 함수가 수행될 경우, 연결되지 않은 컴포넌트를 참조하는 오류가 발생하기 때문이다.
    {

        // 
        InitializeMonster();

        //중요
        //이벤트 발생 시 수행할 함수 연결 (플레이어와 몬스터를 연동함)
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;

        if (!isDie)
        {
            //2.2개의 StartCouroutine 함수를 OnEnable 함수로 옮긴다.
            //추적 대상의 위치를 설정하면 바로 추적 시작
            //agent.destination = playerTr.position;은 몬스터의 목적지를 플레이어의 현재 위치로 설정하여 몬스터가 플레이어를 추적하게 만듭니다.

            StartCoroutine(CheckMonsterState());

            //상태에 따라 몬스터의 행동을 수행하는 코루틴 함수 호출
            StartCoroutine(MonsterAction());
        }
    }
    //스크립트가 비활성화될때마다 호출되는 함수
    void OnDisable()
    {
        //기존에 연결된 함수 해제
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }
    void Awake() //522p.  1.Start 함수를 Awake 함수로 변경
    {
        hp = maxHp;
        hpSlider.value = (float)hp / 100.0f;
        //몬스터의 transfrom 할당
        monsterTr = GetComponent<Transform>();

        //추적 대상인 player의 transform 할당
        playerTr = GameObject.FindWithTag("PLAYER")?.GetComponent<Transform>();
        //NavMeshAgent 컴포넌트 할당
        agent = GetComponent<NavMeshAgent>();


        // Animator 속도 조정
        var animator = GetComponent<Animator>();


        //animator 컴포넌트 할당
        anim = GetComponent<Animator>();
        if (agent != null)
        {
            originalSpeed = agent.speed;
            originalAngularSpeed = agent.angularSpeed;
            originalAcceleration = agent.acceleration;
        }
        //BloodSprayEffect 프리팹 로드
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");
        if (playerTr == null)
        {
            Debug.LogError("플레이어 태그를 가진 오브젝트가 없습니다!");
        }

        //// NavMeshAgent 컴포넌트 할당
        //agent = GetComponent<NavMeshAgent>();

        //// 자동 위치 및 회전 업데이트 비활성화
        //agent.updatePosition = false;
        //agent.updateRotation = false;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        // NavMeshAgent 속도 조정
        var agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = originalSpeed * multiplier;
            agent.angularSpeed = originalAngularSpeed * multiplier;
            agent.acceleration = originalAcceleration * multiplier;
        }

        // Animator 속도 조정
        if (anim != null)
        {
            if (multiplier == 1.0f) // 시간 조작 해제 시 원래 속도로 복원
            {
                anim.speed = originalAnimatorSpeed;
            }
            else // 시간 조작 활성화 시 애니메이터 속도 감소
            {
                originalAnimatorSpeed = anim.speed; // 원래 속도 저장
                anim.speed = originalAnimatorSpeed * multiplier;
            }
        }
    }
    IEnumerator CheckMonsterState()
    {
        //몬스터가 아직 죽지 않았다면 계속 실행. 상태가 isDie가 되면 while문 탈출
        while (!isDie)
        {
            //0.3초 도안 중지(대기)하는 동안 제어권을 메시지 루프에 양보. 한마디로 0.3초마다 검사함. 실시간으로 검사를 하게 되면 그만큼 cpu 부하속도가 느려지기 때문에
            yield return new WaitForSeconds(0.3f);

            //몬스터의 상태가 DIE일때 코루틴을 종료
            if(state == State.DIE) yield break;

            //몬스터와 주인공 캐릭터 사이의 거리 측정. 
            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            //공격 사정거리 범위로 들어왓는지 확인. 
            if (distance <= attackDist)
            {
                state = State.ATTACK; //state를 어택 애니메이션으로 바꿈
            }
            //추적 사정거리 범위로 들어왔는지 확인
            else if (distance <= traceDist)
            {
                state = State.TRACE; //state를 추적 애니메이션으로 바꿈
            }
            //추적 사정거리 범위 밖에 있다면 기본 상태로 변환
            else
            {
                state = State.IDLE; //state를 기본 애니메이션으로 바꿈
            }
        }
    }
    IEnumerator MonsterAction()
    {
        //몬스터가 죽지 않았을때 계속 호출
        while (!isDie) {
            switch (state) { 
                //IDLE 상태
                case State.IDLE:
                    //추적 중지
                    agent.isStopped=true; //움직임 멈춤
                    //Animator의 IsTrace 변수를 false로 설정 -> 추적 안함
                    anim.SetBool(hashTrace,false);
                    break;

                //추적 상태    
                case State.TRACE:
                    //추적 대상의 좌표로 이동하기 시작함
                    agent.SetDestination(playerTr.position);
                    agent.isStopped=false;
                    //Animator의 IsTrace 변수를 true로 설정 -> 추적하는 애니메이션 나옴
                    anim.SetBool(hashTrace, true);
                    //Animator의 IsAttack 변수를 false로 설정 -> 공격안함
                    anim.SetBool(hashAttack, false);
                    break;

                //공격 상태
                case State.ATTACK:
                    //Animator의 IsAttack 변수를 true로 설정=> 공격하는 애니메이션 나옴
                    agent.isStopped = true;
                    anim.SetBool(hashAttack, true);
                    break;

                //사망 상태
                case State.DIE:
                    isDie = true;
                    agent.isStopped = true;
                    anim.SetTrigger(hashDie);

                    //일정 시간 대기 후 오브젝트 삭제
                    yield return new WaitForSeconds(2.0f);
                    Destroy(this.gameObject);
                    break;
            }
            //0.3초마다 검사
            yield return new WaitForSeconds(0.3f);
        }
    }
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            //충돌한 총알을 삭제
            Destroy(coll.gameObject);
            
            
        }
    }
    //레이캐스트를 사용해 데미지를 입히는 로직
    public void OnDamage(Vector3 pos,Vector3 normal)
    {
        //피격 리액션 애니메이션 실행
        anim.SetTrigger(hashHit);

        //총알의 충돌 지점
        //Vector3 pos = coll.GetContact(0).point;

        //총알의 충돌 지점의 법선 벡터
        //Quaternion rot = Quaternion.LookRotation(-coll.GetContact(0).normal);
        Quaternion rot = Quaternion.LookRotation(normal);

        //혈흔 효과를 생성하는 함수 호출
        ShowBloodEffect(pos, rot);



        //몬스터의 hp 차감
        hp -= 50;

        // Slider UI 업데이트
        if (hpSlider != null)
        {
            hpSlider.value = (float)hp / 100.0f;
        }
        if (hp <= 0)
        {
            state = State.DIE; //상태를 죽었다고 바꿈
                               //몬스터가 사망했을때 50점을 추가
            if (GameManager.instance != null)
            {
                GameManager.totScore += 10;
                GameManager.instance.DisplayScore(10); // 100점 추가 예시
            }
        }

    }
    public void ForceDie()
    {
        // HP를 0으로 만들거나 state를 DIE로 설정하는 로직
        // 예를 들어, hp = 0; state = State.DIE; 등을 처리하는 코드를 사용
        hp -= 1000;
        state = State.DIE;
        if (GameManager.instance != null)
        {
            GameManager.totScore += 10;
            GameManager.instance.DisplayScore(10); // 100점 추가 예시
        }
    }
    void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        //혈흔 효과 생성
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTr);
        Destroy(blood, 1.0f);
    }

    void OnDrawGizmos()
    {
        //추적 사정거리 표시
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }

        //공격 사정거리 표시
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }
    void OnTriggerEnter(Collider coll)
    {
        Debug.Log(coll.gameObject.name);
    }
    
    //플레이어가 죽었을때 실행하는 함수
    void OnPlayerDie()
    {
        //몬스터의 상태를 체크하는 코루틴 함수를 모두 정지시킴
        StopAllCoroutines();
        //추적을 정지하고 애니메이션을 수행
        agent.isStopped = true;
        //랜덤시간대로 애니메이션 출력
        anim.SetFloat(hashSpeed, UnityEngine.Random.Range(0.8f, 1.2f));
        anim.SetTrigger(hashPlayerDie);

    }


    // Update is called once per frame
    void Update()
    {
       agent.destination = playerTr.position;

    }
    private void FixedUpdate()
    {
        // 경로 계산 중일 경우 이동 처리 생략
        if (agent.pathPending) return;

        // 다음 위치로 이동 (Lerp로 부드럽게 보간)
        Vector3 nextPosition = agent.nextPosition;
        transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * agent.speed);
    }
}
