using System;
using DifferentialEvolution;

namespace TestMcCormick
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
                InitType = InitTypes.Random,
                SearchStrategy = SearchStrategies.Best1Exp
            };

            // Define bounds of variables
            var bounds = new (double, double)[]
            {
                (-1, 4), (-3, 4)
            };

            // Solve
            var result = solver.Start(McCormick, bounds, randomSeed:60);
            
            // Print results
            Console.WriteLine(
                $"Solution: [{string.Join(", ", result.BestSolution)}]\n" +
                $"Function value: {result.Energy}\n" +
                $"True solution: [-0.54719, -1.54719]\n" +
                $"True function value: -1.9133\n" +
                $"Optimization time: {result.OptimizationTime:f4} sec\n" +
                $"Function evaluations count: {result.FunctionEvaluations}\n");
            Console.ReadLine();
        }
                
        /// <summary>
        ///     McCormick function.
        /// </summary>
        /// <param name="variables">Variable values.</param>
        /// <param name="args">Arguments.</param>
        /// <returns>Result.</returns>
        public static double McCormick(
            double[] variables, object[] args = null)
        {
            var x = variables[0];
            var y = variables[1];
            return Math.Sin(x + y) + Math.Pow(x - y, 2) - 1.5 * x + 2.5 * y + 1;
        }
    }
}