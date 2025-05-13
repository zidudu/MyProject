class Person{} // 부보 클래스
class Student extends Person{} // 스튜던트는 펄슨을 상속받음
class Researcher extends Person{}  // 리서쳐는 펄슨을 상속받음
class Professor extends Researcher{} // 프로페서는 리서쳐를 상속받음



public class Book {
	
	//호출할때 부모 클래스부터 호출하게 되는 방식이라 생각하면 됨. 학생 호출할때 그 위에 부모는 사람 클래스임.
	static void print(Person p) {
		if(p instanceof Person) {
			System.out.print("Person ");
		}
		if(p instanceof Student) {
			System.out.print("Student ");
		}
		if(p instanceof Researcher) {
			System.out.print("Researcher ");
		}
		if(p instanceof Professor) {
			System.out.print("Professor ");
		}
		System.out.println();
	}
	
	//메인함수
    public static void main(String[] args) {
    	System.out.print("new Student() -> "); print(new Student());
    	System.out.print("new Researcher() -> "); print(new Researcher());
    	System.out.print("new Professor()  ->"); print(new Professor());
    }
}

/*
new Student() -> Person Student 
new Researcher() -> Person Researcher 
new Professor()  ->Person Researcher Professor 

호출할때 부모 클래스부터 호출하게 되는 방식이라 생각하면 됨

 */
 
