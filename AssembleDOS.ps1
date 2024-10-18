$SrcDir = "./src/DOS/src"
$BuildDir = "./src/DOS/build"

$assembler = Resolve-Path "./AssemblerBCG/build/AssemblerBCG.exe"
$linker = Resolve-Path "./BCGLinker/build/BCGLinker.exe"
$compiler = Resolve-Path "./Compiler/build/Compiler.exe"

& $assembler -i $SrcDir/Bootloader/stage1/newBootloader.acl -o $BuildDir/Bootloader/stage1/newBootloader.o
& $assembler -i $SrcDir/Bootloader/stage2/stage2.acl -o $BuildDir/Bootloader/stage2/stage2.o
& $assembler -i $SrcDir/DOS/dos.acl -o $BuildDir/DOS/dos.o

& $compiler -i $SrcDir/Bootloader/stage2/main.ccl -o $BuildDir/Bootloader/stage2/CCL/main.acl -cpu BC16CE
& $assembler -i $BuildDir/Bootloader/stage2/CCL/main.acl -o $BuildDir/Bootloader/stage2/main.o

& $linker -i $BuildDir/DOS/dos.o -o $BuildDir/DOS/dos.bin -fbin -ls $SrcDir/DOS/DOS.conf
& $linker -i $BuildDir/Bootloader/stage1/newBootloader.o -o $BuildDir/Bootloader/stage1/Bootloader.bin -fbin -ls $SrcDir/Bootloader/stage1/stage1.conf
& $linker -i $BuildDir/Bootloader/stage2/stage2.o $BuildDir/Bootloader/stage2/main.o -o $BuildDir/Bootloader/stage2/stage2.bin -fbin -ls $SrcDir/Bootloader/stage2/stage2.conf


./fileSystem/bin/Debug/netcoreapp3.1/filesystem.exe -i [ $BuildDir/Bootloader/stage2/stage2.bin $SrcDir/Test.txt $BuildDir/DOS/DOS.bin ] -o ./CPU/src/DOS/disk.bin -B $BuildDir/Bootloader/stage1/Bootloader.bin -S 0x81
