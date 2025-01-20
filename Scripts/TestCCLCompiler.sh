# /bin/bash

assembler="./AssemblerBCG/build/AssemblerBCG"
linker="./BCGLinker/build/BCGLinker"
compiler="./Compiler/build/Compiler"
filesystem="./filesystem/build/filesystem"
HLCLCompiler="./HLCLCompiler/bin/Debug/net8.0/HLCLCompiler"
CCLCompiler="./Compiler/build/Compiler"

dotnet build

$CCLCompiler -i ./src/CCL/testprogram.CCL -o ./src/CCL/CCLOutput.acl -cpu BC16
