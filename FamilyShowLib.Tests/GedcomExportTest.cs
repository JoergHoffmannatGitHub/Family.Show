using System.Globalization;
using System.Text;

using Microsoft.Extensions.Time.Testing;

namespace FamilyShowLib.Tests;

public class GedcomExportTest
{
  [Theory, CombinatorialData]
  public void ExportSummaryTest(
    [CombinatorialValues("Kennedy.ged", "")] string filename,
    [CombinatorialValues("Kennedy.familyx", "")] string nameOfSourceDate,
    [CombinatorialValues("en-US", "en-GB", "it-IT", "es-ES", "fr-FR", "de-DE", "somthing else")] string languageOfText)
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      string gedcomFilePath = @"C:\SomePath\" + filename;
      string familyxFilePath = string.IsNullOrEmpty(nameOfSourceDate) ? string.Empty : @"C:\SomePath\" + nameOfSourceDate;
      var fakeTimeProvider = new FakeTimeProvider();
      fakeTimeProvider.SetUtcNow(new DateTime(2025, 1, 18, 16, 08, 35));
      fakeTimeProvider.SetLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"));
      GedcomExport gedcomExport = new();
      using MemoryStream memoryStream = new();
      gedcomExport._writer = new(memoryStream);
      gedcomExport._timeProvider = fakeTimeProvider;

      // Act
      gedcomExport.ExportSummary(gedcomFilePath, familyxFilePath, languageOfText);

