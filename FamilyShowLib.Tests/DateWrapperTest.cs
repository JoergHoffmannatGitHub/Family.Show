using Genealogy;

namespace FamilyShowLib.Tests;

public class DateWrapperTest
{
  [Fact]
  public void DefaultConstructor_ShouldInitializeWithNullDate()
  {
    // Arrange & Act
    DateWrapper dateWrapper = new();

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Null(dateWrapper.Date);
    Assert.True(DateWrapper.IsNullOrEmpty(dateWrapper));
  }

  [Fact]
  public void Constructor_ShouldInitializeWithParsedDate_WhenValidDateStringIsProvided()
  {
    // Arrange
    _ = Date.TryParse("BET 1982 AND 1984", out IDate expectedDate);

    // Act
    DateWrapper dateWrapper = new(expectedDate);

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Equal(expectedDate, dateWrapper.Date);
    Assert.False(DateWrapper.IsNullOrEmpty(dateWrapper));
  }

  [Fact]
  public void Constructor_ShouldInitializeWithCreatedDate_WhenYearMonthDayAreProvided()
  {
    // Arrange
    int year = 2023;
    int month = 7;
    int day = 22;
    IDate expectedDate = Date.Create(year, month, day);

    // Act
    DateWrapper dateWrapper = new(year, month, day);

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Equal(expectedDate, dateWrapper.Date);
    Assert.False(DateWrapper.IsNullOrEmpty(dateWrapper));
  }

  [Theory, CombinatorialData]
  public void Constructor_ShouldInitializeWithNullDate_WhenInvalidDateStringIsProvided(
    [CombinatorialValues("12", "", "InvalidDate", null)] string invalidDateString
    )
  {
    // Arrange & Act
    DateWrapper dateWrapper = new(invalidDateString);

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.True(DateWrapper.IsNullOrEmpty(dateWrapper));
    //Assert.False(dateWrapper.Date is IDateExact);
    //Assert.False(DateWrapper.IsDateExact(dateWrapper, out IDateExact exact));
    //Assert.Null(exact);
  }

  [Fact]
  public void ToGedcom_ShouldReturnCorrectGedcomString_WhenDateIsValid()
  {
    // Arrange
    DateWrapper dateWrapper = new("22 JUL 2023");

    // Act
    string gedcomString = dateWrapper.ToGedcom();

    // Assert
    Assert.Equal("22 JUL 2023", gedcomString);
  }

  [Fact]
  public void ToGedcom_ShouldReturnEmptyString_WhenDateIsNull()
  {
    // Arrange
    DateWrapper dateWrapper = new();

    // Act
    string gedcomString = dateWrapper.ToGedcom();

    // Assert
    Assert.Equal(string.Empty, gedcomString);
  }

  [Fact]
  public void ToShortString_ShouldReturnCorrectShortDateString_WhenDateIsExact()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      DateWrapper dateWrapper = new("22 JUL 2023");

      // Act
      string shortDateString = dateWrapper.ToShortString();

