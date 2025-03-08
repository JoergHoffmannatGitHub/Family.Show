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
  public void Extension_ToShortString_ShallRetunExpectedDate(DateTime date, string cultureInfo, string expected)
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
  public void Extension_NullableDateToShortString_ShallRetunExpectedDate(DateTime? date, string cultureInfo, string expected)
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

  public static readonly TheoryData<string> InvalidStringDateCases =
    new()
    {
      { string.Empty },
      { "invalid" },
      { "31 DEV 9999" },
    };

  [Theory, MemberData(nameof(InvalidStringDateCases))]
  public void Extension_SetBirthDate_InvalidDateShallReturnNull(string birthDate)
  {
    // Arrange
    Person newPerson = new("firstNames", "lastName")
    {
      IsLiving = false
    };

    // Act
    newPerson.SetBirthDate(birthDate);

    // Assert
    Assert.Null(newPerson.BirthDate);
  }

  [Theory, MemberData(nameof(InvalidStringDateCases))]
  public void Extension_IsNullOrEmpty_InvalidDateShallReturnNull(string birthDate)
  {
    // Arrange
    Person newPerson = new("firstNames", "lastName")
    {
      IsLiving = false
    };

    // Act
    newPerson.SetBirthDate(birthDate);

    // Assert
    Assert.True(DateWrapper.IsNullOrEmpty(newPerson.BirthDate));
  }

  [Fact]
  public void Extension_SetBirthDate_ValidDateShallReturnExpectedDate()
  {
    // Arrange
    Person newPerson = new("firstNames", "lastName")
    {
      IsLiving = false
    };

    // Act
    newPerson.SetBirthDate("6 SEP 1888");

    // Assert
    Assert.Equal(new(1888, 9, 6), newPerson.BirthDate);
  }

  [Fact]
  public void Extension_IsNullOrEmpty_ValidDateShallReturnExpectedDate()
  {
    // Arrange
    Person newPerson = new("firstNames", "lastName")
    {
      IsLiving = false
    };

    // Act
    newPerson.SetBirthDate("6 SEP 1888");

    // Assert
    Assert.False(DateWrapper.IsNullOrEmpty(newPerson.BirthDate));
  }
}
