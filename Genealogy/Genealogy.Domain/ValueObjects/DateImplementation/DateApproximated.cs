using System;
using System.Runtime.Serialization;

using Architect.DomainModeling;

using Genealogy.Domain.Interfaces;

namespace Genealogy.Domain.ValueObjects.DateImplementation;

/// <summary>
/// Represents a date that is approximated, providing a level of uncertainty such as "about", "calculated", or
/// "estimated".
/// </summary>
/// <remarks>
/// This class encapsulates a date with an associated approximation type, allowing for the representation
/// of dates that are not exact. The approximation type is determined from a string prefix and can be one of the
/// following: "About", "Calculated", or "Estimated".
/// </remarks>
[ValueObject]
internal partial class DateApproximated : IDate
{
    /// <summary>
    /// Specifies the type of date approximation.
    /// </summary>
    internal enum Approximated
    {
        /// <summary>
        /// About, meaning the date is not exact.
        /// </summary>
        [EnumMember(Value = "ABT ")]
        About,

        /// <summary>
        /// Calculated mathematically, for example, from an event date and age.
        /// </summary>
        [EnumMember(Value = "CAL ")]
        Calculated,

        /// <summary>
        /// Estimated based on an algorithm using some other event date.
        /// </summary>
        [EnumMember(Value = "EST ")]
        Estimated
    }

    /// <summary>
    /// Gets the type of approximation for the date.
    /// </summary>
    internal Approximated Type { get; private init; }

    /// <summary>
    /// Gets the exact date value associated with the approximation.
    /// </summary>
    internal DateExact Date { get; private init; }

    /// <summary>
    /// Determines whether the specified date string starts with a valid prefix.
    /// </summary>
    /// <param name="date">The date string to validate.</param>
    /// <returns><see langword="true"/> if the <paramref name="date"/> starts with a valid prefix; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool IsValidFormat(string date) =>
        date.StartsWith(
            EnumConverting.GetEnumValue(
                Approximated.About),
                StringComparison.InvariantCultureIgnoreCase) ||
        date.StartsWith(
            EnumConverting.GetEnumValue(
                Approximated.Calculated),
                StringComparison.InvariantCultureIgnoreCase) ||
        date.StartsWith(
            EnumConverting.GetEnumValue(
                Approximated.Estimated),
                StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    /// Attempts to parse the specified string representation of an approximated date.
    /// </summary>
    /// <param name="date">A string containing an approximated date to convert.</param>
    /// <param name="result">When this method returns, contains the <see cref="DateApproximated"/> value equivalent
    /// to the date contained in <paramref name="date"/>, if the conversion succeeded, or <c>null</c> if the
    /// conversion failed.</param>
    /// <returns>
    /// <c>true</c> if the <paramref name="date"/> parameter was converted successfully; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string date, out DateApproximated result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(date) || date.Length < 8)
        {
            return false;
        }

        try
        {
            result = new DateApproximated(date);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateApproximated"/> class using the specified date string.
    /// </summary>
    /// <remarks>
    /// The <paramref name="date"/> parameter must be a non-null string with a minimum length of 8 characters.
    /// The first four characters are used to determine the approximation type, and the rest are used to initialize
    /// the exact date.
    /// </remarks>
    /// <param name="date">A string representing the date, where the first four characters indicate the approximation
    /// type, and the remaining characters represent the exact date.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="date"/> is null.</exception>
    /// <exception cref="GenealogyException">Thrown if <paramref name="date"/> is less than 8 characters.</exception>
    /// <exception cref="NotImplementedException">Thrown if the prefix is not recognized.</exception>
    internal DateApproximated(string date)
    {
        ArgumentNullException.ThrowIfNull(date);
        if (date.Length < 8)
        {
            throw new GenealogyException(
                $"Invalid date string: {date} too short to contain an approximation prefix and a date.");
        }

        string prefix = date[..4].Trim().ToUpper();
        Type = EnumConverting.GetEnumFromString<Approximated>(prefix);
        string remainder = date[4..].Trim();
        Date = new DateExact(remainder);
    }

    #region IDate

    /// <inheritdoc/>
    public override string ToString() => EnumConverting.GetEnumValue(Type) + Date.ToString();

    #endregion IDate
}
