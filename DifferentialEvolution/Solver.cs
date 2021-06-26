using System;
using System.Diagnostics;
using System.Linq;

namespace DifferentialEvolution
{
    /// <summary>
    ///     Differential evolution solver that
    ///     finds the global minimum of a multivariable function.
    /// </summary>
    public class Solver
    {
        /// <summary>
        ///     Start the optimization process.
        /// </summary>
        /// <param name="fitnessFunction">
        ///     Fitness function to be minimized.
        ///     Must be in form 'func(variables, arguments)'
        ///     and return a double value.
        ///     Variables are the values to optimize. Arguments are the values
        ///     to completely specify the function.
        /// </param>
        /// <param name="bounds">
        ///     Search bounds defining the lower and upper
        ///     borders for optimization variables.
        ///     They should be in form (min, max) for each variable.
        ///     The length of this array should be equal to variables count.
        /// </param>
        /// <param name="args">
        ///     Arguments needed for the fitness function (optional).
        /// </param>
        /// <param name="randomSeed">
        ///     The seed value for random object.
        ///     Setting this parameter to constant
        ///     value allows to get repeatable results (optional).
        /// </param>
        /// <returns>Optimization results.</returns>
        public OptimizeResult Start(
            Func<double[], object[], double> fitnessFunction,
            (double minValue, double maxValue)[] bounds,
            object[] args = null, int randomSeed = default)
        {
            // Set parameters
            _fitnessFunction = new FitnessFuncWrapper(fitnessFunction, args);
            Bounds = bounds;
            _random = randomSeed != default
                ? new Random(randomSeed)
                : new Random();
            _mutationFunc =
                MutationFuctions.Binomial.ContainsKey(SearchStrategy)
                    ? MutationFuctions.Binomial[SearchStrategy]
                    : MutationFuctions.Exponential[SearchStrategy];

            // Start optimization
            var causeOfTermination = "Max iterations limit";
            var optimizationTime = new Stopwatch();
            optimizationTime.Start();

            // Population initialization
            var popSize = Math.Max(PopSizeMultiplier * Bounds.Length, 5);
            Individual[] population;
            switch (InitType)
            {
                case InitTypes.Random:
                    population = Individual.GetIndividualsRandom(
                        Bounds.Length, popSize, _random);
                    break;
                case InitTypes.LatinHyperCube:
                    population = Individual.GetIndividualsLatin(
                        Bounds.Length, popSize, _random);
                    break;
                default:
                    throw new ArgumentException("Unknown initialization type");
            }

            for (var i = 0; i < popSize; i++)
            {
                population[i].Energy = _fitnessFunction.Estimate(
                    ToScale(population[i].Values));
            }

            // Set the best individual to the first place
            population = PromoteLowestEnergy(population);

            // Start iterations
            for (var step = 0; step < _maxIterations; step++)
            {
                // Next generation step
                population = NextStep(population);
                if (ToDisplayOptimization)
                {
                    Console.WriteLine(
                        $"Evolution step {step + 1}: " +
                        $"f(best x) = {population[0].Energy}");
                }

                // Check the convergence
                if (IsSolverConverged(population))
                {
                    causeOfTermination = "The solver has converged";
                    break;
                }
            }

            // Collect results
            optimizationTime.Stop();
            var results = new OptimizeResult
            {
                OptimizationTime = optimizationTime.Elapsed.TotalSeconds,
                Convergence = GetConvergence(population),
                BestSolution = ToScale(population[0].Values),
                Energy = population[0].Energy,
                CauseOfTermination = causeOfTermination,
                FunctionEvaluations = _fitnessFunction.EvaluationCount
            };
            return results;
        }

        /// <summary>
        ///     Next evolution step.
        /// </summary>
        /// <param name="population">Current population.</param>
        /// <returns>New population.</returns>
        private Individual[] NextStep(Individual[] population)
        {
            var mutationValue =
                MutationRange.minValue + _random.NextDouble()
                * (MutationRange.maxValue - MutationRange.minValue);

            switch (UpdateType)
            {
                // Update the best solution immediately
                case UpdateTypes.Immediate:
                {
                    for (var candidate = 0; 
                        candidate < population.Length; 
                        candidate++)
                    {
                        // Get new individual
                        var trialSolution = GetTrialSolution(
                            candidate, mutationValue, population);

                        // If the energy of the trial is lower than the original
                        // population member, replace it
                        if (trialSolution.Energy < population[candidate].Energy)
                        {
                            population[candidate] = trialSolution;

                            // Place this to the first 
                            // if it has the lowest energy
                            if (trialSolution.Energy < population[0].Energy)
                            {
                                population = PromoteLowestEnergy(population);
                            }
                        }
                    }
                    break;
                }

                // Update best solution once per generation
                case UpdateTypes.Deferred:
                {
                    var newPopulation = new Individual[population.Length];
                    for (var i = 0; i < newPopulation.Length; i++)
                    {
                        // Get new individual
                        var trialSolution = GetTrialSolution(
                            i, mutationValue, population);

                        // Save current solution
                        newPopulation[i] = trialSolution;
                    }

                    // If the energy of the trial is lower than the original
                    // population member, replace it
                    for (var i = 0; i < population.Length; i++)
                    {
                        if (newPopulation[i].Energy < population[i].Energy)
                        {
                            population[i] = newPopulation[i];
                        }
                    }
                    population = PromoteLowestEnergy(population);
                    break;
                }
            }
            return population;
        }

