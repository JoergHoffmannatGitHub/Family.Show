using System.Xml;

namespace FamilyShowLib.Tests;

public class GedcomImportTest
{
  [StaFact]
  public void Import()
  {
    // Arrange
    GedcomImport sut = new();
    PeopleCollection family = [];
    SourceCollection source = [];
    RepositoryCollection repository = [];

    // Act
    bool loaded = sut.Import(family, source, repository, Sample.FullName("Kennedy.ged"), true);

    // Assert
    Assert.NotNull(sut);
    Assert.True(loaded);
    Assert.Equal(69, family.Count);
    Assert.Equal(11, source.Count);
    Assert.Empty(repository);
  }

  [Fact]
  public void ImportMarriage_WithValidMarriageData_ShouldAddMarriageToBothSpouses()
  {
    // Arrange
    var husband = new Person("John", "Doe", Gender.Male);
    var wife = new Person("Jane", "Doe", Gender.Female);
    string xml = @"
                <root>
                    <FAM>
                        <MARR>
                            <DATE Value='1 JAN 2000' />
                            <PLAC Value='New York' />
                            <SOUR Value='@S1@'>
                                <PAGE Value='Page 1' />
                                <DATA>
                                    <TEXT Value='Marriage Certificate' />
                                </DATA>
                                <NOTE Value='Marriage Note' />
                                <OBJE>
                                    <FORM Value='URL' />
                                    <TITL Value='http://marriage-link.com' />
                                </OBJE>
                            </SOUR>
                        </MARR>
                    </FAM>
                </root>";
    var doc = new XmlDocument();
    doc.LoadXml(xml);
    XmlNode? node = doc.SelectSingleNode("/root/FAM");

    // Act
    GedcomImport.ImportMarriage(husband, wife, node, doc);

    // Assert
    SpouseRelationship husbandMarriage = husband.GetSpouseRelationship(wife);
    SpouseRelationship wifeMarriage = wife.GetSpouseRelationship(husband);

    Assert.NotNull(husbandMarriage);
    Assert.NotNull(wifeMarriage);
    Assert.Equal(new(2000, 1, 1), husbandMarriage.MarriageDate);
    Assert.Equal("New York", husbandMarriage.MarriagePlace);
    Assert.Equal("@S1@", husbandMarriage.MarriageSource);
    Assert.Equal("Page 1", husbandMarriage.MarriageCitation);
    Assert.Equal("Marriage Certificate", husbandMarriage.MarriageCitationActualText);
    Assert.Equal("Marriage Note", husbandMarriage.MarriageCitationNote);
    Assert.Equal("http://marriage-link.com", husbandMarriage.MarriageLink);
  }

  [Fact]
  public void ImportMarriage_WithValidDivorceData_ShouldAddDivorceToBothSpouses()
  {
    // Arrange
    var husband = new Person("John", "Doe", Gender.Male);
    var wife = new Person("Jane", "Doe", Gender.Female);
    string xml = @"
                <root>
                    <FAM>
                        <DIV Value='Y'>
                            <DATE Value='1 JAN 2010' />
                            <SOUR Value='@S2@'>
                                <PAGE Value='Page 2' />
                                <DATA>
                                    <TEXT Value='Divorce Certificate' />
                                </DATA>
                                <NOTE Value='Divorce Note' />
                                <OBJE>
                                    <FORM Value='URL' />
                                    <TITL Value='http://divorce-link.com' />
                                </OBJE>
                            </SOUR>
                        </DIV>
                    </FAM>
                </root>";
    var doc = new XmlDocument();
    doc.LoadXml(xml);
    XmlNode? node = doc.SelectSingleNode("/root/FAM");

    // Act
    GedcomImport.ImportMarriage(husband, wife, node, doc);

    // Assert
    SpouseRelationship husbandMarriage = husband.GetSpouseRelationship(wife);
    SpouseRelationship wifeMarriage = wife.GetSpouseRelationship(husband);

    Assert.NotNull(husbandMarriage);
    Assert.NotNull(wifeMarriage);
    Assert.Equal(new(2010, 1, 1), husbandMarriage.DivorceDate);
    Assert.Equal("@S2@", husbandMarriage.DivorceSource);
    Assert.Equal("Page 2", husbandMarriage.DivorceCitation);
    Assert.Equal("Divorce Certificate", husbandMarriage.DivorceCitationActualText);
    Assert.Equal("Divorce Note", husbandMarriage.DivorceCitationNote);
    Assert.Equal("http://divorce-link.com", husbandMarriage.DivorceLink);
  }

