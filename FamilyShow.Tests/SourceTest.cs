using Microsoft.FamilyShowLib;

namespace FamilyShowLib.Tests;

public class SourceTest
{
  readonly Source sut = new("S0001", "Civil register NY", "Author", "Publisher", "@N0518@", "@R0008@");

  [Fact]
  public void Equality_Null()
  {
    Source nullSource = null!;

    Assert.False(sut.Equals(null));
    Assert.True(nullSource == null);
  }

  [Fact]
  public void Equality_Assigned()
  {
    Source assignedSource = sut;

    Assert.True(sut.Equals(assignedSource));
    Assert.True(sut.Equals((object)assignedSource));

    Assert.True(sut == assignedSource);
    Assert.True(assignedSource == sut);
    Assert.False(sut != assignedSource);
    Assert.False(assignedSource != sut);

    Assert.Equal(sut.GetHashCode(), assignedSource.GetHashCode());
  }

  [Fact]
  public void Equality_SameId()
  {
    Source sameSource = new("S0001", "Civil register NY", "Author", "Publisher", "@N0518@", "@R0008@");

    Assert.True(sut.Equals(sameSource));
    Assert.True(sut.Equals((object)sameSource));

    Assert.True(sut == sameSource);
    Assert.True(sameSource == sut);
    Assert.False(sut != sameSource);
    Assert.False(sameSource != sut);

    Assert.Equal(sut.GetHashCode(), sameSource.GetHashCode());
  }

  [Fact]
  public void Equality_Different()
  {
    Source differentSource = new("S0002", "Civil register NY", "Author", "Publisher", "@N0518@", "@R0008@");

    Assert.False(sut.Equals(differentSource));
    Assert.False(sut.Equals((object)differentSource));

    Assert.False(sut == differentSource);
    Assert.True(sut != differentSource);

    Assert.NotEqual(sut.GetHashCode(), differentSource.GetHashCode());
  }
}
