/*
 * A base class that sorts the data in a ListView control.
*/

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Threading;

namespace Microsoft.FamilyShow;

/// <summary>
/// Class that parses the filter text.
/// </summary>
public class Filter
{
  // Parsed data from the filter string.
  private string filterText;
  private int? maximumAge;
  private int? minimumAge;
  private DateTime? filterDate;
  private bool photos = false;
  private bool restrictions = false;
  private bool attachments = false;
  private bool notes = false;
  private bool images = false;
  private bool living = false;
  private bool citations = false;

  private bool nophotos = false;
  private bool norestrictions = false;
  private bool noattachments = false;
  private bool nonotes = false;
  private bool noimages = false;
  private bool noliving = false;
  private bool nocitations = false;
  private string gender;

  /// <summary>
  /// Indicates if the filter is empty.
  /// </summary>
  public bool IsEmpty
  {
    get { return string.IsNullOrEmpty(filterText); }
  }

  /// <summary>
  /// Return true if the filter contains the specified text.
  /// </summary>
  public bool Matches(string text)
  {
    return (filterText != null && text != null &&
        text.Contains(filterText, StringComparison.CurrentCultureIgnoreCase));
  }

  /// <summary>
  /// Return true if the filter contains the specified date.
  /// </summary>
  public bool Matches(DateTime? date)
  {
    return (date != null && date.Value.ToShortDateString().Contains(filterText));
  }

  /// <summary>
  /// Return true if the filter contains the year in the specified date.
  /// </summary>
  public bool MatchesYear(DateTime? date)
  {
    return (date != null && date.Value.Year.ToString(CultureInfo.CurrentCulture).Contains(filterText));
  }

  public bool MatchesPhotos(bool photo)
  {
    if (photo == true && photos == true)
    {
      return true;
    }

    if (nophotos == true && photo == false)
    {
      return true;
    }

    return false;
  }

  public bool MatchesRestrictions(bool restriction)
  {
    if (restriction == true && restrictions == true)
    {
      return true;
    }

    if (norestrictions == true && restriction == false)
    {
      return true;
    }

    return false;
  }

  public bool MatchesCitations(bool citation)
  {
    if (citation == true && citations == true)
    {
      return true;
    }

    if (nocitations == true && citation == false)
    {
      return true;
    }

    return false;
  }

  public bool MatchesAttachments(bool attachment)
  {
    if (attachment == true && attachments == true)
    {
      return true;
    }

    if (noattachments == true && attachment == false)
    {
      return true;
    }

    return false;
  }

  public bool MatchesNotes(bool note)
  {
    if (note == true && notes == true)
    {
      return true;
    }

    if (nonotes == true && note == false)
    {
      return true;
    }

    return false;
  }

  public bool MatchesLiving(bool isliving)
  {
    if (isliving == true && living == true)
    {
      return true;
    }

    if (noliving == true && isliving == false)
    {
      return true;
    }

    return false;
  }

  public bool MatchesImages(bool image)
  {
    if (image == true && images == true)
    {
      return true;
    }

    if (noimages == true && image == false)
    {
      return true;
    }

    return false;
  }

  public bool MatchesGender(string genders)
  {
    if (genders == gender)
    {
      return true;
    }

    return false;
  }

  /// <summary>
  /// Return true if the filter contains the month in the specified date.
  /// </summary>
  public bool MatchesMonth(DateTime? date)
  {
    return (date != null && filterDate != null &&
        date.Value.Month == filterDate.Value.Month);
  }

  /// <summary>
  /// Return true if the filter contains the day in the specified date.
  /// </summary>
  public bool MatchesDay(DateTime? date)
  {
    return (date != null && filterDate != null &&
        date.Value.Day == filterDate.Value.Day);
  }

  /// <summary>
  /// Return true if the filter contains the specified age. The filter can 
  /// represent a single age (10), a range (10-20), or an ending (10+).
  /// </summary>
  public bool Matches(int? age)
  {
    if (age == null)
    {
      return false;
    }

    // Check single age.
    if (minimumAge != null && age.Value == minimumAge.Value)
    {
      return true;
    }

    // Check for a range.
    if (minimumAge != null && maximumAge != null &&
        age.Value >= minimumAge && age <= maximumAge)
    {
      return true;
    }

    // Check for an ending age.
    if (minimumAge == null && maximumAge != null && age.Value >= maximumAge)
    {
      return true;
    }

    return false;
  }

  /// <summary>
  /// Parse the specified filter text.
  /// </summary>
  public void Parse(string text)
  {
    // Initialize fields.
    filterText = "";
    gender = "";
    filterDate = null;
    minimumAge = null;
    maximumAge = null;

    photos = false;
    restrictions = false;
    attachments = false;
    notes = false;
    images = false;
    living = false;
    citations = false;

    nophotos = false;
    norestrictions = false;
    noattachments = false;
    nonotes = false;
    noimages = false;
    noliving = false;
    nocitations = false;

    // Store the filter text.
    filterText = string.IsNullOrEmpty(text) ? "" : text.ToLower(CultureInfo.CurrentCulture).Trim();

    // Parse date and age.
    ParseDate();
    ParseAge();
    ParsePhotos();
    ParseRestrictions();
    ParseNotes();
    ParseAttachments();
    ParseImages();
    ParseLiving();
    ParseCitations();
    ParseGender();

  }