  [Fact]
  public void ImportMarriage_WithNoMarriageOrDivorceData_ShouldAddCurrentSpouseRelationship()
  {
    // Arrange
    var husband = new Person("John", "Doe", Gender.Male);
    var wife = new Person("Jane", "Doe", Gender.Female);
    string xml = @"
                <root>
                    <FAM>
                    </FAM>
                </root>";
    var doc = new XmlDocument();
    doc.LoadXml(xml);
    XmlNode? node = doc.SelectSingleNode("/root/FAM");

    // Act
    GedcomImport.ImportMarriage(husband, wife, node, doc);

    // Assert
    SpouseRelationship husbandMarriage = husband.GetSpouseRelationship(wife);
    SpouseRelationship wifeMarriage = wife.GetSpouseRelationship(husband);

    Assert.NotNull(husbandMarriage);
    Assert.NotNull(wifeMarriage);
    Assert.Equal(SpouseModifier.Current, husbandMarriage.SpouseModifier);
    Assert.Equal(SpouseModifier.Current, wifeMarriage.SpouseModifier);
  }

  [Fact]
  public void ImportMarriage_WithNullWife_ShouldNotAddMarriage()
  {
    // Arrange
    var husband = new Person("John", "Doe", Gender.Male);
    Person? wife = null;
    string xml = @"
                <root>
                    <FAM>
                        <MARR>
                            <DATE Value='1 JAN 2000' />
                            <PLAC Value='New York' />
                        </MARR>
                    </FAM>
                </root>";
    var doc = new XmlDocument();
    doc.LoadXml(xml);
    XmlNode? node = doc.SelectSingleNode("/root/FAM");

    // Act
    GedcomImport.ImportMarriage(husband, wife, node, doc);

    // Assert
    SpouseRelationship husbandMarriage = husband.GetSpouseRelationship(wife);
    Assert.Null(husbandMarriage);
  }

  [Fact]
  public void ImportMarriage_WithNullHusband_ShouldNotAddMarriage()
  {
    // Arrange
    Person? husband = null;
    var wife = new Person("Jane", "Doe", Gender.Female);
    string xml = @"
                <root>
                    <FAM>
                        <MARR>
                            <DATE Value='1 JAN 2000' />
                            <PLAC Value='New York' />
                        </MARR>
                    </FAM>
                </root>";
    var doc = new XmlDocument();
    doc.LoadXml(xml);
    XmlNode? node = doc.SelectSingleNode("/root/FAM");

    // Act
    GedcomImport.ImportMarriage(husband, wife, node, doc);

    // Assert
    SpouseRelationship wifeMarriage = wife.GetSpouseRelationship(husband);
    Assert.Null(wifeMarriage);
  }

  [Fact]
  public void GetValueDate_WithValidDate_ShouldReturnCorrectDate()
  {
    // Arrange
    string xmlContent = "<root><DATE Value='12 SEP 1953' /></root>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;

    // Act
    DateWrapper result = GedcomImport.GetValueDate(node!, "DATE");

    // Assert
    Assert.Equal(new(1953, 9, 12), result);
  }

  [Fact]
  public void GetValueDate_WithApproximateDate_ShouldReturnCorrectDate()
  {
    // Arrange
    string xmlContent = "<root><DATE Value='ABT 1953' /></root>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;

    // Act
    DateWrapper result = GedcomImport.GetValueDate(node!, "DATE");

    // Assert
    Assert.Equal(new("ABT 1953"), result);
  }

