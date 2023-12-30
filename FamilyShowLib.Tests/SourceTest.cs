﻿using Microsoft.FamilyShowLib;

namespace FamilyShowLib.Tests;

public class SourceTest
{
  private static readonly Source s_sut = new("S0001", "Civil register NY", "Author", "Publisher", "@N0518@", "@R0008@");

  [Fact]
  public void Equality_Null()
  {
    Source nullSource = null!;

    Assert.False(s_sut.Equals(null));
    Assert.True(nullSource == null);
  }

  [Fact]
  public void Equality_Assigned()
  {
    Source assignedSource = s_sut;

    Assert.True(s_sut.Equals(assignedSource));
    Assert.True(s_sut.Equals((object)assignedSource));

    Assert.True(s_sut == assignedSource);
    Assert.True(assignedSource == s_sut);
    Assert.False(s_sut != assignedSource);
    Assert.False(assignedSource != s_sut);

    Assert.Equal(s_sut.GetHashCode(), assignedSource.GetHashCode());
  }

  [Fact]
  public void Equality_SameId()
  {
    Source sameSource = new("S0001", "Civil register NY", "Author", "Publisher", "@N0518@", "@R0008@");

    Assert.True(s_sut.Equals(sameSource));
    Assert.True(s_sut.Equals((object)sameSource));

    Assert.True(s_sut == sameSource);
    Assert.True(sameSource == s_sut);
    Assert.False(s_sut != sameSource);
    Assert.False(sameSource != s_sut);

    Assert.Equal(s_sut.GetHashCode(), sameSource.GetHashCode());
  }

  [Fact]
  public void Equality_Different()
  {
    Source differentSource = new("S0002", "Civil register NY", "Author", "Publisher", "@N0518@", "@R0008@");

    Assert.False(s_sut.Equals(differentSource));
    Assert.False(s_sut.Equals((object)differentSource));

    Assert.False(s_sut == differentSource);
    Assert.True(s_sut != differentSource);

    Assert.NotEqual(s_sut.GetHashCode(), differentSource.GetHashCode());
  }
}
