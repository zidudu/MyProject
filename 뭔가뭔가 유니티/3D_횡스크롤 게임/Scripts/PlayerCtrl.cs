using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [Header("기본 이동 제어")]
    [Tooltip("이동속도 정의")]
    public float moveSpeed = 5.0f; // 이동속도
    [Tooltip("이동방향 정의")]
    public float moveDir; // 이동방향
    [Tooltip("오른쪽 방향이면 참이되고, 왼쪽방향이면 거짓이 됨. 초기화는 true로 함")]
    public bool dirRight = true; //오른쪽이면 참
    [Header("점프 제어")]
    [Tooltip("점프속도 정의")]
    public float jumpSpeed = 500.0f; // 점프속도
    [Tooltip("바닥과 닿았는지 아닌지 검사함., 초깃값 false")]
    public bool isFloor = false; // 바닥인가? 거짓
    [Tooltip("더블점프중인지 아닌지 검사, 초깃값 false")]
    public bool isDoubleJump = false; // 더블점프중인가? 거짓

    private Rigidbody playerRB; //플레이어 리지드바디
    // Start is called before the first frame update
    void Awake()
    {
        playerRB = this.GetComponent<Rigidbody>(); // 플레이어 리지드바디 저장
    }

    // Update is called once per frame
    void Update() // 매 프레임 실행
    {
        moveDir = Input.GetAxis("Horizontal"); // 키보드 좌우 입력 값 왼쪽은 -1, 오른쪽은 1로 됨

        //스페이스바를 눌렀을때, 바닥이 감지되었을때만 점프 가능하게 함.그걸 DetectFloorPoint가 감지함.
        if((isFloor || !isDoubleJump)&& Input.GetKeyDown(KeyCode.Space)) // 플레이어가 바닥에 있거나 더블점프중이 아니고, 스페이스바 키를 눌렀을때
        {
            playerRB.AddForce(new Vector2(0, jumpSpeed)); //점프키(스페이스바)를 입력받아 Y축으로 점프속도만큼의 힘을가함

            if (!isDoubleJump && !isFloor) {  // 플레이어가 더블점프중이 아니고, 바닥에 있지 않을때
                isDoubleJump = true; // 더블점프 검사를 참으로 바꿈
            }
        }
    }
    private void FixedUpdate() // 고정 프레임을 써서 물리연산을 쓰기 좋음
    {
        //velocity : 속도 , 좌우에 무브스피드 곱함. 그리고 y축의 수치를 따라가게 됨.
        playerRB.velocity = new Vector2 (moveDir * moveSpeed, playerRB.velocity.y); // 좌우이동

        //더블점프
        if (isFloor) {  //플레이어가 바닥에 있다면
            isDoubleJump = false; // 더블점프 거짓으로 초기화
        }

        //왼쪽일땐 왼쪽으로 방향돌리고, 오른쪽일땐 오른쪽으로 방향 돌림.
        if (moveDir > 0.0f && !dirRight) //오른쪽 키를 누를때임. 근데 왼쪽을 보고 있는 경우
                                         //이동방향이 0보다 크고, 오른쪽이 아니라면
        {
            ChangeDir();
        }
        else if (moveDir < 0.0f && dirRight) // 이동방향이 0보다 작고, 오른쪽이라면
        {
            ChangeDir();
        }
    }
    void ChangeDir()
    {
        dirRight = !dirRight; // 현재 판정의 반대를 저장함
        transform.Rotate(Vector3.up, 180.0f, Space.World); // 로컬 y축 기준 180도 회전함.
    }
}
