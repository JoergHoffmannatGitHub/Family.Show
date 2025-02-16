namespace Genealogy
{
  /// <summary>
  /// Represents a date and provides methods for date manipulation and comparison.
  /// </summary>
  public interface IDate
  {
    /// <summary>
    /// Converts the date to its GEDCOM representation.
    /// </summary>
    /// <returns>A string representing the date in GEDCOM format.</returns>
    string ToGedcom();
  }
}
