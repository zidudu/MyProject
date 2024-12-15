using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//책에서는 FireCtrl로 표기


//컴포넌트를 요구한다. 오디오 소스를 요구한다.
[RequireComponent(typeof(AudioSource))]
public class FirePos : MonoBehaviour
{
    //총알 프리팹
    public GameObject bullet;
    //총알 발사 좌표
    public Transform firePos;

    //총소리에 사용할 오디오 음원
    public AudioClip fireSfx;

    //AudioSource 컴포넌트를 저장할 변수
    private new AudioSource audio;
    //muzzle Flash의 MeshRenderer 컴포넌트
    private MeshRenderer muzzleFlash;

    //RayCast 결과값을 저장하기 위한 구조체 선언
    private RaycastHit hit;       //Ray를 쏘아 어떤 게임오브젝트에 맞았을때 맞은 게임오브젝트의 정보를 반환받을 변수는 RaycastHit타입으로 선언


    ////// PlayerCtrl에 접근하기 위한 변수
    //private PlayerCtrl playerCtrl;

    // Start is called before the first frame update
    void Start()
    {
        //오디오소스 컴포넌트 받기
        audio = GetComponent<AudioSource>();

        //FirePos 하위에 있ㄴ는 MuzzleFlash의 Material 컴포넌트를 추출
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        //처음 시작할 때 비활성화
        muzzleFlash.enabled = false;

        // Player 라는 이름의 오브젝트에 PlayerCtrl이 붙어있다고 가정
        //GameObject player = GameObject.FindWithTag("Player");
        //if (player != null)
        //{
        //    playerCtrl = player.GetComponent<PlayerCtrl>();
        //}
    }


    // Update is called once per frame
    void Update()
    {
        //Ray를 시각적으로 표시하기 위해 사용
        Debug.DrawRay(firePos.position, firePos.forward * 10.0f,Color.green);

        if (Input.GetKey(KeyCode.Tab))
        {
            //왼쪽마우스클릭
            if (Input.GetMouseButtonDown(0))
            {


                // GameManager의 총알 발사 메서드 호출
                if (GameManager.instance != null && GameManager.instance.CurrentAmmo > 0) // 총알이 남아있을 경우
                {

                    //// PlayerCtrl 내 Animator에 IsShoot 트리거 발동
                    //playerCtrl.anim.SetTrigger("IsShoot");

                    // PlayerAnim 함수 호출: "SHOOT" 상태 전달 (가정)
                    // h, v 값은 필요하다면 playerCtrl 내에서 현재 입력값을 받거나, 여기서는 단순히 0 처리
                    //playerCtrl.PlayerAnim(0.0f, 0.0f, "SHOOT");

                    GameManager.instance.ShootBullet(); // 총알 개수를 줄이고 발사
                    Fire();
                    //Ray를 발사
                    if (Physics.Raycast(firePos.position, //광선의 발사 원점
                                        firePos.forward,  //광선의 발사 방향
                                        out hit,          //광선의 맞은 결과 데이터
                                        20.0f,            //광선의 거리
                                        1 << 6))          //감지하는 범위인 레이어 마스크
                    {
                        //맞은 몬스터의 이름을 로그에 출력
                        Debug.Log($"Hit={hit.transform.name}");

                        //OnDamage 함수로 몬스터 좌푯값 줘서 
                        hit.transform.GetComponent<MonsterCtrl>()?.OnDamage(hit.point, hit.normal); //레이캐스트에 맞은 컴포넌트가 몬스터라면 맞은 위치의 월드좌푯값과 Ray가 맞은 표면의 법선 벡터를 매개변수로 한 OnDamage 함수 실행   
                    }

                }
                else
                {
                    Debug.Log("총알 부족! 재장전이 필요합니다.");
                }
            }

        }
        ////버튼 한번 클릭하면 총알 나가는 함수 호출
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Fire();

        //    //Ray를 발사
        //    if (Physics.Raycast(firePos.position, //광선의 발사 원점
        //                        firePos.forward,  //광선의 발사 방향
        //                        out hit,          //광선의 맞은 결과 데이터
        //                        10.0f,            //광선의 거리
        //                        1 << 6))          //감지하는 범위인 레이어 마스크
        //    {
        //        //맞은 몬스터의 이름을 로그에 출력
        //        Debug.Log($"Hit={hit.transform.name}");

        //        //OnDamage 함수로 몬스터 좌푯값 줘서 
        //        hit.transform.GetComponent<MonsterCtrl>()?.OnDamage(hit.point, hit.normal); //레이캐스트에 맞은 컴포넌트가 몬스터라면 맞은 위치의 월드좌푯값과 Ray가 맞은 표면의 법선 벡터를 매개변수로 한 OnDamage 함수 실행   
        //    }
        //}
    }
    //총알 나가는 함수
    void Fire()
    {
        //Bullet 프리팹을 동적으로 생성

        //잠시 주석
        Instantiate(bullet, firePos.position, firePos.rotation);
        //총소리 발생 , 음량 크기가 100프로가 되었다는 걸 의미
        audio.PlayOneShot(fireSfx, 1.0f);
        //총구 화염 효과 코루틴 함수 호출
        StartCoroutine(ShowMuzzleFlash());
    }
    IEnumerator ShowMuzzleFlash()
    {
        //오프셋 좌푯값을 랜덤 함수로 생성
        Vector2 offset = new Vector2(Random.Range(0,2), Random.Range(0,2))*0.5f;
        //텍스쳐의 오프셋 값 설정
        muzzleFlash.material.mainTextureOffset = offset;

        //MuzzleFlash의 회전 변경
        float angle = Random.Range(0, 360);
        //로컬로 해서 muzzleFlash를 랜덤으로 회전시킴
        muzzleFlash.transform.localRotation = Quaternion.Euler(0,0,angle);

        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;


        //MuzzleFlash 활성화
        muzzleFlash.enabled=true;

        //0.2초 동안 대기(정지)하는 동안 메세지 루프로 제어권을 양보
        yield return new WaitForSeconds(0.2f);

        //MuzzleFlash 활성화
        muzzleFlash.enabled = false;
    }
}
