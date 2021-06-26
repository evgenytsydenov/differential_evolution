namespace DifferentialEvolution
{
    /// <summary>
    ///     Data validation functions.
    /// </summary>
    internal class DataValidation
    {
        /// <summary>
        ///     Validate a double value.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="range">Range in the form (min, max).</param>
        /// <returns>True if valid, False otherwise</returns>
        internal static bool IsValid(
            double value, (double minValue, double maxValue) range)
        {
            return IsValid(value)
                   && !(value < range.minValue)
                   && !(value >= range.maxValue);
        }

        /// <summary>
        ///     Validate an integer value.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="range">Range in the form (min, max).</param>
        /// <returns>True if valid, False otherwise</returns>
        internal static bool IsValid(
            int value, (double minValue, double maxValue) range)
        {
            return !(value < range.minValue) && !(value >= range.maxValue);
        }

        /// <summary>
        ///     Validate a double value.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>True if valid, False otherwise</returns>
        internal static bool IsValid(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        /// <summary>
        ///     Validate a range of doubles.
        /// </summary>
        /// <param name="value">Value as a range in the form (min, max).</param>
        /// <param name="range">Range in the form (min, max).</param>
        /// <returns>True if valid, False otherwise</returns>
        internal static bool IsValid(
            (double minValue, double maxValue) value,
            (double minValue, double maxValue) range)
        {
            return IsValid(value.minValue)
                   && IsValid(value.maxValue)
                   && !(value.minValue < range.minValue)
                   && !(value.maxValue >= range.maxValue)
                   && !(value.minValue >= value.maxValue);
        }
    }
}