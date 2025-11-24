using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syntactic_analyzer_app.Models
{
    public class Lexeme
    {
        public string Value { get; set; }
        public string Type { get; set; }

        public override string ToString() => $"{Value,-10} {Type}";
    }
}