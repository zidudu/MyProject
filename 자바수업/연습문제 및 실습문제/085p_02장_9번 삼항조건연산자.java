import java.util.Scanner;
public class Hello2030 {
	
	
	
	//메인 함수
	public static void main(String[] args) {
		int i;
		int j = 20;
	    i = (j%2 == 0)? 10:20;  // (i%2) 라고 쓰면 오류.0과 1이 c언어에선 bool이었는데 여기선 0과 1은 정수 값이고 boolean 이 아님.
	    	// 즉, 삼항 연산자의 조건식은 무조건 boolean 값이어야 함.
		System.out.println("i = "+i);
		
	}
	}


/*
 

i = 10


*/
