using System;
using System.Collections.Generic;
using System.Text;

public class AssmeblerErrors : AssemblerSettings
{

    public void E_InvalidInstruction(string instruction, string file, int linenumber)
    {
        string errorMessage = "";

        errorMessage += STDErrorPrintStart(file, linenumber);

        errorMessage += $"|\tBGC-ASL16 Error: Invalid instruction{Environment.NewLine}";
        errorMessage += $"|\t\t{instruction}{Environment.NewLine}";

        errorMessage += STDErrorPrintEnd(file, linenumber);

        Error();

        PrintError(errorMessage);
    }

    string STDErrorPrintStart(string file, int linenumber)
    {
        return $"|\t in file {file}:{linenumber + 1}{Environment.NewLine}";
    }
    string STDErrorPrintEnd(string file, int linenumber)
    {
        return $"|{Environment.NewLine}";
    }

    void Error()
    {
        Assembler.m_WriteOut = false;
    }
    void PrintError(string errorMessage)
    {
        Console.WriteLine(errorMessage);
    }
}
