namespace Genealogy.Tests;

public class DateTest
{
  public static readonly TheoryData<string> ValidGedcomDates =
  [
    "6 MAY 2001",
    "1977",
    "BET 1982 AND 1984",
    "EST 1752",
    "ABT JAN 1781",
    "AFT JAN 1781",
    "BEF JAN 1781",
    "FROM 1670 TO 1800",
    "FROM 1670 TO JULIAN 1800",
    "FROM JULIAN 1670 TO 1800",
  ];

  [Theory, MemberData(nameof(ValidGedcomDates))]
  public void ParseTest(string date)
  {
    // Arrange
    // Act
    IDate result = Date.Parse(date);
    // Assert
    Assert.Equal(date, result.ToGedcom());
  }

  public static readonly TheoryData<string> InvalidGedcomDates =
  [
    null,
    string.Empty,
    "123",
  ];

  [Theory, MemberData(nameof(InvalidGedcomDates))]
  public void ParseThrowsGenealogyException(string date)
  {
    // Arrange
    // Act & Assert
    Assert.Throws<GenealogyException>(() => Date.Parse(date));
  }
}
