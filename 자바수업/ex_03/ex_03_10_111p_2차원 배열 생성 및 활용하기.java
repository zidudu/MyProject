import java.util.Scanner;
public class Hello2030 {
		
	//메인 함수
	public static void main(String[] args) {
		double score[][] = {{3.3,3.4}, // 1학년 1, 2학기 평점
							{3.5,3.6}, 		// 2학년 1, 2학기 평점
							{3.7,4.0},		// 3학년 1,2학기 평점
							{4.1,4.2} 		//4학년 1,2학기 평점
							};
		double sum = 0;
		for(int year=0;year<score.length;year++) { // 각 학년별로 반복 // score.length처럼 []이 없이 쓰면 행 첫번째임.
			for (int term = 0;term<score[year].length;term++) { // 각 학년의 학기별로 반복 // score[year] 은 행 다음 열이라는 뜻. 그래서 score[year].length는 열이니 2개.
				
				System.out.println((year+1) + " 학년 전체 " + (term+1) +" 학기 평점 : " + score[year][term]);								// Q. 근데 행 하나의 열 개수를 3개로 해버리면 어떨까? 
				sum += score[year][term]; // 전체 평점 합}
			}
			System.out.println();
		}
		
		
		// for-each로 2차원 배열 출력
		for(double[] sc: score) { // [D@1b28cdfa   [D@7229724f   [D@4c873330   [D@119d7047
			for(double i : sc)
				System.out.print(i + "   ");
		}
		System.out.println();
		int n = score.length; // 배열의 행 개수 4
		int m = score[0].length; // 배열의 열 개수 2
		System.out.println("4년 전체 평점 평균은 " + sum/(n*m)); // 평균 출력
	
	
	}
	
	
 }


/*
 
1 학년 전체 1 학기 평점 : 3.3
1 학년 전체 2 학기 평점 : 3.4

2 학년 전체 1 학기 평점 : 3.5
2 학년 전체 2 학기 평점 : 3.6

3 학년 전체 1 학기 평점 : 3.7
3 학년 전체 2 학기 평점 : 4.0

4 학년 전체 1 학기 평점 : 4.1
4 학년 전체 2 학기 평점 : 4.2

3.3   3.4   3.5   3.6   3.7   4.0   4.1   4.2   
4년 전체 평점 평균은 3.725

*/
