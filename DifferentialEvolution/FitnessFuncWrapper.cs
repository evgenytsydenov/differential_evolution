using System;

namespace DifferentialEvolution
{
    /// <summary>
    ///     Fitness function wrapper.
    /// </summary>
    public class FitnessFuncWrapper
    {
        /// <summary>
        ///     Arguments.
        /// </summary>
        private readonly object[] _args;

        /// <summary>
        ///     Fitness function.
        /// </summary>
        private readonly Func<double[], object[], double> _fitnessFunction;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="fitnessFunction">Fitness function.</param>
        /// <param name="args">Arguments.</param>
        public FitnessFuncWrapper(
            Func<double[], object[], double> fitnessFunction,
            object[] args = null)
        {
            _args = args;
            _fitnessFunction = fitnessFunction;
        }

        /// <summary>
        ///     Count of fitness function evaluations.
        /// </summary>
        public int EvaluationCount { get; private set; }

        /// <summary>
        ///     Estimate the fitness function.
        /// </summary>
        /// <param name="values">Values.</param>
        /// <returns>Result.</returns>
        public double Estimate(double[] values)
        {
            EvaluationCount++;
            return _fitnessFunction.Invoke(values, _args);
        }
    }
}