import java.util.Random;
import java.util.Scanner;
import java.util.InputMismatchException;

public class Book {
	String title;
	String author;
	
	void show() {System.out.println(title+" "+author);}

	public Book()
	{
		this("","");
		System.out.println("생성자 호출됨");
	}
	public Book(String t) {
		title = t;
		author = "작자미상";
	}
	public Book(String t,String a) {
		title = t;
		author = a;
		
	}
	//메인함수
    public static void main(String[] args) {
    
    	Book littlePrince = new Book("어린왕자","생텍쥐페리");
    	
    	Book loveStory = new Book("춘향전");
    	Book emptyBook = new Book();
    	loveStory.show();
    	//System.out.println(littlePrince.title + " "+ littlePrince.author);
    	//System.out.println(loveStory.title + " "+ loveStory.author);
    
    
    }
}

/*
 생성자 호출됨
춘향전 작자미상
 */
 
