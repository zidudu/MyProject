import java.util.Scanner;
public class Hello2030 {
		
	//일차원 정수 배열 리턴
	static int[] makeArray() {
		int temp[] = new int[4]; //배열 생성
		for(int i=0;i<temp.length;i++) {
			temp[i] = i; // 배열 초기화 0,1,2,3
		}
		return temp; // 배열 리턴
	}
	
	
	//메인 함수
	public static void main(String[] args) {
		int intArray[]; //배열 레퍼런스 변수 선언
		intArray = makeArray(); // 메소드가 리턴한 배열 참조
		for(int i=0;i<intArray.length;i++)
			System.out.print(intArray[i] + " ");
	
	
	}
	
	
 }


/*
 
0 1 2 3 

*/
