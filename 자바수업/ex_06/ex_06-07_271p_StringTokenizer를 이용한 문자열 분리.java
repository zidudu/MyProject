import java.util.StringTokenizer;

public class Main {
    public static void main(String[] args) {
        String query = "name=kitae&addr=seoul&age=21"; // 쌍따옴표 수정
        StringTokenizer st = new StringTokenizer(query, "&"); // 구분자 문자열로 지정

        int n = st.countTokens(); // 분리된 토큰 개수

        System.out.println("토큰개수 = " + n); // println으로 수정
        while(st.hasMoreTokens()) {
            String token = st.nextToken(); // 토큰 얻기
            System.out.println(token); // 토큰 출력
        }
    }
}
/*
토큰개수 = 3
name=kitae
addr=seoul
age=21
*/