        /// <summary>
        ///     Create new individual in accordance with the mutation strategy.
        /// </summary>
        /// <param name="individualIndex">Index of candidate.</param>
        /// <param name="mutationValue">Mutation value.</param>
        /// <param name="population">Current population.</param>
        /// <returns>New individual.</returns>
        private Individual GetTrialSolution(
            int individualIndex, double mutationValue, Individual[] population)
        {
            // Select random individuals for mutation
            var samples = SelectSamples(individualIndex, population.Length);

            // Mutate samples 
            var prime = _mutationFunc(
                samples, population, mutationValue, individualIndex);

            // Crossover
            var trial = new Individual(population[0].Values.Length);
            population[individualIndex].Values.CopyTo(trial.Values , 0);
            var fillPoint = _random.Next(0, trial.Values.Length);
            if (MutationFuctions.Binomial.ContainsKey(SearchStrategy))
            {
                for (var i = 0; i < trial.Values.Length; i++)
                {
                    trial.Values[i] = _random.NextDouble() < _recombProbability 
                        ? prime.Values[i]
                        : population[individualIndex].Values[i];
                }
                trial.Values[fillPoint] = prime.Values[fillPoint];
            }
            else if (MutationFuctions.Exponential.ContainsKey(SearchStrategy))
            {
                var i = 0;
                while (i < trial.Values.Length
                       && _random.NextDouble() < _recombProbability)
                {
                    trial.Values[fillPoint] = prime.Values[fillPoint];
                    fillPoint = (fillPoint + 1) % trial.Values.Length;
                    i++;
                }
            }

            // Ensure the trial is in limits
            trial = EnsureConstraint(trial);

            // Get energy of the trial
            trial.Energy = _fitnessFunction.Estimate(ToScale(trial.Values));
            return trial;
        }

        #region Fields
        /// <summary>
        ///     Fitness function.
        /// </summary>
        private FitnessFuncWrapper _fitnessFunction;

        /// <summary>
        ///     Bounds for each value of individual.
        /// </summary>
        private (double minValue, double maxValue)[] _bounds;

        /// <summary>
        ///     The individuals values are scaled to [0, 1].
        ///     Scaler saves parameters for scaling and unscaling.
        /// </summary>
        private (double mean, double absDifference)[] _scaler;

        /// <summary>
        ///     Mutation bounds.
        /// </summary>
        private (double minValue, double maxValue) _mutationRange = (0.5, 1);

        /// <summary>
        ///     Relative tolerance.
        /// </summary>
        private double _relativeTolerance = 0.01;

        /// <summary>
        ///     Population size multiplier.
        /// </summary>
        private int _popSizeMultiplier = 15;

        /// <summary>
        ///     Absolute tolerance.
        /// </summary>
        private double _absoluteTolerance;

        /// <summary>
        ///     Recombination probability.
        /// </summary>
        private double _recombProbability = 0.7;

        /// <summary>
        ///     Max iterations count.
        /// </summary>
        private int _maxIterations = 1000;

        /// <summary>
        ///     Random object.
        /// </summary>
        private Random _random;

        /// <summary>
        ///     Mutation function.
        /// </summary>
        private Func<int[], Individual[], double, int, Individual>
            _mutationFunc;

        #endregion

        #region Properties
        /// <summary>
        ///     The method of the population initialization
        ///     at the start of optimization.
        ///     By default it is <see cref="InitTypes.LatinHyperCube" />.
        /// </summary>
        public InitTypes InitType { get; set; } = InitTypes.LatinHyperCube;

        /// <summary>
        ///     Type of updating the best solution vector.
        ///     By default it is <see cref="UpdateTypes.Immediate" />
        ///     update type.
        /// </summary>
        public UpdateTypes UpdateType { get; set; } = UpdateTypes.Immediate;

