using System;
using System.Collections.Generic;
using System.IO;

namespace SmallCLexicalAnalyzer {

  /// <summary>
  /// The <c>StatMachine</c> class
  /// Contains following methods:
  /// <list type="bullet">
  /// <item>
  /// <term>StateMachine</term>
  /// <description>Initializes a new instance of the
  /// <see cref="StateMachine(string)"/> class</description>
  /// </item>
  /// </list>
  /// </summary>
  class StateMachine {

    /// <value>Public <c>Dictionary</c> that holds all of the states</value>
    public Dictionary<string, State> States { get; private set; } = new Dictionary<string, State>();

    /// <summary>
    /// Initializer for a <c>StateMacine</c> using
    /// <paramref name="stateTableFile"/> where
    /// </summary>
    /// <param name="stateTableFile">File to open containing the state table
    /// </param>
    public StateMachine(string stateTableFile) {
      try {
        using (StreamReader sr = new StreamReader(stateTableFile)) {
          Dictionary<string, string[]> rawStateData =
              new Dictionary<string, string[]>();

          while (!sr.EndOfStream) {
            string s = sr.ReadLine();
            string[] splitLine = s.Split(',');

            if (!rawStateData.ContainsKey(splitLine[0])) {
              rawStateData.Add(splitLine[0], splitLine);
            }
          }

          CreateStates(rawStateData);

          MapStateTransitions(rawStateData);
        }
      }
      catch (Exception e) { }
    }

    /// <summary>
    /// Populates the state dictionary <see name="States"/> with the data read
    /// from <paramref name="rawStateData"/>
    /// </summary>
    /// <param name="rawStateData">Dictionary of raw state data</param>
    private void CreateStates(Dictionary<string, string[]> rawStateData) {
      foreach (KeyValuePair<string, string[]> entry in rawStateData) {
        string key = entry.Key;

        if (entry.Value[0] != "Valid Chars") {
          State state = entry.Value[1] == "" ?
              new State(key) :
              new State(key, entry.Value[1]);

          States.Add(key, state);
        }
      }
    }

    /// <summary>
    /// Maps all the transitions for <see name="States"/> with the data read
    /// from <paramref name="rawStateData"/>
    /// </summary>
    /// <param name="rawStateData">Dictionary of raw state data</param>
    private void MapStateTransitions(Dictionary<string, string[]> rawStateData) {
      foreach (KeyValuePair<string, State> entry in States) {
        State state = entry.Value;
        string key = entry.Key;
        string[] stateChangeInfo = rawStateData[key];

        for (int i = 2; i < stateChangeInfo.Length; i++) {
          string relatedState = stateChangeInfo[i];

          if (relatedState != "") {
            string inputHeader = rawStateData["Valid Chars"][i];

            char? c = inputHeader.Contains("0x") ?
                Conversions.HexTokenToChar(inputHeader) :
                inputHeader[0];

            if (c != null && States.ContainsKey(relatedState)) {
              state.AddToDictionary((char)c, States[relatedState]);
            }
          }
        }
      }
    }
  }
}
