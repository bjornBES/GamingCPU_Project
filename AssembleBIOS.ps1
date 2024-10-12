.\AssemblerBCG\build\AssemblerBCG.exe -i .\CPU\src\BIOS\BIOS.acl -o .\CPU\src\BIOSBin\OBJ\BIOS.o
.\BCGLinker\build\BCGLinker.exe -i .\CPU\src\BIOSBin\OBJ\BIOS.o -o .\CPU\src\BIOSBin\Bin\BIOS.bin -fbin