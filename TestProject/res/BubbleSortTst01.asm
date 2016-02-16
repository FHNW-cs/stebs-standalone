; --------------------------------------------------------------------
; 
; Programname:	BubbleSortTst01
; Purpose:	sort data table
; Description:	this program is used to test the microprocessor
;		simulator m2Sim.
;		With this program a data table is sorted. Sort
;		technique used is bubble sort, not optimized. 
;		watch out: negative numbers will become first in table
; Author:	Urs Lüchinger
; History:	4.09.2003 original version 	
; 
; --------------------------------------------------------------------
; 

	JMP	Start	; Skip past the data table.

; --------------------------------------------------------------------
	DB	00	; sort flag. 00=unsorted, 01=sorted  
	DB	64	; Data table begins here.
	DB	78	; any value
	DB	31	; any value
	DB	48	; any value
	DB	00	; any value
	DB	12	; any avlue
	DB	04	; any value 
; --------------------------------------------------------------------
	ORG	10
Start:
	MOV	AL,1	; value for sorted
	CMP	AL,[02]	; compare with sort flag
	JZ	End	; done
	MOV	[02],AL	; indicates sorted
	MOV	CL,03	; 03 is start address of data table
	MOV	DL,7	; 7  is number of table items
Rep:
	DEC	DL
	CMP	DL,0	; all items done 
	JZ	Start	; yes. next loop
	MOV	AL,[CL]	; Copy data from table to AL
	INC	CL	; point to next
	MOV	BL,[CL]	; Copy next entry of data table
	CMP	AL,BL	; compare the two items
	JZ	Rep	; AL=BL. compare next
	JS	Rep	; AL<BL. JNS if order should be descending 
	XOR	AL,BL	; change bit settings
	XOR	BL,AL	; "
	XOR	AL,BL	; "
	DEC	CL	; point to first item of the two
	MOV	[CL],AL	; move to data table
	INC	CL	; point to second
	MOV 	[CL],BL	; move to data table
	MOV	AL,0	; value for unsorted
	MOV	[02],AL	; set flag unsorted
	JMP	Rep
	
; --------------------------------------------------------------------
; end of program
; --------------------------------------------------------------------
End:
	END
; _____ Program ends __________________________________________

