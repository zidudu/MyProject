// tip: each public class is put in its own file
public class main 
{
    // tip: arguments are passed via the field below this editor
    public static void main(String[] args)
    {
        //a와 b short 선언
        short a = (short)0x55ff; // 십진수 21759. 55ff는 16진수 값
        short b = (short)0x00ff; // 00ff는 16진수 값값

        //비트를 숫자 연산
         System.out.println("a = " + Integer.toHexString(a));
        System.out.println("b = " + Integer.toHexString(b));
         System.out.println("a & b = " + Integer.toHexString(a&b));
        
        //c와 d 바이트 선언
        byte c =20; //0x14
        byte d = -8; // 0xf8

        //c와 b 연산
        System.out.println("c = " + Integer.toBinaryString(c));
        System.out.println("d = "+ Integer.toBinaryString(d));
         System.out.println("d >>2 "+ Integer.toBinaryString(d>>2));

        //비트 연산
        // printf("%04", ...) 메소드는 값을 4자리의 16진수로 출력하고, 빈 곳에는 0을 삽입입
        System.out.println("[비트 연산 결과]");
        System.out.printf("%04x\n", (short)(a & b)); //비트 AND
        System.out.printf("%04x\n",(short)(a | b)); // 비트 OR
        System.out.printf("%04x\n",(short)(a ^ b)); //비트 XOR
        System.out.printf("%04x\n",(short)(~a)); // 비트 NOT
 /*[비트 연산 결과]
    00ff
    55ff
    5500
    aa00
   */
        byte c = 20; //0x14
        byte d = -8; //0xf8

        //시프트 연산
        System.out.println("[시프트 연산 결과]");
        System.out.println(c << 2); // c를 2비트 왼쪽 시프트
        System.out.println(c >> 2); // c를 2비트 오른쪽 시프트. 0 삽입
        System.out.println(d >> 2); // d를 2비트 오른쪽 시프트. 1 삽입
        System.out.printf("%x\n",(d >>>2)); // d를 2비트 오른쪽 시프트. 0 삽입입
       

       /*
      
    [시프트 연산 결과]
    80
    5
    -2
    3ffffffe
       
       */
    }
}
