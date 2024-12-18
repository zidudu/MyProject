#include "IfxPort.h"
#include "Ifx_Types.h"
#include "IfxCpu.h"
#include "IfxScuWdt.h"
IFX_ALIGN(4) IfxCpu_syncEvent g_cpuSyncEvent = 0;

// LED 핀 설정
#define RLED &MODULE_P33,0 //회색선
#define GLED &MODULE_P33,2 //파란선
#define BLED &MODULE_P33,1 //노란선

// 버튼 핀 설정
#define BUTTON &MODULE_P33,3 //보라선

// 버튼의 이전 상태 저장
boolean lastButton = FALSE;
// LED의 현재 상태 (0: 꺼짐, 1: 빨강, 2: 녹색, 3: 파랑)
int ledState = 0;

void initLEDs(void)
{
    // LED 핀을 출력으로 설정
    IfxPort_setPinModeOutput(RLED, IfxPort_OutputMode_pushPull, IfxPort_OutputIdx_general);
    IfxPort_setPinModeOutput(GLED, IfxPort_OutputMode_pushPull, IfxPort_OutputIdx_general);
    IfxPort_setPinModeOutput(BLED, IfxPort_OutputMode_pushPull, IfxPort_OutputIdx_general);
}

void initButton(void)
{
    // 버튼 핀을 입력으로 설정
    IfxPort_setPinModeInput(BUTTON, IfxPort_InputMode_pullUp);
}

void setLEDs(int state)
{
    switch (state)
    {
    case 1: // 빨강
        IfxPort_setPinHigh(RLED);
        IfxPort_setPinLow(GLED);
        IfxPort_setPinLow(BLED);
        break;
    case 2: // 녹색
        IfxPort_setPinLow(RLED);
        IfxPort_setPinHigh(GLED);
        IfxPort_setPinLow(BLED);
        break;
    case 3: // 파랑
        IfxPort_setPinLow(RLED);
        IfxPort_setPinLow(GLED);
        IfxPort_setPinHigh(BLED);
        break;
    default: // 꺼짐
        IfxPort_setPinLow(RLED);
        IfxPort_setPinLow(GLED);
        IfxPort_setPinLow(BLED);
        break;
    }
}

int core0_main(void)
{
    IfxCpu_enableInterrupts();
    // !!WATCHDOG0 AND SAFETY WATCHDOG ARE DISABLED HERE!!
    // Enable the watchdogs and service them periodically if it is required
    IfxScuWdt_disableCpuWatchdog(IfxScuWdt_getCpuWatchdogPassword());
    IfxScuWdt_disableSafetyWatchdog(IfxScuWdt_getSafetyWatchdogPassword());

    // LED와 버튼 초기화
    initLEDs();
    initButton();

    while (1)
    {
        // 버튼 상태 읽기
        boolean currentButton = IfxPort_getPinState(BUTTON);

        // 버튼이 눌렸을 때 LED 상태 변경
        if (lastButton == FALSE && currentButton == TRUE)
        {
            // LED 상태값 증가
            ledState++;
            // ledState가 3을 초과하면 1로 초기화
            if (ledState > 3)
            {
                ledState = 1;
            }
            // LED 설정
            setLEDs(ledState);
        }

        // 이전 버튼 상태를 현재 상태로 업데이트
        lastButton = currentButton;
    }
    return 0;
}
