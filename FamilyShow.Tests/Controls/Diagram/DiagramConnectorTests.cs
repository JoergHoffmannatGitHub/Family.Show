using System.Windows;

using FamilyShowLib;

namespace FamilyShow.Tests.Controls.Diagram;

public class DiagramConnectorTests
{
  [Fact]
  public void MarriedDate_ShouldReturnNull_WhenNotOverridden()
  {
    // Arrange
    TestDiagramConnector connector = new();

    // Act
    DateTime? result = connector.MarriedDate;

    // Assert
    Assert.Null(result);
  }

  [Fact]
  public void PreviousMarriedDate_ShouldReturnNull_WhenNotOverridden()
  {
    // Arrange
    TestDiagramConnector connector = new();

    // Act
    DateTime? result = connector.PreviousMarriedDate;

    // Assert
    Assert.Null(result);
  }

  [StaFact]
  public void MarriedDate_ShouldReturnNull_WhenMarriedWithoutDate()
  {
    // Arrange
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    Marry(person, spouse, null);
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    MarriedDiagramConnector connector = new(true, startNode, endNode, new DpiScale(1, 1));

    // Act
    DateTime? result = connector.MarriedDate;

    // Assert
    Assert.Null(result);
  }

  [StaFact]
  public void MarriedDate_ShouldReturnMarriageDate_WhenMarried()
  {
    // Arrange
    DateTime expectedResult = new(2000, 1, 1);
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    Marry(person, spouse, expectedResult);
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    MarriedDiagramConnector connector = new(true, startNode, endNode, new DpiScale(1, 1));

    // Act
    DateTime? result = connector.MarriedDate;

    // Assert
    Assert.Equal(expectedResult, result);
  }

  [StaFact]
  public void MarriedDate_ShouldReturnNull_WhenRelationshipIsMissed()
  {
    // Arrange
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    MarriedDiagramConnector connector = new(true, startNode, endNode, new DpiScale(1, 1));

    // Act
    DateTime? result = connector.MarriedDate;

    // Assert
    Assert.Null(result);
  }

  [StaFact]
  public void MarriedDate_ShouldReturnNull_WhenDivorced()
  {
    // Arrange
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    Marry(person, spouse, new(2000, 1, 1));
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    MarriedDiagramConnector connector = new(false, startNode, endNode, new DpiScale(1, 1));

    // Act
    DateTime? result = connector.MarriedDate;

    // Assert
    Assert.Null(result);
  }

  [StaFact]
  public void PreviousMarriedDate_ShouldReturnNull_WhenDivorcedWithoutDate()
  {
    // Arrange
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    Divorce(person, spouse, null);
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    MarriedDiagramConnector connector = new(false, startNode, endNode, new DpiScale(1, 1));

    // Act
    DateTime? result = connector.PreviousMarriedDate;

    // Assert
    Assert.Null(result);
  }

  [StaFact]
  public void PreviousMarriedDate_ShouldReturnMarriageDate_WhenDivorced()
  {
    // Arrange
    DateTime expectedResult = new(2000, 1, 1);
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    Divorce(person, spouse, expectedResult);
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    MarriedDiagramConnector connector = new(false, startNode, endNode, new DpiScale(1, 1));

    // Act
    DateTime? result = connector.PreviousMarriedDate;

    // Assert
    Assert.Equal(expectedResult, result);
  }

  [StaFact]
  public void PreviousMarriedDate_ShouldReturnNull_WhenRelationshipIsMissed()
  {
    // Arrange
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    MarriedDiagramConnector connector = new(false, startNode, endNode, new DpiScale(1, 1));

    // Act
    DateTime? result = connector.PreviousMarriedDate;

    // Assert
    Assert.Null(result);
  }

  [StaFact]
  public void PreviousMarriedDate_ShouldReturnNull_WhenMarried()
  {
    // Arrange
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    Divorce(person, spouse, new(2000, 1, 1));
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    MarriedDiagramConnector connector = new(true, startNode, endNode, new DpiScale(1, 1));

    // Act
    DateTime? result = connector.PreviousMarriedDate;

    // Assert
    Assert.Null(result);
  }

