using System;
using System.Reflection;
using System.Runtime.Serialization;

using Architect.DomainModeling;

using Genealogy.Domain.Interfaces;

namespace Genealogy.Domain.ValueObjects.DateImplementation;

/// <summary>
/// Represents a date that is approximated, providing a level of uncertainty such as "about", "calculated", or
/// "estimated".
/// </summary>
/// <remarks>This class encapsulates a date with an associated approximation type, allowing for the representation
/// of dates that are not exact. The approximation type is determined from a string prefix and can be one of the
/// following: "About", "Calculated", or "Estimated".</remarks>
[ValueObject]
internal partial class DateApproximated : IDate, IEquatable<IDate>
{
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

  internal Approximated Type { get; private init; }

  internal DateExact Date { get; private init; }

  /// <summary>
  /// Attempts to parse a date string into a <see cref="DateApproximated"/> instance.
  /// </summary>
  /// <param name="date">The date string to parse.</param>
  /// <param name="result">The parsed <see cref="DateApproximated"/> instance, or null if parsing fails.</param>
  /// <returns>True if parsing succeeded; otherwise, false.</returns>
  public static bool TryParse(string date, out DateApproximated result)
  {
    result = null;
    if (string.IsNullOrWhiteSpace(date) || date.Length <5)
      return false;
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
  /// <remarks>The <paramref name="date"/> parameter must be a non-null string with a minimum length of 5
  /// characters. The first four characters are used to determine the approximation type, and the rest are used to
  /// initialize the exact date.</remarks>
  /// <param name="date">A string representing the date, where the first four characters indicate the approximation type, and the remaining
  /// characters represent the exact date.</param>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="date"/> is null.</exception>
  /// <exception cref="GenealogyException">Thrown if <paramref name="date"/> is less than5 characters.</exception>
  /// <exception cref="NotImplementedException">Thrown if the prefix is not recognized.</exception>
  internal DateApproximated(string date)
  {
    ArgumentNullException.ThrowIfNull(date);
    if (date.Length <5)
    { 
      throw new GenealogyException($"Invalid date string: {date} too short to contain an approximation prefix and a date.");
    }
    string prefix = date.Substring(0,4).Trim().ToUpper();
    Type = GetEnumFromString<Approximated>(prefix);
    string remainder = date.Substring(4).Trim();
    Date = new DateExact(remainder);
  }

  /// <summary>
  /// Converts the date to its GEDCOM representation.
  /// </summary>
  /// <returns>A string representing the date in GEDCOM format.</returns>
  public override string ToString() => GetEnumValue(Type) + Date.ToString();

  #region IEquatable<IDate>

  /// <summary>
  /// Determines whether the specified <see cref="IDate"/> is equal to the current instance.
  /// </summary>
  /// <param name="obj">The <see cref="IDate"/> to compare with the current instance.</param>
  /// <returns><see langword="true"/> if the specified <see cref="IDate"/> is equal to the current instance; otherwise, <see
  /// langword="false"/>.</returns>
  public bool Equals(IDate obj) => obj != null && obj is DateApproximated other && Type == other.Type && Date.Equals(other.Date);

  #endregion

  /// <summary>
  /// Gets the GEDCOM string value for the specified enum value.
  /// </summary>
  /// <typeparam name="T">The enum type.</typeparam>
  /// <param name="enumValue">The enum value.</param>
  /// <returns>The GEDCOM string value.</returns>
  private static string GetEnumValue<T>(T enumValue) where T : Enum
  {
    Type type = enumValue.GetType();
    MemberInfo[] memInfo = type.GetMember(enumValue.ToString());
    object[] attributes = memInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);
    return ((EnumMemberAttribute)attributes[0]).Value;
  }

  /// <summary>
  /// Gets the enum value from the GEDCOM string prefix.
  /// </summary>
  /// <typeparam name="T">The enum type.</typeparam>
  /// <param name="value">The GEDCOM string prefix.</param>
  /// <returns>The enum value.</returns>
  /// <exception cref="NotImplementedException">Thrown if the prefix is not recognized.</exception>
  private static T GetEnumFromString<T>(string value) where T : Enum
  {
    Type type = typeof(T);
    foreach (FieldInfo field in type.GetFields())
    {
      if (Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute)) is EnumMemberAttribute attribute &&
        attribute.Value.Trim().Equals(value, StringComparison.CurrentCultureIgnoreCase))
      {
        return (T)field.GetValue(null);
      }
    }
    throw new NotImplementedException($"Unable to parse '{value}'");
  }
}
