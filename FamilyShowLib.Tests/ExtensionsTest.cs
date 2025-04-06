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
  public void ToShortString_WithDateTime_ShouldReturnCorrectly(DateTime date, string cultureInfo, string expected)
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
  public void ToShortString_WithNullableDateTime_ShouldReturnCorrectly(DateTime? date, string cultureInfo, string expected)
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

  public static readonly TheoryData<DateTime?, bool> IsNullOrEmptyCases =
    new()
    {
      { null, true },
      { DateTime.MinValue, true },
      { DateTime.MaxValue, true },
      { new DateTime(1950, 5, 8), false },
    };

  [Theory, MemberData(nameof(IsNullOrEmptyCases))]
  public void IsNullOrEmpty_ShouldReturnCorrectly(DateTime? date, bool expected)
  {
    // Arrange

    // Act
    bool result = date.IsNullOrEmpty();

    // Assert
    Assert.Equal(expected, result);
  }

  public static readonly TheoryData<string, string, DateTime?> ToDateCases =
    new()
    {
      { string.Empty, "en-GB", null },
      { "01-01-0001", "en-GB", new DateTime(1, 1, 1) },
      { "1950", "en-GB", new DateTime(1950, 1, 1) },
      { "08/05/1950", "en-GB", new DateTime(1950, 5, 8) },
      { "08/05/1950", "en-US", new DateTime(1950, 8, 5) },
      { "08-05-1950", "en-GB", new DateTime(1950, 5, 8) },
      { "08-05-1950", "en-US", new DateTime(1950, 8, 5) },
      { "18-05-1950", "en-GB", new DateTime(1950, 5, 18) },
      { "18-05-1950", "en-US", null },
      { "26061965", "en-GB", null },
    };

  [Theory, MemberData(nameof(ToDateCases))]
  public void ToDate_ShouldReturnCorrectly(string source, string cultureInfo, DateTime? expected)
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

  public static readonly TheoryData<string, int, int, int> BirthDateCases =
    new()
    {
      { string.Empty, 0, 0, 0 },
      { "invalid", 0, 0, 0 },
      { "6 SEP 1888", 1888, 9, 6 },
      { "1 JAN 0001", 1, 1, 1 },
      { "31 DEC 9999", 9999, 12, 31 },
    };

  [Theory, MemberData(nameof(BirthDateCases))]
  public void SetBirthDate_ShouldReturnCorrectly(string birthDate, int year, int month, int day)
  {
    // Arrange
    Person newPerson = new("firstNames", "lastName")
    {
      IsLiving = false
    };
    DateWrapper? expected = (year == 0 ? null : new DateWrapper(year, month, day));

    // Act
    newPerson.SetBirthDate(birthDate);

    // Assert
    Assert.Equal(expected, newPerson.BirthDate);
  }
}