  [StaFact]
  public void NewFilteredState_ShouldReturnTrue_WhenStartNodeIsFiltered()
  {
    // Arrange
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    Marry(person, spouse, new DateTime(2000, 1, 1));
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    startNode.Node.IsFiltered = true;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    TestMarriedDiagramConnector connector = new(true, startNode, endNode, new DpiScale(1, 1));

    // Act
    bool result = connector.TestNewFilteredState;

    // Assert
    Assert.True(result);
  }

  [StaFact]
  public void NewFilteredState_ShouldReturnTrue_WhenEndNodeIsFiltered()
  {
    // Arrange
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    Marry(person, spouse, new DateTime(2000, 1, 1));
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    endNode.Node.IsFiltered = true;
    TestMarriedDiagramConnector connector = new(true, startNode, endNode, new DpiScale(1, 1));

    // Act
    bool result = connector.TestNewFilteredState;

    // Assert
    Assert.True(result);
  }

  [StaFact]
  public void NewFilteredState_ShouldReturnTrue_WhenMarriageDateIsAfterDisplayYear()
  {
    // Arrange
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    Marry(person, spouse, new DateTime(2000, 1, 1));
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    startNode.Node.DisplayYear = 1999;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    TestMarriedDiagramConnector connector = new(true, startNode, endNode, new DpiScale(1, 1));

    // Act
    bool result = connector.TestNewFilteredState;

    // Assert
    Assert.True(result);
  }

  [StaFact]
  public void NewFilteredState_ShouldReturnTrue_WhenDivorceDateIsAfterDisplayYear()
  {
    // Arrange
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    Divorce(person, spouse, new DateTime(2000, 1, 1));
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    startNode.Node.DisplayYear = 1999;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    TestMarriedDiagramConnector connector = new(false, startNode, endNode, new DpiScale(1, 1));

    // Act
    bool result = connector.TestNewFilteredState;

    // Assert
    Assert.True(result);
  }

  [StaFact]
  public void NewFilteredState_ShouldReturnFalse_WhenNotFiltered()
  {
    // Arrange
    Person person = new("John", "Doe");
    Person spouse = new("Jane", "Doe");
    Marry(person, spouse, new DateTime(2000, 1, 1));
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    startNode.Node.Person = person;
    startNode.Node.DisplayYear = 2001;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    endNode.Node.Person = spouse;
    TestMarriedDiagramConnector connector = new(true, startNode, endNode, new DpiScale(1, 1));

    // Act
    bool result = connector.TestNewFilteredState;

    // Assert
    Assert.False(result);
  }

  [StaFact]
  public void MarriageDate_ShouldReturnFormattedMarriageDate_WhenMarriageDateIsNotNull()
  {
    // Arrange
    SpouseRelationship relationship = new()
    {
      MarriageDate = new DateTime(2000, 1, 1),
      MarriageDateDescriptor = "Married: "
    };
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    TestMarriedDiagramConnector connector = new(true, startNode, endNode, new DpiScale(1, 1));

    // Act
    string result = connector.TestMarriageDate(relationship);

    // Assert
    Assert.Equal("Married: 2000", result);
  }

  [StaFact]
  public void MarriageDate_ShouldReturnEmptyString_WhenMarriageDateIsNull()
  {
    // Arrange
    SpouseRelationship relationship = new()
    {
      MarriageDate = null,
      MarriageDateDescriptor = "Married: "
    };
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    TestMarriedDiagramConnector connector = new(true, startNode, endNode, new DpiScale(1, 1));

    // Act
    string result = connector.TestMarriageDate(relationship);

    // Assert
    Assert.Equal(string.Empty, result);
  }

  [StaFact]
  public void MarriageDate_ShouldReturnEmptyString_WhenShowDateIsFalse()
  {
    // Arrange
    SpouseRelationship relationship = new()
    {
      MarriageDate = new DateTime(2000, 1, 1),
      MarriageDateDescriptor = "Married: "
    };
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    TestMarriedDiagramConnector connector = new(true, startNode, endNode, new DpiScale(1, 1))
    {
      ShowDate = false
    };

    // Act
    string result = connector.TestMarriageDate(relationship);

    // Assert
    Assert.Equal(string.Empty, result);
  }

