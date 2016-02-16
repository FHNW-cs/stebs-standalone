; -------------------------------------------------------------------------
;  LOGIC
; -------------------------------------------------------------------------

; -------------------------------------------------------------------------
;  DIRECT     is where logic is done with registers only.
;  IMMEDIATE  is where logic is done with a register and a numeric
;             value.
; -------------------------------------------------------------------------
;	CLO		; Close unwanted windows
Start:

; -------------------------------------------------------------------------
;  AND   Two ones give a one. Nought otherwise.
; -------------------------------------------------------------------------
	MOV	AL,FF	; Copy FF Hex into AL.
	MOV	BL,55	; Copy 55 Hex into BL.
	AND	AL,BL	; AL becomes AL AND BL.  DIRECT
	ADD	AL,0F	; AL begomes AL AND 0F.  IMMEDIATE

; -------------------------------------------------------------------------
;  OR    Two noughts give a nought. One otherwise.
; -------------------------------------------------------------------------

	MOV	AL,0F	; Copy 0F Hex into AL.
	MOV	BL,55	; Copy 55 Hex into BL.
	OR	AL,BL	; AL becomes AL OR BL.  DIRECT
	OR	AL,F0	; AL begomes AL OR F0.  IMMEDIATE

; -------------------------------------------------------------------------
;  XOR    Equal inputs give a nought. One otherwise.
; -------------------------------------------------------------------------
	MOV	AL,55	; Copy 55 Hex into AL.
	MOV	BL,FF	; Copy F5 Hex into BL.
	XOR	AL,BL	; AL becomes AL XOR BL.  DIRECT
	XOR	AL,BL	; AL becomes AL XOR BL.  DIRECT
	XOR	AL,BL	; AL becomes AL XOR BL.  DIRECT
	XOR	AL,BL	; AL becomes AL XOR BL.  DIRECT
	XOR	AL,0F	; AL becomes AL XOR 0F.  IMMEDIATE
	XOR	AL,0F	; AL becomes AL XOR 0F.  IMMEDIATE
	XOR	AL,0F	; AL becomes AL XOR 0F.  IMMEDIATE
	XOR	AL,0F	; AL becomes AL XOR 0F.  IMMEDIATE

; -------------------------------------------------------------------------
;  NOT   Nought becomes one and one becomes nought.
; -------------------------------------------------------------------------
	MOV	AL,AA	; Copy AA Hex into AL register.
	NOT	AL	; AL becomes NOT AL.  DIRECT
	NOT	AL	; AL becomes NOT AL.  DIRECT
	NOT	AL	; AL becomes NOT AL.  DIRECT
	NOT	AL	; AL becomes NOT AL.  DIRECT

	JMP	Start	; Press escape to stop the program.

; -------------------------------------------------------------------------
	END

; -------------------------------------------------------------------------
