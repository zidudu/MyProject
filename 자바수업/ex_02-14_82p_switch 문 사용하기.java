import java.util.Scanner;
public class Hello2030 {
	
	// switch 문
	// 값에 따라 여러 방향으로 분기하는 경우, if 문보다 switch 문을 사용하면 가독성이 높은 좋은 코드를 작성할 수 있다.
	// switch 문은 먼저 식을 계산하고 그 결과 값과 일치하는 case 문으로 분기한다.
	//case 문의 실행 문장을 실행한 후 break를 만나면 switch 문을 벗어난다. 실행 문장이 여러 개인 경우라도 중괄호({})로 둘러싸지 않는다는 점에 유의하기 바란다.
	// 만일 어떤 case 문의 값과도 같지 않은 경우, default 문으로 분기하여 실행 문장 n 을 실행한다. default 문은 생략 가능하다
	// switch 문에서 break 문은 중요. 어떤 case 실행문장 실행되고 만난 break 문장은 switch 문을 벗어나도록 지시함.
	//만일 case 문에 break 문이 없다면 아래의 case 문의 실행문장으로 break 문을 만날 때까지 계속 실행한다.
	
	// case 문에 지정하는 값은 정수 리터럴, 문자 리터럴, 문자열 리터럴 만 허용
	// -> case 문에는 변수나 식을 사용할 수 없다!
	
	//메인 함수
	public static void main(String[] args) {
		Scanner scanner = new Scanner(System.in);
		
		System.out.print("월(1~12)을 입력하시오 : ");
		int month = scanner.nextInt(); // 점수로 월 입력
		
		//날짜에 따른 계절 구분하기
		switch(month) {
			case 3:
			case 4:
			case 5:
				System.out.println("봄입니다.");
				break;
			case 6: case 7:case 8: // case: 를 여러개 나열 가능하다
				System.out.println("여름입니다.");
				break;
			case 9: case 10:case 11:
				System.out.println("가을입니다.");
				break;
			case 12: case 1:case 2:
				System.out.println("겨울입니다.");
				break;
			default:
				System.out.println("잘못된 입력입니다.");
		}
		
		scanner.close();
		
	}
}


/*
 
월(1~12)을 입력하시오 : 4
봄입니다.




*/
