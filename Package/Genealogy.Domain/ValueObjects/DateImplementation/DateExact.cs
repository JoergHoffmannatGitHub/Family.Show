using System;
using System.Globalization;
using System.Linq;

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
internal partial class DateExact : IDateExact
{
    /// <summary>
    /// Represents the prefix used to denote years occurring before the birth of Christ (B.C.).
    /// </summary>
    private const string BeforeCristusPrefix = "B.C.";

    /// <summary>
    /// The character used to indicate an alternative year value (e.g., "1699/00") in date strings.
    /// </summary>
    private const char YearModifier = '/';

    /// <summary>
    /// Gets the compact representation of year, month, day, and calendar system.
    /// </summary>
    internal YearMonthDayCalendar YearMonthDayCalendar { get; private init; }

    /// <summary>
    /// Gets a value indicating whether the date has an alternative year value (e.g., "1699/00").
    /// </summary>
    internal bool YearAlternative { get; private set; }

    /// <summary>
    /// Attempts to parse the specified date string into a <see cref="DateExact"/> object.
    /// </summary>
    /// <param name="date">The date string to parse.</param>
    /// <param name="result">When this method returns, contains the parsed <see cref="DateExact"/> object
    /// if parsing succeeded; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the date string was successfully parsed; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string date, out DateExact result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(date) || date.Length < 4)
        {
            return false;
        }

