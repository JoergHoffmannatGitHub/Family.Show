using System;

using Architect.DomainModeling;

using Genealogy.Domain.Interfaces;

namespace Genealogy.Domain.ValueObjects.DateImplementation;

/// <summary>
/// Represents a period of time defined by a starting date, an ending date, or both.
/// </summary>
/// <remarks>
/// A <see cref="DatePeriod"/> can represent a range of dates using the "FROM" and "TO" prefixes in its
/// string representation. For example, "FROM 2023-01-01 TO 2023-12-31" defines a period starting on January 1, 2023,
/// and ending on December 31, 2023. Either the "FROM" or "TO" prefix may be omitted to represent an open-ended
/// range.
/// </remarks>
[ValueObject]
internal partial class DatePeriod : IDate
{
    private const string FromPrefix = "FROM";

    private const string ToPrefix = "TO";

    /// <summary>
    /// Gets the starting date of the period, or <c>null</c> if not specified.
    /// </summary>
    internal DateExact From { get; private init; } = null;

    /// <summary>
    /// Gets the ending date of the period, or <c>null</c> if not specified.
    /// </summary>
    internal DateExact To { get; private init; } = null;

    /// <summary>
    /// Determines whether the specified date string starts with a valid prefix.
    /// </summary>
    /// <param name="date">The date string to validate.</param>
    /// <returns><see langword="true"/> if the <paramref name="date"/> starts with a valid prefix; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool IsValidFormat(string date) =>
        date.StartsWith(FromPrefix + ' ', StringComparison.InvariantCultureIgnoreCase) ||
        date.StartsWith(ToPrefix + ' ', StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    /// Tries to parse the specified string representation of a date period.
    /// </summary>
    /// <param name="date">A string containing a date period to convert.</param>
    /// <param name="result">
    /// When this method returns, contains the <see cref="DatePeriod"/> value equivalent to the
    /// date contained in <paramref name="date"/>, if the conversion succeeded, or <c>null</c> if the conversion failed.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <paramref name="date"/> parameter was converted successfully; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string date, out DatePeriod result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(date) || date.Length < 7)
        {
            return false;
        }

        try
        {
            result = new DatePeriod(date);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatePeriod"/> class by parsing a date period string.
    /// </summary>
    /// <param name="date">The date period string to parse. Must contain at least one of the prefixes "FROM"
    /// or "TO" followed by a valid date. Examples: "FROM 2023-01-01 TO 2023-12-31", "FROM 2023-01-01",
    /// "TO 2023-12-31".</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="date"/> is <c>null</c>.</exception>
    /// <exception cref="GenealogyException">Thrown when <paramref name="date"/> is too short to contain a
    /// period prefix and a date, or when neither "FROM" nor "TO" is present in the string.</exception>
    public DatePeriod(string date)
    {
        ArgumentNullException.ThrowIfNull(date);
        if (date.Length < 7)
        {
            throw new GenealogyException(
                $"Invalid date string: {date} too short to contain an period prefix and a date.");
        }

        int index = date.IndexOf(ToPrefix + ' ', StringComparison.InvariantCultureIgnoreCase);
        string fromDate = (index != -1) ? date[..Math.Max(index - 1, 0)] : date;
        if (!string.IsNullOrEmpty(fromDate))
        {
            string fromPrefix = fromDate[..4].Trim().ToUpper();
            if (fromPrefix == FromPrefix)
            {
                From = new DateExact(fromDate[5..]);
            }
        }

        if (index != -1)
        {
            string toDate = date[index..];
            string toPrefix = toDate[..2].Trim().ToUpper();
            if (toPrefix == ToPrefix)
            {
                To = new DateExact(toDate[3..]);
            }
        }

        if (From == null && To == null)
        {
            throw new GenealogyException(
                $"Invalid date: {date} must contain at least one of the words 'FROM' or 'TO'.");
        }
    }

    #region IDate

    /// <inheritdoc/>
    public override string ToString()
    {
        string fromResult = From != null ? $"{FromPrefix} {From}" : string.Empty;
        string toResult = To != null ? $"{ToPrefix} {To}" : string.Empty;
        if (From != null && To == null)
        {
            return $"{fromResult}".Trim();
        }
        else if (From == null && To != null)
        {
            return $"{toResult}".Trim();
        }

        return $"{fromResult} {toResult}".Trim();
    }

    #endregion IDate
}
