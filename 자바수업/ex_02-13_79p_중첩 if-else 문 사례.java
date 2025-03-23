import java.util.Scanner;
public class Hello2030 {
	
	//메인 함수
	public static void main(String[] args) {
		char grade;
		Scanner scanner = new Scanner(System.in);
		System.out.print("점수를 입력하세요(0~100) : ");
		int score = scanner.nextInt(); // 점수 읽기
		System.out.print("학년을 입력하세요(1~4) : ");
		int year = scanner.nextInt(); //학년 읽기
		
		if(score >=60) { // 60점 이상
			if(year !=4) // 학년이 4가 아니라면
				System.out.println("합격!"); // 4학년 아니면 합격
			// 4학년은 60점이 아닌 70점 이상이여야 함
			else if(score >=70)
				System.out.println("합격!"); // 4학년이 70점 이상이면 합격
			else
				System.out.println("불합격!"); // 4학년이 70점 미만이면 불합격
		}
		else // 60점 미만 불합격
			System.out.println("불합격!");
			
		
		scanner.close();
		
		// 절댓값 쓸때 abs = (a>b)?a-b:b-a; 로 간단히 쓸 수 있음
	}
}


/*
 
점수를 입력하세요(0~100) : 65
학년을 입력하세요(1~4) : 4
불합격!



*/
