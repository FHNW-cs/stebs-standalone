; -------------------------------------------------------------------------
;  MAKING COMPARISONS USING  CMP.
; -------------------------------------------------------------------------

; -------------------------------------------------------------------------
;  Compare registers.
;  Stop repeating when registers are equal.
; -------------------------------------------------------------------------
Start:
	MOV	CL,00	; Initialise counter.
	MOV	DL,10	; Initialise test value.
Rept1:
	INC	CL	; Increment CL.
	CMP	CL,DL	; Compare CL with DL.
			; 'Z' flag is set if they are equal.
	JNZ	Rept1	; Repeat if 'Z' flag is not set.

; -------------------------------------------------------------------------
;  Compare registers.
;  Stop repeating when CL < DL.
; -------------------------------------------------------------------------
	MOV	CL,20	; Initialize counter.
	MOV	DL,10	; InitialiZe test value.
Rept2:
	DEC	CL	; Decrement CL.
	CMP	CL,DL	; Compare CL with DL.
			; 'S' flag is set if CL < DL.
	JNS	Rept2	; Repeat if 'S' flag is not set.

; -------------------------------------------------------------------------
;  Compare immediate (register with a number).
;  Stop repeating when DL == 10.
; -------------------------------------------------------------------------
	MOV	DL,00	; Initialize counter.
Rept3:
	INC	DL	; Increment DL.
	CMP	DL,10	; Compare DL with 10.
			; 'Z' flag is set if DL == 10.
	JNZ	Rept3	; Repeat if 'Z' flag is not set.

; -------------------------------------------------------------------------
;  Compare immediate (register with a number).
;  Stop repeating when DL < 10.
; -------------------------------------------------------------------------
	MOV	DL,20	; Initialize counter.
Rept4:
	DEC	DL	; Decrement DL.
	CMP	DL,10	; Compare DL with 10.
			; 'S' flag is set if DL < 10.
	JNS	Rept4	; Repeat if 'S' flag is not set.

; -------------------------------------------------------------------------
;  Compare indirect (register with a RAM location).
;  Stop repeating when DL == RAM location 50.
; -------------------------------------------------------------------------
	MOV	DL,00	; Initialize counter.
	MOV	AL,10	; Test value.
	MOV	[50],AL ; Place test value into RAM 50.
Rept5:
	INC	DL	; Increment DL.
	CMP	DL,[50] ; Compare dl with RAM location 50.
			; 'Z' flag is set if DL == 10.
	JNZ	Rept5	; Repeat if 'Z' flag is not set.

; -------------------------------------------------------------------------
;  Compare indirect (register with a RAM location).
;  Stop repeating when DL < RAM location 50.
; -------------------------------------------------------------------------
	MOV	DL,20	; Initialize counter.
	MOV	AL,10	; Test value.
	MOV	[50],AL ; Place test value into ram 50.
Rept6:
	DEC	DL	; Decrement DL.
	CMP	DL,[50] ; Compare DL with 10.
			; 'S' flag is set if DL < 10.
	JNS	Rept6	; Repeat if 'S' flag is not set.

	JMP	Start

; -------------------------------------------------------------------------
	END

; -------------------------------------------------------------------------
