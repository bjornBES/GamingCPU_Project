#!/bin/bash

srcDir="./src/DOS/src"
buildDir="./src/DOS/Build"

srcStage1="$srcDir/Bootloader/stage1"
buildStage1="$buildDir/Bootloader/stage1"

srcStage2="$srcDir/Bootloader/stage2"
buildStage2="$buildDir/Bootloader/stage2"

srcDOS="$srcDir/DOS"
buildDOS="$buildDir/DOS"

declare -a filesInDisk=("$buildStage2/stage2.bin" "$srcDir/Test.txt" "$buildDOS/DOS.bin")

assembler="./AssemblerBCG/build/AssemblerBCG"
linker="./BCGLinker/build/BCGLinker"
compiler="./Compiler/build/Compiler"
filesystem="./filesystem/build/filesystem"

# Create directories if they don't exist
mkdir -p "$buildStage1" "$buildStage2" "$buildDOS"

dotnet build
clear

# Assembly and Compilation
$assembler -i "$srcStage1/newBootloader.acl" -o "$buildStage1/newBootloader.o"
$assembler -i "$srcStage2/stage2.acl" -o "$buildStage2/stage2.o"
# $assembler -i "$srcDOS/dos.acl" -o "$buildDOS/dos.o"

$compiler -i "$srcStage2/main.ccl" -o "$buildStage2/CCL/main.acl" -cpu BC16CE
$assembler -i "$buildStage2/CCL/main.acl" -o "$buildStage2/main.o"

# Linking
$linker -i "$buildStage1/newBootloader.o" -o "$buildStage1/Bootloader.bin" -ls "$srcStage1/stage1.conf"
$linker -i "$buildStage2/stage2.o" "$buildStage2/main.o" -o "$buildStage2/stage2.bin" -ls "$srcStage2/stage2.conf"
# $linker -i "$buildDOS/dos.o" -o "$buildDOS/dos.bin" -ls "$srcDOS/DOS.conf"

# Prepare files for filesystem
files=""
for i in "${filesInDisk[@]}"; do
    files+="$i "
done

# Run filesystem command
$filesystem -i "[" $files "]" -o "./src/DOS/disk.bin" -B "$buildStage1/Bootloader.bin" -S 0x81
