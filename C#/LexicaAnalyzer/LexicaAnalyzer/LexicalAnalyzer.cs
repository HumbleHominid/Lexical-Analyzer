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

    /// <value>Private <c>string</c> of the program
    public string ProgramString { get; set; }

    /// <value>Private <c>Dictionary</c> of the keywords for the language</value>
    private Dictionary<string, string> keywords =
        new Dictionary<string, string>();

    /// <value>Private <c>Dictionary</c> that contains all the states</value>
    private Dictionary<string, State> states = new Dictionary<string, State>();

    /// <value>Public <c>bool</c> for if there is a token available.</value>
    public bool HasNextToken {
          get => (ProgramString.Length > 0);
        }

    /// <summary>
    /// Initializer for a <c>LexicalAnalyzer</c>
    /// </summary>
    public LexicalAnalyzer(string tableFile, string keywordsFile) {
      try {
        using (StreamReader sr = new StreamReader(tableFile)) {
          Dictionary<string, string[]> stateDictionary =
                                       new Dictionary<string, string[]>();

          while (!sr.EndOfStream) {
            string s = sr.ReadLine();
            string[] splitLine = s.Split(',');

            if (!stateDictionary.ContainsKey(splitLine[0])) {
              stateDictionary.Add(splitLine[0], splitLine);
            }
          }

          CreateStates(stateDictionary);

          MapStateTransitions(stateDictionary);

          ReadKeywords(keywordsFile);
        }
      }
      catch (Exception e) { }
    }

    private void CreateStates(Dictionary<string, string[]> stateDictionary) {
      foreach (KeyValuePair<string, string[]> entry in stateDictionary) {
        string key = entry.Key;

        if (entry.Value[0] != "Valid Chars") {
          State state = entry.Value[1] == "" ?
                        new State(key) :
                        new State(key, entry.Value[1]);

          states.Add(key, state);
        }
      }
    }

    private void MapStateTransitions(Dictionary<string, string[]> stateDictionary) {
      foreach (KeyValuePair<string, State> entry in states) {
        State state = entry.Value;
        string key = entry.Key;
        string[] stateChangeInfo = stateDictionary[key];

        for (int i = 2; i < stateChangeInfo.Length; i++) {
          string relatedState = stateChangeInfo[i];

          if (relatedState != "") {
            string inputHeader = stateDictionary["Valid Chars"][i];

            char? c = inputHeader.Contains("0x") ?
                HexTokenToChar(inputHeader) :
                inputHeader[0];

            if (c != null && states.ContainsKey(relatedState)) {
              state.AddToDictionary((char)c, states[relatedState]);
            }
          }
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
    private void ReadKeywords(string keywordsFile) {
      using (StreamReader sr = new StreamReader(keywordsFile)) {
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
      State startState = states["0"];
      State state = startState;
      State nextState;
      string lexeme = "";
      char nextChar;

      while (!state.Dead && HasNextToken) {
        if (state == startState) {
          lexeme = "";
        }

        nextChar = ProgramString[0];
        nextState = state.GetDestination(nextChar);

        if (nextState == null && state.Accepting) {
          return CreateGoodToken(lexeme, state.AcceptedName);
        }
        else if (nextState != null) {
          lexeme = lexeme + nextChar;
          state = nextState;
          ProgramString = ProgramString.Remove(0, 1);
        }
      }

      if (state.Accepting) {
        return CreateGoodToken(lexeme, state.AcceptedName);
      }
      else if (state == startState) {
        return null;
      }
      else {
        return new Token(lexeme);
      }
    }

    private Token CreateGoodToken(string lexeme, string name) {
      if (name == "Identifier" &&  keywords.ContainsKey(lexeme)) {
        return new Token(lexeme, keywords[lexeme]);
      }
      else {
        return new Token(lexeme, name);
      }
    }
  }
}
