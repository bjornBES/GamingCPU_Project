#!/bin/bash

srcDir="./src/DOS/src"
buildDir="./src/DOS/Build"

srcStage1="$srcDir/Bootloader/stage1"
buildStage1="$buildDir/Bootloader/stage1"

srcStage2="$srcDir/Bootloader/stage2"
buildStage2="$buildDir/Bootloader/stage2"

srcDOS="$srcDir/DOS"
buildDOS="$buildDir/DOS"

assembler="./AssemblerBCG/build/AssemblerBCG"
linker="./BCGLinker/build/BCGLinker"
compiler="./Compiler/build/Compiler"
filesystem="./filesystem/build/filesystem"

dotnet build

clear

$filesystem -o "src/DOS/disk.bin" -O -S "0x81"
