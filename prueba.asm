;Primer constructor
; 28/10/2022 08:54:50 p. m.
#make_COM#
include emu8086.inc
ORG 100h
MOV AX, 1
PUSH AX
POP AX
MOV x, AX
MOV AX, 5
PUSH AX
POP AX
MOV y, AL
PRINT 'Hola mundo'
MOV AX, x
PUSH AX
MOV AL, y
PUSH AX
POP BX
POP AX
CMP AX, BX
JL if1
MOV AX, x
PUSH AX
MOV AL, y
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE if2
PRINT 'Esto tampoco'
JMP else2
if2:
MOV AX, x
PUSH AX
MOV AL, y
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if3
PRINT 'Esto nuevamente si'
JMP else3
if3:
PRINT 'Esto nuevamente no'
else3:
PRINT 'Esto si'
else2:
JMP else1
if1:
PRINT 'Esto neh'
else1:

;Variables
	area dd ?
	radio dd ?
	pi dd ?
	resultado dd ?
	a dw ?
	d dw ?
	altura dw ?
	cinco dw ?
	x dd ?
	y db ?
	i dw ?
	j dw ?
ret
define_print_num
define_print_num_uns
define_scan_num
END
