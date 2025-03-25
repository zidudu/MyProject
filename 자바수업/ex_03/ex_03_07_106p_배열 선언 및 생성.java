import java.util.Scanner;
public class Hello2030 {
		
	//메인 함수
	public static void main(String[] args) {
	Scanner scanner = new Scanner(System.in);
	
	int intArray[]; // 배열에 대한 레퍼런스 선언
	intArray = new int[5]; // 배열 생성
	int max = 0; // 현재 가장 큰 수
	int maxIndex = 0; // 현재 가장 큰 수의 인덱스 번호
	System.out.println("양수 5개를 입력하세요.");
	for(int i=0;i<5;i++) {
		intArray[i] = scanner.nextInt(); // 입력받은 정수를 배열에 저장
		System.out.println("번호 : " + i +" | 값 : " + intArray[i]); // 입력한 값의 번호와 값 표시
		if(intArray[i] > max) { //입력받은 수가 현재 제일 크면
			max = intArray[i]; // max 변경
			maxIndex = i; // 큰 값의 인덱스 값 저장
		}
	}
		
	System.out.print("가장 큰 수는 "+max+"입니다. | 인덱스 : "+ maxIndex ); // 큰 수 값과 인덱스 값 표시
	
 }
}


/*
 
양수 5개를 입력하세요.
3 43 32 43 3
번호 : 0 | 값 : 3
번호 : 1 | 값 : 43
번호 : 2 | 값 : 32
번호 : 3 | 값 : 43
번호 : 4 | 값 : 3
가장 큰 수는 43입니다. | 인덱스 : 1

*/
