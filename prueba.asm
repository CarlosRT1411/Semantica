;Primer constructor
; 02/11/2022 03:23:16 p. m.
#make_COM#
include emu8086.inc
ORG 100h
PRINT 'Introduce la altura de la piramide: '
CALL scan_num
MOV altura, CX
PRINTN ''
MOV AX, altura
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE if1
MOV AX, altura
PUSH AX
POP AX
MOV i, AX
InicioFor1:
MOV AX, i
PUSH AX
MOV AX, 0
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE finFor1
MOV AX, 0
PUSH AX
POP AX
MOV j, AX
InicioFor2:
MOV AX, j
PUSH AX
MOV AX, altura
PUSH AX
MOV AX, i
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finFor2
MOV AX, j
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JE if2
PRINT '*'
JMP else2
if2:
PRINT '-'
else2:
INC j
JMP InicioFor2
finFor2:
PRINTN ''
PRINT ''
DEC i
JMP InicioFor1
finFor1:
JMP else1
if1:
PRINTN ''
PRINTN 'Error: la altura debe de ser mayor que 2'
PRINT ''
else1:
MOV AX, 1
PUSH AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JE if3
PRINT 'Esto no se debe imprimir'
MOV AX, 2
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if4
PRINT 'Esto tampoco'
JMP else4
if4:
else4:
JMP else3
if3:
else3:
MOV AX, 256
PUSH AX
POP AX
MOV a, AX
PRINT 'Valor de variable int 'a' antes del casteo: '
MOV AX, a
PUSH AX
POP AX
CALL PRINT_NUM
MOV AX, a
PUSH AX
POP AX
MOV y, AL
PRINTN ''
PRINT 'Valor de variable char 'y' despues del casteo de a: '
MOV AL, y
MOV AH, 0
PUSH AX
POP AX
CALL PRINT_NUM
PRINTN ''
PRINTN 'A continuacion se intenta asignar un int a un char sin usar casteo: '
PRINT ''

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
