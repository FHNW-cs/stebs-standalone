; -------------------------------------------------------------------------
;  TEST  JO  AND  JNO.
; -------------------------------------------------------------------------
;  An overflow occurs when a calculation gives a result
;  that is too big to be stored in a register. This
;  eight bit simulator can hold integer numbers between
;  -128 and +127. Any number outside this range causes an
;  overflow and the 'O' flag is set in the status register (SR).
;
;  The JO and JNO commands work in conjunction with the
;  'O' overflow flag. JO causes the program to jump if the 'O'
;  flag is set. JNO causes the program to jump if the 'O' flag
;  is not set.
; -------------------------------------------------------------------------


; -------------------------------------------------------------------------
;  A counting loop that terminates when AL overflows.
;  This loop exercises the JO (jump overflow) command.
; -------------------------------------------------------------------------
Start:
	MOV	AL,75	; Initialise AL counter to 75 HEX.
			; 7F + 1 causes the overflow.
Foo:
	INC	AL	; Increment AL.
	JO	Bar	; Jump out of loop if overflowed.
	JMP	Foo	; Jump back and repeat the loop.


; -------------------------------------------------------------------------
;  A counting loop that terminates when AL overflows.
;  This loop exercises the JNO (jump not overflow) command.
; -------------------------------------------------------------------------
Bar:
	MOV	AL,75	; Initialise AL counter to 75 HEX.
Puppy:
	INC	AL	; Increment AL.
	JNO	Puppy	; Jump back and repeat the loop
			;  if the overflow bit is not set.

	JMP	Start	; Press escape to stop the program.

; -------------------------------------------------------------------------
	END

; -------------------------------------------------------------------------
