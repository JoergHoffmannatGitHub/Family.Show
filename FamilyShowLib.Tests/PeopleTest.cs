using FamilyShowLib;

namespace FamilyShowLib.Tests;

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
