using System;
using System.Collections.Generic;

namespace DifferentialEvolution
{
    /// <summary>
    ///     Mutation functions.
    /// </summary>
    internal class MutationFuctions
    {
        /// <summary>
        ///     Dictionary for searching mutation
        ///     functions by name of the search strategy.
        /// </summary>
        internal static Dictionary<SearchStrategies,
            Func<int[], Individual[], double, int, Individual>> Binomial
            = new Dictionary<SearchStrategies,
                Func<int[], Individual[], double, int, Individual>>
            {
                {SearchStrategies.Best1Bin, Best1},
                {SearchStrategies.RandToBest1Bin, RandToBest1},
                {SearchStrategies.CurrentToBest1Bin, CurrentToBest1},
                {SearchStrategies.Best2Bin, Best2},
                {SearchStrategies.Rand2Bin, Rand2},
                {SearchStrategies.Rand1Bin, Rand1}
            };

        /// <summary>
        ///     Dictionary for searching mutation
        ///     functions by the name of the search strategy.
        /// </summary>
        internal static Dictionary<SearchStrategies,
            Func<int[], Individual[], double, int, Individual>> Exponential
            = new Dictionary<SearchStrategies,
                Func<int[], Individual[], double, int, Individual>>
            {
                {SearchStrategies.Best1Exp, Best1},
                {SearchStrategies.RandToBest1Exp, RandToBest1},
                {SearchStrategies.CurrentToBest1Exp, CurrentToBest1},
                {SearchStrategies.Best2Exp, Best2},
                {SearchStrategies.Rand2Exp, Rand2},
                {SearchStrategies.Rand1Exp, Rand1}
            };

        #region Functions
        /// <summary>
        ///     Create the prime individual in accordance
        ///     with <see cref="SearchStrategies.Best1Bin" />
        ///     and <see cref="SearchStrategies.Best1Exp" /> strategies.
        /// </summary>
        /// <param name="samples">Samples to choose random individuals.</param>
        /// <param name="population">Whole population.</param>
        /// <param name="mutationValue">Mutation value.</param>
        /// <param name="candidate">Candidate index.</param>
        /// <returns>Prime individual.</returns>
        private static Individual Best1(
            int[] samples, Individual[] population,
            double mutationValue, int candidate)
        {
            var prime = new Individual(population[0].Values.Length);
            for (var i = 0; i < prime.Values.Length; i++)
            {
                prime.Values[i] = population[0].Values[i] 
                                  + mutationValue
                                  * (population[samples[0]].Values[i] 
                                     - population[samples[1]].Values[i]);
            }
            return prime;
        }

        /// <summary>
        ///     Create the prime individual
        ///     in accordance with <see cref="SearchStrategies.Best2Bin" /> and
        ///     <see cref="SearchStrategies.Best2Exp" /> strategies.
        /// </summary>
        /// <param name="samples">Samples to choose random individuals.</param>
        /// <param name="population">Whole population.</param>
        /// <param name="mutationValue">Mutation value.</param>
        /// <param name="candidate">Candidate index.</param>
        /// <returns>Prime individual.</returns>
        private static Individual Best2(
            int[] samples, Individual[] population,
            double mutationValue, int candidate)
        {
            var prime = new Individual(population[0].Values.Length);
            for (var i = 0; i < prime.Values.Length; i++)
            {
                prime.Values[i] = population[0].Values[i] + mutationValue
                    * (population[samples[0]].Values[i] 
                       + population[samples[1]].Values[i]
                       - population[samples[2]].Values[i] 
                       - population[samples[3]].Values[i]);
            }
            return prime;
        }

