using System.Globalization;

namespace FamilyShowLib.Tests;

public class ExtensionsTest
{
  public static readonly TheoryData<DateTime, string, string> DateToShortStringCases =
    new()
    {
      { DateTime.MinValue, "de-DE", "01.01.0001" },
      { new DateTime(1950, 5, 8), "de-DE", "08.05.1950" },
      { new DateTime(1950, 5, 8), "en-GB", "08/05/1950" },
      { new DateTime(1950, 5, 8), "en-US", "5/8/1950" },
      { new DateTime(1950, 5, 8), "es-ES", "8/5/1950" },
      { new DateTime(1950, 5, 8), "it-IT", "08/05/1950" },
    };

  [Theory, MemberData(nameof(DateToShortStringCases))]
  public void ToShortStringTest(DateTime date, string cultureInfo, string expected)
  {
    using (new AnotherCulture(cultureInfo))
    {
      // Arrange

      // Act
      string result = date.ToShortString();

      // Assert
      Assert.Equal(expected, result);
    }
  }

  public static readonly TheoryData<DateTime?, string, string> NullableDateToStringCases =
    new()
    {
      { new DateTime(1950, 5, 8), "de-DE", "08.05.1950" },
      { new DateTime(1950, 5, 8), "en-GB", "08/05/1950" },
      { new DateTime(1950, 5, 8), "en-US", "5/8/1950" },
      { new DateTime(1950, 5, 8), "es-ES", "8/5/1950" },
      { new DateTime(1950, 5, 8), "it-IT", "08/05/1950" },
      { null, "en-US", string.Empty }
    };

  [Theory, MemberData(nameof(NullableDateToStringCases))]
  public void NullableDateToShortStringTest(DateTime? date, string cultureInfo, string expected)
  {
    using (new AnotherCulture(cultureInfo))
    {
      // Arrange
      CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(cultureInfo);

      // Act
      string result = date.ToShortString();

      // Assert
      Assert.Equal(expected, result);
    }
  }

  public static readonly TheoryData<DateTime?, string> DateFormatCases =
    new()
    {
      { new DateTime(1888, 9, 6), "6/9/1888" },
      { null, string.Empty },
    };

  [Theory, MemberData(nameof(DateFormatCases))]
  public void DateFormatTest(DateTime? date, string expected)
  {
    // Arrange

    // Act
    string result = date.Format();

    // Assert
    Assert.Equal(expected, result);
  }

  public static readonly TheoryData<string, string, DateTime?> ToDateCases =
    new()
    {
      { string.Empty, "en-GB", null },
      { "01-01-0001", "en-GB", new DateTime(1, 1, 1) },
      { "08/05/1950", "en-GB", new DateTime(1950, 5, 8) },
      { "08/05/1950", "en-US", new DateTime(1950, 8, 5) },
      { "08-05-1950", "en-GB", new DateTime(1950, 5, 8) },
      { "08-05-1950", "en-US", new DateTime(1950, 8, 5) },
      { "18-05-1950", "en-GB", new DateTime(1950, 5, 18) },
      { "18-05-1950", "en-US", null },
      { "26061965", "en-GB", null },
    };

  [Theory, MemberData(nameof(ToDateCases))]
  public void ToDateTest(string source, string cultureInfo, DateTime? expected)
  {
    using (new AnotherCulture(cultureInfo))
    {
      // Arrange

      // Act
      DateTime? result = source.ToDate();

      // Assert
      Assert.Equal(expected, result);
    }
  }

  public static readonly TheoryData<string, DateTime?> StringDateCases =
    new()
    {
      { string.Empty, null },
      { "invalid", null },
      { "6 SEP 1888", new DateTime(1888, 9, 6) },
      { "1 JAN 0001", null },
      { "31 DEV 9999", null },
    };

  [Theory, MemberData(nameof(StringDateCases))]
  public void SetBirthDateTest(string birthDate, DateTime? expected)
  {
    // Arrange
    Person newPerson = new("firstNames", "lastName")
    {
      IsLiving = false
    };

    // Act
    newPerson.SetBirthDate(birthDate);
    // Assert

    Assert.Equal(expected, newPerson.BirthDate);
  }

  [Theory, MemberData(nameof(StringDateCases))]
  public void IsNullOrEmptyTest(string birthDate, DateTime? expected)
  {
    // Arrange
    Person newPerson = new("firstNames", "lastName")
    {
      IsLiving = false
    };

    // Act
    newPerson.SetBirthDate(birthDate);

    // Assert
    Assert.Equal(expected == null, newPerson.BirthDate.IsNullOrEmpty());
  }
}
