using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace building_a_lexical_analyzer
{
    public class Lexer
    {
        private static readonly HashSet<string> Keywords = new HashSet<string>
        {
            "for", "to", "do"
        };

        private static readonly List<(string Pattern, string Type)> TokenPatterns = new List<(string, string)>
        {
            (@"\b(for|to|do)\b", "ключевое слово"),
            (@"[a-zA-Z][a-zA-Z0-9]*", "идентификатор"),
            (@"\d+", "литерал"),
            (@":=|\+|;", "разделитель/оператор"),
            (@"\s+", "пробел") // игнорируем
        };

        public static List<Lexeme> Analyze(string input)
        {
            var lexemes = new List<Lexeme>();
            var position = 0;

            while (position < input.Length)
            {
                var matched = false;

                // Сначала пропускаем пробелы
                var whitespaceMatch = Regex.Match(input.Substring(position), @"^\s+");
                if (whitespaceMatch.Success)
                {
                    position += whitespaceMatch.Length;
                    continue; // Переходим к следующему символу
                }

                foreach (var (pattern, tokenType) in TokenPatterns)
                {
                    var match = Regex.Match(input.Substring(position), "^" + pattern);
                    if (match.Success)
                    {
                        var value = match.Value;

                        if (tokenType != "пробел")
                        {
                            if (tokenType == "идентификатор" && Keywords.Contains(value))
                            {
                                lexemes.Add(new Lexeme { Value = value, Type = "ключевое слово" });
                            }
                            else
                            {
                                lexemes.Add(new Lexeme { Value = value, Type = tokenType });
                            }
                        }

                        position += match.Length;
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    throw new ArgumentException($"Недопустимый символ '{input[position]}' на позиции {position}");
                }
            }

            return lexemes;
        }
    }
}
