; -------------------------------------------------------------------------
;  USING HARDWARE INTERRUPTS.
;  This program spins the stepper motor continuously and steps
;  the traffic lights on each hardware interrupt.
;  Uncheck the "Show only one peripheral at a time" box to
;  enable both displays to appear simultaneously.
; -------------------------------------------------------------------------
	JMP	Start	; Jump past table of interrupt vectors.

	ORG	02	; Hardware interrupt vector
	DB	50	; Vector pointing to address 50.

; -------------------------------------------------------------------------
Start:
	STI		; Set I flag. Enable hardware interrupts.
	MOV	AL,11
Rep:
	OUT	05	; Stepper motor.
	ROR	AL	; Rotate bits in AL right.
	JMP	Rep
	JMP	Start

; -------------------------------------------------------------------------
	ORG	40	; TABLE
	DB	84	; Red		Green
	DB	C8	; Red+Amber	Amber
	DB	30	; Green		Red
	DB	58	; Amber		Red+Amber

	ORG	48	; Table Pointer
	DB	40	; Initially point to TABLE start.

; -------------------------------------------------------------------------
	ORG	50	; interrupt routine
	PUSH	AL	; Save AL onto the stack.
	PUSH	BL	; Save BL onto the stack.
	PUSHF		; Save flags onto the stack.

	MOV	BL,[48]	; BL now points to the data table.
	MOV	AL,[BL]	; Data from table goes into AL.
	OUT	01	; Send AL data to traffic lights.
	CMP	AL,58	; Last entry in the table.
	JZ	Reset	; If last entry then reset pointer.
	INC	BL	; BL points to next table entry.
	MOV	[48],BL	; Save table pointer.
	JMP	Stop

Reset:
	MOV	BL,40	; Pointer to TABLE start address.
	MOV	[48],BL	; Save table pointer.
 
Stop:
	POPF		; Restore flags to their previous value.
	POP	BL	; Restore BL to its previous value.
	POP	AL	; Restore AL to its previous value.
	IRET

; -------------------------------------------------------------------------
	END

; -------------------------------------------------------------------------
;
; YOUR TASKS
;
; 28) Write a program that controls the heater and thermostat
;    whilst at the same time counting from 0 to 9 repeatedly,
;    displaying the result on one of the seven segment displays.
;    If you want a harder challenge, count from 0 to 99
;    repeatedly using both displays. Use the simulated hardware
;    interrupt to control the heater and thermostat.
;
; 29) A fiendish problem. Solve the Tower of Hanoi problem whilst
;    steering the snake through the maze. Use the text characters
;    A, B, C etc. to represent the disks. Use three of the four
;    rows on the simulated screen to represent the pillars.
;
; -------------------------------------------------------------------------
