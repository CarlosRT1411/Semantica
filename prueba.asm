;Primer constructor
; 29/10/2022 06:05:55 p. m.
#make_COM#
include emu8086.inc
ORG 100h
whileInicio1:
MOV AX, i
PUSH AX
MOV AX, 5
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE whileFin1:
whileInicio2:
MOV AX, j
PUSH AX
MOV AX, 10
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE whileFin2:
PRINT 's'
MOV AX, 2
PUSH AX
POP AX
MOV BX, j
ADD BX, AX 
MOV j, BX
JMP whileInicio2:
whileFin2:
PRINTN ''
PRINTN 'Arriba'
PRINTN ' las chivas'
PRINT ''
INC i
JMP whileInicio1:
whileFin1:

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
