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

    /// <value>Private <c>Dictionary</c> that contains all the states</value>
    private Dictionary<string, State> states = new Dictionary<string, State>();

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
          Dictionary<string, string[]> stateDictionary =
                                       new Dictionary<string, string[]>();

          // Adds the table for lexical analyzer to stateTable
          while (!sr.EndOfStream) {
            string[] splitLine = sr.ReadLine().Split(',');

            if (!stateDictionary.ContainsKey(splitLine[0])) {
              stateDictionary.Add(splitLine[0], splitLine);
            }
          }

          // Creates each state and adds it to the dictionary
          foreach (KeyValuePair<string, string[]> entry in stateDictionary) {
            string key = entry.Key;

            if (key != "Valid Chars") {
              State state = key.Equals("") ?
                            new State() :
                            new State(entry.Value[1]);

              states.Add(key, state);
            }
          }

          // Maps the states to transition states
          foreach (KeyValuePair<string, State> entry in states) {
            State state = entry.Value;
            string key = entry.Key;
            string[] stateChangeInfo = stateDictionary[key];

            for (int i = 2; i < stateChangeInfo.Length; i++) {
              string relatedState = stateChangeInfo[i];

              if (relatedState != null && !relatedState.Equals("")) {
                string inputValue = stateDictionary["Valid Chars"][i];
                char? c = inputValue.Contains("0x") ?
                          HexTokenToChar(inputValue) :
                          inputValue[0];

                if (c != null) {
                  state.AddToDictionary((char)c, states[relatedState]);
                }
              }
            }
          }
        }
      }
      catch (Exception e) { }
    }

    /// <summary>
    /// Converts a hex code in a given string into a character
    /// </summary>
    /// <returns>
    /// A <c>char</c> that was extracted from the string or <c>null</c> if it
    /// could not be converted
    /// </returns>
    private void ReadKeywords() {
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

    /// <summary>
    /// Converts a hex code in a given string into a character
    /// </summary>
    /// <returns>
    /// A <c>char</c> that was extracted from the string or <c>null</c> if it
    /// could not be converted
    /// </returns>
    private char? HexTokenToChar(string s) {
      try {
        string stringHexValue = s.Replace("0x", "");
        int hexValue = Convert.ToInt32(stringHexValue, 16);

        return (char)hexValue;
      }
      catch (Exception e) {
        return null;
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
      // states["0"] is the starting state
      State state = states["0"];
      string lexeme = "";

      while (state != null && HasNextToken) {
        State nextState = null;

        nextState = state.GetDestination((char)programStream.Peek());

        if (nextState != null) {
          char nextChar = (char)programStream.Read();

          if (nextState.Dead) {
            return null;
          }
          else if (nextState.Start) {
            lexeme = "";
          }
          else {
            lexeme = lexeme + nextChar;
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
