using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace building_a_lexical_analyzer
{
    public class SymbolTables
    {
        // Таблица терминальных символов (ключевые слова + операторы/разделители)
        public List<string> Keywords { get; } = new List<string> { "var", "int", "boolean", "begin", "end", "for", "to", "do" };
        public List<string> Separators { get; } = new List<string> { ":", ":=", "+", ";", "*", "<", "<=" };

        // Таблица идентификаторов (динамически пополняется)
        public List<string> Identifiers { get; } = new List<string>();

        // Таблица литералов (чисел)
        public List<string> Literals { get; } = new List<string>();

        // Вспомогательные методы: поиск или добавление
        public (int classCode, int index) GetOrAddIdentifier(string id)
        {
            int idx = Identifiers.IndexOf(id);
            if (idx == -1)
            {
                Identifiers.Add(id);
                idx = Identifiers.Count - 1;
            }
            return (4, idx + 1); // класс 4 — идентификаторы; индекс с 1
        }

        public (int classCode, int index) GetOrAddLiteral(string lit)
        {
            int idx = Literals.IndexOf(lit);
            if (idx == -1)
            {
                Literals.Add(lit);
                idx = Literals.Count - 1;
            }
            return (3, idx + 1); // класс 3 — литералы
        }

        public (int classCode, int index)? GetKeywordOrSeparator(string lexeme)
        {
            int kwIdx = Keywords.IndexOf(lexeme);
            if (kwIdx != -1)
                return (1, kwIdx + 1); // класс 1 — ключевые слова

            int sepIdx = Separators.IndexOf(lexeme);
            if (sepIdx != -1)
                return (2, sepIdx + 1); // класс 2 — разделители/операторы

            return null; // не найдено — значит, идентификатор или литерал
        }
    }
}
