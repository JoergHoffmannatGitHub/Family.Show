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

    [Theory]
    [InlineData("2 AUG 1917", true, "2 Aug 1917")]
    [InlineData("AUG 1917", true, "Aug 1917")]
    [InlineData("AUG 2 1917", false, "")]
    [InlineData("Aug 1917", true, "Aug 1917")]
    [InlineData("1917", true, "1917")]
    [InlineData("BEF 02 AUG 1917", true, "BEF 2 Aug 1917")]
    [InlineData("BEF AUG 1917", true, "BEF Aug 1917")]
    [InlineData("BEF 1917", true, "BEF 1917")]
    [InlineData("EST 02 AUG 1917", true, "EST 2 Aug 1917")]
    [InlineData("EST AUG 1917", true, "EST Aug 1917")]
    [InlineData("EST 1917", true, "EST 1917")]
    [InlineData("ABT 02 AUG 1917", true, "ABT 2 Aug 1917")]
    [InlineData("ABT AUG 1917", true, "ABT Aug 1917")]
    [InlineData("ABT 1917", true, "ABT 1917")]
    [InlineData("CAL 02 AUG 1917", true, "CAL 2 Aug 1917")]
    [InlineData("CAL AUG 1917", true, "CAL Aug 1917")]
    [InlineData("CAL 1917", true, "CAL 1917")]
    [InlineData("AFT 02 AUG 1917", true, "AFT 2 Aug 1917")]
    [InlineData("AFT AUG 1917", true, "AFT Aug 1917")]
    [InlineData("AFT 1917", true, "AFT 1917")]
    [InlineData("BET 02 AUG 1917 AND 15 AUG 1917", true, "BET 2 Aug 1917 AND 15 Aug 1917")]
    [InlineData("BET 02 AUG 1917 AND SEP 1917", true, "BET 2 Aug 1917 AND Sep 1917")]
    [InlineData("BET AUG 1917 AND SEP 1917", true, "BET Aug 1917 AND Sep 1917")]
    [InlineData("BET 1917 AND 1919", true, "BET 1917 AND 1919")]
    [InlineData("FROM 02 AUG 1917 TO 15 AUG 1917", true, "FROM 2 Aug 1917 TO 15 Aug 1917")]
    [InlineData("FROM 02 AUG 1917 TO SEP 1917", true, "FROM 2 Aug 1917 TO Sep 1917")]
    [InlineData("FROM AUG 1917 TO SEP 1917", true, "FROM Aug 1917 TO Sep 1917")]
    [InlineData("FROM 1917 TO 1919", true, "FROM 1917 TO 1919")]
    [InlineData("FROM 02 AUG 1917", true, "FROM 2 Aug 1917")]
    [InlineData("TO SEP 1917", true, "TO Sep 1917")]
    [InlineData("INT 1917 (3 years after marriage)", true, "INT 1917 (3 years after marriage)")]
    [InlineData("INT (3 years after marriage)", false, "")]
    [InlineData("(3 years after marriage)", true, "(3 years after marriage)")]
    [InlineData("3 years after marriage", false, "")]
    [InlineData("(3 years after marriage following the birth of their first child)", false, "")]
    [InlineData("(3 years after marriage", false, "")]
    [InlineData("INT (2 days before wedding)", false, "")]
    [InlineData("INT (two days before wedding)", false, "")]
    [InlineData("15 APR 1699/00", true, "15 Apr 1699/00")]
    [InlineData("11 FEB 1750/51", true, "11 Feb 1750/51")]
    [InlineData("AUG 1917/18", true, "Aug 1917/18")]
    [InlineData("1917/18", true, "1917/18")]
    [InlineData("2 AUG 1917/19", false, "")]
    [InlineData("@#DGREGORIAN@ 02 AUG 1917", true, "2 Aug 1917")]
    [InlineData("2 AUG 1917/18", true, "2 Aug 1917/18")]
    [InlineData("@#DGREGORIAN@ 02 AUG 1917/18", true, "2 Aug 1917/18")]
    [InlineData("@#DJULIAN@ 02 AUG 1917/18", false, "")]
    [InlineData("100 B.C.", true, "100 B.C.")]
    [InlineData("100B.C.", true, "100 B.C.")]
    [InlineData("BEF 100B.C.", true, "BEF 100 B.C.")]
    [InlineData("100BC", false, "")]
    [InlineData("2 JUL 100B.C.", false, "")]
    [InlineData("@#DJULIAN@ 02 AUG 1917", true, "@#DJULIAN@ 2 Aug 1917")]
    [InlineData("@#DHEBREW@ 02 TSH 1917", true, "@#DHEBREW@ 2 Tsh 1917")]
    [InlineData("02 TSH 5250", false, "")]
    [InlineData("@#DFRENCH R@ 02 VEND 1917", true, "@#DFRENCH R@ 2 Vend 1917")]
    [InlineData("@#DFRENCHR@ 02 VEND 1917", false, "")]
    public void TryParse_ShouldReturn_WhenIDateStringIsProvided(string gedcomInput, bool valid, string expectedGedcom)
    {
        // Act
        bool result = Date.TryParse(gedcomInput, out IDate dateResult);

        // Assert
        Assert.Equal(valid, result);
        if (valid)
        {
            Assert.NotNull(dateResult);
            Assert.Equal(expectedGedcom, dateResult.ToString());
        }
        else
        {
            Assert.Null(dateResult);
        }
    }
}
