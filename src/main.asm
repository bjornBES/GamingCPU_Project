section .text
bits 16 ; use 16 bits 
org 0x7c00 ; sets the start address init: 
  mov si, msg ; loads the address of "msg" into SI register 
  mov ah, 0x0e ; sets AH to 0xe (function teletype) 
print_char: 
  lodsb ; loads the current byte from SI into AL and increments the address in SI 
  cmp al, 0 ; compares AL to zero 
  je done ; if AL == 0, jump to "done" 
  int 0x10 ; print to screen using function 0xe of interrupt 0x10
  jmp print_char ; repeat with next byte 
done: 
  hlt 
times 510-($-$$) db 0
dw 0x55AA