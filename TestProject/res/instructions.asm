; -------------------------------------------------------------------------
;  TEST INSTRUCTIONS IN THE INSTRUCTION SET.
; -------------------------------------------------------------------------

; -------------------------------------------------------------------------
	ORG	A0
	DB	"HELLO"
	DB	00

; -------------------------------------------------------------------------
	ORG	48
	DB	"world"
	DB	00

; -------------------------------------------------------------------------
	ORG	00
	MOV	CL,A0
	MOV	BL,C0
Rep:
	MOV	AL,[CL]
	CMP	AL,00
	JZ	Foo
	MOV	[BL],AL
	INC	BL
	INC	CL
	JMP	Rep
Foo:
	MOV	CL,[A0]
	MOV	BL,C6
Poo:
	MOV	AL,[CL]
	CMP	AL,[A5]
	JZ	Azz
	MOV	[BL],AL
	INC	BL
	INC	CL
	JMP	Poo
Azz:
	MOV	AL,5A
	MOV	[C0],AL

	MOV	DL,78
	MOV	AL,8F
Ree:
	CALL    62
	Add	DL,01
	CMP	DL,AL
	JNZ	Ree
	JMP	Laa

; -------------------------------------------------------------------------
	ORG	4E
Laa:
	MOV	BL,10
	MOV	AL,88
Goo:
	OUT	05
	ROR	AL
	SUB	BL,01
	CMP	BL,f0
	JNZ	Goo
	JMP	Here

; -------------------------------------------------------------------------
;  Sent a byte to the traffic lights.
; -------------------------------------------------------------------------
	ORG	62
	PUSH    AL
	PUSH    DL
	POP	AL
	OUT	01
	POP	AL
	NOP
	RET

Here:
	INT	06
	IN	00
	MOV	CL,F0	; 1111 0000b
	MOV	DL,FF	; 1111 1111b

	AND	CL,DL	; 1111 0000b
	OR	CL,DL	; 1111 1111b
	XOR	AL,BL
	NOT	BL
	ROL	AL
	SHL	CL
	SHR	DL
	AND	AL,0F
	OR	CL,F0
	XOR	AL,AA

	HALT

; -------------------------------------------------------------------------
;  Test INT.
; -------------------------------------------------------------------------
	ORG	D3

	PUSHF
	PUSH    AL

	MOV	AL,08
	MOV	BL,02
	SUB	AL,BL
	MUL	AL,BL
	DIV	AL,BL
	MOD	AL,BL

	DEC	AL
	ADD	AL,11
	SUB	AL,02
	MUL	AL,03
	DIV	AL,06
	MOD	AL,05

	POP	AL
	POPF

	IRET

; -------------------------------------------------------------------------
	END

; -------------------------------------------------------------------------
