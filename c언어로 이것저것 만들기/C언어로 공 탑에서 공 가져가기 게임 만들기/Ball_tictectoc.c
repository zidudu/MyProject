#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
int main()
{
	//범위값
	int list_value = 5;

	//보드
	char board[5][5];
	//for 변수
	int k;
	//좌표 변수
	int level=0, b_pos=0, b_value=0;

	//플레이어 2명
	char P1[30];
	char P2[30];

	//턴 변수
	bool turn = false;

	//검사 변수
	bool check = false;


	//선택 턴
	int choice =0;

	//플레이어 이름 입력

	printf("플레이어 P1 이름을 입력하세요 : ");
	scanf("%s", &P1);


	printf("플레이어 P2 이름을 입력하세요 : ");
	scanf("%s", &P2);

	//턴 정하기
	printf("누가 먼저 하실 건가요?(1 : P1, 2: P2 가 선공) => ");
	scanf("%d", &choice);
	if (choice == 1) turn = false;
	else if (choice == 2) turn = true;

	//보드 초기화
	for (int i = 0; i < 5; i++) {
		for (int j = 0; j < i + 1; j++) {
			board[i][j] = 'O';
		}
	}
	//보드 출력
	printf("\n===============================================================\n");
	for (int n = 0; n < 5; n++) {
		//공백 출력
		for (int m = (list_value - 1) - n; m > 0; m--) {
			printf(" ");
		}
		//문자 출력
		for (int b = 0; b < n + 1; b++) {
			printf("%c ", board[n][b]);
		}
		printf("\n");
	}
	printf("\n===============================================================\n");

		
	//15번 반복하며 플레이어 턴 반복복
	for (k = 0; k < 15; k++) {
		printf("[%s : X] [%s : @]\n", P1, P2);
		//플레이어1일때 (turn이 false일때 )
		if (!turn) {
			//플레이어가 선택
			printf("%s 님의 값을 입력해주세요(level,b_pos,b_value) => ",P1);
			scanf("%d %d %d", &level, &b_pos, &b_value);
		}
		//플레이어2일때 (turn이 true일때 )
		else if(turn) {
			printf("%s 님의 값을 입력해주세요(level,b_pos,b_value) => ",P2);
			scanf("%d %d %d", &level, &b_pos, &b_value);
		}	
    
		/////////////
		// level,b_pos,b_value 범위 검사
		/////////////
		//level 검사
		if (level > 5 || level < 1) {
			printf("level 값의 범위가 1~5가 아닙니다..[현재 level : %d\n", level);
			k--;
			continue;
		}
		//b_pos 검사
		if (b_pos > 5 || b_pos < 1) {
			printf("b_pos 값의 범위가 1~5가 아닙니다...[현재 b_pos : %d]\n", b_pos);
			k--;
			continue;
		}
	
		//b_pos가 level보다 크다면 continue 
		else if (b_pos > level) {
			printf("level=%d 에 따른 b_pos=%d 의 값이 적절하지 않습니다..\n", level, b_pos);
			k--;
			continue;
		}
		
		//b_pos+b_value-1이 level 보다 크다면 continue;
		else if (b_pos + b_value - 1 > level) {
			printf("level=%d 과 b_pos=%d 에 따른 b_value=%d 값이 맞지 않습니다..\n", level, b_pos, b_value);
			k--;
			continue;
		}
		//그 자리에 X나 @가 있는지 검사함.
		for (int c = b_pos - 1; c <= (b_pos - 1) + (b_value - 1); c++) {
			//입력한 좌표에 값이 이미 있다면 못넣으니 다시 입력하라고 함 
			if (board[level - 1][c] != 'O') {
				printf("중간에 다른 값이 있습니다. 다시 입력하세요 \n");
				k--;
				check = true;
				break; // 여기 안 for문을 나감. k for문은 안 나감.
			}
			else {
				check = false;
			}
		}
		//체킹
		if (check) continue;
    
		//초깃값을 b_pos-1로 함, b_pos=3이면 3-1=2가 됨.
		// b_pos + b_value-1을 하면 2 
		//////////////
		// 즉, 좌표값 넣기
		/////////////////
		for (int c = b_pos - 1; c <= (b_pos - 1) + (b_value - 1); c++) {
			//turn이 false라면 , 즉 플1이면 그 좌표에 X를 넣음
			if (!turn) board[level - 1][c] = 'X';
			//플2면 그 좌표에 @를 넣음
			else board[level - 1][c] = '@';
		}



		//보드 그리기
		///			//
		//  보드 출력   //
		///		  //
		//범위값
		printf("\n===============================================================\n");

		for (int n = 0; n < 5; n++) {
			//공백 출력
			for (int m = (list_value - 1) - n; m > 0; m--) {
				printf(" ");
			}
			//문자 출력
			for (int b = 0; b < n + 1; b++) {
				printf("%c ", board[n][b]);
			}
			printf("\n");
		}
		printf("===============================================================\n");

		//남은 공 체크 카운팅 변수
		int count = 0;

		// 모두 그려져 있으면, 즉 board를 모두 검사한 다음 그 값에 O가 하나라도 없으면 turn을 검사하여 마지막에 넣은 사람이 우승하게 출력함
		for (int i = 0; i < list_value; i++) {
			for (int j = 0; j < i+1; j++) {
				//보드값에 빈 값이 있으면 
        if (board[i][j] == 'O') count++; // count를 증가해 남은 공 개수를 체크해줌.
			}
		}
		//남은 공 개수가 0이면 남은 공이 없다는 뜻이 됨
		if (count == 0) {
			printf("\n<<게임 종료!!>>");
      //플레이어 turn에 따라 승리한 플레이어의 이름을 출력
			if (!turn) {
				printf("\n<<%s 승리!!>>\n",P1);
				
			}
			else {
				printf("\n<<%s 승리!!>>\n",P2);
			}
			return 0;
		}
    //남은 공 개수가 있으면 남은 공 개수 출력
		else{
			printf("<<남은 공 개수 : %d 개>>\n", count);
		}

		
		//턴 교체
		turn = !turn;
	}
}
