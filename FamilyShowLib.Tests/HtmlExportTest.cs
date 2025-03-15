namespace FamilyShowLib.Tests;

public class HtmlExportTest
{
  private readonly Person _livingWoman = new("Jane", "Livinig")
  {
    BirthDate = new DateTime(1900, 5, 15)
  };
  private readonly Person _deceasedMan = new("John", "Death")
  {
    BirthDate = new DateTime(1890, 7, 22),
    DeathDate = new DateTime(1995, 1, 22),
    DeathPlace = "Somewhere",
    IsLiving = false
  };
  private readonly Person _deceasedWoman = new("Jane", "Death")
  {
    BirthDate = new DateTime(1900, 5, 15),
    BirthPlace = "Somewhere",
    DeathDate = new DateTime(1980, 3, 10),
    IsLiving = false
  };
  private readonly Person _buriedMan = new("John", "Burial")
  {
    BirthDate = new DateTime(1890, 7, 22),
    BurialDate = new DateTime(1995, 1, 22),
    BurialPlace = "Somewhere",
    IsLiving = false
  };
  private readonly Person _buriedWoman = new("Jane", "Burial")
  {
    BirthDate = new DateTime(1900, 5, 15),
    BirthPlace = "Somewhere",
    BurialDate = new DateTime(1980, 3, 10),
    IsLiving = false
  };
  private readonly Person _crematedMan = new("John", "Cremation")
  {
    BirthDate = new DateTime(1890, 7, 22),
    CremationDate = new DateTime(1995, 1, 22),
    CremationPlace = "Somewhere",
    IsLiving = false
  };
  private readonly Person _crematedWoman = new("Jane", "Cremation")
  {
    BirthDate = new DateTime(1900, 5, 15),
    BirthPlace = "Somewhere",
    CremationDate = new DateTime(1980, 3, 10),
    IsLiving = false
  };

  private static PeopleCollection CreateMariedPeopleCollection()
  {
    Person husband = new("John", "Doe") { BirthDate = new DateTime(1970, 7, 22), Gender = Gender.Male };
    Person wife = new("Jane", "Doe") { BirthDate = new DateTime(1980, 5, 15), Gender = Gender.Female };
    PeopleCollection peopleCollection = [husband, wife];
    peopleCollection.Current = husband;
    Marry(husband, wife);
    return peopleCollection;
  }

  private static string GetHtmlContent(string htmlFilePath)
  {
    return File.ReadAllText(Path.GetFileName(htmlFilePath));
  }

  private static void Marry(Person husband, Person wife)
  {
    SpouseRelationship spouseRelationship = new(husband, SpouseModifier.Current);
    wife.Relationships.Add(spouseRelationship);
    spouseRelationship = new(wife, SpouseModifier.Current);
    husband.Relationships.Add(spouseRelationship);
  }

  [Fact]
  public void ExportDirect_WithValidData_ShouldGenerateHtml()
  {
    // Arrange
    PeopleCollection peopleCollection = CreateMariedPeopleCollection();
    SourceCollection sourceCollection = [];
    RepositoryCollection repositoryCollection = [];
    HtmlExport htmlExport = new();
    string htmlFilePath = Path.GetTempFileName();
    string familyxFileName = "FamilyTree";

    // Act
    htmlExport.ExportDirect(peopleCollection, sourceCollection, repositoryCollection, htmlFilePath, familyxFileName, false, false);

    // Assert
    string htmlContent = GetHtmlContent(htmlFilePath);
    Assert.Contains("<b>John Doe</b>", htmlContent);
    Assert.Contains("class=\"person\"><td>", htmlContent);
    Assert.Contains(Properties.Resources.Spouse, htmlContent);
    Assert.Contains("<td>Jane</td><td>Doe</td>", htmlContent);
    Assert.Contains("<td> 15/5/1980</td>", htmlContent);
    Assert.DoesNotContain("class=\"notelink\"", htmlContent);
  }

  [Fact]
  public void ExportDirect_WithValidDataWithNote_ShouldGenerateHtml()
  {
    // Arrange
    PeopleCollection peopleCollection = CreateMariedPeopleCollection();
    peopleCollection.Current.Spouses[0].Note = "Jane is married with John!";
    SourceCollection sourceCollection = [];
    RepositoryCollection repositoryCollection = [];
    HtmlExport htmlExport = new();
    string htmlFilePath = Path.GetTempFileName();
    string familyxFileName = "FamilyTree";

    // Act
    htmlExport.ExportDirect(peopleCollection, sourceCollection, repositoryCollection, htmlFilePath, familyxFileName, false, false);

    // Assert
    string htmlContent = GetHtmlContent(htmlFilePath);
    Assert.Contains("<b>John Doe</b>", htmlContent);
    Assert.Contains("class=\"personhighlight\"><td>", htmlContent);
    Assert.Contains(Properties.Resources.Spouse, htmlContent);
    Assert.Contains("<td>Jane</td><td>Doe</td>", htmlContent);
    Assert.Contains("<td> 15/5/1980</td>", htmlContent);
    Assert.Contains("class=\"notelink\"", htmlContent);
    Assert.Contains("Jane is married with John!", htmlContent);
  }

