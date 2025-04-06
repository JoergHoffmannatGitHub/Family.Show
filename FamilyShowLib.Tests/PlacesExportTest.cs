namespace FamilyShowLib.Tests;

public class PlacesExportTest
{
  [Fact]
  public void ExportPlaces_EmptyPeopleCollection_ReturnsNoPlaces()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      PeopleCollection peopleCollection = [];
      string fileName = "test.kml";

      // Act
      string[] result = PlacesExport.ExportPlaces(peopleCollection, fileName, false, ExportPlacesType.Places, true, true, true, true, true);

      // Assert
      Assert.Equal(Properties.Resources.NoPlaces, result[0]);
      Assert.Equal("No file", result[1]);
    }
  }

  [Fact]
  public void ExportPlaces_WithBirths_ExportsCorrectly()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      PeopleCollection peopleCollection = [];
      Person person = new("John", "Doe")
      {
        BirthPlace = "New York",
        BirthDate = new(2000, 1, 1),
        Gender = Gender.Male
      };
      peopleCollection.Add(person);
      string fileName = "test.kml";

      // Act
      string[] result = PlacesExport.ExportPlaces(peopleCollection, fileName, false, ExportPlacesType.Times, false, false, false, true, false);

      // Assert
      Assert.Contains("1 " + Properties.Resources.PlacesExported, result[0]);
      Assert.Equal(fileName, result[1]);
      Assert.True(File.Exists(fileName));
      File.Delete(fileName);
    }
  }

  [Fact]
  public void ExportPlaces_WithDeaths_ExportsCorrectly()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      PeopleCollection peopleCollection = [];
      Person person = new("Jane", "Doe")
      {
        DeathPlace = "Los Angeles",
        DeathDate = new(2020, 1, 1),
        Gender = Gender.Female,
        IsLiving = false
      };
      peopleCollection.Add(person);
      string fileName = "test.kml";

      // Act
      string[] result = PlacesExport.ExportPlaces(peopleCollection, fileName, false, ExportPlacesType.Times, false, true, false, false, false);

      // Assert
      Assert.Contains("1 " + Properties.Resources.PlacesExported, result[0]);
      Assert.Equal(fileName, result[1]);
      Assert.True(File.Exists(fileName));
      File.Delete(fileName);
    }
  }

  [Fact]
  public void ExportPlaces_WithBurials_ExportsCorrectly()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      PeopleCollection peopleCollection = [];
      Person person = new("John", "Smith")
      {
        BurialPlace = "Chicago",
        BurialDate = new(2021, 1, 1),
        Gender = Gender.Male,
        IsLiving = false
      };
      peopleCollection.Add(person);
      string fileName = "test.kml";

      // Act
      string[] result = PlacesExport.ExportPlaces(peopleCollection, fileName, false, ExportPlacesType.Times, true, false, false, false, false);

      // Assert
      Assert.Contains("1 " + Properties.Resources.PlacesExported, result[0]);
      Assert.Equal(fileName, result[1]);
      Assert.True(File.Exists(fileName));
      File.Delete(fileName);
    }
  }

  [Fact]
  public void ExportPlaces_WithCremations_ExportsCorrectly()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      PeopleCollection peopleCollection = [];
      Person person = new("Jane", "Smith")
      {
        CremationPlace = "San Francisco",
        CremationDate = new(2022, 1, 1),
        Gender = Gender.Female,
        IsLiving = false
      };
      peopleCollection.Add(person);
      string fileName = "test.kml";

      // Act
      string[] result = PlacesExport.ExportPlaces(peopleCollection, fileName, false, ExportPlacesType.Times, false, false, true, false, false);

      // Assert
      Assert.Contains("1 " + Properties.Resources.PlacesExported, result[0]);
      Assert.Equal(fileName, result[1]);
      Assert.True(File.Exists(fileName));
      File.Delete(fileName);
    }
  }

  [Fact]
  public void ExportPlaces_WithMarriages_ExportsCorrectly()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      PeopleCollection peopleCollection = [];
      Person person = new("John", "Doe")
      {
        Gender = Gender.Male,
        IsLiving = false
      };
      Person spouse = new("Jane", "Doe")
      {
        Gender = Gender.Female,
        IsLiving = false
      };
      SpouseRelationship relationship = new(person, SpouseModifier.Current)
      {
        MarriagePlace = "Las Vegas",
        MarriageDate = new(2010, 1, 1)
      };
      person.Relationships.Add(relationship);
      peopleCollection.Add(person);
      peopleCollection.Add(spouse);
      string fileName = "test.kml";

      // Act
      string[] result = PlacesExport.ExportPlaces(peopleCollection, fileName, false, ExportPlacesType.Times, false, false, false, false, true);

      // Assert
      Assert.Contains("1 " + Properties.Resources.PlacesExported, result[0]);
      Assert.Equal(fileName, result[1]);
      Assert.True(File.Exists(fileName));
      File.Delete(fileName);
    }
  }

  [Theory, CombinatorialData]
  public void ExportPlaces_WithExportPlacesTypes_ExportsCorrectly(
    [CombinatorialValues(ExportPlacesType.Places, ExportPlacesType.Times, ExportPlacesType.Lifespans)] ExportPlacesType exportPlacesType
    )
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      PeopleCollection peopleCollection = [];
      Person person = new("John", "Doe")
      {
        BirthPlace = "New York",
        BirthDate = new(2000, 1, 1),
        Gender = Gender.Male
      };
      peopleCollection.Add(person);
      person = new("Jane", "Doe")
      {
        BirthPlace = "Los Angeles",
        BirthDate = new(2000, 1, 1),
        Gender = Gender.Female
      };
      peopleCollection.Add(person);
      person = new("John", "Smith")
      {
        BirthPlace = "Chicago",
        BirthDate = new(1980, 1, 1),
        DeathPlace = "San Francisco",
        DeathDate = new(2020, 1, 1),
        Gender = Gender.Male,
        IsLiving = false
      };
      peopleCollection.Add(person);
      string fileName = "test_exportPlacesTypes.kml";

      // Act
      string[] result = PlacesExport.ExportPlaces(
        peopleCollection, fileName, false,
        exportPlacesType,
        false, false, false, true, false);

      // Assert
      Assert.Contains("3 " + Properties.Resources.PlacesExported, result[0]);
      Assert.Equal(fileName, result[1]);
      Assert.True(File.Exists(fileName));
      File.Delete(fileName);
    }
  }

  [Theory, CombinatorialData]
  public void ExportPlaces_WithTimesOnly_Burial_ExportsCorrectly(
    [CombinatorialValues(true, false)] bool withBurialDate
    )
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      PeopleCollection peopleCollection = [];
      Person person = new("Jane", "Doe")
      {
        BirthPlace = "Los Angeles",
        BirthDate = new(2000, 1, 1),
        BurialPlace = "San Francisco",
        BurialDate = withBurialDate ? new(2020, 1, 1) : null,
        Gender = Gender.Female
      };
      peopleCollection.Add(person);
      string fileName = "test_exportPlacesTypes.kml";

      // Act
      string[] result = PlacesExport.ExportPlaces(
        peopleCollection, fileName, false,
        ExportPlacesType.Times,
        true, false, false, false, false);

      // Assert
      Assert.Contains(withBurialDate ? "1 " + Properties.Resources.PlacesExported : Properties.Resources.NoPlaces, result[0]);
      Assert.Equal(withBurialDate ? fileName : "No file", result[1]);
      Assert.Equal(withBurialDate, File.Exists(fileName));
      File.Delete(fileName);
    }
  }

  [Theory, CombinatorialData]
  public void ExportPlaces_WithTimesOnly_Cremation_ExportsCorrectly(
    [CombinatorialValues(true, false)] bool withCremationDate
    )
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      PeopleCollection peopleCollection = [];
      Person person = new("Jane", "Doe")
      {
        BirthPlace = "Los Angeles",
        BirthDate = new(2000, 1, 1),
        CremationPlace = "San Francisco",
        CremationDate = withCremationDate ? new(2020, 1, 1) : null,
        Gender = Gender.Female
      };
      peopleCollection.Add(person);
      string fileName = "test_exportPlacesTypes.kml";

      // Act
      string[] result = PlacesExport.ExportPlaces(
        peopleCollection, fileName, false,
        ExportPlacesType.Times,
        false, false, true, false, false);

      // Assert
      Assert.Contains(withCremationDate ? "1 " + Properties.Resources.PlacesExported : Properties.Resources.NoPlaces, result[0]);
      Assert.Equal(withCremationDate ? fileName : "No file", result[1]);
      Assert.Equal(withCremationDate, File.Exists(fileName));
      File.Delete(fileName);
    }
  }

  [Theory, CombinatorialData]
  public void ExportPlaces_WithTimesOnly_Marriages_ExportsCorrectly(
    [CombinatorialValues(true, false)] bool withMarriagesDate
    )
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      PeopleCollection peopleCollection = [];
      Person john = new("John", "Doe")
      {
        BirthPlace = "New York",
        BirthDate = new(2000, 1, 1),
        Gender = Gender.Male
      };
      peopleCollection.Add(john);
      Person jane = new("Jane", "Doe")
      {
        BirthPlace = "Los Angeles",
        BirthDate = new(2000, 1, 1),
        Gender = Gender.Female
      };
      peopleCollection.Add(jane);
      SpouseRelationship relationship = new(john, SpouseModifier.Current)
      {
        MarriagePlace = "Las Vegas",
        MarriageDate = withMarriagesDate ? new(2020, 1, 1) : null
      };
      jane.Relationships.Add(relationship);
      relationship = new(jane, SpouseModifier.Current)
      {
        MarriagePlace = "Las Vegas",
        MarriageDate = withMarriagesDate ? new(2020, 1, 1) : null
      };
      john.Relationships.Add(relationship);
      string fileName = "test_exportPlacesTypes.kml";

      // Act
      string[] result = PlacesExport.ExportPlaces(
        peopleCollection, fileName, false,
        ExportPlacesType.Times,
        false, false, false, false, true);

      // Assert
      Assert.Contains("2 " + Properties.Resources.PlacesExported, result[0]);
      Assert.Equal(fileName, result[1]);
      Assert.True(File.Exists(fileName));
      File.Delete(fileName);
    }
  }
}
