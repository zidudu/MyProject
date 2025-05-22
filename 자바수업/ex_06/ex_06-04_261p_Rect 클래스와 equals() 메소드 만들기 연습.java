
class Rect{
	int width,height;
	public Rect(int width,int height) {
		this.width = width;this.height = height; 
	}
	public boolean equals(Object obj) {
		Rect p = (Rect)obj; // obj를 Rect로 다운 캐스팅
		if(width*height == p.width*p.height) return true;
		else return false;
		
		
	}
	
	
}
//이 사각형과 객체 p 의 면적 비교

public class Book{
	//메인함수
    public static void main(String[] args) {
    	Rect a = new Rect(2,3);
    	Rect b = new Rect(3,2);
    	Rect c = new Rect(3,4);
    	if(a.equals(b)) System.out.println("a is equal to b");
    	if(a.equals(c)) System.out.println("a is equal to c");
    	if(b.equals(c)) System.out.println("b is equal to c");
    }
}

/*
a is equal to b
 */
 
