using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class CompilerErrors
{
    public static void SystaxError(Token line)
    {
        Console.WriteLine($"Systax error on line {line.line}");
        ExitOut();
    }
    
    public static void error_expected(Token line, string msg)
    {
        Console.WriteLine($"Expected {msg} on line {line.line}");
        ExitOut();
    }
    public static void error_expected(Token line, TokenType msg)
    {
        Console.WriteLine($"Expected {msg} on line {line.line}");
        ExitOut();
    }

    public static void error_VariableIsConst(Token line, Var var)
    {
        Console.WriteLine($"{var.m_Name} is const and can't be reassigned on line {line.line}");
        ExitOut();
    }

    static void ExitOut()
    {
        Environment.Exit(1);
    }
}
