namespace building_a_lexical_analyzer
{
    public class Program
    {
        static void Main(string[] args)
        {
            //string code = "for i := 1 to 10 do x := x + 1"; // "@ i := 1 to 10 @ do x := x + 1;"
            string code = "var\r\n  n, fact: integer;\r\nbegin\r\n  write('Введите число: ');\r\n  readln(n);\r\n  fact := 1;\r\n  for i := 2 to n do\r\n    fact := fact * i;\r\n  writeln('Факториал ', n, ' = ', fact);\r\nend.";

            var tables = new SymbolTables();
            var tokenCodes = Lexer.AnalyzeWithTables(code, tables);

            // Вывод
            // Таблица ключевых слов
            Console.WriteLine("Таблица ключевых слов:");
            for (int i = 0; i < tables.Keywords.Count; i++)
            {
                Console.WriteLine($"{i + 1,2}. {tables.Keywords[i]}");
            }

            // Таблица разделителей
            Console.WriteLine("\nТаблица разделителей:");
            for (int i = 0; i < tables.Separators.Count; i++)
            {
                Console.WriteLine($"{i + 1,2}. {tables.Separators[i]}");
            }   

            // Таблица идентификаторов
            Console.WriteLine("\nТаблица идентификаторов:");
            for (int i = 0; i < tables.Identifiers.Count; i++)
            {
                Console.WriteLine($"{i + 1,2}. {tables.Identifiers[i]}");
            }

            // Таблица литералов
            Console.WriteLine("\nТаблица литералов:");
            for (int i = 0; i < tables.Literals.Count; i++)
            {
                Console.WriteLine($"{i + 1,2}. {tables.Literals[i]}");
            }

            // Таблица стандартных символов
            Console.WriteLine("\nТаблица стандартных символов:");
            foreach (var (cls, idx) in tokenCodes)
            {
                Console.WriteLine($"{cls},{idx}");
            }
        }
    }
}
