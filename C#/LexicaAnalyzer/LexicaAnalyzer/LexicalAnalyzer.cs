namespace SmallCLexicalAnalyzer {

  /// <summary>
  /// A <c>LexicalAnalyzer</c> that uses <c>State</c> objects to travers a
  /// DFA.
  /// <list>
  /// <term>NextToken<term>
  /// <descritpion>Gets the next token</description>
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
  class LexicalAnalyzer {

    /// <value>Private <c>string</c> representation of the program</value>
    private string program;

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
    private LexicalAnalyzer() { program = null; }

    /// <summary>
    /// Gets the next token available from <paramref name="program"/>
    /// </summary>
    /// <returns>
    /// Returns the next <c>Token</c> or <c>null</c> if there is no next token
    /// available
    /// </returns>
    public Token NextToken() { return null; }

    /// <summary>
    /// Loads a new program from <paramref name="filename"/> into
    /// <paramref name="program"/>
    /// </summary>
    /// <returns>
    /// A <c>bool</c> for if the load was successfull or not
    /// </returns>
    /// <param name="filename">A filename as a <c>string</c></param>
    public bool LoadProgram(string filename) { return false; }

    /// <summary>
    /// Pre-processes the program
    /// </summary>
    private void PreProcess() { }
  }
}
