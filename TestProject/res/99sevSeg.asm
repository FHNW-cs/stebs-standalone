; -------------------------------------------------------------------------
;  A PROGRAM TO EXERCISE THE SEVEN SEGMENT DISPLAYS.
; -------------------------------------------------------------------------
;  It is up to you to modify this program to get the displays 
;  to step through recognisable sequences. Try counting or 
;  programming the segments to spell H E L L O.
;  Use a data table. It is poor practice to mix code and data
;  as happens in this example.
; -------------------------------------------------------------------------

Start:
	MOV	AL,FF	; Turn on all the right segments.
	OUT	02	; Send data to port 02.
	MOV	AL,01	; Turn off all the right segments.
	OUT	02
	MOV	AL,FE	; Turn on all the left segments.
	OUT	02
	MOV	AL,0	; Turn off all the left segments.
	OUT	02
	JMP	Start	; Jump back to the start.

	END
	
; -------------------------------------------------------------------------
