using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Genealogy;

namespace FamilyShowLib.Tests;

public class DateWrapperTest
{
  [Fact]
  public void DateWrapper_DefaultConstructor_ShouldInitializeWithNullDate()
  {
    // Arrange & Act
    DateWrapper dateWrapper = new();

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Null(dateWrapper.Date);
    Assert.True(DateWrapper.IsNullOrEmpty(dateWrapper));
    Assert.False(dateWrapper.Date is IDateExact);
  }

  [Fact]
  public void DateWrapper_IDateConstructor_ShouldInitializeWithParsedDate_WhenDateParseIsProvided()
  {
    // Arrange
    _ = Date.TryParse("BET 1982 AND 1984", out IDate expectedDate);

    // Act
    DateWrapper dateWrapper = new(expectedDate);

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Equal(expectedDate, dateWrapper.Date);
    Assert.False(DateWrapper.IsNullOrEmpty(dateWrapper));
    Assert.False(dateWrapper.Date is IDateExact);
  }

  [Fact]
  public void DateWrapper_IDateConstructor_ShouldInitializeWithCreatedDate_WhenDateCreateIsProvided()
  {
    // Arrange
    IDate date = Date.Create(2023, 1, 1);

    // Act
    DateWrapper dateWrapper = new(date);

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Equal(date, dateWrapper.Date);
    Assert.False(DateWrapper.IsNullOrEmpty(dateWrapper));
    Assert.True(dateWrapper.Date is IDateExact);
  }

  [Fact]
  public void DateWrapper_IDateConstructor_ShouldInitializeWithNullDate_WhenDateIsNull()
  {
    // Arrange
    IDate? date = null;

    // Act
    DateWrapper dateWrapper = new(date);

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Null(dateWrapper.Date);
    Assert.True(DateWrapper.IsNullOrEmpty(dateWrapper));
    Assert.False(dateWrapper is IDateExact);
  }

  [Fact]
  public void DateWrapper_IDateConstructor_ShouldInitializeWithParsedDate_WhenDateStringIsProvided()
  {
    // Arrange
    string dateString = "1 JAN 2023";
    _ = Date.TryParse(dateString, out IDate expectedDate);

    // Act
    DateWrapper dateWrapper = new(dateString);

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.Equal(expectedDate, dateWrapper.Date);
    Assert.True(dateWrapper.Date is IDateExact);
  }

  [Theory]
  [InlineData("1946-07-06T00:00:00")]
  [InlineData("6 JUL 1946")]
  public void DateWrapper_StringConstructor_ShouldInitializeWithParsedDate(string date)
  {
    // Arrange

    // Act
    DateWrapper dateWrapper = new(date);

    // Assert
    Assert.NotNull(dateWrapper);
    Assert.False(DateWrapper.IsNullOrEmpty(dateWrapper));
    Assert.True(dateWrapper.Date is IDateExact);
  }

  [Theory, CombinatorialData]
  public void DateWrapper_StringConstructor_ShouldInitializeWithNullDate_WhenInvalidDateStringIsProvided(
    [CombinatorialValues("12", "", null)] string invalidDateString
    )
  {
    // Arrange & Act
    DateWrapper dateWrapper = new(invalidDateString);

    // Assert
    Assert.True(DateWrapper.IsNullOrEmpty(dateWrapper));
    Assert.False(dateWrapper.Date is IDateExact);
  }

  [Fact]
  public void DateWrapper_IsNullOrEmpty_ShouldReturnTrueForNull()
  {
    // Arrange

    // Act
    bool dateWrapper = DateWrapper.IsNullOrEmpty(null);

    // Assert
    Assert.True(dateWrapper);
  }

  [Fact]
  public void DateWrapper_IsNullOrEmpty_ShouldReturnTrue_WhenDateIsNull()
  {
    // Arrange
    DateWrapper dateWrapper = new();

    // Act
    bool result = DateWrapper.IsNullOrEmpty(dateWrapper);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void DateWrapper_IsNullOrEmpty_ShouldReturnFalse_WhenDateIsValid()
  {
    // Arrange
    DateWrapper dateWrapper = new(Date.Create(2023, 1, 1));

    // Act
    bool result = DateWrapper.IsNullOrEmpty(dateWrapper);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void DateWrapper_Equals_ShouldReturnTrue_WhenDatesAreEqual()
  {
    // Arrange
    IDate date = Date.Create(2023, 1, 1);
    DateWrapper dateWrapper1 = new(date);
    DateWrapper dateWrapper2 = new(date);

    // Act
    bool result = dateWrapper1.Equals(dateWrapper2);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void DateWrapper_Equals_ShouldReturnFalse_WhenDatesAreNotEqual()
  {
    // Arrange
    DateWrapper dateWrapper1 = new(Date.Create(2023, 1, 1));
    DateWrapper dateWrapper2 = new(Date.Create(2023, 1, 1));

    // Act
    bool result = dateWrapper1.Equals(dateWrapper2);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void DateWrapper_Equals_ShouldReturnFalse_WhenOtherIsNull()
  {
    // Arrange
    DateWrapper dateWrapper = new(Date.Create(2023, 1, 1));

    // Act
    bool result = dateWrapper.Equals(null);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void DateWrapper_EqualsObject_ShouldReturnTrue_WhenDatesAreEqual()
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
  public void DateWrapper_EqualsObject_ShouldReturnFalse_WhenDatesAreNotEqual()
  {
    // Arrange
    DateWrapper dateWrapper1 = new(Date.Create(2023, 1, 1));
    DateWrapper dateWrapper2 = new(Date.Create(2023, 1, 1));

    // Act
    bool result = dateWrapper1.Equals((object)dateWrapper2);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public void DateWrapper_EqualsObject_ShouldReturnFalse_WhenObjectIsNotDateWrapper()
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
  public void DateWrapper_GetHashCode_ShouldReturnSameHashCode_WhenDatesAreEqual()
  {
    // Arrange
    IDate date = Date.Create(2023, 1, 1);
    DateWrapper dateWrapper1 = new(date);
    DateWrapper dateWrapper2 = new(date);

    // Act
    int hashCode1 = dateWrapper1.GetHashCode();
    int hashCode2 = dateWrapper2.GetHashCode();

    // Assert
    Assert.Equal(hashCode1, hashCode2);
  }

  [Fact]
  public void DateWrapper_GetHashCode_ShouldReturnDifferentHashCode_WhenDatesAreNotEqual()
  {
    // Arrange
    DateWrapper dateWrapper1 = new(Date.Create(2023, 1, 1));
    DateWrapper dateWrapper2 = new(Date.Create(2023, 1, 2));

    // Act
    int hashCode1 = dateWrapper1.GetHashCode();
    int hashCode2 = dateWrapper2.GetHashCode();

    // Assert
    Assert.NotEqual(hashCode1, hashCode2);
  }
}
