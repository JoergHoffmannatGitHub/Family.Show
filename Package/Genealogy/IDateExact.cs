namespace Genealogy
{
  /// <summary>
  /// Represents an exact date with year, month, and day components.
  /// </summary>
  public interface IDateExact : IDate
  {
    /// <summary>
    /// Gets the year component of the date.
    /// </summary>
    int Year { get; }

    /// <summary>
    /// Gets the month component of the date.
    /// </summary>
    int Month { get; }

    /// <summary>
    /// Gets the day component of the date.
    /// </summary>
    int Day { get; }
  }
}
