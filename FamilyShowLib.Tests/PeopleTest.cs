using System.Globalization;
using System.Xml.Serialization;

namespace FamilyShowLib.Tests;

// These tests will run infinite on GitHub
public class PeopleTest
{
  [Fact]
  public void LoadOPC()
  {
    People sut = new()
    {
      FullyQualifiedFilename = Sample.FullName("Windsor.familyx")
    };
    bool loaded = sut.LoadOPC();

    Assert.NotNull(sut);
    Assert.True(loaded);
    Assert.Equal(81, sut.PeopleCollection.Count);
    Assert.Empty(sut.SourceCollection);
    Assert.Empty(sut.RepositoryCollection);
  }

  [Fact]
  public void LoadVersion2()
  {
    People sut = new()
    {
      FullyQualifiedFilename = Sample.FullName("Windsor.family")
    };
    bool loaded = sut.LoadVersion2();

    Assert.NotNull(sut);
    Assert.True(loaded);
    Assert.Equal(81, sut.PeopleCollection.Count);
    Assert.Empty(sut.SourceCollection);
    Assert.Empty(sut.RepositoryCollection);
  }
}

public class ContentChangedEventArgsTest
{
  [Fact]
  public void ContentChangedEventArgs()
  {
    ContentChangedEventArgs sut = new(new())!;

    Assert.NotNull(sut);
  }
}

public class SourceContentChangedEventArgsTest
{
  [Fact]
  public void SourceContentChangedEventArgs()
  {
    SourceContentChangedEventArgs sut = new(new())!;

    Assert.NotNull(sut);
  }
}

public class RepositoryContentChangedEventArgsTest
{
  [Fact]
  public void RepositoryContentChangedEventArgs()
  {
    RepositoryContentChangedEventArgs sut = new(new())!;

    Assert.NotNull(sut);
  }
}
