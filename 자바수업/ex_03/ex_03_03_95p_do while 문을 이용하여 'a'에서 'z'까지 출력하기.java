import java.util.Scanner;
public class Hello2030 {
		
	//메인 함수
	public static void main(String[] args) {
		char a = 'a'; // 문자 a
		
		do {
			System.out.print(a + " "); // 문자 출력
			a = (char)(a+1);// 아스키 값에 1 더함. b 가 됨. 알파벳의 경우 1을 더하면 다음 문자의 코드 값
		} while(a<='z'); //z 까지 반복해서 출력
		
	
	
 }
}


/*
 

a b c d e f g h i j k l m n o p q r s t u v w x y z 

*/
