using System;
using System.Runtime.Serialization;

using Architect.DomainModeling;

using Genealogy.Domain.Interfaces;

namespace Genealogy.Domain.ValueObjects.DateImplementation;

/// <summary>
/// Represents a date range with a specific type (Before, After, Between) and optional start and end dates.
/// </summary>
/// <remarks>
/// The <see cref="DateRange"/> class parses and represents date ranges in various formats, supporting
/// "BEF", "AFT", and "BET" prefixes. It provides functionality to parse, compare, and convert date ranges
/// to string representations, including GEDCOM format.
/// </remarks>
[ValueObject]
internal partial class DateRange : IDate, IEquatable<IDate>
{
    private const string BetweenDateConnector = "AND ";

    /// <summary>
    /// Specifies the type of date range: Before, After, or Between.
    /// </summary>
    internal enum Range
    {
        /// <summary>
        /// Represents a date before the specified start date.
        /// </summary>
        [EnumMember(Value = "BEF ")]
        Before,

        /// <summary>
        /// Represents a date after the specified start date.
        /// </summary>
        [EnumMember(Value = "AFT ")]
        After,

        /// <summary>
        /// Represents a date between the specified start and end dates.
        /// </summary>
        [EnumMember(Value = "BET ")]
        Between
    }

    /// <summary>
    /// Gets the type of the date range.
    /// </summary>
    internal Range Type { get; private init; }

    /// <summary>
    /// Gets the start date of the range.
    /// </summary>
    internal DateExact Start { get; private init; } = null;

    /// <summary>
    /// Gets the end date of the range, if applicable.
    /// </summary>
    internal DateExact End { get; private init; } = null;

    /// <summary>
    /// Attempts to parse the specified date string into a <see cref="DateRange"/> object.
    /// </summary>
    /// <param name="date">The date string to parse.</param>
    /// <param name="result">
    /// When this method returns, contains the parsed <see cref="DateRange"/> object if parsing succeeded;
    /// otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the date string was successfully parsed; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string date, out DateRange result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(date) || date.Length < 8)
        {
            return false;
        }

        try
        {
            result = new DateRange(date);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateRange"/> class with a specified date string.
    /// </summary>
    /// <param name="date">The date string to parse into a range. Must include a valid prefix and date(s).</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="date"/> is null.</exception>
    /// <exception cref="GenealogyException">
    /// Thrown if <paramref name="date"/> is less than 8 characters or has an invalid format.
    /// </exception>
    public DateRange(string date)
    {
        ArgumentNullException.ThrowIfNull(date);
        if (date.Length < 8)
        {
            throw new GenealogyException(
                $"Invalid date string: {date} too short to contain an rang prefix and a date.");
        }

        string prefix = date[..4].Trim().ToUpper();
        Type = EnumConverting.GetEnumFromString<Range>(prefix);
        string remainder = date[4..].Trim();
        int index = remainder.IndexOf(BetweenDateConnector, StringComparison.InvariantCultureIgnoreCase);
        string startDate = (index != -1) ? remainder[..Math.Max(index - 1, 0)] : remainder;

        if (Type == Range.Between && index == -1)
        {
            throw new GenealogyException($"Invalid date string: {date} of type BETWEEN must contain an end date.");
        }

        if (Type != Range.Between && index != -1)
        {
            throw new GenealogyException($"Invalid date string: {date} of type {Type} must not contain an end date.");
        }

        Start = new DateExact(startDate);
        if (index != -1)
        {
            string endDate = remainder[(index + 4)..];
            End = new DateExact(endDate);
        }
    }

    #region IDate

    /// <inheritdoc/>
    public override string ToString()
    {
        string result = EnumConverting.GetEnumValue(Type) + Start.ToString();
        if (Type == Range.Between)
        {
            result += " " + BetweenDateConnector + End.ToString();
        }
        return result;
    }

    #endregion IDate

    #region IEquatable<IDate>

    /// <inheritdoc/>
    public bool Equals(IDate obj) => obj != null && obj is DateRange other &&
      Type == other.Type && Start.Equals(other.Start) && End.Equals(other.End);

    #endregion
}
