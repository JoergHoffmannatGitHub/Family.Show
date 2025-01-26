using FamilyShowLib;

namespace FamilyShow.Tests.Controls;

[CollectionDefinition(nameof(DetailsTest), DisableParallelization = true)]
public class DetailsTest
{
  [StaTheory]
  [InlineData(FamilyMemberComboBoxValue.Father, "Add Father")]
  [InlineData(FamilyMemberComboBoxValue.Mother, "Add Mother")]
  [InlineData(FamilyMemberComboBoxValue.Brother, "Add Brother")]
  [InlineData(FamilyMemberComboBoxValue.Sister, "Add Sister")]
  [InlineData(FamilyMemberComboBoxValue.Spouse, "Add Spouse/Partner")]
  [InlineData(FamilyMemberComboBoxValue.Son, "Add Son")]
  [InlineData(FamilyMemberComboBoxValue.Daughter, "Add Daughter")]
  [InlineData(FamilyMemberComboBoxValue.Unrelated, "Add ")]
  [InlineData(FamilyMemberComboBoxValue.Existing, "Add ")]
  public void SetNextFamilyMemberActionTest(FamilyMemberComboBoxValue familyMemberComboBoxValue, string expected)
  {
    using (new AnotherCulture("en-US"))
    {
      // Arrange
      Details sut = new();
      // Act
      sut.SetNextFamilyMemberAction(familyMemberComboBoxValue);
      // Assert
      Assert.Equal(expected, sut.FamilyMemberAddButton.Content);
    }
  }

  public static readonly TheoryData<string, DateTime?> StringDateCases =
    new()
    {
      { "", null },
      { "invalid", null },
      { "6 SEP 1888", new DateTime(1888, 9, 6) },
    };

  [StaTheory, MemberData(nameof(StringDateCases))]
  public void AddButton_ClickTest(string birthDate, DateTime? expected)
  {
    // Arrange
    InitAppCollections();
    PeopleCollection family = App.Family;
    Details sut = new();

    Person firstPerson = new("first", "person");
    family.Current = firstPerson;
    family.Add(firstPerson);
    family.OnContentChanged();

    sut.SetNextFamilyMemberAction(FamilyMemberComboBoxValue.Unrelated);
    sut.FamilyMemberAddButton_Click(null, null);

    sut.NamesInputTextBox.Text = "unrelated";
    sut.SurnameInputTextBox.Text = "person";
    sut.BirthDateInputTextBox.Text = birthDate;
    // Act
    sut.AddButton_Click(null, null);
    // Assert
    Assert.Equal(expected, family.Current.BirthDate);
  }

  private static void InitAppCollections()
  {
    People familyCollection = new();
    App.FamilyCollection = familyCollection;
    App.Family = familyCollection.PeopleCollection;
    App.Sources = familyCollection.SourceCollection;
    App.Repositories = familyCollection.RepositoryCollection;
  }
}
