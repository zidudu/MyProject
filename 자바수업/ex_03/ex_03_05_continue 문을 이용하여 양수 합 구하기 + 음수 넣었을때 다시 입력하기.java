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
 

정수를 5 개 입력하세요.
>> 3 4 -5 6 7
현재 입력개수:1 | 입력한 값 : 3 | 현재 sum = 3
정수를 4 개 더 입력하세요
>> 현재 입력개수:2 | 입력한 값 : 4 | 현재 sum = 7
정수를 3 개 더 입력하세요
>> 음수입니다. 다시 입력하세요
현재 입력개수:3 | 입력한 값 : 6 | 현재 sum = 13
정수를 2 개 더 입력하세요
>> 현재 입력개수:4 | 입력한 값 : 7 | 현재 sum = 20
정수를 1 개 더 입력하세요
>> 2 4 5
현재 입력개수:5 | 입력한 값 : 2 | 현재 sum = 22
정수를 0 개 더 입력하세요
>> 
[전체 입력값 = 22]

*/
