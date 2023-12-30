/*
 * Derived class that filters data in the family data view.
*/

using Microsoft.FamilyShowLib;

namespace Microsoft.FamilyShow;

class FamilyEditListView : FilterSortListView
{
  /// <summary>
  /// Called for each item in the list. Return true if the item should be in
  /// the current result set, otherwise return false to exclude the item.
  /// </summary>    
  protected override bool FilterCallback(object item)
  {
    Person person = item as Person;
    if (person == null)
    {
      return false;
    }

    // Check for match.
    // Additional filters to search other columns
    if (Filter.Matches(person.FirstName) ||
        Filter.Matches(person.LastName) ||
        Filter.Matches(person.Suffix) ||
        Filter.Matches(person.Name) ||
        Filter.Matches(person.BurialPlace) ||
        Filter.Matches(person.BurialDate) ||
        Filter.Matches(person.Occupation) ||
        Filter.Matches(person.Education) ||
        Filter.Matches(person.Religion) ||
        Filter.Matches(person.CremationPlace) ||
        Filter.Matches(person.CremationDate) ||
        Filter.Matches(person.DeathPlace) ||
        Filter.Matches(person.DeathDate) ||
        Filter.Matches(person.BirthDate) ||
        Filter.Matches(person.BirthPlace) ||
        Filter.Matches(person.Age))
    {
      return true;
    }

    // Check for the special case of birthdays, if
    // matches the month and day, but don't check year.
    if (Filter.MatchesMonth(person.BirthDate) &&
        Filter.MatchesDay(person.BirthDate))
    {
      return true;
    }

    //Special filters
    if (Filter.MatchesImages(person.HasAvatar) ||
        Filter.MatchesRestrictions(person.HasRestriction) ||
        Filter.MatchesPhotos(person.HasPhoto) ||
        Filter.MatchesCitations(person.HasCitations) ||
        Filter.MatchesLiving(person.IsLiving) ||
        Filter.MatchesNotes(person.HasNote) ||
        Filter.MatchesAttachments(person.HasAttachments))
    {
      return true;
    }

    //filter for gender
    if (person.Gender == Gender.Male)
    {
      if (Filter.MatchesGender(Properties.Resources.Male.ToLower()))
      {
        return true;
      }
    }
    if (person.Gender == Gender.Female)
    {
      if (Filter.MatchesGender(Properties.Resources.Female.ToLower()))
      {
        return true;
      }
    }

    return false;
  }
}
