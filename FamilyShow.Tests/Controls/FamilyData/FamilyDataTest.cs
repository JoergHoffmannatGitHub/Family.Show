using FamilyShowLib;

//[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace FamilyShow.Tests.Controls.FamilyData;

[CollectionDefinition("SequentialTest", DisableParallelization = true)]

[Collection("SequentialTest")]
public class FamilyDataTest
{
  [StaFact]
  public void UpdateFilterCommand_ShouldUpdateBirthdateFilter()
  {
    // Arrange
    FamilyShow.FamilyData familyData = new();
    DateTime date = new(1980, 1, 1);

    // Act
    familyData.UpdateFilterCommand(date);

    // Assert
    Assert.Equal(date.ToShortString(), familyData._birthdateFilter);
  }

  [StaFact]
  public void UpdateFilterCommand_ShouldNotUpdateBirthdateFilter()
  {
    // Arrange
    FamilyShow.FamilyData familyData = new()
    {
      _birthdateFilter = "date"
    };

    // Act
    familyData.UpdateFilterCommand(null);

    // Assert
    Assert.Equal("date", familyData._birthdateFilter);
    familyData._birthdateFilter = null;
  }
}
