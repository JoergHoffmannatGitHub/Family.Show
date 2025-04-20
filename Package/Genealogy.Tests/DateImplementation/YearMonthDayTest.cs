// based on NodaTime.Test.YearMonthDayTest (see: https://github.com/nodatime/nodatime)

using System.Globalization;

using Genealogy.DateImplementation;

namespace Genealogy.Tests.DateImplementation;

public class YearMonthDayTest
{
  [Theory]
  [InlineData(2023, 12, 31)]
  [InlineData(1, 1, 1)]
  [InlineData(9999, 12, 31)]
  public void Constructor_ShouldStoreYearMonthDayCorrectly(int year, int month, int day)
  {
    // Arrange & Act
    var ymd = new YearMonthDay(year, month, day);

    // Assert
    Assert.Equal(year, ymd.Year);
    Assert.Equal(month, ymd.Month);
    Assert.Equal(day, ymd.Day);
  }

  [Fact]
  public void YearMonthDay_ShouldHandleAllYears()
  {
    // Range of years we actually care about. We support more, but that's okay.
    for (int year = -9999; year <= 9999; year++)
    {
      var ymd = new YearMonthDay(year, 5, 20);
      Assert.Equal(year, ymd.Year);
      Assert.Equal(5, ymd.Month);
      Assert.Equal(20, ymd.Day);
    }
  }

  [Fact]
  public void YearMonthDay_ShouldHandleAllMonths()
  {
    // We'll never actually need 32 months, but we support that many...
    for (int month = 0; month < 32; month++)
    {
      var ymd = new YearMonthDay(-123, month, 20);
      Assert.Equal(-123, ymd.Year);
      Assert.Equal(month, ymd.Month);
      Assert.Equal(20, ymd.Day);
    }
  }

  [Fact]
  public void YearMonthDay_ShouldHandleAllDays()
  {
    // We'll never actually need 64 days, but we support that many...
    for (int day = 0; day < 64; day++)
    {
      var ymd = new YearMonthDay(-123, 30, day);
      Assert.Equal(-123, ymd.Year);
      Assert.Equal(30, ymd.Month);
      Assert.Equal(day, ymd.Day);
    }
  }

  [Theory]
  [InlineData("1000-01-01", "1000-01-02")]
  [InlineData("1000-01-01", "1000-02-01")]
  [InlineData("999-16-64", "1000-01-01")]
  [InlineData("999-16-64", "1000-01-00")]
  [InlineData("999-16-64", "1000-00-00")]
  [InlineData("-1-01-01", "-1-01-02")]
  [InlineData("-1-01-01", "-1-02-01")]
  [InlineData("-2-16-64", "-1-01-01")]
  [InlineData("-1-16-64", "0-01-01")]
  [InlineData("-1-16-64", "1-01-01")]
  public void YearMonthDay_ShouldHandleComparisonsCorrectly(string smallerText, string greaterText)
  {
    // Arrange
    YearMonthDay smaller = Parse(smallerText);
    YearMonthDay greater = Parse(greaterText);

    // Act & Assert
    TestHelper.TestCompareToStruct(smaller, smaller, greater);
    TestHelper.TestOperatorComparisonEquality(smaller, smaller, greater);
    TestHelper.TestEqualsStruct(smaller, smaller, greater);
  }

  // Just for testing purposes... note that this does not perform clean validation.
  private static YearMonthDay Parse(string text)
  {
    // Handle a leading - to negate the year
    if (text[0] == '-')
    {
      YearMonthDay ymd = Parse(text.Substring(1));
      return new(-ymd.Year, ymd.Month, ymd.Day);
    }

    string[] bits = text.Split('-');
    return new(
        int.Parse(bits[0], CultureInfo.InvariantCulture),
        int.Parse(bits[1], CultureInfo.InvariantCulture),
        int.Parse(bits[2], CultureInfo.InvariantCulture));
  }
}
