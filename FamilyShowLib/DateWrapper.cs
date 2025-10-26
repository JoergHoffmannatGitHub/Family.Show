using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Genealogy.Domain.Interfaces;

namespace FamilyShowLib;

/// <summary>
/// Represents a wrapper for a <see cref="DateTime"/> object.
/// </summary>
public sealed class DateWrapper : IEquatable<DateWrapper>, IXmlSerializable
{
  /// <summary>
  /// Gets the wrapped date.
  /// </summary>
  internal IDate Date { get; private set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="DateWrapper"/> class.
  /// </summary>
  public DateWrapper()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="DateWrapper"/> class with the specified date.
  /// </summary>
  /// <param name="date">The date to wrap.</param>
  public DateWrapper(IDate date) => Date = date;

  /// <summary>
  /// Initializes a new instance of the <see cref="DateWrapper"/> class with the specified date string.
  /// </summary>
  /// <param name="dateString">The date string to parse and wrap.</param>
  public DateWrapper(string dateString)
  {
    Date = Genealogy.Domain.ValueObjects.Date.TryParse(dateString, out IDate date) ? date : null;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="DateWrapper"/> class with the specified year, month, and day.
  /// </summary>
  /// <param name="year">The year component of the date.</param>
  /// <param name="month">The month component of the date.</param>
  /// <param name="day">The day component of the date.</param>
  public DateWrapper(int year, int month = 0, int day = 0) => Date = Genealogy.Domain.ValueObjects.Date.Create(year, month, day);

  /// <summary>
  /// Indicates whether the specified <see cref="DateWrapper"/> is null or an empty <see cref="DateWrapper"/>.
  /// </summary>
  /// <param name="value">The <see cref="DateWrapper"/> to test.</param>
  /// <returns>True if the <see cref="DateWrapper"/> is null or empty; otherwise, false.</returns>
  public static bool IsNullOrEmpty([NotNullWhen(false)] DateWrapper value) => value == null || value.Date == null;

  /// <summary>
  /// Indicates whether the specified <see cref="DateWrapper"/> is empty.
  /// </summary>
  /// <param name="value">The <see cref="DateWrapper"/> to test.</param>
  /// <returns>True if the <see cref="DateWrapper"/> is empty; otherwise, false.</returns>
  public static bool IsDateEmpty(DateWrapper value) => value != null && value.Date == null;

  /// <summary>
  /// Indicates whether the specified <see cref="DateWrapper"/> contains an exact date.
  /// </summary>
  /// <param name="value">The <see cref="DateWrapper"/> to test.</param>
  /// <param name="dateExact">When this method returns, contains the exact date if the <see cref="DateWrapper"/> contains an exact date; otherwise, null.</param>
  /// <returns>True if the <see cref="DateWrapper"/> contains an exact date; otherwise, false.</returns>
  public static bool IsDateExact([NotNullWhen(true)] DateWrapper value, out IDateExact dateExact)
  {
    return (dateExact = IsNullOrEmpty(value) ? null : value.Date as IDateExact) != null;
  }

  /// <summary>
  /// Converts the wrapped date to its GEDCOM representation.
  /// </summary>
  /// <returns>A string representing the date in GEDCOM format.</returns>
  public string ToGedcom() => IsNullOrEmpty(this) ? string.Empty : Date.ToString();

  /// <summary>
  /// Converts the wrapped date to a short date string.
  /// </summary>
  /// <returns>A short date string representing the wrapped date.</returns>
  public string ToShortString()
  {
    return IsDateExact(this, out IDateExact date)
      ? new DateTime(date.Year, Math.Max(date.Month, 1), Math.Max(date.Day, 1)).ToShortDateString()
      : string.Empty;
  }

  /// <summary>
  /// Formats the wrapped date as a string in the format "day/month/year".
  /// </summary>
  /// <returns>A string representing the formatted date.</returns>
  public static string Format(DateWrapper value)
  {
    if (IsDateExact(value, out IDateExact date))
    {
      int day = date.Day;
      int month = date.Month;
      int year = date.Year;
      return day + "/" + month + "/" + year;
    }

    return string.Empty;
  }

  #region IEquatable<DateWrapper>

  /// <summary>
  /// Indicates whether the current object is equal to another object of the same type.
  /// </summary>
  /// <param name="other">An object to compare with this object.</param>
  /// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
  public bool Equals(DateWrapper other) => other != null && Date != null && Date.Equals(other.Date);

  /// <summary>
  /// Determines whether the specified object is equal to the current object.
  /// </summary>
  /// <param name="obj">The object to compare with the current object.</param>
  /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
  public override bool Equals(object obj) => obj is DateWrapper other && Equals(other);

  /// <summary>
  /// Serves as the default hash function.
  /// </summary>
  /// <returns>A hash code for the current object.</returns>
  public override int GetHashCode() => HashCode.Combine(Date);

  /// <summary>
  /// Determines whether two specified instances of <see cref="DateWrapper"/> are equal.
  /// </summary>
  /// <param name="dateWrapper1">The first <see cref="DateWrapper"/> to compare.</param>
  /// <param name="dateWrapper2">The second <see cref="DateWrapper"/> to compare.</param>
  /// <returns>True if the two <see cref="DateWrapper"/> instances are equal; otherwise, false.</returns>
  public static bool operator ==(DateWrapper dateWrapper1, DateWrapper dateWrapper2) => Equals(dateWrapper1, dateWrapper2);

  /// <summary>
  /// Determines whether two specified instances of <see cref="DateWrapper"/> are not equal.
  /// </summary>
  /// <param name="dateWrapper1">The first <see cref="DateWrapper"/> to compare.</param>
  /// <param name="dateWrapper2">The second <see cref="DateWrapper"/> to compare.</param>
  /// <returns>True if the two <see cref="DateWrapper"/> instances are not equal; otherwise, false.</returns>
  public static bool operator !=(DateWrapper dateWrapper1, DateWrapper dateWrapper2) => !Equals(dateWrapper1, dateWrapper2);

  #endregion

  #region IXmlSerializable

  /// <summary>
  /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, 
  /// you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a 
  /// custom schema is required, apply the XmlSchemaProviderAttribute to the class.
  /// </summary>
  /// <returns>Null.</returns>
  XmlSchema IXmlSerializable.GetSchema() => null;

  /// <summary>
  /// Generates an object from its XML representation.
  /// </summary>
  /// <param name="reader">The XmlReader stream from which the object is deserialized.</param>
  void IXmlSerializable.ReadXml(XmlReader reader)
  {
    reader.MoveToContent();

    bool isEmptyElement = reader.IsEmptyElement;
    reader.ReadStartElement();
    if (!isEmptyElement)
    {
      string str = reader.ReadContentAsString();
      string[] sa = str.Split(' ');
      if (DateTime.TryParse(sa[0], out DateTime date))
      {
        Date = Genealogy.Domain.ValueObjects.Date.Create(date.Year, date.Month, date.Day);
      }
      else
      {
        _ = Genealogy.Domain.ValueObjects.Date.TryParse(str, out IDate iDate);
        Date = iDate;
      }

      reader.ReadEndElement();
    }
  }

  /// <summary>
  /// Converts an object into its XML representation.
  /// </summary>
  /// <param name="writer">The XmlWriter stream to which the object is serialized.</param>
  void IXmlSerializable.WriteXml(XmlWriter writer)
  {
    if (Date != null)
    {
      writer.WriteString(Date.ToString());
    }
    else
    {
      writer.WriteAttributeString("xsi", "nil", @"http://www.w3.org/2001/XMLSchema-instance", "true");
    }
  }

  #endregion
}
