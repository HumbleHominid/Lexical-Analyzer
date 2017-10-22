using System;
using System.Collections.Generic;
using System.IO;

namespace SmallCLexicalAnalyzer {
  public struct PreProcessorResponse {
    public string Program { get; private set; }

    public List<Token> Warnings { get; private set; }

    public List<Token> Errors { get; private set; }

    public PreProcessorResponse(string program,
        List<Token> warnings, List<Token>  errors) {
      Program = program;
      Warnings = warnings;
      Errors = errors;
    }
  }

  class PreProcessor {
    /// <value>Private <c>StreamReader</c> of the currently open program.
    private StreamReader programStream = null;

    /// <value>Private <c>Dictionary</c> that contains all the states</value>
    private Dictionary<string, State> states = new Dictionary<string, State>();

    private StateMachine stateMachine;

    /// <value>Public <c>bool</c> for if there is a token available.</value>
    public bool HasNextToken {
        get => programStream != null &&
                !programStream.EndOfStream;
        }

    public PreProcessor(string stateTable) {
      stateMachine = new StateMachine(stateTable);
    }

    public PreProcessorResponse Process() {
      string output = "";
      State startState = stateMachine.States["Start"];
      State state = startState;
      List<Token> warnings = new List<Token>();
      List<Token> errors = new List<Token>();

      while (HasNextToken) {
        Token token = NextToken();

        if (!token.Bad) {
          switch (token.Name) {
            case "Add":
              output = output + token.Lexeme;

              break;
            case "Brace":
              warnings.Add(token);

              break;
          }
        }
        else {
          errors.Add(token);
        }
      }

      return new PreProcessorResponse(output, warnings, errors);
    }

    private Token NextToken() {
      State startState = stateMachine.States["Start"];
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
  }
}
