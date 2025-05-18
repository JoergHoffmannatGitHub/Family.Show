namespace FamilyShowLib.Tests;

// These tests will run infinite on GitHub
[CollectionDefinition("SequentialTest", DisableParallelization = true)]

[Collection("SequentialTest")]
public class PeopleTest
{
  string PhotoFolder { get; set; }

  string StoryFolder { get; set; }

  public PeopleTest()
  {
    string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    string tempFolder = Path.Combine(localApplicationData, App.ApplicationFolderName, App.AppDataFolderName);
    PhotoFolder = Path.Combine(tempFolder, Photo.PhotosFolderName);
    StoryFolder = Path.Combine(tempFolder, Story.StoriesFolderName);
  }

  [Fact]
  public void People_LoadOPC()
  {
    // Arrange
    People sut = new()
    {
      FullyQualifiedFilename = Sample.FullName("Windsor.familyx")
    };

    // Act
    bool loaded = sut.LoadOPC();

    // Assert
    Assert.True(loaded);
    Assert.NotNull(sut);
    Assert.Equal(81, sut.PeopleCollection.Count);
    (int photoCount, int storyCount) = CalculatePhotoAndStoryCounts(sut.PeopleCollection);
    Assert.Equal(10, photoCount);
    Assert.Equal(10, Directory.GetFiles(PhotoFolder).Length);
    Assert.Equal(9, storyCount);
    Assert.Equal(9, Directory.GetFiles(StoryFolder).Length);
    Assert.Empty(sut.SourceCollection);
    Assert.Empty(sut.RepositoryCollection);
  }

  [Fact]
  public void People_Save()
  {
    // Arrange
    string oldFile = Sample.FullName("Windsor.familyx");
    People sut = new()
    {
      FullyQualifiedFilename = oldFile
    };
    sut.LoadOPC();
    string newFile = Sample.FullName("Windsor_new.familyx");

    // Act
    sut.Save(newFile);

    // Assert
    Assert.True(sut.LoadOPC());
    Assert.Equal(81, sut.PeopleCollection.Count);
    (int photoCount, int storyCount) = CalculatePhotoAndStoryCounts(sut.PeopleCollection);
    Assert.Equal(10, photoCount);
    Assert.Equal(10, Directory.GetFiles(PhotoFolder).Length);
    Assert.Equal(9, storyCount);
    Assert.Equal(9, Directory.GetFiles(StoryFolder).Length);
    Assert.Empty(sut.SourceCollection);
    Assert.Empty(sut.RepositoryCollection);
  }

  [Fact]
  public void People_LoadVersion2()
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
    //(int photoCount, int storyCount) = CalculatePhotoAndStoryCounts(sut.PeopleCollection);
    //Assert.Equal(10, photoCount);
    //Assert.Equal(10, Directory.GetFiles(PhotoFolder).Length);
    //Assert.Equal(9, storyCount);
    //Assert.Equal(9, Directory.GetFiles(StoryFolder).Length);
    Assert.Empty(sut.SourceCollection);
    Assert.Empty(sut.RepositoryCollection);
  }

  private static (int photoCount, int storyCount) CalculatePhotoAndStoryCounts(PeopleCollection peopleCollection)
  {
    int photoCount = 0;
    int storyCount = 0;
    foreach (Person person in peopleCollection)
    {
      photoCount += person.Photos.Count;
      if (person.Story != null)
      {
        storyCount++;
      }
    }
    return (photoCount, storyCount);
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
