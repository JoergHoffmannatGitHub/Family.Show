using Genealogy.Domain.Interfaces;
using Genealogy.Domain.ValueObjects;
using Genealogy.Domain.ValueObjects.DateImplementation;

namespace Genealogy.Domain.Tests.ValueObjects;

public class DateTest
{
    public static readonly TheoryData<string, Type> ValidGedcomDates =
      new()
      {
        { "1977", typeof(DateExact) },
        { "22 Jul 2023", typeof(DateExact) },
        { "6 May 2001", typeof(DateExact) },
        { "ABT Jan 1781", typeof(DateApproximated) },
        { "AFT Jan 1781", typeof(DateRange) },
        { "BEF Jan 1781", typeof(DateRange) },
        { "BET 1982 AND 1984", typeof(DateRange) },
        { "CAL Sep 1888", typeof(DateApproximated) },
        { "EST 1752", typeof(DateApproximated) },
        { "FROM 1670 TO 1800", typeof(DatePeriod) },
        { "FROM 1670 TO @#DJULIAN@ 1800", typeof(DatePeriod) },
        { "FROM @#DJULIAN@ 1670 TO 1800", typeof(DatePeriod) },
        { "Jul 2023", typeof(DateExact) }
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
    public void Create_ShouldReturnCorrectDate_WhenValidYearMonthDayAreProvided(
        int year,
        int month,
        int day,
        string expectedGedcom)
    {
        // Act
        IDate result = Date.Create(year, month, day);

        // Assert
        Assert.Equal(expectedGedcom, result.ToString());
    }
}
