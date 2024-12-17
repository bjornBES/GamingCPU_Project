SrcDir="./src/basic/src"
BuildDir="./src/basic/build"

assembler="./AssemblerBCG/build/AssemblerBCG"
linker="./BCGLinker/build/BCGLinker"
compiler="./Compiler/build/Compiler"
filesystem="./filesystem/build/filesystem"

$assembler -i $SrcDir/Basic.acl -o $BuildDir/Basic.o

$linker -i $BuildDir/Basic.o -o $BuildDir/Basic.bin -fbin -ls $SrcDir/Basic.conf