  [StaFact]
  public void DivorceDate_ShouldReturnFormattedDivorceDate_WhenDivorceDateIsNotNull()
  {
    // Arrange
    SpouseRelationship relationship = new()
    {
      DivorceDate = new DateTime(2000, 1, 1),
      DivorceDateDescriptor = "Divorced: "
    };
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    TestMarriedDiagramConnector connector = new(false, startNode, endNode, new DpiScale(1, 1));

    // Act
    string result = connector.TestDivorceDate(relationship);

    // Assert
    Assert.Equal("Divorced: 2000", result);
  }

  [StaFact]
  public void DivorceDate_ShouldReturnEmptyString_WhenDivorceDateIsNull()
  {
    // Arrange
    SpouseRelationship relationship = new()
    {
      DivorceDate = null,
      DivorceDateDescriptor = "Divorced: "
    };
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    TestMarriedDiagramConnector connector = new(false, startNode, endNode, new DpiScale(1, 1));

    // Act
    string result = connector.TestDivorceDate(relationship);

    // Assert
    Assert.Equal(string.Empty, result);
  }

  [StaFact]
  public void DivorceDate_ShouldReturnEmptyString_WhenShowDateIsFalse()
  {
    // Arrange
    SpouseRelationship relationship = new()
    {
      DivorceDate = new DateTime(2000, 1, 1),
      DivorceDateDescriptor = "Divorced: "
    };
    DiagramConnectorNode startNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    startNode.Node._testing = true;
    DiagramConnectorNode endNode = new(new DiagramNode(), new DiagramGroup(), new DiagramRow());
    endNode.Node._testing = true;
    TestMarriedDiagramConnector connector = new(false, startNode, endNode, new DpiScale(1, 1))
    {
      ShowDate = false
    };

    // Act
    string result = connector.TestDivorceDate(relationship);

    // Assert
    Assert.Equal(string.Empty, result);
  }

  private static void Marry(Person husband, Person wife, DateTime? date)
  {
    SpouseRelationship relationship = new(husband, SpouseModifier.Current)
    {
      MarriageCitation = "Citation1",
      MarriageSource = "Source1",
      MarriagePlace = "Place1",
      MarriageDate = date
    };

    wife.Relationships.Add(relationship);
    relationship = new(wife, SpouseModifier.Current)
    {
      MarriageCitation = "Citation1",
      MarriageSource = "Source1",
      MarriagePlace = "Place1",
      MarriageDate = date
    };
    husband.Relationships.Add(relationship);
  }

  private static void Divorce(Person husband, Person wife, DateTime? date)
  {
    SpouseRelationship formerRelationship = new(husband, SpouseModifier.Former)
    {
      DivorceCitation = "Citation2",
      DivorceSource = "Source2",
      DivorceDate = date
    };
    wife.Relationships.Add(formerRelationship);
    formerRelationship = new(wife, SpouseModifier.Former)
    {
      DivorceCitation = "Citation2",
      DivorceSource = "Source2",
      DivorceDate = date
    };
    husband.Relationships.Add(formerRelationship);
  }

  // Helper class to test the abstract DiagramConnector class
  private class TestDiagramConnector : DiagramConnector
  {
    public TestDiagramConnector() : base(null, null) { }
  }

  // Helper class to test the protected NewFilteredState property
  private class TestMarriedDiagramConnector : MarriedDiagramConnector
  {
    public TestMarriedDiagramConnector(bool married, DiagramConnectorNode startNode, DiagramConnectorNode endNode, DpiScale dpiScale)
        : base(married, startNode, endNode, dpiScale) { }

    public bool TestNewFilteredState => NewFilteredState;

    public string TestMarriageDate(SpouseRelationship relationship) => MarriageDate(relationship);

    public string TestDivorceDate(SpouseRelationship relationship) => DivorceDate(relationship);
  }
}
