namespace IsFalse;

/// <summary>
/// Provides extension methods for boolean values.
/// </summary>
public static class BooleanExtensions
{
    /// <summary>
    /// Determines whether the specified boolean value is false.
    /// </summary>
    /// <param name="value">The boolean value to check.</param>
    /// <returns><c>true</c> if the specified value is false; otherwise, <c>false</c>.</returns>
    public static bool IsFalse(this bool value) => !value;

    /// <summary>
    /// Determines whether the specified nullable boolean value is false.
    /// </summary>
    /// <param name="value">The nullable boolean value to check.</param>
    /// <returns>
    /// <c>true</c> if the specified value is false; <c>false</c> if the specified value is true;
    /// <c>null</c> if the specified value is <c>null</c>.
    /// </returns>
    public static bool? IsFalse(this bool? value) => value.HasValue ? !value.Value : null;
}