        /// <summary>
        ///     Create the prime individual
        ///     in accordance with <see cref="SearchStrategies.Rand1Bin" /> and
        ///     <see cref="SearchStrategies.Rand1Exp" /> strategies.
        /// </summary>
        /// <param name="samples">Samples to choose random individuals.</param>
        /// <param name="population">Whole population.</param>
        /// <param name="mutationValue">Mutation value.</param>
        /// <param name="candidate">Candidate index.</param>
        /// <returns>Prime individual.</returns>
        private static Individual Rand1(
            int[] samples, Individual[] population,
            double mutationValue, int candidate)
        {
            var prime = new Individual(population[0].Values.Length);
            for (var i = 0; i < prime.Values.Length; i++)
            {
                prime.Values[i] = population[samples[0]].Values[i] 
                                  + mutationValue
                                  * (population[samples[1]].Values[i] 
                                     - population[samples[2]].Values[i]);
            }
            return prime;
        }

        /// <summary>
        ///     Create the prime individual
        ///     in accordance with <see cref="SearchStrategies.Rand2Bin" />
        ///     and <see cref="SearchStrategies.Rand2Exp" /> strategies.
        /// </summary>
        /// <param name="samples">Samples to choose random individuals.</param>
        /// <param name="population">Whole population.</param>
        /// <param name="mutationValue">Mutation value.</param>
        /// <param name="candidate">Candidate index.</param>
        /// <returns>Prime individual.</returns>
        private static Individual Rand2(
            int[] samples, Individual[] population,
            double mutationValue, int candidate)
        {
            var prime = new Individual(population[0].Values.Length);
            for (var i = 0; i < prime.Values.Length; i++)
            {
                prime.Values[i] = population[samples[0]].Values[i] 
                                  + mutationValue
                                  * (population[samples[1]].Values[i] 
                                     + population[samples[2]].Values[i]
                                     - population[samples[3]].Values[i] 
                                     - population[samples[4]].Values[i]);
            }
            return prime;
        }

        /// <summary>
        ///     Create the prime individual in accordance 
        ///     with <see cref="SearchStrategies.RandToBest1Bin" />
        ///     and <see cref="SearchStrategies.RandToBest1Exp" /> strategies.
        /// </summary>
        /// <param name="samples">Samples to choose random individuals.</param>
        /// <param name="population">Whole population.</param>
        /// <param name="mutationValue">Mutation value.</param>
        /// <param name="candidate">Candidate index.</param>
        /// <returns>Prime individual.</returns>
        private static Individual RandToBest1(
            int[] samples, Individual[] population,
            double mutationValue, int candidate)
        {
            var prime = new Individual(population[0].Values.Length);
            for (var i = 0; i < prime.Values.Length; i++)
            {
                prime.Values[i] = population[samples[0]].Values[i];
                prime.Values[i] += mutationValue * (population[0].Values[i] 
                    - prime.Values[i]);
                prime.Values[i] += mutationValue 
                                   * (population[samples[1]].Values[i] 
                                      - population[samples[2]].Values[i]);
            }
            return prime;
        }

        /// <summary>
        ///     Create the prime individual in accordance 
        ///     with <see cref="SearchStrategies.CurrentToBest1Bin" /> and 
        ///     <see cref="SearchStrategies.CurrentToBest1Exp" /> strategies.
        /// </summary>
        /// <param name="samples">Samples to choose random individuals.</param>
        /// <param name="candidate">Index of candidate.</param>
        /// <param name="population">Whole population.</param>
        /// <param name="mutationValue">Mutation value.</param>
        /// <returns>Prime individual.</returns>
        private static Individual CurrentToBest1(
            int[] samples, Individual[] population,
            double mutationValue, int candidate)
        {
            var prime = new Individual(population[0].Values.Length);
            for (var i = 0; i < prime.Values.Length; i++)
            {
                prime.Values[i] = population[candidate].Values[i] 
                                  + mutationValue 
                                  * (population[0].Values[i] 
                                     - population[candidate].Values[i]
                                     + population[samples[0]].Values[i] 
                                     - population[samples[1]].Values[i]);
            }
            return prime;
        }
        #endregion
    }
}