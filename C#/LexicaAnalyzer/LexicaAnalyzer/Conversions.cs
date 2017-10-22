using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace SmallCLexicalAnalyzer {

  /// <summary>
  /// Static conversion class
  /// <list type="bullet">
  /// <item>
  /// <term>HexTokenToChar</term>
  /// <description>Converts a single hex string, e.g. `0x20` into its
  /// equivalent value as a character</description>
  /// <term>ToLiteral/term>
  /// <description>Converts the string into the escaped string literal
  //// representation</description>
  /// </item>
  /// </list>
  /// </summary>
  public static class Conversions {

    /// <summary>
    /// Converts a hex code in a given string into a character
    /// </summary>
    /// <returns>
    /// A <c>char</c> that was extracted from the string or <c>null</c> if it
    /// could not be converted
    /// </returns>
    public static char? HexTokenToChar(string s) {
      try {
        string stringHexValue = s.Replace("0x", "");
        int hexValue = Convert.ToInt32(stringHexValue, 16);

        return (char)hexValue;
      }
      catch (Exception e) {
        return null;
      }
    }

    /// <summary>
    /// Converts input string to literal
    /// </summary>
    /// <returns>
    /// string
    /// </returns>
    /// <param name="input">The input string to convert</param>
    public static string ToLiteral(string input) {
      using (var writer = new StringWriter()) {
        using (var provider = CodeDomProvider.CreateProvider("CSharp")) {
            provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);

          return writer.ToString();
        }
      }
    }
  }
}
