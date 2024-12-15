using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Slider를 사용하기 위해 선언

public class BarrelCtrl : MonoBehaviour
{
    //폭발 효과 파티클을 연결할 변수
    public GameObject expEffect;

    //무작위로 적용할 텍스처 배열
    public Texture[] textures;

    //폭발 반경
    public float radius = 10.0f;

    //하위에 있는 Mesh Renderer 컴포넌트를 저장할 변수
    private new MeshRenderer renderer;


    //컴포넌트를 저장할 변수
    private Transform tr;
    private Rigidbody rb;

    //총알 맞은 횟수를 누적시킬 변수
    private int hitCount = 0;

    private int hp = 100;
    private int maxHp = 100; // 초기 체력값

    public Slider hpSlider; // 몬스터 HP를 표시할 Slider UI

    //총소리에 사용할 오디오 음원
    public AudioClip fireSfx;


    //AudioSource 컴포넌트를 저장할 변수
    private new AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;

        //오디오소스 컴포넌트 받기
        audio = GetComponent<AudioSource>();


        //트랜스폼과 리지드바디 변수 저장
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        //하위에 있는 MeshRenderer 컴포넌트를 추출
        renderer = GetComponentInChildren<MeshRenderer>();

        //난수 발생
        int idx = Random.Range(0, textures.Length);
        //텍스쳐 지정
        renderer.material.mainTexture = textures[idx];
    }

    //충돌 시 발생하는 콜백 함수
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            Debug.Log(hitCount);

            hp -= 50;

            // Slider UI 업데이트
            if (hpSlider != null)
            {
                hpSlider.value = (float)hp / 100.0f;
            }
            if (hp <= 0)
            {
                //state = State.DIE; //상태를 죽었다고 바꿈
                //몬스터가 사망했을때 50점을 추가
                //if (GameManager.instance != null)
                //{
                //    GameManager.totScore += 10;
                //    GameManager.instance.DisplayScore(10); // 100점 추가 예시
                //}

                //총알 맞은 횟수를 증가시키고 3회 이상이면 폭발 처리

                ExpBarrel();
            }
        }
    }

    //드럼통 폭발시킬 함수
    void ExpBarrel()
    {
        //폭발 효과 파티클 생성
        GameObject exp = Instantiate(expEffect, tr.position, Quaternion.identity);
        //폭발 효과 파티클 5초 후에 제거
        Destroy(exp, 2.0f);

        //rigidbody 컴포넌트의 mass를 1.0으로 수정해 무게를 가볍게 함
        //rb.mass = 1.0f;
        //위로 솟구치는 힘을 가함
        //rb.AddForce(Vector3.up * 1500.0f);

        //간접 폭발력 전달
        IndirectDamage(tr.position);

        //총소리 발생 , 음량 크기가 100프로가 되었다는 걸 의미
        audio.PlayOneShot(fireSfx, 1.0f);


        //3초 후에 드럼통 제거
        Destroy(gameObject, 3.0f);
    }
    //폭발력을 주변에 전달하는 함수
    void IndirectDamage(Vector3 pos)
    {
        //주변에 있는 드럼통을 모두 추출
        Collider[] colls = Physics.OverlapSphere(pos, radius, 1 << 6); //1<<3

        foreach (var coll in colls)
        {



            // 플레이어나 카메라가 아닌 경우만 폭발력 적용
            if (coll.CompareTag("PLAYER") || coll.CompareTag("MainCamera"))
            {
                continue; // 플레이어나 카메라면 건너뜀
            }
            else if (coll.CompareTag("BARREL"))
            {
                //폭발 범위에 포함된 드럼통의 Rigidbody 컴포넌트 추출
                rb = coll.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    //드럼통의 무게를 가볍게 함
                    rb.mass = 1.0f;
                    //freezeRotation 제한값을 해제
                    rb.constraints = RigidbodyConstraints.None;
                    //폭발력을 전달
                    rb.AddExplosionForce(1500.0f, pos, radius, 1200.0f);

                }
            }
            else if (coll.CompareTag("MONSTER"))
            {
                // 몬스터 rigidbody 추출 후 폭발력 적용
                rb = coll.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.mass = 1.0f;
                    rb.constraints = RigidbodyConstraints.None;
                    rb.AddExplosionForce(1500.0f, pos, radius, 1200.0f);
                }

                // 몬스터 컨트롤 스크립트 가져오기 (부모 오브젝트에 붙어 있다고 가정)
                MonsterCtrl monsterCtrl = coll.GetComponentInParent<MonsterCtrl>();
                if (monsterCtrl != null)
                {

                    var navAgent = monsterCtrl.GetComponent<UnityEngine.AI.NavMeshAgent>();
                    if (navAgent != null)
                    {
                        navAgent.enabled = false;
                    }
                    // 3초 뒤 몬스터를 사망 처리하기 위한 코루틴 실행
                    StartCoroutine(DelayMonsterDie(monsterCtrl, 2.0f));
                }
            }

        }
        // 3초 뒤 몬스터 죽이기
        IEnumerator DelayMonsterDie(MonsterCtrl monster, float delay)
        {
            yield return new WaitForSeconds(delay);

            // 몬스터를 강제적으로 사망 상태로 바꾸는 메서드 호출
            // MonsterCtrl 클래스에 public 메서드를 하나 만든다고 가정
            // 예: public void ForceDie() { hp = 0; state = State.DIE; }
            monster.ForceDie();
            //3초 후에 드럼통 제거
            Destroy(this.gameObject, 0.0f);
        }
       
    }
}

