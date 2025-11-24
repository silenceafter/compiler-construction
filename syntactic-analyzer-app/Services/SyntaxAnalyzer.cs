using syntactic_analyzer_app.Models;
using System;
using System.Collections.Generic;

namespace syntactic_analyzer_app.Services
{
    public class SyntaxAnalyzer
    {
        private readonly SymbolTables symbolTables;
        private readonly Dictionary<(string nonterminal, string terminal), string> ll1Table;

        public SyntaxAnalyzer(SymbolTables symbolTables)
        {
            this.symbolTables = symbolTables;
            ll1Table = InitializeLL1Table();
        }

        public class SyntaxResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public List<string> ParseSteps { get; set; } = new List<string>();
            public string ErrorPosition { get; set; }
        }

        public SyntaxResult Analyze(List<(int classCode, int index)> tokens)
        {
            var result = new SyntaxResult();
            var stack = new Stack<string>();
            var parseSteps = new List<string>();

            try
            {
                // Начальный символ — Stmt (т.к. анализируем отдельный оператор, а не Program)
                stack.Push("Stmt");

                int position = 0;

                while (stack.Count > 0 && position < tokens.Count)
                {
                    string top = stack.Pop();

                    string tokenName = GetTokenName(tokens[position]);

                    if (IsTerminal(top))
                    {
                        // Совпадение терминала
                        if (top == tokenName)
                        {
                            parseSteps.Add($"Совпадение: {top} == {tokenName}");
                            position++;
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = $"Ожидалось '{top}', получено '{tokenName}'";
                            result.ErrorPosition = $"Позиция {position}";
                            return result;
                        }
                    }
                    else
                    {
                        // Нетерминал — ищем правило в таблице
                        if (ll1Table.TryGetValue((top, tokenName), out string rule))
                        {
                            parseSteps.Add($"Применяем: {rule}");
                            // Разбор и вставка в стек в обратном порядке
                            if (rule.Contains("->"))
                            {
                                var parts = rule.Split(new[] { "->" }, 2, StringSplitOptions.None);
                                var right = parts[1].Trim();
                                var symbols = right.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                                for (int i = symbols.Length - 1; i >= 0; i--)
                                {
                                    string sym = symbols[i];
                                    if (sym != "ε" && sym != "")
                                        stack.Push(sym);
                                }
                            }
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = $"Нет правила для {top} при входе {tokenName}";
                            result.ErrorPosition = $"Позиция {position}";
                            return result;
                        }
                    }
                }

                if (stack.Count == 0 && position == tokens.Count)
                {
                    result.Success = true;
                    result.Message = "Синтаксический анализ завершён успешно.";
                    result.ParseSteps = parseSteps;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Разбор не завершён";
                    result.ErrorPosition = $"Осталось в стеке: {stack.Count} элементов";
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ошибка в синтаксическом анализаторе: {ex.Message}";
                return result;
            }
        }

        private bool IsTerminal(string symbol)
        {
            return new HashSet<string>
            {
                "for", "to", "do", ":=", ";", "+", "(", ")", "<", "id", "literal"
            }.Contains(symbol);
        }

        private string GetTokenName((int classCode, int index) token)
        {
            return token.classCode switch
            {
                1 => GetKeyword(token.index),
                2 => GetSeparator(token.index),
                3 => "literal",
                4 => "id",
                _ => "unknown"
            };
        }

        private string GetKeyword(int index)
        {
            if (index >= 1 && index <= symbolTables.Keywords.Count)
                return symbolTables.Keywords[index - 1];
            return "unknown";
        }

        private string GetSeparator(int index)
        {
            if (index >= 1 && index <= symbolTables.Separators.Count)
                return symbolTables.Separators[index - 1];
            return "unknown";
        }

        private Dictionary<(string nonterminal, string terminal), string> InitializeLL1Table()
        {
            return new Dictionary<(string, string), string>
            {
                // Stmt
                { ("Stmt", "for"), "Stmt -> ForStmt" },
                { ("Stmt", "id"), "Stmt -> AssignStmt" },

                // ForStmt
                { ("ForStmt", "for"), "ForStmt -> for id := Expr to Expr do Stmt" },

                // AssignStmt
                { ("AssignStmt", "id"), "AssignStmt -> id := Expr ;" },

                // Expr
                { ("Expr", "id"), "Expr -> Term ExprTail" },
                { ("Expr", "literal"), "Expr -> Term ExprTail" },
                { ("Expr", "("), "Expr -> Term ExprTail" },

                // Term
                { ("Term", "id"), "Term -> Factor TermTail" },
                { ("Term", "literal"), "Term -> Factor TermTail" },
                { ("Term", "("), "Term -> Factor TermTail" },

                // Factor
                { ("Factor", "id"), "Factor -> id" },
                { ("Factor", "literal"), "Factor -> literal" },
                { ("Factor", "("), "Factor -> ( Expr )" },
                { ("Factor", "-"), "Factor -> - Factor" },

                // ExprTail
                { ("ExprTail", "<"), "ExprTail -> < Term ExprTail" },
                { ("ExprTail", "to"), "ExprTail -> ε" },
                { ("ExprTail", "do"), "ExprTail -> ε" },
                { ("ExprTail", ";"), "ExprTail -> ε" },
                { ("ExprTail", ")"), "ExprTail -> ε" },

                // TermTail
                { ("TermTail", "+"), "TermTail -> + Factor TermTail" },
                { ("TermTail", "<"), "TermTail -> ε" },
                { ("TermTail", "to"), "TermTail -> ε" },
                { ("TermTail", "do"), "TermTail -> ε" },
                { ("TermTail", ";"), "TermTail -> ε" },
                { ("TermTail", ")"), "TermTail -> ε" },
            };
        }
    }
}