using System.Globalization;
using System.Text;
using System.Xml;

namespace FamilyShowLib.Tests;

public class GedcomExportTest
{
  [Theory]
  [InlineData(2019, 6, 21, "21 JUN 2019")]
  [InlineData(1982, 1, 9, "9 JAN 1982")]
  public void ExportDate(int year, int month, int day, string expected)
  {
    DateTime date = new(year, month, day);
    string result = GedcomExport.ExportDateWrapper(date);
    Assert.Equal(expected, result);
    Assert.Equal(date, DateTime.Parse(result, CultureInfo.InvariantCulture));
  }
}

public class NameImportExportTest
{
  [StaFact]
  public void ImportName_WithAllComponents_SetsAllProperties()
  {
    // Arrange
    string xmlContent = @"<INDI Value=""@I1@"">
      <NAME Value=""John /Smith/"">
        <SURN Value=""Smith"" />
        <NPFX Value=""Dr."" />
        <NSFX Value=""Jr."" />
        <SPFX Value=""von"" />
        <TYPE Value=""BIRTH"" />
      </NAME>
      <SEX Value=""M"" />
    </INDI>";

    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode node = doc.DocumentElement!;

    // Act
    Person person = new();
    GedcomImport.ImportNamesWrapper(person, node);

    // Assert
    Assert.Single(person.Names);
    Name name = person.Names[0];
    Assert.Equal("John", name.FirstName);
    Assert.Equal("Smith", name.Surname);
    Assert.Equal("Dr.", name.Prefix);
    Assert.Equal("Jr.", name.Suffix);
    Assert.Equal("von", name.SurnamePrefix);
    Assert.Equal(NameType.Birth, name.NameType);
    Assert.True(name.IsPrimary);
  }

  [StaFact]
  public void ImportNames_MultipleNames_BirthNameIsPrimary()
  {
    // Arrange
    string xmlContent = @"<INDI Value=""@I1@"">
      <NAME Value=""Joe /Smith/"">
        <TYPE Value=""AKA"" />
      </NAME>
      <NAME Value=""John /Smith/"">
        <TYPE Value=""BIRTH"" />
      </NAME>
      <NAME Value=""Johnny /Smith/"">
        <TYPE Value=""MARRIED"" />
      </NAME>
      <SEX Value=""M"" />
    </INDI>";

    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode node = doc.DocumentElement!;

    // Act
    Person person = new();
    GedcomImport.ImportNamesWrapper(person, node);

    // Assert
    Assert.Equal(3, person.Names.Count);

    // Birth name should be primary even though it's not first
    Name primaryName = person.Names.PrimaryName;
    Assert.Equal("John", primaryName.FirstName);
    Assert.Equal(NameType.Birth, primaryName.NameType);
    Assert.True(primaryName.IsPrimary);

    // Verify other names are not primary
    Assert.False(person.Names[0].IsPrimary); // AKA
    Assert.True(person.Names[1].IsPrimary);  // Birth
    Assert.False(person.Names[2].IsPrimary); // Married
  }

  [StaFact]
  public void ImportNames_NoBirthName_FirstNameIsPrimary()
  {
    // Arrange
    string xmlContent = @"<INDI Value=""@I1@"">
      <NAME Value=""Joe /Smith/"">
        <TYPE Value=""AKA"" />
      </NAME>
      <NAME Value=""Johnny /Smith/"">
        <TYPE Value=""MARRIED"" />
      </NAME>
      <SEX Value=""M"" />
    </INDI>";

    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode node = doc.DocumentElement!;

    // Act
    Person person = new();
    GedcomImport.ImportNamesWrapper(person, node);

    // Assert
    Assert.Equal(2, person.Names.Count);

    // First name should be primary when no birth name exists
    Assert.True(person.Names[0].IsPrimary);
    Assert.False(person.Names[1].IsPrimary);
  }

  [StaFact]
  public void ImportNames_CaseInsensitiveType_ParsesCorrectly()
  {
    // Test lowercase type values
    string xmlContent = @"<INDI Value=""@I1@"">
      <NAME Value=""John /Smith/"">
        <TYPE Value=""aka"" />
      </NAME>
      <SEX Value=""M"" />
    </INDI>";

    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode node = doc.DocumentElement!;

    Person person = new();
    GedcomImport.ImportNamesWrapper(person, node);

    Assert.Equal(NameType.Aka, person.Names[0].NameType);
  }

  [StaFact]
  public void ImportNames_MixedCaseType_ParsesCorrectly()
  {
    // Test mixed case type values
    string xmlContent = @"<INDI Value=""@I1@"">
      <NAME Value=""John /Smith/"">
        <TYPE Value=""Married"" />
      </NAME>
      <SEX Value=""M"" />
    </INDI>";

    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode node = doc.DocumentElement!;

    Person person = new();
    GedcomImport.ImportNamesWrapper(person, node);

    Assert.Equal(NameType.Married, person.Names[0].NameType);
  }

  [StaFact]
  public void ImportNames_EmptyFirstName_ParsesCorrectly()
  {
    // Surname only (missing given name)
    string xmlContent = @"<INDI Value=""@I1@"">
      <NAME Value=""/Smith/"" />
      <SEX Value=""M"" />
    </INDI>";

    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode node = doc.DocumentElement!;

    Person person = new();
    GedcomImport.ImportNamesWrapper(person, node);

    Assert.Single(person.Names);
    Assert.Equal("", person.Names[0].FirstName);
    Assert.Equal("Smith", person.Names[0].Surname);
  }

