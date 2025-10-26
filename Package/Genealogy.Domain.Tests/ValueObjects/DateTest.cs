using Genealogy.Domain.Interfaces;
using Genealogy.Domain.ValueObjects;
using Genealogy.Domain.ValueObjects.DateImplementation;

namespace Genealogy.Domain.Tests.ValueObjects;

public class DateTest
{
  public static readonly TheoryData<string, Type> ValidGedcomDates =
    new()
    {
        { "22 Jul 2023", typeof(DateExact) },
        { "Jul 2023", typeof(DateExact) },
        { "2023", typeof(DateExact) },
        { "6 May 2001", typeof(DateExact) },
        { "1977", typeof(DateExact) },
        { "CAL SEP 1888", typeof(SimpleDate) }, // DateEstimate
        { "BET 1982 AND 1984", typeof(SimpleDate) }, // DateRange
        { "EST 1752", typeof(SimpleDate) }, // DateEstimate
        { "ABT JAN 1781", typeof(SimpleDate) }, // DateApproximate
        { "AFT JAN 1781", typeof(SimpleDate) }, // DateApproximate
        { "BEF JAN 1781", typeof(SimpleDate) }, // DateApproximate
        { "FROM 1670 TO 1800", typeof(SimpleDate) }, // DateRange
        { "FROM 1670 TO JULIAN 1800", typeof(SimpleDate) }, // DateRange
        { "FROM JULIAN 1670 TO 1800", typeof(SimpleDate) } // DateRange
    };

  [Theory, MemberData(nameof(ValidGedcomDates))]
  public void TryParse_ShouldReturnTrueAndDateType_WhenValidDateStringIsProvided(string inputDate, Type type)
  {
    // Act
    bool result = Date.TryParse(inputDate, out IDate dateResult);

    // Assert
    Assert.True(result);
    Assert.Equal(inputDate, dateResult.ToString());
    Assert.IsType(type, dateResult);
  }

  public static readonly TheoryData<string> InvalidGedcomDates =
    [
      null,
      string.Empty,
      "123",
    ];

  [Theory, MemberData(nameof(InvalidGedcomDates))]
  public void TryParse_ShouldReturnFalse_WhenInvalidDateStringIsProvided(string date)
  {
    // Act & Assert
    Assert.False(Date.TryParse(date, out _));
  }

  public static readonly TheoryData<int, int, int, string> ValidCreateDates =
    new()
    {
      { 2023, 7, 22, "22 Jul 2023" },
      { 2023, 7, 0, "Jul 2023" },
      { 2023, 0, 0, "2023" }
    };

  [Theory, MemberData(nameof(ValidCreateDates))]
  public void Create_ShouldReturnCorrectDate_WhenValidYearMonthDayAreProvided(int year, int month, int day, string expectedGedcom)
  {
    // Act
    IDate result = Date.Create(year, month, day);

    // Assert
    Assert.Equal(expectedGedcom, result.ToString());
  }
}
