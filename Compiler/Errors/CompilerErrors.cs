using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler;
using static Crayon.Output;

public static class CompilerErrors
{
    public static Dictionary<int, ErrorMessage> ErrorMessages = new Dictionary<int, ErrorMessage>()
    {
        { 0, new ErrorMessage("", 0, MessageType.Error)},
        { 1, new ErrorMessage("Expected {0}", 1, MessageType.Error)},
        { 2, new ErrorMessage("Can't use {0} when declaring a variabel", 1, MessageType.Error)},
        { 3, new ErrorMessage("Can't use a void type here", 0, MessageType.Error)}
    };

    public static void SystaxError(Token line)
    {
        // CompilerMessages.AddMessage($"Systax error", MessageType.Error, line);
        exitOut();
    }
    public static void Error_UseVoid(Token line, Parser parser)
    {
        line.m_SrcLineNumbers = parser.LineNumbers.Last();
        CompilerMessages.AddMessage(ErrorMessages[3].GetMessage(line, 3, parser));
        exitOut();
    }
    public static void Error_InvalidOperatorInUse(Token line, AssignmentOperators assignmentOperators, Parser parser)
    {
        line.m_SrcLineNumbers = parser.LineNumbers.Last();
        CompilerMessages.AddMessage(ErrorMessages[2].GetMessage(line, 2, parser, assignmentOperators));
        exitOut();
    }
    public static void Error_expected(Token line, string msg, Parser parser)
    {
        line.m_SrcLineNumbers = parser.LineNumbers.Last();
        CompilerMessages.AddMessage(ErrorMessages[1].GetMessage(line, 1, parser, msg));
        exitOut();
    }
    public static void Error_expected(Token line, TokenType msg, Parser parser)
    {
        line.m_SrcLineNumbers = parser.LineNumbers.Last();
        CompilerMessages.AddMessage(ErrorMessages[1].GetMessage(line, 1, parser, msg));
        exitOut();
    }

    public static void Error_VariableIsConst(Token line, Variabel var, Parser parser)
    {
        line.m_SrcLineNumbers = parser.LineNumbers.Last();
        Console.WriteLine($"{var.Name} is const and can't be reassigned on line {line.m_Line} in {line.m_File}");
        exitOut();
    }

    public static void Error_StmtNotFound(Token line, Parser parser)
    {
        line.m_SrcLineNumbers = parser.LineNumbers.Last();
        // CompilerMessages.AddMessage($"on stmt found", MessageType.Error, line);
        exitOut();
    }

    static void exitOut()
    {
        CompilerSettings.m_DoWriteOut = false;
    }
}

public class ErrorMessage
{
    public string Message;
    public int NumberOfArguments;
    public MessageType MessageType;

    public ErrorMessage(string message, int numberOfArguments, MessageType messageType)
    {
        Message = message;
        NumberOfArguments = numberOfArguments;
        MessageType = messageType;
    }

    public string GetMessage(Token line, int errorcode, Parser parser, params object[] args)
    {
        if (args.Length == 0 || args.Length != NumberOfArguments)
        {
            throw new ArgumentException("");
        }
        string result = "";

        if (parser.inFunction)
        {
            result += $"{Path.GetFileName(line.m_File)}: In function ‘{parser.functionNames.Last()}’:" + Environment.NewLine;
        }
        result += $"{Path.GetFileName(line.m_File)}:{line.m_Line + 1}: ";

        if (CompilerSettings.m_DoPrintToSTDOut)
        {
            if (parser.inFunction)
            {
                Console.Write(Bold($"{Path.GetFileName(line.m_File)}: ") + "In function " + Bold($"‘{parser.functionNames.Last()}’") + ":" + Environment.NewLine);
            }
            Console.Write(Bold($"{Path.GetFileName(line.m_File)}:{line.m_Line + 1}: "));
        }

        if (MessageType == MessageType.Error)
        {
            if (CompilerSettings.m_DoPrintToSTDOut)
            {
                Console.Write(Bold(Red($"error: ")));
            }
            result += $"error: ";
        }
        else if (MessageType == MessageType.Warning)
        {
            if (CompilerSettings.m_DoPrintToSTDOut)
            {
                Console.Write(Yellow($"warning: "));
            }
            result += "warning: ";
        }

        result += $"code {errorcode}: ";
        result += Message;

        if (CompilerSettings.m_DoPrintToSTDOut)
        {
            Console.Write($"code {errorcode}: ");
        }
        string msg = Message;
        for (int i = 0; i < NumberOfArguments; i++)
        {
            result = result.Replace($"{{{i}}}", args[i].ToString());
            msg = msg.Replace($"{{{i}}}", args[i].ToString());
        }

        if (CompilerSettings.m_DoPrintToSTDOut)
        {
            Console.WriteLine($"{msg}{Environment.NewLine}");
        }

        return result + Environment.NewLine;
    }
}