using Genealogy.DateImplementation;

using Xunit;

namespace Genealogy.Tests.DateImplementation;

public class SimpleDateTests
{
  [Fact]
  public void Constructor_ShouldInitializeDate_WhenValidDateStringIsProvided()
  {
    // Arrange
    string dateString = "2023";

    // Act
    SimpleDate simpleDate = new(dateString);

    // Assert
    Assert.Equal(dateString, simpleDate.ToGedcom());
  }

  [Fact]
  public void Constructor_ShouldThrowException_WhenInvalidDateStringIsProvided()
  {
    // Arrange
    string invalidDateString = "123";

    // Act & Assert
    Assert.Throws<GenealogyException>(() => new SimpleDate(invalidDateString));
  }

  [Fact]
  public void ToGedcom_ShouldReturnCorrectGedcomString()
  {
    // Arrange
    string dateString = "2023";
    SimpleDate simpleDate = new(dateString);

    // Act
    string gedcomString = simpleDate.ToGedcom();

    // Assert
    Assert.Equal(dateString, gedcomString);
  }

  [Fact]
  public void Equals_ShouldReturnTrue_WhenDatesAreEqual()
  {
    // Arrange
    SimpleDate date1 = new("2023");
    SimpleDate date2 = new("2023");

    // Act
    bool result = date1.Equals(date2);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void Equals_ShouldReturnFalse_WhenDatesAreNotEqual()
  {
    // Arrange
    SimpleDate date1 = new("2023");
    SimpleDate date2 = new("2024");

    // Act
    bool result = date1.Equals(date2);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void Equals_ShouldReturnFalse_WhenOtherIsNull()
  {
    // Arrange
    SimpleDate date = new("2023");

    // Act
    bool result = date.Equals(null);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void Equals_ShouldReturnFalse_WhenOtherIsNotSimpleDate()
  {
    // Arrange
    SimpleDate date = new("2023");
    object obj = new();

    // Act
    bool result = date.Equals(obj);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void GetHashCode_ShouldReturnSameHashCode_WhenDatesAreEqual()
  {
    // Arrange
    SimpleDate date1 = new("2023");
    SimpleDate date2 = new("2023");

    // Act
    int hashCode1 = date1.GetHashCode();
    int hashCode2 = date2.GetHashCode();

    // Assert
    Assert.Equal(hashCode1, hashCode2);
  }

  [Fact]
  public void GetHashCode_ShouldReturnDifferentHashCode_WhenDatesAreNotEqual()
  {
    // Arrange
    SimpleDate date1 = new("2023");
    SimpleDate date2 = new("2024");

    // Act
    int hashCode1 = date1.GetHashCode();
    int hashCode2 = date2.GetHashCode();

    // Assert
    Assert.NotEqual(hashCode1, hashCode2);
  }
}
