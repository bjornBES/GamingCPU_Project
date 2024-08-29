using System;
using System.Collections.Generic;
using System.Text;

public class AssmeblerErrors : AssemblerSettings
{
    public Assembler m_assembler;

    public void E_InvalidInstruction(string instruction)
    {
        string errorMessage = "";

        errorMessage += STDErrorPrintStart();

        errorMessage += $"|\tBGC-ASL16 Error: Invalid instruction{Environment.NewLine}";
        errorMessage += $"|\t\t{instruction}{Environment.NewLine}";

        errorMessage += STDErrorPrintEnd();

        Error();

        PrintError(errorMessage);
    }
    public void E_InvalidRegister(RegisterInfo registerInfo)
    {
        string errorMessage = "";

        errorMessage += STDErrorPrintStart();

        errorMessage += $"|\tBGC-ASL16 Error: Invalid Register{Environment.NewLine}";
        errorMessage += $"|\t\t{registerInfo.m_Register}{Environment.NewLine}";

        errorMessage += STDErrorPrintEnd();

        Error();

        PrintError(errorMessage);
    }
    public void E_InvalidCPUFeature(CPUType NeededCPU)
    {
        string errorMessage = "";

        errorMessage += STDErrorPrintStart();

        errorMessage += $"|\tBGC-ASL16 Error: this feature is not available using the {CPUType}{Environment.NewLine}";
        errorMessage += $"|\tYou need to use something this or the veriant over the {NeededCPU} to get the feature. You can do this by putting .SETCPU \"{NeededCPU}\"";

        errorMessage += STDErrorPrintEnd();

        Error();

        PrintError(errorMessage);
    }

    string STDErrorPrintStart()
    {
        return $"|\t in file {m_assembler.m_file}:{m_assembler.lineNumber + 1}{Environment.NewLine}";
    }
    string STDErrorPrintEnd()
    {
        string message = "";

        message += $"|{Environment.NewLine}";
        
        message += $"|\t{m_assembler.m_Src[m_assembler.lineNumber - 1]}{Environment.NewLine}";
        message += $"|\t{m_assembler.m_Src[m_assembler.lineNumber]}{Environment.NewLine}";
        message += $"|\t{m_assembler.m_Src[m_assembler.lineNumber + 1]}{Environment.NewLine}";

        message += $"|{Environment.NewLine}";

        return message;
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
