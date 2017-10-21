namespace SmallCLexicalAnalyzer {

  /// <summary>
  /// The <c>Token</c> struct
  /// Contains following methods:
  /// <list type="bullet">
  /// <item>
  /// <term>Token</term>
  /// <description>Initializes a new instance of the
  /// <see cref="Token(string, string)"/> class</description>
  /// <term>Token</term>
  /// <description>Initializes a new instance of the
  /// <see cref="Token(string)"/> class</description>
  /// </item>
  /// </list>
  /// </summary>
  /// <remarks>
  /// This is what the <c>LexicalAnalyer</c> returns when it is stepping
  /// through the program
  /// </remarks>
  public struct Token {

    /// <value>Gets and private sets the <c>Lexeme</c> property</value>
    public string Lexeme { get; private set; }

    /// <value>Gets and private sets the <c>Name</c> property</value>
    public string Name { get; private set; }

    /// <value>Computed property for if the token is bad</value>
    public bool Bad { get => (Name == null); }

    /// <summary>
    /// Initializer for a <c>Token</c> using <paramref name="lexeme"/> where
    /// <see cref="Name"/> is null
    /// </summary>
    /// <param name="lexeme">A string literal</param>
    public Token(string lexeme) {
      Lexeme = lexeme;
      Name = null;
    }

    /// <summary>
    /// Initializer for a <c>Token</c> using <paramref name="lexeme"/> and
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
