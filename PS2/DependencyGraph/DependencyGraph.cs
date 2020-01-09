// Luke Ludlow
// CS 3500 
// 2019 September 

// NOTE: i use the One True Brace Style (1TBS), essentially K&R. 
// https://en.wikipedia.org/wiki/Indentation_style#K&R_style

// 1TBS style details:
// each function has its opening brace on the next line 
// however, the blocks inside a function have their opening braces on the same 
// line as their respective control statements.

// example:
// void Foo(string[] args)
// {
//   if (x) {
//     a();
//   } else {
//     b();
//     c();
//   }
// }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

  /// <summary>
  /// (s1,t1) is an ordered pair of strings
  /// t1 depends on s1; s1 must be evaluated before t1
  /// 
  /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
  /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
  /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
  /// set, and the element is already in the set, the set remains unchanged.
  /// 
  /// Given a DependencyGraph DG:
  /// 
  ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
  ///        (The set of things that depend on s)    
  ///        
  ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
  ///        (The set of things that s depends on) 
  ///
  /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
  ///     dependents("a") = {"b", "c"}
  ///     dependents("b") = {"d"}
  ///     dependents("c") = {}
  ///     dependents("d") = {"d"}
  ///     dependees("a") = {}
  ///     dependees("b") = {"a"}
  ///     dependees("c") = {"a"}
  ///     dependees("d") = {"b", "d"}
  /// </summary>
  public class DependencyGraph
  {

    private Dictionary<string, HashSet<string>> dependents;
    private Dictionary<string, HashSet<string>> dependees;

    /// <summary>
    /// Creates an empty DependencyGraph.
    /// </summary>
    public DependencyGraph()
    {
      this.dependents = new Dictionary<string, HashSet<string>>();
      this.dependees = new Dictionary<string, HashSet<string>>();
    }


    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// </summary>
    public int Size
    {
      get
      {
        int size = 0;
        foreach (KeyValuePair<string, HashSet<string>> entry in dependents) {
          size += entry.Value.Count;
        }
        return size;
      }
    }


    /// <summary>
    /// The size of dependees(s).
    /// invoke it like this:
    /// dg["a"]
    /// It should return the size of dependees("a")
    /// </summary>
    public int this[string s]
    {
      get
      {
        return GetDependees(s).Count();
      }
    }


    /// <summary>
    /// Reports whether dependents(s) is non-empty.
    /// </summary>
    public bool HasDependents(string s)
    {
      return dependents.ContainsKey(s);
    }


    /// <summary>
    /// Reports whether dependees(s) is non-empty.
    /// </summary>
    public bool HasDependees(string s)
    {
      return dependees.ContainsKey(s);
    }


    /// <summary>
    /// Enumerates dependents(s).
    /// </summary>
    public IEnumerable<string> GetDependents(string s)
    {
      return dependents.GetValue(s);
    }

    /// <summary>
    /// Enumerates dependees(s).
    /// </summary>
    public IEnumerable<string> GetDependees(string s)
    {
      return dependees.GetValue(s);
    }


    /// <summary>
    /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
    /// 
    /// <para>This should be thought of as:</para>   
    /// 
    ///   t depends on s
    ///
    /// </summary>
    /// <param name="s"> s must be evaluated first. T depends on S</param>
    /// <param name="t"> t cannot be evaluated until s is</param>        
    public void AddDependency(string s, string t)
    {
      if (dependents.ContainsKey(s)) {
        dependents[s].Add(t);
      } else {
        dependents.Add(s, new HashSet<string>());
        dependents[s].Add(t);
      }
      if (dependees.ContainsKey(t)) {
        dependees[t].Add(s);
      } else {
        dependees.Add(t, new HashSet<string>());
        dependees[t].Add(s);
      }
    }


    /// <summary>
    /// Removes the ordered pair (s,t), if it exists
    /// </summary>
    /// <param name="s"></param>
    /// <param name="t"></param>
    public void RemoveDependency(string s, string t)
    {
      if (dependents.ContainsKey(s)) {
        dependents[s].Remove(t);
        dependents.RemoveEntryIfValueIsEmpty(s);
      }
      if (dependees.ContainsKey(t)) {
        dependees[t].Remove(s);
        dependees.RemoveEntryIfValueIsEmpty(t);
      }
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (s,r).  Then, for each
    /// t in newDependents, adds the ordered pair (s,t).
    /// </summary>
    public void ReplaceDependents(string s, IEnumerable<string> newDependents)
    {
      foreach (string r in dependents.GetValue(s).Clone()) {
        RemoveDependency(s, r);
      }
      foreach (string t in newDependents) {
        AddDependency(s, t);
      }
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
    /// t in newDependees, adds the ordered pair (t,s).
    /// </summary>
    public void ReplaceDependees(string s, IEnumerable<string> newDependees)
    {
      foreach (string r in dependees.GetValue(s).Clone()) {
        RemoveDependency(r, s);
      }
      foreach (string t in newDependees) {
        AddDependency(t, s);
      }
    }

  }
}

