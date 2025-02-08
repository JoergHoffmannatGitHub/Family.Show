namespace FamilyShowLib.Tests;

// These tests will run infinite on GitHub
public class PeopleTest
{
  [Fact]
  public void LoadOPC()
  {
    // Arrange
    People sut = new()
    {
      FullyQualifiedFilename = Sample.FullName("Windsor.familyx")
    };

    // Act
    bool loaded = sut.LoadOPC();

    // Assert
    Assert.NotNull(sut);
    Assert.True(loaded);
    Assert.Equal(81, sut.PeopleCollection.Count);
    Assert.Empty(sut.SourceCollection);
    Assert.Empty(sut.RepositoryCollection);
  }

  [Fact]
  public void LoadVersion2()
  {
    // Arrange
    People sut = new()
    {
      FullyQualifiedFilename = Sample.FullName("Windsor.family")
    };

    // Act
    bool loaded = sut.LoadVersion2();

    // Assert
    Assert.NotNull(sut);
    Assert.True(loaded);
    Assert.Equal(81, sut.PeopleCollection.Count);
    Assert.Empty(sut.SourceCollection);
    Assert.Empty(sut.RepositoryCollection);
  }

  [Fact]
  public void MergeOPC_WithValidFile_ShouldMergeSuccessfully()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      People sut = new();
      string fileName = Sample.FullName("Windsor.familyx");

      // Act
      string[,] result = sut.MergeOPC(fileName);

      // Assert
      Assert.NotNull(result);
      Assert.Equal("All 81 people imported.", result[0, 0]);
      Assert.Equal("\n\nNo sources to import.", result[1, 0]);
      Assert.Equal("\n\nNo repositories to import.", result[2, 0]);
    }
  }

  [Fact]
  public void MergeOPC_WithDuplicateFile_ShouldHandleDuplicates()
  {
    using (AnotherCulture.UnitedStates())
    {
      // Arrange
      People sut = new()
      {
        FullyQualifiedFilename = Sample.FullName("Windsor.familyx")
      };
      bool loaded = sut.LoadOPC();
      Assert.True(loaded);
      string fileName = Sample.FullName("Windsor.familyx");

      // Act
      string[,] result = sut.MergeOPC(fileName);

      // Assert
      Assert.NotNull(result);
      Assert.Contains("Imported people: 0\nDuplicate people: 81", result[0, 0]);
      Assert.Contains("\n\nNo sources to import.", result[1, 0]);
      Assert.Contains("\n\nNo repositories to import.", result[2, 0]);
    }
  }

  [Fact]
  public void MergeOPC_WithInvalidFile_ShouldReturnNull()
  {
    // Arrange
    People sut = new();
    string fileName = "InvalidPath.familyx";

    // Act
    string[,] result = sut.MergeOPC(fileName);

    // Assert
    Assert.Null(result);
  }
}

public class ContentChangedEventArgsTest
{
  [Fact]
  public void ContentChangedEventArgs()
  {
    // Act
    ContentChangedEventArgs sut = new(new())!;

    // Assert
    Assert.NotNull(sut);
  }
}

public class SourceContentChangedEventArgsTest
{
  [Fact]
  public void SourceContentChangedEventArgs()
  {
    // Act
    SourceContentChangedEventArgs sut = new(new())!;

    // Assert
    Assert.NotNull(sut);
  }
}

public class RepositoryContentChangedEventArgsTest
{
  [Fact]
  public void RepositoryContentChangedEventArgs()
  {
    // Act
    RepositoryContentChangedEventArgs sut = new(new())!;

    // Assert
    Assert.NotNull(sut);
  }
}
