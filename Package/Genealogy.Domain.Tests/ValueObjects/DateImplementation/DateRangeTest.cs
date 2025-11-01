using Genealogy.Domain.ValueObjects.DateImplementation;

namespace Genealogy.Domain.Tests.ValueObjects.DateImplementation;

public class DateRangeTest
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenNullStringIsProvided()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DateRange(null));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenInvalidDatePrefixIsProvided()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new DateRange("CAL 22 Jul 2023"));
    }

    [Theory]
    [InlineData("BEF 17")]
    [InlineData("BET 1782 1784")]
    [InlineData("AFT 1982 AND 1984")]
    public void Constructor_ShouldThrowException_WhenInvalidDateStringIsProvided(string invalidDate)
    {
        // Act & Assert
        Assert.Throws<GenealogyException>(() => new DateRange(invalidDate));
    }

    [Theory]
    [InlineData("BEF Jan 1781")]
    [InlineData("AFT Jan 1781")]
    [InlineData("BET 1982 AND 1984")]
    public void Constructor_ShouldInitializeDate_WhenDateApproximatedStringIsProvided(
      string dateString)
    {
        // Act
        DateRange dateApproximated = new(dateString);

        // Assert
        Assert.Equal(dateString, dateApproximated.ToString());
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForSameTypeAndDate()
    {
        // Arrange
        var a = new DateRange("BET 1982 AND 1984");
        var b = new DateRange("BET 1982 AND 1984");

        // Act & Assert
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentTypeOrDate()
    {
        // Arrange
        var a = new DateRange("BET 1982 AND 1984");
        var differentType = new DateRange("AFT 1982");
        var differentDate = new DateRange("BET 1983 AND 1984");

        // Act & Assert
        Assert.False(a.Equals(differentType));
        Assert.False(a.Equals(differentDate));
    }

    [Theory]
    [InlineData("BEF Jan 1781", true)]
    [InlineData("AFT Jan 1781", true)]
    [InlineData("BET 1982 AND 1984", true)]
    [InlineData("BEF2023", false)]
    [InlineData("AFT", false)]
    [InlineData("", false)]
    public void TryParse_ShouldReturnExpectedResult(string input, bool expected)
    {
        bool result = DateRange.TryParse(input, out DateRange? date);
        Assert.Equal(expected, result);
        if (expected)
        {
            Assert.NotNull(date);
            Assert.Equal(input, date.ToString());
        }
        else
        {
            Assert.Null(date);
        }
    }
}
