; ===================================================================================
; Program to control the 7-segment display.
; ===================================================================================

    ORG   00         
; ===================================================================================
; Endlessly control the 7-segment display.
; Left cypher in display is decremented, right cypher is incremented.
; ===================================================================================
Main:
    MOV   CL,80   ; Init table_startaddress for right display 
    MOV   DL,89   ; Init table_endaddress for left display 

Loop:

RIGHT:
    CMP   CL,89   ; Check if cypher '9' table_address reached
    JNZ   IncrRight
    
ResetRight:
    MOV   CL,7F   ; Reset to table_startaddress - 1
    
IncrRight:
    INC   CL      ; Increment cypher table_address
    CALL  50      ; Call DisplayRight


LEFT:
    CMP   DL,80   ; Check if cypher '0' table_address reached
    JNZ   DecrLeft
    
ResetLeft:
    MOV   DL,8A   ; Reset to table_endaddress + 1
    
DecrLeft:
    DEC  DL      ; Decrement cypher table_address
    CALL  60      ; Call DisplayLeft

    JMP   Loop    

    HALT          
; -----------------------------------------------------------------------------------


    ORG   50      
; ===================================================================================
; Update the right cypher of the 7-segment display.
;
; Register Input:
;   CL = startaddress for right display 
; Register Output:
;   changed: AL, SR
; ===================================================================================
DisplayRight:
    MOV   AL,[CL] ; Get cypher code
    OR    AL,80   ; Update right segment code
    OUT   02      ; Write cypher
    RET           
; -----------------------------------------------------------------------------------


    ORG   60      
; ===================================================================================
; Update the left cypher of the 7-segment display.
;
; Register Input:
;   DL = startaddress for right display 
; Register Output:
;   changed: AL, SR
; ===================================================================================
DisplayLeft:
    MOV   AL,[DL] ; Get cypher code
;   AND   AL,7F   ; Update left segment code
    OUT   02      ; Write cypher
    RET                
; -----------------------------------------------------------------------------------


    ORG   80      
; ===================================================================================
; Codes for the 7-segment displays
; ===================================================================================
Cyphers:
    DB    3F      ; '0'
    DB    06      ; '1'
    DB    5B      ; '2'
    DB    4F      ; '3'
    DB    66      ; '4'
    DB    6D      ; '5'
    DB    7D      ; '6'
    DB    07      ; '7'
    DB    7F      ; '8'
    DB    6F      ; '9'
; -----------------------------------------------------------------------------------

    END           
