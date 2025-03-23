import java.util.Scanner;
public class Hello2030 {
	
	// 비트 논리 연산 : 피연산자의 각 비트들끼리 이루어지는 AND, OR, XOR, NOT 의 논리 연산
	//  &(AND) , |(OR) , ^(XOR) , ~(NOT)
	
	//시프트 연산자 : 각 비트들을 대상으로 연산 이루어짐. 3개의 연산자 있음. 비트를 왼쪽이나 오른쪽으로 이동시킴
	//산술적 시프트 : 최상위 비트는 기존꺼 남기고 이동 (그냥 산술 먹고 난 떠나보내기 싫다고 생각)
	//논리적 시프트 최상위 비트도 같이 옮겨버림. 결국 0이 됨.
	// a>>b : a의 각 비트를 오른쪽으로 b 번 시프트함. 최상위 비트의 빈자리는 시프트 전의 최상위 비트로 다시 채운다. 산술적 오른쪽 시프트
	// a>>>b : a의 각 비트를 오른쪽으로 b번 시프트한다. 그리고 최상위 비트의 빈자리는 0으로 채운다. 논리적 오른쪽 시프트라고 한다.
	// a<<b : a의 각 비트를 왼쪽으로 b번 시프트한다. 그리고 최하위 비트의 빈자리는 0으로 채운다. 산술적 왼쪽 시프트라고 한다.(이건 예외. 최하위껄 0으로 만들어버림;;;)
	
	//메인 함수
	public static void main(String[] args) {
		short a = (short)0x55ff;
		short b= (short)0x00ff;
		//비트 연산
		System.out.println("[비트 연산 결과]");
		System.out.printf("%04x\n",(short)(a & b)); // 비트 AND
		System.out.printf("%04x\n",(short)(a | b)); // 비트 OR
		System.out.printf("%04x\n",(short)(a ^ b)); // 비트 XOR
		System.out.printf("%04x\n",(short)(~a)); // 비트 NOT
		
		byte c = 20; //0x14. 16진수로 14.
		byte d = -8; //0xf8. 16진수로 -8.
		
		//시프트 연산
		System.out.println("[시프트 연산 결과]");
		System.out.println(c<<2); // c를 2비트 왼쪽 시프트 (c에 4를 곱한 결과가 나타난다. 2를 시프트 하는데 1번 할때마다 2씩 곱하고 나눈다고 생각하면 편하다). 20*4=80
		System.out.println(c >> 2); // c를 2비트 오른쪽 시프트 . 0 삽입. 산술적 오른쪽 시프트 했는데 c가 양수라 최상위 비트가 0임. 그래서 시프트해도 0됨. 나누기 4
		System.out.println(d >> 2); //d를 2비트 오른쪽 시프트. 1삽입. 산술적 오른쪽 시프트인데 d가 음수이므로 최상위 비트가 1임. 그래서 시프트 시에 1이 삽입. 그래서 나누기 4가 됨.
		System.out.printf("%x\n", (d >>> 2)); // d를 2비트 오른쪽 시프트. 0삽입. d(0xf8)는 시프트 연산 전에 int 타입으로 바뀌어 32비트의 fffffff8가 된다. 
		//그러고 나서 >>> 연산 이루어지면 0이 2번 삽입되어 3ffffffe 가 된다.
		
		
		//75p check time 문제 1번
		int x=2,y=10,z=0;
		z = x++ * 2 + --y - 5 + x * (y%2);
		System.out.println("[z 연산 결과] : " + z); // [z 연산 결과] : 11
		System.out.println(8>>1); //4
		
		int opr =4;
		System.out.println(opr++); //4
		
	}
	
	
	
	
}


/*
[비트 연산 결과]
00ff
55ff
5500
aa00
[시프트 연산 결과]
80
5
-2
3ffffffe

*/