  [Fact]
  public void ExportEventsByDecade_WithValidData_ShouldGenerateHtml()
  {
    // Arrange
    PeopleCollection peopleCollection = [_deceasedMan, _deceasedWoman, _buriedMan, _buriedWoman, _crematedMan, _crematedWoman];
    HtmlExport htmlExport = new();
    string htmlFilePath = Path.GetTempFileName();
    string familyxFileName = "FamilyTree";

    // Act
    htmlExport.ExportEventsByDecade(peopleCollection, htmlFilePath, familyxFileName, false, 1880, 2000);

    // Assert
    string htmlContent = GetHtmlContent(htmlFilePath);
    Assert.Contains("John Death", htmlContent);
    Assert.Contains("Jane Death", htmlContent);
    Assert.Contains("John Burial", htmlContent);
    Assert.Contains("Jane Burial", htmlContent);
    Assert.Contains("John Cremation", htmlContent);
    Assert.Contains("Jane Cremation", htmlContent);
    Assert.Contains("1890-1899", htmlContent);
    Assert.Contains("1900-1909", htmlContent);
    Assert.Contains("1990-1999", htmlContent);
  }

  [Fact]
  public void ExportEventsByDecade_WithPrivacy_ShouldHideLivingPeople()
  {
    // Arrange
    PeopleCollection peopleCollection = [_deceasedMan, _livingWoman];
    HtmlExport htmlExport = new();
    string htmlFilePath = Path.GetTempFileName();
    string familyxFileName = "FamilyTree";

    // Act
    htmlExport.ExportEventsByDecade(peopleCollection, htmlFilePath, familyxFileName, true, 1880, 2000);

    // Assert
    string htmlContent = GetHtmlContent(htmlFilePath);
    Assert.Contains("John Death", htmlContent);
    Assert.DoesNotContain("Jane Livinig", htmlContent);
  }

  [Fact]
  public void ExportEventsByDecade_WithEmptyCollection_ShouldGenerateEmptyHtml()
  {
    // Arrange
    PeopleCollection peopleCollection = new();
    HtmlExport htmlExport = new();
    string htmlFilePath = Path.GetTempFileName();
    string familyxFileName = "FamilyTree";

    // Act
    htmlExport.ExportEventsByDecade(peopleCollection, htmlFilePath, familyxFileName, false, 1880, 2000);

    // Assert
    string htmlContent = GetHtmlContent(htmlFilePath);
    Assert.Contains("Family.Show", htmlContent);
    Assert.DoesNotContain("John Death", htmlContent);
    Assert.DoesNotContain("Jane Doe", htmlContent);
  }

  [Fact]
  public void ExportEventsByDecade_WithInvalidDateRange_ShouldGenerateEmptyHtml()
  {
    // Arrange
    PeopleCollection peopleCollection = [_deceasedMan, _deceasedWoman];
    HtmlExport htmlExport = new();
    string htmlFilePath = Path.GetTempFileName();
    string familyxFileName = "FamilyTree";

    // Act
    htmlExport.ExportEventsByDecade(peopleCollection, htmlFilePath, familyxFileName, false, 2000, 2100);

    // Assert
    string htmlContent = GetHtmlContent(htmlFilePath);
    Assert.Contains("Family.Show", htmlContent);
    Assert.DoesNotContain("John Death", htmlContent);
    Assert.DoesNotContain("Jane Death", htmlContent);
  }

  [Fact]
  public void Dateformat_WithNullDate_ShouldReturnEmptyString()
  {
    // Act
    string result = HtmlExport.dateformat(null);

    // Assert
    Assert.Equal(string.Empty, result);
  }


  [Fact]
  public void Dateformat_WithValidDate_ShouldReturnFormattedDate()
  {
    // Arrange
    DateTime date = new(2023, 10, 5);

    // Act
    string result = HtmlExport.dateformat(date);

    // Assert
    Assert.Equal("5/10/2023", result);
  }
}
