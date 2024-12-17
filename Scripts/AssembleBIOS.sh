#!/bin/bash

assembler="./AssemblerBCG/build/AssemblerBCG"
linker="./BCGLinker/build/BCGLinker"
compiler="./Compiler/build/Compiler"
filesystem="./filesystem/build/filesystem"

$assembler -i "./src/BIOS/src/BIOS.acl" -o "./src/BIOS/Builds/BIOS.o"
$linker -i "./src/BIOS/Builds/BIOS.o" -o "./src/BIOS/BIOS.bin" -ls "./src/BIOS/BIOSLinker.conf"