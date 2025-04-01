import java.util.Scanner;
import java.util.InputMismatchException;
public class Hello2030 
{
	//메인 함수
	public static void main(String[] args) {
		Scanner scanner = new Scanner(System.in);
		
		String arr;
		
		//입력 문자열에 문자 갯수
		System.out.println("소문자,대문자,숫자를 입력하세요");
		arr = scanner.nextLine();
		int upperCnt = 0;
		int lowerCnt = 0;
		int digitCnt = 0;
		//입력한 값에서 for을 돌려 소문자,대문자,숫자를 구분함.
		for(int i=0;i<arr.length();i++) {
			//소문자
			if((arr.charAt(i)>='a') && (arr.charAt(i) <='z') )
				lowerCnt++;
			//대문자
			else if((arr.charAt(i) >='A') && (arr.charAt(i) <='Z'))
				upperCnt++;
			//숫자
			else if((arr.charAt(i)>='0')&&(arr.charAt(i) <='9'))
				digitCnt++;
			
		}
		System.out.println("대문자 = "+upperCnt + " 소문자 = "+lowerCnt+" 숫자 = " + digitCnt + " ");
		
		//1. 문자열을 입력받고 입력받은 문자열의 순서를 거꾸로 출력해라
		//2.입력한 문자열의 대문자와 소문자,숫자가 각각 몇개인지 세는 프로그램을 작성해라. 그 외 특수문자 무시
		
		//1. 문자열을 입력받고 입력받은 문자열의 순서를 거꾸로 출력해라
		// String next()
		
		 
		
		
	
	}
}


/*
 a e d E d 3 4
대문자 = 1소문자 = 4 숫자 = 2 


*/
