using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Genealogy;

namespace FamilyShowLib.Tests;

public class DateWrapperSerializableTest
{
  private const string FullName = "Sylvester Stallone";
  private const string BirthDateTime = "1946-07-06T00:00:00";
  private const string BirthGenealogyDate = "6 Jul 1946";
  private const string PersonWithoutDateXml = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
    @"<Person xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" +
      "<FullName>" + FullName + "</FullName>" +
    "</Person>";
  private const string PersonWithEmptyDateXml = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
    @"<Person xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" +
      "<FullName>" + FullName + "</FullName>" +
      "<BirthDate xsi:nil=\"true\" />" +
    "</Person>";
  private const string PersonWithDateTimeXml = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
    @"<Person xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" +
      "<FullName>" + FullName + "</FullName>" +
      "<BirthDate>" + BirthDateTime + "</BirthDate>" +
    "</Person>";
  private const string PersonWithGenealogyDateXml = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
    @"<Person xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" +
      "<FullName>" + FullName + "</FullName>" +
      "<BirthDate>" + BirthGenealogyDate + "</BirthDate>" +
    "</Person>";

  [Serializable]
  public class Person
  {
    public required string FullName;
    public DateWrapper? BirthDate;
  }

  [Fact]
  public void DateWrapper_GetSchema_ShouldReturnNull()
  {
    // Arrange
    DateWrapper dateWrapper = new();

    // Act
    XmlSchema? schema = ((IXmlSerializable)dateWrapper).GetSchema();

    // Assert
    Assert.Null(schema);
  }

  [Fact]
  public void DateWrapper_ReadXml_ShouldDeserializeDate_WhenValidDateIsProvided()
  {
    // Arrange
    string xml = "<DateWrapper>2023-01-01</DateWrapper>";
    byte[] byteArray = Encoding.ASCII.GetBytes(xml);
    using MemoryStream memoryStream = new(byteArray);

    // Act
    DateWrapper result = ReadFrom<DateWrapper>(memoryStream);

    // Assert
    Assert.Equal(Date.Create(2023, 1, 1), result.Date);
  }

  [Fact]
  public void DateWrapper_ReadXml_ShouldDeserializeDate_WhenInvalidDateIsProvided()
  {
    // Arrange
    string xml = "<DateWrapper>InvalidDate</DateWrapper>";
    DateWrapper dateWrapper = new();
    byte[] byteArray = Encoding.ASCII.GetBytes(xml);
    using MemoryStream memoryStream = new(byteArray);

    // Act
    DateWrapper result = ReadFrom<DateWrapper>(memoryStream);

    // Assert
    Assert.Null(dateWrapper.Date); // Assuming str.ToDate() returns null for invalid date
  }

  [Theory]
  [InlineData(PersonWithEmptyDateXml)]
  [InlineData(PersonWithoutDateXml)]
  public void DateWrapper_ReadPersonWithoutDate_ShouldDeserializeCorrectly(string xmlData)
  {
    // Arrange
    byte[] byteArray = Encoding.ASCII.GetBytes(xmlData);
    using (MemoryStream memoryStream = new(byteArray))
    {
      // Act
      Person person = ReadFrom<Person>(memoryStream);

      // Assert
      Assert.Equal(FullName, person.FullName);
      Assert.True(DateWrapper.IsNullOrEmpty(person.BirthDate));
    }
  }

  [Theory]
  [InlineData(PersonWithGenealogyDateXml)]
  [InlineData(PersonWithDateTimeXml)]
  public void DateWrapper_ReadPersonWithDate_ShouldDeserializeCorrectly(string xmlData)
  {
    // Arrange
    byte[] byteArray = Encoding.ASCII.GetBytes(xmlData);
    using (MemoryStream memoryStream = new(byteArray))
    {
      // Act
      Person person = ReadFrom<Person>(memoryStream);

      // Assert
      Assert.Equal(FullName, person.FullName);
      Assert.False(DateWrapper.IsNullOrEmpty(person.BirthDate));
      Assert.Equal(BirthGenealogyDate, person.BirthDate.Date!.ToGedcom());
    }
  }

  [Fact]
  public void DateWrapper_WriteXml_ShouldSerializeDate_WhenDateIsNotNull()
  {
    // Arrange
    DateWrapper dateWrapper = new(Date.Create(2023, 1, 1));
    using MemoryStream memoryStream = new();

    // Act
    WriteTo<DateWrapper>(memoryStream, dateWrapper);

    // Assert
    string expectedXml = "1 Jan 2023"; // Assuming _date.ToGedcom() returns "1 Jan 2023"
    StreamReader reader = new(memoryStream);
    string buffer = reader.ReadToEnd();
    Assert.Contains(expectedXml, buffer);
  }

  [Fact]
  public void DateWrapper_WriteXml_ShouldSerializeNil_WhenDateIsNull()
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

  [Fact]
  public void DateWrapper_WritePersonWithoutDate_ShouldSerializeCorrectly()
  {
    // Arrange
    Person personMissingBirthDate = new() { FullName = FullName, BirthDate = null };
    using (MemoryStream memoryStream = new())
    {
      // Act
      WriteTo<Person>(memoryStream, personMissingBirthDate);

      // Assert
      StreamReader reader = new(memoryStream);
      string buffer = reader.ReadToEnd();
      Assert.Equal(PersonWithoutDateXml, buffer);
    }
  }

  [Fact]
  public void DateWrapper_WritePersonWithEmptyDate_ShouldSerializeCorrectly()
  {
    // Arrange
    Person personWithEmptyDate = new() { FullName = FullName, BirthDate = new() };
    using (MemoryStream memoryStream = new())
    {
      // Act
      WriteTo<Person>(memoryStream, personWithEmptyDate);

      // Assert
      StreamReader reader = new(memoryStream);
      string buffer = reader.ReadToEnd();
      Assert.Equal(PersonWithEmptyDateXml, buffer);
    }
  }

  [Fact]
  public void DateWrapper_WritePersonWithDateTime_ShouldSerializeCorrectly()
  {
    // Arrange
    Person personWithDateTime = new() { FullName = FullName, BirthDate = new(BirthDateTime) };
    using (MemoryStream memoryStream = new())
    {
      // Act
      WriteTo<Person>(memoryStream, personWithDateTime);

      // Assert
      StreamReader reader = new(memoryStream);
      string buffer = reader.ReadToEnd();
      Assert.Equal(PersonWithGenealogyDateXml, buffer);
    }
  }

  [Fact]
  public void DateWrapper_WritePersonWithGenealogyDate_ShouldSerializeCorrectly()
  {
    // Arrange
    Person personWithValidDate = new() { FullName = FullName, BirthDate = new(BirthGenealogyDate) };
    using (MemoryStream memoryStream = new())
    {
      // Act
      WriteTo<Person>(memoryStream, personWithValidDate);

      // Assert
      StreamReader reader = new(memoryStream);
      string buffer = reader.ReadToEnd();
      Assert.Equal(PersonWithGenealogyDateXml, buffer);
    }
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
