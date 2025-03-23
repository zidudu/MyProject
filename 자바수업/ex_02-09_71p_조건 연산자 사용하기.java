import java.util.Scanner;
public class Hello2030 {
	
	//  int x=5,y=3; int big = (x>y)?x:y; // x가 y보다 크기 때문에 x 값 5가 big에 대입됨.
	
	//메인 함수
	public static void main(String[] args) {
		int a = 3, b=5;
		
		System.out.println("두 수의 차는 " + ((a>b)? (a-b):(b-a))); // b 가 더 크니 false이고 b-a = 5-3 =2 가 출력됨.
		
	}
}


/*

두 수의 차는 2

*/
