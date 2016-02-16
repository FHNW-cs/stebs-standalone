; -------------------------------------------------------------------------
;  LIFT TEST.
; -------------------------------------------------------------------------
	MOV	AL,00
	OUT	06
Loop:
	IN	06	; Test for lift at top.
	PUSH	AL
	POP	BL
	AND	BL,05	; Test for MotorUp AND TopSwitch.
	CMP	BL,05
	JZ	StopTop

	IN	06	; Test lift at bottom.
	PUSH	AL
	POP	BL
	AND	BL,0A	; Test for MotorDown AND BottomSwitch.
	CMP	BL,0A
	JZ	StopBot

	PUSH	AL
	POP	BL
	AND	BL,10	; test for the Call Down button.
	JNZ	CallDown

	PUSH	AL
	POP	BL
	AND	BL,20	; test for the Call Up button.
	JNZ	CallUp

	JMP	Loop

StopTop:
	AND	AL,DE	; Switch off motor bits and call button.
	OUT	06
	JMP	Loop

StopBot:
	AND	AL,ED	; Switch off motor bits and call button.
	OUT	06
	JMP	Loop

CallDown:
	IN	06	; Is lift already down.
	AND	AL,08
	CMP	AL,08
	JZ	errDown ; Don't turn on the motor.
	MOV	AL,12
	OUT	06
	JMP	Loop
errDown:
	MOV	AL,00
	OUT	06
	JMP	Loop

CallUp:
	IN	06	; Is lift already up.
	AND	AL,04
	CMP	AL,04
	JZ	errUp	; Don't turn on the motor.
	MOV	AL,21
	OUT	06
	JMP	Loop
errUp:
	MOV	AL,00
	OUT	06
	JMP	Loop

; -------------------------------------------------------------------------
	END

; -------------------------------------------------------------------------
