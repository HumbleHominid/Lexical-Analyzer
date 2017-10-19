using System;
using System.Collections.Generic;

namespace SmallCLexicalAnalyzer {

  /// <summary>
  /// A <c>State</c> the <c>LexicalAnalyzer</c> can be in
  /// Contains following methods:
  /// <list>
  /// <item>
  /// <term>State</term>
  /// <description>Initializes a new instance of the
  /// <see cref="State{}"> class</description>
  /// </item>
  /// <item>
  /// <term>State</term>
  /// <description>Initializes a new instance of the
  /// <see cref="State{string}"> class</description>
  /// </item>
  /// <item>
  /// <term>AddToDictionary</term>
  /// <description>Adds a key/value pair to the dictionary</description>
  /// </item>
  /// <item>
  /// <term>GetDestination</term>
  /// <description>Gets the destination state for a given <c>char</c>
  /// </description>
  /// </item>
  /// </list>
  /// </summary>
  /// <remarks>
  /// The <c>LexicalAnalyzer</c> can have one to many states
  /// </remarks>
  public class State {

    /// <value>A private dictionary that represents the state transition for
    /// for this <c>State</c> with keys of type <c>char</c> and values of type
    /// <c>State</c></value>
    private Dictionary<char, State> transition = new Dictionary<char, State>();

    /// <value>Gets and private sets the <c>TokenName</c></value>
    public string TokenName { get; private set; }

    /// <summary>
    /// Initializer for a for a <c>State</c>
    /// </summary>
    /// <remarks>
    /// Sets <paramref name="TokenName"/> to <c>null</c>
    /// </remarks>
    public State() {
      TokenName = null;
    }

    /// <summary>
    /// Initializer for a <c>State</c>
    /// </summary>
    /// <remarks>
    /// Sets <paramref name="TokenName"/> to <paramref name="tokenName"/>
    /// </remarks>
    /// <param name="tokenName">A <c>string</c></param>
    public State(string tokenName) {
      TokenName = tokenName;
    }

    /// <summary>
    /// Adds the value <paramref name="state"/> to the dictionary with key
    /// <paramref name="c"/>
    /// </summary>
    /// <returns>
    /// A <c>bool</c> if the add was successful or not
    /// </returns>
    /// <param name="c">A <c>char</c> as the key</param>
    /// <param name="state">A <c>State</c> as the value</param>
    public bool AddToDictionary(char c, State state) {
      try {
        transition.Add(c, state);

        return true;
      }
      catch(ArgumentException e) {
        return false;
      }
    }

    /// <summary>
    /// Gets the destination state given a <c>char</c> <paramref name="c"/>
    /// </summary>
    /// <returns>
    /// A destination <c>State</c> or <c>null</c> if no transition is available
    /// </returns>
    /// <param name="c">A <c>char</c> as a key</param>
    public State GetDestination(char c) {
      return transition.TryGetValue(c, out State dest) ? dest : null;
    }
  }
}
