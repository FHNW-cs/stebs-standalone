; -------------------------------------------------------------------------
;  DEMO
; -------------------------------------------------------------------------
;  A demonstration program that exercises many instructions
;  and most of the peripherals available in the simulator.
;  CALL, RET, INT and IRET are not used because these
;  commands are not available in the shareware version.
; -------------------------------------------------------------------------

; -------------------------------------------------------------------------
;  Data table
; -------------------------------------------------------------------------
;	CLO		; Close all peripheral windows.

	JMP	HERE	; Skip past data section.

    ; Load text string "HELLO WORLD!" into RAM.
	DB	'H'
    DB	'E'
    DB	'L'
    DB	'L'
    DB	'O'
    DB	20
    DB	'W'
    DB	'O'
    DB	'R'
    DB	'L'
    DB	'D'
    DB	'!'
    
	DB	00	; Null terminated.


; -------------------------------------------------------------------------
;  Display text
; -------------------------------------------------------------------------
Here:
	MOV	CL,C0	; Video ram base address.
	MOV	BL,03	; Offset of text string.

Start:			; Text output to vdu.
	MOV	AL,[BL]	; Text pointer into AL.
	CMP	AL,00	; At end yet?.
	JZ	End1	; Jump out of loop.
	MOV	[CL],AL	; AL into video memory.
	INC	CL	; Next video location.
	INC	BL	; Next text character.
	JMP	Start	; Not there yet.
End1:


; -------------------------------------------------------------------------
;  Traffic lights
; -------------------------------------------------------------------------
	MOV	BL,0C	; Flash the traffic lights.
	MOV	AL,AA	; 1010 1010b
Rep1:
	OUT	01	; Lights are on port one.
	NOT	AL	; 0101 0101b
	DEC	BL	; Count down.
	JNZ	Rep1	; Jump out of loop on zero.

			; KLUDGE BECAUSE MAX JUMP IS -128.
	JMP	Skipovr	; Jump forward past jump back.
Middle:
	JMP	Here	; Jump back rest of the way.
Skipovr:


; -------------------------------------------------------------------------
;  Seven segment display
; -------------------------------------------------------------------------
	MOV	BL,0C	; Flash seven segment displays.
Rep2:
	MOV	AL,FF	; 1111 1111b
	OUT	02	; Lights are on port one.
	MOV	AL,FE	; 1111 1110b
	OUT	02	; LSB used for multiplexing.

	MOV	AL,01	; 0000 0001b
	OUT	02
	MOV	AL,00	; 0000 0000b
	OUT	02

	DEC	BL	; Count down.
	JNZ	Rep2	; Jump out of loop on zero.


; -------------------------------------------------------------------------
;  Heater and thermostat
; -------------------------------------------------------------------------
	IN	03	; Input from thermostat on port 3.
	CMP	AL,01	; Is it too warm.
	JZ	Off	; If no then jump to OFF.
	MOV	AL,80	; Use MSB to turn heater on.
	OUT	03	; Send 1000 0000b to port 3.
	JMP	Skip2	; Jump past heater-off code.
Off:
	MOV	AL,00	; Turn of heater with 0000 0000b.
	OUT	03	; Send 0000 0000b to port 3.
Skip2:
	MOV	BL,20	; Time Delay.
On:
	DEC	BL	; BL counts down for time delay.
	JNZ	On	; Jump out of loop on zero.


; -------------------------------------------------------------------------
;  Snake in the maze
; -------------------------------------------------------------------------
	MOV	AL,FF	; Maze reset.
	OUT	04	; Snake is on port 4.
	MOV	BL,0A	; Count down start value.
	MOV	AL,4F	; 4 means down. F means 15..
Rep5:
	OUT	04	; Send data to snake.
	DEC	BL	; Count down.
	JNZ	Rep5	; Jump out of loop on zero.


; -------------------------------------------------------------------------
;  Spin the stepper motor
; -------------------------------------------------------------------------
	MOV	BL,20	; Count down start value.
	MOV	AL,11	; 0001 0001b
Rep6:
	OUT	05	; Stepper motor is on port 5.
	ROL	AL	; Rotate bits left.
	DEC	BL	; Count down.
	JNZ	Rep6	; Jump out of loop on zero.


; -------------------------------------------------------------------------
;  Clear the vdu screen
; -------------------------------------------------------------------------
	MOV	CL,C0	; Screen base address.
	MOV	BL,0C	; Clear row on screen.
	MOV	AL,20	; ASCII space.
Rep7:
	MOV	[CL],AL	; Data to video RAM.
	INC	CL	; Next RAM location.
	DEC	BL	; Count down.
	JNZ	Rep7	; Jump out of loop on zero.
	JMP	Middle	; Jump towards start.
; -------------------------------------------------------------------------

	END
; -------------------------------------------------------------------------
