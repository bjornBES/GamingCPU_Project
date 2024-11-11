using HLCLCompiler.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class CompilerErrors
{
    public static void SystaxError(Token line)
    {
        Console.WriteLine($"Systax error on line {line.m_File}:{line.m_Line}");
        exitOut();
    }
    
    public static void Error_expected(Token line, string msg)
    {
        Console.WriteLine($"Expected {msg} token got {line.Kind} {line.m_File}:{line.m_Line}");
        exitOut();
    }
    public static void Error_expected(Token line, TokenKind msg)
    {
        Console.WriteLine($"Expected {msg} token got {line.Kind} {line.m_File}:{line.m_Line}");
        exitOut();
    }

    static void exitOut()
    {
        Environment.Exit(1);
    }
}