        /// <summary>
        ///     Differential evolution strategy.
        ///     By default it is <see cref="SearchStrategies.Best1Bin" />
        ///     strategy.
        /// </summary>
        public SearchStrategies SearchStrategy { get; set; }
            = SearchStrategies.Best1Bin;

        /// <summary>
        ///     Range for mutation constant. Is should be in form (min, max).
        ///     The mutation constant for each generation
        ///     is randomly taken from [min, max) range.
        ///     By default it is equal to (0.5, 1).
        /// </summary>
        public (double minValue, double maxValue) MutationRange
        {
            get => _mutationRange;
            set
            {
                if (!DataValidation.IsValid(value, (0, 2)))
                    throw new ArgumentException(
                        "Mutation value should be in range [0, 2) " +
                        "and min value should be less than max value.");
                _mutationRange = value;
            }
        }

        /// <summary>
        ///     Search bounds for variables
        ///     defining the lower and upper borders for optimization variables.
        ///     It should be in form (min, max) for each variable.
        ///     The length of this array should be equal to variables count.
        /// </summary>
        private (double minValue, double maxValue)[] Bounds
        {
            get => _bounds;
            set
            {
                foreach (var bound in value)
                {
                    if (!DataValidation.IsValid(
                        bound, 
                        (double.NegativeInfinity, double.PositiveInfinity)))
                    {
                        throw new ArgumentException(
                            "Min value of bound should be less " +
                            "than max value, " +
                            "and the values should not be equal to infinity.");
                    }
                }
                _bounds = value;
                _scaler = GetScaler(value);
            }
        }

        /// <summary>
        ///     The maximum number of generations 
        ///     which is necessary to limit the optimization process.
        ///     By default it is equal to 1000.
        /// </summary>
        public int MaxIterations
        {
            get => _maxIterations;
            set
            {
                if (!DataValidation.IsValid(
                    value, (1, double.PositiveInfinity)))
                {
                    throw new ArgumentException(
                        "The max iterations value should be " +
                        "in range [1, positive infinity).");
                }
                _maxIterations = value;
            }
        }

        /// <summary>
        ///     Relative tolerance for convergence.
        ///     The optimization process stops
        ///     when the standard deviation of individuals energies
        ///     becomes equal or less than (<see cref="AbsoluteTolerance" /> +
        ///     <see cref="RelativeTolerance" /> * Absolute average
        ///     value of individuals energies).
        ///     By default it is equal to 0.01.
        /// </summary>
        public double RelativeTolerance
        {
            get => _relativeTolerance;
            set
            {
                if (!DataValidation.IsValid(
                    value, (0, double.PositiveInfinity)))
                {
                    throw new ArgumentException(
                        "Relative tolerance should " +
                        "be in range [0, positive infinity).");
                }
                _relativeTolerance = value;
            }
        }

        /// <summary>
        ///     Recombination probability.
        ///     It should be in range [0, 1].
        ///     Increasing this value allows a larger
        ///     number of mutants to progress into the next generation.
        ///     By default it equal to 0.7.
        /// </summary>
        public double RecombProbability
        {
            get => _recombProbability;
            set
            {
                if (!DataValidation.IsValid(value, (0, 1)))
                {
                    throw new ArgumentException(
                        "Recombination probability should" +
                        " be in range [0, 1).");
                }
                _recombProbability = value;
            }
        }

        /// <summary>
        ///     Absolute tolerance for convergence.
        ///     The optimization process stops
        ///     when the standard deviation of individuals
        ///     energies becomes equal or less
        ///     than (<see cref="AbsoluteTolerance" /> +
        ///     <see cref="RelativeTolerance" /> * Absolute average value
        ///     of individuals energies).
        ///     By default it is equal to 0.
        /// </summary>
        public double AbsoluteTolerance
        {
            get => _absoluteTolerance;
            set
            {
                if (!DataValidation.IsValid(
                    value, (0, double.PositiveInfinity)))
                {
                    throw new ArgumentException(
                        "Absolute tolerance should " +
                        "be in range [0, positive infinity).");
                }
                _absoluteTolerance = value;
            }
        }

        /// <summary>
        ///     The value for setting the total population size.
        ///     The population will have
        ///     (<see cref="PopSizeMultiplier" /> * variables count)
        ///     individuals. By default it is equal to 15.
        /// </summary>
        public int PopSizeMultiplier
        {
            get => _popSizeMultiplier;
            set
            {
                if (!DataValidation.IsValid(
                    value, (1, double.PositiveInfinity)))
                {
                    throw new ArgumentException(
                        "Multiplier should be in range" +
                        " [1, positive infinity), " +
                        "Min population size is 5 because 'Rand2Bin' " +
                        "strategy needs at least 5 individuals.");
                }
                _popSizeMultiplier = value;
            }
        }

