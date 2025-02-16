using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Genealogy;

namespace FamilyShowLib.Tests;

public class DateWrapperTest
{
  [Fact]
  public void DefaultConstructor_ShoudBeEmpty()
  {
    // Arrange & Act
    DateWrapper dateWrapper = new();

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Null(dateWrapper._date);
    Assert.True(DateWrapper.IsNullOrEmpty(dateWrapper));
  }

  [Fact]
  public void Constructor_ShouldInitializeDate_WhenDateParseIsProvided()
  {
    // Arrange
    _ = Date.TryParse("BET 1982 AND 1984", out IDate expectedDate);

    // Act
    DateWrapper dateWrapper = new(expectedDate);

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Equal(expectedDate, dateWrapper._date);
    Assert.False(DateWrapper.IsNullOrEmpty(dateWrapper));
  }

  [Fact]
  public void Constructor_ShouldInitializeDate_WhenDateCreateIsProvided()
  {
    // Arrange
    IDate date = Date.Create(2023, 1, 1);

    // Act
    DateWrapper dateWrapper = new(date);

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Equal(date, dateWrapper._date);
    Assert.False(DateWrapper.IsNullOrEmpty(dateWrapper));
  }

  [Fact]
  public void Constructor_ShouldInitializeDateToNull_WhenDateIsNull()
  {
    // Arrange
    IDate? date = null;

    // Act
    DateWrapper dateWrapper = new(date);

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Null(dateWrapper._date);
    Assert.True(DateWrapper.IsNullOrEmpty(dateWrapper));
  }

  [Fact]
  public void Constructor_ShouldInitializeDate_WhenDateStringIsProvided()
  {
    // Arrange
    string dateString = "1 JAN 2023";
    _ = Date.TryParse(dateString, out IDate expectedDate);

    // Act
    DateWrapper dateWrapper = new(dateString);

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Equal(expectedDate, dateWrapper._date);
  }

  [Theory, CombinatorialData]
  public void Constructor_ShouldThrowException_WhenInvalidDateStringIsProvided(
    [CombinatorialValues("12", "", null)] string invalidDateString
    )
  {
    // Arrange

    // Act
    DateWrapper dateWrappe = new(invalidDateString);

    // Assert
    Assert.True(DateWrapper.IsNullOrEmpty(dateWrappe));
  }

