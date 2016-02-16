; -------------------------------------------------------------------------
;  A PROGRAM TO DEMONSTRATE MOV COMMANDS. MOV IS SHORT FOR MOVE.
; -------------------------------------------------------------------------

;	CLO		; Close unwanted windows.

;	IMMEDIATE MOVES
	MOV	AL,15	; Copy 15 HEX into the AL register.
	MOV	BL,40	; Copy 40 HEX into the BL register.
	MOV	CL,50	; Copy 50 HEX into the CL register.
	MOV	DL,60	; Copy 60 HEX into the DL register.
Foo:
	INC	AL	; Increment AL for no particular reason.

;	INDIRECT MOVES
	MOV	[A0],AL	; Copy value in AL to RAM location [A0].
	MOV	BL,[40]	; Copy value in RAM location [40] into BL.

;	REGISTER INDIRECT MOVES
	MOV	[CL],AL	; Copy the value in AL to the RAM.
			; location that CL points to.
	MOV	BL,[CL]	; Copy the RAM location that CL points
			; to into the BL register.
	JMP	Foo	; PRESS ESCAPE TO STOP THE PROGRAM.

	END

; -------------------------------------------------------------------------
;
; YOUR TASK
;
; 6)  Look up the ASCII codes of H, E, L, L and O and copy these
;    values to memory locations C0, C1, C2, C3 and C4. This is a
;    simple and somewhat crude way to display text on a memory
;    mapped display.
;
; -------------------------------------------------------------------------
