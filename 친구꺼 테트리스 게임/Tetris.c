#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <Windows.h>
#include <conio.h>
#include <time.h>
#include <sys/timeb.h>

#define SCR_WIDTH	50
#define SCR_HEIGHT	70

int nBufferIndex;
HANDLE hBuffer[2];

void CreateBuffer()
{
	CONSOLE_CURSOR_INFO cci;
	COORD size = { SCR_WIDTH, SCR_HEIGHT };
	SMALL_RECT rect;

	rect.Left = 0;
	rect.Right = SCR_WIDTH - 1;
	rect.Top = 0;
	rect.Bottom = SCR_HEIGHT - 1;

	hBuffer[0] = CreateConsoleScreenBuffer(GENERIC_READ | GENERIC_WRITE, 0, NULL, CONSOLE_TEXTMODE_BUFFER, NULL);
	SetConsoleScreenBufferSize(hBuffer[0], size);
	SetConsoleWindowInfo(hBuffer[0], TRUE, &rect);
	hBuffer[1] = CreateConsoleScreenBuffer(GENERIC_READ | GENERIC_WRITE, 0, NULL, CONSOLE_TEXTMODE_BUFFER, NULL);
	SetConsoleScreenBufferSize(hBuffer[1], size);
	SetConsoleWindowInfo(hBuffer[1], TRUE, &rect);

	cci.dwSize = 1;
	cci.bVisible = FALSE;
	SetConsoleCursorInfo(hBuffer[0], &cci);
	SetConsoleCursorInfo(hBuffer[1], &cci);
	
	nBufferIndex = 0;
}

void BufferWrite(short x, short y, const char* string)
{
	DWORD dw;
	COORD CursorPosition = { x, y };
	SetConsoleCursorPosition(hBuffer[nBufferIndex], CursorPosition);
	WriteFile(hBuffer[nBufferIndex], string, strlen(string), &dw, NULL);
}

void Flipping()
{
	SetConsoleActiveScreenBuffer(hBuffer[nBufferIndex]);
	
	COORD CursorPosition = { 100, 100 };
	SetConsoleCursorPosition(hBuffer[nBufferIndex], CursorPosition);

	nBufferIndex = !nBufferIndex;
}

void BufferClear()
{
	COORD Coor = { 0, 0 };
	DWORD dw;
	FillConsoleOutputCharacter(hBuffer[nBufferIndex], ' ', SCR_WIDTH * SCR_HEIGHT, Coor, &dw);
}

void Release()
{
	CloseHandle(hBuffer[0]);
	CloseHandle(hBuffer[1]);
}

#define BLK_LEFT	1
#define BLK_RIGHT	2
#define BLK_DOWN	3

typedef struct Position
{
	int x;
	int y;
} Pos;

typedef long long mill;

void ResetMap();
void PrintMap();

int CreateBlock(Pos shape[4][4]);
int MoveBlock(int dir);
void DropBlock();
void ChangeBlock();
int FixBlock();
int SwapBlock();

void ResetBlockPos();

void GetNextBlockMap(int subMap[2][4]);

int RemoveLine();

long long GetCurrentMillis();

void PrintMessage(const char* msg);

int shapeNo;
int rotateNo;
Pos curShape[4][4];
Pos nextShape[4][4];

int point;
int stagePoint;
int stage;
int stageGoal[5] = { 500, 1000, 2000, 3500, 6000 };
int stageDownSpeed[5] = { 1000, 800, 700, 650, 600 };

int map[22][14] = {
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
	1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
	1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
};

