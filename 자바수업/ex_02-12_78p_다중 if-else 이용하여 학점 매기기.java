import java.util.Scanner;
public class Hello2030 {
	
	//메인 함수
	public static void main(String[] args) {
		char grade;
		Scanner scanner = new Scanner(System.in);
	
		
		System.out.print("점수를 입력하세요(0~100) : ");
		int score = scanner.nextInt(); // 점수 입력
		if(score >=90) // score 가 90 이상인지 검사
			grade = 'A';
		else if(score >= 80) // score가 90보다 작고 80 이상
			grade = 'B';
		else if(score >=70) //score가 80보다 작고 70 이상
			grade = 'C';
		else if(score >=60) //score가 70보다 작고 60 이상
			grade = 'D';
		else // score 가 60 미만
			grade = 'F';
		
		System.out.println("학점은 "+grade + "입니다");
		scanner.close();
		
		
	}
}


/*
 
점수를 입력하세요(0~100) : 89
학점은 B입니다



*/
