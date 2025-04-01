import java.util.Random;
import java.util.Scanner;
import java.util.InputMismatchException;

public class Hello2030 {
	/*
	 종합 과제: 숫자 추측 게임 – 통계와 예외 처리까지!
	문제 설명:
	사용자가 1~100 사이의 숫자를 맞추는 게임입니다. 컴퓨터는 무작위로 숫자 하나를 생성하고, 사용자는 계속 추측을 합니다. 단, 다음과 같은 기능을 포함해야 합니다
	 */
	//메인함수
    public static void main(String[] args) {
        Random random = new Random(); //랜덤 객체 생성
        int answer = random.nextInt(100) + 1; // 1~100 사이 임의의 숫자 생성
        Scanner scanner = new Scanner(System.in); //사용자 입력을 받음

        // 입력 기록 저장할 배열
        int[] Input_Save_Array = new int[100]; // []를 앞에써도 문제없다
        int Try_Count = 0; // 실제 시도 횟수. 일단 0으로 초기화한다.

        System.out.println("숫자를 입력하여 1~100까지 중의 랜덤한 수를 맞춰보세요.");

        //while 문을 돌려 숫자를 입력하고 그 값이 정답이면 while을 나오고, 정답이 아니면 값에 따라 Down이나 UP을 출력한다.
        while (true) {
        	//입력하라는 문구 출력
            System.out.print("숫자 입력 >> ");
            int User_Input_Value = 0; // 사용자 입력한 값 저장 변수

            try {
            	User_Input_Value = scanner.nextInt(); // 사용자가 입력한 정수 값을  User_Input_Value 에 넣는다.
            } 
            // InputMismatchException 은 정수가 아닌 문자를 입력했을 경우의 오류이다.
            // 그래서 catch로 정수가 아닌 문자 입력했을때 예외처리를 해준다.
            // 이건 실수(예 : 11.2)를 입력해도 예외처리가 된다.
            catch (InputMismatchException e) {
                // 문자를 입력했을 경우 예외 처리
                System.out.println("정수 숫자가 아닌 값을 입력하셨습니다. 다시 입력해주세요.");
                scanner.nextLine(); // 문자를 받고 버려버린다. 쓰레기 값이기 때문.
                continue;      // 다시 while문 위쪽으로 넘어감
            }

            // 범위 벗어난 숫자 예외 처리
            if (User_Input_Value < 1 || User_Input_Value > 100) {
                System.out.println("1부터 100 사이의 숫자만 입력해주세요.");
                continue; 
            }

            // 입력 기록 User_Input_Value를 Input_Save_Array 배열에 저장한다. 저장한 후 Try_Count를 1 증가한다.
            Input_Save_Array[Try_Count] = User_Input_Value;
            Try_Count++;

            // 정답 판별
            if (User_Input_Value > answer) {
                System.out.println("Down 입니다.");
            } else if (User_Input_Value < answer) {
                System.out.println("Up 입니다.");
            } else {
                System.out.println("정답입니다. [시도 횟수] : " + Try_Count + "회");
                break;
            }

            // 100이 넘어가게 되면 배열에 인덱스가 100이라 더 저장할 수 없기 때문에 종료한다.
            if (Try_Count >= 100) {
                System.out.println("시도 횟수가 너무 많음. 종료합니다");
                break;
            }
        }

        // 정답을 맞추거나 시도 횟수가 종료 조건에 도달했다면 결과를 출력
        if (Try_Count > 0) {
            // 입력 기록(실제 입력한 횟수만큼) 출력
            System.out.print("입력 기록: ");
            for (int i = 0; i < Try_Count; i++) {
                System.out.print(Input_Save_Array[i] + " "); // 입력한 것들 모두 출력
                
            }
            System.out.println();

            // 평균 변수
            int sum = 0;
            // 최댓값 변수
            int max = Input_Save_Array[0];
            // 최솟값 변수
            int min = Input_Save_Array[0];
            
            // 시도한 횟수만큼 for문을 돌리고 총합과 최댓값, 최솟값을 구한다.
            for (int i = 0; i < Try_Count; i++) {
            	// 총합 더해감
                sum += Input_Save_Array[i];
                
                // 최댓값 구하기
                if (Input_Save_Array[i] > max) {
                    max = Input_Save_Array[i];
                }
                //최솟값 구하기
                if (Input_Save_Array[i] < min) {
                    min = Input_Save_Array[i];
                }
            }
            //평균 값
            double avg = (double) sum / Try_Count;
            
            //평균,최댓값,최솟값 출력
            System.out.println("평균값 : " + avg + " 최댓값 : " + max +" 최솟값 : " + min);
            
        }
        
        scanner.close();
    }
}

/*
 [결과]
숫자를 입력하여 1~100까지 중의 랜덤한 수를 맞춰보세요.
숫자 입력 >> 15
Up 입니다.
숫자 입력 >> 50
Up 입니다.
숫자 입력 >> 70
Up 입니다.
숫자 입력 >> 80
Up 입니다.
숫자 입력 >> 90
Down 입니다.
숫자 입력 >> 85
Down 입니다.
숫자 입력 >> 83
정답입니다. [시도 횟수] : 7회
입력 기록: 15 50 70 80 90 85 83 
평균값 : 67.57142857142857 최댓값 : 90 최솟값 : 15

 */
 
