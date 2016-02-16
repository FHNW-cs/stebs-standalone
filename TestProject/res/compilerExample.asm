; HIGH LEVEL CODE
;
; Calculate r : = m % n
; Parameter m is to be stored in address FF
; Parameter n is to be stored in address FE
; The result is stored in address FD
; This demonstration shows parameter passing etc as a compiler would translate
; the following high level language (except user input, output):
;   program mod
;   global
;     proc modulo(in copy m:int,in copy n:int,out ref r:int)
;     do
;       r init := m;
;       while r >= n do
;         r := r - n;
;       endwhile
;     endproc;
;
;     var m:int;
;     var n:int;
;     var r:int;
;   do
;     ? m init;
;     ? n init;
;     call divide(m,n,r init);
;     ! r
;   endprogram


; TRANSLATED LOW LEVEL CODE
;
; Set up stack (save memory space as no init done by machine)
       
; Alloc(0,3);            ; allocate 3 slots on stack for vars m,n and r
; IntLoad(1,0);          ;  load addr(m)
; IntInput(2,"m");       ;  get input m = 7 and store it at addr(m)
; IntLoad(3,1);          ;  load addr(n)
; IntInput(4,"n");       ;  get input n = 3 and store it at addr(n)
    ORG   FD
    DB    00       ; output value r (to be calculated: r = m % n)

    ORG   FE
    DB    09       ; input value n  ; 9dec

    ORG   FF
    DB    17       ; input value m  ; 23dec
                   ;  result r = 23dec modulo 9dec = 5dec

  
    ORG   00
    ; init SP and FP
    MOV   BL,FF    ; init BL (in the role of FP)
    MOV   AL,F0    ; init 
    MOV   SP,AL    ;  SP

; IntLoad(5,0);          ;   load addr(m) 
; Deref(6);              ;   dereference addr(m) to get m
    MOV   AL,FF    ; intLoad(FF),deref()
    MOV   AL,[AL]
    PUSH  AL
    
; IntLoad(7,1);          ;   load addr(n)
; Deref(8);              ;   dereference addr(n) to get n
    MOV   AL,FE    ; intLoad(FE),deref()
    MOV   AL,[AL]
    PUSH  AL
    
; IntLoad(9,2);          ;   load addr(r)
    MOV   AL,FD    ; intLoad(FD)
    PUSH  AL

; Call(10,15);           ;  call procedure
    MOV   AL,SP    ; save sp                        ; MOV   FP,SP     !!!!
    PUSH  BL       ; save fp onto stack
    MOV   BL,AL    ; fp := sp_old
    CALL  30       ; modulo

; Stop(14);              ; done
    HALT
  
  
    ORG   30       ; modulo
; Enter(15,0,0);         ;  (sp := fp + 3): protect old fp and return address

; LoadRel(16,(-3));      ;  load address of procedure param m relative to fp
; Deref(17);             ;  get value of m
    MOV   AL,BL
    ADD   AL,03    ; loadRel(03),deref()
    MOV   AL,[AL]
    PUSH  AL

; LoadRel(18,(-1));      ;  load address of procedure param r relative to fp
; Deref(19);             ;  get ref(r)
    MOV   AL,BL
    ADD   AL,01    ; loadRel(01),deref()
    MOV   AL,[AL]
    PUSH  AL
    
; Store(20);             ;  store: initialise r := m
    POP   AL       ; addr
    POP   CL       ; value
    MOV   [AL],CL  ; r := m
    
;                        ; WHILE
; LoadRel(21,(-1));      ;  load address of procedure param r relative to fp
; Deref(22);             ;  get ref(r)
While:
    MOV   AL,BL
    ADD   AL,01    ; loadRel(01),deref()
    MOV   AL,[AL]
    PUSH  AL
     
; Deref(23);             ;  get value of r
    POP   AL
    MOV   AL,[AL]
    PUSH  AL
    
; LoadRel(24,(-2));      ;  load address of procedure param n relative to fp
; Deref(25);             ;  get value of copy n
    MOV   AL,BL
    ADD   AL,02    ; loadRel(02),deref()
    MOV   AL,[AL]
    PUSH  AL

; IntGE(26);             ;  compare r >= n
; CondJump(27,38);       ;  if true continue else jump
    POP   CL       ; BL := n
    POP   AL       ; AL := r
    CMP   AL,CL    ; r - n
    JS    Endwhile ; r - n > 0
    
; LoadRel(28,(-1));      ;  load address of procedure param r relative to fp
; Deref(29);             ;  get ref(r)
    MOV   AL,BL
    ADD   AL,01    ; loadRel(01),deref()
    MOV   AL,[AL]
    PUSH  AL

; Deref(30);             ;  get value of r
    POP   AL
    MOV   AL,[AL]
    PUSH  AL
    
; LoadRel(31,(-2));      ;  load address of procedure param n relative to fp
; Deref(32);             ;  get value of n
    MOV   AL,BL
    ADD   AL,02    ; loadRel(02),deref()
    MOV   AL,[AL]
    PUSH  AL
    
; IntSub(33);            ;  calculate r - n
    POP   CL       ; BL := n
    POP   AL       ; AL := r
    SUB   AL,CL    ; AL := r - n
    PUSH  AL
    
; LoadRel(34,(-1));      ;  load address of procedure param r relative to fp
; Deref(35);             ;  get ref(r)
    MOV   AL,BL
    ADD   AL,01    ; loadRel(01),deref()
    MOV   AL,[AL]
    PUSH  AL
    
; Store(36);             ;  store:  r := r - n
    POP   AL       ; addr
    POP   CL       ; value
    MOV   [AL],CL  ; r := r - n

; UncondJump(37,21);     ;  goto while
    JMP   While
    
;                        ; ENDWHILE
; Return(38,3);          ;  (sp := fp - 3)
Endwhile:
    POP   CL       ; save return addr
    POP   BL       ; restore old fp
    
    MOV   AL,SP
    ADD   AL,03    ; deallocate 3 bytes
    MOV   SP,AL    ; restore sp
  
    PUSH  CL       ; return
    RET            ;  past call
    

    END
