$SrcDir = "./src/basic/src"
$BuildDir = "./src/basic/build"

$assembler = Resolve-Path "./AssemblerBCG/build/AssemblerBCG.exe"
$linker = Resolve-Path "./BCGLinker/build/BCGLinker.exe"
$compiler = Resolve-Path "./Compiler/build/Compiler.exe"

& $assembler -i $SrcDir/stage1/Bootloader.acl -o $BuildDir/stage1/Bootloader.o
& $assembler -i $SrcDir/basic/basic.acl -o $BuildDir/basic/basic.o

& $linker -i $BuildDir/stage1/Bootloader.o -o $BuildDir/stage1/Bootloader.bin -fbin -ls $SrcDir/stage1/stage1.conf
& $linker -i $BuildDir/basic/basic.o -o $BuildDir/basic/basic.bin -fbin -ls $SrcDir/basic/basic.conf

./fileSystem/bin/Debug/netcoreapp3.1/filesystem.exe -i [ $BuildDir/basic/basic.bin ] -o ./src/basic/disk.bin -B $BuildDir/stage1/Bootloader.bin -S 0x81
