; -------------------------------------------------------------------------
;  PROCEDURE CALLS.
;  There is no recursion.
; -------------------------------------------------------------------------

; -------------------------------------------------------------------------
;  Main program
; -------------------------------------------------------------------------
Rep:
	CALL	30	; Call the procedures at these addresses.
	CALL	40
	CALL	50
	CALL	60
	CALL	70
	CALL	80
	CALL	90
	JMP	Rep	; Jump back to the start.

; -------------------------------------------------------------------------
;  A procedure
; -------------------------------------------------------------------------
	ORG	30	; Origin or start address of procedure.
	CALL	40	; Call the procedures at these addresses.
	CALL	50
	CALL	60
	CALL	70
	CALL	80
	CALL	90
	RET		; Return to address saved on stack.

; -------------------------------------------------------------------------
;  A procedure
; -------------------------------------------------------------------------
	ORG	40	; Origin or start address of procedure.
	CALL	50	; Call the procedures at these addresses.
	CALL	60
	CALL	70
	CALL	80
	CALL	90
	RET		; Return to address saved on stack.

; -------------------------------------------------------------------------
;  A procedure
; -------------------------------------------------------------------------
	ORG	50	; Origin or start address of procedure.
	CALL	60	; Call the procedures at these addresses.
	CALL	70
	CALL	80
	CALL	90
	RET		; Return to address saved on stack.

; -------------------------------------------------------------------------
;  A procedure
; -------------------------------------------------------------------------
	ORG	60	; Origin or start address of procedure.
	CALL	70	; Call the procedures at these addresses.
	CALL	80
	CALL	90
	RET		; Return to address saved on stack.

; -------------------------------------------------------------------------
;  A procedure
; -------------------------------------------------------------------------
	ORG	70	; Origin or start address of procedure.
	CALL	80	; Call the procedures at these addresses.
	CALL	90
	RET		; Return to address saved on stack.

; -------------------------------------------------------------------------
;  A procedure
; -------------------------------------------------------------------------
	ORG	80	; Origin or start address of procedure.
	CALL	90	; Call the procedures at these addresses.
	RET		; Return to address saved on stack.

; -------------------------------------------------------------------------
;  A procedure
; -------------------------------------------------------------------------
	ORG	90	; Origin or start address of procedure.
	NOP		; Do Nothing for one clock cycle.
	POP	AL
	PUSH	AL
	POP	BL
	PUSH	BL
	POP	CL
	PUSH	CL
	POP	DL
	PUSH	DL
	NOP
	RET		; Return to address saved on stack.

	END

; -------------------------------------------------------------------------
