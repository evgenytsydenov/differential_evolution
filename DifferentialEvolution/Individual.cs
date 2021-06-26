using System;
using System.Linq;

namespace DifferentialEvolution
{
    /// <summary>
    ///     Individual description.
    /// </summary>
    internal class Individual
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="variablesCount">Variables count.</param>
        internal Individual(int variablesCount)
        {
            Values = new double [variablesCount];
        }

        /// <summary>
        ///     Variables of the individual.
        /// </summary>
        internal double[] Values { get; set; }

        /// <summary>
        ///     Energy of the individual.
        /// </summary>
        internal double Energy { get; set; } = double.PositiveInfinity;

        #region Initialization methods
        /// <summary>
        ///     Initializes a random population.
        /// </summary>
        /// <param name="variablesCount">Variables count.</param>
        /// <param name="popSize">Population size.</param>
        /// <param name="random">Random object.</param>
        /// <returns>New population.</returns>
        internal static Individual[] GetIndividualsRandom(
            int variablesCount, int popSize, Random random)
        {
            var population = new Individual[popSize];
            for (var j = 0; j < popSize; j++)
            {
                population[j] = new Individual(variablesCount);
                for (var i = 0; i < variablesCount; i++)
                    population[j].Values[i] = random.NextDouble();
            }
            return population;
        }

        /// <summary>
        ///     Initializes the individual by Latin Hypercube Sampling
        ///     which ensures that each parameter is uniformly sampled
        ///     over its range.
        /// </summary>
        /// <param name="variablesCount">Variables count.</param>
        /// <param name="popSize">Population size.</param>
        /// <param name="random">Random object.</param>
        /// <returns>New population.</returns>
        internal static Individual[] GetIndividualsLatin(
            int variablesCount, int popSize, Random random)
        {
            // The range of each parameter needs to be sampled uniformly.
            // So it needs to be split into segments
            var linSpace = new double[popSize];
            var segSize = 1.0 / popSize;
            for (var i = 0; i < linSpace.Length; i++)
            {
                linSpace[i] = i * segSize;               
            }

            // Create empty population
            var population = new Individual[popSize];
            for (var j = 0; j < popSize; j++)
            {
                population[j] = new Individual(variablesCount);
            }

            // Set values for each variable of individual
            for (var i = 0; i < variablesCount; i++)
            {
                var values = new double [popSize];
                for (var j = 0; j < popSize; j++)
                {
                    values[j] = linSpace[j] + random.NextDouble() * segSize;
                }
                values = values.OrderBy(x => random.Next()).ToArray();
                for (var j = 0; j < popSize; j++)
                {
                    population[j].Values[i] = values[j];
                }
            }
            return population;
        }
        #endregion
    }
}