// Luke Ludlow
// CS 3500 
// 2019 September 

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


namespace DependencyGraphTests
{
  /// <summary>
  /// test method names follow this structure:
  /// [MethodName_StateUnderTest_ExpectedBehavior]
  /// </summary>
  [TestClass()]
  public class DependencyGraphTests
  {


    [TestMethod(), Timeout(5000)]
    public void Constructor_EmptyConstructor_ShouldContainNothing()
    {
      DependencyGraph dg = new DependencyGraph();
      Assert.AreEqual(0, dg.Size);
    }

    [TestMethod(), Timeout(5000)]
    public void Constructor_MultipleInstances_ShouldBePossibleToHaveMultipleAtSameTime()
    {
      DependencyGraph dg1 = new DependencyGraph();
      DependencyGraph dg2 = new DependencyGraph();
      dg1.AddDependency("x", "y");
      Assert.AreEqual(1, dg1.Size);
      Assert.AreEqual(0, dg2.Size);
    }




    [TestMethod(), Timeout(5000)]
    public void Size_ZeroDependencies_ShouldReturnZero()
    {
      DependencyGraph dg = new DependencyGraph();
      Assert.AreEqual(0, dg.Size);
    }

    [TestMethod(), Timeout(5000)]
    public void Size_DependenciesAddedThenRemoved_ShouldReturnZero()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.AreEqual(1, dg.Size);
      dg.RemoveDependency("a", "b");
      Assert.AreEqual(0, dg.Size);
    }

