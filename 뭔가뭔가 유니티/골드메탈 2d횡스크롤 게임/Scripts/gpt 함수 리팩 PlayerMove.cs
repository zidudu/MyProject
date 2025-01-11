using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    #region Fields & Inspector

    // ====== Animator 해시값 ======
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Jump = Animator.StringToHash("Jump");

    // ====== 이동 설정 ======
    [Header("최대 속도 정의")]
    public float maxSpeed = 5f;             // 최대 수평 속도
    [Header("감속 계수(클수록 빨리 멈춤)")]
    public float decelerationFactor = 5f;   // (현재 사용X: 필요 시 가속/감속 보간)

    [Header("점프 설정")]
    [Tooltip("점프의 파워(Impulse)")]
    public float jumpPower = 50.0f;

    // ====== 상태/플래그 ======
    public enum State { IDLE, WALK, JUMP, DIE }
    [Header("플레이어 상태")]
    public State state = State.IDLE;        // 현재 상태
    [Header("플레이어 사망 여부")]
    public bool isDie = false;             // 사망 체크

    [Tooltip("노크백 중인지 여부")]
    private bool isKnockback = false;

    // ====== 레이캐스트 설정 ======
    [Header("레이어 설정 값 정의")]
    public float offset = 0.3f;        // 좌우 레이 x 오프셋
    public float rayLength = 1f;       // 레이캐스트 길이

    // ====== 내부 컴포넌트 ======
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    // ====== 이동 입력 ======
    private float h;                   // 좌우 방향키 입력값
    [Tooltip("오른쪽 방향이면 true, 왼쪽이면 false")]
    [SerializeField]
    private bool dirRight = true;      // 현재 바라보는 방향 (true = 오른쪽)

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        // Animator, Rigidbody2D, SpriteRenderer 할당
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 애니메이션 기본값
        anim.SetBool(Walk, false);
    }

    private void Update()
    {
        HandleInput();
        HandleJump();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (isKnockback)
        {
            // 노크백 중이면 수평 이동 로직 스킵
            return;
        }

        HandleMovement();
        CheckGroundOrEnemy();
    }

    // 충돌 발생 시 호출
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("플레이어가 맞았습니다");
            OnDamaged(collision.transform.position);
        }
    }

    #endregion

    #region Update Logic

    /// <summary>
    /// 좌우 입력 처리
    /// </summary>
    private void HandleInput()
    {
        // 좌우 방향 입력(-1,0,1)
        h = Input.GetAxisRaw("Horizontal");
    }

    /// <summary>
    /// 점프 입력 처리
    /// </summary>
    private void HandleJump()
    {
        // 스페이스바 & 현재 점프 중 아님
        if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool(Jump))
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool(Jump, true);
        }
    }

    /// <summary>
    /// 이동 관련 애니메이션 상태 갱신
    /// </summary>
    private void UpdateAnimations()
    {
        // X축 속도가 어느 정도 이상이면 Walk
        if (Mathf.Abs(rb.velocity.x) < 0.3f)
        {
            anim.SetBool(Walk, false);
        }
        else
        {
            anim.SetBool(Walk, true);
        }
    }

    #endregion

    #region FixedUpdate Logic

    /// <summary>
    /// 수평 이동 로직 (노크백 아닐 때만)
    /// </summary>
    private void HandleMovement()
    {
        // 방향키 입력이 있으면 velocity 설정
        if (Mathf.Abs(h) > 0.01f)
        {
            rb.velocity = new Vector2(h * maxSpeed, rb.velocity.y);

            // 방향 전환
            if (h > 0 && !dirRight)
            {
                ChangeDir();
            }
            else if (h < 0 && dirRight)
            {
                ChangeDir();
            }

            // 속도 제한
            if (rb.velocity.x > maxSpeed)
            {
                rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
            }
            else if (rb.velocity.x < -maxSpeed)
            {
                rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
            }
        }
        else
        {
            // 방향키 없으면 X속도 0
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    /// <summary>
    /// 레이캐스트로 바닥(Platforms) 또는 Enemy 감지
    /// </summary>
    private void CheckGroundOrEnemy()
    {
        // "Platforms"와 "Enemy" 레이어만
        int layerMask = LayerMask.GetMask("Platforms", "Enemy");

        // 레이 발사 위치(좌/우/중앙)
        Vector2 centerPos = rb.position;
        Vector2 rightPos  = centerPos + (Vector2)transform.right * offset;
        Vector2 leftPos   = centerPos - (Vector2)transform.right * offset;

        // 디버그용 선
        Debug.DrawRay(centerPos, Vector3.down * rayLength, Color.green);
        Debug.DrawRay(leftPos,   Vector3.down * rayLength, Color.red);
        Debug.DrawRay(rightPos,  Vector3.down * rayLength, Color.blue);

        // 실제 레이캐스트
        RaycastHit2D hitCenter = Physics2D.Raycast(centerPos, Vector2.down, rayLength, layerMask);
        RaycastHit2D hitLeft   = Physics2D.Raycast(leftPos,   Vector2.down, rayLength, layerMask);
        RaycastHit2D hitRight  = Physics2D.Raycast(rightPos,  Vector2.down, rayLength, layerMask);

        RaycastHit2D[] arrayHit = { hitCenter, hitLeft, hitRight };

        // 아래로 이동 중일 때만 (점프 후 착지 시점)
        if (rb.velocity.y < 0)
        {
            foreach (var rayHit in arrayHit)
            {
                if (rayHit.collider != null && rayHit.distance < 0.4f)
                {
                    int hitLayer = rayHit.collider.gameObject.layer;

                    // 바닥 감지
                    if (hitLayer == LayerMask.NameToLayer("Platforms"))
                    {
                        // 착지 처리
                        anim.SetBool(Jump, false);
                    }
                    // 적 감지
                    else if (hitLayer == LayerMask.NameToLayer("Enemy") && !isKnockback)
                    {
                        // 적 제거
                        Destroy(rayHit.collider.gameObject);

                        // 밟은 후 위로 점프
                        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                        anim.SetBool(Jump, true);
                    }
                }
            }
        }
    }

    #endregion

    #region Damage & Knockback

    /// <summary>
    /// 적 충돌 시 데미지/노크백 처리
    /// </summary>
    void OnDamaged(Vector2 targetPos)
    {
        // 노크백 시작
        isKnockback = true;

        // 무적 레이어로 변경
        gameObject.layer = LayerMask.NameToLayer("PlayerDamaged");
        // 투명 처리
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 노크백 힘 계산
        int dirc = (transform.position.x - targetPos.x > 0) ? 1 : -1;
        rb.AddForce(new Vector2(dirc, 1) * 7f, ForceMode2D.Impulse);

        // 수평 이동 차단 (0.5초)
        StartCoroutine(DisableMovement(0.5f));
        // 일정 시간 뒤 무적 해제
        StartCoroutine(EndInvincibility(2f));
    }

    /// <summary>
    /// 일정 시간 동안 이동 로직을 차단, 이후 해제
    /// </summary>
    IEnumerator DisableMovement(float duration)
    {
        float originalH = h;
        h = 0;  // 이동 입력 불가
        yield return new WaitForSeconds(duration);

        h = originalH;
        isKnockback = false;  // 노크백 종료
    }

    /// <summary>
    /// 일정 시간 후 무적 해제
    /// </summary>
    IEnumerator EndInvincibility(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.layer = LayerMask.NameToLayer("Player");
        spriteRenderer.color = new Color(1, 1, 1, 1f);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// 좌우 방향 전환
    /// </summary>
    void ChangeDir()
    {
        dirRight = !dirRight;
        spriteRenderer.flipX = !dirRight;
    }

    #endregion
}
