using Genealogy.Domain.ValueObjects.DateImplementation;

namespace Genealogy.Domain.Tests.ValueObjects.DateImplementation;

public class DateExactTests
{
  public static readonly TheoryData<int, int, int, string> ValidToGedcomDates =
    new()
    {
      { 2023, 7, 22, "22 Jul 2023" },
      { 2023, 7, 0, "Jul 2023" },
      { 2023, 0, 0, "2023" }
    };

  [Theory, MemberData(nameof(ValidToGedcomDates))]
  public void Constructor_ShouldInitializeDate_WhenDateStringWithYearMonthDayCalendarIsProvided(
    int expectedYear,
    int expectedMonth,
    int expectedDay,
    string dateString)
  {
    // Act
    DateExact dateExact = new(dateString);

    // Assert
    Assert.Equal(expectedYear, dateExact.Year);
    Assert.Equal(expectedMonth, dateExact.Month);
    Assert.Equal(expectedDay, dateExact.Day);
  }

  [Fact]
  public void Constructor_ShouldThrowException_WhenInvalidDateStringIsProvided()
  {
    // Arrange
    string invalidDateString = "InvalidDate";

    // Act & Assert
    Assert.Throws<NotImplementedException>(() => new DateExact(invalidDateString));
  }

  [Theory, MemberData(nameof(ValidToGedcomDates))]
  public void ToGedcom_ShouldReturnCorrectGedcomString_WhenValidYearMonthDayAreProvided(int year, int month, int day, string expectedGedcom)
  {
    // Arrange
    DateExact dateExact = new(year, month, day);

    // Act
    string result = dateExact.ToGedcom();

    // Assert
    Assert.Equal(expectedGedcom, result);
  }

  [Theory]
  [InlineData("1670", "1670")]
  [InlineData("@#DGREGORIAN@1670", "1670")]
  [InlineData("@#DJULIAN@1670","@#DJULIAN@1670")]
  [InlineData("@#DHEBREW@1670", "@#DHEBREW@1670")]
  [InlineData("@#DFRENCH R@1670", "@#DFRENCH R@1670")]
  [InlineData("@#DROMAN@1670", "@#DROMAN@1670")]
  [InlineData("@#DUNKNOWN@1670", "@#DUNKNOWN@1670")]
  public void ToGedcom_ShouldReturnValidGedcom_WhenGedcomWithCalendarIsProvided(string gedcom, string expectedGedcom)
  {
    // Arrange
    DateExact dateExact = new(gedcom);

    // Act
    string result = dateExact.ToGedcom();

    // Assert
    Assert.Equal(expectedGedcom, result);
  }

  public static readonly TheoryData<int, string> GregorianMonthCases =
    new()
    {
      { 0, "2023" },
      { 1, "Jan 2023" },
      { 2, "Feb 2023" },
      { 3, "Mar 2023" },
      { 4, "Apr 2023" },
      { 5, "May 2023" },
      { 6, "Jun 2023" },
      { 7, "Jul 2023" },
      { 8, "Aug 2023" },
      { 9, "Sep 2023" },
      { 10, "Oct 2023" },
      { 11, "Nov 2023" },
      { 12, "Dec 2023" }
    };

  [Theory, MemberData(nameof(GregorianMonthCases))]
  public void ToGedcom_ShouldReturnCorrectGedcomString_WhenYearAndEachGregorianMonthAreProvided(int month, string expectedDate)
  {
    // Arrange
    DateExact dateExact = new(2023, month);

    // Act
    string gedcomString = dateExact.ToGedcom();

    // Assert
    Assert.Equal(expectedDate, gedcomString);
  }

  [Fact]
  public void ToGedcom_ShouldThrowException_WhenDateMonthIsWrong()
  {
    // Arrange
    DateExact dateExact = new(2023, 13);

    // Act & Assert
    Assert.Throws<GenealogyException>(() => dateExact.ToGedcom());
  }

  public static readonly TheoryData<int, int, int, bool> EqualsDates =
    new()
    {
      { 2023, 7, 22, true },
      { 2023, 7, 23, false },
      { 2023, 8, 22, false },
      { 2024, 7, 22, false }
    };

  [Theory, MemberData(nameof(EqualsDates))]
  public void Equals_ShouldReturnCorrectComparisonResult_WhenComparingTwoDates(
    int year,
    int month,
    int day,
    bool expectedResult)
  {
    // Arrange
    DateExact date1 = new(2023, 7, 22);
    DateExact date2 = new(year, month, day);

    // Act
    bool result = date1.Equals(date2);

    // Assert
    Assert.Equal(expectedResult, result);
  }

  public static readonly TheoryData<int, int, int, bool> EqualsDateExactObjectCases =
    new()
    {
      { 2023, 7, 22, true },
      { 2023, 7, 23, false },
      { 2023, 8, 22, false },
      { 2024, 7, 22, false }
    };

  [Theory, MemberData(nameof(EqualsDateExactObjectCases))]
  public void EqualsObject_ShouldReturnCorrectComparisonResult_WhenComparingWithVariousDateExactObjectts(
    int year,
    int month,
    int day,
    bool expectedResult)
  {
    // Arrange
    DateExact date = new(2023, 7, 22);
    object obj = new DateExact(year, month, day);

    // Act
    bool result = date.Equals(obj);

    // Assert
    Assert.Equal(expectedResult, result);
  }

  public static readonly TheoryData<string?> EqualsDateObjectCases =
    new()
    {
      { null },
      { "NotADateExact" }
    };

  [Theory, MemberData(nameof(EqualsDateObjectCases))]
  public void EqualsObject_ShouldReturnCorrectComparisonResult_WhenComparingWithVariousObjects(
    object? obj)
  {
    // Arrange
    DateExact date = new(2023, 7, 22);

    // Act
    bool result = date.Equals(obj);

    // Assert
    Assert.False(result);
  }

  public static readonly TheoryData<int, int, int, int> HashCodeDates =
    new()
    {
      { 2023, 7, 22, new DateExact(2023, 7, 22).GetHashCode() },
      { 2023, 7, 23, new DateExact(2023, 7, 23).GetHashCode() },
      { 2023, 8, 22, new DateExact(2023, 8, 22).GetHashCode() },
      { 2024, 7, 22, new DateExact(2024, 7, 22).GetHashCode() }
    };

  [Theory, MemberData(nameof(HashCodeDates))]
  public void GetHashCode_ShouldReturnCorrectHashCode_WhenDateIsProvided(
    int year,
    int month,
    int day,
    int expectedHashCode)
  {
    // Arrange
    DateExact date = new(year, month, day);

    // Act
    int result = date.GetHashCode();

    // Assert
    Assert.Equal(expectedHashCode, result);
  }
}
