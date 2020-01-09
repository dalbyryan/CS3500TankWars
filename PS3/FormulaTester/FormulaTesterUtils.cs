// Luke Ludlow
// CS 3500
// 2019 September

using System;

namespace FormulaTester
{
    /// <summary>
    /// static helper class to create delegates that are more complicated than a simple lambda 
    /// (normalizer, validator, lookup)
    /// </summary>
    internal static class FormulaTesterUtils
    {

        /// <summary>
        /// helper method to create a validator delegate, isValid, that maps the specified variables to valus.
        /// essentially, you just provide a list of variables that will be valid (or you can specify if they're invalid).
        /// example usage:
        /// CreateValidatorDelegate(("x1", true), ("x5", false), ...add any more pairs...);
        /// </summary>
        /// <param name="pairs">array of value tuple pairs</param>
        public static Func<string, bool> CreateValidatorDelegate(params ValueTuple<string, bool>[] pairs)
        {
            Func<string, bool> isValid = s => {
                foreach (ValueTuple<string, bool> pair in pairs) {
                    if (s == pair.Item1) {
                        return pair.Item2;
                    }
                }
                // if the variable isn't one of the known specified variables, then say it's invalid
                return false;
            };
            return isValid;
        }

        /// <summary>
        /// TODO not sure if i need this function
        /// </summary>
        /// <param name="pairs">array of value tuple pairs</param>
        public static Func<string, string> CreateNormalizerDelegate(params ValueTuple<string, string>[] pairs)
        {
            Func<string, string> normalize = s => {
                foreach (ValueTuple<string, string> pair in pairs) {
                    if (s == pair.Item1) {
                        return pair.Item2;
                    }
                }
                throw new ArgumentException("variable not found");
            };
            return normalize;
        }

        /// <summary>
        /// helper method to create a lookup delegate that maps the specified variables to values.
        /// example usage:
        /// CreateLookupDelegate(("x", 2), ("X", 4), ...add any more pairs...);
        /// </summary>
        /// <param name="pairs">array of value tuple pairs</param>
        public static Func<string, double> CreateLookupDelegate(params ValueTuple<string, double>[] pairs)
        {
            Func<string, double> lookup = s => {
                foreach (ValueTuple<string, double> pair in pairs) {
                    if (s == pair.Item1) {
                        return pair.Item2;
                    }
                }
                throw new ArgumentException("variable not found");
            };
            return lookup;
        }

    }
}