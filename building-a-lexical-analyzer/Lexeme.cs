using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace building_a_lexical_analyzer
{
    public class Lexeme
    {
        public string Value { get; set; }
        public string Type { get; set; }

        public override string ToString() => $"{Value,-10} {Type}";
    }
}
