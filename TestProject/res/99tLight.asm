; -------------------------------------------------------------------------
;  CONTROL THE TRAFFIC LIGHTS.
; -------------------------------------------------------------------------
;  This simple example does not step the lights realistically.
;  It lacks a time delay facility.
;  It would be better to use a data table.
;  It works otherwise! 
;  Solving this properly is one of the learning tasks.
; -------------------------------------------------------------------------
;	CLO		; Close unwanted windows.

Rep:
	MOV	AL,84	; Red		Green
	OUT	01	; Output to port 01 (traffic lights).
	MOV	AL,C8	; Red+Amber	Amber
	OUT	01
	MOV	AL,30	; Green		Red
	OUT	01
	MOV	AL,58	; Amber		Red+Amber
	OUT	01
	JMP	Rep

	END
	
; -------------------------------------------------------------------------
