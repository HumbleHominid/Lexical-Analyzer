namespace SmallCLexicalAnalyzer {

  /// <summary>
  /// The <c>Token</c> struct
  /// Contains following methods:
  /// <list type="bullet">
  /// <term>Token</term>
  /// <description>Constructs a new instance of the
  /// <see cref="Token(string)"/> class</description>
  /// <term>Token</term>
  /// <description>Constructs a new instance of the
  /// <see cref="Token(string, string)"/> class</description>
  /// </list>
  /// </summary>
  public struct Token {

    /// <value>Gets and private sets the <c>Lexeme</c> property</value>
    public string Lexeme { get; private set; }

    /// <value>Gets and private sets the <c>Name</c> property</value>
    public string Name { get; private set; }

    /// <value>Computed property for if the token is bad</value>
    public bool Bad { get => (Name == null); }

    /// <summary>
    /// Constructor for a <c>Token</c> using <paramref name="lexeme"/> where
    /// <see cref="Name"/> is null
    /// </summary>
    /// <param name="lexeme">A string literal</param>
    public Token(string lexeme) {
      Lexeme = lexeme;
      Name = null;
    }

    /// <summary>
    /// Constructor for a <c>Token</c> using <paramref name="lexeme"/> and
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