      // Assert
      gedcomExport._writer.Flush();
      string actual = Encoding.UTF8.GetString(memoryStream.ToArray());
      Assert.NotEmpty(actual);
      Assert.Contains(FormatLine(1, "SOUR", string.Empty), actual);
      // Don't ccheck the versionnumber
      Assert.Contains("2 VERS", actual);
      Assert.Contains(FormatLine(2, "NAME", "Family.Show"), actual);
      Assert.Contains(FormatLine(2, "CORP", "Microsoft"), actual);
      Assert.Equal(!string.IsNullOrEmpty(nameOfSourceDate), actual.Contains(FormatLine(2, "DATA", nameOfSourceDate)));
      Assert.Contains(FormatLine(1, "DATE", "18 JAN 2025"), actual);
      Assert.Contains(FormatLine(2, "TIME", "5:08:35 PM"), actual);
      Assert.Contains(FormatLine(1, "FILE", filename), actual);
      Assert.Contains(FormatLine(1, "GEDC", string.Empty), actual);
      Assert.Contains(FormatLine(2, "VERS", "5.5"), actual);
      Assert.Contains(FormatLine(2, "FORM", "LINEAGE-LINKED"), actual);
      Assert.Contains(FormatLine(1, "CHAR", "UTF-8"), actual);
      Assert.Contains(FormatLine(1, "LANG", gedcomExport._culturLanguageIdMap.GetValueOrDefault(languageOfText, "English")), actual);
    }
  }

  private static string FormatLine(int level, string tag, string value) =>
    string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}" + Environment.NewLine, level, tag, value);

  [Theory]
  [InlineData("BIRT", "Birth", "About", "2025-01-18", "New York", "Citation1", "Note1", "ActualText1", "http://link1.com", "Source1")]
  [InlineData("DEAT", "Death", "Before", "2020-05-10", "Los Angeles", "Citation2", "Note2", "ActualText2", "http://link2.com", "Source2")]
  [InlineData("MARR", "Marriage", "After", "2015-08-25", "Chicago", "Citation3", "Note3", "ActualText3", "http://link3.com", "Source3")]
  public void ExportEvent_WithValidData_WritesExpectedOutput(
    string tag, string tagDescription, string descriptor, string dateString, string place, string citation, string citationNote, string citationActualText, string link, string source)
  {
    // Arrange
    DateWrapper date = new(dateString);
    GedcomExport gedcomExport = new()
    {
      _sources = []
    };
    gedcomExport._sources.Add(new Source(source, "sourceName", "sourceAuthor", "sourcePublisher", "sourceNote", "sourceRepository"));
    using MemoryStream memoryStream = new();
    gedcomExport._writer = new(memoryStream);

    // Act
    gedcomExport.ExportEvent(tag, tagDescription, descriptor, date, place, citation, citationNote, citationActualText, link, source);

    // Assert
    gedcomExport._writer.Flush();
    string actual = Encoding.UTF8.GetString(memoryStream.ToArray());
    Assert.Contains(FormatLine(1, tag, tagDescription), actual);
    Assert.Contains(FormatLine(2, "DATE", descriptor + GedcomExport.ExportDate(date)), actual);
    Assert.Contains(FormatLine(2, "PLAC", place), actual);
    Assert.Contains(FormatLine(2, "SOUR", "@" + source + "@"), actual);
    Assert.Contains(FormatLine(3, "PAGE", citation), actual);
    Assert.Contains(FormatLine(3, "DATA", string.Empty), actual);
    Assert.Contains(FormatLine(4, "TEXT", citationActualText), actual);
    Assert.Contains(FormatLine(3, "NOTE", citationNote + " " + link), actual);
    Assert.Contains(FormatLine(3, "OBJE", string.Empty), actual);
    Assert.Contains(FormatLine(4, "FORM", "URL"), actual);
    Assert.Contains(FormatLine(4, "TITL", "URL of citation"), actual);
    Assert.Contains(FormatLine(4, "FILE", link), actual);
  }

  [Fact]
  public void ExportEvent_WithValidData_WritesExpectedOutput_WithoutPlace()
  {
    // Arrange
    string tag = "BURI";
    string tagDescription = "Burial";
    string descriptor = "About";
    DateWrapper date = new("2015-07-30"); ;
    string place = string.Empty;
    string citation = "Citation1";
    string citationNote = "Note1";
    string citationActualText = "ActualText1";
    string link = "http://link1.com";
    string source = "Source1";
    GedcomExport gedcomExport = new()
    {
      _sources = []
    };
    gedcomExport._sources.Add(new Source(source, "sourceName", "sourceAuthor", "sourcePublisher", "sourceNote", "sourceRepository"));
    using MemoryStream memoryStream = new();
    gedcomExport._writer = new(memoryStream);

    // Act
    gedcomExport.ExportEvent(tag, tagDescription, descriptor, date, place, citation, citationNote, citationActualText, link, source);

    // Assert
    gedcomExport._writer.Flush();
    string actual = Encoding.UTF8.GetString(memoryStream.ToArray());
    Assert.Contains(FormatLine(1, tag, tagDescription), actual);
    Assert.Contains(FormatLine(2, "DATE", descriptor + GedcomExport.ExportDate(date)), actual);
    Assert.DoesNotContain(FormatLine(2, "PLAC", place), actual);
    Assert.Contains(FormatLine(2, "SOUR", "@" + source + "@"), actual);
    Assert.Contains(FormatLine(3, "PAGE", citation), actual);
    Assert.Contains(FormatLine(3, "DATA", string.Empty), actual);
    Assert.Contains(FormatLine(4, "TEXT", citationActualText), actual);
    Assert.Contains(FormatLine(3, "NOTE", citationNote + " " + link), actual);
    Assert.Contains(FormatLine(3, "OBJE", string.Empty), actual);
    Assert.Contains(FormatLine(4, "FORM", "URL"), actual);
    Assert.Contains(FormatLine(4, "TITL", "URL of citation"), actual);
    Assert.Contains(FormatLine(4, "FILE", link), actual);
  }

  [Fact]
  public void ExportEvent_WithValidData_WritesExpectedOutput_WithoutSource()
  {
    // Arrange
    string tag = "BURI";
    string tagDescription = "Burial";
    string descriptor = "About";
    DateWrapper date = new("2015-07-30"); ;
    string place = "New York";
    string citation = "Citation1";
    string citationNote = "Note1";
    string citationActualText = "ActualText1";
    string link = "http://link1.com";
    string source = string.Empty;
    GedcomExport gedcomExport = new();
    using MemoryStream memoryStream = new();
    gedcomExport._writer = new(memoryStream);

    // Act
    gedcomExport.ExportEvent(tag, tagDescription, descriptor, date, place, citation, citationNote, citationActualText, link, source);

    // Assert
    gedcomExport._writer.Flush();
    string actual = Encoding.UTF8.GetString(memoryStream.ToArray());
    Assert.Contains(FormatLine(1, tag, tagDescription), actual);
    Assert.Contains(FormatLine(2, "DATE", descriptor + GedcomExport.ExportDate(date)), actual);
    Assert.Contains(FormatLine(2, "PLAC", place), actual);
    Assert.DoesNotContain(FormatLine(2, "SOUR", "@" + source + "@"), actual);
    Assert.DoesNotContain(FormatLine(3, "PAGE", citation), actual);
    Assert.DoesNotContain(FormatLine(3, "DATA", string.Empty), actual);
    Assert.DoesNotContain(FormatLine(4, "TEXT", citationActualText), actual);
    Assert.DoesNotContain(FormatLine(3, "NOTE", citationNote + " " + link), actual);
    Assert.DoesNotContain(FormatLine(3, "OBJE", string.Empty), actual);
    Assert.DoesNotContain(FormatLine(4, "FORM", "URL"), actual);
    Assert.DoesNotContain(FormatLine(4, "TITL", "URL of citation"), actual);
    Assert.DoesNotContain(FormatLine(4, "FILE", link), actual);
  }

  [Fact]
  public void ExportEvent_WithValidData_WritesExpectedOutput_WithoutCitation()
  {
    // Arrange
    string tag = "BURI";
    string tagDescription = "Burial";
    string descriptor = "About";
    DateWrapper date = new("2015-07-30"); ;
    string place = "New York";
    string citation = string.Empty;
    string citationNote = "Note1";
    string citationActualText = "ActualText1";
    string link = "http://link1.com";
    string source = "Source1";
    GedcomExport gedcomExport = new()
    {
      _sources = []
    };
    gedcomExport._sources.Add(new Source(source, "sourceName", "sourceAuthor", "sourcePublisher", "sourceNote", "sourceRepository"));
    using MemoryStream memoryStream = new();
    gedcomExport._writer = new(memoryStream);

    // Act
    gedcomExport.ExportEvent(tag, tagDescription, descriptor, date, place, citation, citationNote, citationActualText, link, source);

    // Assert
    gedcomExport._writer.Flush();
    string actual = Encoding.UTF8.GetString(memoryStream.ToArray());
    Assert.Contains(FormatLine(1, tag, tagDescription), actual);
    Assert.Contains(FormatLine(2, "DATE", descriptor + GedcomExport.ExportDate(date)), actual);
    Assert.Contains(FormatLine(2, "PLAC", place), actual);
    Assert.DoesNotContain(FormatLine(2, "SOUR", "@" + source + "@"), actual);
    Assert.DoesNotContain(FormatLine(3, "PAGE", citation), actual);
    Assert.DoesNotContain(FormatLine(3, "DATA", string.Empty), actual);
    Assert.DoesNotContain(FormatLine(4, "TEXT", citationActualText), actual);
    Assert.DoesNotContain(FormatLine(3, "NOTE", citationNote + " " + link), actual);
    Assert.DoesNotContain(FormatLine(3, "OBJE", string.Empty), actual);
    Assert.DoesNotContain(FormatLine(4, "FORM", "URL"), actual);
    Assert.DoesNotContain(FormatLine(4, "TITL", "URL of citation"), actual);
    Assert.DoesNotContain(FormatLine(4, "FILE", link), actual);
  }

  [Fact]
  public void ExportEvent_WithValidData_WritesExpectedOutput_WithSources_WithoutCitationActualText()
  {
    // Arrange
    string tag = "BURI";
    string tagDescription = "Burial";
    string descriptor = "About";
    DateWrapper date = new("2015-07-30"); ;
    string place = "New York";
    string citation = "Citation1";
    string citationNote = "Note1";
    string citationActualText = string.Empty;
    string link = "http://link1.com";
    string source = "Source1";
    GedcomExport gedcomExport = new()
    {
      _sources = []
    };
    gedcomExport._sources.Add(new Source(source, "sourceName", "sourceAuthor", "sourcePublisher", "sourceNote", "sourceRepository"));
    using MemoryStream memoryStream = new();
    gedcomExport._writer = new(memoryStream);

    // Act
    gedcomExport.ExportEvent(tag, tagDescription, descriptor, date, place, citation, citationNote, citationActualText, link, source);

    // Assert
    gedcomExport._writer.Flush();
    string actual = Encoding.UTF8.GetString(memoryStream.ToArray());
    Assert.Contains(FormatLine(1, tag, tagDescription), actual);
    Assert.Contains(FormatLine(2, "DATE", descriptor + GedcomExport.ExportDate(date)), actual);
    Assert.Contains(FormatLine(2, "PLAC", place), actual);
    Assert.Contains(FormatLine(2, "SOUR", "@" + source + "@"), actual);
    Assert.Contains(FormatLine(3, "PAGE", citation), actual);
    Assert.Contains(FormatLine(3, "DATA", string.Empty), actual);
    Assert.DoesNotContain(FormatLine(4, "TEXT", citationActualText), actual);
    Assert.Contains(FormatLine(3, "NOTE", citationNote + " " + link), actual);
    Assert.Contains(FormatLine(3, "OBJE", string.Empty), actual);
    Assert.Contains(FormatLine(4, "FORM", "URL"), actual);
    Assert.Contains(FormatLine(4, "TITL", "URL of citation"), actual);
    Assert.Contains(FormatLine(4, "FILE", link), actual);
  }

  [Fact]
  public void ExportEvent_WithValidData_WritesExpectedOutput_WithSources_WithNoteWithoutLink()
  {
    // Arrange
    string tag = "BURI";
    string tagDescription = "Burial";
    string descriptor = "About";
    DateWrapper date = new("2015-07-30"); ;
    string place = "New York";
    string citation = "Citation1";
    string citationNote = "Note1";
    string citationActualText = "ActualText1";
    string link = string.Empty;
    string source = "Source1";
    GedcomExport gedcomExport = new()
    {
      _sources = []
    };
    gedcomExport._sources.Add(new Source(source, "sourceName", "sourceAuthor", "sourcePublisher", "sourceNote", "sourceRepository"));
    using MemoryStream memoryStream = new();
    gedcomExport._writer = new(memoryStream);

    // Act
    gedcomExport.ExportEvent(tag, tagDescription, descriptor, date, place, citation, citationNote, citationActualText, link, source);

    // Assert
    gedcomExport._writer.Flush();
    string actual = Encoding.UTF8.GetString(memoryStream.ToArray());
    Assert.Contains(FormatLine(1, tag, tagDescription), actual);
    Assert.Contains(FormatLine(2, "DATE", descriptor + GedcomExport.ExportDate(date)), actual);
    Assert.Contains(FormatLine(2, "PLAC", place), actual);
    Assert.Contains(FormatLine(2, "SOUR", "@" + source + "@"), actual);
    Assert.Contains(FormatLine(3, "PAGE", citation), actual);
    Assert.Contains(FormatLine(3, "DATA", string.Empty), actual);
    Assert.Contains(FormatLine(4, "TEXT", citationActualText), actual);
    Assert.Contains(FormatLine(3, "NOTE", citationNote), actual);
    Assert.DoesNotContain(FormatLine(3, "OBJE", string.Empty), actual);
    Assert.DoesNotContain(FormatLine(4, "FORM", "URL"), actual);
    Assert.DoesNotContain(FormatLine(4, "TITL", "URL of citation"), actual);
    Assert.DoesNotContain(FormatLine(4, "FILE", link), actual);
  }

  [Fact]
  public void ExportEvent_WithValidData_WritesExpectedOutput_WithSources_WithNoteWithoutCitationNote()
  {
    // Arrange
    string tag = "BURI";
    string tagDescription = "Burial";
    string descriptor = "About";
    DateWrapper date = new("2015-07-30"); ;
    string place = "New York";
    string citation = "Citation1";
    string citationNote = string.Empty;
    string citationActualText = "ActualText1";
    string link = "http://link1.com";
    string source = "Source1";
    GedcomExport gedcomExport = new()
    {
      _sources = []
    };
    gedcomExport._sources.Add(new Source(source, "sourceName", "sourceAuthor", "sourcePublisher", "sourceNote", "sourceRepository"));
    using MemoryStream memoryStream = new();
    gedcomExport._writer = new(memoryStream);

    // Act
    gedcomExport.ExportEvent(tag, tagDescription, descriptor, date, place, citation, citationNote, citationActualText, link, source);

    // Assert
    gedcomExport._writer.Flush();
    string actual = Encoding.UTF8.GetString(memoryStream.ToArray());
    Assert.Contains(FormatLine(1, tag, tagDescription), actual);
    Assert.Contains(FormatLine(2, "DATE", descriptor + GedcomExport.ExportDate(date)), actual);
    Assert.Contains(FormatLine(2, "PLAC", place), actual);
    Assert.Contains(FormatLine(2, "SOUR", "@" + source + "@"), actual);
    Assert.Contains(FormatLine(3, "PAGE", citation), actual);
    Assert.Contains(FormatLine(3, "DATA", string.Empty), actual);
    Assert.Contains(FormatLine(4, "TEXT", citationActualText), actual);
    Assert.Contains(FormatLine(3, "NOTE", link), actual);
    Assert.Contains(FormatLine(3, "OBJE", string.Empty), actual);
    Assert.Contains(FormatLine(4, "FORM", "URL"), actual);
    Assert.Contains(FormatLine(4, "TITL", "URL of citation"), actual);
    Assert.Contains(FormatLine(4, "FILE", link), actual);
  }

  [Fact]
  public void ExportEvent_WithValidData_WritesExpectedOutput_WithoutDate()
  {
    // Arrange
    string tag = "BURI";
    string tagDescription = "Burial";
    string descriptor = "About";
    DateWrapper? date = null;
    string place = "New York";
    string citation = "Citation1";
    string citationNote = "Note1";
    string citationActualText = "ActualText1";
    string link = "http://link1.com";
    string source = "Source1";
    GedcomExport gedcomExport = new()
    {
      _sources = []
    };
    gedcomExport._sources.Add(new Source(source, "sourceName", "sourceAuthor", "sourcePublisher", "sourceNote", "sourceRepository"));
    using MemoryStream memoryStream = new();
    gedcomExport._writer = new(memoryStream);

    // Act
    gedcomExport.ExportEvent(tag, tagDescription, descriptor, date, place, citation, citationNote, citationActualText, link, source);

    // Assert
    gedcomExport._writer.Flush();
    string actual = Encoding.UTF8.GetString(memoryStream.ToArray());
    Assert.Contains(FormatLine(1, tag, tagDescription), actual);
    Assert.Contains(FormatLine(2, "PLAC", place), actual);
    Assert.Contains(FormatLine(2, "SOUR", "@" + source + "@"), actual);
    Assert.Contains(FormatLine(3, "PAGE", citation), actual);
    Assert.Contains(FormatLine(3, "DATA", string.Empty), actual);
    Assert.Contains(FormatLine(4, "TEXT", citationActualText), actual);
    Assert.Contains(FormatLine(3, "NOTE", citationNote + " " + link), actual);
    Assert.Contains(FormatLine(3, "OBJE", string.Empty), actual);
    Assert.Contains(FormatLine(4, "FORM", "URL"), actual);
    Assert.Contains(FormatLine(4, "TITL", "URL of citation"), actual);
    Assert.Contains(FormatLine(4, "FILE", link), actual);
  }

  [Fact]
  public void ExportEvent_WithNullDateAndEmptyPlace_WritesNothing()
  {
    // Arrange
    string tag = "BIRT";
    string tagDescription = "Birth";
    string descriptor = "About";
    DateWrapper? date = null;
    string place = string.Empty;
    string citation = "Citation1";
    string citationNote = "Note1";
    string citationActualText = "ActualText1";
    string link = "http://link1.com";
    string source = "Source1";
    GedcomExport gedcomExport = new();
    using MemoryStream memoryStream = new();
    gedcomExport._writer = new(memoryStream);

    // Act
    gedcomExport.ExportEvent(tag, tagDescription, descriptor, date, place, citation, citationNote, citationActualText, link, source);

    // Assert
    gedcomExport._writer.Flush();
    string actual = Encoding.UTF8.GetString(memoryStream.ToArray());
    Assert.True(string.IsNullOrEmpty(actual));
  }

  [Theory]
  [InlineData(2019, 6, 21, "21 JUN 2019")]
  [InlineData(1982, 1, 9, "9 JAN 1982")]
  public void ExportDateTest(int year, int month, int day, string expected)
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      DateWrapper date = new(year, month, day);

      // Act
      string result = GedcomExport.ExportDate(date);

      // Assert
      Assert.Equal(expected, result);
      Assert.Equal(date, new(result));
    }
  }
}

public class GedcomIdMapTest
{
  [Fact]
  public void GetTest()
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
  public void FamilyConstructorTest()
  {
    Family sut = new(new(), new())!;

    Assert.NotNull(sut);
  }
}
