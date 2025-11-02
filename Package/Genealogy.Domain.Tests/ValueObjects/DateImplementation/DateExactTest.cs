using Genealogy.Domain.ValueObjects.DateImplementation;

namespace Genealogy.Domain.Tests.ValueObjects.DateImplementation;

public class DateExactTest
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenDayWithoutMonthStringIsProvided()
    {
        // Act & Assert
        Assert.Throws<GenealogyException>(() => new DateExact(1980, 0, 15));
    }

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
        Assert.Throws<GenealogyException>(() => new DateExact(invalidDateString));
    }

    [Theory, MemberData(nameof(ValidToGedcomDates))]
    public void ToGedcom_ShouldReturnCorrectGedcomString_WhenValidYearMonthDayAreProvided(
        int year,
        int month,
        int day,
        string expectedGedcom)
    {
        // Arrange
        DateExact dateExact = new(year, month, day);

        // Act
        string result = dateExact.ToString();

        // Assert
        Assert.Equal(expectedGedcom, result);
    }

    [Theory]
    [InlineData("1670", "1670")]
    [InlineData("@#DGREGORIAN@1670", "1670")]
    [InlineData("@#DJULIAN@1670", "@#DJULIAN@ 1670")]
    [InlineData("@#DHEBREW@1670", "@#DHEBREW@ 1670")]
    [InlineData("@#DFRENCH R@1670", "@#DFRENCH R@ 1670")]
    [InlineData("@#DROMAN@1670", "@#DROMAN@ 1670")]
    [InlineData("@#DUNKNOWN@1670", "@#DUNKNOWN@ 1670")]
    [InlineData("@#DGREGORIAN@JUN 1670", "Jun 1670")]
    [InlineData("@#DJULIAN@ OCT 1670", "@#DJULIAN@ Oct 1670")]
    [InlineData("@#DHEBREW@ SHV 1670", "@#DHEBREW@ Shv 1670")]
    [InlineData("@#DFRENCH R@ VENT 1670", "@#DFRENCH R@ Vent 1670")]
    public void ToGedcom_ShouldReturnValidGedcom_WhenGedcomWithCalendarIsProvided(string gedcom, string expectedGedcom)
    {
        // Arrange
        DateExact dateExact = new(gedcom);

        // Act
        string result = dateExact.ToString();

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
    public void ToGedcom_ShouldReturnCorrectGedcomString_WhenYearAndEachGregorianMonthAreProvided(
        int month,
        string expectedDate)
    {
        // Arrange
        DateExact dateExact = new(2023, month);

        // Act
        string gedcomString = dateExact.ToString();

        // Assert
        Assert.Equal(expectedDate, gedcomString);
    }

    [Theory]
    [InlineData("@#DFRENCH R@ 1670", "@#DFRENCH R@ 1670")]
    [InlineData("@#DFRENCH R@ VEND 1670", "@#DFRENCH R@ Vend 1670")]
    [InlineData("@#DFRENCH R@ BRUM 1670", "@#DFRENCH R@ Brum 1670")]
    [InlineData("@#DFRENCH R@ FrIM 1670", "@#DFRENCH R@ Frim 1670")]
    [InlineData("@#DFRENCH R@ NIvO 1670", "@#DFRENCH R@ Nivo 1670")]
    [InlineData("@#DFRENCH R@ PLUV 1670", "@#DFRENCH R@ Pluv 1670")]
    [InlineData("@#DFRENCH R@ VENt 1670", "@#DFRENCH R@ Vent 1670")]
    [InlineData("@#DFRENCH R@ GERM 1670", "@#DFRENCH R@ Germ 1670")]
    [InlineData("@#DFRENCH R@ FLOR 1670", "@#DFRENCH R@ Flor 1670")]
    [InlineData("@#DFRENCH R@ PRAI 1670", "@#DFRENCH R@ Prai 1670")]
    [InlineData("@#DFRENCH R@ MESS 1670", "@#DFRENCH R@ Mess 1670")]
    [InlineData("@#DFRENCH R@ THER 1670", "@#DFRENCH R@ Ther 1670")]
    [InlineData("@#DFRENCH R@ FRUC 1670", "@#DFRENCH R@ Fruc 1670")]
    [InlineData("@#DFRENCH R@ COMP 1670", "@#DFRENCH R@ Comp 1670")]
    public void ToGedcom_ShouldReturnCorrectGedcomString_WhenYearAndEachFrenchMonthAreProvided(
    string gedcom,
    string expectedDate)
    {
        // Arrange
        DateExact dateExact = new(gedcom);

        // Act
        string gedcomString = dateExact.ToString();

        // Assert
        Assert.Equal(expectedDate, gedcomString);
    }

    [Theory]
    [InlineData("@#DHEBREW@ 1670", "@#DHEBREW@ 1670")]
    [InlineData("@#DHEBREW@ TSH 1670", "@#DHEBREW@ Tsh 1670")]
    [InlineData("@#DHEBREW@ CSh 1670", "@#DHEBREW@ Csh 1670")]
    [InlineData("@#DHEBREW@ KSL 1670", "@#DHEBREW@ Ksl 1670")]
    [InlineData("@#DHEBREW@ 04 TVT 1670", "@#DHEBREW@ 4 Tvt 1670")]
    [InlineData("@#DHEBREW@ SHV 1670", "@#DHEBREW@ Shv 1670")]
    [InlineData("@#DHEBREW@ ADR 1670", "@#DHEBREW@ Adr 1670")]
    [InlineData("@#DHEBREW@ ADS 1670", "@#DHEBREW@ Ads 1670")]
    [InlineData("@#DHEBREW@ NSN 1670", "@#DHEBREW@ Nsn 1670")]
    [InlineData("@#DHEBREW@ IYR 1670", "@#DHEBREW@ Iyr 1670")]
    [InlineData("@#DHEBREW@ SVN 1670", "@#DHEBREW@ Svn 1670")]
    [InlineData("@#DHEBREW@ TMZ 1670", "@#DHEBREW@ Tmz 1670")]
    [InlineData("@#DHEBREW@ AAV 1670", "@#DHEBREW@ Aav 1670")]
    [InlineData("@#DHEBREW@ ELL 1670", "@#DHEBREW@ Ell 1670")]
    public void ToGedcom_ShouldReturnCorrectGedcomString_WhenYearAndEachHebrewMonthAreProvided(
    string gedcom,
    string expectedDate)
    {
        // Arrange
        DateExact dateExact = new(gedcom);

        // Act
        string gedcomString = dateExact.ToString();

        // Assert
        Assert.Equal(expectedDate, gedcomString);
    }

    [Fact]
    public void ToGedcom_ShouldThrowException_WhenDateMonthIsWrong()
    {
        // Arrange
        DateExact dateExact = new(2023, 13);

        // Act & Assert
        Assert.Throws<GenealogyException>(() => dateExact.ToString());
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

    [Fact]
    public void GetHashCode_ShouldReturnCorrectHashCode_WhenDateIsProvided()
    {
        // Arrange
        DateExact date1 = new(2023, 7, 22);
        DateExact date2 = new(2023, 7, 22);

        // Act
        int hash1 = date1.GetHashCode();
        int hash2 = date2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Theory]
    [InlineData("22 Jul 2023", true)]
    [InlineData("Jul 2023", true)]
    [InlineData("2023", true)]
    [InlineData("XYZ2023", false)]
    [InlineData("ABT", false)]
    [InlineData("", false)]
    public void TryParse_ShouldReturnExpectedResult(string input, bool expected)
    {
        bool result = DateExact.TryParse(input, out DateExact? date);
        Assert.Equal(expected, result);
        if (expected)
        {
            Assert.NotNull(date);
            Assert.Equal(input, date!.ToString());
        }
        else
        {
            Assert.Null(date);
        }
    }
}
