
#include <stdio.h>
#include <math.h>
#include <stdlib.h> 
#include <windows.h>
#include <string.h>

int A = 0;
int M = 1101;
int Q = 1001;
int C = 0;

void Plus() {
    int nanugi = 10, suchi = 1;
    int total = 0, Carry = 0;

    for (int i = 0; i < 4; i++) {
        int A_bit = (A % nanugi) / suchi;
        int M_bit = (M % nanugi) / suchi;
        int sum = A_bit + M_bit + Carry;
        
        Carry = sum > 1 ? 1 : 0;
        sum = sum % 2;
        
        total += sum * suchi;
        nanugi *= 10;
        suchi *= 10;
    }

    A = total;
    C = Carry;
}

void R_Shift() {
    Q = (A % 10) * 1000 + (Q / 10);
    A = (C * 1000) + (A / 10);
    C = 0;
}

int two_to_ten(int Two_total, int x) {
    int Ten_total = 0;
    for (int i = 0; i < x; i++) {
        Ten_total += ((Two_total % 10) * (1 << i));
        Two_total /= 10;
    }
    return Ten_total;
}

int main() {
    for (int i = 0; i < 4; i++) {
        int Q_Check = Q % 10;
        if (Q_Check == 1) {
            Plus();
        }
        R_Shift();
        printf("Cycle %d => C: %d, A: %04d, Q: %04d\n", i, C, A, Q);
    }

    int Two_total = A * 10000 + Q;
    int M_ten = two_to_ten(M, 4);
    int Q_ten = two_to_ten(Q, 4);
    int Ten_total = two_to_ten(Two_total, 8);
    printf("\nM(%d) X Q(%d) = %d\n", M_ten, Q_ten, Ten_total);

    return 0;
}
/*
Cycle 0 => C: 0, A: 0110, Q: 1100
Cycle 1 => C: 0, A: 0011, Q: 0110
Cycle 2 => C: 0, A: 0001, Q: 1011
Cycle 3 => C: 0, A: 0111, Q: 0101

M(13) X Q(5) = 117
*/
