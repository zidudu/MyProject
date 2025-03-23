import java.util.Scanner;
public class Hello2030 {
	
	//비교연산자 : 두개의 피연산자를 비교하여 true 또는 false의 논리 결과를 내는 연산자
	// 논리 연산자 : 논리 값을 대상으로 AND,OR,XOR,NOT 의 논리 연산을 하여 논리 값을 내는 연산자
	// 비교 연산자 =>     <,>,<=,>=,==,!=
	// 논리 연산자 =>     !(NOT), ^(XOR),||(OR),&&(AND)
	/*
	 ex)
	  (age >=20 && age < 30) //나이가 20대인 경우
	  (c>='A')&&(c<='Z') // 문자가 대문자인 경우(아스키값으로 값 사이에 있는지 판별)
	  (x>=0)&&(y>=0)&&(x<=50)&&(y<=50) // (x,y)가 00과 50 50 의 사각형 내에 있음
	  
	  // * 20 <=age <30 은 오류
	 * */
	
	//조건 연산자 : 세개의 피연산자로 구성되어 삼항 연산자라고도 한다.
	// condition ? opr2 : opr3  // condition 이 true면 opr2, false 면 opr3 가 된다.
	
	//메인 함수
	public static void main(String[] args) {
		
		System.out.println('a'>'b'); // false
		System.out.println(3 >= 2); // true
		System.out.println(-1<0); // T
		System.out.println(3.45<=2); // F
		System.out.println(3==2); // F
		System.out.println(3!=2); // T
		System.out.println(!(3!=2)); // F
		System.out.println((3>2) && (3>4)); // F
		System.out.println( (3!=2) || (-1 > 0) ); // T
		System.out.println((3!=2) ^ (-1>0)); // T // ^는 두개가 같지 않을때 T, 같으면 F임.
	}
}


/*
 
false
true
true
false
false
true
false
false
true
true

*/
