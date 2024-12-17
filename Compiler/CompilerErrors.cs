using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class CompilerErrors
{
    public static void SystaxError(Token line)
    {
        CompilerMessages.AddMessage($"Systax error", MessageType.Error, line);
        exitOut();
    }
    
    public static void Error_expected(Token line, string LastLineNumber, string msg)
    {
        line.m_SrcLineNumbers = LastLineNumber;
        CompilerMessages.AddMessage($"Expected {msg}", MessageType.Error, line);
        exitOut();
    }
    public static void Error_expected(Token line, string LastLineNumber, TokenType msg)
    {
        line.m_SrcLineNumbers = LastLineNumber;
        CompilerMessages.AddMessage($"Expected {msg}", MessageType.Error, line);
        exitOut();
    }

    public static void Error_VariableIsConst(Token line, string LastLineNumber, Var var)
    {
        line.m_SrcLineNumbers = LastLineNumber;
        Console.WriteLine($"{var.m_Name} is const and can't be reassigned on line {line.m_Line} in {line.m_File}");
        exitOut();
    }

    public static void Error_StmtNotFound(Token line, string LastLineNumber)
    {
        line.m_SrcLineNumbers = LastLineNumber;
        CompilerMessages.AddMessage($"on stmt found", MessageType.Error, line);
        exitOut();
    }

    static void exitOut()
    {
        CompilerSettings.m_DoWriteOut = false;
    }
}
