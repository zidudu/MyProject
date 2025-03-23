import java.util.Scanner;
public class Hello2030 {
	
	//연산: 주어진 식을 계산하여 결과를 얻어내는 과정
	
	//메인 함수
	public static void main(String[] args) {
		Scanner scanner = new Scanner(System.in); 
		
		System.out.print("정수를 입력하세요 : ");
		int time = scanner.nextInt(); //정수 입력
		int second = time % 60; //60으로 나눈 나머지는 시간과 분이 다 빠져나간 후의 남은 초다.
		int minute = (time / 60) % 60; // 60으로 나눈 몫을 다시 60으로 나눈 나머지는 분
		int hour = (time / 60) / 60; // 60으로 나눈 몫을 다시 60으로 나눈 몫은 시간(분을 구하고, 시간을 구함)
		
		System.out.print(time + "초는 ");
		System.out.print(hour + " 시간, ");
		System.out.print(minute + "분, ");
		System.out.print(second + "초입니다. ");
		scanner.close(); // scanner 스트림 닫기
	}
}


/*
 
정수를 입력하세요 : 4000
4000초는 1 시간, 6분, 40초입니다. 

*/
