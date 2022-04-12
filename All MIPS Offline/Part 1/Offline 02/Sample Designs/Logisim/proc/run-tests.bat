for %%i in (src-*.txt) do MyProcAssembler.exe %%i

for %%i in (src-*.txt.out) do call :renameit %%~ni

for %%i in (test-*.txt) do java -jar logisim-generic-2.7.1.jar test-proc.circ -tty table -load %%i > %%i.out

ProcTestValidator.exe

del test-*.txt
del test-*.txt.out


goto end



:renameit
set old=%1
ren %1.out test%old:~3%
:end

:: java -jar logisim-generic-2.7.1.jar test-proc.circ -tty table -load test-error-code.txt > test-error-code.txt.out

:: java -jar logisim-generic-2.7.1.jar test-proc.circ -tty table -load test-addi-code.txt > test-addi-code.txt.out
:: java -jar logisim-generic-2.7.1.jar test-proc.circ -tty table -load test-add-code.txt > test-add-code.txt.out
:: java -jar logisim-generic-2.7.1.jar test-proc.circ -tty table -load test-shli-code.txt > test-shli-code.txt.out
:: java -jar logisim-generic-2.7.1.jar test-proc.circ -tty table -load test-jumps-code.txt > test-jumps-code.txt.out
:: java -jar logisim-generic-2.7.1.jar test-proc.circ -tty table -load test-mem-code.txt > test-mem-code.txt.out
:: java -jar logisim-generic-2.7.1.jar test-proc.circ -tty table -load test-stack-code.txt > test-stack-code.txt.out
:: java -jar logisim-generic-2.7.1.jar test-proc.circ -tty table -load test-proc-code.txt > test-proc-code.txt.out


