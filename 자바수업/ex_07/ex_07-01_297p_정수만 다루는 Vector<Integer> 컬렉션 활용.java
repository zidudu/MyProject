import java.util.Vector;

public class Main {
    public static void main(String[] args) {
        // 정수 값만 다루는 제네릭 벡터 생성
        Vector<Integer> v = new Vector<Integer>();
        v.add(5); // 5 삽입
        v.add(4); // 4 삽입
        v.add(-1); // -1 삽입

        // 벡터 중간에 삽입하기
        v.add(2,100);

        System.out.println("벡터 내 요소 객체 수 : " + v.size());
        System.out.println("벡터의 현재 용량 : "+ v.capacity());

        for(int i=0;i<v.size();i++){
            int n=v.get(i);
            System.out.println(n);
        }
        int sum =0;
        for(int i=0;i<v.size(); i++){
            int n = v.elementAt(i);
            sum +=n;
        }
        System.out.println("벡터에 있는 정수 합 : "+ sum);
    }
}
/*
벡터 내 요소 객체 수 : 4
벡터의 현재 용량 : 10
5
4
100
-1
벡터에 있는 정수 합 : 108
*/
