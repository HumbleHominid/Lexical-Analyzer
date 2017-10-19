using System;
using System.IO;
using System.Collections.Generic;

namespace SmallCLexicalAnalyzer {

  /// <summary>
  /// A <c>LexicalAnalyzer</c> that uses <c>State</c> objects to traverse a
  /// DFA.
  /// <list>
  /// <term>NextToken<term>
  /// <description>Gets the next token</description>
  /// <term>LoadProgram</term>
  /// <description>Loads a new program</description>
  /// </list>
  /// </summary>
  /// <remarks>
  /// The <c>LexicalAnalyzer</c> class makes use of the Singleton design pattern
  /// so there will only ever be one instance of a <c>LexicalAnalyzer</c>. To
  /// access the methods use the following:
  /// <code>
  /// LexicalAnalyzer lexicalAnalyzer = LexicalAnalyzer.Instance
  /// </code>
  /// </remarks>
  public class LexicalAnalyzer {

    /// <value>Private <c>string</c> representation of the program</value>
    private List<string> program = new List<string>();

    /// <value>Private <c>Dictionary</c> of th keywords for the language<value>
    private Dictionary<string, string> keywords = new Dictionary<string, string>();

    /// <value>Private <c>int</c> that is the current column number</value>
    private int column;

    /// <value>Private <c>int</c> that is the current line number</value>
    private int lineNumber;

    /// <value>Private <c>List</c> that contains all the states</value>
    private List<State> states = new List<State>();

    /// <value>Public <c>bool</c> for if there is a token available.
    public bool HasNextToken { get => lineNumber < program.Count; }

    /// <value>Private instance of the class <c>LexicalAnalyzer</c></value>
    /// <remarks>
    /// This creates an instance of itself to abide by the Singleton pattern
    /// </remarks>
    public static LexicalAnalyzer Instance { get; private set; } = new LexicalAnalyzer();

    /// <summary>
    /// Initializer for a <c>LexicalAnalyzer</c>
    /// </summary>
    /// <remarks>
    /// This is private to abide by the Singleton pattern
    /// </remarks>
    private LexicalAnalyzer() {
      column = 0;
      lineNumber = 0;

      try {
        using (StreamReader sr = new StreamReader("Lexical Analyzer Table.csv")) {
          List<string[]> table = new List<string[]>();

          while (!sr.EndOfStream) {
            table.Add(sr.ReadLine().Split(','));
          }

          for (int i = 1; i < table.Count; i++) {
            string tokenName = table[i][1];

            if (tokenName != "") {
              states.Add(new State(tokenName));
            }
            else {
              states.Add(new State());
            }
          }

          for (int i = 0; i < states.Count; i++) {
            string[] stateChangeInfo = table[i + 1];

            for (int j = 2; j < stateChangeInfo.Length; j++) {
              string s = table[0][j];
              char c;

              int stateNumber;

              if (int.TryParse(stateChangeInfo[j], out stateNumber)) {
                if (s.Contains("0x")) {
                  int hexValue = Convert.ToInt32(s.Replace("0x", ""), 16);

                  c = (char)hexValue;
                }
                else {
                  c = s[0];
                }

                states[i].AddToDictionary(c, states[stateNumber]);
              }
            }
          }
        }

        using (StreamReader sr = new StreamReader("Keyword Table.csv")) {
          while (!sr.EndOfStream) {
            string[] splitLine = sr.ReadLine().Split(',');

            try {
              keywords.Add(splitLine[0], splitLine[1]);
            }
            catch(ArgumentException e) { }
          }
        }
      }
      catch (Exception e) {
        Console.WriteLine("Table could not be read.");
        Console.WriteLine(e.Message);
      }
    }

    /// <summary>
    /// Gets the next token available from <paramref name="program"/>
    ///
    /// </summary>
    /// <returns>
    /// Returns the next <c>Token</c> or <c>null</c> if there is no
    /// valid next token
    /// Recursively calls itself if a comment is found
    /// </returns>
    public Token? NextToken() {
      State state = states[0];

      string lexeme = "";
      string name = "";

      do {
        if (lineNumber >= program.Count) {
          break;
        }
        else if (program[lineNumber].Length == 0) {
          lineNumber = lineNumber + 1;

          continue;
        }

        char nextChar = program[lineNumber][column];

        state = state.GetDestination(nextChar);

        if (state != null) {
          if (state.TokenName == "Dead") {
            Console.WriteLine($"Syntax error at {lineNumber + 1}:{column + 1}");

            lexeme = "";
            name = "";

            state = states[0];
          }
          else if (state != states[0]) {
            lexeme = lexeme + nextChar;
            name = state.TokenName;
          }
          else {
            lexeme = "";
            name = "";
          }

          column = (column + 1) % program[lineNumber].Length;

          if (column == 0) {
            lineNumber = lineNumber + 1;
          }
        }
      } while (state != null);

      if (state == states[0]) {
        return null;
      }

      if (keywords.ContainsKey(lexeme)) {
        keywords.TryGetValue(lexeme, out name);
      }
      else if (name == "Line Comment" || name == "Block Comment") {
        Token? newToken = NextToken();

        if (newToken == null) {
          return null;
        }

        Token token = (Token)newToken;

        lexeme = token.Lexeme;
        name = token.Name;
      }

      return new Token(lexeme, name);
    }

    /// <summary>
    /// Loads a new program from <paramref name="filename"/> into
    /// <paramref name="program"/>
    /// </summary>
    /// <returns>
    /// A <c>bool</c> for if the load was successful or not
    /// </returns>
    /// <param name="filename">A filename as a <c>string</c></param>
    public bool LoadProgram(string filename) {
      program.Clear();

      try {
        using (StreamReader sr = new StreamReader(filename)) {
          while(!sr.EndOfStream) {
            program.Add($"{sr.ReadLine()}\r\n");
          }
        }

        return true;
      }
      catch (Exception e) {
        Console.WriteLine("File could not be read.");
        Console.WriteLine(e.Message);

        return false;
      }
    }
  }
}
