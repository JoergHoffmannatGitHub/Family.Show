using System.Xml;

namespace FamilyShowLib.Tests;

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
    Assert.Equal("Dr. John von Smith Jr.", name.FullName);
    Assert.Equal("John", name.FirstName);
    Assert.Equal("Smith", name.Surname);
    Assert.Equal("Dr.", name.Prefix);
    Assert.Equal("Jr.", name.Suffix);
    Assert.Equal("von", name.SurnamePrefix);
    Assert.Equal(NameType.Birth, name.NameType);
    Assert.True(name.IsPrimary);
  }

  [StaFact]
  public void ImportName_MultiLanguageExamples_SetsPropertiesCorrectly()
  {
    string xmlContent = @"<INDI Value=""@I1@"">
      <NAME Value=""John /Smith/"">
        <SURN Value=""Smith"" />
        <NPFX Value=""Dr"" />
        <NSFX Value=""Jr"" />
        <SPFX Value=""von"" />
        <LANG Value=""English"" />
      </NAME>
      <SEX Value=""M"" />
      <NAME Value=""John /Schmidt/"">
        <NPFX Value=""Dr."" />
        <SPFX Value=""von"" />
        <NSFX Value=""jun."" />
        <LANG Value=""Deutsch"" />
      </NAME>

    </INDI>";

    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode node = doc.DocumentElement!;

    // Act
    Person person = new();
    GedcomImport.ImportNamesWrapper(person, node);

    // English
    Name name = person.Names[0];
    Assert.Equal("Dr John von Smith Jr", name.FullName);
    Assert.Equal("John", name.FirstName);
    Assert.Equal("Smith", name.Surname);
    Assert.Equal("Dr", name.Prefix);
    Assert.Equal("Jr", name.Suffix);
    Assert.Equal("von", name.SurnamePrefix);
    Assert.Equal(NameType.Birth, name.NameType);
    Assert.True(name.IsPrimary);

    // Deutsch
    name = person.Names[1];
    Assert.Equal("Dr. John von Schmidt jun.", name.FullName);
    Assert.Equal("John", name.FirstName);
    Assert.Equal("Schmidt", name.Surname);
    Assert.Equal("Dr.", name.Prefix);
    Assert.Equal("jun.", name.Suffix);
    Assert.Equal("von", name.SurnamePrefix);
    Assert.Equal(NameType.Birth, name.NameType);
    Assert.False(name.IsPrimary);

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
