#include <stdio.h>
#include <math.h>
#include <stdlib.h> 
#include <windows.h>
#include <string.h>

//전역변수 A, M, Q, C
int A = 0000;
int M = 1101;
int Q = 1001;
int C = 0;


void Plus() {
	printf("--------PLUS 함수 시작-----\n");
	int A_bit, M_bit;
	int nanugi = 10, suchi = 1;
	int total_bit[4];
	int su =1;
	int Carry = 0; 
	int total = 0;
	//A + M 수행
	//A의 하나하나의 비트를 가져오고, M도 그렇게 한 후, 그 두개와 Carry를 더해서 total_bit에 넣는다.
	//total_bit 값이 3이면 Carry는 = 1로 만들고, total_bit값은 1로 만드는 식으로 한다.
	//마지막에 for문으로 total 배열값에 모든 값을 더한다.
	for (int i = 0; i < 4; i++) {
		//A와 M의 비트를 가져옴
		A_bit = (A % nanugi) / suchi;
		M_bit = (M % nanugi) / suchi;
		printf("[%d번째] A_bit : %d, M_bit : %d\n", i, A_bit, M_bit);
		//나누기 값과 수치값 10씩 곱함
		nanugi = nanugi * 10;
		suchi = suchi * 10;

		//값을 더함
		total_bit[i] = A_bit + M_bit + Carry;
		printf("[%d번째] 값 바꾸기 전 total_bit[%d] = A_bit + M_bit + Carry  = %d + %d + %d = %d\n", i, i,A_bit,M_bit,Carry, total_bit[i]);
		//total값 검사
		if (total_bit[i] == 3) {
			total_bit[i] = 1;
			Carry = 1;
		}
		else if (total_bit[i] == 2) {
			total_bit[i] = 0;
			Carry = 1;
		}
		else if (total_bit[i] == 1) {
			total_bit[i] = 1;
			Carry = 0;
		}
		else if (total_bit[i] == 0) {
			total_bit[i] = 0;
			Carry = 0;
		}
		printf("[%d번째] 값 바꾼 후 total_bit[%d] : %d,  Carry : %d\n", i, i,total_bit[i],Carry);
	}
	//total 모든 값을 일일이 더함
	for (int k = 0; k < 4; k++) {
		total += total_bit[k] * su;
		su = su * 10;
		printf("[%d번째 total = %04d]\n", k, total);
	}
	printf("[ 최종 total = %04d ", total);
	printf(" => A = %04d ] \n", total);


	//만약 마지막 Carry가 1이면 C를 1로 바꿈
	if (Carry == 1) {
		C = 1;
	}
	else {
		C = 0;
	}
	//A 값을 total로 바꿈
	A = total;

	printf("------PLUS 함수 종료---------\n");
}
void R_Shift() {
	printf("-----R_Shift 함수 실행-------\n");
	printf("R_shift 하기전 => C : %d, A : %04d, Q : %04d\n", C, A, Q);
	// 옮기는 순서는 Q -> A -> C 임
	//Q => A에 10을 %하고  그걸 천의 자리로 만듬 + Q에 10을 나누면 뒷 자리는 없어지고 백의 자리만 남음.
	Q = ((A % 10) * 1000) + (Q / 10);
	//A => C 값에 1000을 곱함 + A
	A = (C * 1000) + (A / 10);
	//C = 0
	C = 0;
	printf("R_shift 한 후 => C : %d, A : %04d, Q : %04d\n", C, A, Q);
	printf("-----R_Shift 함수 종료-------\n");

}
void SleepPrinting(char Word[], int time) {
	for (int i = time; i >= 0; i--) {
		printf("\n<%d 초 대기 중...>\n", i);
		Sleep(1000);
	}
	printf("\n<%s 실행!>\n\n", Word);
	Sleep(500);

}
int two_to_ten(int Two_total, int x) {
	//ex_ten_total 배열. 십진수 배열의 하나하나 값을 출력하고 싶어서 만듬
	int* TeT_Array = (int*)malloc(x * sizeof(int));
	//8자리 비트를 하나하나 가져감
	int* TwT_array = (int*)malloc(x * sizeof(int));
	//나누기와 수치값 변수
	int nanugi = 10, suchi = 1;

	//2.이진수를 잘게 쪼개서 TwT_array 배열에 넣음. 배열은 동적할당한 배열로 만들고 크기는 size 값으로 함. 그리고 nanugi와 suchi값 필요
	for (int i = 0; i < x; i++) {
		//일의자리부터 한자리씩 얻어서 TwT_array 배열에 넣음
		TwT_array[i] = (Two_total % nanugi) / suchi;
		printf("{[%d번째] TwT_array[%d] : %d} -> ", i, i, TwT_array[i]);
		//나누기 값과 수치값 10씩 곱함
		nanugi = nanugi * 10;
		suchi = suchi * 10;
	}

	//십진수 최종값
	int Ten_total = 0;

	printf("\n[[이진수->십진수 변환]]\n");
	for (int i = 0; i < x; i++) {
		int value = TwT_array[i] * (1 << i);  // 1 << i 는 2^i와 동일
		Ten_total += value;
		TeT_Array[i] = value;		// 0번째면 2의 0승, 1번째면 2의 1승, 2번째면 2의 2승
		printf("Ten_total += TwT_array[%d] * 2^%d = %d * 2^%d = %d\n", i, i, TwT_array[i], i, value);
	}
	//과정 출력 후 최종 십진수 결과값 출력
	printf("==> 결과값 : Ten_total = ");
	for (int i = 0; i < x; i++) {
		printf("%d", TeT_Array[i]);
		if (i != x-1) printf(" + ");
	}
	printf(" = %d\n", Ten_total);

	//동적 메모리 할당 해제
	free(TeT_Array);
	free(TwT_array);
	//십진수 값 반환
	return Ten_total;
}

