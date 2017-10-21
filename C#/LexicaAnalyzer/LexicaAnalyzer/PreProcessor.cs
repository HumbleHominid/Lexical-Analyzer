using System;
using System.Collections.Generic;
using System.IO;

namespace SmallCLexicalAnalyzer {
  class PreProcessor {
    /// <value>Private <c>StreamReader</c> of the currently open program.
    private StreamReader programStream = null;

    /// <value>Private <c>Dictionary</c> that contains all the states</value>
    private Dictionary<string, State> states = new Dictionary<string, State>();

    /// <value>Public <c>bool</c> for if there is a token available.</value>
    public bool HasNextToken {
        get => programStream != null &&
                !programStream.EndOfStream;
       }

    public PreProcessor(string table) {
      try {
        using (StreamReader sr = new StreamReader(table)) {
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

    public string Process() {
      string output = "";
      State startState = states["Start"];
      State state = startState;

      while (HasNextToken) {
        Token token = NextToken();

        if (!token.Bad) {
          switch (token.Name) {
            case "Add":
              output = output + token.Lexeme;

              break;
            case "Curly":
              // Raise warning

              break;
            case "Toss":
              // Raise Err

              break;
          }
        }
      }

      return output;
    }

    private Token NextToken() {
      State startState = states["Start"];
      State state = startState;
      string lexeme = "";

      while (!state.Dead && HasNextToken) {
        char nextChar = (char)programStream.Read();
        State nextState = state.GetDestination(nextChar);

        lexeme = lexeme + nextChar;

        if (nextState == null && state.Accepting) {
          return new Token(lexeme, state.AcceptedName);
        }
        else if (nextState != null) {
          state = nextState;
        }
      }

      return new Token(lexeme, state.AcceptedName);
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
  }
}
