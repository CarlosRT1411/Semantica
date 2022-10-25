;Primer constructor
; 25/10/2022 09:45:40 a. m.
#make_COM#
include emu8086.inc
ORG 100h
	area dw ?
	radio dw ?
	pi dw ?
	resultado dw ?
	a dw ?
	d dw ?
	altura dw ?
	x dw ?
	y dw ?
	i dw ?
	j dw ?
MOV AX, 3
PUSH AX
MOV AX, 5
PUSH AX
POP BX
POP AX
ADD AX, BX
PUSH AX
MOV AX, 8
PUSH AX
POP BX
POP AX
MUL BX
PUSH AX
MOV AX, 10
PUSH AX
MOV AX, 4
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
DIV BX 
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP AX
MOV y, AX
MOV AX, 60
PUSH AX
MOV AX, 61
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if1
MOV AX, 10
PUSH AX
POP AX
MOV x, AX
if1:
ret
END
