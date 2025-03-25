import java.util.Scanner;
public class Hello2030 {
		
	//메인 함수
	public static void main(String[] args) {
		Scanner scanner = new Scanner(System.in);
		
		
		int sum =0; // 전체 값 
		int count = 0; // 양수 카운트 값
		int maxCounting = 5; // 카운트되는 최대값
		System.out.println("정수를 " + maxCounting +" 개 입력하세요.");
		System.out.print(">> ");
		
		//반복하면서 maxCounting까지 될때까지 반복하여 양수 입력
		while(count < maxCounting) {
			int n=scanner.nextInt(); // 키보드에서 정수 입력
			if(n<=0) {
				System.out.println("음수입니다. 다시 입력하세요");
				continue;
				
			} //0이나 음수인 경우 더하지 않고 다음 반복으로 진행
			else {
				count ++;
				sum +=n;
				System.out.println("현재 입력개수:"+ count + " | 입력한 값 : " + n + " | 현재 sum = " + sum);
				System.out.println("정수를 " + (maxCounting - count) +" 개 더 입력하세요");
				System.out.print(">> ");
				} // 양수인 경우 덧셈
		}
		System.out.println("\n[전체 입력값 = "+ sum +"]" );
	
		
		scanner.close();
	
 }
}


/*
 

a b c d e f g h i j k l m n o p q r s t u v w x y z 

*/
