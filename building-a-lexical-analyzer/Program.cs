namespace building_a_lexical_analyzer
{
    public class Program
    {
        static void Main(string[] args)
        {
            string code = "for @ i := 1 to 10 @ do x := x + 1;";
            try
            {
                var tokens = Lexer.Analyze(code);

                Console.WriteLine($"{"Лексема",-10} Тип");
                Console.WriteLine(new string('-', 30));
                //
                foreach (var token in tokens)
                {
                    Console.WriteLine(token);
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
