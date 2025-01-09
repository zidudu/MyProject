using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
   
    
   
    private new Camera camera; // 카메라 객체를 저장할 변수
    [Header("카메라 사이즈")]
    [Tooltip("초기화 카메라 사이즈 정의")]
    public float OrthographicSize = 3f;  
    void Start()
    {
        // 카메라 컴포넌트 가져오기
        camera = Camera.main;

        // 초기 Orthographic Size 설정
        camera.orthographicSize = OrthographicSize;
    }

    void Update()
    {
        // 위 방향키 입력 시 줌 아웃
        if (Input.GetKey(KeyCode.UpArrow))
        {
            camera.orthographicSize += 0.1f; // 줌 아웃
            if (camera.orthographicSize > 30f)
            {
                camera.orthographicSize = 30f;
            }
        }

        // 아래 방향키 입력 시 줌 인
        if (Input.GetKey(KeyCode.DownArrow))
        {
            camera.orthographicSize -= 0.1f; // 줌 인
            if(camera.orthographicSize < 0.5f)
            {
                camera.orthographicSize = 0.5f;
            }
        }
        // 기존 상태로 되돌아옴
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            camera.orthographicSize = OrthographicSize; // 기존 상태
        }

    }
}
