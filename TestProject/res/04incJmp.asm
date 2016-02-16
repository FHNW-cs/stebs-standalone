; -------------------------------------------------------------------------
;  COUNTING.
; -------------------------------------------------------------------------

	MOV	BL,40	; Initial value stored in BL.

Rep:			; Jump back to this label.
	INC	BL	; Add one to BL.
	JMP	Rep	; Jump back to Rep.

	END		; Program ends.

; -------------------------------------------------------------------------
;
; YOUR TASKS
;
; 7) Rewrite the program to count backwards using DEC BL.
;
; 8) Rewrite the program to count in threes using ADD BL,3.
;
; 9) Rewrite the program to count 1 2 4 8 16 using MUL BL,2
;
; 10) Here is a more difficult task. Count 0 1 1 2 3 5 8 13 21 34 55 98
;    overflow. Here each number is the sum of the previous two. You will
;    need to use two registers and two RAM locations for temporary storage
;    of numbers. If you have never programmed before, this is a real brain
;    teaser. Remember that the result will overflow when it goes above 127.
;
;    This number sequence was first described by Leonardo Fibonacci of Pisa
;    (1170-1230).
;
; -------------------------------------------------------------------------
