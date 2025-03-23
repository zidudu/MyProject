import java.util.Scanner;
public class Hello2030 {
	
	//증감연산자 : ++,--. 피연산자의 앞 또는 뒤에 붙어 값 1 증가시키거나 1감소함.
	
	//메인 함수
	public static void main(String[] args) {
		int a=3,b=3,c=3;
		//대입 연산자 사례
		a += 3; // a+3 = 3+3 =6
		b*=3; // b*3= 3*3 = 9
		c%=2; // c%2= 3%2= 1;
		System.out.println("a="+a+", b="+b+", c="+c); // 변한 값들 표시
		
		int d = 3;
		//증감 연산자 사례
		a = d++; // d는 4, 근데 d가 3인상태로 a에 들어가고 증감됨. a=3, d=4
		System.out.println("a="+a+", d="+d);
		a = ++d; // d=5이고, 증감된 후 a에 들어감. a = 5, d=5
		System.out.println("a="+a+", d="+d);
		a = d--; // a= 5, d =4
		System.out.println("a="+a+", d="+d);
		a = --d; // a= 3, d=3
		System.out.println("a="+a+", d="+d);
		
	}
}


/*
 
a=6, b=9, c=1
a=3, d=4
a=5, d=5
a=5, d=4
a=3, d=3

*/
