namespace SmallCLexicalAnalyzer {

  /// <summary>
  /// The <c>Token</c> struct
  /// </summary>
  /// <remarks>
  /// This is what the <c>LexicalAnalyer</c> returns when it is stepping
  /// through the program
  /// </remarks>
  struct Token {

    /// <value>Gets and sets the <c>Lexeme</c> property</value>
    public string Lexeme { get; private set; }

    /// <value>Gets and sets the <c>Name</c> property</value>
    public string Name { get; private set; }

    /// <summary>
    /// Creates a <c>Token</c> using <paramref name="lexem"/> and
    /// <paramref name="name"/>.
    /// </summary>
    /// <param name="lexeme">A string.</param>
    /// <param name="name">A string.</param>
    public Token(string lexeme, string name) {
      Lexeme = lexeme;
      Name = name;
    }
  }
}
