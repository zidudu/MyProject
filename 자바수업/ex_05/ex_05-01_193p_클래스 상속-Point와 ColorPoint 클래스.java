// 좌표 출력하는 클래스
class Point{
	private int x,y; // 한점을 구성하는 x y 좌표
	// x,y 좌표 받는 메소드
	public void set(int x,int y) {
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
	public void setColor(String color) {
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
		Point p = new Point(); // Point 객체 생성
		p.set(1, 2); // Point 클래스의 set() 호출
		p.showPoint(); // 1,2 출력
		
		ColorPoint cp = new ColorPoint(); // ColorPoint 객체 생성
		cp.set(3, 4); // Point 클래스의 set() 호출
		cp.setColor("red"); // ColorPoint 클래스의 setColor() 호출
		cp.showColorPoint(); // 컬러와 좌표 출력	
	}	
}

/*
 (1,2)
red(3,4)
 */
