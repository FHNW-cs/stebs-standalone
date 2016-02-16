; ===================================================================================
; Program to control the heater.
; ===================================================================================

    ORG   00         
; ===================================================================================
; Endlessly control the heater switching it on or off aiming at holding the
; target temperature.
; ===================================================================================
Main:
    MOV   BL,14   ; Init the target temperature: 20°C
    
Loop:   
    IN    01      ; Read the current heater temperature
    AND   AL,40   ;  in consulting the thermostat flag
    CMP   AL,40   ; Test if above target temperature
    JZ    Off     ; If flag set, turn the heater on, else off

On:
    MOV   AL,80   ; Prepare heater-on action
    JMP   Continue

Off:
    MOV   AL,00   ; Prepare heater-off action
    
Continue:
    OR    AL,BL   ; Preserve target temperature
    OUT   01      ; Apply action: on or off
    JMP   Loop
; -----------------------------------------------------------------------------------

    END
