/*
 * A base class that sorts the data in a ListView control.
*/

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Threading;

using FamilyShowLib;

using Genealogy.Domain.Interfaces;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FamilyShow;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Class that parses the filter text.
/// </summary>
public class Filter
{
    // Parsed data from the filter string.
    private string _filterText;
    private int? _maximumAge;
    private int? _minimumAge;
    private DateTime? _filterDate;
    private bool _photos;
    private bool _restrictions;
    private bool _attachments;
    private bool _notes;
    private bool _images;
    private bool _living;
    private bool _citations;

    private bool _nophotos;
    private bool _norestrictions;
    private bool _noattachments;
    private bool _nonotes;
    private bool _noimages;
    private bool _noliving;
    private bool _nocitations;
    private string _gender;

    /// <summary>
    /// Indicates if the filter is empty.
    /// </summary>
    public bool IsEmpty
    {
        get { return string.IsNullOrEmpty(_filterText); }
    }

    /// <summary>
    /// Return true if the filter contains the specified text.
    /// </summary>
    public bool Matches(string text)
    {
        return (_filterText != null && text != null &&
            text.Contains(_filterText, StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Return true if the filter contains the specified date.
    /// </summary>
    public bool Matches(DateWrapper date)
    {
        return (date != null && date.ToShortString().Contains(_filterText));
    }

    /// <summary>
    /// Return true if the filter contains the year in the specified date.
    /// </summary>
    public bool MatchesYear(DateWrapper date)
    {
        return (DateWrapper.IsDateExact(date, out IDateExact exactDate) &&
            exactDate.Year.ToString(CultureInfo.CurrentCulture).Contains(_filterText));
    }

    public bool MatchesPhotos(bool photo)
    {
        if (photo == true && _photos == true)
        {
            return true;
        }

        if (_nophotos == true && photo == false)
        {
            return true;
        }

        return false;
    }

    public bool MatchesRestrictions(bool restriction)
    {
        if (restriction == true && _restrictions == true)
        {
            return true;
        }

        if (_norestrictions == true && restriction == false)
        {
            return true;
        }

        return false;
    }

    public bool MatchesCitations(bool citation)
    {
        if (citation == true && _citations == true)
        {
            return true;
        }

        if (_nocitations == true && citation == false)
        {
            return true;
        }

        return false;
    }

    public bool MatchesAttachments(bool attachment)
    {
        if (attachment == true && _attachments == true)
        {
            return true;
        }

        if (_noattachments == true && attachment == false)
        {
            return true;
        }

        return false;
    }

    public bool MatchesNotes(bool note)
    {
        if (note == true && _notes == true)
        {
            return true;
        }

        if (_nonotes == true && note == false)
        {
            return true;
        }

        return false;
    }

    public bool MatchesLiving(bool isliving)
    {
        if (isliving == true && _living == true)
        {
            return true;
        }

        if (_noliving == true && isliving == false)
        {
            return true;
        }

        return false;
    }

    public bool MatchesImages(bool image)
    {
        if (image == true && _images == true)
        {
            return true;
        }

        if (_noimages == true && image == false)
        {
            return true;
        }

        return false;
    }

    public bool MatchesGender(string genders)
    {
        if (genders == _gender)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Return true if the filter contains the month in the specified date.
    /// </summary>
    public bool MatchesMonth(DateWrapper date)
    {
        return DateWrapper.IsDateExact(date, out IDateExact exact) && _filterDate != null &&
            exact.Month == _filterDate.Value.Month;
    }

    /// <summary>
    /// Return true if the filter contains the day in the specified date.
    /// </summary>
    public bool MatchesDay(DateWrapper date)
    {
        return DateWrapper.IsDateExact(date, out IDateExact exact) && _filterDate != null &&
            exact.Day == _filterDate.Value.Day;
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
        if (_minimumAge != null && age.Value == _minimumAge.Value)
        {
            return true;
        }

        // Check for a range.
        if (_minimumAge != null && _maximumAge != null &&
            age.Value >= _minimumAge && age <= _maximumAge)
        {
            return true;
        }

        // Check for an ending age.
        if (_minimumAge == null && _maximumAge != null && age.Value >= _maximumAge)
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
        _filterText = "";
        _gender = "";
        _filterDate = null;
        _minimumAge = null;
        _maximumAge = null;

        _photos = false;
        _restrictions = false;
        _attachments = false;
        _notes = false;
        _images = false;
        _living = false;
        _citations = false;

        _nophotos = false;
        _norestrictions = false;
        _noattachments = false;
        _nonotes = false;
        _noimages = false;
        _noliving = false;
        _nocitations = false;

        // Store the filter text.
        _filterText = string.IsNullOrEmpty(text) ? "" : text.ToLower(CultureInfo.CurrentCulture).Trim();

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
    internal void ParseDate()
    {
        if (DateTime.TryParse(_filterText, out DateTime date))
        {
            _filterDate = date;
        }
    }

    /// <summary>
    /// Parse photos.
    /// </summary>
    private void ParsePhotos()
    {
        if (_filterText.Equals(Properties.Resources.Photos, StringComparison.CurrentCultureIgnoreCase))
        {
            _photos = true;
        }

        if (_filterText.Equals("!" + Properties.Resources.Photos, StringComparison.CurrentCultureIgnoreCase))
        {
            _nophotos = true;
        }
    }

    /// <summary>
    /// Parse genders.
    /// </summary>
    private void ParseGender()
    {
        if (_filterText.Equals(Properties.Resources.Female, StringComparison.CurrentCultureIgnoreCase))
        {
            _gender = Properties.Resources.Female.ToLower();
        }

        if (_filterText.Equals(Properties.Resources.Male, StringComparison.CurrentCultureIgnoreCase))
        {
            _gender = Properties.Resources.Male.ToLower();
        }
    }

    /// <summary>
    /// Parse restrictions.
    /// </summary>
    private void ParseRestrictions()
    {
        if (_filterText.Equals(Properties.Resources.Restriction, StringComparison.CurrentCultureIgnoreCase))
        {
            _restrictions = true;
        }

        if (_filterText.Equals("!" + Properties.Resources.Restriction, StringComparison.CurrentCultureIgnoreCase))
        {
            _norestrictions = true;
        }
    }

    /// <summary>
    /// Parse images.
    /// </summary>
    private void ParseImages()
    {
        if (_filterText.Equals(Properties.Resources.Image, StringComparison.CurrentCultureIgnoreCase))
        {
            _images = true;
        }

        if (_filterText.Equals("!" + Properties.Resources.Image, StringComparison.CurrentCultureIgnoreCase))
        {
            _noimages = true;
        }
    }

    /// <summary>
    /// Parse notes.
    /// </summary>
    private void ParseNotes()
    {
        if (_filterText.Equals(Properties.Resources.Note, StringComparison.CurrentCultureIgnoreCase))
        {
            _notes = true;
        }

        if (_filterText.Equals("!" + Properties.Resources.Note, StringComparison.CurrentCultureIgnoreCase))
        {
            _nonotes = true;
        }
    }

    /// <summary>
    /// Parse attachments.
    /// </summary>
    private void ParseAttachments()
    {
        if (_filterText.Equals(Properties.Resources.Attachment, StringComparison.CurrentCultureIgnoreCase))
        {
            _attachments = true;
        }

        if (_filterText.Equals("!" + Properties.Resources.Attachment, StringComparison.CurrentCultureIgnoreCase))
        {
            _noattachments = true;
        }
    }

    /// <summary>
    /// Parse living.
    /// </summary>
    private void ParseLiving()
    {
        if (_filterText.Equals(Properties.Resources.Living, StringComparison.CurrentCultureIgnoreCase))
        {
            _living = true;
        }

        if (_filterText.Equals(Properties.Resources.Deceased, StringComparison.CurrentCultureIgnoreCase))
        {
            _noliving = true;
        }
    }

    /// <summary>
    /// Parse citations.
    /// </summary>
    private void ParseCitations()
    {
        if (_filterText.Equals(Properties.Resources.Citations, StringComparison.CurrentCultureIgnoreCase))
        {
            _citations = true;
        }

        if (_filterText.Equals("!" + Properties.Resources.Citations, StringComparison.CurrentCultureIgnoreCase))
        {
            _nocitations = true;
        }
    }

    /// <summary>
    /// Parse the filter age. The filter can represent a
    /// single age (10), a range (10-20), or an ending (10+).
    /// </summary>
    private void ParseAge()
    {

        // Single age.
        if (int.TryParse(_filterText, out int age))
        {
            _minimumAge = age;
        }

        // Age range.
        if (_filterText.Contains('-'))
        {
            string[] list = _filterText.Split('-');

            if (int.TryParse(list[0], out age))
            {
                _minimumAge = age;
            }

            if (int.TryParse(list[1], out age))
            {
                _maximumAge = age;
            }
        }

        // Ending age.
        if (_filterText.EndsWith('+'))
        {
            if (int.TryParse(_filterText.AsSpan(0, _filterText.Length - 1), out age))
            {
                _maximumAge = age;
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
