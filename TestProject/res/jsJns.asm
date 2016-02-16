; -------------------------------------------------------------------------
;  TEST  JS  AND  JNS.
; -------------------------------------------------------------------------
;  The 'S' or sign flag in the status register (SR) is set
;  whenever a calculation gives a negative result. The JS
;  and JNS commands work in conjunction with the 'S' flag.
;  JS causes the program to jump if the 'S' flag is set.
;  JNS causes the program to jump if the 'S' flag is not set.
; -------------------------------------------------------------------------

; -------------------------------------------------------------------------
;  A count down loop that terminates when AL becomes negative.
;  This loop exercises the JS (jump sign) command.
; -------------------------------------------------------------------------
Start:
	MOV	AL,05	; Initialise AL register counter to 5.
Foo:
	DEC	AL	; Decrement AL.
	JS	Bar	; Jump out of loop if sign bit is set.
	JMP	Foo	; Jump back and repeat the loop.


; -------------------------------------------------------------------------
;  A count down loop that terminates when AL becomes negative.
;  This loop exercises the JNS (jump not sign) command.
; -------------------------------------------------------------------------
Bar:
	MOV	AL,05	; Initialise AL register counter to 5.
Puppy:
	DEC	AL	; Decrement AL.
	JNS	Puppy	; Jump back and repeat the loop
			;  if the sign bit is not set.

	JMP	Start	; Press escape to stop the program.

; -------------------------------------------------------------------------
	END

; -------------------------------------------------------------------------
