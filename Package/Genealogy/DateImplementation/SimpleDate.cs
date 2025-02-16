using System;

namespace Genealogy.DateImplementation
{
  /// <summary>
  /// Represents a simple date implementation of the <see cref="IDate"/> interface.
  /// </summary>
  internal class SimpleDate : IDate, IEquatable<SimpleDate>
  {
    private string _date;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleDate"/> class with the specified date string.
    /// </summary>
    /// <param name="date">The date string to parse and store.</param>
    /// <exception cref="GenealogyException">Thrown when the date string is invalid.</exception>
    internal SimpleDate(string date)
    {
      ParseDate(date);
    }

    #region IDate

    /// <summary>
    /// Converts the date to its GEDCOM representation.
    /// </summary>
    /// <returns>A string representing the date in GEDCOM format.</returns>
    public string ToGedcom()
    {
      return _date;
    }

    #endregion

    #region IEquatable<SimpleDate>

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(SimpleDate other) => other != null && _date == other._date;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj) => obj is SimpleDate other && Equals(other);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() => _date.GetHashCode();

    #endregion

    /// <summary>
    /// Parses the specified date string and stores it.
    /// </summary>
    /// <param name="date">The date string to parse.</param>
    /// <exception cref="GenealogyException">Thrown when the date string is invalid.</exception>
    private void ParseDate(string date)
    {
      // There is a minimum length of 4 characters
      if (date.Length < 4)
      {
        throw new GenealogyException("Invalid Date: Must have at least YYYY");
      }

      _date = date;
    }
  }
}
