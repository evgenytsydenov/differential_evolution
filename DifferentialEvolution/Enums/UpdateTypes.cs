namespace DifferentialEvolution
{
    /// <summary>
    ///     Types of updating the best solution vector.
    /// </summary>
    public enum UpdateTypes
    {
        /// <summary>
        ///     The best solution vector is continuously updated
        ///     within a single generation. It takes advantage of continuous
        ///     improvements and can lead to faster convergence.
        /// </summary>
        Immediate,

        /// <summary>
        ///     The best solution vector is updated per generation.
        /// </summary>
        Deferred
    }
}