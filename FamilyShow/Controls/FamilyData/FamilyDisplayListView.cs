/*
 * Derived class that filters data in the diagram view.
*/

using Microsoft.FamilyShowLib;

namespace Microsoft.FamilyShow;

class FamilyDisplayListView : FilterSortListView
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

    if (Filter.Matches(person.Name) ||
        Filter.MatchesYear(person.BirthDate) ||
        Filter.MatchesYear(person.DeathDate) ||
        Filter.Matches(person.Age))
    {
      return true;
    }

    return false;
  }
}
