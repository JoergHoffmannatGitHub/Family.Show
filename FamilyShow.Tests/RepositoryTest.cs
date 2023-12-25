using Microsoft.FamilyShowLib;

namespace FamilyShowLib.Tests;

public class RepositoryTest
{
  readonly Repository sut = new("R1", "Archive", "here");

  [Fact]
  public void Equality_Null()
  {
    Repository nullRepository = null!;

    Assert.False(sut.Equals(null));
    Assert.True(nullRepository == null);
  }

  [Fact]
  public void Equality_Assigned()
  {
    Repository assignedRepository = sut;

    Assert.True(sut.Equals(assignedRepository));
    Assert.True(sut.Equals((object)assignedRepository));

    Assert.True(sut == assignedRepository);
    Assert.True(assignedRepository == sut);
    Assert.False(sut != assignedRepository);
    Assert.False(assignedRepository != sut);

    Assert.Equal(sut.GetHashCode(), assignedRepository.GetHashCode());
  }

  [Fact]
  public void Equality_SameId()
  {
    Repository sameRepository = new("R1", "Archive", "here");

    Assert.True(sut.Equals(sameRepository));
    Assert.True(sut.Equals((object)sameRepository));

    Assert.True(sut == sameRepository);
    Assert.True(sameRepository == sut);
    Assert.False(sut != sameRepository);
    Assert.False(sameRepository != sut);

    Assert.Equal(sut.GetHashCode(), sameRepository.GetHashCode());
  }

  [Fact]
  public void Equality_Different()
  {
    Repository differentRepository = new("R2", "Archive", "here");

    Assert.False(sut.Equals(differentRepository));
    Assert.False(sut.Equals((object)differentRepository));

    Assert.False(sut == differentRepository);
    Assert.True(sut != differentRepository);

    Assert.NotEqual(sut.GetHashCode(), differentRepository.GetHashCode());
  }
}
