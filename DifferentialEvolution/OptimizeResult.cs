namespace DifferentialEvolution
{
    /// <summary>
    ///     Implementation of optimization results.
    /// </summary>
    public class OptimizeResult
    {
        /// <summary>
        ///     Values of the best individual.
        /// </summary>
        public double[] BestSolution { get; internal set; }

        /// <summary>
        ///     Cause of solver termination.
        /// </summary>
        public string CauseOfTermination { get; internal set; }

        /// <summary>
        ///     Fitness function value with the best solution.
        /// </summary>
        public double Energy { get; internal set; }

        /// <summary>
        ///     Convergence of the solver.
        /// </summary>
        public double Convergence { get; internal set; }

        /// <summary>
        ///     Time of optimizations process.
        /// </summary>
        public double OptimizationTime { get; internal set; }

        /// <summary>
        ///     Count of fitness function evaluations.
        /// </summary>
        public double FunctionEvaluations { get; internal set; }
    }
}