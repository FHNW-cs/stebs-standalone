; -------------------------------------------------------------------------
;  ARITHMETICS.
; -------------------------------------------------------------------------
;  This program is intended to fail with a divide by zero error.
;
;  - Direct Addessing is where arithmetic is done with registers only.
;  - Immediate Addressing is where arithmetic is done with a register
;    and a numeric value.
; -------------------------------------------------------------------------

; Addition
	MOV	AL,02	; Copy 2 Hex into AL.
	MOV	BL,02	; Copy 2 Hex into BL.
	ADD	AL,BL	; AL becomes AL + BL.  DIRECT
	ADD	AL,02	; AL becomes AL + 2.   IMMEDIATE
	INC	AL	; Increment or add one to AL.

; Addition that causes an overflow
	MOV	AL,7F	; Copy 7F Hex into AL.
	MOV	BL,01	; Copy 1 Hex into BL.
	ADD	AL,BL	; AL becomes AL + BL.  DIRECT
			; Answer is wrong because of the overflow.


; Subtraction
	MOV	AL,08	; Copy 8 Hex into AL.
	MOV	BL,05	; Copy 5 Hex into BL.
	SUB	AL,BL	; AL becomes AL - BL.  DIRECT
	SUB	AL,02	; AL becomes AL - 2.   IMMEDIATE
	DEC	AL	; Decrement or subtract one from AL.

; Subtraction that causes the sign bit to be set.
	MOV	AL,05	; Copy 5 Hex into AL.
	MOV	BL,08	; Copy 5 Hex into BL.
	SUB	AL,BL	; AL becomes AL + BL.  DIRECT



; Multiplication
	MOV	AL,02	; Copy 2 Hex into AL.
	MOV	BL,02	; Copy 2 Hex into BL.
	MUL	AL,BL	; AL becomes AL * BL.  DIRECT
	MUL	AL,02	; AL becomes AL * 2.   IMMEDIATE

; Multiplication that causes an overflow
	MOV	AL,55	; Copy 55 Hex into AL.
	MOV	BL,02	; Copy 2 Hex into BL.
	MUL	AL,BL	; AL becomes AL * BL.  DIRECT
			; Answer is wrong because of the overflow.


; MOD  This is the remainder left over after doing a division
	MOV	AL,55	; Copy 55 Hex into AL.
	MOD	AL,10	; AL becomes the remainder after
			;  dividing by 10.  IMMEDIATE

	MOV	AL,76	; Copy 76 Hex into AL.
	MOV	DL,20	; Copy 20 Hex into AL.
	MOD	AL,DL	; AL becomes the remainder after
			;  dividing by DL.  DIRECT



; Division
	MOV	AL,10	; Copy 10 Hex into AL.
	MOV	BL,02	; Copy 2 Hex into BL.
	DIV	AL,BL	; AL becomes AL div BL.  DIRECT
	DIV	AL,02	; AL becomes AL div 2.   IMMEDIATE



; Division by zero ERROR
	MOV	AL,10	; Copy 10 Hex into AL.
	MOV	BL,00	; Copy 0 Hex into BL.
	DIV	AL,BL	; AL becomes AL div BL.  FAILS

	MOV	AL,10	; Copy 10 Hex into AL
	DIV	AL,00	; AL becomes AL div 0.  FAILS

	END		; End of the program.

; -------------------------------------------------------------------------
