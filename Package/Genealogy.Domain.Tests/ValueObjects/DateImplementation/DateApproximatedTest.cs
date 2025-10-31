using Genealogy.Domain.ValueObjects.DateImplementation;

namespace Genealogy.Domain.Tests.ValueObjects.DateImplementation;

public class DateApproximatedTest
{
  [Fact]
  public void Constructor_ShouldThrowArgumentNullException_WhenNullProvided()
  {
    // Act & Assert
    Assert.Throws<ArgumentNullException>(() => new DateApproximated(null));
  }

  [Fact]
  public void Constructor_ShouldThrowException_WhenInvalidDatePrefixIsProvided()
  {
    // Act & Assert
    Assert.Throws<ArgumentException>(() => new DateApproximated("BEF 22 Jul 2023"));
  }

  [Fact]
  public void Constructor_ShouldThrowException_WhenInvalidDateStringIsProvided()
  {
    // Act & Assert
    Assert.Throws<GenealogyException>(() => new DateApproximated("CAL 17"));
  }

  [Theory]
  [InlineData("ABT 22 Jul 2023")]
  [InlineData("CAL Jul 2023")]
  [InlineData("EST 2023")]
  public void Constructor_ShouldInitializeDate_WhenDateApproximatedStringIsProvided(
    string dateString)
  {
    // Act
    DateApproximated dateApproximated = new(dateString);

    // Assert
    Assert.Equal(dateString, dateApproximated.ToString());
  }

  [Fact]
  public void Constructor_ShouldTrimRemainder_BeforeParsingExactDate()
  {
    // Arrange
    // Double space after prefix and trailing spaces in remainder
    string input = "ABT  2023 ";

    // Act
    DateApproximated sut = new(input);

    // Assert
    // ToString should be normalized (single space after prefix and trimmed remainder)
    Assert.Equal("ABT 2023", sut.ToString());
  }

  [Fact]
  public void Equals_ShouldReturnTrue_ForSameTypeAndDate()
  {
    // Arrange
    var a = new DateApproximated("ABT 22 Jul 2023");
    var b = new DateApproximated("ABT 22 Jul 2023");

    // Act & Assert
    Assert.True(a.Equals(b));
  }

  [Fact]
  public void Equals_ShouldReturnFalse_ForDifferentTypeOrDate()
  {
    // Arrange
    var a = new DateApproximated("ABT 22 Jul 2023");
    var differentType = new DateApproximated("CAL Jul 2023");
    var differentDate = new DateApproximated("ABT 23 Jul 2023");

    // Act & Assert
    Assert.False(a.Equals(differentType));
    Assert.False(a.Equals(differentDate));
  }

  [Fact]
  public void Constructor_ShouldThrowGenealogyException_WhenDateStringTooShort()
  {
    // Arrange
    string tooShort = "ABT";

    // Act & Assert
    Assert.Throws<GenealogyException>(() => new DateApproximated(tooShort));
  }

  [Theory]
  [InlineData("ABT 22 Jul 2023", true)]
  [InlineData("CAL Jul 2023", true)]
  [InlineData("EST 2023", true)]
  [InlineData("XYZ2023", false)]
  [InlineData("ABT", false)]
  [InlineData("", false)]
  public void TryParse_ShouldReturnExpectedResult(string input, bool expected)
  {
    bool result = DateApproximated.TryParse(input, out DateApproximated? date);
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
