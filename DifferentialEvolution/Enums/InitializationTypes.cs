namespace DifferentialEvolution
{
    /// <summary>
    ///     Types of population initialization.
    /// </summary>
    public enum InitTypes
    {
        /// <summary>
        ///     Latin Hypercube type tries to maximize
        ///     coverage of the available parameter space.
        /// </summary>
        LatinHyperCube,

        /// <summary>
        ///     Initializes the population randomly.
        /// </summary>
        Random
    }
}