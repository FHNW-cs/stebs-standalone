Start:
        MOV AL, BL
    MOV [D4],DL
MOV CL , 3E
MOV DL ,FE
IN 05
INC AL ;test
Fisch:
ROL BL;commentttt
JMP Start
        HALT
END;test
;test comment
;sldfjsldkf

ORG 20
DB "test"
DB 'c'
DB 5E

;sldkfjslkdf