.\AssemblerBCG16\build\AssemblerBCG16.exe -i .\CPU\src\DOS/dos.acl -o .\CPU\src\DOS/build/DOS.o
.\BCGLinker\build\BCGLinker.exe -i .\CPU\src\DOS/build/DOS.o -o .\CPU\src\DOS/build/DOS.bin -fbin