namespace FamilyShowLib.Tests;

/// <summary>
/// Round-trip tests: Import GEDCOM -> internal model -> Export GEDCOM -> re-import
/// and assert names, prefixes, suffixes, and type preserved.
/// </summary>
public class GedcomRoundTripTest
{
  [StaFact]
  public void RoundTrip_SingleNameWithAllComponents_PreservesData()
  {
    // Arrange: Create a person with a complete name
    Person originalPerson = new();
    originalPerson.Names.Clear();
    Name originalName = new Name(
      firstName: "John",
      surname: "Smith",
      prefix: "Dr.",
      suffix: "Jr.",
      surnamePrefix: "von",
      nameType: NameType.Birth,
      isPrimary: true
    );
    originalPerson.Names.Add(originalName);
    originalPerson.Gender = Gender.Male;

    // Act: Export to GEDCOM
    PeopleCollection people = new();
    people.Add(originalPerson);
    string tempFile = Path.GetTempFileName() + ".ged";

    try
    {
      GedcomExport exporter = new();
      exporter.Export(people, new SourceCollection(), new RepositoryCollection(), tempFile, "1.0", "FamilyShow");

      // Read the exported file content
      string gedcomContent = File.ReadAllText(tempFile);

      // Verify GEDCOM contains expected tags
      Assert.Contains("1 NAME John /Smith/", gedcomContent);
      Assert.Contains("2 SURN Smith", gedcomContent);
      Assert.Contains("2 NPFX Dr.", gedcomContent);
      Assert.Contains("2 NSFX Jr.", gedcomContent);
      Assert.Contains("2 SPFX von", gedcomContent);

      // Re-import
      GedcomImport importer = new();
      PeopleCollection reimportedPeople = new();
      SourceCollection sources = new();
      RepositoryCollection repos = new();
      importer.Import(reimportedPeople, sources, repos, tempFile, true);

      // Assert: Verify round-trip preserved data
      Assert.Single(reimportedPeople);
      Person reimportedPerson = reimportedPeople[0];
      Assert.Single(reimportedPerson.Names);
      Name reimportedName = reimportedPerson.Names[0];

      Assert.Equal("John", reimportedName.FirstName);
      Assert.Equal("Smith", reimportedName.Surname);
      Assert.Equal("Dr.", reimportedName.Prefix);
      Assert.Equal("Jr.", reimportedName.Suffix);
      Assert.Equal("von", reimportedName.SurnamePrefix);
      Assert.Equal(NameType.Birth, reimportedName.NameType);
      Assert.True(reimportedName.IsPrimary);
    }
    finally
    {
      if (File.Exists(tempFile))
        File.Delete(tempFile);
    }
  }

  [StaFact]
  public void RoundTrip_MultipleNames_PreservesAllNamesAndTypes()
  {
    // Arrange: Create a person with multiple names
    Person originalPerson = new();
    originalPerson.Names.Clear();
    originalPerson.Names.Add(new Name("John", "Smith", null, null, null, NameType.Birth, true));
    originalPerson.Names.Add(new Name("Johnny", "Smith", null, null, null, NameType.Aka, false));
    originalPerson.Names.Add(new Name("John", "Doe", null, null, null, NameType.Married, false));
    originalPerson.Gender = Gender.Male;

    PeopleCollection people = new();
    people.Add(originalPerson);
    string tempFile = Path.GetTempFileName() + ".ged";

    try
    {
      // Export
      GedcomExport exporter = new();
      exporter.Export(people, new SourceCollection(), new RepositoryCollection(), tempFile, "1.0", "FamilyShow");

      // Verify GEDCOM contains multiple NAME tags with TYPE
      string gedcomContent = File.ReadAllText(tempFile);
      Assert.Contains("1 NAME John /Smith/", gedcomContent);
      Assert.Contains("1 NAME Johnny /Smith/", gedcomContent);
      Assert.Contains("1 NAME John /Doe/", gedcomContent);
      Assert.Contains("2 TYPE AKA", gedcomContent);
      Assert.Contains("2 TYPE MARRIED", gedcomContent);
      // Birth name should NOT have TYPE tag (it's the default)
      int birthTypeCount = gedcomContent.Split("2 TYPE BIRTH").Length - 1;
      Assert.Equal(0, birthTypeCount);

      // Re-import
      GedcomImport importer = new();
      PeopleCollection reimportedPeople = new();
      importer.Import(reimportedPeople, new SourceCollection(), new RepositoryCollection(), tempFile, true);

      // Assert
      Assert.Single(reimportedPeople);
      Person reimportedPerson = reimportedPeople[0];
      Assert.Equal(3, reimportedPerson.Names.Count);

      // Verify all names are preserved with correct types
      Name birthName = reimportedPerson.Names.FirstOrDefault(n => n.NameType == NameType.Birth);
      Name akaName = reimportedPerson.Names.FirstOrDefault(n => n.NameType == NameType.Aka);
      Name marriedName = reimportedPerson.Names.FirstOrDefault(n => n.NameType == NameType.Married);

      Assert.NotNull(birthName);
      Assert.NotNull(akaName);
      Assert.NotNull(marriedName);

      Assert.Equal("John", birthName.FirstName);
      Assert.Equal("Smith", birthName.Surname);
      Assert.True(birthName.IsPrimary);

      Assert.Equal("Johnny", akaName.FirstName);
      Assert.Equal("Smith", akaName.Surname);
      Assert.False(akaName.IsPrimary);

      Assert.Equal("John", marriedName.FirstName);
      Assert.Equal("Doe", marriedName.Surname);
      Assert.False(marriedName.IsPrimary);
    }
    finally
    {
      if (File.Exists(tempFile))
        File.Delete(tempFile);
    }
  }

