using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    // 따라가야 할 대상을 연결할 변수 (캐릭터 본체)
    public Transform targetTr;

    // 캐릭터의 눈 위치를 저장할 변수 (눈에 해당하는 게임 오브젝트)
    public Transform eyeTr;


    // 에임 모드(우클릭 시)의 눈 위치를 저장할 변수
    public Transform aimingEyeTr;

    // Main Camera 자신의 Transform 컴포넌트
    private Transform camTr;

    // 따라갈 대상으로부터 떨어질 거리 (3인칭 카메라)
    [Range(2.0f, 20.0f)]
    public float distance = 10.0f;

    // Y축으로 이동할 높이 (3인칭 카메라)
    [Range(0.0f, 10.0f)]
    public float height = 2.0f;

    // 1인칭 시점에서 카메라와의 거리
    public float firstPersonDistance = 0.5f;

    // 반응 속도
    public float damping = 10.0f;

    // 카메라 LookAt의 Offset 값
    public float targetOffset = 2.0f;

    // SmoothDamp에서 사용할 변수
    private Vector3 velocity = Vector3.zero;

    // 1인칭과 3인칭 전환 상태 저장
    private bool isFirstPerson = false;

    void Start()
    {
        camTr = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        // V 키를 눌러 시점 전환
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;
        }

        // 에임 모드(우클릭) 상태 확인
        bool isAiming = Input.GetKey(KeyCode.Tab);
        // 현재 시점에 따라 카메라 위치 계산
        Vector3 pos;
        if (isFirstPerson)
        {
            // 1인칭 시점: 눈 오브젝트의 위치에 카메라를 맞춤
            if (eyeTr != null)
            {
                // 눈의 위치와 방향에 맞춰 카메라 위치와 회전 설정
                camTr.position = eyeTr.position;
                camTr.rotation = eyeTr.rotation;
            }
        }
        else
        {
            // isFirstPerson이 false일 때 우클릭(에임) 중이면 aimingEyeTr 시점으로 이동
            if (isAiming && aimingEyeTr != null)
            {
                camTr.position = aimingEyeTr.position;
                camTr.rotation = aimingEyeTr.rotation;
            }
            else
            {
                // 3인칭 카메라 위치 설정
                pos = targetTr.position + (-targetTr.forward * distance) + (Vector3.up * height);

                // 카메라 위치를 부드럽게 변경 (SmoothDamp 사용)
                camTr.position = Vector3.SmoothDamp(camTr.position, pos, ref velocity, damping);

                // 카메라가 대상 캐릭터를 바라보도록 설정
                camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
            }
        }
    }
}
