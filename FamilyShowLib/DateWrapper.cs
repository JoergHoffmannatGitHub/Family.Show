using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Genealogy;

namespace FamilyShowLib;

/// <summary>
/// Represents a wrapper for a <see cref="DateTime"/> object.
/// </summary>
public class DateWrapper : IEquatable<DateWrapper>, IXmlSerializable
{
  public IDate Date {  get; private set; }

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
  /// <param name="date">The date string to parse and wrap.</param>
  public DateWrapper(string dateString)
  {
    if (Genealogy.Date.TryParse(dateString, out IDate date))
    {
      Date = date;
    }
    else
    {
      Date = null;
    }
  }

  /// <summary>
  /// Indicates whether the specified <see cref="DateWrapper"/> is null or an empty <see cref="DateWrapper"/>.
  /// </summary>
  /// <param name="value">The <see cref="DateWrapper"/> to test.</param>
  /// <returns>True if the <see cref="DateWrapper"/> is null or empty; otherwise, false.</returns>
  public static bool IsNullOrEmpty([NotNullWhen(false)] DateWrapper value) => value == null || value.Date == null;

  #region IEquatable<DateWrapper>

  /// <summary>
  /// Indicates whether the current object is equal to another object of the same type.
  /// </summary>
  /// <param name="other">An object to compare with this object.</param>
  /// <returns>True if the current object is equal to the other parameter; otherwise, false.</returns>
  public bool Equals(DateWrapper other) => other != null && Date == other.Date;

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
  public override int GetHashCode() => Date.GetHashCode();

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
        Date = Genealogy.Date.Create(date.Year, date.Month, date.Day);
      }
      else
      {
        _ = Genealogy.Date.TryParse(str, out IDate iDate);
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
      writer.WriteString(Date.ToGedcom());
    }
    else
    {
      writer.WriteAttributeString("xsi", "nil", @"http://www.w3.org/2001/XMLSchema-instance", "true");
    }
  }

  #endregion
}
