namespace FamilyShowLib.Tests;

public class RelationshipHelperTests
{
  [Fact]
  public void UpdateMarriageDate_ShouldUpdateMarriageDateForBothPersonAndSpouse()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);
    DateWrapper marriageDate = new(2020, 1, 1);

    SpouseRelationship spouseRelationship = new(spouse, SpouseModifier.Current);
    person.Relationships.Add(spouseRelationship);
    spouse.Relationships.Add(new SpouseRelationship(person, SpouseModifier.Current));

    // Act
    RelationshipHelper.UpdateMarriageDate(person, spouse, marriageDate);

    // Assert
    Assert.Equal(marriageDate, person.GetSpouseRelationship(spouse).MarriageDate);
    Assert.Equal(marriageDate, spouse.GetSpouseRelationship(person).MarriageDate);
  }

  [Fact]
  public void UpdateMarriageDate_ShouldNotUpdateIfNoSpouseRelationshipExists()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);
    DateWrapper marriageDate = new(2020, 1, 1);

    // Act
    RelationshipHelper.UpdateMarriageDate(person, spouse, marriageDate);

    // Assert
    Assert.Null(person.GetSpouseRelationship(spouse));
    Assert.Null(spouse.GetSpouseRelationship(person));
  }

  [Fact]
  public void UpdateMarriageDate_ShouldHandleNullDate()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);
    DateWrapper? marriageDate = null;

    SpouseRelationship spouseRelationship = new(spouse, SpouseModifier.Current);
    person.Relationships.Add(spouseRelationship);
    spouse.Relationships.Add(new SpouseRelationship(person, SpouseModifier.Current));

    // Act
    RelationshipHelper.UpdateMarriageDate(person, spouse, marriageDate);

    // Assert
    Assert.Null(person.GetSpouseRelationship(spouse).MarriageDate);
    Assert.Null(spouse.GetSpouseRelationship(person).MarriageDate);
  }

  [Fact]
  public void UpdateMarriageDateDescriptor_ShouldUpdateMarriageDateDescriptorForBothPersonAndSpouse()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);
    string marriageDateDescriptor = "Spring 2020";

    SpouseRelationship spouseRelationship = new(spouse, SpouseModifier.Current);
    person.Relationships.Add(spouseRelationship);
    spouse.Relationships.Add(new SpouseRelationship(person, SpouseModifier.Current));

    // Act
    RelationshipHelper.UpdateMarriageDateDescriptor(person, spouse, marriageDateDescriptor);

    // Assert
    Assert.Equal(marriageDateDescriptor, person.GetSpouseRelationship(spouse).MarriageDateDescriptor);
    Assert.Equal(marriageDateDescriptor, spouse.GetSpouseRelationship(person).MarriageDateDescriptor);
  }

  [Fact]
  public void UpdateMarriageDateDescriptor_ShouldNotUpdateIfNoSpouseRelationshipExists()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);
    string marriageDateDescriptor = "Spring 2020";

    // Act
    RelationshipHelper.UpdateMarriageDateDescriptor(person, spouse, marriageDateDescriptor);

    // Assert
    Assert.Null(person.GetSpouseRelationship(spouse));
    Assert.Null(spouse.GetSpouseRelationship(person));
  }

  [Fact]
  public void UpdateMarriageDateDescriptor_ShouldHandleNullDescriptor()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);
    string? marriageDateDescriptor = null;

    SpouseRelationship spouseRelationship = new(spouse, SpouseModifier.Current);
    person.Relationships.Add(spouseRelationship);
    spouse.Relationships.Add(new SpouseRelationship(person, SpouseModifier.Current));

    // Act
    RelationshipHelper.UpdateMarriageDateDescriptor(person, spouse, marriageDateDescriptor);

    // Assert
    Assert.Null(person.GetSpouseRelationship(spouse).MarriageDateDescriptor);
    Assert.Null(spouse.GetSpouseRelationship(person).MarriageDateDescriptor);
  }

  [Fact]
  public void UpdateDivorceDate_ShouldUpdateDivorceDateForBothPersonAndSpouse()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);
    DateWrapper divorceDate = new(2021, 1, 1);

    SpouseRelationship spouseRelationship = new(spouse, SpouseModifier.Current);
    person.Relationships.Add(spouseRelationship);
    spouse.Relationships.Add(new SpouseRelationship(person, SpouseModifier.Current));

    // Act
    RelationshipHelper.UpdateDivorceDate(person, spouse, divorceDate);

    // Assert
    Assert.Equal(divorceDate, person.GetSpouseRelationship(spouse).DivorceDate);
    Assert.Equal(divorceDate, spouse.GetSpouseRelationship(person).DivorceDate);
  }

  [Fact]
  public void UpdateDivorceDate_ShouldNotUpdateIfNoSpouseRelationshipExists()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);
    DateWrapper divorceDate = new(2021, 1, 1);

    // Act
    RelationshipHelper.UpdateDivorceDate(person, spouse, divorceDate);

    // Assert
    Assert.Null(person.GetSpouseRelationship(spouse));
    Assert.Null(spouse.GetSpouseRelationship(person));
  }

  [Fact]
  public void UpdateDivorceDate_ShouldHandleNullDate()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);
    DateWrapper? divorceDate = null;

    SpouseRelationship spouseRelationship = new(spouse, SpouseModifier.Current);
    person.Relationships.Add(spouseRelationship);
    spouse.Relationships.Add(new SpouseRelationship(person, SpouseModifier.Current));

    // Act
    RelationshipHelper.UpdateDivorceDate(person, spouse, divorceDate);

    // Assert
    Assert.Null(person.GetSpouseRelationship(spouse).DivorceDate);
    Assert.Null(spouse.GetSpouseRelationship(person).DivorceDate);
  }

  [Fact]
  public void UpdateDivorceDateDescriptor_ShouldUpdateDivorceDateDescriptorForBothPersonAndSpouse()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);
    string divorceDateDescriptor = "Winter 2021";

    SpouseRelationship spouseRelationship = new(spouse, SpouseModifier.Current);
    person.Relationships.Add(spouseRelationship);
    spouse.Relationships.Add(new SpouseRelationship(person, SpouseModifier.Current));

    // Act
    RelationshipHelper.UpdateDivorceDateDescriptor(person, spouse, divorceDateDescriptor);

    // Assert
    Assert.Equal(divorceDateDescriptor, person.GetSpouseRelationship(spouse).DivorceDateDescriptor);
    Assert.Equal(divorceDateDescriptor, spouse.GetSpouseRelationship(person).DivorceDateDescriptor);
  }

  [Fact]
  public void UpdateDivorceDateDescriptor_ShouldNotUpdateIfNoSpouseRelationshipExists()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);
    string divorceDateDescriptor = "Winter 2021";

    // Act
    RelationshipHelper.UpdateDivorceDateDescriptor(person, spouse, divorceDateDescriptor);

    // Assert
    Assert.Null(person.GetSpouseRelationship(spouse));
    Assert.Null(spouse.GetSpouseRelationship(person));
  }

  [Fact]
  public void UpdateDivorceDateDescriptor_ShouldHandleNullDescriptor()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);
    string? divorceDateDescriptor = null;

    SpouseRelationship spouseRelationship = new(spouse, SpouseModifier.Current);
    person.Relationships.Add(spouseRelationship);
    spouse.Relationships.Add(new SpouseRelationship(person, SpouseModifier.Current));

    // Act
    RelationshipHelper.UpdateDivorceDateDescriptor(person, spouse, divorceDateDescriptor);

    // Assert
    Assert.Null(person.GetSpouseRelationship(spouse).DivorceDateDescriptor);
    Assert.Null(spouse.GetSpouseRelationship(person).DivorceDateDescriptor);
  }
}
