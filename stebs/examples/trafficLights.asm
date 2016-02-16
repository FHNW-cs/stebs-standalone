; ===================================================================================
; Program to control the traffic lights.
; Hardware interrupts toggle blinking lights (active/inactive). 
; ===================================================================================

    ORG   00         
; ===================================================================================
; Endlessly change traffic lights.
; ===================================================================================
Main:
    JMP   Init    ; Jump over hw interrupt vector
; -----------------------------------------------------------------------------------


    ORG   02
; ===================================================================================
    DB    40      ; Hw interrupt routine
; ===================================================================================


    ORG   08
; -----------------------------------------------------------------------------------
Init:
    STI           ; Enable hw interupts
    MOV   CL,80   ; Init pointer with startaddress of left traffic light table
    MOV   DL,86   ; Init pointer with startaddress of right traffic light table
    CALL  50      ; Call SetLights

Loop:
    CMP   CL,83   ; Table end address reached?
    JNZ   IncrLeft
InitLeft:
    MOV   CL,80   ; Init left traffic light table address
    JMP   Continue
IncrLeft:
    INC   CL      ; Point to next table address

Continue:
    CMP   DL,87   ; Table end address reached?
    JNZ   IncrRight
InitRight:
    MOV   DL,84   ; Init right traffic light table address
    JMP   Cont2
IncrRight:
    INC   DL      ; increment DL

Cont2:
    CALL  60      ; Call SetLights

    JMP   Loop    ; Repeat endlessly
; -----------------------------------------------------------------------------------


    ORG   40
; ===================================================================================
; Activate/deactivate blinking of left and right lights.
; A single interrupt toggles from one to the other state.
;
; Register Input:
;   --
; Register output:
;   changed: AL
; ===================================================================================
IntRoutine:
    IN    00      ; Read traffic lights
    AND   AL,11   ; Test whether blinking is on or off
    JZ    BlinkingOn
    
BlinkingOff:
    IN    00      ; Read traffic lights
    AND   AL,EE   ; Set blinking of both traffic lights off
    JMP   Exit
    
BlinkingOn:
    IN    00      ; Read traffic lights
    OR    AL,11   ; Set blinking of both traffic lights on
    
Exit:
    OUT   00      ; Apply on or off
    IRET          
; ------------------------------------------------------------------------------


    ORG   60      
; ===================================================================================
; Set the left and the right lights.
;
; Register Input:
;   CL = pointer_left_lights_table
;   DL = pointer_right_lights_table
; Register output:
;   changed: AL,BL, SR
; ===================================================================================
SetLights:
    CLI           ; Inhibit hardware interrupts
    MOV   AL,[CL] ; Get code for left lights
    MOV   BL,[DL] ; Get code for right lights
    OR    BL,AL   ; Combine them
    IN    00      ; Get blinking
    AND   AL,11   ;  state
    OR    AL,BL   ;  and preserve it
    OUT   00      ; Send code to lights
    STI           ; Allow hardware interrupts
    RET           
; -----------------------------------------------------------------------------------

                  
    ORG   80
; ===================================================================================
; Codes for the left lights
; -----------------------------------------------------------------------------------
LeftLights:
    DB    80      ; left red
    DB    C0      ; left red + yellow
    DB    20      ; left green
    DB    40      ; left yellow

     
    ORG   84
; ===================================================================================
; Codes for the right lights
; -----------------------------------------------------------------------------------
RightLights:
    DB    08      ; right red
    DB    0C      ; right red + yellow
    DB    02      ; right green
    DB    04      ; right yellow
; -----------------------------------------------------------------------------------

    END           
