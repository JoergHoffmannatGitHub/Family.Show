using System;

using Architect.DomainModeling;

using Genealogy.Domain.Interfaces;

namespace Genealogy.Domain.ValueObjects.DateImplementation;

/// <summary>
/// Represents a date that has been interpreted from a specific string format,
/// including a date phrase and an optional exact date.
/// </summary>
/// <remarks>
/// This class is used to parse and validate date strings that follow a specific
/// format, such as those prefixed with "INT" or enclosed in parentheses.
/// It ensures that the  date phrase adheres to length constraints and is
/// properly formatted. Instances of this class are immutable once created.
/// </remarks>
[ValueObject]
internal partial class DateInterpreted : IDate
{
    private const string IntPrefix = "INT ";

    /// <summary>
    /// Any statement offered as a date when the year is not recognizable to a
    /// date parser, but which gives information about when an event occurred.
    /// </summary>
    internal string DatePhrase { get; private init; }

    /// <summary>
    /// Gets the exact date associated with this instance.
    /// </summary>
    internal DateExact Date { get; private init; }

    /// <summary>
    /// Determines whether the specified date string starts with a valid prefix.
    /// </summary>
    /// <param name="date">The date string to validate.</param>
    /// <returns><see langword="true"/> if the <paramref name="date"/> starts with a valid prefix; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool IsValidFormat(string date)
    {
        return date.EndsWith(')') &&
            (date.StartsWith(IntPrefix, StringComparison.InvariantCultureIgnoreCase) ||
            date.StartsWith('('));
    }

    /// <summary>
    /// Attempts to parse the specified date string into a <see cref="DateInterpreted"/> object.
    /// </summary>
    /// <param name="date">The date string to parse. Cannot be <see langword="null"/>.</param>
    /// <param name="result">
    /// When this method returns, contains the <see cref="DateInterpreted"/> object representing
    /// the parsed date, if the parsing was successful; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryParse(string date, out DateInterpreted result)
    {
        ArgumentNullException.ThrowIfNull(date);
        result = null;

        try
        {
            result = new DateInterpreted(date);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateInterpreted"/> class using the specified date string.
    /// </summary>
    /// <remarks>
    /// The constructor validates the format of the input string and extracts
    /// the date phrase and, if applicable, the exact date. The date phrase must
    /// be enclosed in parentheses, and its length must not exceed 35 characters.
    /// If the string starts with the "INT" prefix, the exact date is extracted
    /// and interpreted.
    /// </remarks>
    /// <param name="date">
    /// A string representing the date to be interpreted. The string must contain
    /// a date phrase enclosed in parentheses and, if prefixed with "INT", an
    /// exact date following the prefix.
    /// </param>
    /// <exception cref="GenealogyException">
    /// Thrown if the <paramref name="date"/> string does not contain a date phrase
    /// enclosed in parentheses, if the date phrase exceeds 35 characters, or if
    /// the date phrase is invalid.
    /// </exception>
    public DateInterpreted(string date)
    {
        ArgumentNullException.ThrowIfNull(date);

        if (IsValidFormat(date))
        {
            int leftParenthesisIndex = date.IndexOf('(');
            if (leftParenthesisIndex < 0)
            {
                throw new GenealogyException($"Date phrases {date} must be enclosed in parenthesis.");
            }

            string datePhrase = date.Substring(leftParenthesisIndex).Trim();
            if (datePhrase.Length > 35)
            {
                throw new GenealogyException($"Date phrase {datePhrase} exceeds maximum length of 35 characters.");
            }

            DatePhrase = datePhrase;

            if (date.StartsWith(IntPrefix))
            {
                Date = new DateExact(date.Substring(4, leftParenthesisIndex - 4).Trim());
            }
        }

        if (string.IsNullOrEmpty(DatePhrase))
        {
            throw new GenealogyException($"Invalid date phrase for DateInterpreted {date}.");
        }
    }

    #region IDate

    /// <inheritdoc/>
    public override string ToString() =>
        Date != null ? IntPrefix + Date.ToString() + " " + DatePhrase : DatePhrase;

    #endregion IDate
}
