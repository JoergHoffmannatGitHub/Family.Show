using Genealogy.Domain.ValueObjects.DateImplementation;

namespace Genealogy.Domain.Tests.ValueObjects.DateImplementation;

public class DatePeriodTest
{
  [Fact]
  public void Constructor_ShouldThrowException_WhenNullStringIsProvided()
  {
    // Act & Assert
    Assert.Throws<ArgumentNullException>(() => new DatePeriod(null));
  }

  [Theory]
  [InlineData("BEF 22 Jul 2023")]
  [InlineData("TO 17")]
  public void Constructor_ShouldThrowException_WhenInvalidDateStringIsProvided(string invalidDateString)
  {
    // Act & Assert
    Assert.Throws<GenealogyException>(() => new DatePeriod(invalidDateString));
  }

  [Theory]
  [InlineData("FROM 1904 to 1915")]
  [InlineData("FROM 1904")]
  [InlineData("To 1915")]
  public void Constructor_ShouldInitializeDate_WhenDateApproximatedStringIsProvided(
    string dateString)
  {
    // Act
    DatePeriod datePeriod = new(dateString);

    // Assert
    Assert.Equal(dateString.ToUpper(), datePeriod.ToString());
  }

  [Theory]
  [InlineData("FROM 1904 TO 1915", true)]
  [InlineData("FROM 1904", true)]
  [InlineData("TO 1915", true)]
  [InlineData("TO2023", false)]
  [InlineData("FROM", false)]
  [InlineData("", false)]
  public void TryParse_ShouldReturnExpectedResult(string input, bool expected)
  {
    bool result = DatePeriod.TryParse(input, out DatePeriod? date);
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