  [StaFact]
  public void ImportNames_EmptySurname_ParsesCorrectly()
  {
    // First name only (missing surname)
    string xmlContent = @"<INDI Value=""@I1@"">
      <NAME Value=""John //"" />
      <SEX Value=""M"" />
    </INDI>";

    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode node = doc.DocumentElement!;

    Person person = new();
    GedcomImport.ImportNamesWrapper(person, node);

    Assert.Single(person.Names);
    Assert.Equal("John", person.Names[0].FirstName);
    Assert.Equal("", person.Names[0].Surname);
  }

  [StaFact]
  public void ImportNames_DuplicateNameTags_ImportsAll()
  {
    // Duplicate NAME tags with same content
    string xmlContent = @"<INDI Value=""@I1@"">
      <NAME Value=""John /Smith/"" />
      <NAME Value=""John /Smith/"" />
      <NAME Value=""John /Smith/"" />
      <SEX Value=""M"" />
    </INDI>";

    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode node = doc.DocumentElement!;

    Person person = new();
    GedcomImport.ImportNamesWrapper(person, node);

    // All duplicate NAME tags should be imported
    Assert.Equal(3, person.Names.Count);
    // First one should be primary (no TYPE specified means Birth, and first Birth wins)
    Assert.True(person.Names[0].IsPrimary);
    Assert.False(person.Names[1].IsPrimary);
    Assert.False(person.Names[2].IsPrimary);
  }

  [StaFact]
  public void ImportNames_AlsoKnownAsVariant_ParsesCorrectly()
  {
    // Test "ALSO KNOWN AS" variant
    string xmlContent = @"<INDI Value=""@I1@"">
      <NAME Value=""John /Smith/"">
        <TYPE Value=""ALSO KNOWN AS"" />
      </NAME>
      <SEX Value=""M"" />
    </INDI>";

    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode node = doc.DocumentElement!;

    Person person = new();
    GedcomImport.ImportNamesWrapper(person, node);

    Assert.Equal(NameType.Aka, person.Names[0].NameType);
  }

  [Fact]
  public void BuildNameValue_BothNames_ReturnsCorrectFormat()
  {
    string result = GedcomExport.BuildNameValueWrapper("John", "Smith");
    Assert.Equal("John /Smith/", result);
  }

  [Fact]
  public void BuildNameValue_SurnameOnly_ReturnsCorrectFormat()
  {
    string result = GedcomExport.BuildNameValueWrapper("", "Smith");
    Assert.Equal("/Smith/", result);
  }

  [Fact]
  public void BuildNameValue_FirstNameOnly_ReturnsCorrectFormat()
  {
    string result = GedcomExport.BuildNameValueWrapper("John", "");
    Assert.Equal("John //", result);
  }

  [Fact]
  public void BuildNameValue_BothEmpty_ReturnsSlashes()
  {
    string result = GedcomExport.BuildNameValueWrapper("", "");
    Assert.Equal("//", result);
  }

  [Fact]
  public void BuildNameValue_NullValues_ReturnsSlashes()
  {
    string result = GedcomExport.BuildNameValueWrapper(null, null);
    Assert.Equal("//", result);
  }

  [Fact]
  public void ConvertNameTypeToGedcom_ReturnsUppercase()
  {
    Assert.Equal("AKA", GedcomExport.ConvertNameTypeToGedcomWrapper(NameType.Aka));
    Assert.Equal("BIRTH", GedcomExport.ConvertNameTypeToGedcomWrapper(NameType.Birth));
    Assert.Equal("MARRIED", GedcomExport.ConvertNameTypeToGedcomWrapper(NameType.Married));
    Assert.Equal("MAIDEN", GedcomExport.ConvertNameTypeToGedcomWrapper(NameType.Maiden));
    Assert.Equal("IMMIGRANT", GedcomExport.ConvertNameTypeToGedcomWrapper(NameType.Immigration));
    Assert.Equal("PROFESSIONAL", GedcomExport.ConvertNameTypeToGedcomWrapper(NameType.Professional));
    Assert.Equal("OTHER", GedcomExport.ConvertNameTypeToGedcomWrapper(NameType.Other));
  }
}

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

public class GedcomIdMapTest
{
  [Fact]
  public void Get()
  {
    GedcomIdMap sut = new();
    string personId = Guid.NewGuid().ToString();
    Assert.Equal("I0", sut.Get(Guid.NewGuid().ToString()));
    Assert.Equal("I1", sut.Get(Guid.NewGuid().ToString()));
    Assert.Equal("I2", sut.Get(Guid.NewGuid().ToString()));
    Assert.Equal("I3", sut.Get(personId));
    Assert.Equal("I4", sut.Get(Guid.NewGuid().ToString()));
    Assert.Equal("I5", sut.Get(Guid.NewGuid().ToString()));
    Assert.Equal("I3", sut.Get(personId));
  }
}

public class FamilyTest
{
  [Fact]
  public void Family()
  {
    Family sut = new(new(), new())!;

    Assert.NotNull(sut);
  }
}