      // Assert
      Assert.Equal("7/22/2023", shortDateString);
    }
  }

  [Fact]
  public void ToShortString_ShouldReturnEmptyString_WhenDateIsNotExact()
  {
    // Arrange
    DateWrapper dateWrapper = new("AFT JUL 2023");

    // Act
    string shortDateString = dateWrapper.ToShortString();

    // Assert
    Assert.Equal(string.Empty, shortDateString);
  }

  [Fact]
  public void Format_ShouldReturnCorrectFormattedString_WhenDateIsExact()
  {
    // Arrange
    DateWrapper dateWrapper = new("22 JUL 2023");

    // Act
    string formattedString = dateWrapper.Format();

    // Assert
    Assert.Equal("22/7/2023", formattedString);
  }

  [Fact]
  public void Format_ShouldReturnEmptyString_WhenDateIsNotExact()
  {
    // Arrange
    DateWrapper dateWrapper = new("BEF JUL 2023");

    // Act
    string formattedString = dateWrapper.Format();

    // Assert
    Assert.Equal(string.Empty, formattedString);
  }

  [Fact]
  public void Equals_ShouldReturnTrue_WhenDatesAreEqual()
  {
    // Arrange
    DateWrapper dateWrapper1 = new("22 JUL 2023");
    DateWrapper dateWrapper2 = new("22 JUL 2023");

    // Act
    bool result = dateWrapper1.Equals(dateWrapper2);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void Equals_ShouldReturnFalse_WhenDatesAreNotEqual()
  {
    // Arrange
    DateWrapper dateWrapper1 = new("22 JUL 2023");
    DateWrapper dateWrapper2 = new("23 JUL 2023");

    // Act
    bool result = dateWrapper1.Equals(dateWrapper2);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void Equals_ShouldReturnFalse_WhenOtherIsNull()
  {
    // Arrange
    DateWrapper dateWrapper = new("22 JUL 2023");

    // Act
    bool result = dateWrapper.Equals(null);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void GetHashCode_ShouldReturnSameHashCode_WhenDatesAreEqual()
  {
    // Arrange
    DateWrapper dateWrapper1 = new("22 JUL 2023");
    DateWrapper dateWrapper2 = new("22 JUL 2023");

    // Act
    int hashCode1 = dateWrapper1.GetHashCode();
    int hashCode2 = dateWrapper2.GetHashCode();

    // Assert
    Assert.Equal(hashCode1, hashCode2);
  }

  [Fact]
  public void GetHashCode_ShouldReturnDifferentHashCode_WhenDatesAreNotEqual()
  {
    // Arrange
    DateWrapper dateWrapper1 = new("22 JUL 2023");
    DateWrapper dateWrapper2 = new("23 JUL 2023");

    // Act
    int hashCode1 = dateWrapper1.GetHashCode();
    int hashCode2 = dateWrapper2.GetHashCode();

    // Assert
    Assert.NotEqual(hashCode1, hashCode2);
  }

  [Fact]
  public void IsNullOrEmpty_ShouldReturnTrueForNull()
  {
    // Act
    bool result = DateWrapper.IsNullOrEmpty(null);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void IsNullOrEmpty_ShouldReturnTrue_WhenDateIsNull()
  {
    // Arrange
    DateWrapper dateWrapper = new();

    // Act
    bool result = DateWrapper.IsNullOrEmpty(dateWrapper);

    // Assert
    Assert.True(result);
    Assert.False(DateWrapper.IsDateExact(dateWrapper, out IDateExact exact));
    Assert.Null(exact);
  }

  [Fact]
  public void IsNullOrEmpty_ShouldReturnFalse_WhenDateIsValid()
  {
    // Arrange
    DateWrapper dateWrapper = new(Date.Create(2023, 1, 1));

    // Act
    bool result = DateWrapper.IsNullOrEmpty(dateWrapper);

    // Assert
    Assert.False(result);
    Assert.True(DateWrapper.IsDateExact(dateWrapper, out IDateExact exact));
    Assert.NotNull(exact);
  }

  [Fact]
  public void EqualsObject_ShouldReturnTrue_WhenDatesAreEqual()
  {
    // Arrange
    IDate date = Date.Create(2023, 1, 1);
    DateWrapper dateWrapper1 = new(date);
    DateWrapper dateWrapper2 = new(date);

    // Act
    bool result = dateWrapper1.Equals((object)dateWrapper2);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void EqualsObject_ShouldReturnFalse_WhenDatesAreNotEqual()
  {
    // Arrange
    DateWrapper dateWrapper1 = new(Date.Create(2023, 1, 1));
    DateWrapper dateWrapper2 = new(Date.Create(2023, 1, 1));

    // Act
    bool result = dateWrapper1.Equals((object)dateWrapper2);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void EqualsObject_ShouldReturnFalse_WhenObjectIsNotDateWrapper()
  {
    // Arrange
    DateWrapper dateWrapper = new(Date.Create(2023, 1, 1));
    object obj = new();

    // Act
    bool result = dateWrapper.Equals(obj);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void IsDateExact_ShouldReturnTrue_WhenDateIsExact()
  {
    // Arrange
    DateWrapper dateWrapper = new("22 JUL 2023");

    // Act
    bool result = DateWrapper.IsDateExact(dateWrapper, out IDateExact dateExact);

    // Assert
    Assert.True(result);
    Assert.NotNull(dateExact);
  }

  [Fact]
  public void IsDateExact_ShouldReturnFalse_WhenDateIsNotExact()
  {
    // Arrange
    DateWrapper dateWrapper = new("AFT JUL 2023");

    // Act
    bool result = DateWrapper.IsDateExact(dateWrapper, out IDateExact dateExact);

    // Assert
    Assert.False(result);
    Assert.Null(dateExact);
  }

  [Fact]
  public void IsDateExact_ShouldReturnFalse_WhenDateIsNull()
  {
    // Arrange
    DateWrapper dateWrapper = new();

    // Act
    bool result = DateWrapper.IsDateExact(dateWrapper, out IDateExact dateExact);

    // Assert
    Assert.False(result);
    Assert.Null(dateExact);
  }
}
