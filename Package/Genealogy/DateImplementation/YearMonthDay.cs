// based on NodaTime.YearMonthDay (see: https://github.com/nodatime/nodatime)

using System;

namespace Genealogy.DateImplementation
{
  /// <summary>
  /// A compact representation of a year, month, and day  in a single 32-bit integer.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This struct uses bitwise operations to store and retrieve year, month, and day values efficiently.
  /// </para>
  /// <para>
  /// In contrast to NodaTime, the representation 0 for 1 is only used for year.
  /// 0 for day or for month and day means that the specification is missing.
  /// </para>
  /// </remarks>
  internal readonly struct YearMonthDay : IComparable<YearMonthDay>, IEquatable<YearMonthDay>
  {
    private const int DayBits = 6;   // Up to 64 days in a month.
    private const int MonthBits = 5; // Up to 32 months per year.

    private const int DayMonthBits = DayBits + MonthBits;

    private const int DayMask = (1 << DayBits) - 1;
    private const int MonthMask = ((1 << MonthBits) - 1) << DayBits;

    private readonly int _dateBits;

    /// <summary>
    /// Constructs a new value for the given year, month and day. No validation is performed.
    /// </summary>
    /// <param name="year">The year of the date.</param>
    /// <param name="month">The month of the date.</param>
    /// <param name="day">The day of the date.</param>
    public YearMonthDay(int year, int month, int day)
    {
      unchecked
      {
        _dateBits = ((year - 1) << DayMonthBits) | (month << DayBits) | day;
      }
    }

    /// <summary>
    /// Gets the year component of the date.
    /// </summary>
    internal int Year => unchecked((_dateBits >> DayMonthBits) + 1);

    /// <summary>
    /// Gets the month component of the date.
    /// </summary>
    internal int Month => unchecked((_dateBits & MonthMask) >> DayBits);

    /// <summary>
    /// Gets the day component of the date.
    /// </summary>
    internal int Day => unchecked(_dateBits & DayMask);

    /// <summary>
    /// Compares the current instance with another <see cref="YearMonthDay"/> instance.
    /// </summary>
    /// <param name="other">The other <see cref="YearMonthDay"/> instance to compare.</param>
    /// <returns>
    /// A value less than zero if this instance is earlier than <paramref name="other"/>, 
    /// zero if they are equal, or greater than zero if this instance is later than <paramref name="other"/>.
    /// </returns>
    public int CompareTo(YearMonthDay other) => _dateBits.CompareTo(other._dateBits);

    /// <summary>
    /// Indicates whether the current instance is equal to another <see cref="YearMonthDay"/> instance.
    /// </summary>
    /// <param name="other">The other <see cref="YearMonthDay"/> instance to compare.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(YearMonthDay other)
    {
      return _dateBits == other._dateBits;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true"/> if the specified object is equal to the current instance; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object obj) => obj is YearMonthDay other && Equals(other);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override int GetHashCode() => _dateBits;

    /// <summary>
    /// Determines whether two <see cref="YearMonthDay"/> instances are equal.
    /// </summary>
    /// <param name="lhs">The first <see cref="YearMonthDay"/> instance to compare.</param>
    /// <param name="rhs">The second <see cref="YearMonthDay"/> instance to compare.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(YearMonthDay lhs, YearMonthDay rhs) => lhs._dateBits == rhs._dateBits;

    /// <summary>
    /// Determines whether two <see cref="YearMonthDay"/> instances are not equal.
    /// </summary>
    /// <param name="lhs">The first <see cref="YearMonthDay"/> instance to compare.</param>
    /// <param name="rhs">The second <see cref="YearMonthDay"/> instance to compare.</param>
    /// <returns><see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(YearMonthDay lhs, YearMonthDay rhs) => lhs._dateBits != rhs._dateBits;

    /// <summary>
    /// Determines whether one <see cref="YearMonthDay"/> instance is earlier than another.
    /// </summary>
    /// <param name="lhs">The first <see cref="YearMonthDay"/> instance to compare.</param>
    /// <param name="rhs">The second <see cref="YearMonthDay"/> instance to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="lhs"/> is earlier than <paramref name="rhs"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator <(YearMonthDay lhs, YearMonthDay rhs) => lhs._dateBits < rhs._dateBits;

    /// <summary>
    /// Determines whether one <see cref="YearMonthDay"/> instance is earlier than or equal to another.
    /// </summary>
    /// <param name="lhs">The first <see cref="YearMonthDay"/> instance to compare.</param>
    /// <param name="rhs">The second <see cref="YearMonthDay"/> instance to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="lhs"/> is earlier than or equal to <paramref name="rhs"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator <=(YearMonthDay lhs, YearMonthDay rhs) => lhs._dateBits <= rhs._dateBits;

    /// <summary>
    /// Determines whether one <see cref="YearMonthDay"/> instance is later than another.
    /// </summary>
    /// <param name="lhs">The first <see cref="YearMonthDay"/> instance to compare.</param>
    /// <param name="rhs">The second <see cref="YearMonthDay"/> instance to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="lhs"/> is later than <paramref name="rhs"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator >(YearMonthDay lhs, YearMonthDay rhs) => lhs._dateBits > rhs._dateBits;

    /// <summary>
    /// Determines whether one <see cref="YearMonthDay"/> instance is later than or equal to another.
    /// </summary>
    /// <param name="lhs">The first <see cref="YearMonthDay"/> instance to compare.</param>
    /// <param name="rhs">The second <see cref="YearMonthDay"/> instance to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="lhs"/> is later than or equal to <paramref name="rhs"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator >=(YearMonthDay lhs, YearMonthDay rhs) => lhs._dateBits >= rhs._dateBits;
  }
}
