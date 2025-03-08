using System;
using System.Globalization;

namespace Genealogy.DateImplementation
{
  /// <summary>
  /// Represents an exact date with year, month, and day components.
  /// </summary>
  internal class DateExact : IDateExact, IEquatable<IDate>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DateExact"/> class with the specified date string.
    /// </summary>
    /// <param name="date">The date string to parse.</param>
    /// <exception cref="GenealogyException">Thrown when the date string is invalid.</exception>
    internal DateExact(string date)
    {
      ParseDate(date);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateExact"/> class with the specified year, month, and day.
    /// </summary>
    /// <param name="year">The year of the date.</param>
    /// <param name="month">The month of the date. Default is 0.</param>
    /// <param name="day">The day of the date. Default is 0.</param>
    internal DateExact(int year, int month = 0, int day = 0)
    {
      Year = year;
      Month = month;
      Day = day;
    }

    #region IDate

    /// <inheritdoc />
    public string ToGedcom()
    {
      string result = string.Empty;
      if (Day > 0)
      {
        result += Day + " ";
      }

      if (Month > 0)
      {
        result += GetMMM(Month) + " ";
      }

      return result + Year;
    }

    #endregion IDate

    #region IDateExact

    /// <inheritdoc />
    public int Year { get; private set; }

    /// <inheritdoc />
    public int Month { get; private set; }

    /// <inheritdoc />
    public int Day { get; private set; }

    #endregion IDateExact

    #region IEquatable<DateExact>

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(IDate obj) => obj != null && obj is DateExact other && Year == other.Year && Month == other.Month && Day == other.Day;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj) => obj is DateExact other && Equals(other);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
      int hashCode = 592158470;
      hashCode = hashCode * -1521134295 + Year.GetHashCode();
      hashCode = hashCode * -1521134295 + Month.GetHashCode();
      hashCode = hashCode * -1521134295 + Day.GetHashCode();
      return hashCode;
    }

    #endregion

    /// <summary>
    /// Parses the specified date string and sets the year, month, and day properties.
    /// </summary>
    /// <param name="date">The date string to parse.</param>
    /// <exception cref="GenealogyException">Thrown when the date string is invalid.</exception>
    private void ParseDate(string date)
    {
      // There is a minimum length of 4 characters
      if (date.Length < 4)
      {
        throw new GenealogyException("Invalid Date: Must have at least YYYY");
      }

      if (DateTime.TryParseExact(date, new string[] { "d MMM yyyy", "yyyy-MM-dd", "yyyy-MM-ddThh:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
      {
        // year, month and day are given
        Year = dateValue.Year;
        Month = dateValue.Month;
        Day = dateValue.Day;
      }
      else if (DateTime.TryParseExact(date, "MMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
      {
        // year and month day are given
        Year = dateValue.Year;
        Month = dateValue.Month;
      }
      else if (DateTime.TryParseExact(date, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
      {
        // only year is given
        Year = dateValue.Year;
      }
      else
      {
        Console.WriteLine("  Unable to parse '{0}'.", date);
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Gets the three-letter month abbreviation for the specified month number.
    /// </summary>
    /// <param name="month">The month number (1-12).</param>
    /// <returns>The three-letter month abbreviation.</returns>
    /// <exception cref="NotImplementedException">Thrown when the month number is invalid.</exception>
    private static string GetMMM(int month)
    {
      switch (month)
      {
        case 1:
          return "JAN";
        case 2:
          return "FEB";
        case 3:
          return "MAR";
        case 4:
          return "APR";
        case 5:
          return "MAY";
        case 6:
          return "JUN";
        case 7:
          return "JUL";
        case 8:
          return "AUG";
        case 9:
          return "SEP";
        case 10:
          return "OCT";
        case 11:
          return "NOV";
        case 12:
          return "DEC";
        default:
          throw new GenealogyException("Not a valid month in date.");
      };
    }
  }
}