        try
        {
            result = new DateExact(date);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateExact"/> class with a specified date string.
    /// </summary>
    /// <remarks>The <see cref="DateExact"/> constructor parses the provided date string and initializes the <see
    /// cref="YearMonthDayCalendar"/> property. Ensure that the date string is in a recognized format to avoid parsing
    /// errors.</remarks>
    /// <param name="date">
    /// The date string to parse into a year, month, and day format. The string must be in a valid date format.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="date"/> is null.</exception>
    /// <exception cref="GenealogyException">Thrown if <paramref name="date"/> is less than 4 characters.</exception>
    internal DateExact(string date)
    {
        ArgumentNullException.ThrowIfNull(date);

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
        if (month == 0 && day != 0)
        {
            throw new GenealogyException("Month cannot be zero, when day is not zero.");
        }

        YearMonthDayCalendar = new YearMonthDayCalendar(year, month, day, CalendarOrdinal.Gregorian);
    }

    #region IDate

    /// <inheritdoc/>
    public override string ToString()
    {
        string[] dateParts = new string[5];
        int part = 0;
        // only output type if it isn't the default (Gregorian)
        if (YearMonthDayCalendar.CalendarOrdinal != CalendarOrdinal.Gregorian)
        {
            dateParts[part++] = CalendarSystem.ForOrdinal(YearMonthDayCalendar.CalendarOrdinal).Escape;
        }

        if (YearMonthDayCalendar.Day > 0)
        {
            dateParts[part++] = YearMonthDayCalendar.Day.ToString();
        }

        if (YearMonthDayCalendar.Month > 0)
        {
            string[] monthNames = CalendarSystem.MonthNames(YearMonthDayCalendar.CalendarOrdinal);
            if (YearMonthDayCalendar.Month > monthNames.Length)
            {
                throw new GenealogyException("Not a valid month in date.");
            }

            dateParts[part++] = monthNames[YearMonthDayCalendar.Month - 1];
        }

        if (Year < 0)
        {
            dateParts[part++] = (-YearMonthDayCalendar.Year).ToString();
            dateParts[part++] = BeforeCristusPrefix;
        }
        else
        {
            dateParts[part++] = YearMonthDayCalendar.Year.ToString();
            if (YearAlternative)
            {
                dateParts[part - 1] += YearModifier + ((YearMonthDayCalendar.Year + 1) % 100).ToString("00");
            }
        }

        return string.Join(" ", dateParts, 0, part);
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

    /// <summary>
    /// Parses a date string into a <see cref="YearMonthDayCalendar"/> object.
    /// </summary>
    /// <param name="date">
    /// The date string to parse, which must be in one of the following formats: "d MMM yyyy", "yyyy-MM-dd",
    /// "yyyy-MM-ddThh:mm:ss", "MMM yyyy", or "yyyy".
    /// </param>
    /// <returns>
    /// A <see cref="YearMonthDayCalendar"/> object representing the parsed date, with year, month, and day components
    /// as specified in the input string.
    /// </returns>
    /// <exception cref="GenealogyException">Thrown if the date string is less than 4 characters long.</exception>
    /// <exception cref="NotImplementedException">
    /// Thrown if the date string cannot be parsed into any of the supported formats.
    /// </exception>
    private YearMonthDayCalendar ParseDate(string date)
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

        if (DateTime.TryParseExact(
            date,
            ["d MMM yyyy", "yyyy-MM-dd", "yyyy-MM-ddThh:mm:ss"],
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime dateValue))
        {
            // year, month and day are given
            year = dateValue.Year;
            month = dateValue.Month;
            day = dateValue.Day;
        }
        else if (DateTime.TryParseExact(
            date,
            "MMM yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out dateValue))
        {
            // year and month are given
            year = dateValue.Year;
            month = dateValue.Month;
            day = 0;
        }
        else if (DateTime.TryParseExact(
            date,
            "yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out dateValue))
        {
            // only year is given
            year = dateValue.Year;
            month = 0;
            day = 0;
        }
        else
        {
            bool bc = false;
            if (date.EndsWith(BeforeCristusPrefix, true, CultureInfo.InvariantCulture))
            {
                bc = true;
                date = date.Substring(0, date.Length - BeforeCristusPrefix.Length);
            }

            string[] dateSplit = date.Split([' ', '-'], StringSplitOptions.RemoveEmptyEntries);
            string yearString = string.Empty;
            string monthString = string.Empty;
            string dayString = string.Empty;

            if (dateSplit.Length == 1)
            {
                yearString = dateSplit[0];
            }
            else if (dateSplit.Length == 2)
            {
                monthString = dateSplit[0];
                yearString = dateSplit[1];
            }
            else if (dateSplit.Length == 3)
            {
                dayString = dateSplit[0];
                monthString = dateSplit[1];
                yearString = dateSplit[2];
            }

            day = ParseDay(dayString);
            month = ParseMonth(monthString, ordinal);
            year = ParseYear(yearString, ordinal);

            if (bc)
            {
                if (day != 0 || month != 0)
                {
                    throw new GenealogyException($"'{BeforeCristusPrefix}' dates can only have year component.");
                }

                year *= -1;
            }
        }

        return new YearMonthDayCalendar(year, month, day, ordinal);
    }

    /// <summary>
    /// Parses the year component from a string representation.
    /// </summary>
    /// <param name="yearString">The string containing the year to parse.</param>
    /// <param name="ordinal">
    /// The calendar ordinal used to determine the set of month names for parsing.
    /// This is required when the input is a month name.
    /// </param>
    /// <returns>The parsed year as an integer.</returns>
    /// <exception cref="GenealogyException">
    /// Thrown when the <paramref name="yearString"/> cannot be parsed as an integer.
    /// </exception>
    private int ParseYear(string yearString, CalendarOrdinal ordinal)
    {
        // year could be of the form 1699/00
        // have 2 datetimes for each date ?
        // only having 1 won't lose the data, could prevent proper merge
        // though as the DateTime will be used for comparison
        int YearAlternativeValue = -1;
        if (yearString.Contains(YearModifier))
        {
            if (ordinal != CalendarOrdinal.Gregorian)
            {
                throw new GenealogyException("Year alternatives are only supported for Gregorian calendar.");
            }

            YearAlternative = true;
            int yearModifierIndex = yearString.IndexOf(YearModifier);
            _ = int.TryParse(yearString.AsSpan(yearModifierIndex + 1, 2), out YearAlternativeValue);
            yearString = yearString.Substring(0, yearModifierIndex);
        }

        bool parseResult = int.TryParse(yearString, out int year);
        if (YearAlternativeValue != -1 && YearAlternativeValue != ((year % 100) + 1) % 100)
        {
            throw new GenealogyException($"The date alternatives for {year}{YearModifier}{YearAlternativeValue} are not displayed with the following year. ");
        }

        return !parseResult ? throw new GenealogyException($"Unable to parse year '{yearString}'.") : year;
    }

    /// <summary>
    /// Parses a month string and converts it to its corresponding numeric representation.
    /// </summary>
    /// <param name="monthString">
    /// The string representation of the month. This can be a numeric string (e.g., "1" for January)  or a month name
    /// (e.g., "January"). The comparison is case-insensitive.
    /// </param>
    /// <param name="ordinal">
    /// The calendar ordinal used to determine the set of month names for parsing.
    /// This is required when the input is a month name.
    /// </param>
    /// <returns>
    /// An integer representing the month, where 1 corresponds to January, 2 to February, and so on.
    /// </returns>
    /// <exception cref="GenealogyException">
    /// Thrown if <paramref name="monthString"/> is not a valid numeric month or does not match any
    /// month name in the specified calendar system.
    /// </exception>
    private static int ParseMonth(string monthString, CalendarOrdinal ordinal)
    {
        if ((!int.TryParse(monthString, out int month)) && monthString != string.Empty)
        {
            string[] monthNames = [.. CalendarSystem.MonthNames(ordinal).Select(s => s.ToUpperInvariant())];

            int monthIndex = Array.IndexOf(monthNames, monthString.ToUpperInvariant());

            if (monthIndex == -1)
            {
                throw new GenealogyException("Not a valid month in date.");
            }

            month = monthIndex + 1;
        }

        return month;
    }

    /// <summary>
    /// Parses a string representation of a day into an integer.
    /// </summary>
    /// <param name="dayString">
    /// The string representation of the day to parse. This can be a numeric string or an empty string.
    /// </param>
    /// <returns>
    /// The parsed integer value of the day. If <paramref name="dayString"/> is empty, returns 0.
    /// </returns>
    /// <exception cref="GenealogyException">
    /// Thrown if <paramref name="dayString"/> is not a valid numeric string and is not empty.
    /// </exception>
    private static int ParseDay(string dayString)
    {
        return !int.TryParse(dayString, out int day) && dayString != string.Empty
            ? throw new GenealogyException($"Unable to parse day '{dayString}'.")
            : day;
    }

    /// <summary>
    /// Parses the calendar ordinal from the specified date string.
    /// </summary>
    /// <remarks>The method identifies the calendar type by checking for specific prefixes in the date string.
    /// Supported calendar types include Gregorian, Julian, Hebrew, French Republican, Roman, and Unknown.</remarks>
    /// <param name="date">
    /// A reference to the date string to parse. The method modifies this string to remove the calendar type prefix.
    /// </param>
    /// <returns>
    /// A <see cref="CalendarOrdinal"/> value representing the calendar type found in the date string. Defaults to <see
    /// cref="CalendarOrdinal.Gregorian"/> if no recognized prefix is found.
    /// </returns>
    private static CalendarOrdinal ParseCalendarOrdinal(ref string date)
    {
        string dateType = string.Empty;
        if (date.StartsWith("@#"))
        {
            int i = date.IndexOf('@', 2);
            if (i != -1)
            {
                dateType = date[..(i + 1)].ToUpper();
                date = date[(i + 1)..].Trim();
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
}
