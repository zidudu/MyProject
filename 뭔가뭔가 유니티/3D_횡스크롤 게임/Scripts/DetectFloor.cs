using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectFloor : MonoBehaviour
{
    private PlayerCtrl playerCtrl; //플레이어컨트롤 스크립트를 담을 변수

    // Start is called before the first frame update
    void Awake()
    {
        playerCtrl = GameObject.FindWithTag("Player")?.GetComponent<PlayerCtrl>(); //플레이어 오브젝트 태그 찾아 플레이어컨트롤 스크립트를 저장함.
    }

    //충돌 되어있을때 지속적으로
    private void OnTriggerStay(Collider other)
    {
        //바닥에 닿아있을때
        if (other.tag == "Floor") { 
            playerCtrl.isFloor = true; //바닥감지를 참으로 바꿈
        }
    }
    //충돌이 때졌을때
    private void OnTriggerExit(Collider other)
    {
        //바닥에 닿아있지 않을때
        if (other.tag == "Floor")
        {
            playerCtrl.isFloor = false; //바닥감지를 거짓으로 바꿈
        }
    }
    
}
