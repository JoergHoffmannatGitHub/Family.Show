using Genealogy.Domain.Interfaces;
using Genealogy.Domain.ValueObjects.DateImplementation;

namespace Genealogy.Domain.Tests.ValueObjects.DateImplementation;

public class SimpleDateTests
{
  public static readonly TheoryData<string, string> ValidDates =
    new()
    {
      { "22 JUL 2023", "22 JUL 2023" },
      { "JUL 2023", "JUL 2023" },
      { "2023", "2023" },
      { "CAL SEP 1888", "CAL SEP 1888" },
      { "BET 1982 AND 1984", "BET 1982 AND 1984" },
      { "EST 1752", "EST 1752" },
      { "ABT JAN 1781", "ABT JAN 1781" },
      { "AFT JAN 1781", "AFT JAN 1781" },
      { "BEF JAN 1781", "BEF JAN 1781" },
      { "FROM 1670 TO 1800", "FROM 1670 TO 1800" },
      { "FROM 1670 TO JULIAN 1800", "FROM 1670 TO JULIAN 1800" },
      { "FROM JULIAN 1670 TO 1800", "FROM JULIAN 1670 TO 1800" }
    };

  [Theory, MemberData(nameof(ValidDates))]
  public void Constructor_ShouldStoreDate_WhenValidDateStringIsProvided(string inputDate, string expectedDate)
  {
    // Act
    var simpleDate = new SimpleDate(inputDate);

    // Assert
    Assert.Equal(expectedDate, simpleDate.ToString());
  }

  public static readonly TheoryData<string?> InvalidDates =
    [
      null,
      string.Empty,
      "123"
    ];

  [Theory, MemberData(nameof(InvalidDates))]
  public void Constructor_ShouldThrowGenealogyException_WhenInvalidDateStringIsProvided(string? inputDate)
  {
    // Act & Assert
    Assert.Throws<GenealogyException>(() => new SimpleDate(inputDate));
  }

  [Fact]
  public void Object_ShouldReturnCorrectComparisonResult_WhenComparingWithVariousObjects()
  {
    // Arrange
    SimpleDate date = new("22 JUL 2023");
    IDate? otherNull = null;
    IDate otherDateExact = new DateExact(2023, 7, 22);
    IDate otherSome = new SimpleDate("22 JUL 2023");
    IDate otherDifferent = new SimpleDate("23 JUL 2023");

    // Act & Assert
    Assert.False(date.Equals(otherNull));
    Assert.False(date.Equals(otherDateExact));
    Assert.True(date.Equals(otherSome));
    Assert.False(date.Equals(otherDifferent));
  }

  public static readonly TheoryData<string?, bool> EqualsObjectCases =
    new()
    {
      { "22 JUL 2023", true },
      { "23 JUL 2023", false },
      { "22 AUG 2023", false },
      { "22 JUL 2024", false },
      { null, false },
      { "NotASimpleDate", false }
    };

  [Theory, MemberData(nameof(EqualsObjectCases))]
  public void EqualsObject_ShouldReturnCorrectComparisonResult_WhenComparingWithVariousObjects(string? stringDate, bool expectedResult)
  {
    // Arrange
    SimpleDate date = new("22 JUL 2023");
    object? obj = stringDate == null ? null : new SimpleDate(stringDate);

    // Act
    bool result = date.Equals(obj);

    // Assert
    Assert.Equal(expectedResult, result);
  }

  public static readonly TheoryData<string> HashCodeCases =
    new()
    {
        { "22 JUL 2023" },
        { "23 JUL 2023" },
        { "22 AUG 2023" },
        { "22 JUL 2024" }
    };

  [Theory, MemberData(nameof(HashCodeCases))]
  public void GetHashCode_ShouldReturnCorrectHashCode_WhenDateIsProvided(string dateString)
  {
    // Arrange
    SimpleDate date1 = new(dateString);
    SimpleDate date2 = new(dateString);

    // Act
    int hashCode1 = date1.GetHashCode();
    int hashCode2 = date2.GetHashCode();

    // Assert
    Assert.Equal(hashCode1, hashCode2);
  }
}
