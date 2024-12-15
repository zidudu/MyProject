using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RemoveBullet : MonoBehaviour
{
    //스파크 파티클 프리팹을 연결할 변수
    public GameObject sparkEffect;

    //충돌이 시작할때 발생하는 이벤트
    private void OnCollisionEnter(Collision coll)
    {
        //충돌한 게임옹브젝트의 태그값 비교 //if(coll.collider.tag == "BULLET")
        if (coll.collider.CompareTag("BULLET"))
        {
            // 충돌한 총알의 Rigidbody 제거 (물리 연산 중지)
            Rigidbody bulletRb = coll.collider.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.isKinematic = true;
            }

            //첫번째 충돌 지점의 정보 추출
            ContactPoint cp = coll.GetContact(0);
            //충돌한 총알의 법선 벡터를 쿼터니언 타입으로 변환
            Quaternion rot = Quaternion.LookRotation(-cp.normal);
            //스파크 파티클을 동적으로 생성
            //Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);
            GameObject spark=  Instantiate(sparkEffect,cp.point,rot);

            //일정 시간이 지난 후 스파클 파티클을 삭제
            Destroy(spark, 0.5f);
            //충돌한 게임 오브젝트 삭제
            Destroy(coll.gameObject);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 드럼통의 X,y Z 축 회전을 제한
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        }
    }
}
