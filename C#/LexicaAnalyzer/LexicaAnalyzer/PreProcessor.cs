using System;
using System.Collections.Generic;
using System.IO;

namespace SmallCLexicalAnalyzer {

  /// <summary>
  /// The <c>PreProcessorResponse</c> struct
  /// Contains following methods:
  /// <list type="bullet">
  /// <item>
  /// <term>PreProcessorResponse</term>
  /// <description>Initializes a new instance of the
  /// <see cref="PreProcessorResponse(string, List{Token}, List{Token})"/>
  /// token</description>
  /// </item>
  /// </list>
  /// </summary>
  /// <remarks>
  /// This is what the <c>PreProcessor</c> returns when it is done processing
  /// </remarks>
  public struct PreProcessorResponse {

    /// <value>Public <c>string</c> representation of the program</value>
    public string Program { get; private set; }

    /// <value>Public <c>List</c> of all the warning tokens</value>
    public List<Token> Warnings { get; private set; }

    /// <value>Public <c>List</c> of all the error tokens</value>
    public List<Token> Errors { get; private set; }

    /// <summary>
    /// Constructor for a <c>PreProcessorResponse</c>
    /// </summary>
    /// <param name="program">A <c>string</c> representation of the
    /// program</param>
    /// <param name="warnings">A <c>List</c> of all the warnings the
    /// preprocessor found</param>
    /// <param name="errors">A <c>List</c> of all the errors the
    /// preprocessor found</param>
    public PreProcessorResponse (string program, List<Token> warnings,
        List<Token>  errors) {
      Program = program;
      Warnings = warnings;
      Errors = errors;
    }
  }

  /// <summary>
  /// A <c>PreProcessor</c> that uses a <c>StateMachine</c> to traverse a
  /// DFA
  /// <list type="bullet">
  /// <term>PreProcessor</term>
  /// <description>Initializes a new instance of the
  /// <see cref="PreProcessor(string)"/> class </description>
  /// <term>Process</term>
  /// <description>Processes the program</description>
  /// <term>OpenProgram</term>
  /// <description>Opens a new program</description>
  /// <term>CloseProgram</term>
  /// <description>Closes the currently open program</description>
  /// </list>
  /// </summary>
  class PreProcessor {

    /// <value>Private <c>StreamReader</c> of the currently open program.
    /// </value>
    private StreamReader programStream = null;

    /// <value>Private <c>StateMachine</c> used for the PreProcessor</value>
    private StateMachine stateMachine;

    /// <value>Public <c>bool</c> for if there is a token available.</value>
    public bool HasNextToken {
        get => programStream != null &&
                !programStream.EndOfStream;
    }

    /// <summary>
    /// Initializer for a <c>PreProcessor</c>
    /// </summary>
    /// <param name="tableFile">String of the table file to read</param>
    public PreProcessor(string tableFile) {
      stateMachine = new StateMachine(tableFile);
    }

    /// <summary>
    /// Process the program
    /// </summary>
    /// <returns><see name="PreProcessorResponse"/> after processing has been
    /// completed
    /// </returns>
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

    /// <summary>
    /// Gets the next token available from <see name="programStream"/>
    /// </summary>
    /// <returns>
    /// Returns the next <c>Token</c>
    /// </returns>
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
