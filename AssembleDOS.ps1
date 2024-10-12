$SrcDir = "./CPU/src/DOS/src"
$BuildDir = "./CPU/src/DOS/build"

$assembler = Resolve-Path "./AssemblerBCG/build/AssemblerBCG.exe"
$linker = Resolve-Path "./BCGLinker/build/BCGLinker.exe"
$compiler = Resolve-Path "./Compiler/build/Compiler.exe"

& $assembler -i $SrcDir/Bootloader/stage1/newBootloader.acl -o $BuildDir/Bootloader/stage1/newBootloader.o
& $assembler -i $SrcDir/Bootloader/stage2/stage2.acl -o $BuildDir/Bootloader/stage2/stage2.o
& $assembler -i $SrcDir/kernel/kernel.acl -o $BuildDir/kernel.o
& $assembler -i $SrcDir/dos.acl -o $BuildDir/DOS.o

& $compiler -i $SrcDir/Bootloader/stage2/main.ccl -o $BuildDir/Bootloader/stage2/CCL/main.acl -cpu BC16CE
& $assembler -i $BuildDir/Bootloader/stage2/CCL/main.acl -o $BuildDir/Bootloader/stage2/main.o

& $linker -i $BuildDir/DOS.o -o $BuildDir/DOS.bin -fbin
& $linker -i $BuildDir/kernel.o -o $BuildDir/kernel.bin -fbin
& $linker -i $BuildDir/Bootloader/stage1/newBootloader.o -o $BuildDir/Bootloader/stage1/Bootloader.bin -fbin
& $linker -i $BuildDir/Bootloader/stage2/stage2.o $BuildDir/Bootloader/stage2/main.o -o $BuildDir/Bootloader/stage2/stage2.bin -fbin


./fileSystem/bin/Debug/netcoreapp3.1/filesystem.exe -i [ $BuildDir/Bootloader/stage2/stage2.bin $SrcDir/Test.txt $BuildDir/kernel.bin $BuildDir/DOS.bin ] -o ./CPU/src/DOS/disk.bin -B $BuildDir/Bootloader/stage1/Bootloader.bin -S 0x81
