import java.util.Scanner;
public class Hello2030 {
		
	//메인 함수
	public static void main(String[] args) {
	int [] n = {1,2,3,4,5}; // 배열 생성 및 초기화
	int sum = 0; // 총합
	for(int k : n) { // k 는 n[0],n[1], ... , n[4] 로 반복
		System.out.print(k+ " "); // 반복되는 k 값 출력
		sum += k;	
	}
	System.out.println("합은 "+ sum);
	
	String f[] = {"사과","배","바나나","체리","딸기","포도"};
	for(String s:f) // s는 f[0],f[1], ..., f[5]로 반복
		System.out.print(s + " ");		
	}
	
	
 }


/*
 
1 2 3 4 5 합은 15
사과 배 바나나 체리 딸기 포도 

*/
