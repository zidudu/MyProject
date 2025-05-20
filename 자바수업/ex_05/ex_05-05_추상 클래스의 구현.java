abstract class Calculator
{
	public abstract int add(int a,int b);
	public abstract int substract(int a,int b);
	public abstract double average(int []a);
}



public class Book extends Calculator{
	@Override
	public int add(int a,int b) { // 추상 메소드 구현
		return a+b;
	}
	public int substract(int a,int b) { // 추상 메소드 구현
		return a-b;
	}
	public double average(int []a) { // 추상 메소드 구현
		double sum = 0;
		for(int i=0;i<a.length;i++) 
			sum += a[i];
		return sum/a.length;
	}
	
	//메인함수
    public static void main(String[] args) {
    	Book c = new Book();
    	System.out.println(c.add(2,3));
    	System.out.println(c.substract(2,3));
    	System.out.println(c.average(new int [] {2,3,4}));
    }
}

/*
5
-1
3.0

 */
 
