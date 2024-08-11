using System.Xml;

namespace FamilyShowLib.Tests;

public class GedcomImportTest
{
  [StaFact]
  public void Import()
  {
    GedcomImport sut = new();
    PeopleCollection family = [];
    SourceCollection source = [];
    RepositoryCollection repository = [];
    bool loaded = sut.Import(family, source, repository, Sample.FullName("Kennedy.ged"), true);

    Assert.NotNull(sut);
    Assert.True(loaded);
    Assert.Equal(69, family.Count);
    Assert.Equal(11, source.Count);
    Assert.Empty(repository);
  }

  [StaFact]
  public void ImportDate()
  {
    string xmlContent = "<FAM Value=\"@F13@\">" +
      "  <MARR Value=\"\">" +
      "    <DATE Value=\"12 SEP 1953\" />" +
      "    <PLAC Value=\"Newport, RI\" />" +
      "  </MARR>" +
      "</FAM>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;
    Assert.NotNull(node);
    DateTime? marriageDate = GedcomImport.GetValueDateWrapper(node!, "MARR/DATE");

    Assert.Equal(new DateTime(1953, 9, 12), marriageDate);
  }

  [StaFact]
  public void ImportDateRange()
  {
    string xmlContent = "<INDI Value=\"@I0120@\">" +
      "  <DEAT Value=\"\">" +
      "    <DATE Value=\"1982\" />" +
      // see https://github.com/JoergHoffmannatGitHub/Family.Show/issues/4
      //"    <DATE Value=\"BET 1982 AND 1984\" />" +
      "  </DEAT>" +
      "</INDI>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;
    Assert.NotNull(node);
    DateTime? deatDate = GedcomImport.GetValueDateWrapper(node!, "DEAT/DATE");

    Assert.Equal(new DateTime(1982, 1, 1), deatDate);
  }

  [StaFact]
  public void ImportDateEstimate()
  {
    string xmlContent = "<INDI Value=\"@I0228@\">" +
      "  <BIRT Value=\"\">" +
      "    <DATE Value=\"1752\" />" +
      // see https://github.com/JoergHoffmannatGitHub/Family.Show/issues/4
      //"    <DATE Value=\"EST 1752\" />" +
      "  </BIRT>" +
      "</INDI>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;
    Assert.NotNull(node);
    DateTime? deatDate = GedcomImport.GetValueDateWrapper(node!, "BIRT/DATE");

    Assert.Equal(new DateTime(1752, 1, 1), deatDate);
  }

  [StaFact]
  public void ImportDateOnlyPlace()
  {
    string xmlContent = "<INDI Value=\"@I0560@\">" +
      "  <BIRT Value=\"\">" +
      "    <DATE Value=\"1752\" />" +
      // see https://github.com/JoergHoffmannatGitHub/Family.Show/issues/4
      "    <PLAC Value=\"some place\" />" +
      "  </BIRT>" +
      "</INDI>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;
    Assert.NotNull(node);
    DateTime? deatDate = GedcomImport.GetValueDateWrapper(node!, "BIRT/DATE");

    Assert.Equal(new DateTime(1752, 1, 1), deatDate);
  }
}
