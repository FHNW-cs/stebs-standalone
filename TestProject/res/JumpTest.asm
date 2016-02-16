  JZ Fish
  MOV DL,02
DB 'h'
DB 55
DB "efg"
ORG 60
  JMP Start
Start:
  MOV AL,0F
  JMP Fish
Fish:
  MOV BL,01
  JMP Start

END

; Anderer Test
ORG 10
JMP Gary
MOV Al, EE

ORG 20
Gary:
MOV BL,CC


ORG 00
JMP Gary
MOV DL, AA

ORG 1A
JMP Gary

ORG 24
JMP Gary

END

