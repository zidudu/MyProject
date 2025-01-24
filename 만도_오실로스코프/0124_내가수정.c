#include <MsTimer2.h>

#define CHECK_PORT_1ms 12 // 1ms마다 토글할 핀
#define CHECK_PORT_5ms 13 // 5ms마다 토글할 핀

// 1ms / 5ms 토글 상태 저장용
bool toggle_1ms = false;
bool toggle_5ms = false;

// 5ms를 세기 위한 카운터
int interruptCounter = 0;
int counter_i = 0;
void setup() {
  // ────────── (1) 10Hz PWM 설정 (Timer1, 핀9) ──────────
  //pinMode(9, OUTPUT);      // 핀9(OC1A)를 PWM 출력용으로
  ICR1 = 24999;            // TOP=24999 → 주파수=약 10Hz
  // Fast PWM (WGM13=1, WGM12=1, WGM11=1, WGM10=0), OC1A 비반전
  TCCR1A = (1 << COM1A1) | (1 << WGM11);
  TCCR1B = (1 << WGM13) | (1 << WGM12);
  // 분주율 64 => (16MHz / 64) / 25000 ≈ 10Hz
  TCCR1B |= (1 << CS11) | (1 << CS10);
  
  // 초기 듀티 50% 예시 → OCR1A = (ICR1/2) = 12500
  //   (원하는 듀티에 맞춰 값 변경 가능)
  OCR1A = 12500;

  // ────────── (2) MsTimer2 설정: 1ms마다 인터럽트 ──────────
  MsTimer2::set(1, Interrupt_1ms);  // 1ms 주기
  MsTimer2::start();

  // ────────── (3) 1ms, 5ms 출력 확인용 핀 ──────────
  pinMode(CHECK_PORT_1ms, OUTPUT);
  pinMode(CHECK_PORT_5ms, OUTPUT);

  // 필요시 시리얼 출력
  // Serial.begin(115200);
}

// ────────────────────────────────────────────────────────
// 1ms마다 호출되는 함수
// ────────────────────────────────────────────────────────
void Interrupt_1ms() {
  // [A] 1ms마다 핀11 토글 (토글→ digitalWrite)

  digitalWrite(CHECK_PORT_5ms, toggle_5ms);
 
  //digitalWrite(CHECK_PORT_1ms, toggle_1ms);

  interruptCounter++;
    // 핀13 토글
    toggle_5ms = !toggle_5ms;
   
}

void loop() {
  // 10Hz PWM은 Timer1이 자동 생성,
  // 1ms/5ms 토글은 MsTimer2 ISR에서 처리.
  OCR1A = counter_i;

  analogWrite(CHECK_PORT_1ms,counter_i);
  do{
    //딜레이를 1정도 넣어서 실행이 너무 빨리 되지 않게 함.
    delay(1);
  } while(interruptCounter < 5); // max_toggle값보다 toggle_count가 더 커지게 된다면 while문을 빠져나오게 됨

  // [B] 5ms 주기를 세기 위한 카운터 증가
    interruptCounter = 0;
    counter_i+=1;  
    
    //추가
    if(counter_i > ICR1){
      counter_i = 0;
    }
}
