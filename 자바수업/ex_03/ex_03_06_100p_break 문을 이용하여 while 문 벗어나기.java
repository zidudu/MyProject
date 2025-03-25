import java.util.Scanner;
public class Hello2030 {
		
	//메인 함수
	public static void main(String[] args) {
		Scanner scanner = new Scanner(System.in);
		
		System.out.println("exit을 입력하면 종료합니다");
		while(true) {
			System.out.print(">>");
			String text = scanner.nextLine();
			if(text.equals("exit")) //exit 입력되면 반복 종료
				break; // while 문을 벗어남
		}
		System.out.println("종료합니다..");
		
		
		scanner.close();
		
		
		
		/* int n=0;
		while(true){
			n++;
			if(n>=20) break;
			if(n%3 == 0) continue;
			System.out.print(n+ " ");	
		}
		*/
	
 }
}


/*
 
exit을 입력하면 종료합니다
>>edit
>>exit
종료합니다..


1 2 4 5 7 8 10 11 13 14 16 17 19 

*/
