import java.util.Scanner;
public class Hello2030 {
	
	// 조건문 . c언어와 똑같이 씀
	
	//메인 함수
	public static void main(String[] args) {
		Scanner scanner = new Scanner(System.in);
		
		System.out.print("나이를 입력하시오");
		int age = scanner.nextInt(); // 나이 입력
		if((age>=20) && (age<30)) { // age가 20~29 사이인지 검사
			System.out.print("20대 입니다.");
			System.out.println("20대라서 행복합니다.."); 
		}
		else
			System.out.println("20대가 아닙니다");
		
		scanner.close();
		
		
	}
}


/*
 
나이를 입력하시오 23
20대 입니다.20대라서 행복합니다..


*/
