namespace FamilyShow.Tests.Controls.FamilyData;

public class FilterSortListViewTest
{
  [Fact]
  public void Matches()
  {
    Filter sut = new();
    sut.Parse("aBc");

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
  public void MatchesPhotos(string text, bool photo)
  {
    Filter sut = new();
    sut.Parse(text);

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
  public void MatchesGender(string text, bool gender)
  {
    Filter sut = new();
    sut.Parse(text);

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
  public void MatchesRestrictions(string text, bool restriction)
  {
    Filter sut = new();
    sut.Parse(text);

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
  public void MatchesImages(string text, bool image)
  {
    Filter sut = new();
    sut.Parse(text);

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
  public void MatchesNotes(string text, bool note)
  {
    Filter sut = new();
    sut.Parse(text);

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
  public void MatchesAttachments(string text, bool attachment)
  {
    Filter sut = new();
    sut.Parse(text);

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
  public void MatchesLiving(string text, bool living)
  {
    Filter sut = new();
    sut.Parse(text);

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
  public void MatchesCitations(string text, bool citations)
  {
    Filter sut = new();
    sut.Parse(text);

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
