using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    [Header("가져올 카메라 정의")]
    public new Camera camera;
    void Start()
    {
        // SpriteRenderer 가져오기
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        //// Main Camera 가져오기
        //Camera camera = Camera.main;

        // 배경 크기 조정
        AdjustBackgroundSize(spriteRenderer, camera);
    }

    void AdjustBackgroundSize(SpriteRenderer spriteRenderer, Camera camera)
    {
        // 카메라의 화면 크기 계산
        float screenHeight = camera.orthographicSize * 2; // 화면 높이
        float screenWidth = screenHeight * camera.aspect; // 화면 너비

        // 스프라이트 크기 가져오기
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // 배경 크기 조정 (너비와 높이에 맞게)
        Vector3 scale = spriteRenderer.transform.localScale;
        scale.x = screenWidth / spriteSize.x;
        scale.y = screenHeight / spriteSize.y;

        // 적용
        spriteRenderer.transform.localScale = scale;
    }
}
