#include <MsTimer2.h>
#define CHECK_PORT_OSC1 12
#define CHECK_PORT_OSC2 13

//횟수. toggle_count를 인터럽트에서 증가시킴. 정해진 값까지 증가됨
int toggle_count = 0;

//OSC1 HIGH LOW 반전시키기 위한 변수
int oscillo_toggle = 1;
//OSC2 HIGH LOW 반전시키기 위한 변수
int oscillo_toggle2 = 1;

//최대 횟수 변수
int max_toggle = 4;
//딜레이 값
int delay_value = 1;
//인터럽트 ms 값
int Interrupt_ms_value = 1; 

//처음 시작때 실행
void setup() {
 //시리얼 통신
 Serial.begin(115200);

 //핀 OSC1, OSC2 출력으로 설정(다른 선은 GND로 연결되어있음)
 pinMode(CHECK_PORT_OSC1, OUTPUT);
 pinMode(CHECK_PORT_OSC2, OUTPUT);

//-> Interrupt_ms_value로 인터럽트 함수를 실행시키기 위해 세팅해놓음
  MsTimer2::set(Interrupt_ms_value, Interrupt_MS);
  // 인터럽트 실행
  MsTimer2::start();
}

//인터럽트 함수
void Interrupt_MS(){

  // OSC1 주파수 출력
  digitalWrite(CHECK_PORT_OSC1,  oscillo_toggle); //toggle 오실로스코프

  // 횟수 증가
  toggle_count += 1;
  
  //주파수 값 반전. HIGH, LOW 가 계속 반전되어 주파수가 위 아래로 움직임.
  oscillo_toggle = !oscillo_toggle; // 이걸로 주파수가 0,1,0,1이 됨
}

//Q.loop로 계속 돌게 되는데 toggle_count가 0이 된다고 해서 달라지는게 있나?
void loop() {
  //if 문으로 만들기  

    // if(toggle_count > max_toggle){
      
    //   digitalWrite(CHECK_PORT_OSC2, oscillo_toggle2);

    //   // OSC2 출력 반전
    //  oscillo_toggle2 = !oscillo_toggle2;
    //   // 0 초기화
    //   toggle_count = 0;
    // }

    //do while 문으로 만들기

  do{
    //딜레이를 1정도 넣어서 실행이 너무 빨리 되지 않게 함.
    delay(delay_value);
  } while(toggle_count <= max_toggle); // max_toggle값보다 toggle_count가 더 커지게 된다면 while문을 빠져나오게 됨

      //OSC2 주파수 출력
      digitalWrite(CHECK_PORT_OSC2, oscillo_toggle2);
      // OSC2 출력 반전
     oscillo_toggle2 = !oscillo_toggle2;
      // 0 초기화
      toggle_count = 0;
}
