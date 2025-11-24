using syntactic_analyzer_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace syntactic_analyzer_app.Services
{
    public class Lexer
    {
        private static readonly HashSet<string> Keywords = new HashSet<string>
    {
        "for", "to", "do"
    };

        public static List<Lexeme> Analyze(string input)
        {
            var lexemes = new List<Lexeme>();
            int position = 0;

            while (position < input.Length)
            {
                char ch = input[position];

                // Пропускаем пробелы и табуляцию
                if (char.IsWhiteSpace(ch))
                {
                    position++;
                    continue;
                }

                // Начинаем распознавание новой лексемы
                int start = position;
                string value = "";
                string type = "";

                // Состояние автомата: 0 — начальное
                if (IsLetter(ch))
                {
                    // Состояние: идентификатор или ключевое слово
                    value += ch;
                    position++;

                    while (position < input.Length && (IsLetterOrDigit(input[position])))
                    {
                        value += input[position];
                        position++;
                    }

                    if (Keywords.Contains(value))
                        type = "ключевое слово";
                    else
                        type = "идентификатор";
                }
                else if (char.IsDigit(ch))
                {
                    // Состояние: целое число
                    value += ch;
                    position++;

                    while (position < input.Length && char.IsDigit(input[position]))
                    {
                        value += input[position];
                        position++;
                    }

                    type = "литерал";
                }
                else if (ch == ':')
                {
                    // Проверяем := 
                    if (position + 1 < input.Length && input[position + 1] == '=')
                    {
                        value = ":=";
                        type = "разделитель/оператор";
                        position += 2;
                    }
                    else
                    {
                        throw new ArgumentException($"Недопустимый символ ':' на позиции {position}");
                    }
                }
                else if (ch == '+' || ch == ';')
                {
                    value = ch.ToString();
                    type = "разделитель/оператор";
                    position++;
                }
                else
                {
                    // Недопустимый символ
                    throw new ArgumentException($"Недопустимый символ '{ch}' на позиции {position}");
                }

                lexemes.Add(new Lexeme { Value = value, Type = type });
            }

            return lexemes;
        }

        public static List<(int classCode, int index)> AnalyzeWithTables(string input, SymbolTables tables)
        {
            var tokens = new List<(int classCode, int index)>();
            int position = 0;

            while (position < input.Length)
            {
                char ch = input[position];
                if (char.IsWhiteSpace(ch))
                {
                    position++;
                    continue;
                }

                string value = "";

                // 👇👇👇 Вот что вы должны вставить вместо комментария 👇👇👇
                if (IsLetter(ch))
                {
                    value += ch;
                    position++;
                    while (position < input.Length && IsLetterOrDigit(input[position]))
                    {
                        value += input[position];
                        position++;
                    }
                }
                else if (char.IsDigit(ch))
                {
                    value += ch;
                    position++;
                    while (position < input.Length && char.IsDigit(input[position]))
                    {
                        value += input[position];
                        position++;
                    }
                }
                else if (ch == ':')
                {
                    if (position + 1 < input.Length && input[position + 1] == '=')
                    {
                        value = ":=";
                        position += 2;
                    }
                    else
                    {
                        throw new ArgumentException($"Недопустимый символ ':' на позиции {position}");
                    }
                }
                else if (ch == '+' || ch == ';')
                {
                    value = ch.ToString();
                    position++;
                }
                else
                {
                    throw new ArgumentException($"Недопустимый символ '{ch}' на позиции {position}");
                }
                // 👆👆👆 Конец вставки 👆👆👆

                // Теперь классификация через таблицы:
                var code = tables.GetKeywordOrSeparator(value);
                if (code.HasValue)
                {
                    tokens.Add(code.Value);
                }
                else if (char.IsLetter(value[0]))
                {
                    tokens.Add(tables.GetOrAddIdentifier(value));
                }
                else if (char.IsDigit(value[0]))
                {
                    tokens.Add(tables.GetOrAddLiteral(value));
                }
                else
                {
                    throw new ArgumentException($"Недопустимая лексема '{value}'");
                }
            }

            return tokens;
        }

        private static bool IsLetter(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        private static bool IsLetterOrDigit(char c) => IsLetter(c) || char.IsDigit(c);
    }
}