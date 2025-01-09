#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <stdbool.h>
#include <stdlib.h>
#include<string.h>

char* solution(const char* words[], int words_len) {

    //문제는 주어진 문자열들이 있는 포인터 배열 words를 가지고 문자열 길이가 words_len 값보다 이상인 것들만 answer 뒤에 붙여야함.
    
    //for 변수 
    int i;

    // 초기 상태에서 answer을 초기화해서 포인터 배열을 선언함 (char* answer[51];)
    // 근데 이건 방만 만들어둔거라서 값이 초기화되어있지 않음. 즉 NULL 값도 없음.
    char* answer = (char*)malloc(sizeof(char) * 10 * words_len + 1); // 10 * 5+1  = 51이 됨
   
    // **해결(1)** : answer의 0값에 NULL 값인 \0을 붙임. 이렇게 되면 값이 아예 비어있는 값이 됨. 
    //왜 ' '는 안되나요? => 왜냐면 공백도 하나의 문자로 취급하기 때문. 
    // 그리고 임의로 값을 넣은 것이기 때문에 공백만 넣으면 answer[0] = ' ';  -> NULL 이 뒤에 추가되지 않기 때문에 strcat에서 문제가 생김
    // => 그렇기 때문에 answer의 처음 0 값을 NULL로 초기화해줘야 strcat으로 문자열을 연결이 됨. 
    answer[0] = '\0';

    //반복을 words_len 길이만큼 반복함.
    for (i = 0; i < words_len; i++) {

        //strlen을 계산할때는 NULL(\0)를 계산하지 않음
        if (strlen(words[i]) >= 5) { 
            strcat(answer, words[i]); // 뒤에 한 단어씩 붙여나감
        }
        //else if (strlen(answer) == NULL) {
        //   
        //    //answer = "empty";
        //}
    }

    // **해결(2)** :  for문 다 돌렸는데 아직도 answer에 문자열이 붙어있는게 없다면(문자열에 연결한 값이 아예 없다면),
    // => 즉, answer 문자열이 null이면  strcat함수로 answer에 empty를 연결시키고, 그 값을 반환
    if (strlen(answer) == NULL ) { // for문 다 돌렸는데 아직도 answer에 문자열이 붙어있는게 없다면 empty를 연결함.
        strcat(answer, "empty");
    }
    
    return answer;
}


int main() {
    //문자열의 길이가 5보다 길면 이어붙이고, answer이 0이 반환되면 empty를 출력한다.
    
    // 이렇게 초기화하면 각각의 문자열 뒤에 NULL(\0)이 붙게 됨. 
    const char* words1[5] = { "my", "favorite", "color", "is", "violet" };
    int words_len1 = 5;
    char* ret1 = solution(words1, words_len1);


    printf("solution  %s .\n", ret1); //5보다 긴 문자열들을 이어붙인 총 문자열을 출력

    const char* words2[3] = { "yes", "i", "am" };
    int words_len2 = 3;
    char* ret2 = solution(words2, words_len2);


    printf("solution  %s .\n", ret2);

    //결과값
    /*
    
    solution  favoritecolorviolet .
    solution  empty .
    
    */
    // 뒤 .은 원래 printf에 있는 거임.
}
