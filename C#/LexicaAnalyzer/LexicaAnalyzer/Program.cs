using System;

namespace SmallCLexicalAnalyzer {
  class Program {
    static void Main(string[] args) {
      LexicalAnalyzer lex = LexicalAnalyzer.Instance;

      bool runAnalysis = true;

      DisplayCommandsMessage();

      while (runAnalysis) {
        string programPath = null;

        do {
          Console.Write("Enter a path to the program you want to analyze or q to quit> ");

          programPath = Console.ReadLine();

          if (programPath.ToLower().Equals("q")) {
            runAnalysis = false;

            break;
          }
        } while (!lex.OpenProgram(programPath));

        if (runAnalysis) {
          RunAnalysis();
        }
      }
    }

    /// TODO Add documentation
    private static void RunAnalysis() {
      LexicalAnalyzer lex = LexicalAnalyzer.Instance;

      Token? nextToken = null;
      string userInput = "";
      bool continueAnalysis = true;
      bool rushAnalysis = false;

      Console.WriteLine("Starting analysis...");

      while (lex.HasNextToken && continueAnalysis) {
        nextToken = lex.NextToken();

        if (nextToken != null) {
          Token token = (Token)nextToken;

          Console.WriteLine($"Lexeme: {token.Lexeme,-10}Name: {token.Name}");
        }
        else {
          Console.WriteLine("Invalid token");
        }

        userInput = rushAnalysis ? "" : Console.ReadLine();

        switch (userInput.ToLower()) {
          case "q":
            Console.Write("Are you sure you want to quit? (y/n)> ");

            continueAnalysis = !UserConfirmation();

            break;
          case "r":
            Console.Write("Are you sure you want to analyze the rest of the file? (y/n)> ");

            rushAnalysis = UserConfirmation();

            break;
          case "h":
            break;
        }
      }

      lex.CloseProgram();
    }

    /// TODO Add documentation
    private static bool UserConfirmation() {
      string userResponse = null;

      do {
        if (userResponse != null) {
          Console.Write("Please answer with (y/n)> ");
        }

        userResponse = Console.ReadLine().ToLower();
      } while (!userResponse.Equals("y") && !userResponse.Equals("n"));

      return (userResponse.Equals("y"));
    }

    /// TODO Add documentation
    private static void DisplayCommandsMessage() {
      string message = "";

      message = message + $"Lexical Analyzer Commands:\n";
      message = message + $"{'q',-5}Quit\n";
      message = message + $"{'r',-5}Read the rest of the analysis\n";
      message = message + $"{'h',-5}Help with commands\n";

      Console.Write(message);
    }
  }
}
