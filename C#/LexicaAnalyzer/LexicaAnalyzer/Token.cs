namespace SmallCLexicalAnalyzer {

  /// <summary>
  /// The <c>Token</c> struct
  /// Contains following methods:
  /// <list>
  /// <item>
  /// <term>Token</term>
  /// <description>Initializes a new instance of the
  /// <see cref="Token{string, string}"> class</description>
  /// </item>
  /// </list>
  /// </summary>
  /// <remarks>
  /// This is what the <c>LexicalAnalyer</c> returns when it is stepping
  /// through the program
  /// </remarks>
  struct Token {

    /// <value>Gets and private sets the <c>Lexeme</c> property</value>
    public string Lexeme { get; private set; }

    /// <value>Gets and private sets the <c>Name</c> property</value>
    public string Name { get; private set; }

    /// <summary>
    /// Initializer for a <c>Token</c> using <paramref name="lexem"/> and
    /// <paramref name="name"/>
    /// </summary>
    /// <param name="lexeme">A string literal</param>
    /// <param name="name">A string literal</param>
    public Token(string lexeme, string name) {
      Lexeme = lexeme;
      Name = name;
    }
  }
}