  [Fact]
  public void GetValueDate_WithAfterDate_ShouldReturnCorrectDate()
  {
    // Arrange
    string xmlContent = "<root><DATE Value='AFT 1953' /></root>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;

    // Act
    DateWrapper result = GedcomImport.GetValueDate(node!, "DATE");

    // Assert
    Assert.Equal(new("AFT 1953"), result);
  }

  [Fact]
  public void GetValueDate_WithBeforeDate_ShouldReturnCorrectDate()
  {
    // Arrange
    string xmlContent = "<root><DATE Value='BEF 1953' /></root>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;

    // Act
    DateWrapper result = GedcomImport.GetValueDate(node!, "DATE");

    // Assert
    Assert.Equal(new("BEF 1953"), result);
  }

  [Theory]
  [InlineData("JAN-FEB-MAR 1953")]
  [InlineData("APR-MAY-JUN 1953")]
  [InlineData("JUL-AUG-SEP 1953")]
  [InlineData("OCT-NOV-DEC 1953")]
  [InlineData("JAN FEB MAR 1953")]
  [InlineData("APR MAY JUN 1953")]
  [InlineData("JUL AUG SEP 1953")]
  [InlineData("OCT NOV DEC 1953")]
  public void GetValueDate_WithQuarterDate_ShouldReturnCorrectDate_FixIt(string quarter)
  {
    // Arrange
    // see https://github.com/JoergHoffmannatGitHub/Family.Show/issues/4
    //string xmlContent = "<root><DATE Value='BET JAN 1953 AND MAR 1953' /></root>";
    string xmlContent = "<root><DATE Value='" + quarter + "' /></root>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;

    // Act
    DateWrapper result = GedcomImport.GetValueDate(node!, "DATE");

    // Assert
    Assert.Null(result);
  }

  [Fact]
  public void GetValueDate_WithInvalidDate_ShouldReturnNull_FixIt()
  {
    // Arrange
    string xmlContent = "<root><DATE Value='INVALID DATE' /></root>";
    //string xmlContent = "<root></root>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;

    // Act
    DateWrapper result = GedcomImport.GetValueDate(node!, "DATE");

    // Assert
    Assert.Null(result);
  }

  [Fact]
  public void GetValueDate_WithEmptyDate_ShouldReturnNull()
  {
    // Arrange
    string xmlContent = "<root><DATE Value='' /></root>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;

    // Act
    DateWrapper result = GedcomImport.GetValueDate(node!, "DATE");

    // Assert
    Assert.Null(result);
  }

  [Fact]
  public void GetValueDate_WithOutDate()
  {
    // Arrange
    string xmlContent = "<root><PLAC Value='some place' /></root>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;
    Assert.NotNull(node);

    // Act
    DateWrapper result = GedcomImport.GetValueDate(node!, "DATE");

    // Assert
    Assert.Null(result);
  }

  [Fact]
  public void GetValueDate_WithDateRange_FixIt()
  {
    // Arrange
    // see https://github.com/JoergHoffmannatGitHub/Family.Show/issues/4
    //string xmlContent = "<root><DATE Value='BET 1982 AND 1984' /></root>";
    string xmlContent = "<root><DATE Value='1983' /></root>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;
    Assert.NotNull(node);

    // Act
    DateWrapper result = GedcomImport.GetValueDate(node!, "DATE");

    // Assert
    Assert.Equal(new(1983), result);
  }

  [Fact]
  public void GetValueDate_WithDateEstimated_FixIt()
  {
    // Arrange
    // see https://github.com/JoergHoffmannatGitHub/Family.Show/issues/4
    //string xmlContent = "<root><DATE Value=EST 1752' /></root>";
    string xmlContent = "<root><DATE Value='1752' /></root>";
    XmlDocument doc = new();
    doc.LoadXml(xmlContent);
    XmlNode? node = doc.DocumentElement;
    Assert.NotNull(node);

    // Act
    DateWrapper result = GedcomImport.GetValueDate(node!, "DATE");

    // Assert
    Assert.Equal(new(1752), result);
  }
}
