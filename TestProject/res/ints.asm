; -------------------------------------------------------------------------
;  INTERRUPT TESTS.
; -------------------------------------------------------------------------
;  This was used to check that interrupts worked properly
;  and would co-exist with a procedure. It does nothing
;  interesting.
; -------------------------------------------------------------------------

	JMP	Start

	DB	30
	DB	50
	DB	D0

Start:
	INT	02
	INT	03
	INT	04

	JMP	Start

	ORG	30
	NOP
	NOP
	NOP
	NOP
	NOP
	IRET

	ORG	50
	NOP
	NOP
	NOP
	NOP
	NOP
	IRET

	ORG	D0
	NOP
	NOP
	CALL	E0
	NOP
	NOP
	IRET

	ORG	E0
	NOP
	NOP
	INT	02
	INT	03
	NOP
	NOP
	RET

; -------------------------------------------------------------------------
	END
	
; -------------------------------------------------------------------------