  [StaFact]
  public void RoundTrip_MissingSurname_PreservesFirstNameOnly()
  {
    // Arrange: Person with first name only
    Person originalPerson = new();
    originalPerson.Names.Clear();
    originalPerson.Names.Add(new Name("Madonna", "", null, null, null, NameType.Birth, true));
    originalPerson.Gender = Gender.Female;

    PeopleCollection people = new();
    people.Add(originalPerson);
    string tempFile = Path.GetTempFileName() + ".ged";

    try
    {
      GedcomExport exporter = new();
      exporter.Export(people, new SourceCollection(), new RepositoryCollection(), tempFile, "1.0", "FamilyShow");

      string gedcomContent = File.ReadAllText(tempFile);
      Assert.Contains("1 NAME Madonna //", gedcomContent);

      GedcomImport importer = new();
      PeopleCollection reimportedPeople = new();
      importer.Import(reimportedPeople, new SourceCollection(), new RepositoryCollection(), tempFile, true);

      Assert.Single(reimportedPeople);
      Name reimportedName = reimportedPeople[0].Names[0];
      Assert.Equal("Madonna", reimportedName.FirstName);
      Assert.Equal("", reimportedName.Surname);
    }
    finally
    {
      if (File.Exists(tempFile))
        File.Delete(tempFile);
    }
  }

  [StaFact]
  public void RoundTrip_MissingGivenName_PreservesSurnameOnly()
  {
    // Arrange: Person with surname only
    Person originalPerson = new();
    originalPerson.Names.Clear();
    originalPerson.Names.Add(new Name("", "Unknown", null, null, null, NameType.Birth, true));
    originalPerson.Gender = Gender.Male;

    PeopleCollection people = new();
    people.Add(originalPerson);
    string tempFile = Path.GetTempFileName() + ".ged";

    try
    {
      GedcomExport exporter = new();
      exporter.Export(people, new SourceCollection(), new RepositoryCollection(), tempFile, "1.0", "FamilyShow");

      string gedcomContent = File.ReadAllText(tempFile);
      Assert.Contains("1 NAME /Unknown/", gedcomContent);

      GedcomImport importer = new();
      PeopleCollection reimportedPeople = new();
      importer.Import(reimportedPeople, new SourceCollection(), new RepositoryCollection(), tempFile, true);

      Assert.Single(reimportedPeople);
      Name reimportedName = reimportedPeople[0].Names[0];
      Assert.Equal("", reimportedName.FirstName);
      Assert.Equal("Unknown", reimportedName.Surname);
    }
    finally
    {
      if (File.Exists(tempFile))
        File.Delete(tempFile);
    }
  }

  [StaFact]
  public void RoundTrip_AllNameTypes_PreservesTypeValues()
  {
    // Test all supported name types
    Person originalPerson = new();
    originalPerson.Names.Clear();
    originalPerson.Names.Add(new Name("A", "Birth", null, null, null, NameType.Birth, true));
    originalPerson.Names.Add(new Name("B", "Aka", null, null, null, NameType.Aka, false));
    originalPerson.Names.Add(new Name("C", "Married", null, null, null, NameType.Married, false));
    originalPerson.Names.Add(new Name("D", "Maiden", null, null, null, NameType.Maiden, false));
    originalPerson.Names.Add(new Name("E", "Immigration", null, null, null, NameType.Immigration, false));
    originalPerson.Names.Add(new Name("F", "Professional", null, null, null, NameType.Professional, false));
    originalPerson.Names.Add(new Name("G", "Other", null, null, null, NameType.Other, false));
    originalPerson.Gender = Gender.Male;

    PeopleCollection people = new();
    people.Add(originalPerson);
    string tempFile = Path.GetTempFileName() + ".ged";

    try
    {
      GedcomExport exporter = new();
      exporter.Export(people, new SourceCollection(), new RepositoryCollection(), tempFile, "1.0", "FamilyShow");

      GedcomImport importer = new();
      PeopleCollection reimportedPeople = new();
      importer.Import(reimportedPeople, new SourceCollection(), new RepositoryCollection(), tempFile, true);

      Assert.Single(reimportedPeople);
      Assert.Equal(7, reimportedPeople[0].Names.Count);

      // Verify each type was preserved
      Assert.Contains(reimportedPeople[0].Names, n => n.NameType == NameType.Birth);
      Assert.Contains(reimportedPeople[0].Names, n => n.NameType == NameType.Aka);
      Assert.Contains(reimportedPeople[0].Names, n => n.NameType == NameType.Married);
      Assert.Contains(reimportedPeople[0].Names, n => n.NameType == NameType.Maiden);
      Assert.Contains(reimportedPeople[0].Names, n => n.NameType == NameType.Immigration);
      Assert.Contains(reimportedPeople[0].Names, n => n.NameType == NameType.Professional);
      Assert.Contains(reimportedPeople[0].Names, n => n.NameType == NameType.Other);
    }
    finally
    {
      if (File.Exists(tempFile))
        File.Delete(tempFile);
    }
  }
}
