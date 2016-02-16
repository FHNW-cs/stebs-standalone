; -------------------------------------------------------------------------
;  A PROGRAM TO TURN THE HEATER ON AND OFF.
; -------------------------------------------------------------------------
;  The system starts with the heater off allowing it to cool.
;  The heater is then turned on until about 22 degrees is reached.
;  This temperature is maintained for a period.
;  The temperature is then forced up for a period before being
;  returned to 22 degrees.
;  The above is then repeated.
; -------------------------------------------------------------------------
	MOV	BL,80
	PUSH	BL
Main:
			; Heat/cool system.
	POP	AL
	XOR	AL,80
	PUSH	AL
	OUT	03	; Send data to heater.

	MOV	AL,30	; 30 time units delay.
	CALL	90	; Call time delay procedure.

			; Bang Bang Controller.
			; Run this loop 20 times.
	MOV	CL,20	; Use CL as a counter.
Bang:
	DEC	CL
	JZ	Main
	IN	03
	CMP	AL,80
	JZ	TurnOn
	CMP	AL,81
	JZ	TurnOff
	CMP	AL,00
	JZ	TurnOn

TurnOff:
	MOV	AL,00
	OUT	03
	JMP	Bang

TurnOn:
	MOV	AL,80
	OUT	03
	JMP	Bang


; -------------------------------------------------------------------------
	ORG	90
	
	PUSH	AL	; Time Delay. Determined by the value passed in AL.
Delay:
	DEC	AL
	JNZ	Delay

	POP	AL
	RET

; -------------------------------------------------------------------------
	END
	
; -------------------------------------------------------------------------
