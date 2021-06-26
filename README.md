# Differential evolution algorithm

This is a .NET implementation of the differential evolution algorithm for global optimization of multivariable functions.
The implementation is mostly based on the [SciPy](https://github.com/scipy/scipy/blob/v1.3.2/scipy/optimize/_differentialevolution.py) realization.

## Work description

The optimization starts with population initialization by Random or Latin Hypercube type.
At each step the algorithm creates trial candidates by changing existing solutions
in accordance with mutation strategies and recombination property.
If the trial individual is better then the original one, it takes its place.  
The optimization continues until the iterations count is more than MaxIteration property
or until the solver converges.

The list of supported mutation strategies:
- Best1Bin
- Best1Exp
- Best2Bin
- Best2Exp
- Rand1Bin
- Rand1Exp
- Rand2Bin
- Rand2Exp
- RandToBest1Bin
- RandToBest1Exp
- CurrentToBest1Bin
- CurrentToBest1Exp

There are also two update types:
- immediate (the best solution is updated continuously within a single iteration)
- deferred (the best solution is updated once per generation)

## Getting started:

Firstly, it is necessary to determine the fitness function. For example, [Rosenbroke](https://en.wikipedia.org/wiki/Rosenbrock_function):

```C#
public static double Rosenbroke(
    double [] variables, object [] args = null)
{
    double result = 0;
    for (int i = 0; i < variables.Length - 1; i++)
    {
        result += Math.Pow(1 - variables[i], 2)
                  + 100 * Math.Pow(variables[i + 1] 
                                   - Math.Pow(variables[i], 2), 2);
    }
    return result;
}  
```

After adding the reference of the library to the project, 
it is necessary to create the Solver and Start the optimization process. 
There are a lot of parameters that should be tuned to reach the best performance and accuracy.

```C#
public static void Main()
{
    // Create solver
    var solver = new Solver
    {
        ToDisplayOptimization = false,
        PopSizeMultiplier = 20,
        MutationRange = (0.4, 0.7),
        SearchStrategy = SearchStrategies.Rand1Bin,
        UpdateType = UpdateTypes.Deferred,
    };

    // Define bounds of variables
    var bounds = new (double, double)[]
    {
        (-1, 4), (0, 8), (-10, 15),
    };

    // Solve
    var result = solver.Start(Rosenbrock, bounds, randomSeed:40);
            
    // Print results
    Console.WriteLine(
        $"Solution: [{string.Join(", ", result.BestSolution)}]\n" +
        $"Function value: {result.Energy}\n" +
        $"True solution: [1, 1, 1]\n" +
        $"True function value: 0\n" +
        $"Optimization time: {result.OptimizationTime:f4} sec\n" +
        $"Function evaluations count: {result.FunctionEvaluations}\n");
}
```

Output:
```
Solution: [1, 1, 1]
Function value: 0
True solution: [1, 1, 1]
True function value: 0
Optimization time: 0.2299 sec
Function evaluations count: 28320
```