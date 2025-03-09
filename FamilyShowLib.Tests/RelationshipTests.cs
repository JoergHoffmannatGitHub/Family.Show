namespace FamilyShowLib.Tests;

public class SpouseRelationshipTests
{
  [Fact]
  public void MarriageDate_SetAndGet_ReturnsCorrectValue()
  {
    // Arrange
    var spouseRelationship = new SpouseRelationship();
    DateTime? expectedDate = new DateTime(2020, 5, 15);

    // Act
    spouseRelationship.MarriageDate = expectedDate;
    DateTime? actualDate = spouseRelationship.MarriageDate;

    // Assert
    Assert.Equal(expectedDate, actualDate);
  }

  [Fact]
  public void MarriageDate_SetNull_ReturnsNull()
  {
    // Arrange
    var spouseRelationship = new SpouseRelationship
    {
      // Act
      MarriageDate = null
    };

    DateTime? actualDate = spouseRelationship.MarriageDate;

    // Assert
    Assert.Null(actualDate);
  }

  [Fact]
  public void MarriageDate_SetAndGet_ReturnsCorrectValueAfterMultipleAssignments()
  {
    // Arrange
    var spouseRelationship = new SpouseRelationship();
    DateTime? firstDate = new DateTime(2018, 3, 10);
    DateTime? secondDate = new DateTime(2021, 7, 20);

    // Act
    spouseRelationship.MarriageDate = firstDate;
    spouseRelationship.MarriageDate = secondDate;
    DateTime? actualDate = spouseRelationship.MarriageDate;

    // Assert
    Assert.Equal(secondDate, actualDate);
  }

  [Fact]
  public void MarriageDateDescriptor_SetAndGet_ReturnsCorrectValue()
  {
    // Arrange
    var spouseRelationship = new SpouseRelationship();
    string expectedDescriptor = "Spring 2020";

    // Act
    spouseRelationship.MarriageDateDescriptor = expectedDescriptor;
    string actualDescriptor = spouseRelationship.MarriageDateDescriptor;

    // Assert
    Assert.Equal(expectedDescriptor, actualDescriptor);
  }

  [Fact]
  public void MarriageDateDescriptor_SetNull_ReturnsNull()
  {
    // Arrange
    var spouseRelationship = new SpouseRelationship
    {
      // Act
      MarriageDateDescriptor = null
    };
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    string actualDescriptor = spouseRelationship.MarriageDateDescriptor;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    // Assert
    Assert.Null(actualDescriptor);
  }

  [Fact]
  public void MarriageDateDescriptor_SetAndGet_ReturnsCorrectValueAfterMultipleAssignments()
  {
    // Arrange
    var spouseRelationship = new SpouseRelationship();
    string firstDescriptor = "Summer 2018";
    string secondDescriptor = "Winter 2021";

    // Act
    spouseRelationship.MarriageDateDescriptor = firstDescriptor;
    spouseRelationship.MarriageDateDescriptor = secondDescriptor;
    string actualDescriptor = spouseRelationship.MarriageDateDescriptor;

    // Assert
    Assert.Equal(secondDescriptor, actualDescriptor);
  }

  [Fact]
  public void DivorceDate_SetAndGet_ReturnsCorrectValue()
  {
    // Arrange
    var spouseRelationship = new SpouseRelationship();
    DateTime? expectedDate = new DateTime(2022, 8, 25);

    // Act
    spouseRelationship.DivorceDate = expectedDate;
    DateTime? actualDate = spouseRelationship.DivorceDate;

    // Assert
    Assert.Equal(expectedDate, actualDate);
  }

  [Fact]
  public void DivorceDate_SetNull_ReturnsNull()
  {
    // Arrange
    var spouseRelationship = new SpouseRelationship
    {
      // Act
      DivorceDate = null
    };
    DateTime? actualDate = spouseRelationship.DivorceDate;

    // Assert
    Assert.Null(actualDate);
  }

  [Fact]
  public void DivorceDate_SetAndGet_ReturnsCorrectValueAfterMultipleAssignments()
  {
    // Arrange
    var spouseRelationship = new SpouseRelationship();
    DateTime? firstDate = new DateTime(2019, 4, 10);
    DateTime? secondDate = new DateTime(2023, 11, 15);

    // Act
    spouseRelationship.DivorceDate = firstDate;
    spouseRelationship.DivorceDate = secondDate;
    DateTime? actualDate = spouseRelationship.DivorceDate;

    // Assert
    Assert.Equal(secondDate, actualDate);
  }

  [Fact]
  public void DivorceDateDescriptor_SetAndGet_ReturnsCorrectValue()
  {
    // Arrange
    var spouseRelationship = new SpouseRelationship();
    string expectedDescriptor = "Fall 2022";

    // Act
    spouseRelationship.DivorceDateDescriptor = expectedDescriptor;
    string actualDescriptor = spouseRelationship.DivorceDateDescriptor;

    // Assert
    Assert.Equal(expectedDescriptor, actualDescriptor);
  }

  [Fact]
  public void DivorceDateDescriptor_SetNull_ReturnsNull()
  {
    // Arrange
    var spouseRelationship = new SpouseRelationship
    {
      // Act
      DivorceDateDescriptor = null
    };
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    string actualDescriptor = spouseRelationship.DivorceDateDescriptor;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    // Assert
    Assert.Null(actualDescriptor);
  }

  [Fact]
  public void DivorceDateDescriptor_SetAndGet_ReturnsCorrectValueAfterMultipleAssignments()
  {
    // Arrange
    var spouseRelationship = new SpouseRelationship();
    string firstDescriptor = "Spring 2019";
    string secondDescriptor = "Winter 2023";

    // Act
    spouseRelationship.DivorceDateDescriptor = firstDescriptor;
    spouseRelationship.DivorceDateDescriptor = secondDescriptor;
    string actualDescriptor = spouseRelationship.DivorceDateDescriptor;

    // Assert
    Assert.Equal(secondDescriptor, actualDescriptor);
  }
}
