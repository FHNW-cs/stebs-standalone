; -------------------------------------------------------------------------
;  TEST PUSH, POP, PUSHF and POPF
; -------------------------------------------------------------------------
;  This program fails in an unpredictable way.
;  The POPF command sets the CPU flags in an unplanned way.
;  The I flag is soon set, which causes the hardware interrupt
;  02 to start working. The address in RAM location 02 is zero.
;  This causes the program to restart without clearing up the
;  stack. The program works correctly and then fails unexpectedly
;  when the I flag is set.
; -------------------------------------------------------------------------

;	CLO		; Close unwanted windows.
Start:

	MOV	AL,10	; Initialise counter.
Rep1:
	PUSH	AL	; Test push.
	PUSH	AL	; Test push.
	POP	BL	; Test pop.
	PUSHF		; Test pushf.
	DEC	AL	; Decrement counter.
	JNZ	Rep1	; Repeat until al = 0.

; -------------------------------------------------------------------------


	MOV	AL,10	; Initialise counter.
Rep2:
	POP	CL	; Test pop.
	POPF		; Test popf.
	DEC	AL	; Decrement counter.
	JNZ	Rep2	; Repeat until al = 0.

	JMP	Start	; Press ESCAPE to stop the program.

; -------------------------------------------------------------------------
	END

; -------------------------------------------------------------------------
