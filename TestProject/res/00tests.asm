; -------------------------------------------------------------------------
;  CONTROL THE TRAFFIC LIGHTS.
; -------------------------------------------------------------------------

;	CLO		; Close unwanted windows.
Start:
			; Turn off all the traffic lights.
	MOV	AL,FF	; Copy 0000'0000bin into the AL register.
	PUSH	AL	; Copy AL into Stack register.
	POP	CL	; Copy CL From Stack
	IN	00	; Input from Keyboard (ASCI-Wert)
	IN	06	; Input from Lift

			; Turn on all the traffic lights.
	MOV	AL,80	; Copy 1111'1100bin into the AL register.
	OUT	03	; Send AL to Port One (the traffic lights).

	JMP	Start	; Jump back to the start.

	END		; Program ends.

; -------------------------------------------------------------------------
;
; YOUR TASK
;
; 5)  Use the help page on hexadecimal and binary numbers. Work
;    out what hexadecimal numbers will activate the correct
;    traffic lights. Modify the program to step the lights
;    through a realistic sequence.
;
; -------------------------------------------------------------------------
