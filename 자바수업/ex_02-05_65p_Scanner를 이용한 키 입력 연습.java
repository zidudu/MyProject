import java.util.Scanner;
public class Hello2030 {
	
	// System.in
	// 키보드 장치와 직접 연결되는 표준 입력 스트림 객체
	// 키 값을 바이트 정보로 바꿔 제공하는 저수준 스트림 객체
	// 키보드 입력 받기 위해 System.in 을 직접 사용하면 읽은 바이트 정보를 응용프로그램이 문자나 숫자로 변환해야 하는 번거로움 있음.
	// -> 키보드에서 입력된 키보드를 문자나 정수,실수,문자열 등 사용자가 원하는 타입으로 변환해주는 고수준 스트림 클래스인 Scanner를 사용할 것을 권장
	// Scanner : 키 입력을 위한 목적으로 자바 패키지에서 제공되는 클래스
	// Scanner 객체 생성 : Scanner scanner = new Scanner(System.in);
	//scanner 객체는 System.in으로 해서 키보드로부터 입력을 받게 하고, System.in 이 변환하는 바이트 스트림을 응용프로그램의 입맛에 따라 문자,문자열,정수,실수 등으로 변환하여 리턴함.
	// import 문 필요 : import java.util.Scanner; // Scanner 사용하려면 이 import문 필요함.
	// import 문은 Scanner 클래스의 경로명이 java.util.Scanner임을 알려줌. 이 import 문 없으면 자바 컴파일러가 Scanner 클래스 코드 어디 있는지 못찾음
	//Scanner 클래스는 사용자가 입력하는 키 값을 공백('\f','\r',' ', '\n')으로 구분되는 토큰 단위로 읽는다.
	//응용프로그램은 Scanner 클래스의 메소드(함수)를 호출하여 각 토큰을 원하는 타입으로 변환하여 얻어낼 수 있음.
	//응용프로그램에서 Scanner를 닫는 코드가 없으면 컴파일 시에 경고 발생하지만 특별한 문제 x.  프로그램 종료하면 자동으로 닫힘
	
	
	//메인 함수
	public static void main(String[] args) {
		System.out.println("이름, 도시, 나이, 체중, 독신 여부를 빈칸으로 분리하여 입력하세요");
		
		Scanner scanner = new Scanner(System.in); //Scanner 객체 생성함. 여기서 입력값 받고 난 후 scanner 메소드를 통해 분배
		String name = scanner.next(); //문자열 토큰 읽기
		System.out.println("당신의 이름은 "+name+"입니다");
		String city = scanner.next(); // 문자열 토큰 읽어서 city 클래스에 넣음
		System.out.println("당신이 사는 도시는 "+city+"입니다");
		int age = scanner.nextInt(); //정수 토큰 읽기
		System.out.println("당신의 나이는 "+age+"살입니다");
		double weight = scanner.nextDouble(); //실수 토큰 읽기
		System.out.println("당신의 체중은 "+weight+"kg입니다");
		boolean single = scanner.nextBoolean(); // 논리 토큰 읽기
		System.out.println("당신은 독신 여부는 "+single + "입니다");
		
		scanner.close(); // scanner 스트림 닫기
	}
}


/*
 
이름, 도시, 나이, 체중, 독신 여부를 빈칸으로 분리하여 입력하세요
kim Seoul 20 65.1 true
당신의 이름은 kim입니다
당신이 사는 도시는 Seoul입니다
당신의 나이는 20살입니다
당신의 체중은 65.1kg입니다
당신은 독신 여부는 true입니다

*/
