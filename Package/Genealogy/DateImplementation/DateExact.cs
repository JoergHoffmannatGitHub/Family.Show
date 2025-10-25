using System;
using System.Globalization;

namespace Genealogy.DateImplementation
{
  /// <summary>
  /// Represents an exact date with year, month, and day components.
  /// </summary>
  internal class DateExact : IDateExact, IEquatable<IDate>
  {
    private YearMonthDayCalendar _yearMonthDayCalendar;

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
      _yearMonthDayCalendar = new YearMonthDayCalendar(year, month, day, CalendarOrdinal.Gregorian);
    }

    #region IDate

    /// <inheritdoc />
    public string ToGedcom()
    {
      string result = string.Empty;

      // only output type if it isn't the default (Gregorian)
      if (_yearMonthDayCalendar.CalendarOrdinal != CalendarOrdinal.Gregorian)
      {
        result += CalendarSystem.ForOrdinal(_yearMonthDayCalendar.CalendarOrdinal).Escape;
      }

      if (_yearMonthDayCalendar.Day > 0)
      {
        result += _yearMonthDayCalendar.Day + " ";
      }

      if (_yearMonthDayCalendar.Month > 0)
      {
        result += GetMMM(_yearMonthDayCalendar.Month) + " ";
      }

      return result + _yearMonthDayCalendar.Year;
    }

    #endregion IDate

    #region IDateExact

    /// <inheritdoc />
    public int Year => _yearMonthDayCalendar.Year;

    /// <inheritdoc />
    public int Month => _yearMonthDayCalendar.Month;

    /// <inheritdoc />
    public int Day => _yearMonthDayCalendar.Day;

    #endregion IDateExact

    #region IEquatable<DateExact>

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(IDate obj) => obj != null && obj is DateExact other && _yearMonthDayCalendar == other._yearMonthDayCalendar;

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
    public override int GetHashCode() => _yearMonthDayCalendar.GetHashCode();

    #endregion

    /// <summary>
    /// Parses the specified date string and sets the year, month, and day properties.
    /// </summary>
    /// <param name="date">The date string to parse.</param>
    /// <exception cref="GenealogyException">Thrown when the date string is invalid.</exception>
    private void ParseDate(string date)
    {
      CalendarOrdinal ordinal = ParseCalendarOrdinal(ref date);
      int year;
      int month;
      int day;

      // There is a minimum length of 4 characters
      if (date.Length < 4)
      {
        throw new GenealogyException("Invalid Date: Must have at least YYYY");
      }

      if (DateTime.TryParseExact(date, new string[] { "d MMM yyyy", "yyyy-MM-dd", "yyyy-MM-ddThh:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
      {
        // year, month and day are given
        year = dateValue.Year;
        month = dateValue.Month;
        day = dateValue.Day;
      }
      else if (DateTime.TryParseExact(date, "MMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
      {
        // year and month are given
        year = dateValue.Year;
        month = dateValue.Month;
        day = 0;
      }
      else if (DateTime.TryParseExact(date, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
      {
        // only year is given
        year = dateValue.Year;
        month = 0;
        day = 0;
      }
      else
      {
        Console.WriteLine("  Unable to parse '{0}'.", date);
        throw new NotImplementedException();
      }

      _yearMonthDayCalendar = new YearMonthDayCalendar(year, month, day, ordinal);
    }

    private static CalendarOrdinal ParseCalendarOrdinal(ref string date)
    {
      string dateType = string.Empty;
      if (date.StartsWith("@#"))
      {
        int i = date.IndexOf("@", 2);
        if (i != -1)
        {
          dateType = date.Substring(0, i + 1).ToUpper();
          date = date.Substring(i + 1);
        }
      }

      CalendarOrdinal ordinal;
      switch (dateType)
      {
        case "@#DGREGORIAN@":
          ordinal = CalendarOrdinal.Gregorian;
          break;
        case "@#DJULIAN@":
          ordinal = CalendarOrdinal.Julian;
          break;
        case "@#DHEBREW@":
          ordinal = CalendarOrdinal.Hebrew;
          break;
        case "@#DFRENCH R@":
          ordinal = CalendarOrdinal.FrenchRepublican;
          break;
        case "@#DROMAN@":
          ordinal = CalendarOrdinal.Roman;
          break;
        case "@#DUNKNOWN@":
          ordinal = CalendarOrdinal.Unknown;
          break;
        default:
          ordinal = CalendarOrdinal.Gregorian;
          break;
      }

      return ordinal;
    }

    /// <summary>
    /// Gets the three-letter month abbreviation for the specified month number.
    /// </summary>
    /// <param name="month">The month number (1-12).</param>
    /// <returns>The three-letter month abbreviation.</returns>
    /// <exception cref="NotImplementedException">Thrown when the month number is invalid.</exception>
    private static string GetMMM(int month)
    {
      // Use IniCaps because it's more readable.
      switch (month)
      {
        case 1:
          return "Jan";
        case 2:
          return "Feb";
        case 3:
          return "Mar";
        case 4:
          return "Apr";
        case 5:
          return "May";
        case 6:
          return "Jun";
        case 7:
          return "Jul";
        case 8:
          return "Aug";
        case 9:
          return "Sep";
        case 10:
          return "Oct";
        case 11:
          return "Nov";
        case 12:
          return "Dec";
        default:
          throw new GenealogyException("Not a valid month in date.");
      }
      ;
    }
  }
}
