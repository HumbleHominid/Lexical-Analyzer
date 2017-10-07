using System;
using System.Collections.Generic;

namespace SmallCLexicalAnalyzer {
  class State {
    private Dictionary<char, State> transition = new Dictionary<char, State>();

    public string TokenName { get; private set; }

    public State() {
      TokenName = null;
    }

    public State(string tokenName) {
      TokenName = tokenName;
    }

    public bool AddDictionary(char c, State state) {
      try {
        transition.Add(c, state);

        return true;
      }
      catch(ArgumentException e) {
        return false;
      }
    }

    public State GetDestination(char c) {
      return transition.TryGetValue(c, out State dest) ? dest : null;
    }
  }
}
