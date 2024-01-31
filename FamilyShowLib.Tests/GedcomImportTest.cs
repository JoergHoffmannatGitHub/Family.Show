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
}