  [Fact]
  public void IsNullOrEmpty_ShouldReturnTrue_WhenDateWrapperIsNull()
  {
    // Arrange
    DateWrapper? dateWrapper = null;

    // Act
    bool result = DateWrapper.IsNullOrEmpty(dateWrapper);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void IsNullOrEmpty_ShouldReturnTrue_WhenDateIsNull()
  {
    // Arrange
    DateWrapper dateWrapper = new();

    // Act
    bool result = DateWrapper.IsNullOrEmpty(dateWrapper);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void IsNullOrEmpty_ShouldReturnFalse_WhenDateIsValid()
  {
    // Arrange
    DateWrapper dateWrapper = new() { _date = Date.Create(2023, 1, 1) };

    // Act
    bool result = DateWrapper.IsNullOrEmpty(dateWrapper);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void Equals_ShouldReturnTrue_WhenDatesAreEqual()
  {
    // Arrange
    IDate date = Date.Create(2023, 1, 1);
    DateWrapper dateWrapper1 = new(date);
    DateWrapper dateWrapper2 = new(date);

    // Act
    bool result = dateWrapper1.Equals(dateWrapper2);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void Equals_ShouldReturnFalse_WhenDatesAreNotEqual()
  {
    // Arrange
    DateWrapper dateWrapper1 = new(Date.Create(2023, 1, 1));
    DateWrapper dateWrapper2 = new(Date.Create(2023, 1, 1));

    // Act
    bool result = dateWrapper1.Equals(dateWrapper2);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void Equals_ShouldReturnFalse_WhenOtherIsNull()
  {
    // Arrange
    DateWrapper dateWrapper = new(Date.Create(2023, 1, 1));

    // Act
    bool result = dateWrapper.Equals(null);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void EqualsObject_ShouldReturnTrue_WhenDatesAreEqual()
  {
    // Arrange
    IDate date = Date.Create(2023, 1, 1);
    DateWrapper dateWrapper1 = new(date);
    DateWrapper dateWrapper2 = new(date);

    // Act
    bool result = dateWrapper1.Equals((object)dateWrapper2);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void EqualsObject_ShouldReturnFalse_WhenDatesAreNotEqual()
  {
    // Arrange
    DateWrapper dateWrapper1 = new(Date.Create(2023, 1, 1));
    DateWrapper dateWrapper2 = new(Date.Create(2023, 1, 1));

    // Act
    bool result = dateWrapper1.Equals((object)dateWrapper2);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void EqualsObject_ShouldReturnFalse_WhenObjectIsNotDateWrapper()
  {
    // Arrange
    DateWrapper dateWrapper = new(Date.Create(2023, 1, 1));
    object obj = new();

    // Act
    bool result = dateWrapper.Equals(obj);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void GetHashCode_ShouldReturnSameHashCode_WhenDatesAreEqual()
  {
    // Arrange
    IDate date = Date.Create(2023, 1, 1);
    DateWrapper dateWrapper1 = new(date);
    DateWrapper dateWrapper2 = new(date);

    // Act
    int hashCode1 = dateWrapper1.GetHashCode();
    int hashCode2 = dateWrapper2.GetHashCode();

    // Assert
    Assert.Equal(hashCode1, hashCode2);
  }

  [Fact]
  public void GetHashCode_ShouldReturnDifferentHashCode_WhenDatesAreNotEqual()
  {
    // Arrange
    DateWrapper dateWrapper1 = new(Date.Create(2023, 1, 1));
    DateWrapper dateWrapper2 = new(Date.Create(2023, 1, 2));

    // Act
    int hashCode1 = dateWrapper1.GetHashCode();
    int hashCode2 = dateWrapper2.GetHashCode();

    // Assert
    Assert.NotEqual(hashCode1, hashCode2);
  }

  [Fact]
  public void GetSchema_ShouldReturnNull()
  {
    // Arrange
    DateWrapper dateWrapper = new();

    // Act
    XmlSchema? schema = ((IXmlSerializable)dateWrapper).GetSchema();

    // Assert
    Assert.Null(schema);
  }

  [Fact]
  public void ReadXml_ShouldDeserializeDate_WhenValidDateIsProvided()
  {
    // Arrange
    string xml = "<DateWrapper>2023-01-01</DateWrapper>";
    byte[] byteArray = Encoding.ASCII.GetBytes(xml);
    using MemoryStream memoryStream = new(byteArray);

    // Act
    DateWrapper result = ReadFrom<DateWrapper>(memoryStream);

    // Assert
    Assert.Equal(Date.Create(2023, 1, 1), result._date);
  }

  [Fact]
  public void ReadXml_ShouldDeserializeDate_WhenInvalidDateIsProvided()
  {
    // Arrange
    string xml = "<DateWrapper>InvalidDate</DateWrapper>";
    DateWrapper dateWrapper = new();
    byte[] byteArray = Encoding.ASCII.GetBytes(xml);
    using MemoryStream memoryStream = new(byteArray);

    // Act
    DateWrapper result = ReadFrom<DateWrapper>(memoryStream);

    // Assert
    Assert.Null(dateWrapper._date); // Assuming str.ToDate() returns null for invalid expectedDate
  }

  [Fact]
  public void WriteXml_ShouldSerializeDate_WhenDateIsNotNull()
  {
    // Arrange
    DateWrapper dateWrapper = new(Date.Create(2023, 1, 1));
    using MemoryStream memoryStream = new();

    // Act
    WriteTo<DateWrapper>(memoryStream, dateWrapper);

    // Assert
    string expectedXml = "1 JAN 2023"; // Assuming _date.ToGedcom() returns "1 JAN 2023"
    StreamReader reader = new(memoryStream);
    string buffer = reader.ReadToEnd();
    Assert.Contains(expectedXml, buffer);
  }

  [Fact]
  public void WriteXml_ShouldSerializeNil_WhenDateIsNull()
  {
    // Arrange
    DateWrapper dateWrapper = new();
    using MemoryStream memoryStream = new();

    // Act
    WriteTo<DateWrapper>(memoryStream, dateWrapper);

    // Assert
    string expectedXml = "xsi:nil=\"true\"";
    StreamReader reader = new(memoryStream);
    string buffer = reader.ReadToEnd();
    Assert.Contains(expectedXml, buffer);
  }

  private static TSerializationData ReadFrom<TSerializationData>(Stream location)
  {
    location.Seek(0, SeekOrigin.Begin);
    using (StreamReader streamReader = new(location))
    {
      XmlSerializer xmlSerializer = new(typeof(TSerializationData));
      object? o = xmlSerializer.Deserialize(XmlReader.Create(streamReader));
      Assert.NotNull(o);

      return (TSerializationData)o;
    }
  }

  private static void WriteTo<TSerializationData>(Stream location, TSerializationData data)
  {
    XmlSerializer xmlSerializer = new(typeof(TSerializationData));
    xmlSerializer.Serialize(XmlWriter.Create(location), data);
    location.Flush();
    location.Seek(0, SeekOrigin.Begin);
  }
}
