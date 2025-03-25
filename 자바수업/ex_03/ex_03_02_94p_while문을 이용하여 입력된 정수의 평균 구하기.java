import java.util.Scanner;
public class Hello2030 {
	//정수 여러개 입력 후 그 정수값의 개수와 정수들의 평균을 구함
	
	
	
	//메인 함수
	public static void main(String[] args) {
		Scanner scanner = new Scanner(System.in);
		int count = 0,n=0; // count는 입력되는 수의 개수
		double sum =0; //sum 은 합
		
		//정수 입력
		System.out.println("정수를 입력하고 마지막에 0을 입력하세요.");
		while((n=scanner.nextInt()) != 0) { //0이 입력되면 while 문 벗어남, // 문제점: n은 입력한 값을 계속 새로고침 하기 때문에 나중에 저장한 값을 볼때는 어려움. 배열을 사용해야 함
			sum = sum+n; // 입력 값 n 을 sum 과 더함 // sum+=n;
			count++;  // 입력 개수 셈
		}
		System.out.print("수의 개수는 " + count + "개이며 ");
		System.out.println("평균은 " + sum/count + "입니다.");
		scanner.close();
		
	
 }
}


/*
 

1+2+3+4+5+6+7+8+9+10==55

*/
