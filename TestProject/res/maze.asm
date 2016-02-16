; -------------------------------------------------------------------------
;  MAZE DEMO.
; -------------------------------------------------------------------------

Start:
	MOV	AL,FF
	OUT	04
	MOV	CL,30
	MOV	DL,40
More:
	MOV	AL,[CL]
	MOV	BL,[DL]
	CMP	AL,00
	JZ	Start
Rep:
	OUT	04
	DEC	BL
	JNZ	Rep
	INC	CL
	INC	DL
	JMP	More


	ORG	30	; DATA TABLE TO RUN MAZE: Steering
	DB	4F	; Down 0B
	DB	1D	; Right 3
	DB	8B	; Up 8
	DB	1D	; Right 3
	DB	4E	; Down 6
	DB	1E	; Right 3
	DB	8E	; Up 9
	DB	2F	; Left 5
	DB	8C	; Up 3
	DB	1F	; Right 8
	DB	4E	; Down 0C

	ORG	40	; DATA TABLE TO RUN MAZE: Distances
	DB	0B	; Down B
	DB	03	; Right 3
	DB	08	; Up 8
	DB	03	; Right 3
	DB	06	; Down 6
	DB	03	; Right 3
	DB	09	; Up 9
	DB	05	; Left 5
	DB	03	; Up 3
	DB	08	; Right 8
	DB	0C	; Down C

; -------------------------------------------------------------------------
	END

; -------------------------------------------------------------------------
