using Genealogy.Domain.ValueObjects.DateImplementation;

namespace Genealogy.Domain.Tests.ValueObjects.DateImplementation;

public class DateInterpretedTest
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenNullProvided()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DateInterpreted(null));
    }

    [Theory]
    // There must be a date after the INT and before the date phrase.
    [InlineData("INT (3 years after marriage)")]
    // Date phrase must be enclosed in parentheses.
    [InlineData("3 years after marriage")]
    // Date phrase exceeds maximum length of 35 characters.
    [InlineData("(3 years after marriage following the birth of their first child)")]
    // Date phrases must be enclosed in parenthesis.
    [InlineData("(3 years after marriage")]
    [InlineData("CAL 22 Jul 2023")]
    [InlineData("INT 1917 3 years after marriage)")]
    public void Constructor_ShouldThrowException_WhenInvalidDateStringIsProvided(
        string dateString)
    {
        // Act & Assert
        Assert.Throws<GenealogyException>(() => new DateInterpreted(dateString));
    }

    [Theory]
    [InlineData("INT 1917 (3 years after marriage)")]
    [InlineData("(3 years after marriage)")]
    public void Constructor_ShouldInitializeDate_WhenDateInterpretedStringIsProvided(
      string dateString)
    {
        // Act
        DateInterpreted dateInterpreted = new(dateString);

        // Assert
        Assert.Equal(dateString, dateInterpreted.ToString());
    }

    [Fact]
    public void Constructor_ShouldTrimRemainder_BeforeParsingExactDate()
    {
        // Arrange
        // Double space after prefix and trailing spaces in remainder
        string input = "INT  1917  (3 years after marriage)";

        // Act
        DateInterpreted sut = new(input);

        // Assert
        // ToString should be normalized (single space after prefix and trimmed remainder)
        Assert.Equal("INT 1917 (3 years after marriage)", sut.ToString());
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForSameTypeAndDate()
    {
        // Arrange
        DateInterpreted a = new("INT 1917 (3 years after marriage)");
        DateInterpreted b = new("INT 1917 (3 years after marriage)");

        // Act & Assert
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentDateOrPhrase()
    {
        // Arrange
        var a = new DateInterpreted("INT 1917 (3 years after marriage)");
        var differentDate = new DateInterpreted("INT 1918 (3 years after marriage)");
        var differentPhrase = new DateInterpreted("INT 1917 (4 years after marriage)");

        // Act & Assert
        Assert.False(a.Equals(differentDate));
        Assert.False(a.Equals(differentPhrase));
    }

    [Theory]
    [InlineData("INT 1917 (3 years after marriage)", true)]
    [InlineData("INT (3 years after marriage)", false)]
    [InlineData("(3 years after marriage)", true)]
    [InlineData("3 years after marriage", false)]
    [InlineData("(3 years after marriage following the birth of their first child)", false)]
    [InlineData("(3 years after marriage", false)]
    [InlineData("", false)]
    public void TryParse_ShouldReturnExpectedResult(string input, bool expected)
    {
        bool result = DateInterpreted.TryParse(input, out DateInterpreted? date);
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
