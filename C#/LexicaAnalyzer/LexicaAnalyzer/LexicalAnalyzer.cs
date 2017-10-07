namespace SmallCLexicalAnalyzer {
  class LexicalAnalyzer {
    private string program;

    // instance variable called Instance that is created at first reference
    public static LexicalAnalyzer Instance { get; private set; } =
                  new LexicalAnalyzer();

    // private constructor for creating a singleton
    private LexicalAnalyzer() { }

    public Token NextToken() { }

    public SetProgram(string filename) { }

    private PreProcess() { }
  }
}
