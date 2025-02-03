namespace FamilyShowLib.Tests;

public class HtmlExportTest
{
  [Fact]
  public void ExportEventsByDecade_WithValidData_ShouldGenerateHtml()
  {
    // Arrange
    var peopleCollection = new PeopleCollection
        {
            new Person("John", "Doe") { BirthDate = new DateTime(1890, 7, 22), DeathDate = new DateTime(1995, 1, 22) },
            new Person("Jane", "Doe") { BirthDate = new DateTime(1900, 5, 15), DeathDate = new DateTime(1980, 3, 10) }
        };
    var htmlExport = new HtmlExport();
    string htmlFilePath = Path.GetTempFileName();
    string familyxFileName = "FamilyTree";

    // Act
    htmlExport.ExportEventsByDecade(peopleCollection, htmlFilePath, familyxFileName, false, 1880, 2000);

    // Assert
    string htmlContent = File.ReadAllText(Path.GetFileName(htmlFilePath));
    Assert.Contains("John Doe", htmlContent);
    Assert.Contains("Jane Doe", htmlContent);
    Assert.Contains("1890-1899", htmlContent);
    Assert.Contains("1900-1909", htmlContent);
    Assert.Contains("1990-1999", htmlContent);
  }

  [Fact]
  public void ExportEventsByDecade_WithPrivacy_ShouldHideLivingPeople()
  {
    // Arrange
    var peopleCollection = new PeopleCollection
        {
            new Person("John", "Doe") { BirthDate = new DateTime(1890, 7, 22), DeathDate = new DateTime(1995, 1, 22), IsLiving = false },
            new Person("Jane", "Doe") { BirthDate = new DateTime(1900, 5, 15), IsLiving = true }
        };
    var htmlExport = new HtmlExport();
    string htmlFilePath = Path.GetTempFileName();
    string familyxFileName = "FamilyTree";

    // Act
    htmlExport.ExportEventsByDecade(peopleCollection, htmlFilePath, familyxFileName, true, 1880, 2000);

    // Assert
    string htmlContent = File.ReadAllText(Path.GetFileName(htmlFilePath));
    Assert.Contains("John Doe", htmlContent);
    Assert.DoesNotContain("Jane Doe", htmlContent);
  }

  [Fact]
  public void ExportEventsByDecade_WithEmptyCollection_ShouldGenerateEmptyHtml()
  {
    // Arrange
    var peopleCollection = new PeopleCollection();
    var htmlExport = new HtmlExport();
    string htmlFilePath = Path.GetTempFileName();
    string familyxFileName = "FamilyTree";

    // Act
    htmlExport.ExportEventsByDecade(peopleCollection, htmlFilePath, familyxFileName, false, 1880, 2000);

    // Assert
    string htmlContent = File.ReadAllText(Path.GetFileName(htmlFilePath));
    Assert.Contains("Family.Show", htmlContent);
    Assert.DoesNotContain("John Doe", htmlContent);
    Assert.DoesNotContain("Jane Doe", htmlContent);
  }

  [Fact]
  public void ExportEventsByDecade_WithInvalidDateRange_ShouldGenerateEmptyHtml()
  {
    // Arrange
    var peopleCollection = new PeopleCollection
        {
            new Person("John", "Doe") { BirthDate = new DateTime(1890, 7, 22), DeathDate = new DateTime(1995, 1, 22) },
            new Person("Jane", "Doe") { BirthDate = new DateTime(1900, 5, 15), DeathDate = new DateTime(1980, 3, 10) }
        };
    var htmlExport = new HtmlExport();
    string htmlFilePath = Path.GetTempFileName();
    string familyxFileName = "FamilyTree";

    // Act
    htmlExport.ExportEventsByDecade(peopleCollection, htmlFilePath, familyxFileName, false, 2000, 2100);

    // Assert
    string htmlContent = File.ReadAllText(Path.GetFileName(htmlFilePath));
    Assert.Contains("Family.Show", htmlContent);
    Assert.DoesNotContain("John Doe", htmlContent);
    Assert.DoesNotContain("Jane Doe", htmlContent);
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
