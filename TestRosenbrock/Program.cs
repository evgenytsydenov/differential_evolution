using System;
using DifferentialEvolution;

namespace TestRosenbrock
{
    /// <summary>
    ///     Test differential evolution algorithm with Rosenbrock function.
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
                ToDisplayOptimization = false
            };

            // Define bounds of variables
            var bounds = new (double, double)[]
            {
                (0, 2), (-5, 5), (-100, 50), (1, 34)
            };

            // Solve
            var result = solver.Start(Rosenbrock, bounds);
            
            // Print results
            Console.WriteLine(
                $"Solution: [{string.Join(", ", result.BestSolution)}]\n" +
                $"Function value: {result.Energy}\n" +
                $"True solution: [1, 1, 1, 1]\n" +
                $"True function value: 0\n" +
                $"Optimization time: {result.OptimizationTime:f4} sec\n" +
                $"Function evaluations count: {result.FunctionEvaluations}\n");
            Console.ReadLine();
        }
        
        /// <summary>
        ///     Rosenbrock function.
        /// </summary>
        /// <param name="variables">Variable values.</param>
        /// <param name="args">Arguments.</param>
        /// <returns>Result.</returns>
        public static double Rosenbrock(
            double[] variables, object[] args = null)
        {
            double result = 0;
            for (var i = 0; i < variables.Length - 1; i++)
            {
                result += Math.Pow(1 - variables[i], 2)
                          + 100 * Math.Pow(variables[i + 1] 
                                           - Math.Pow(variables[i], 2), 2);
            }
            return result;
        }
    }
}