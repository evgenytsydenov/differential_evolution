using System;
using DifferentialEvolution;

namespace TestAckley
{
    /// <summary>
    ///     Test differential evolution algorithm with McCormick function.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Start point.
        /// </summary>
        public static void Main()
        {
            // Create solver
            var solver = new Solver
            {
                ToDisplayOptimization = false,
                UpdateType = UpdateTypes.Deferred,
                MutationRange = (0.7, 1.2),
                RecombProbability = 0.8,
                PopSizeMultiplier = 20
            };

            // Define bounds of variables
            var bounds = new (double, double)[]
            {
                (-3, 2), (0, 5)
            };

            // Solve
            var result = solver.Start(Ackley, bounds, randomSeed: 40);
            
            // Print results
            Console.WriteLine(
                $"Solution: [{string.Join(", ", result.BestSolution)}]\n" +
                $"Function value: {result.Energy}\n" +
                $"True solution: [0, 0]\n" +
                $"True function value: 0\n" +
                $"Optimization time: {result.OptimizationTime:f4} sec\n" +
                $"Function evaluations count: {result.FunctionEvaluations}\n");
            Console.ReadLine();
        }
        
        /// <summary>
        ///     Ackley function.
        /// </summary>
        /// <param name="variables">Variable values.</param>
        /// <param name="args">Arguments.</param>
        /// <returns>Result.</returns>
        public static double Ackley(double[] variables, object[] args = null)
        {
            var x = variables[0];
            var y = variables[1];
            return - 20 * Math.Exp(-0.2 * Math.Sqrt(0.5 * (Math.Pow(x, 2) + Math.Pow(y, 2))))
                   - Math.Exp(0.5 * (Math.Cos(2 * Math.PI * x) + Math.Cos(2 * Math.PI * y)))
                   + Math.Exp(1) + 20;
        }
    }
}