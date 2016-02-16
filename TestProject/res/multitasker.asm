; ----------------------------------------------------------------------------
;  TIME SLICE MULTITASKER PROGRAM.
; ----------------------------------------------------------------------------
;  This program spins the stepper motor continuously in one task.
;  The traffic lights are controlled in a second task.
;  Task three repeatedly writes to the visual display unit VDU.
;  Task switching is enforced by the multitasker program on abround robin
;  basis.
;  The task running is in active state, all other tasks are in ready state.
;
;  Settings:
;  Configuration: Hardware Timer Interval = 6 secsonds.
;                 Uncheck 'Show only one peripheral at a time'.
;  Program Speed: Fastest.
;
;  Note:
;  Do NOT use the stack in the tasks.
; ----------------------------------------------------------------------------
	JMP	Start

; ----------------------------------------------------------------------------
	ORG	02
	DB	06	; Vector to hardware interrupt routine (multitasker)
; ----------------------------------------------------------------------------

Start:
	STI		; Enable hardware interrupts.
	JMP	Stepper

; ----------------------------------------------------------------------------
;  Multitasker
;  The Multitasker (MT)is invoked by answering the interrupt request set by
;  the periodic hardware interrupt.
;  MT saves the context (= AL, BL, CL, DL, SR and IP) of the active task into
;  the context table of this task.
;  The next task (cp. ContextTableEntries) is activated. Its context is re-
;  stored. Control is then passed to this new task (IRET).
; ----------------------------------------------------------------------------
	ORG	06
Tasker:
	CLI		; Inhibit hardware interrupts.
			; Switch Contexts.
	PUSHF		; Save SR to stack.
	PUSH	AL	;      AL
	PUSH	BL	;      BL
	PUSH	CL	;      CL
	PUSH	DL	;      DL
	
	MOV	DL,[60]	; Get CTEPointer
	MOV	DL,[DL]	; Get context table address of last active task.
	MOV	CL,06	; Init loop counter.
SaveContext:
	POP	AL	; Get saved values from the stack
	MOV	[DL],AL	; Save values into the context table.
	INC	DL
	DEC	CL
	JNZ	SaveContext
	
	MOV	AL,[60]	; Get CTEPointer.
	INC	AL
	MOV	DL,[AL]	; Get next context table address.
	CMP	DL,00	; Delimiter?
	JNZ	Continue
	MOV	AL,61	; Delimiter reached, wrap.
	MOV	DL,[61]	; Get first context table address.
Continue:
	MOV	[60],AL	; Update CTEPointer (active task).
	
	ADD	DL,05
	MOV	CL,06	; Init loop counter.
RestoreContext:
	MOV	AL,[DL]	; Get IP.
	PUSH	AL	; Put values onto the stack.
	DEC	DL
	DEC	CL
	JNZ	RestoreContext
	
	POP	DL	; Restore DL from stack.
	POP	CL	;         CL
	POP	BL	;         BL
	POP	AL	;         AL
	POPF		;         SR
	
	STI		; Enable hardware interrupts again.
	IRET		; Perform task switch, invoke active task.

	ORG	60
CTEPointer:
	DB	61	; Pointer to entry of currently active task (var)
ContextTableEntries:
	DB	70	; Context Table Address of Task One (const)
	DB	80	; Context Table Address of Task Two (const)
	DB	D0	; Context Table Address of Task Three (const)
	DB	00	; Delimiter: eot
; ----------------------------------------------------------------------------

; ----------------------------------------------------------------------------
; Task One (Stepper Motor)
; Stepper motor data is generated in rotating a bit pattern which in turn is
; output to the motor.
; ----------------------------------------------------------------------------
	ORG	70
ContextTbl1:			; Context table of Stepper Motor Task
	DB	00	; Saved AL
	DB	00	;       BL
	DB	00	;       CL
	DB	00	;       DL
	DB	00	;       SR (I Bit always cleared/set by MT).
	DB	76	;       IP, initially points to task start address.

Stepper:
	MOV	AL,33	; Initialize stepper pattern bits.
NextStep:
	OUT	05	; Move stepper motor.
	ROR	AL	; Rotate pattern bits.
	JMP	NextStep
; ----------------------------------------------------------------------------

; ----------------------------------------------------------------------------
; Task Two (Traffic Lights)
; Traffic Light data is consecutively read from a table and sent to the
; lights.
; ----------------------------------------------------------------------------
	ORG	80
ContextTbl2:			; Context table of Traffic Light Task
	DB	00	; Saved AL
	DB	00	;       BL
	DB	00	;       CL
	DB	00	;       DL
	DB	00	;       SR (I Bit always cleared/set by MT).
	DB	86	;       IP, initially points to task start address.

Lights:
	MOV	BL,[A7]	; Initialize TLTable pointer.
	MOV	AL,[BL]	; Get traffic light data.
	OUT	01	; Change traffic lights.
	CMP	AL,58	; End of table?
	JZ	Reset	; Reset the table pointer if last entry is reached
	INC	BL	; Point to next table entry.
	MOV	[A7],BL	; Save the table pointer.
	JMP	Lights
Reset:
	MOV	BL,A8	; Reload pointer to the table start address.
	MOV	[A7],BL	; Save the table pointer.
	JMP	Lights

	ORG	A7
TLTPointer:
	DB	A8	; Pointer to TLTable, initially set to start (var)
TLTable:
	DB	84	; Red           Green             (const)
	DB	C8	; Red+Amber     Amber             (const)
	DB	30	; Green         Red               (const)
	DB	58	; Amber         Red+Amber         (const)
; ----------------------------------------------------------------------------

; ----------------------------------------------------------------------------
; Task Three (VDU)
; Repeatedly write the string "HELLO" to the Visual Display Unit and erase
; it again.
; ----------------------------------------------------------------------------
	ORG	D0
ContextTbl3:			; Context table of VDU Task
	DB	00	; Saved AL
	DB	00	;       BL
	DB	00	;       CL
	DB	00	;       DL
	DB	00	;       SR (I Bit always cleared/set by MT).
	DB	D6	;       IP, initially points to task start address.

VDU:
	MOV	DL,C8	; Initialize char source address.
	MOV	CL,05	; Initialize loop counter with string length.
Display:
	MOV	AL,[DL]	; Get char of string.
	SUB	DL,08	; Calculate char destination address.
	MOV	[DL],AL	; Display char.
	ADD	DL,09	; Get next char source address.
	DEC	CL	; Decerement loop counter
	JNZ	Display	; Done?

	MOV	AL,20	; Set ASCII Blank char.
	MOV	DL,C0	; Initialize char destination address.
	MOV	CL,05	; Initialize loop counter with string length.
Erase:
	MOV	[DL],AL	; Display blank char.
	INC	DL	; Point to next destination address.
	DEC	CL	; Decerement loop counter.
	JNZ	Erase	; Done?
	JMP	VDU     ;

	ORG	C8
	
VDUString:
	DB	"HELLO"
; ----------------------------------------------------------------------------

	END
; ----------------------------------------------------------------------------
