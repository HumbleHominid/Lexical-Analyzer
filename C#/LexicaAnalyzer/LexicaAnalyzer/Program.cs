using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallCLexicalAnalyzer {
  class Program {
    static void Main(string[] args) {
      LexicalAnalyzer lex = LexicalAnalyzer.Instance;

      lex.LoadProgram("test.txt");

      Token? nextToken = null;
      string userInput = "";
      bool stopAnalysis = false;

      do {
        nextToken = lex.NextToken();

        if (nextToken != null) {

          Token token = (Token)nextToken;

          Console.WriteLine($"Lexeme: {token.Lexeme,-10} Name: {token.Name}");

          userInput = Console.ReadLine();

          if (userInput == "q") {
            Console.WriteLine("Are you sure you want to quit? (y/n)");

            stopAnalysis = UserConfirmation();
          }
          else if (userInput == "r") {

          }
        }
      } while (lex.HasNextToken && !stopAnalysis);
    }

    private static bool UserConfirmation() {
      string userResponse = null;

      do {
        if (userResponse != null) {
          Console.WriteLine("Please answer with (y/n).");
        }

        userResponse = Console.ReadLine().ToLower();
      } while (userResponse != "y" && userResponse != "n");

      return (userResponse == "y");
    }
  }
}
