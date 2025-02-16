using Genealogy.DateImplementation;

namespace Genealogy.Tests.DateImplementation;

public class DateExactTests
{
  [Fact]
  public void Constructor_ShouldInitializeDate_WhenValidDateStringIsProvided()
  {
    // Arrange
    string dateString = "22 JUL 2023";

    // Act
    DateExact dateExact = new(dateString);

    // Assert
    Assert.Equal(2023, dateExact.Year);
    Assert.Equal(7, dateExact.Month);
    Assert.Equal(22, dateExact.Day);
  }

  [Fact]
  public void Constructor_ShouldThrowException_WhenInvalidDateStringIsProvided()
  {
    // Arrange
    string invalidDateString = "InvalidDate";

    // Act & Assert
    _ = Assert.Throws<NotImplementedException>(() => new DateExact(invalidDateString));
  }

  [Fact]
  public void Constructor_ShouldInitializeDate_WhenYearMonthDayAreProvided()
  {
    // Arrange
    int year = 2023;
    int month = 7;
    int day = 22;

    // Act
    DateExact dateExact = new(year, month, day);

    // Assert
    Assert.Equal(year, dateExact.Year);
    Assert.Equal(month, dateExact.Month);
    Assert.Equal(day, dateExact.Day);
  }

  [Fact]
  public void ToGedcom_ShouldReturnCorrectGedcomString_WhenDateIsComplete()
  {
    // Arrange
    DateExact dateExact = new(2023, 7, 22);

    // Act
    string gedcomString = dateExact.ToGedcom();

    // Assert
    Assert.Equal("22 JUL 2023", gedcomString);
  }

  [Fact]
  public void ToGedcom_ShouldReturnCorrectGedcomString_WhenOnlyYearIsProvided()
  {
    // Arrange
    DateExact dateExact = new(2023);

    // Act
    string gedcomString = dateExact.ToGedcom();

    // Assert
    Assert.Equal("2023", gedcomString);
  }

  [Fact]
  public void ToGedcom_ShouldReturnCorrectGedcomString_WhenYearAndMonthAreProvided()
  {
    // Arrange
    DateExact dateExact = new(2023, 7);

    // Act
    string gedcomString = dateExact.ToGedcom();

    // Assert
    Assert.Equal("JUL 2023", gedcomString);
  }

  public static readonly TheoryData<int, string> MonthCases =
    new()
    {
      { 0, "2023" },
      { 1, "JAN 2023" },
      { 2, "FEB 2023" },
      { 3, "MAR 2023" },
      { 4, "APR 2023" },
      { 5, "MAY 2023" },
      { 6, "JUN 2023" },
      { 7, "JUL 2023" },
      { 8, "AUG 2023" },
      { 9, "SEP 2023" },
      { 10, "OCT 2023" },
      { 11, "NOV 2023" },
      { 12, "DEC 2023" },
    };

  [Theory, MemberData(nameof(MonthCases))]
  public void ToGedcom_ShouldReturnCorrectGedcomString_WhenYearAndEachMonthAreProvided(int month, string expectedDate)
  {
    // Arrange
    DateExact dateExact = new(2023, month);

    // Act
    string gedcomString = dateExact.ToGedcom();

    // Assert
    Assert.Equal(expectedDate, gedcomString);
  }

  [Fact]
  public void Constructor_ShouldThrowException_WhenInvaliMonthIsProvided()
  {
    // Arrange
    DateExact dateExact = new(2023, 13);

    // Act & Assert
    _ = Assert.Throws<NotImplementedException>(dateExact.ToGedcom);
  }

  [Fact]
  public void Equals_ShouldReturnTrue_WhenDatesAreEqual()
  {
    // Arrange
    DateExact date1 = new(2023, 7, 22);
    DateExact date2 = new(2023, 7, 22);

    // Act
    bool result = date1.Equals(date2);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void Equals_ShouldReturnFalse_WhenDatesAreNotEqual()
  {
    // Arrange
    DateExact date1 = new(2023, 7, 22);
    DateExact date2 = new(2023, 7, 23);

    // Act
    bool result = date1.Equals(date2);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void Equals_ShouldReturnFalse_WhenOtherIsNull()
  {
    // Arrange
    DateExact date = new(2023, 7, 22);

    // Act
    bool result = date.Equals(null);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void Equals_ShouldReturnFalse_WhenOtherIsNotDateExact()
  {
    // Arrange
    DateExact date = new(2023, 7, 22);
    object obj = new();

    // Act
    bool result = date.Equals(obj);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void GetHashCode_ShouldReturnSameHashCode_WhenDatesAreEqual()
  {
    // Arrange
    DateExact date1 = new(2023, 7, 22);
    DateExact date2 = new(2023, 7, 22);

    // Act
    int hashCode1 = date1.GetHashCode();
    int hashCode2 = date2.GetHashCode();

    // Assert
    Assert.Equal(hashCode1, hashCode2);
  }

  [Fact]
  public void GetHashCode_ShouldReturnDifferentHashCode_WhenDatesAreNotEqual()
  {
    // Arrange
    DateExact date1 = new(2023, 7, 22);
    DateExact date2 = new(2023, 7, 23);

    // Act
    int hashCode1 = date1.GetHashCode();
    int hashCode2 = date2.GetHashCode();

    // Assert
    Assert.NotEqual(hashCode1, hashCode2);
  }
}
