using System;
using System.IO;

using Gammer0909.LoxSharp;

namespace Gammer0909.LoxSharp;

public class Lox {
    public static bool hadError = false;

    public static void Main(string[] args) {

        if (args.Length > 1) {
            Console.WriteLine("Usage: LoxSharp [script]");
            return;
        } else if (args.Length == 1) {
            try {
                RunFile(args[0]);
            } catch (IOException e) {
                Console.WriteLine(e.Message);
            }
            
        } else {
            RunPrompt();
        }

    }

    private static void RunFile(string path) {

        if (!Path.Exists(path))
            throw new IOException($"The file \"{path}\" does not exist!");

        string lines = File.ReadAllText(Path.GetFullPath(path));
        Run(lines);

        // There was an error!
        if (hadError)
            Environment.Exit(65);

    }

    private static void RunPrompt() {
        for (;;) {
            Console.Write("> ");
            string? line = Console.ReadLine();
            if (line == null || line == "")
                break;
            Run(line);
            hadError = false;
        }
    }

    private static void Run(string source) {

        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        // For Now, just Print the tokens
        foreach (var t in tokens) {
            Console.WriteLine(t.ToString());
        }

    }

    public static void Error(int line, string message) {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message) {
        Console.Error.WriteLine($"[Line {line}] Error{where}: {message}");
        hadError = true;
    }
}