    [TestMethod(), Timeout(5000)]
    public void Size_DuplicateDependencies_ShouldReturnOne()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("a", "b");
      dg.AddDependency("a", "b");
      Assert.AreEqual(1, dg.Size);
    }

    [TestMethod(), Timeout(5000)]
    public void Size_MultipleDependencies_ShouldReturnCount()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("a", "c");
      dg.AddDependency("c", "b");
      dg.AddDependency("b", "d");
      Assert.AreEqual(4, dg.Size);
    }




    [TestMethod(), Timeout(5000)]
    public void DependeesSize_OneDependee_ShouldReturnOne()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      // AddDependency(a, b) 
      // means "b depends on a"
      // = "a is a dependee of b"
      // = "b is a dependent of a"
      Assert.AreEqual(0, dg["a"]);
      Assert.AreEqual(1, dg["b"]);
    }

    [TestMethod(), Timeout(5000)]
    public void DependeesSize_MultipleDependees_ShouldReturnCount()
    {
      DependencyGraph dg = new DependencyGraph();
      // "b depends on a"
      // "a is a dependee of b"
      dg.AddDependency("a", "b");
      // "b depends on x"
      // "x is a dependee of b"
      dg.AddDependency("x", "b");
      // "b depends on z"
      // "z is a dependee of b"
      dg.AddDependency("z", "b");
      Assert.AreEqual(3, dg["b"]);
    }

    [TestMethod(), Timeout(5000)]
    public void DependeesSize_NoDependees_ShouldReturnZero()
    {
      DependencyGraph dg = new DependencyGraph();
      Assert.AreEqual(0, dg["a"]);  // graph is empty
      dg.AddDependency("a", "b");
      Assert.AreEqual(0, dg["a"]);  // b has one dependee, but a has zero dependees
    }

    [TestMethod(), Timeout(5000)]
    public void DependeesSize_DependeesAddedThenSomeAreRemoved_ShouldReturnUpdatedCount()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("x", "b");
      dg.AddDependency("z", "b");
      Assert.AreEqual(3, dg["b"]);
      dg.RemoveDependency("a", "b");
      Assert.AreEqual(2, dg["b"]);
      dg.AddDependency("b", "e");  // this should not change the size of dependees("b")
      Assert.AreEqual(2, dg["b"]);
      dg.RemoveDependency("x", "b");
      Assert.AreEqual(1, dg["b"]);
      dg.RemoveDependency("z", "b");
      Assert.AreEqual(0, dg["b"]);
    }




    [TestMethod(), Timeout(5000)]
    public void HasDependents_OneDependent_ShouldReturnTrue()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.IsTrue(dg.HasDependents("a"));
    }

    [TestMethod(), Timeout(5000)]
    public void HasDependents_MultipleDependents_ShouldReturnTrue()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("a", "c");
      dg.AddDependency("a", "d");
      Assert.IsTrue(dg.HasDependents("a"));
    }

    [TestMethod(), Timeout(5000)]
    public void HasDependents_IsActuallyADependee_ShouldReturnFalse()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.IsFalse(dg.HasDependents("b"));
    }

    [TestMethod(), Timeout(5000)]
    public void HasDependents_DependentAddedThenRemoved_ShouldReturnTrueThenFalse()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.IsTrue(dg.HasDependents("a"));
      dg.RemoveDependency("a", "b");
      Assert.IsFalse(dg.HasDependents("a"));
    }




    [TestMethod(), Timeout(5000)]
    public void HasDependees_OneDependee_ShouldReturnTrue()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.IsTrue(dg.HasDependees("b"));
    }

    [TestMethod(), Timeout(5000)]
    public void HasDependees_MultipleDependees_ShouldReturnTrue()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("x", "b");
      dg.AddDependency("z", "b");
      Assert.IsTrue(dg.HasDependees("b"));
    }

    [TestMethod(), Timeout(5000)]
    public void HasDependees_IsActuallyADependent_ShouldReturnFalse()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.IsFalse(dg.HasDependees("a"));
    }

    [TestMethod(), Timeout(5000)]
    public void HasDependees_DependeeAddedThenRemoved_ShouldReturnTrueThenFalse()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.IsTrue(dg.HasDependees("b"));
      dg.RemoveDependency("a", "b");
      Assert.IsFalse(dg.HasDependees("b"));
    }




    [TestMethod(), Timeout(5000)]
    public void GetDependents_OneDependent_ShouldReturnDependents()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      IEnumerable<string> dependents = dg.GetDependents("a");
      Assert.AreEqual(1, dependents.Count());
      Assert.IsTrue(dependents.Contains("b"));
    }

    [TestMethod(), Timeout(5000)]
    public void GetDependents_MultipleDependents_ShouldReturnDependents()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("a", "c");
      dg.AddDependency("a", "d");
      IEnumerable<string> dependents = dg.GetDependents("a");
      Assert.AreEqual(3, dependents.Count());
      Assert.IsTrue(dependents.Contains("b"));
      Assert.IsTrue(dependents.Contains("c"));
      Assert.IsTrue(dependents.Contains("d"));
    }

    [TestMethod(), Timeout(5000)]
    public void GetDependents_ZeroDependents_ShouldReturnEmptyEnumerable()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      IEnumerable<string> dependents = dg.GetDependents("b");
      Assert.IsTrue(dependents.IsEmpty());
    }

    [TestMethod(), Timeout(5000)]
    public void GetDependents_EmptyGraph_ShouldReturnEmptyEnumerable()
    {
      DependencyGraph dg = new DependencyGraph();
      IEnumerable<string> dependents = dg.GetDependents("a");
      Assert.IsTrue(dependents.IsEmpty());
    }

    [TestMethod(), Timeout(5000)]
    public void GetDependents_DependentsAreAddedAndRemoved_ShouldReturnUpdatedDependents()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("a", "c");
      dg.AddDependency("a", "d");
      IEnumerable<string> dependents = dg.GetDependents("a");
      Assert.AreEqual(3, dependents.Count());
      Assert.IsTrue(dependents.Contains("b"));
      Assert.IsTrue(dependents.Contains("c"));
      Assert.IsTrue(dependents.Contains("d"));
      dg.RemoveDependency("a", "b");
      dependents = dg.GetDependents("a");
      Assert.AreEqual(2, dependents.Count());
      Assert.IsTrue(dependents.Contains("c"));
      Assert.IsTrue(dependents.Contains("d"));
    }




    [TestMethod(), Timeout(5000)]
    public void GetDependees_OneDependee_ShouldReturnDependees()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      IEnumerable<string> dependees = dg.GetDependees("b");
      Assert.AreEqual(1, dependees.Count());
      Assert.IsTrue(dependees.Contains("a"));
    }

    [TestMethod(), Timeout(5000)]
    public void GetDependees_MultipleDependees_ShouldReturnDependees()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("x", "b");
      dg.AddDependency("z", "b");
      IEnumerable<string> dependees = dg.GetDependees("b");
      Assert.AreEqual(3, dependees.Count());
      Assert.IsTrue(dependees.Contains("a"));
      Assert.IsTrue(dependees.Contains("x"));
      Assert.IsTrue(dependees.Contains("z"));
    }

    [TestMethod(), Timeout(5000)]
    public void GetDependees_ZeroDependees_ShouldReturnEmptyEnumerable()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      IEnumerable<string> dependees = dg.GetDependees("a");
      Assert.IsTrue(dependees.IsEmpty());
    }

    [TestMethod(), Timeout(5000)]
    public void GetDependees_EmptyGraph_ShouldReturnEmptyEnumerable()
    {
      DependencyGraph dg = new DependencyGraph();
      IEnumerable<string> dependees = dg.GetDependees("b");
      Assert.IsTrue(dependees.IsEmpty());
    }

    [TestMethod(), Timeout(5000)]
    public void GetDependees_DependeesAreAddedAndRemoved_ShouldReturnUpdatedDependees()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("x", "b");
      dg.AddDependency("z", "b");
      IEnumerable<string> dependees = dg.GetDependees("b");
      Assert.AreEqual(3, dependees.Count());
      Assert.IsTrue(dependees.Contains("a"));
      Assert.IsTrue(dependees.Contains("x"));
      Assert.IsTrue(dependees.Contains("z"));
      dg.RemoveDependency("a", "b");
      dependees = dg.GetDependees("b");
      Assert.AreEqual(2, dependees.Count());
      Assert.IsTrue(dependees.Contains("x"));
      Assert.IsTrue(dependees.Contains("z"));
    }




    [TestMethod(), Timeout(5000)]
    public void AddDependency_Unique_ShouldContainOne()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.AreEqual(1, dg.Size);
    }

    [TestMethod(), Timeout(5000)]
    public void AddDependency_Duplicates_ShouldContainOne()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("a", "b");
      dg.AddDependency("a", "b");
      Assert.AreEqual(1, dg.Size);
    }

    [TestMethod(), Timeout(5000)]
    public void AddDependency_SameKeyButDifferentValues_ShouldContainAll()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("a", "c");
      dg.AddDependency("a", "d");
      Assert.AreEqual(3, dg.Size);
      Assert.AreEqual(3, dg.GetDependents("a").Count());
    }

    [TestMethod(), Timeout(5000)]
    public void AddDependency_DifferentKeyButSameValues_ShouldContainAll()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("b", "a");
      dg.AddDependency("c", "a");
      dg.AddDependency("d", "a");
      Assert.AreEqual(3, dg.Size);
      Assert.AreEqual(1, dg.GetDependents("b").Count());
      Assert.AreEqual(1, dg.GetDependents("c").Count());
      Assert.AreEqual(1, dg.GetDependents("d").Count());
    }




    [TestMethod(), Timeout(5000)]
    public void RemoveDependency_AddThenRemoveDependency_ShouldContainNothing()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("x", "y");
      Assert.AreEqual(1, dg.Size);
      dg.RemoveDependency("x", "y");
      Assert.AreEqual(0, dg.Size);
    }

    [TestMethod(), Timeout(5000)]
    public void RemoveDependency_KeyHasMultipleDependents_ShouldOnlyRemoveTheOne()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("a", "c");
      dg.AddDependency("a", "d");
      Assert.AreEqual(3, dg.Size);
      dg.RemoveDependency("a", "c");
      Assert.AreEqual(2, dg.Size);
    }

    [TestMethod(), Timeout(5000)]
    public void RemoveDependency_OrderedPairDoesNotExist_ShouldDoNothing()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.AreEqual(1, dg.Size);
      dg.RemoveDependency("x", "y");
      Assert.AreEqual(1, dg.Size);
    }

    [TestMethod(), Timeout(5000)]
    public void RemoveDependency_ButIsActuallyADependee_ShouldDoNothing()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.AreEqual(1, dg.Size);
      dg.RemoveDependency("b", "a");
      Assert.AreEqual(1, dg.Size);
    }




    [TestMethod(), Timeout(5000)]
    public void ReplaceDependents_EmptyGraph_ShouldNotFail()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.ReplaceDependents("a", new HashSet<string> { "x", "y", "z" });
    }

    [TestMethod(), Timeout(5000)]
    public void ReplaceDependents_ZeroDependentsThenReplaceWithNewDependents_ShouldAddNewDependenciesToGraph()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.AreEqual(1, dg.Size);
      Assert.AreEqual(0, dg.GetDependents("b").Count());
      dg.ReplaceDependents("b", new HashSet<string> { "x", "y", "z" });
      Assert.AreEqual(4, dg.Size);
      Assert.AreEqual(3, dg.GetDependents("b").Count());
    }

    [TestMethod(), Timeout(5000)]
    public void ReplaceDependents_AddNewDependent_ShouldUpdateDependentsAndDependees()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.AreEqual(1, dg.Size);
      Assert.AreEqual(1, dg.GetDependents("a").Count());
      Assert.AreEqual(0, dg.GetDependents("b").Count());
      Assert.AreEqual(0, dg.GetDependees("a").Count());
      Assert.AreEqual(1, dg.GetDependees("b").Count());
      // this is the equivalent of doing dg.AddDependency("b", "a");
      dg.ReplaceDependents("b", new HashSet<string> { "a" });
      Assert.AreEqual(2, dg.Size);
      Assert.AreEqual(1, dg.GetDependents("a").Count());
      Assert.AreEqual(1, dg.GetDependents("b").Count());
      Assert.AreEqual(1, dg.GetDependees("a").Count());
      Assert.AreEqual(1, dg.GetDependees("b").Count());
    }

    [TestMethod(), Timeout(5000)]
    public void ReplaceDependents_EmptySet_ShouldRemoveFromGraph()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("a", "c");
      dg.AddDependency("a", "d");
      Assert.AreEqual(3, dg.Size);
      Assert.AreEqual(3, dg.GetDependents("a").Count());
      dg.ReplaceDependents("a", new HashSet<string>());
      Assert.AreEqual(0, dg.Size);
    }

    [TestMethod(), Timeout(5000)]
    public void ReplaceDependents_NewDependentsSet_ShouldOverwriteOriginalSet()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("a", "c");
      dg.AddDependency("a", "d");
      Assert.AreEqual(3, dg.Size);
      Assert.AreEqual(3, dg.GetDependents("a").Count());
      dg.ReplaceDependents("a", new HashSet<string> { "x" });
      Assert.AreEqual(1, dg.Size);
      Assert.AreEqual(1, dg.GetDependents("a").Count());
    }




    [TestMethod(), Timeout(5000)]
    public void ReplaceDependees_EmptyGraph_ShouldNotFail()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.ReplaceDependees("a", new HashSet<string> { "x", "y", "z" });
    }

    [TestMethod(), Timeout(5000)]
    public void ReplaceDependees_ZeroDependeesThenReplaceWithNewDependees_ShouldAddNewDependenciesToGraph()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.AreEqual(1, dg.Size);
      Assert.AreEqual(0, dg.GetDependees("a").Count());
      dg.ReplaceDependees("a", new HashSet<string> { "x", "y", "z" });
      Assert.AreEqual(4, dg.Size);
      Assert.AreEqual(3, dg.GetDependees("a").Count());
    }

    [TestMethod(), Timeout(5000)]
    public void ReplaceDependees_AddNewDependee_ShouldUpdateDependentsAndDependees()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      Assert.AreEqual(1, dg.Size);
      Assert.AreEqual(1, dg.GetDependents("a").Count());
      Assert.AreEqual(0, dg.GetDependents("b").Count());
      Assert.AreEqual(0, dg.GetDependees("a").Count());
      Assert.AreEqual(1, dg.GetDependees("b").Count());
      // this is the equivalent of doing dg.AddDependency("b", "a");
      dg.ReplaceDependees("a", new HashSet<string> { "b" });
      Assert.AreEqual(2, dg.Size);
      Assert.AreEqual(1, dg.GetDependents("a").Count());
      Assert.AreEqual(1, dg.GetDependents("b").Count());
      Assert.AreEqual(1, dg.GetDependees("a").Count());
      Assert.AreEqual(1, dg.GetDependees("b").Count());
    }

    [TestMethod(), Timeout(5000)]
    public void ReplaceDependees_EmptySet_ShouldRemoveFromGraph()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("a1", "b");
      dg.AddDependency("a2", "b");
      Assert.AreEqual(3, dg.Size);
      Assert.AreEqual(3, dg.GetDependees("b").Count());
      dg.ReplaceDependees("b", new HashSet<string>());
      Assert.AreEqual(0, dg.Size);
    }

    [TestMethod(), Timeout(5000)]
    public void ReplaceDependees_NewDependeesSet_ShouldOverwriteOriginalSet()
    {
      DependencyGraph dg = new DependencyGraph();
      dg.AddDependency("a", "b");
      dg.AddDependency("a1", "b");
      dg.AddDependency("a2", "b");
      Assert.AreEqual(3, dg.Size);
      Assert.AreEqual(3, dg.GetDependees("b").Count());
      dg.ReplaceDependees("b", new HashSet<string> { "x" });
      Assert.AreEqual(1, dg.Size);
      Assert.AreEqual(1, dg.GetDependees("b").Count());
    }




    [TestMethod(), Timeout(5000)]
    public void DependencyGraph_StressTest()
    {
      // dependency graph
      DependencyGraph dg = new DependencyGraph();

      // a bunch of strings to use
      const int SIZE = 200;
      string[] letters = new string[SIZE];
      for (int i = 0; i < SIZE; i++) {
        letters[i] = ("" + (char)('a' + i));
      }

      // the correct answers
      HashSet<string>[] dependents = new HashSet<string>[SIZE];
      HashSet<string>[] dependees = new HashSet<string>[SIZE];
      for (int i = 0; i < SIZE; i++) {
        dependents[i] = new HashSet<string>();
        dependees[i] = new HashSet<string>();
      }

      // add a bunch of dependencies
      for (int i = 0; i < SIZE; i++) {
        for (int j = i + 1; j < SIZE; j++) {
          dg.AddDependency(letters[i], letters[j]);
          dependents[i].Add(letters[j]);
          dependees[j].Add(letters[i]);
        }
      }

      // remove a bunch of dependencies
      for (int i = 0; i < SIZE; i++) {
        for (int j = i + 4; j < SIZE; j += 4) {
          dg.RemoveDependency(letters[i], letters[j]);
          dependents[i].Remove(letters[j]);
          dependees[j].Remove(letters[i]);
        }
      }

      // add some back
      for (int i = 0; i < SIZE; i++) {
        for (int j = i + 1; j < SIZE; j += 2) {
          dg.AddDependency(letters[i], letters[j]);
          dependents[i].Add(letters[j]);
          dependees[j].Add(letters[i]);
        }
      }

      // remove some more
      for (int i = 0; i < SIZE; i += 2) {
        for (int j = i + 3; j < SIZE; j += 3) {
          dg.RemoveDependency(letters[i], letters[j]);
          dependents[i].Remove(letters[j]);
          dependees[j].Remove(letters[i]);
        }
      }

      // make sure everything is right
      for (int i = 0; i < SIZE; i++) {
        Assert.IsTrue(dependents[i].SetEquals(new HashSet<string>(dg.GetDependents(letters[i]))));
        Assert.IsTrue(dependees[i].SetEquals(new HashSet<string>(dg.GetDependees(letters[i]))));
      }
    }

  }
}