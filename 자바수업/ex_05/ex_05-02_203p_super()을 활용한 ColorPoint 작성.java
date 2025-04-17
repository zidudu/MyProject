// 좌표 출력하는 클래스
class Point{
	private int x,y; // 한점을 구성하는 x y 좌표
	public Point() {
		this.x=0; this.y=0;
	}
	// x,y 좌표 받는 메소드
	public Point(int x,int y) {
		this.x = x; this.y= y;
	}
	// 점 좌표 출력 메소드
	public void showPoint() { // 점의 좌표 출력
		System.out.println("(" + x + "," + y + ")");
	}
	
}
// Point 상속받은 컬러 색 출력 클래스
class ColorPoint extends Point{ // Point 를 상속받은 ColorPoint 선언
	private String color; // 점의 색
	// 색 받는 메소드
	public ColorPoint(int x, int y, String color) {
		super(x,y); // Point의 생성자 Point(x,y) 호출
		this.color = color;
	}
	//색과 좌표 출력하는 메소드
	public void showColorPoint() { // 컬러 점의 좌표 출력
		System.out.print(color);
		showPoint(); // Point 클래스의 showPoint() 호출
	}
}
//메인 클래스 
public class ColorPointEx {
	public static void main(String [] args) {
		ColorPoint cp = new ColorPoint(5, 6, "blue"); // ColorPoint 객체 생성
		cp.showColorPoint(); // 컬러와 좌표 출력	
	}	
}

/*
blue(5,6)
 */
