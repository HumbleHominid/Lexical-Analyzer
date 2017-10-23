﻿using System;
using System.IO;
using System.Collections.Generic;

namespace SmallCLexicalAnalyzer {

  /// <summary>
  /// A <c>LexicalAnalyzer</c> that uses a <c>StateMachine</c> to traverse a
  /// DFA
  /// <list type="bullet">
  /// <term>LexicalAnlyzer</term>
  /// <description>Initializes a new instance of the
  /// <see cref="LexicalAnalyzer(string, string)"/></description>
  /// <term>NextToken</term>
  /// <description>Gets the next token</description>
  /// </list>
  /// </summary>
  public class LexicalAnalyzer {

    /// <value>Private <c>string</c> of the program</value>
    public string ProgramString { private get; set; }

    /// <value>Private <c>Dictionary</c> of the keywords for the language
    /// </value>
    private Dictionary<string, string> keywords =
        new Dictionary<string, string>();

    /// <value>Private <c>StateMachine</c> for the <c>LexicalAnalyzer</c>
    /// </value>
    private StateMachine stateMachine;

    /// <value>Public <c>bool</c> for if there is a token available.</value>
    public bool HasNextToken {
        get => (ProgramString.Length > 0);
    }

    /// <summary>
    /// Initializer for a <c>LexicalAnalyzer</c>
    /// </summary>
    /// <param name="tableFile">String of the table file to read</param>
    /// <param name="keywordsFile">String of the keywords file to read</param>
    public LexicalAnalyzer(string tableFile, string keywordsFile) {
      stateMachine = new StateMachine(tableFile);

      ReadKeywords(keywordsFile);
    }

    /// <summary>
    /// Reads in all the keywords and adds them to <see name="keywords"/>
    /// </summary>
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
    /// Gets the next token available from <see name="ProgramString"/>
    /// </summary>
    /// <returns>
    /// Returns the next <c>Token</c> or <c>null</c> if there is no
    /// valid next toke
    /// </returns>
    public Token? NextToken() {

      // stateMachine.States["0"] is the starting state
      State startState = stateMachine.States["0"];
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

    /// <summary>
    /// Creates a good token by replacing identifiers that are keywords with
    /// their correct name
    /// </summary>
    /// <returns>
    /// The new good <c>Token</c>
    /// </returns>
    /// <param name="lexeme">The token's lexeme</param>
    /// <param name="name">The name of the token</param>
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
