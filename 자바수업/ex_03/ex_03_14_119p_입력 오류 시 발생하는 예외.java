import java.util.Scanner;
import java.util.InputMismatchException;
public class Hello2030 
{
	//메인 함수
	public static void main(String[] args) {
		Scanner scanner = new Scanner(System.in);
		System.out.println("정수 3개를 입력하세요");
		int sum =0; // 총합
		int n = 0; //각각의 입력값
		//for 문 돌려 sum에 더함
		// 문자가 입력되면 다시 입력하라고 나옴
		for(int i=0;i<3;i++) {
			System.out.print(i+"번째 >>");
			try {
				n = scanner.nextInt(); // 정수 입력
			}
			catch(InputMismatchException e) {
				System.out.println("정수가 아닙니다. 다시 입력하세요!");
				scanner.next(); // 입력스트림에 있는 정수가 아닌 토큰을 버린다. 즉 문자를 읽고 나서 버리게 된다.
				i--; // 인덱스가 증가하지 않도록 미리 감소
				continue; // 다음 루프
			}
			sum+=n; // 합하기
		}
		System.out.println("합은 "+sum);
		scanner.close(); 
		
		
	
	}
}


/*
 
0 1 2 3 

*/
