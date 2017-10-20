using System;
using System.IO;
using System.Collections.Generic;

namespace SmallCLexicalAnalyzer {

  /// <summary>
  /// A <c>LexicalAnalyzer</c> that uses <c>State</c> objects to traverse a
  /// DFA.
  /// <list type="bullet">
  /// <term>NextToken</term>
  /// <description>Gets the next token</description>
  /// <term>OpenProgram</term>
  /// <description>Opens a new program</description>
  /// <term>CloseProgram</term>
  /// <description>Closes the currently open program</description>
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

    /// <value>Private <c>StreamReader</c> of the currently open program.
    private StreamReader programStream = null;

    /// <value>Private <c>Dictionary</c> of the keywords for the language</value>
    private Dictionary<string, string> keywords =
            new Dictionary<string, string>();

    /// <value>Private <c>List</c> that contains all the states</value>
    private List<State> states = new List<State>();

    /// <value>Public <c>bool</c> for if there is a token available.</value>
    public bool HasNextToken { get => !programStream.EndOfStream; }

    /// <value>Private instance of the class <c>LexicalAnalyzer</c></value>
    /// <remarks>
    /// This creates an instance of itself to abide by the Singleton pattern
    /// </remarks>
    public static LexicalAnalyzer Instance { get; private set; } =
           new LexicalAnalyzer();

    /// <summary>
    /// Initializer for a <c>LexicalAnalyzer</c>
    /// </summary>
    /// <remarks>
    /// This is private to abide by the Singleton pattern
    /// </remarks>
    private LexicalAnalyzer() {
      try {
        using (StreamReader sr = new StreamReader("Lexical Analyzer Table.csv")) {
          List<string[]> table = new List<string[]>();

          while (!sr.EndOfStream) {
            table.Add(sr.ReadLine().Split(','));
          }

          for (int i = 1; i < table.Count; i++) {
            string tokenName = table[i][1];
            State state;

            state = tokenName.Equals("") ? new State() : new State(tokenName);

            states.Add(state);
          }

          for (int i = 0; i < states.Count; i++) {
            string[] stateChangeInfo = table[i + 1];

            for (int j = 2; j < stateChangeInfo.Length; j++) {
              string s = table[0][j];
              char c;
              int stateNumber;

              if (int.TryParse(stateChangeInfo[j], out stateNumber)) {
                c = s.Contains("0x") ?
                    (char)Convert.ToInt32(s.Replace("0x", ""), 16) :
                    s[0];

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
        Console.WriteLine($"Table could not be read: {e.Message}");
      }
    }

    /// <summary>
    /// Gets the next token available from <see name="programStream"/>
    /// </summary>
    /// <returns>
    /// Returns the next <c>Token</c> or <c>null</c> if there is no
    /// valid next token
    /// Recursively calls itself if a comment is found
    /// </returns>
    public Token? NextToken() {
      // states[0] is the starting state
      State state = states[0];
      string lexeme = "";

      while (state != null && HasNextToken) {
        State nextState = null;

        nextState = state.GetDestination((char)programStream.Peek());

        if (nextState != null) {
          char nextChar = (char)programStream.Read();
          // states[1] is the dead state
          if (nextState.Dead) {
            return null;
          }
          else if (nextState != states[0]) {
            lexeme = lexeme + nextChar;
          }
          else {
            lexeme = "";
          }

          state = nextState;
        }
        else {
          if (state.Accepting) {
            if (state.TokenName.Equals("Line Comment") ||
                state.TokenName.Equals("Block Comment")) {
              return NextToken();
            }
            else {
              return new Token(lexeme, state.TokenName);
            }
          }
          else {
            return null;
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Opens a program from <paramref name="filename"/>
    /// </summary>
    /// <returns>
    /// A <c>bool</c> for if the open was successful or not
    /// </returns>
    /// <param name="filename">A filename as a <c>string</c></param>
    public bool OpenProgram(string filename) {
      try {
        programStream = new StreamReader(filename);

        return true;
      }
      catch (Exception e) {
        Console.WriteLine($"File could not be read: {e.Message}");

        return false;
      }
    }

    /// <summary>
    /// Closes previous StreamReader stored in <see name="programStream"/> if
    /// it exists and sets it to null
    /// </summary>
    /// <returns>
    /// A <c>bool</c> for if the StreamReader was closed or not
    /// </returns>
    public bool CloseProgram() {
      if (programStream != null) {
        programStream.Close();

        programStream = null;

        return true;
      }

      return false;
    }
  }
}
