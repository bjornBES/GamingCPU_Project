.\AssemblerBCG\build\AssemblerBCG.exe -i .\src\BIOS\src\BIOS.acl -o .\src\BIOS\Builds\BIOS.o
.\BCGLinker\build\BCGLinker.exe -i .\src\BIOS\Builds\BIOS.o -o .\src\BIOS\BIOS.bin -ls .\src\BIOS\BIOSLinker.conf -fbin