  /// <summary>
  /// Parse the filter date.
  /// </summary>
  private void ParseDate()
  {
    if (DateTime.TryParse(filterText, out DateTime date))
    {
      filterDate = date;
    }
  }

  /// <summary>
  /// Parse photos.
  /// </summary>
  private void ParsePhotos()
  {
    if (filterText.Equals(Properties.Resources.Photos, StringComparison.CurrentCultureIgnoreCase))
    {
      photos = true;
    }

    if (filterText.Equals("!" + Properties.Resources.Photos, StringComparison.CurrentCultureIgnoreCase))
    {
      nophotos = true;
    }
  }

  /// <summary>
  /// Parse genders.
  /// </summary>
  private void ParseGender()
  {
    if (filterText.Equals(Properties.Resources.Female, StringComparison.CurrentCultureIgnoreCase))
    {
      gender = Properties.Resources.Female.ToLower();
    }

    if (filterText.Equals(Properties.Resources.Male, StringComparison.CurrentCultureIgnoreCase))
    {
      gender = Properties.Resources.Male.ToLower();
    }
  }

  /// <summary>
  /// Parse restrictions.
  /// </summary>
  private void ParseRestrictions()
  {
    if (filterText.Equals(Properties.Resources.Restriction, StringComparison.CurrentCultureIgnoreCase))
    {
      restrictions = true;
    }

    if (filterText.Equals("!" + Properties.Resources.Restriction, StringComparison.CurrentCultureIgnoreCase))
    {
      norestrictions = true;
    }
  }

  /// <summary>
  /// Parse images.
  /// </summary>
  private void ParseImages()
  {
    if (filterText.Equals(Properties.Resources.Image, StringComparison.CurrentCultureIgnoreCase))
    {
      images = true;
    }

    if (filterText.Equals("!" + Properties.Resources.Image, StringComparison.CurrentCultureIgnoreCase))
    {
      noimages = true;
    }
  }

  /// <summary>
  /// Parse notes.
  /// </summary>
  private void ParseNotes()
  {
    if (filterText.Equals(Properties.Resources.Note, StringComparison.CurrentCultureIgnoreCase))
    {
      notes = true;
    }

    if (filterText.Equals("!" + Properties.Resources.Note, StringComparison.CurrentCultureIgnoreCase))
    {
      nonotes = true;
    }
  }

  /// <summary>
  /// Parse attachments.
  /// </summary>
  private void ParseAttachments()
  {
    if (filterText.Equals(Properties.Resources.Attachment, StringComparison.CurrentCultureIgnoreCase))
    {
      attachments = true;
    }

    if (filterText.Equals("!" + Properties.Resources.Attachment, StringComparison.CurrentCultureIgnoreCase))
    {
      noattachments = true;
    }
  }

  /// <summary>
  /// Parse living.
  /// </summary>
  private void ParseLiving()
  {
    if (filterText.Equals(Properties.Resources.Living, StringComparison.CurrentCultureIgnoreCase))
    {
      living = true;
    }

    if (filterText.Equals(Properties.Resources.Deceased, StringComparison.CurrentCultureIgnoreCase))
    {
      noliving = true;
    }
  }

  /// <summary>
  /// Parse citations.
  /// </summary>
  private void ParseCitations()
  {
    if (filterText.Equals(Properties.Resources.Citations, StringComparison.CurrentCultureIgnoreCase))
    {
      citations = true;
    }

    if (filterText.Equals("!" + Properties.Resources.Citations, StringComparison.CurrentCultureIgnoreCase))
    {
      nocitations = true;
    }
  }

  /// <summary>
  /// Parse the filter age. The filter can represent a
  /// single age (10), a range (10-20), or an ending (10+).
  /// </summary>
  private void ParseAge()
  {

    // Single age.
    if (int.TryParse(filterText, out int age))
    {
      minimumAge = age;
    }

    // Age range.
    if (filterText.Contains('-'))
    {
      string[] list = filterText.Split('-');

      if (int.TryParse(list[0], out age))
      {
        minimumAge = age;
      }

      if (int.TryParse(list[1], out age))
      {
        maximumAge = age;
      }
    }

    // Ending age.
    if (filterText.EndsWith('+'))
    {
      if (int.TryParse(filterText.AsSpan(0, filterText.Length - 1), out age))
      {
        maximumAge = age;
      }
    }
  }
}


/// <summary>
/// ??
/// </summary>
public class FilterSortListView : SortListView
{
  private delegate void FilterDelegate();

  /// <summary>
  /// Get the filter for this control.
  /// </summary>
  protected Filter Filter { get; } = new Filter();

  /// <summary>
  /// Filter the data using the specified filter text.
  /// </summary>
  public void FilterList(string text)
  {
    // Setup the filter object.
    Filter.Parse(text);

    // Start an async operation that filters the list.
    Dispatcher.BeginInvoke(
        DispatcherPriority.ApplicationIdle,
        new FilterDelegate(FilterWorker));
  }

  /// <summary>
  /// Worker method that filters the list.
  /// </summary>
  private void FilterWorker()
  {
    // Get the data the ListView is bound to.
    ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);

    // Clear the list if the filter is empty, otherwise filter the list.
    view.Filter = Filter.IsEmpty ? null :
        new Predicate<object>(FilterCallback);
  }

  /// <summary>
  /// This is called for each item in the list. The derived classes 
  /// override this method.
  /// </summary>
  protected virtual bool FilterCallback(object item)
  {
    return false;
  }
}
