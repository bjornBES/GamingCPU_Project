using AssemblerBCG;
using System;
using System.Collections.Generic;
using System.Text;

public class AssmeblerErrors : AssemblerVariabels
{
    public void E_InvalidInstruction(string instruction)
    {
        string errorMessage = "";

        errorMessage += sTDErrorPrintStart();

        errorMessage += $"|\tBGC-ASL16 Error: Invalid instruction{Environment.NewLine}";
        errorMessage += $"|\t\t{instruction}{Environment.NewLine}";

        errorMessage += sTDErrorPrintEnd();

        error();

        printError(errorMessage);
    }
    public void E_InvalidRegister(RegisterInfo registerInfo)
    {
        string errorMessage = "";

        errorMessage += sTDErrorPrintStart();

        errorMessage += $"|\tBGC-ASL16 Error: Invalid Register{Environment.NewLine}";
        errorMessage += $"|\t\t{registerInfo.m_Register}{Environment.NewLine}";

        errorMessage += sTDErrorPrintEnd();

        error();

        printError(errorMessage);
    }
    public void E_InvalidCPUFeature(CPUType NeededCPU)
    {
        string errorMessage = "";

        errorMessage += sTDErrorPrintStart();

        errorMessage += $"|\tBGC-ASL16 Error: this feature is not available using the {m_CPUType}{Environment.NewLine}";
        errorMessage += $"|\tYou need to use something this or the veriant over the {NeededCPU} to get the feature. You can do this by putting .SETCPU \"{NeededCPU}\"";

        errorMessage += sTDErrorPrintEnd();

        error();

        printError(errorMessage);
    }

    string sTDErrorPrintStart()
    {
        return $"|\t in file {m_file}:{Linenumber + 1}{Environment.NewLine}";
    }
    string sTDErrorPrintEnd()
    {
        string message = "";

        message += $"|{Environment.NewLine}";
        
        message += $"|\t{m_src[m_index - 1]}{Environment.NewLine}";
        message += $"|\t{m_src[m_index]}{Environment.NewLine}";
        message += $"|\t{m_src[m_index + 1]}{Environment.NewLine}";

        message += $"|{Environment.NewLine}";

        return message;
    }

    void error()
    {
        m_WriteOut = false;
    }
    void printError(string errorMessage)
    {
        Console.WriteLine(errorMessage);
    }
}
