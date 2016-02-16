; Meine Testprogramme
; -------------------

ROL BL ; Ein ROL Test

; blbla


RET

STI ; ohne Parameter

; 1 Parameter
INC AL ; AL erhöhen
IN 05

ADD BL,AL
ADD BL , AL ;Parameter mit Leezeichen

ADD CL,65
ADD DL,D4 ; Hex Wert

MUL AL, 4D

XOR SP,AD

MOV CL,3E
MOV CL,F    ; einstellige Konstante
MOV AL,[05]
MOV [D],BL    ;einstellige Konstante
MOV [D4],DL
MOV CL,[AL]  ; opCode D3
MOV [BL],AL
MOV AL,BL


HALT


ORG 80
DB 'H'
DB "sdf"
DB 33

END
