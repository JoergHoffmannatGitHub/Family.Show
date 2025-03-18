using FamilyShowLib;

namespace FamilyShow.Tests.Controls.FamilyData;

public class FilterSortListViewTest
{
  [Theory]
  [InlineData("10/5/2023", "2023-10-05", true)]
  [InlineData("10/5/2023", "2023-10-06", false)]
  [InlineData("10/5/2023", null, false)]
  public void Filter_MatchesDate(string filterText, string? dateText, bool expected)
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      Filter sut = new();
      sut.Parse(filterText);
      DateTime? date = string.IsNullOrEmpty(dateText) ? null : DateTime.Parse(dateText);

      // Act
      bool result = sut.Matches(date);

      // Assert
      Assert.Equal(expected, result);
    }
  }

  [Theory]
  [InlineData("2023", "2023-10-05", true)]
  [InlineData("2022", "2023-10-05", false)]
  [InlineData("2023", null, false)]
  public void Filter_MatchesYear(string filterText, string? dateText, bool expected)
  {
    // Arrange
    Filter sut = new();
    sut.Parse(filterText);
    DateTime? date = string.IsNullOrEmpty(dateText) ? null : DateTime.Parse(dateText);

    // Act
    bool result = sut.MatchesYear(date);

    // Assert
    Assert.Equal(expected, result);
  }

  [Theory]
  [InlineData("2023-10-05", "2023-10-05", true)]
  [InlineData("2023-11-05", "2023-10-05", false)]
  [InlineData("2023-10-05", null, false)]
  [InlineData(null, "2023-10-05", false)]
  public void Filter_MatchesMonth(string? filterText, string? dateText, bool expected)
  {
    // Arrange
    Filter sut = new();
    sut.Parse(filterText);
    sut.ParseDate();
    DateTime? date = string.IsNullOrEmpty(dateText) ? null : DateTime.Parse(dateText);

    // Act
    bool result = sut.MatchesMonth(date);

    // Assert
    Assert.Equal(expected, result);
  }

  [Theory]
  [InlineData("2023-10-05", "2023-10-05", true)]
  [InlineData("2023-10-06", "2023-10-05", false)]
  [InlineData("2023-10-05", null, false)]
  [InlineData(null, "2023-10-05", false)]
  public void Filter_MatchesDay(string? filterText, string? dateText, bool expected)
  {
    // Arrange
    Filter sut = new();
    sut.Parse(filterText);
    sut.ParseDate();
    DateTime? date = string.IsNullOrEmpty(dateText) ? null : DateTime.Parse(dateText);

    // Act
    bool result = sut.MatchesDay(date);

    // Assert
    Assert.Equal(expected, result);
  }

  [Fact]
  public void Filter_Matches()
  {
    // Arrange
    Filter sut = new();

    // Act
    sut.Parse("aBc");

    // Assert
    Assert.False(sut.IsEmpty);
    Assert.True(sut.Matches("aBC"));
    Assert.False(sut.MatchesPhotos(true));
    Assert.False(sut.MatchesPhotos(false));
    Assert.False(sut.MatchesGender("male"));
    Assert.False(sut.MatchesGender("female"));
    Assert.False(sut.MatchesRestrictions(true));
    Assert.False(sut.MatchesRestrictions(false));
    Assert.False(sut.MatchesImages(true));
    Assert.False(sut.MatchesImages(false));
    Assert.False(sut.MatchesNotes(true));
    Assert.False(sut.MatchesNotes(false));
    Assert.False(sut.MatchesAttachments(true));
    Assert.False(sut.MatchesAttachments(false));
    Assert.False(sut.MatchesLiving(true));
    Assert.False(sut.MatchesLiving(false));
    Assert.False(sut.MatchesCitations(true));
    Assert.False(sut.MatchesCitations(false));
  }

  [Theory]
  [InlineData("Photos", true)]
  [InlineData("!Photos", false)]
  public void Filter_MatchesPhotos(string text, bool photo)
  {
    // Arrange
    Filter sut = new();

    // Act
    sut.Parse(text);

    // Assert
    Assert.True(sut.Matches(text.ToUpper()));
    Assert.True(sut.MatchesPhotos(photo));
    Assert.False(sut.MatchesPhotos(!photo));
    Assert.False(sut.MatchesGender("male"));
    Assert.False(sut.MatchesGender("female"));
    Assert.False(sut.MatchesRestrictions(true));
    Assert.False(sut.MatchesRestrictions(false));
    Assert.False(sut.MatchesImages(true));
    Assert.False(sut.MatchesImages(false));
    Assert.False(sut.MatchesNotes(true));
    Assert.False(sut.MatchesNotes(false));
    Assert.False(sut.MatchesAttachments(true));
    Assert.False(sut.MatchesAttachments(false));
    Assert.False(sut.MatchesLiving(true));
    Assert.False(sut.MatchesLiving(false));
    Assert.False(sut.MatchesCitations(true));
    Assert.False(sut.MatchesCitations(false));
  }

  [Theory]
  [InlineData("Male", true)]
  [InlineData("Female", false)]
  public void Filter_MatchesGender(string text, bool gender)
  {
    // Arrange
    Filter sut = new();

    // Act
    sut.Parse(text);

    // Assert
    Assert.True(sut.Matches(text.ToUpper()));
    Assert.False(sut.MatchesPhotos(true));
    Assert.False(sut.MatchesPhotos(false));
    Assert.Equal(gender, sut.MatchesGender("male"));
    Assert.Equal(!gender, sut.MatchesGender("female"));
    Assert.False(sut.MatchesRestrictions(true));
    Assert.False(sut.MatchesRestrictions(false));
    Assert.False(sut.MatchesImages(true));
    Assert.False(sut.MatchesImages(false));
    Assert.False(sut.MatchesNotes(true));
    Assert.False(sut.MatchesNotes(false));
    Assert.False(sut.MatchesAttachments(true));
    Assert.False(sut.MatchesAttachments(false));
    Assert.False(sut.MatchesLiving(true));
    Assert.False(sut.MatchesLiving(false));
    Assert.False(sut.MatchesCitations(true));
    Assert.False(sut.MatchesCitations(false));
  }

  [Theory]
  [InlineData("Restriction", true)]
  [InlineData("!Restriction", false)]
  public void Filter_MatchesRestrictions(string text, bool restriction)
  {
    // Arrange
    Filter sut = new();

    // Act
    sut.Parse(text);

    // Assert
    Assert.True(sut.Matches(text.ToUpper()));
    Assert.False(sut.MatchesPhotos(true));
    Assert.False(sut.MatchesPhotos(false));
    Assert.False(sut.MatchesGender("male"));
    Assert.False(sut.MatchesGender("female"));
    Assert.True(sut.MatchesRestrictions(restriction));
    Assert.False(sut.MatchesRestrictions(!restriction));
    Assert.False(sut.MatchesImages(true));
    Assert.False(sut.MatchesImages(false));
    Assert.False(sut.MatchesNotes(true));
    Assert.False(sut.MatchesNotes(false));
    Assert.False(sut.MatchesAttachments(true));
    Assert.False(sut.MatchesAttachments(false));
    Assert.False(sut.MatchesLiving(true));
    Assert.False(sut.MatchesLiving(false));
    Assert.False(sut.MatchesCitations(true));
    Assert.False(sut.MatchesCitations(false));
  }

  [Theory]
  [InlineData("Image", true)]
  [InlineData("!Image", false)]
  public void Filter_MatchesImages(string text, bool image)
  {
    // Arrange
    Filter sut = new();

    // Act
    sut.Parse(text);

    // Assert
    Assert.True(sut.Matches(text.ToUpper()));
    Assert.False(sut.MatchesPhotos(true));
    Assert.False(sut.MatchesPhotos(false));
    Assert.False(sut.MatchesGender("male"));
    Assert.False(sut.MatchesGender("female"));
    Assert.False(sut.MatchesRestrictions(true));
    Assert.False(sut.MatchesRestrictions(false));
    Assert.True(sut.MatchesImages(image));
    Assert.False(sut.MatchesImages(!image));
    Assert.False(sut.MatchesNotes(true));
    Assert.False(sut.MatchesNotes(false));
    Assert.False(sut.MatchesAttachments(true));
    Assert.False(sut.MatchesAttachments(false));
    Assert.False(sut.MatchesLiving(true));
    Assert.False(sut.MatchesLiving(false));
    Assert.False(sut.MatchesCitations(true));
    Assert.False(sut.MatchesCitations(false));
  }

  [Theory]
  [InlineData("Note", true)]
  [InlineData("!Note", false)]
  public void Filter_MatchesNotes(string text, bool note)
  {
    // Arrange
    Filter sut = new();

    // Act
    sut.Parse(text);

    // Assert
    Assert.True(sut.Matches(text.ToUpper()));
    Assert.False(sut.MatchesPhotos(true));
    Assert.False(sut.MatchesPhotos(false));
    Assert.False(sut.MatchesGender("male"));
    Assert.False(sut.MatchesGender("female"));
    Assert.False(sut.MatchesRestrictions(true));
    Assert.False(sut.MatchesRestrictions(false));
    Assert.False(sut.MatchesImages(true));
    Assert.False(sut.MatchesImages(false));
    Assert.True(sut.MatchesNotes(note));
    Assert.False(sut.MatchesNotes(!note));
    Assert.False(sut.MatchesAttachments(true));
    Assert.False(sut.MatchesAttachments(false));
    Assert.False(sut.MatchesLiving(true));
    Assert.False(sut.MatchesLiving(false));
    Assert.False(sut.MatchesCitations(true));
    Assert.False(sut.MatchesCitations(false));
  }

  [Theory]
  [InlineData("Attachment", true)]
  [InlineData("!Attachment", false)]
  public void Filter_MatchesAttachments(string text, bool attachment)
  {
    // Arrange
    Filter sut = new();

    // Act
    sut.Parse(text);

    // Assert
    Assert.True(sut.Matches(text.ToUpper()));
    Assert.False(sut.MatchesPhotos(true));
    Assert.False(sut.MatchesPhotos(false));
    Assert.False(sut.MatchesGender("male"));
    Assert.False(sut.MatchesGender("female"));
    Assert.False(sut.MatchesRestrictions(true));
    Assert.False(sut.MatchesRestrictions(false));
    Assert.False(sut.MatchesImages(true));
    Assert.False(sut.MatchesImages(false));
    Assert.False(sut.MatchesNotes(true));
    Assert.False(sut.MatchesNotes(false));
    Assert.True(sut.MatchesAttachments(attachment));
    Assert.False(sut.MatchesAttachments(!attachment));
    Assert.False(sut.MatchesLiving(true));
    Assert.False(sut.MatchesLiving(false));
    Assert.False(sut.MatchesCitations(true));
    Assert.False(sut.MatchesCitations(false));
  }

  [Theory]
  [InlineData("Living", true)]
  [InlineData("Deceased", false)]
  public void Filter_MatchesLiving(string text, bool living)
  {
    // Arrange
    Filter sut = new();

    // Act
    sut.Parse(text);

    // Assert
    Assert.True(sut.Matches(text.ToUpper()));
    Assert.False(sut.MatchesPhotos(true));
    Assert.False(sut.MatchesPhotos(false));
    Assert.False(sut.MatchesGender("male"));
    Assert.False(sut.MatchesGender("female"));
    Assert.False(sut.MatchesRestrictions(true));
    Assert.False(sut.MatchesRestrictions(false));
    Assert.False(sut.MatchesImages(true));
    Assert.False(sut.MatchesImages(false));
    Assert.False(sut.MatchesNotes(true));
    Assert.False(sut.MatchesNotes(false));
    Assert.False(sut.MatchesAttachments(true));
    Assert.False(sut.MatchesAttachments(false));
    Assert.True(sut.MatchesLiving(living));
    Assert.False(sut.MatchesLiving(!living));
    Assert.False(sut.MatchesCitations(true));
    Assert.False(sut.MatchesCitations(false));
  }

  [Theory]
  [InlineData("Citations", true)]
  [InlineData("!Citations", false)]
  public void Filter_MatchesCitations(string text, bool citations)
  {
    // Arrange
    Filter sut = new();

    // Act
    sut.Parse(text);

    // Assert
    Assert.True(sut.Matches(text.ToUpper()));
    Assert.False(sut.MatchesPhotos(true));
    Assert.False(sut.MatchesPhotos(false));
    Assert.False(sut.MatchesGender("male"));
    Assert.False(sut.MatchesGender("female"));
    Assert.False(sut.MatchesRestrictions(true));
    Assert.False(sut.MatchesRestrictions(false));
    Assert.False(sut.MatchesImages(true));
    Assert.False(sut.MatchesImages(false));
    Assert.False(sut.MatchesNotes(true));
    Assert.False(sut.MatchesNotes(false));
    Assert.False(sut.MatchesAttachments(true));
    Assert.False(sut.MatchesAttachments(false));
    Assert.False(sut.MatchesLiving(true));
    Assert.False(sut.MatchesLiving(false));
    Assert.True(sut.MatchesCitations(citations));
    Assert.False(sut.MatchesCitations(!citations));
  }
}
