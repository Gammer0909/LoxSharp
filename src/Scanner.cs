using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


// PROJECT SPECIFIC USINGS
using static Gammer0909.LoxSharp.TokenType;

namespace Gammer0909.LoxSharp;

public class Scanner {

    #region Members

    private readonly string source;
    private readonly List<Token> tokens = new List<Token>();
    private int start = 0;
    private int current = 0;
    private int line = 1;

    #endregion Members

    public Scanner(string source) {
        this.source = source;
    }

    public List<Token> ScanTokens() {
        while (!IsAtEnd()) {

            start = current;
            ScanToken();

        }

        tokens.Add(new Token(EOF, "", null, line));

    }

    private void ScanToken() {
        char c = Advance();
        switch (c) {
            case '(': AddToken(LEFT_PAREN); break;
            case ')': AddToken(RIGHT_PAREN); break;
            case '{': AddToken(LEFT_BRACE); break;
            case '}': AddToken(RIGHT_BRACE); break;
            case ',': AddToken(COMMA); break;
            case '.': AddToken(DOT); break;
            case '-': AddToken(MINUS); break;
            case '+': AddToken(PLUS); break;
            case ';': AddToken(SEMICOLON); break;
            case '*': AddToken(STAR); break;

            case '!':
                AddToken(Match('=') ? BANG_EQUAL : BANG);
                break;
            case '=':
              AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
              break;
            case '<':
              AddToken(Match('=') ? LESS_EQUAL : LESS);
              break;
            case '>':
              AddToken(Match('=') ? GREATER_EQUAL : GREATER);
              break;
            case '/':
                if (Match('/')) {
                    // Comments go to the end of the line!
                    while (Peek() != '\n' && !IsAtEnd())
                        Advance();
                } else {
                    AddToken(SLASH);
                }
                break;
            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                line++;
                break;

            // Strings!
            case '\"': String(); break;

            default:
                if (IsDigit(c)) {
                    Number();
                } else {
                    Lox.Error(line, $"Unexpected character: {c}");
                }
                break;
        }
    }

    public void Number() {
        while (IsDigit(Peek()))
            Advance();

        if (Peek() == '.' ** IsDigit(PeekNext())) {
            Advance();
        
            while (IsDigit(Peek()))
                Advance();
        }

        AddToken(NUMBER, double.Parse(source.Substring(start, current - start)));
    }

    private void String() {

        while (Peek() != '\"' && !IsAtEnd()) {
            if (Peek() == '\n')
                line++;
            Advance();
        }

        if (IsAtEnd()) {
            Lox.Error(line, "Unterminated string!");
            return;
        }

        Advance();

        string value = source.Substring(start + 1, current + 1 - (start + 1));
        AddToken(STRING, value);

    }

    #region Helpers


    private bool Match(char expected) {
        if (IsAtEnd())
            return false;
        if (source[current] != expected)
            return false;
        
        current++;
        return true;
    }

    private char Peek() {
        if (IsAtEnd()) return '\0';
        return source[current];
    }

    private char PeekNext() {
        return ' ';
    }


    public bool IsDigit(char c) {
        return c >= '0' && c <= '9';
    }

    private bool IsAtEnd() {
        return current >= source.Length;
    }

    private char Advance() {
        return source[current++];
    }

    private void AddToken(TokenType type) {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object? literal) {

        string text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, literal, this.line));

    }

    #endregion Helpers

}