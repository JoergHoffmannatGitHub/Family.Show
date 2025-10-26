using System;
using System.Globalization;

using Architect.DomainModeling;

using Genealogy.Domain.Interfaces;

namespace Genealogy.Domain.ValueObjects.DateImplementation;

/// <summary>
/// Represents an exact date with year, month, and day components, supporting multiple calendar systems.
/// </summary>
/// <remarks>The <see cref="DateExact"/> class provides functionality to parse and represent dates in various
/// formats and calendar systems. It supports conversion to GEDCOM format and implements equality comparison with other
/// date objects.</remarks>
[ValueObject]
internal partial class DateExact : IDateExact, IEquatable<IDate>
{
  internal YearMonthDayCalendar YearMonthDayCalendar { get; private init; }

  /// <summary>
  /// Initializes a new instance of the <see cref="DateExact"/> class with a specified date string.
  /// </summary>
  /// <remarks>The <see cref="DateExact"/> constructor parses the provided date string and initializes the <see
  /// cref="YearMonthDayCalendar"/> property. Ensure that the date string is in a recognized format to avoid parsing
  /// errors.</remarks>
  /// <param name="date">The date string to parse into a year, month, and day format. The string must be in a valid date format.</param>
  internal DateExact(string date)
  {
    YearMonthDayCalendar = ParseDate(date);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="DateExact"/> class with the specified year, month, and day.
  /// </summary>
  /// <remarks>This constructor creates a date using the Gregorian calendar. The month and day parameters are
  /// optional and default to 0.</remarks>
  /// <param name="year">The year component of the date.</param>
  /// <param name="month">The month component of the date. Defaults to 0 if not specified.</param>
  /// <param name="day">The day component of the date. Defaults to 0 if not specified.</param>
  internal DateExact(int year, int month = 0, int day = 0)
  {
    YearMonthDayCalendar = new YearMonthDayCalendar(year, month, day, CalendarOrdinal.Gregorian);
  }

  #region IDate

  /// <summary>
  /// Converts the date to its GEDCOM representation.
  /// </summary>
  /// <returns>A string representing the date in GEDCOM format.</returns>
  public override string ToString()
  {
    string result = string.Empty;

    // only output type if it isn't the default (Gregorian)
    if (YearMonthDayCalendar.CalendarOrdinal != CalendarOrdinal.Gregorian)
    {
      result += CalendarSystem.ForOrdinal(YearMonthDayCalendar.CalendarOrdinal).Escape;
    }

    if (YearMonthDayCalendar.Day > 0)
    {
      result += YearMonthDayCalendar.Day + " ";
    }

    if (YearMonthDayCalendar.Month > 0)
    {
      result += GetMMM(YearMonthDayCalendar.Month) + " ";
    }

    return result + YearMonthDayCalendar.Year;
  }

  #endregion IDate

  #region IDateExact

  /// <inheritdoc />
  public int Year => YearMonthDayCalendar.Year;

  /// <inheritdoc />
  public int Month => YearMonthDayCalendar.Month;

  /// <inheritdoc />
  public int Day => YearMonthDayCalendar.Day;

  #endregion IDateExact

  #region IEquatable<IDate>

  /// <summary>
  /// Determines whether the specified <see cref="IDate"/> is equal to the current instance.
  /// </summary>
  /// <param name="obj">The <see cref="IDate"/> to compare with the current instance.</param>
  /// <returns><see langword="true"/> if the specified <see cref="IDate"/> is equal to the current instance; otherwise, <see
  /// langword="false"/>.</returns>
  public bool Equals(IDate obj) => obj != null && obj is DateExact other && YearMonthDayCalendar == other.YearMonthDayCalendar;

  #endregion

  /// <summary>
  /// Parses a date string into a <see cref="YearMonthDayCalendar"/> object.
  /// </summary>
  /// <param name="date">The date string to parse, which must be in one of the following formats: "d MMM yyyy", "yyyy-MM-dd",
  /// "yyyy-MM-ddThh:mm:ss", "MMM yyyy", or "yyyy".</param>
  /// <returns>A <see cref="YearMonthDayCalendar"/> object representing the parsed date, with year, month, and day components as
  /// specified in the input string.</returns>
  /// <exception cref="GenealogyException">Thrown if the date string is less than 4 characters long.</exception>
  /// <exception cref="NotImplementedException">Thrown if the date string cannot be parsed into any of the supported formats.</exception>
  private static YearMonthDayCalendar ParseDate(string date)
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

    if (DateTime.TryParseExact(date, ["d MMM yyyy", "yyyy-MM-dd", "yyyy-MM-ddThh:mm:ss"], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
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

    return new YearMonthDayCalendar(year, month, day, ordinal);
  }

  /// <summary>
  /// Parses the calendar ordinal from the specified date string.
  /// </summary>
  /// <remarks>The method identifies the calendar type by checking for specific prefixes in the date string.
  /// Supported calendar types include Gregorian, Julian, Hebrew, French Republican, Roman, and Unknown.</remarks>
  /// <param name="date">A reference to the date string to parse. The method modifies this string to remove the calendar type prefix.</param>
  /// <returns>A <see cref="CalendarOrdinal"/> value representing the calendar type found in the date string. Defaults to <see
  /// cref="CalendarOrdinal.Gregorian"/> if no recognized prefix is found.</returns>
  private static CalendarOrdinal ParseCalendarOrdinal(ref string date)
  {
    string dateType = string.Empty;
    if (date.StartsWith("@#"))
    {
      int i = date.IndexOf('@', 2);
      if (i != -1)
      {
        dateType = date.Substring(0, i + 1).ToUpper();
        date = date.Substring(i + 1);
      }
    }

    CalendarOrdinal ordinal = dateType switch
    {
      "@#DGREGORIAN@" => CalendarOrdinal.Gregorian,
      "@#DJULIAN@" => CalendarOrdinal.Julian,
      "@#DHEBREW@" => CalendarOrdinal.Hebrew,
      "@#DFRENCH R@" => CalendarOrdinal.FrenchRepublican,
      "@#DROMAN@" => CalendarOrdinal.Roman,
      "@#DUNKNOWN@" => CalendarOrdinal.Unknown,
      _ => CalendarOrdinal.Gregorian,
    };

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
    return month switch
    {
      1 => "Jan",
      2 => "Feb",
      3 => "Mar",
      4 => "Apr",
      5 => "May",
      6 => "Jun",
      7 => "Jul",
      8 => "Aug",
      9 => "Sep",
      10 => "Oct",
      11 => "Nov",
      12 => "Dec",
      _ => throw new GenealogyException("Not a valid month in date."),
    };
    ;
  }
}
