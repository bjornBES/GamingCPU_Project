using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class CompilerErrors
{
    public static void SystaxError(Token line)
    {
        Console.WriteLine($"Systax error on line {line.m_Line}");
        exitOut();
    }
    
    public static void Error_expected(Token line, string msg)
    {
        Console.WriteLine($"Expected {msg} on line {line.m_Line}");
        exitOut();
    }
    public static void Error_expected(Token line, TokenType msg)
    {
        Console.WriteLine($"Expected {msg} on line {line.m_Line}");
        exitOut();
    }

    public static void Error_VariableIsConst(Token line, Var var)
    {
        Console.WriteLine($"{var.m_Name} is const and can't be reassigned on line {line.m_Line}");
        exitOut();
    }

    static void exitOut()
    {
        Environment.Exit(1);
    }
}