        /// <summary>
        ///     Display optimization process.
        ///     By default it is equal to true.
        /// </summary>
        public bool ToDisplayOptimization { get; set; } = true;
        #endregion

        #region Service
        /// <summary>
        ///     Verify if the solver converged.
        /// </summary>
        /// <param name="population">Population.</param>
        /// <returns>Flag of convergence.</returns>
        private bool IsSolverConverged(Individual[] population)
        {
            var individEnergies = population.Select(x => x.Energy);
            return MathFunctions.Std(individEnergies) <= AbsoluteTolerance
                + RelativeTolerance * Math.Abs(individEnergies.Average());
        }

        /// <summary>
        ///     The standard deviation of the individual
        ///     energies divided by their mean.
        /// </summary>
        /// <param name="population">Population.</param>
        /// <returns>Convergence of the population.</returns>
        private double GetConvergence(Individual[] population)
        {
            var individEnergies = population.Select(x => x.Energy);
            return MathFunctions.Std(individEnergies)
                   / Math.Abs(individEnergies.Average() + double.Epsilon);
        }

        /// <summary>
        ///     Select random samples for mutation.
        ///     The candidate index can not be among samples.
        /// </summary>
        /// <param name="candidateIndex">Index of candidate.</param>
        /// <param name="populationCount">Population count.</param>
        /// <returns>Indexes of samples.</returns>
        private int[] SelectSamples(int candidateIndex, int populationCount)
        {
            var indexes = new int[populationCount - 1];
            var counter = 0;
            for (var i = 0; i < populationCount; i++)
            {
                if (candidateIndex != i)
                {
                    indexes[counter] = i;
                    counter++;
                }
            }
            return indexes.OrderBy(x => _random.Next()).ToArray();
        }

        /// <summary>
        ///     Set the best solution to the first place in population.
        /// </summary>
        /// <param name="population">Population to sort.</param>
        /// <returns>Sorted population.</returns>
        private Individual[] PromoteLowestEnergy(Individual[] population)
        {
            var bestIndex = 0;
            var bestEnergy = population[0].Energy;
            for (var index = 1; index < population.Length; index++)
            {
                if (population[index].Energy < bestEnergy)
                {
                    bestIndex = index;
                    bestEnergy = population[index].Energy;
                }
            }
            var temp = population[0];
            population[0] = population[bestIndex];
            population[bestIndex] = temp;
            return population;
        }

        /// <summary>
        ///     Make sure the parameters lie between the limits.
        /// </summary>
        /// <param name="trial">Trial individual.</param>
        /// <returns>Checked individual.</returns>
        private Individual EnsureConstraint(Individual trial)
        {
            for (var i = 0; i < trial.Values.Length; i++)
            {
                if ((trial.Values[i] < 0) || (trial.Values[i] > 1))
                {
                    trial.Values[i] = _random.NextDouble();
                }
            }
            return trial;
        }
        #endregion

        #region Scaler
        /// <summary>
        ///     Get scaler parameters.
        /// </summary>
        /// <param name="bounds">Variables bounds.</param>
        /// <returns>Scaler properties.</returns>
        private (double mean, double absDifference)[] GetScaler(
            (double minValue, double maxValue)[] bounds)
        {
            var scaler = new (double mean, double absDifference)[bounds.Length];
            for (var i = 0; i < scaler.Length; i++)
            {
                scaler[i] = (0.5 * (bounds[i].maxValue + bounds[i].minValue),
                    Math.Abs(bounds[i].maxValue - bounds[i].minValue));
            }
            return scaler;
        }

        /// <summary>
        ///     Scale from (0, 1) range to real values.
        /// </summary>
        /// <param name="values">Values.</param>
        /// <returns>Scaled values.</returns>
        private double[] ToScale(double[] values)
        {
            var newValues = new double [values.Length];
            for (var index = 0; index < values.Length; index++)
            {
                newValues[index] = _scaler[index].mean
                                   + _scaler[index].absDifference *
                                   (values[index] - 0.5);
            }
            return newValues;
        }

        /// <summary>
        ///     Unscale from real values to range (0, 1).
        /// </summary>
        /// <param name="values">Scaled values.</param>
        /// <returns>Unscaled values.</returns>
        private double[] UnScale(double[] values)
        {
            var newValues = new double[values.Length];
            for (var index = 0; index < values.Length; index++)
            {
                newValues[index] = (values[index] - _scaler[index].mean)
                    / _scaler[index].absDifference + 0.5;
            }
            return newValues;
        }
        #endregion
    }
}