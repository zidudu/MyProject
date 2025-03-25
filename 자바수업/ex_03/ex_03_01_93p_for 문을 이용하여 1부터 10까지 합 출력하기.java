import java.util.Scanner;
public class Hello2030 {	
	
	//메인 함수
	public static void main(String[] args) {
		int i,sum=0;
		for(i=1;i<=10;i++) { //1~10까지 반복
			sum+=i;
			System.out.print(i); // 더하는 수 출력
			if(i<=9) //1~0까지는 + 출력
				System.out.print("+");
			else { // i 10인경우
				System.out.print("=");
				System.out.print(sum);
			}
		}
		
		
	
		
	}
	}


/*
 

1+2+3+4+5+6+7+8+9+10==55

*/
