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
        Person person = new("John", "Doe") { BirthDate = new DateWrapper(2010, 1, 1) };
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
        Person person = new("John", "Doe") { BirthDate = new DateWrapper(1990, 1, 1) };
        DiagramNode diagramNode = new() { _testing = true, Person = person };
        double newDisplayYear = 2000;
        string expectedBottomLabel = "John Doe\r1990 | 10";

        // Act
        diagramNode.DisplayYear = newDisplayYear;

        // Assert
        Assert.Equal(expectedBottomLabel, diagramNode.BottomLabel);
    }

    public static readonly TheoryData<Restriction, bool, int, int, int, int, int, int, string> DateInformationCases =
      new()
      {
      { Restriction.Private, true,     0, 0, 0,    0, 0, 0, string.Empty },
      { Restriction.None,    true,     0, 0, 0,    0, 0, 0, string.Empty },
      { Restriction.None,    false,    0, 0, 0,    0, 0, 0, string.Empty },
      { Restriction.None,    true,  1910, 3, 3,    0, 0, 0, "1910 | 114" },
      { Restriction.None,    false, 1910, 3, 3,    0, 0, 0, "1910 - ? | ?" },
      { Restriction.None,    false,    0, 0, 0, 2017, 2, 2, "? - 2017 | ?" },
      { Restriction.None,    false, 1910, 3, 3, 2017, 2, 2, "1910 - 2017 | 106" },
      { Restriction.None,    false, 1920, 3, 3, 2025, 2, 2, "1920 - 2025 | 104" },
      };

    [StaTheory, MemberData(nameof(DateInformationCases))]
    public void DateInformation_ShouldCalculateCorrectly(
      Restriction restriction,
      bool living,
      int birthYear,
      int birthMonth,
      int birthDay,
      int deathYear,
      int deathMonth,
      int deathDay,
      string expected)
    {
        // Arrange
        DateWrapper? birthDate = (birthYear == 0 ? null : new DateWrapper(birthYear, birthMonth, birthDay));
        DateWrapper? deathDate = (deathYear == 0 ? null : new DateWrapper(deathYear, deathMonth, deathDay));
        Person person = new("John", "Doe")
        {
            Restriction = restriction,
            IsLiving = living,
            BirthDate = birthDate,
            DeathDate = deathDate
        };
        var diagramNode = new DiagramNode { _testing = true, _displayYear = DateTime.Now.Year - 1, Person = person };

        // Act
        string result = diagramNode.DateInformation;

        // Assert
        Assert.Equal(expected, result);
    }
}
