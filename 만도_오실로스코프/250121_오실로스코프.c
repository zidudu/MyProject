#include <MsTimer2.h>
#define CHECK_PORT_10ms 12
#define CHECK_PORT_100ms 13

//횟수. toggle_count를 인터럽트에서 증가시킴. 정해진 값까지 증가됨
int toggle_count = 0;
//OSC1 HIGH LOW 반전시키기 위한 변수
int oscillo_toggle = 1;
//OSC2 HIGH LOW 반전시키기 위한 변수
int oscillo_toggle2 = 1;
//최대 횟수 변수
int max_toggle = 4;
//Q.setup과 loop 가 같이 도는 건가?

//처음 시작때 실행
void setup() {
 //시리얼 통신
 Serial.begin(115200);

 //핀 OSC1, OSC2 출력으로 설정(다른 선은 GND로 연결되어있음)
 pinMode(CHECK_PORT_10ms, OUTPUT);
 pinMode(CHECK_PORT_100ms, OUTPUT);

//Q.인터럽트 (10번을 실행시키고 끝나는 건가?)
// 매개변수 첫번째 값 ms 설정
//매개변수 두번째 값 함수 실행시킬거 설정 
// 1ms 만큼 코드를 실행시킴. 한칸마다 이 함수가 실행되게 됨
//-> 1ms로 인터럽트 함수를 실행시키기 위해 세팅해놓음
  MsTimer2::set(1,Interrupt_10ms);
  //Q. start 함수가 의미하는 게 뭘까? 이 함수를 주석으로 처리하였을때 실행이 잘됨.
  //A.Start로 세팅해놓은 것을 실행시킴.
  MsTimer2::start();
}

//인터럽트
void Interrupt_10ms(){
  //CHECK_PORT_10ms 핀의 OSC1 주파수 출력
  digitalWrite(CHECK_PORT_10ms, oscillo_toggle); //toggle 오실로스코프

  // 횟수 증가
  toggle_count +=1;
  
  //주파수 값 반전. HIGH, LOW 가 계속 반전되어 주파수가 위 아래로 움직임.
  oscillo_toggle = !oscillo_toggle; // 이걸로 주파수가 0,1,0,1이 됨
  
}

//Q.loop로 계속 돌게 되는데 toggle_count가 0이 된다고 해서 달라지는게 있나?
void loop() {
  
  //처음은 실행되고 while 조건문이 참이면 do가 다시 실행됨.
  //Q. while을 안쓰고 do while을 쓰는 이유가 무엇일까? 첫번째 실행될때 딜레이가 없으면 안되는 걸까? 아니면 속도가 더 빨리 실행되는 걸까?
  do{
    //딜레이를 1정도 넣어서 실행이 너무 빨리 되지 않게 함.
    delay(1);
  } while(toggle_count <= max_toggle); // max_toggle값보다 toggle_count가 더 커지게 된다면 while문을 빠져나오게 됨
  // => whille문을 빠져나오면서 CHECK_PORT_100ms 핀(13핀) 의 주파수 출력이 됨. 
  //즉 1ms 로 CHECK_PORT_10ms 를 실행시키다가 max_toggle 만큼 실행이 다 되면 CHECK_PORT_100ms 주파수를 출력시킴. 
  // 예를 들어 1ms로 5번씩 출력되다가 5번째에 두번째 OSC 의 주파수가 바뀜. 그래서 위는 1ms, 아래는 5ms 가 됨.
      digitalWrite(CHECK_PORT_100ms, oscillo_toggle2);
//출력을 시키고 나서 HIGH,LOW를 반전시킴. 
oscillo_toggle2 = !oscillo_toggle2;
//toggle_count를 0으로 다시 초기화시킴.
//Q. 이 코드문을 안넣고 배수만큼 증가될때마다 실행되게 하면 어떨까?
//A. toggle_count가 너무 커져서 오버플로우가 나게 됨.
  toggle_count = 0;
}
