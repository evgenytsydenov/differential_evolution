using System;
using System.Collections.Generic;
using System.Linq;

namespace DifferentialEvolution
{
    /// <summary>
    ///     Math functions.
    /// </summary>
    internal class MathFunctions
    {
        /// <summary>
        ///     Calculate standard deviation.
        /// </summary>
        /// <param name="values">Values.</param>
        /// <returns>Deviation.</returns>
        internal static double Std(IEnumerable<double> values)
        {
            var mean = values.Average();
            return Math.Sqrt(values.Average(x => Math.Pow(x - mean, 2)));
        }
    }
}