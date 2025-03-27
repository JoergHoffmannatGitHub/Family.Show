using System.Collections.Specialized;

using FamilyShowLib;

namespace FamilyShow.Tests.Controls.Diagram;

public class DiagramNodeTest
{
  [StaFact]
  public void DisplayYear_ShouldUpdateDisplayYearField()
  {
    // Arrange
    DiagramNode diagramNode = new();
    double newDisplayYear = 2000;

    // Act
    diagramNode.DisplayYear = newDisplayYear;

    // Assert
    Assert.Equal(newDisplayYear, diagramNode.DisplayYear);
  }

  [StaFact]
  public void DisplayYear_ShouldUpdateIsFilteredBasedOnBirthDate()
  {
    // Arrange
    Person person = new("John", "Doe") { BirthDate = new DateTime(2010, 1, 1) };
    var diagramNode = new DiagramNode { _testing = true, Person = person };
    double newDisplayYear = 2000;

    // Act
    diagramNode.DisplayYear = newDisplayYear;

    // Assert
    Assert.True(diagramNode.IsFiltered);
  }

  [StaFact]
  public void DisplayYear_ShouldUpdateBottomLabel()
  {
    // Arrange
    Person person = new("John", "Doe") { BirthDate = new DateTime(1990, 1, 1) };
    DiagramNode diagramNode = new() { _testing = true, Person = person };
    double newDisplayYear = 2000;
    string expectedBottomLabel = "John Doe\r1990 | 10";

    // Act
    diagramNode.DisplayYear = newDisplayYear;

    // Assert
    Assert.Equal(expectedBottomLabel, diagramNode.BottomLabel);
  }

  public static readonly TheoryData<Restriction, bool, DateTime?, DateTime?, string> DateInformationCases =
    new()
    {
      { Restriction.Private, true, null, null, string.Empty },
      { Restriction.None, true, null, null, string.Empty },
      { Restriction.None, false, null, null, string.Empty },
      { Restriction.None, true, new DateTime(1910, 3, 3), null, "1910 | 114" },
      { Restriction.None, false, new DateTime(1910, 3, 3), null, "1910 - ? | ?" },
      { Restriction.None, false, null, new DateTime(2017, 2, 2), "? - 2017 | ?" },
      { Restriction.None, false, new DateTime(1910, 3, 3), new DateTime(2017, 2, 2), "1910 - 2017 | 106" },
      { Restriction.None, false, new DateTime(1920, 3, 3), new DateTime(2025, 2, 2), "1920 - 2025 | 103" },
    };

  [StaTheory, MemberData(nameof(DateInformationCases))]
  public void DateInformation_ShouldCalculateCorrectly(Restriction restriction, bool living, DateTime? birthDate, DateTime? deathDate, string expected)
  {
    // Arrange
    Person person = new("John", "Doe") { Restriction = restriction, IsLiving = living, BirthDate = birthDate, DeathDate = deathDate };
    var diagramNode = new DiagramNode { _testing = true, _displayYear = 2024, Person = person };

    // Act
    string result = diagramNode.DateInformation;

    // Assert
    Assert.Equal(expected, result);
  }
}
