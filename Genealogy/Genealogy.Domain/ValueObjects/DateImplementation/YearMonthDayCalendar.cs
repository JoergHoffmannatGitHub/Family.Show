// based on NodaTime.YearMonthDayCalendar (see: https://github.com/nodatime/nodatime)

using System;

namespace Genealogy.Domain.ValueObjects.DateImplementation;

/// <summary>
/// A compact representation of a year, month, and day and calendar ordinal (integer ID) in a single 32-bit integer.
/// </summary>
/// <remarks>
/// <para>
/// The calendar is represented in bits 0-5.
/// The day is represented in bits 6-11.
/// The month is represented in bits 12-16.
/// The year is represented in bits 17-31. (It's convenient to put this at the top as it can be negative.)
///
/// This type does not implement IComparable[YearMonthDayCalendar] as it turns out it doesn't need to:
/// comparisons are always done through the calendar system, which uses YearMonthDay instead. We could potentially
/// optimize by bypassing the calendar and embedding knowledge of calendars which have "odd" month numberings
/// in here, but it would be a bit of a design smell.
///
/// Equality is easily tested, however, as it can check for calendar equality.
/// </para>
/// <para>
/// In contrast to NodaTime, the representation 0 for 1 is only used for year.
/// 0 for day or for month and day means that the specification is missing.
/// </para>
/// </remarks>
internal readonly struct YearMonthDayCalendar : IEquatable<YearMonthDayCalendar>
{
    // These constants are internal so they can be used in YearMonthDay
    internal const int CalendarBits = 4; // Up to 16 calendars.
    internal const int DayBits = 6;   // Up to 64 days in a month.
    internal const int MonthBits = 5; // Up to 32 months per year.
    private const int YearBits = 17; // 128K range; only need -10K to +10K.

    // Just handy constants to use for shifting and masking.
    private const int CalendarDayBits = CalendarBits + DayBits;
    private const int CalendarDayMonthBits = CalendarDayBits + MonthBits;

    private const int CalendarMask = (1 << CalendarBits) - 1;
    private const int DayMask = (1 << DayBits) - 1 << CalendarBits;
    private const int MonthMask = (1 << MonthBits) - 1 << CalendarDayBits;
    private const int YearMask = (1 << YearBits) - 1 << CalendarDayMonthBits;

    private readonly int _dateBits;

    /// <summary>
    /// Constructs a new value for the given year, month, day and calendar. No validation is performed.
    /// </summary>
    /// <param name="year">The year of the date.</param>
    /// <param name="month">The month of the date.</param>
    /// <param name="day">The day of the date.</param>
    internal YearMonthDayCalendar(int year, int month, int day, CalendarOrdinal calendarOrdinal)
    {
        unchecked
        {
            _dateBits = year - 1 << CalendarDayMonthBits |
              month << CalendarDayBits |
              day << CalendarBits |
              (int)calendarOrdinal;
        }
    }

    internal CalendarOrdinal CalendarOrdinal => (CalendarOrdinal)unchecked(_dateBits & CalendarMask);

    /// <summary>
    /// Gets the year component of the date.
    /// </summary>
    internal int Year => unchecked(((_dateBits & YearMask) >> CalendarDayMonthBits) + 1);

    /// <summary>
    /// Gets the month component of the date.
    /// </summary>
    internal int Month => unchecked((_dateBits & MonthMask) >> CalendarDayBits);

    /// <summary>
    /// Gets the day component of the date.
    /// </summary>
    internal int Day => unchecked((_dateBits & DayMask) >> CalendarBits);

    public static bool operator ==(YearMonthDayCalendar lhs, YearMonthDayCalendar rhs) =>
        lhs._dateBits == rhs._dateBits;

    public static bool operator !=(YearMonthDayCalendar lhs, YearMonthDayCalendar rhs) =>
        lhs._dateBits != rhs._dateBits;

    /// <summary>
    /// Indicates whether the current instance is equal to another <see cref="YearMonthDayCalendar"/> instance.
    /// </summary>
    /// <param name="other">The other <see cref="YearMonthDayCalendar"/> instance to compare.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(YearMonthDayCalendar other) => _dateBits == other._dateBits;

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <see langword="true"/> if the specified object is equal to the current instance;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(object obj) => obj is YearMonthDayCalendar other && Equals(other);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override int GetHashCode() => _dateBits;
}