int main() {
	
	int time = 0;
	char Word[30] = " ";
	//시작 값들 출력
	printf("M : %04d, C : %d, A : %04d, Q : %04d\n", M, C, A, Q);

	//4 사이클 돌림
	for (int i = 0; i < 4; i++) {
		//실행 딜레이
		snprintf(Word, sizeof(Word), "%d번째 사이클", i);
		SleepPrinting(Word,time);

		printf("==============[%d 번째]==============\n",i);

		//Q의 일의자리 추출
		int Q_Check;
		Q_Check = Q % 10;
		//Q 값에 따라 수행
		//Q==1이면 A= A+M 수행
		if (Q_Check == 1) {
			printf("Q == 1\n");
			// A + M 수행
			Plus();
		}
		//Q==0이면 Plus 함수 실행 X
		else
		{
			printf("Q == 0\n");
		}
		//우측 시프트 수행
		R_Shift();
		
		//사이클 한번 돈 후 결과 출력
		printf("\n--------------------------------\n");
		printf("{{%d번째 사이클 결과 => C : %d, A : %04d, Q : %04d }}\n", i, C, A, Q);
		printf("--------------------------------\n\n");

	}
	//이진수 십진수 변환한다고 메세지 알림
	snprintf(Word, sizeof(Word), "이진수->십진수 변환 ");
	SleepPrinting(Word, time);


	//이진수를 십진수로 만들어 출력하기
	
	//최종 이진수값을 십진수로 변환함.
	
	//A값에 10000을 곱하고 Q 와 더함
	int Two_total = (A * 10000) + Q;
	//M값과 Q값도 이진수->십진수 변환해줌
	printf("==== M 값 이진수->십진수 변환! ====\n");
	int M_ten = two_to_ten(M, 4);
	printf("=== Q 값 이진수->십진수 변환! ====\n");
	int Q_ten = two_to_ten(Q, 4);

	//이진수 -> 십진수 변환 함수 호출
	printf("==== Two_total 값 이진수->십진수 변환! ====\n");
	int Ten_total = two_to_ten(Two_total, 8);
	//  M X Q = Ten_total
	printf("\n즉, M(%d) X Q(%d)는 %d 이다\n\n", M_ten, Q_ten, Ten_total);
}

//two_to_ten 함수:
//이진수를 십진수로 변환하는 함수
//일단 배열의 size 값을 매개변수로 받는다. 그리고 반환하고 싶은 값을 보냄.
//1.이진수의 수를 받아야 함. 하지만 이것들은 결국 전역변수라 매개변수로 안받아도 됨
//2.이진수를 잘게 쪼개서 TwT_array 배열에 넣음. 배열은 동적할당한 배열로 만들고 크기는 size 값으로 함. 그리고 nanugi와 suchi값 필요
//3.TeT_array 배열을 만들고, 최종값(Ten_total)에 TwT_array에 넣어놓은 이진수 값을 2의 i승을 곱해 계속 더함.
//4.만들어진 배열을 반환하고, Ten_total값은 포인터를 통해 반환함(int *sum   &sum)
//int* Two_To_Ten(int x)    int *array = Two_To_Ten(size);
