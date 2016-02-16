; Multiply

; plus * plus, no overflow
	MOV	AL,35
 	MOV	BL,02
	MUL	AL,BL	; AL = 6A
; plus * plus, overflow
	MOV	CL,15
 	MOV	DL,22
	MUL	CL,DL	; CL = CA


; plus * minus, no overflow
	MOV	AL,19 
 	MOV	BL,FC 
	MUL	AL,BL	; AL = 9C
; plus * minus, overflow
	MOV	CL,55 
 	MOV	DL,FD 
	MUL	CL,DL	; CL = 01


; minus * plus, no overflow
	MOV	AL,F7
 	MOV	BL,09
	MUL	AL,BL	; AL = AF
; minus * plus, overflow
	MOV	CL,E5
 	MOV	DL,22
	MUL	CL,DL	; CL = 6A


; minus * minus, no overflow
	MOV	AL,FD 
 	MOV	BL,F7
	MUL	AL,BL	; AL = 1B
; minus * minus, overflow
	MOV	CL,ED 
 	MOV	DL,F3
	MUL	CL,DL	; CL = F7


; zero, no overflow
	MOV	AL,00 
 	MOV	BL,00
	MUL	AL,BL	; AL = 00

	MOV	AL,00 
 	MOV	BL,1B
	MUL	AL,BL	; AL = 00

	MOV	AL,00 
 	MOV	BL,F7
	MUL	AL,BL	; AL = 00

	MOV	AL,1C 
 	MOV	BL,00
	MUL	AL,BL	; AL = 00

	MOV	AL,DA 
 	MOV	BL,00
	MUL	AL,BL	; AL = 00


	END