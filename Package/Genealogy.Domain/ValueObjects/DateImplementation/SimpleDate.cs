using System;

using Architect.DomainModeling;

using Genealogy.Domain.Interfaces;

namespace Genealogy.Domain.ValueObjects.DateImplementation;

/// <summary>
/// Represents a simple date value object that implements the <see cref="IDate"/> interface.
/// </summary>
/// <remarks>The <see cref="SimpleDate"/> class provides functionality to parse and store a date string, and
/// convert it to its GEDCOM representation. It also supports equality comparison with other <see cref="IDate"/>
/// objects.</remarks>
[ValueObject]
internal partial class SimpleDate : IDate, IEquatable<IDate>
{
  internal string Date { get; private init; }

  /// <summary>
  /// Initializes a new instance of the <see cref="SimpleDate"/> class with the specified date string.
  /// </summary>
  /// <param name="date">The date string to be parsed into a <see cref="SimpleDate"/> object. The string must be in a valid date format.</param>
  internal SimpleDate(string date)
  {
    Date = ParseDate(date);
  }

  #region IDate

  /// <summary>
  /// Converts the date to its GEDCOM representation.
  /// </summary>
  /// <returns>A string representing the date in GEDCOM format.</returns>
  public override string ToString() => Date;

  #endregion

  #region IEquatable<IDate>

  /// <summary>
  /// Determines whether the specified <see cref="IDate"/> object is equal to the current instance.
  /// </summary>
  /// <remarks>Two <see cref="IDate"/> objects are considered equal if they are not null and their <c>Date</c>
  /// properties are equal.</remarks>
  /// <param name="obj">The <see cref="IDate"/> object to compare with the current instance.</param>
  /// <returns><see langword="true"/> if the specified <see cref="IDate"/> object is equal to the current instance; otherwise,
  /// <see langword="false"/>.</returns>
  public bool Equals(IDate obj)
  {
    return obj != null && obj is SimpleDate other && Date == other.Date;
  }

  #endregion

  /// <summary>
  /// Parses the specified date string and stores it.
  /// </summary>
  /// <param name="date">The date string to parse.</param>
  /// <exception cref="GenealogyException">Thrown when the date string is invalid.</exception>
  private static string ParseDate(string date)
  {
    // There is a minimum length of 4 characters
    if (date == null || date.Length < 4)
    {
      throw new GenealogyException("Invalid Date: Must have at least YYYY");
    }

    return date;
  }
}