int main()
{
	// 화면 갱신을 위한 버퍼 생성(프로그램 실행 후 처음 1회 수행)
	CreateBuffer();

	// 현재 스테이지 정보 저장
	stage = 0;

	point = 0;

	while (stage < 5) {
		// 벽을 제외한 게임 영역을 모두 0(빈 공간)으로 변경(게임이 시작할 때마다 수행)
		ResetMap();

		// 점수 변수를 0으로 세팅(게임이 시작할 때마다 수행)
		stagePoint = 0;

		mill blkDown = 0;

		// 새 블록 생성할지 안할지 결정(게임이 시작할 때마다 수행)
		int needBlock = 0;
		CreateBlock(curShape);
		CreateBlock(nextShape);

		int stageClear = 0;

		PrintMap();

		while (1) // 프레임
		{
			// 새 블록 생성
			if (needBlock) {
				if (!SwapBlock()) {
					PrintMessage("게임 종료! 계속하시려면 아무키나 눌러주세요.");
					Sleep(500);
					_getch();
					point -= stagePoint;
					break;
				}
				CreateBlock(nextShape);
				needBlock = 0;
			}

			// 유저 키 입력 처리
			if (_kbhit()) // 키보드 입력이 발생하면 참을 반환해줌
			{
				int key = _getch();

				if (key == 224) { // 방향키가 입력됐다.
					key = _getch();

					if (key == 77) {
						// 오른쪽(이동)
						MoveBlock(BLK_RIGHT);
					}
					else if (key == 75) {
						//왼쪽(이동)
						MoveBlock(BLK_LEFT);
					}
					else if (key == 72) {
						//위쪽(모양 변경)
						ChangeBlock();
					}
					else if (key == 80) {
						//아래쪽(하강)
						if (!MoveBlock(BLK_DOWN)) {
							stageClear = FixBlock();
							needBlock = 1;
						}
					}
					PrintMap();
				}
				else if (key == 32) // 한 번에 내려가기
				{
					DropBlock();
					stageClear = FixBlock();
					needBlock = 1;
				}
			}

			// 자연 하강 처리
			mill cur = GetCurrentMillis();
			mill gap = cur - blkDown;
			if (gap >= stageDownSpeed[stage]) { // 1초마다 아래로 하강
				if (!MoveBlock(BLK_DOWN)) {
					stageClear = FixBlock();
					needBlock = 1;
				}
				blkDown = cur;
				PrintMap();
			}

			if (stageClear) {
				PrintMessage("스테이지 클리어! 계속하시려면 아무키나 눌러주세요.");
				Sleep(500);
				_getch();
				stage++;
				break;
			}
		}
	}

	PrintMessage("모든 스테이지를 클리어하였습니다. 축하합니다.");
	Sleep(500);
	_getch();

	Release();
	
	return 1;
}
void ResetMap()
{
	for (int i = 0; i < 20; i++)
	{
		for (int j = 2; j < 12; j++)
		{
			map[i][j] = 0;
		}
	}
}
void PrintMap()
{
	BufferClear();

	char buff[3500] = {0};
	int  blkMap[2][4] = {0};
	GetNextBlockMap(blkMap);

	for (int j = 0; j < 22; j++)
	{
		for (int i = 0; i < 14; i++)
		{
			// 1일 때는 벽
			if (map[j][i] == 1) {
				strcat_s(buff, "▩");
			}
			// 2일 때는 블록
			else if (map[j][i] == 2) {
				strcat_s(buff, "□");
			}
			// 3일 때는 고정 블록
			else if (map[j][i] == 3) {
				strcat_s(buff, "▣");
			}
			// 그 외 값은 빈 공간
			else {
				strcat_s(buff, "  ");
			}
		}

		if (j == 1) {
			strcat_s(buff, "    <Next>");
		}
		else if (j == 3) {
			strcat_s(buff, "    ");
			for (int k = 0; k < 4; k++)
			{
				if (blkMap[0][k] == 2) {
					strcat_s(buff, "□");
				}
				else {
					strcat_s(buff, "  ");
				}
			}
		}
		else if (j == 4) {
			strcat_s(buff, "    ");
			for (int k = 0; k < 4; k++)
			{
				if (blkMap[1][k] == 2) {
					strcat_s(buff, "□");
				}
				else {
					strcat_s(buff, "  ");
				}
			}
		}
		else if (j == 8) {
			char stageStr[100];
			sprintf_s(stageStr, "    단계: %d (필요 점수 %d)", stage + 1, stageGoal[stage]);
			strcat_s(buff, stageStr);
		}
		else if (j == 10) {
			char pointStr[100];
			sprintf_s(pointStr, "    점수: %d (누적 %d)", stagePoint, point);
			strcat_s(buff, pointStr);
		}

		strcat_s(buff, "\n");
	}
	BufferWrite(0, 0, buff);
	Flipping();
}
int CreateBlock(Pos shape[4][4])
{
	srand((unsigned int)time(NULL));

	shapeNo = rand() % 7;

	rotateNo = 0;	// 항상 새 블록은 0번 회전부터 시작

	switch (shapeNo)
	{
		case 0:
		{
			shape[0][0].x = 5;
			shape[0][0].y = 0;
			shape[0][1].x = 6;
			shape[0][1].y = 0;
			shape[0][2].x = 7;
			shape[0][2].y = 0;
			shape[0][3].x = 8;
			shape[0][3].y = 0;

			shape[1][0].x = 7;
			shape[1][0].y = 0;
			shape[1][1].x = 7;
			shape[1][1].y = 1;
			shape[1][2].x = 7;
			shape[1][2].y = 2;
			shape[1][3].x = 7;
			shape[1][3].y = 3;

			shape[2][0].x = 5;
			shape[2][0].y = 0;
			shape[2][1].x = 6;
			shape[2][1].y = 0;
			shape[2][2].x = 7;
			shape[2][2].y = 0;
			shape[2][3].x = 8;
			shape[2][3].y = 0;

			shape[3][0].x = 7;
			shape[3][0].y = 0;
			shape[3][1].x = 7;
			shape[3][1].y = 1;
			shape[3][2].x = 7;
			shape[3][2].y = 2;
			shape[3][3].x = 7;
			shape[3][3].y = 3;
			break;
		}
		case 1:
		{
			shape[0][0].x = 5;
			shape[0][0].y = 0;
			shape[0][1].x = 5;
			shape[0][1].y = 1;
			shape[0][2].x = 6;
			shape[0][2].y = 1;
			shape[0][3].x = 7;
			shape[0][3].y = 1;

			shape[1][0].x = 6;
			shape[1][0].y = 0;
			shape[1][1].x = 7;
			shape[1][1].y = 0;
			shape[1][2].x = 6;
			shape[1][2].y = 1;
			shape[1][3].x = 6;
			shape[1][3].y = 2;

			shape[2][0].x = 5;
			shape[2][0].y = 1;
			shape[2][1].x = 6;
			shape[2][1].y = 1;
			shape[2][2].x = 7;
			shape[2][2].y = 1;
			shape[2][3].x = 7;
			shape[2][3].y = 2;

			shape[3][0].x = 6;
			shape[3][0].y = 0;
			shape[3][1].x = 6;
			shape[3][1].y = 1;
			shape[3][2].x = 5;
			shape[3][2].y = 2;
			shape[3][3].x = 6;
			shape[3][3].y = 2;
			break;
		}
		case 2:
		{
			shape[0][0].x = 8;
			shape[0][0].y = 0;
			shape[0][1].x = 6;
			shape[0][1].y = 1;
			shape[0][2].x = 7;
			shape[0][2].y = 1;
			shape[0][3].x = 8;
			shape[0][3].y = 1;

			shape[1][0].x = 7;
			shape[1][0].y = 0;
			shape[1][1].x = 7;
			shape[1][1].y = 1;
			shape[1][2].x = 7;
			shape[1][2].y = 2;
			shape[1][3].x = 8;
			shape[1][3].y = 2;

			shape[2][0].x = 6;
			shape[2][0].y = 1;
			shape[2][1].x = 7;
			shape[2][1].y = 1;
			shape[2][2].x = 8;
			shape[2][2].y = 1;
			shape[2][3].x = 6;
			shape[2][3].y = 2;

			shape[3][0].x = 6;
			shape[3][0].y = 0;
			shape[3][1].x = 7;
			shape[3][1].y = 0;
			shape[3][2].x = 7;
			shape[3][2].y = 1;
			shape[3][3].x = 7;
			shape[3][3].y = 2;
			break;
		}
		case 3:
		{
			shape[0][0].x = 6;
			shape[0][0].y = 0;
			shape[0][1].x = 7;
			shape[0][1].y = 0;
			shape[0][2].x = 6;
			shape[0][2].y = 1;
			shape[0][3].x = 7;
			shape[0][3].y = 1;

			shape[1][0].x = 6;
			shape[1][0].y = 0;
			shape[1][1].x = 7;
			shape[1][1].y = 0;
			shape[1][2].x = 6;
			shape[1][2].y = 1;
			shape[1][3].x = 7;
			shape[1][3].y = 1;

			shape[2][0].x = 6;
			shape[2][0].y = 0;
			shape[2][1].x = 7;
			shape[2][1].y = 0;
			shape[2][2].x = 6;
			shape[2][2].y = 1;
			shape[2][3].x = 7;
			shape[2][3].y = 1;

			shape[3][0].x = 6;
			shape[3][0].y = 0;
			shape[3][1].x = 7;
			shape[3][1].y = 0;
			shape[3][2].x = 6;
			shape[3][2].y = 1;
			shape[3][3].x = 7;
			shape[3][3].y = 1;
			break;
		}
		case 4:
		{
			shape[0][0].x = 7;
			shape[0][0].y = 0;
			shape[0][1].x = 8;
			shape[0][1].y = 0;
			shape[0][2].x = 6;
			shape[0][2].y = 1;
			shape[0][3].x = 7;
			shape[0][3].y = 1;

			shape[1][0].x = 7;
			shape[1][0].y = 0;
			shape[1][1].x = 7;
			shape[1][1].y = 1;
			shape[1][2].x = 8;
			shape[1][2].y = 1;
			shape[1][3].x = 8;
			shape[1][3].y = 2;

			shape[2][0].x = 7;
			shape[2][0].y = 0;
			shape[2][1].x = 8;
			shape[2][1].y = 0;
			shape[2][2].x = 6;
			shape[2][2].y = 1;
			shape[2][3].x = 7;
			shape[2][3].y = 1;

			shape[3][0].x = 7;
			shape[3][0].y = 0;
			shape[3][1].x = 7;
			shape[3][1].y = 1;
			shape[3][2].x = 8;
			shape[3][2].y = 1;
			shape[3][3].x = 8;
			shape[3][3].y = 2;
			break;
		}
		case 5:
		{
			shape[0][0].x = 5;
			shape[0][0].y = 0;
			shape[0][1].x = 6;
			shape[0][1].y = 0;
			shape[0][2].x = 6;
			shape[0][2].y = 1;
			shape[0][3].x = 7;
			shape[0][3].y = 1;

			shape[1][0].x = 7;
			shape[1][0].y = 0;
			shape[1][1].x = 6;
			shape[1][1].y = 1;
			shape[1][2].x = 7;
			shape[1][2].y = 1;
			shape[1][3].x = 6;
			shape[1][3].y = 2;

			shape[2][0].x = 5;
			shape[2][0].y = 0;
			shape[2][1].x = 6;
			shape[2][1].y = 0;
			shape[2][2].x = 6;
			shape[2][2].y = 1;
			shape[2][3].x = 7;
			shape[2][3].y = 1;
			
			shape[3][0].x = 7;
			shape[3][0].y = 0;
			shape[3][1].x = 6;
			shape[3][1].y = 1;
			shape[3][2].x = 7;
			shape[3][2].y = 1;
			shape[3][3].x = 6;
			shape[3][3].y = 2;
			break;
		}
		case 6:
		{
			shape[0][0].x = 6;
			shape[0][0].y = 0;
			shape[0][1].x = 5;
			shape[0][1].y = 1;
			shape[0][2].x = 6;
			shape[0][2].y = 1;
			shape[0][3].x = 7;
			shape[0][3].y = 1;

			shape[1][0].x = 6;
			shape[1][0].y = 0;
			shape[1][1].x = 6;
			shape[1][1].y = 1;
			shape[1][2].x = 7;
			shape[1][2].y = 1;
			shape[1][3].x = 6;
			shape[1][3].y = 2;

			shape[2][0].x = 5;
			shape[2][0].y = 1;
			shape[2][1].x = 6;
			shape[2][1].y = 1;
			shape[2][2].x = 7;
			shape[2][2].y = 1;
			shape[2][3].x = 6;
			shape[2][3].y = 2;

			shape[3][0].x = 6;
			shape[3][0].y = 0;
			shape[3][1].x = 5;
			shape[3][1].y = 1;
			shape[3][2].x = 6;
			shape[3][2].y = 1;
			shape[3][3].x = 6;
			shape[3][3].y = 2;
			break;
		}
	}

	return 1;
}
int MoveBlock(int dir)
{
	Pos nextBlkPos[4][4] = { 0 };
	if (dir == BLK_DOWN)
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				nextBlkPos[i][j].x = curShape[i][j].x;
				nextBlkPos[i][j].y = curShape[i][j].y + 1;
			}
		}
	}
	else if (dir == BLK_LEFT) {
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				nextBlkPos[i][j].x = curShape[i][j].x - 1;
				nextBlkPos[i][j].y = curShape[i][j].y;
			}
		}
	}
	else if (dir == BLK_RIGHT) {
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				nextBlkPos[i][j].x = curShape[i][j].x + 1;
				nextBlkPos[i][j].y = curShape[i][j].y;
			}
		}
	}

	for (int i = 0; i < 4; i++)
	{
		if (map[nextBlkPos[rotateNo][i].y][nextBlkPos[rotateNo][i].x] != 0 &&
			map[nextBlkPos[rotateNo][i].y][nextBlkPos[rotateNo][i].x] != 2) {
			return 0;
		}
	}

	for (int i = 0; i < 4; i++)
	{
		map[curShape[rotateNo][i].y][curShape[rotateNo][i].x] = 0;
	}

	for (int i = 0; i < 4; i++)
	{
		for (int j = 0; j < 4; j++)
		{
			curShape[i][j].x = nextBlkPos[i][j].x;
			curShape[i][j].y = nextBlkPos[i][j].y;
		}
	}
	
	for (int i = 0; i < 4; i++)
	{
		map[curShape[rotateNo][i].y][curShape[rotateNo][i].x] = 2;
	}

	return 1;
}
void DropBlock()
{
	Pos p[4];
	for (int i = 0; i < 4; i++)
	{
		p[i].x = curShape[rotateNo][i].x;
		p[i].y = curShape[rotateNo][i].y;
	}

	int enable = 1;
	while (enable) {
		for (int i = 0; i < 4; i++)
		{
			if (map[p[i].y + 1][p[i].x] != 0 &&
				map[p[i].y + 1][p[i].x] != 2) {
				enable = 0;
			}
		}
		if (enable) {
			for (int i = 0; i < 4; i++)
			{
				p[i].y += 1;
			}
		}
	}

	for (int i = 0; i < 4; i++)
	{
		map[curShape[rotateNo][i].y][curShape[rotateNo][i].x] = 0;
	}

	for (int i = 0; i < 4; i++)
	{
		curShape[rotateNo][i].x = p[i].x;
		curShape[rotateNo][i].y = p[i].y;
	}

	for (int i = 0; i < 4; i++)
	{
		map[curShape[rotateNo][i].y][curShape[rotateNo][i].x] = 2;
	}
}
void ChangeBlock()
{	
	int rno = (rotateNo + 1) % 4;
	for (int i = 0; i < 4; i++)
	{
		// 진행중인 블록 이동 완료
		if (map[curShape[rno][i].y][curShape[rno][i].x] != 0 &&
			map[curShape[rno][i].y][curShape[rno][i].x] != 2) {
			return;
		}
	}

	for (int i = 0; i < 4; i++)
	{
		map[curShape[rotateNo][i].y][curShape[rotateNo][i].x] = 0;
	}

	rotateNo = rno;

	for (int i = 0; i < 4; i++)
	{
		map[curShape[rotateNo][i].y][curShape[rotateNo][i].x] = 2;
	}
}
int FixBlock()
{
	for (int i = 0; i < 4; i++)
	{
		map[curShape[rotateNo][i].y][curShape[rotateNo][i].x] = 3;
	}

	int clear = RemoveLine();

	return clear;
}
int SwapBlock()
{
	// 생성이 가능한지 체크, 불가능하다면 게임 종료
	for (int i = 0; i < 4; i++)
	{
		if (map[nextShape[0][i].y][nextShape[0][i].x] != 0 &&
			map[nextShape[0][i].y][nextShape[0][i].x] != 2) {
			return 0;
		}
	}

	// 생성이 가능하므로 화면에 출력
	for (int i = 0; i < 4; i++)
	{
		map[nextShape[0][i].y][nextShape[0][i].x] = 2;
	}

	memcpy(curShape, nextShape, sizeof(curShape));

	return 1;
}
void ResetBlockPos()
{
	for (int i = 0; i < 4; i++)
	{
		curShape[rotateNo][i].x = -1;
		curShape[rotateNo][i].y = -1;
	}
}
void GetNextBlockMap(int subMap[2][4])
{
	int sx = 50;
	int sy = 50;

	Pos s[4];

	for (int i = 0; i < 4; i++)
	{
		s[i] = nextShape[0][i];
	}

	for (int i = 0; i < 4; i++)
	{
		if (sx > s[i].x) {
			sx = s[i].x;
		}
		if (sy > s[i].y) {
			sy = s[i].y;
		}
	}

	for (int i = 0; i < 4; i++)
	{
		s[i].x -= sx;
		s[i].y -= sy;
	}

	for (int i = 0; i < 4; i++)
	{
		subMap[s[i].y][s[i].x] = 2;
	}
}
int RemoveLine()
{
	int clear = 0;		// 이번 스테이지 클리어 정보 저장
	int addPoint = 0;	// 동시에 여러줄 제거시 보너스 점수 정보 저장
	int i = 19;
	while (i >= 0)
	{
		//삭제할 줄인지 검사
		int j;
		for (j = 2; j < 12; j++)
		{
			if (map[i][j] != 3) {	// 한 줄안에서 한 칸이라도 고정된 블록이 아닌 경우 
				break;				// 삭제할 필요가 없으므로 바로 검사 종료
			}
		}

		// j가 12까지 증가했다는 것은 break가 한번도 수행되지
		// 않았으므로 한 줄이 모두 고정 블록(3)으로 채워져 있다.
		if (j == 12) {

			//우선 삭제되어야할 줄 전체를 0으로 변경
			for (int x = 2; x < 12; x++)
			{
				map[i][x] = 0;
			}

			// 삭제된 줄 윗줄부터 아래로 한칸씩 이동
			for (int k = i - 1; k >= 0; k--) {
				for (int m = 2; m < 12; m++)
				{
					if (map[k][m] == 3) {
						map[k][m] = 0;
						map[k + 1][m] = 3;
					}
				}
			}

			// 점수 획득
			point += 100 + addPoint;
			stagePoint += 100 + addPoint;
			addPoint += 50;
			if (stagePoint >= stageGoal[stage])
			{
				clear = 1;
			}
		}
		else {
			i--;
		}
	}
	
	return clear;
}
long long GetCurrentMillis()
{
	__timeb64 t;
	_ftime64_s(&t);
	
	return t.time * 1000 + t.millitm;
}
void PrintMessage(const char* msg)
{
	BufferClear();

	char buff[3500] = { 0 };

	sprintf_s(buff, msg);

	BufferWrite(0, 0, buff);

	Flipping();
}
