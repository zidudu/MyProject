public class Hello2030 {
	
	//상수:  선언시 값이 초기화되면 변경 불가능
	// final 키워드로 선언, 리터럴을 상수로 선언하면 변수처럼 표현 가능하다.
	// ex : final double PI = 3.141592;
	// * static을 앞에 붙여 선언하는게 더 바람직함.
	//메인 함수
	public static void main(String[] args) {
	final double PI = 3.14; //원주율을 상수로 선언
	double radius = 10.2; // 원의 반지름
	double circleArea = radius*radius*PI; // 원의 면적 계산
	
	//원의 면적 화면에 출력
	System.out.print("반지름 " + radius + ", "); //반지름 출력
	System.out.println("원의 면적 = " + circleArea ); // 원의 면적 출력
	}
}


/*
 
반지름 10.2, 원의 면적 = 326.68559999999997


*/
