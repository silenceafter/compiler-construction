using Microsoft.AspNetCore.Mvc;
using syntactic_analyzer_app.Models;
using syntactic_analyzer_app.Services;

namespace syntactic_analyzer_app.Controllers
{
    public class CompilerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Analyze(string code)
        {
            try
            {
                var tables = new SymbolTables();
                var tokens = Lexer.AnalyzeWithTables(code, tables);
                var syntaxAnalyzer = new SyntaxAnalyzer(tables);
                var syntaxResult = syntaxAnalyzer.Analyze(tokens);

                ViewBag.Code = code;
                ViewBag.Tokens = tokens;
                ViewBag.SymbolTables = tables;
                ViewBag.SyntaxResult = syntaxResult;

                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Index");
            }
        }
    }
}