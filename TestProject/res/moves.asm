; -------------------------------------------------------------------------
;  A PROGRAM TO DEMONSTRATE  MOV  COMMANDS.
; -------------------------------------------------------------------------

; -------------------------------------------------------------------------
;  Register to register moves:
;  These are NOT supported by this simulator
;  MOV     AL,BL   This is commented out! It wouldn't work.
;  To do this use PUSH BL followed by POP AL.
; -------------------------------------------------------------------------
;	CLO		; Close unwanted windows.


; -------------------------------------------------------------------------
;  Immediate moves:
;  A hexadecimal number is copied into a register.
; -------------------------------------------------------------------------
	MOV	AL,15	; Copy 15 HEX into the AL register.
	MOV	BL,40	; Copy 40 HEX into the BL register.
	MOV	CL,50	; Copy 50 HEX into the CL register.
	MOV	DL,60	; Copy 60 HEX into the DL register.
Foo:
	INC	AL	; Increment AL for no particular reason.


; -------------------------------------------------------------------------
;  Indirect moves:
;  A value is moved to or from RAM.
;  The ram address is given as a number like [22] in
;  square brackets.
; -------------------------------------------------------------------------
	MOV	[A0],AL	; Copy value in AL to RAM location 40.
	MOV	BL,[40]	; Copy value in RAM location A0 into BL.


; -------------------------------------------------------------------------
;  Register indirect moves:
;  Copy a value from RAM to a register or
;  copy a value from a register to RAM.
;  The RAM address is contained in a second register
;  enclosed in square brackets like this [CL].
; -------------------------------------------------------------------------
	MOV	[CL],AL	; Copy the value in AL to the RAM
			;  location that CL points to.
	MOV	BL,[CL]	; Copy the RAM location that CL points
			;  to into the BL register.

	JMP	Foo	; Press escape to stop the program.

; -------------------------------------------------------------------------
	END
; -------------------------------------------------------------------------
