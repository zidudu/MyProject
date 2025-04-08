import java.util.Random;
import java.util.Scanner;
import java.util.InputMismatchException;

public class Hello2030 {
	String title;
	String author;
	
	public Hello2030(String t) {
		title = t;
		author = "작자미상";
	}
	public Hello2030(String t,String a) {
		title = t;
		author = a;
		
	}
	//메인함수
    public static void main(String[] args) {
    
    	Hello2030 littlePrince = new Hello2030("어린왕자","생텍쥐페리");
    	
    	Hello2030 loveStory = new Hello2030("춘향전");
    	System.out.println(littlePrince.title + " "+ littlePrince.author);
    	System.out.println(loveStory.title + " "+ loveStory.author);
    
    
    }
}

/*
 어린왕자 생텍쥐페리
춘향전 작자미상
 */
 
