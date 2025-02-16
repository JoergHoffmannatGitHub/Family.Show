using Genealogy.DateImplementation;

namespace Genealogy
{
  /// <summary>
  /// Provides methods for creating and parsing date objects.
  /// </summary>
  public class Date
  {
    /// <summary>
    /// Creates an <see cref="IDate"/> object with the specified year, month, and day.
    /// </summary>
    /// <param name="year">The year of the date.</param>
    /// <param name="month">The month of the date. Default is 0.</param>
    /// <param name="day">The day of the date. Default is 0.</param>
    /// <returns>An <see cref="IDate"/> object representing the specified date.</returns>
    public static IDate Create(int year, int month = 0, int day = 0)
    {
      return new DateExact(year, month, day);
    }

    /// <summary>
    /// Tries to parse the specified date string and returns a value that indicates whether
    /// the parsing succeeded.
    /// </summary>
    /// <param name="date">The date string to parse.</param>
    /// <param name="result">When this method returns, contains the parsed <see cref="IDate"/>
    /// object if the parsing succeeded, or a default value if the parsing failed.</param>
    /// <returns><c>true</c> if the date string was parsed successfully; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string date, out IDate result)
    {
      try
      {
        result = Parse(date);
        return true;
      }
      catch
      {
        result = new DateExact(0);
        return false;
      }
    }

    /// <summary>
    /// Parses the specified date string and returns an <see cref="IDate"/> object.
    /// </summary>
    /// <param name="date">The date string to parse.</param>
    /// <returns>An <see cref="IDate"/> object representing the parsed date.</returns>
    /// <exception cref="GenealogyException">Thrown when the date string is null or empty.</exception>
    private static IDate Parse(string date)
    {
      if (string.IsNullOrEmpty(date))
      {
        throw new GenealogyException("Invalid Date");
      }

      if (date.StartsWith("BEF ") ||
        date.StartsWith("AFT ") ||
        date.StartsWith("BET "))
      {
        // DateRange
        return new SimpleDate(date);
      }
      else if (date.StartsWith("FROM ") ||
        date.StartsWith("TO "))
      {
        // DatePeriod
        return new SimpleDate(date);
      }
      else if (date.Contains("ABT ") ||
        date.StartsWith("CAL ") ||
        date.StartsWith("EST "))
      {
        // DateApproximated
        return new SimpleDate(date);
      }

      return new DateExact(date);
    }
  }
}
