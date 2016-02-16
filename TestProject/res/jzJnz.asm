; -------------------------------------------------------------------------
;  TEST  JZ  AND  JNZ.
; -------------------------------------------------------------------------
;  The 'Z' or zero flag in the status register (SR) is set
;  whenever a calculation gives a zero result. The JZ
;  and JNZ commands work in conjunction with the 'Z' flag.
;  JZ causes the program to jump if the 'Z' flag is set.
;  JNZ causes the program to jump if the 'Z' flag is not set.
; -------------------------------------------------------------------------

; -------------------------------------------------------------------------
;  A count down loop that terminates when AL becomes zero.
;  This loop exercises the JZ (jump zero) command.
; -------------------------------------------------------------------------
Start:
	MOV	AL,05	; Initialise AL register counter to 5.
Foo:
	DEC	AL	; Decrement AL.
	JZ	Bar	; Jump out of loop if zero bit is set.
	JMP	Foo	; Jump back and repeat the loop.


; -------------------------------------------------------------------------
;  A count down loop that terminates when AL becomes zero.
;  This loop exercises the JNZ (jump not zero) command.
; -------------------------------------------------------------------------
Bar:
	MOV	AL,05	; Initialise AL register counter to 5.
Puppy:
	DEC	AL	; Decrement AL.
	JNZ	Puppy	; Jump back and repeat the loop
			;  if the zero bit is not set.

	JMP	Start	; Press escape to stop the program.

; -------------------------------------------------------------------------
	END

; -------------------------------------------------------------------------
