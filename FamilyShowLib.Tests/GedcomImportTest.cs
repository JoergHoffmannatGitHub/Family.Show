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
    string location = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
    string fileName = Path.Combine(location, "Sample Files", "Kennedy.ged");
    bool loaded = sut.Import(family, source, repository, fileName, true);

    Assert.NotNull(sut);
    Assert.True(loaded);
    Assert.Equal(69, family.Count);
    Assert.Equal(11, source.Count);
    Assert.Empty(repository);
  }
}
