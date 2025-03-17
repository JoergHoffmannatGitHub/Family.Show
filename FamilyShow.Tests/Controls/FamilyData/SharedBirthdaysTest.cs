using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using FamilyShowLib;

namespace FamilyShow.Tests.Controls.FamilyData;

public class SharedBirthdaysTest
{
  [Fact]
  public void FilterPerson_ShouldReturnTrueForPersonWithBirthDate()
  {
    // Arrange
    Person person = new("John", "Doe") { BirthDate = new DateTime(1980, 1, 1) };

    // Act
    bool result = SharedBirthdays.FilterPerson(person);

    // Assert
    Assert.True(result);
  }

  [Fact]
  public void FilterPerson_ShouldReturnFalseForPersonWithoutBirthDate()
  {
    // Arrange
    Person person = new("John", "Doe");

    // Act
    bool result = SharedBirthdays.FilterPerson(person);

    // Assert
    Assert.False(result);
  }

  [StaFact]
  public void PeopleCollectionProperty_Changed_ShouldSetUpListCollectionView()
  {
    // Arrange
    PeopleCollection peopleCollection =
    [
      new Person("John", "Doe") { BirthDate = new DateTime(1980, 1, 1) },
      new Person("Jane", "Doe") { BirthDate = new DateTime(1990, 2, 2) }
    ];
    SharedBirthdays sharedBirthdays = new();

    // Act
    SharedBirthdays.PeopleCollectionProperty_Changed(sharedBirthdays, new DependencyPropertyChangedEventArgs(SharedBirthdays.PeopleCollectionProperty, null, peopleCollection));

    // Assert
    ListCollectionView lcv = (ListCollectionView)sharedBirthdays.GroupedItemsControl.ItemsSource;
    Assert.NotNull(lcv);
    Assert.Equal(2, lcv.Count);
    Assert.True(lcv.Filter(new Person("John", "Doe") { BirthDate = new DateTime(1980, 1, 1) }));
    Assert.False(lcv.Filter(new Person("John", "Doe")));

    SharedBirthdays.s_lcv = null;
  }

  [StaFact]
  public void GroupedListBox_SelectionChanged_ShouldRaiseEventWithCorrectBirthDate()
  {
    // Arrange
    SharedBirthdays sharedBirthdays = new();
    ListBox listBox = new();
    Person person = new("John", "Doe") { BirthDate = new DateTime(1980, 1, 1) };
    listBox.Items.Add(person);
    listBox.SelectedItem = person;
    bool eventRaised = false;
    sharedBirthdays.SharedBirthdaysSelectionChanged += (sender, e) =>
    {
      eventRaised = true;
      Assert.Equal(person.BirthDate, e.OriginalSource);
    };

    // Act
    sharedBirthdays.GroupedListBox_SelectionChanged(listBox, new SelectionChangedEventArgs(ListBox.SelectionChangedEvent, new List<object>(), new List<object> { person }));

    // Assert
    Assert.True(eventRaised);
  }
}

public class MonthDayComparerTest
{
  [Fact]
  public void Compare_SamePersons_ShouldReturnCorrectly()
  {
    // Arrange
    Person person = new("John", "Doe") { BirthDate = new DateTime(1980, 1, 1) };
    MonthDayComparer monthDayComparer = new();

    // Act & Assert
    Assert.Equal(0, monthDayComparer.Compare(person, person));
  }

  [Fact]
  public void Compare_DifferentPersonsDifferenMonth_ShouldReturnCorrectly()
  {
    // Arrange
    Person john = new("John", "Doe") { BirthDate = new DateTime(1980, 2, 1) };
    Person jane = new("Jane", "Doe") { BirthDate = new DateTime(1980, 1, 1) };
    MonthDayComparer monthDayComparer = new();

    // Act & Assert
    Assert.Equal(1, monthDayComparer.Compare(john, jane));
    Assert.Equal(-1, monthDayComparer.Compare(jane, john));
  }

  [Fact]
  public void Compare_DifferentPersonsDifferenDay_ShouldReturnCorrectly()
  {
    // Arrange
    Person john = new("John", "Doe") { BirthDate = new DateTime(1980, 1, 2) };
    Person jane = new("Jane", "Doe") { BirthDate = new DateTime(1980, 1, 1) };
    MonthDayComparer monthDayComparer = new();

    // Act & Assert
    Assert.Equal(1, monthDayComparer.Compare(john, jane));
    Assert.Equal(-1, monthDayComparer.Compare(jane, john));
  }

  [Fact]
  public void Compare_DifferentPersonsSameBirthDate_ShouldReturnCorrectly()
  {
    // Arrange
    Person john = new("John", "Doe") { BirthDate = new DateTime(1980, 1, 1) };
    Person jane = new("Jane", "Doe") { BirthDate = new DateTime(1980, 1, 1) };
    MonthDayComparer monthDayComparer = new();

    // Act & Assert
    Assert.Equal(1, monthDayComparer.Compare(john, jane));
    Assert.Equal(-1, monthDayComparer.Compare(jane, john));
  }
}
