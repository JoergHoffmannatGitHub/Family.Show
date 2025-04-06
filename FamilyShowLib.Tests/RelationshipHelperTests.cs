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
  public void UpdateMarriageDate_ShouldNotUpdateMarriageDateForBothPersons()
  {
    // Arrange
    Person father = new("John", "Doe", Gender.Male);
    Person mother = new("Jane", "Doe", Gender.Female);

    ParentRelationship parentRelationship = new(mother, ParentChildModifier.Natural);
    father.Relationships.Add(parentRelationship);
    mother.Relationships.Add(new ParentRelationship(father, ParentChildModifier.Natural));

    // Act
    RelationshipHelper.UpdateMarriageDate(father, mother, new(2020, 1, 1));

    // Assert
    Assert.Null(father.GetSpouseRelationship(mother));
    Assert.Null(mother.GetSpouseRelationship(father));
  }

  [Fact]
  public void UpdateMarriageDate_ShouldNotUpdateIfNoSpouseRelationshipExists()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);

    // Act
    RelationshipHelper.UpdateMarriageDate(person, spouse, new(2020, 1, 1));

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

    SpouseRelationship spouseRelationship = new(spouse, SpouseModifier.Current);
    person.Relationships.Add(spouseRelationship);
    spouse.Relationships.Add(new SpouseRelationship(person, SpouseModifier.Current));

    // Act
    RelationshipHelper.UpdateMarriageDate(person, spouse, null);

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
  public void UpdateMarriageDateDescriptor_ShouldNotUpdateMarriageDateDescriptorForBothPersons()
  {
    // Arrange
    Person father = new("John", "Doe", Gender.Male);
    Person mother = new("Jane", "Doe", Gender.Female);

    ParentRelationship parentRelationship = new(mother, ParentChildModifier.Natural);
    father.Relationships.Add(parentRelationship);
    mother.Relationships.Add(new ParentRelationship(father, ParentChildModifier.Natural));

    // Act
    RelationshipHelper.UpdateMarriageDateDescriptor(father, mother, "Spring 2020");

    // Assert
    Assert.Null(father.GetSpouseRelationship(mother));
    Assert.Null(mother.GetSpouseRelationship(father));
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
  public void UpdateDivorceDate_ShouldNotUpdateMarriageDateForBothPersons()
  {
    // Arrange
    Person father = new("John", "Doe", Gender.Male);
    Person mother = new("Jane", "Doe", Gender.Female);

    ParentRelationship spouseRelationship = new(mother, ParentChildModifier.Natural);
    father.Relationships.Add(spouseRelationship);
    mother.Relationships.Add(new ParentRelationship(father, ParentChildModifier.Natural));

    // Act
    RelationshipHelper.UpdateDivorceDate(father, mother, new(2020, 1, 1));

    // Assert
    Assert.Null(father.GetSpouseRelationship(mother));
    Assert.Null(mother.GetSpouseRelationship(father));
  }

  [Fact]
  public void UpdateDivorceDate_ShouldNotUpdateIfNoSpouseRelationshipExists()
  {
    // Arrange
    Person person = new("John", "Doe", Gender.Male);
    Person spouse = new("Jane", "Doe", Gender.Female);

    // Act
    RelationshipHelper.UpdateDivorceDate(person, spouse, new(2020, 1, 1));

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

    SpouseRelationship spouseRelationship = new(spouse, SpouseModifier.Current);
    person.Relationships.Add(spouseRelationship);
    spouse.Relationships.Add(new SpouseRelationship(person, SpouseModifier.Current));

    // Act
    RelationshipHelper.UpdateDivorceDate(person, spouse, null);

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
  public void UpdateDivorceDateDescriptor_ShouldNotUpdateDivorceDateDescriptorForBothPersons()
  {
    // Arrange
    Person father = new("John", "Doe", Gender.Male);
    Person mother = new("Jane", "Doe", Gender.Female);

    ParentRelationship parentRelationship = new(mother, ParentChildModifier.Natural);
    father.Relationships.Add(parentRelationship);
    mother.Relationships.Add(new ParentRelationship(father, ParentChildModifier.Natural));

    // Act
    RelationshipHelper.UpdateDivorceDateDescriptor(father, mother, "Winter 2021");

    // Assert
    Assert.Null(father.GetSpouseRelationship(mother));
    Assert.Null(mother.GetSpouseRelationship(father));
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
