; -------------------------------------------------------------------------
;  HARDWARE INTERRUPT
; -------------------------------------------------------------------------
;  Here is a solution to an exercise for a more advanced student.
;
;  This program increments the digits in the seven segment
;  display. The left hand digit counts from 0 to 9 repeatedly
;  at a rate determined by the CPU clock speed.
;
;  The right hand digit increments from 0 to 9 repeatedly at
;  a rate determined by the simulated hardware interrupt timer
;  ticks. The two digits should function independently.
;
;  This exercise requires the correct use of PUSH, POP, PUSHF
;  and POPF because the main program might be interrupted at
;  any time. If the interrupt code does not tidy up after
;  itself, chaos would soon result.
;
;  Things start go wrong if the interrupt is called again before
;  it has had time to complete. It still works but eventually
;  the stack will overwrite the program.
; -------------------------------------------------------------------------

	JMP	Start	; Jump past the data table Etc.

	DB	30	; Hardware Timer Interrupt Vector
			;  points to code at address 30.

	DB	FA	; Data table starts at address 03.
	DB	0A
	DB	B6
	DB	9E
	DB	4E
	DB	DC
	DB	FC
	DB	8A
	DB	FE
	DB	CE	; Last data entry.

	DB	03	; This RAM Location points to the data
		  	;  table. The interrupt code uses this
		  	;  address to keep track of its progress
		  	;  through the data table.

; -------------------------------------------------------------------------
;  MAIN PROGRAM
; -------------------------------------------------------------------------

Start:
	STI		; Enable Hardware Timer Interrupts.
	MOV	BL,03	; BL contains data table start address.
Rep:
	MOV	AL,[BL]	; Move data from table into AL.
	OUT	02	; Send data to display.
	CMP	AL,CE	; Test for the table end value.
	JZ	Start	; If table end, jump to Start.
	INC	BL	; Next data table position.
	JMP	Rep	  ; Jump back and do the next digit.

; -------------------------------------------------------------------------
;  INTERRUPT HANDLER. The code below runs on each clock tick.
; -------------------------------------------------------------------------
	ORG	30	; Code starts at address 30.

	PUSH	AL	; Save AL register onto the stack.
	PUSH	BL	; Save BL register onto the stack.
	PUSHF		  ; Save CPU flags onto the stack.

	MOV	BL,[0D]	; RAM location 0D contains pointer to data table.
	MOV	AL,[BL]	; Fetch data from table.
	OR	AL,01	; Set LSB to One. This triggers the other display.
	OUT	02	; Send data to the display.
	CMP	AL,CF	; Test for the end value.
	JZ	Reset	; If at table end, reset the table index
			;  stored in RAM.
	INC	BL	; Next table position.
	MOV	[0D],BL	; Save this table position into RAM.
	JMP	Done	; Skip past the pointer reset code.

Reset:
	MOV	BL,03	; 03 is the address of table start.
	MOV	[0D],BL	; Save this pointer into RAM 0E.

Done:			; All finished. Tidy up and return.
	POPF		; Restore CPU flags from the stack.
	POP	BL	; Restore BL register from stack.
	POP	AL	; Restore AL register from stack.
	IRET		; Interrupt return.

; -------------------------------------------------------------------------
	END

; -------------------------------------------------------------------------
