using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using FamilyShowLib;

using Genealogy.Domain.Interfaces;

namespace FamilyShow;

/// <summary>
/// Interaction logic for SharedBirthdays.xaml
/// </summary>

public partial class SharedBirthdays : UserControl
{
  internal static ListCollectionView s_lcv;

  #region dependency properties

  public static readonly DependencyProperty PeopleCollectionProperty =
      DependencyProperty.Register("PeopleCollection", typeof(object), typeof(SharedBirthdays),
          new FrameworkPropertyMetadata(new PropertyChangedCallback(PeopleCollectionProperty_Changed)));

  /// <summary>
  /// The Collection that will be used to build the Tag Cloud
  /// </summary>
  public object PeopleCollection
  {
    get { return GetValue(PeopleCollectionProperty); }
    set { SetValue(PeopleCollectionProperty, value); }
  }

  #endregion

  #region routed events

  public static readonly RoutedEvent SharedBirthdaysSelectionChangedEvent = EventManager.RegisterRoutedEvent(
      "SharedBirthdaysSelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SharedBirthdays));

  public event RoutedEventHandler SharedBirthdaysSelectionChanged
  {
    add { AddHandler(SharedBirthdaysSelectionChangedEvent, value); }
    remove { RemoveHandler(SharedBirthdaysSelectionChangedEvent, value); }
  }

  #endregion

  public SharedBirthdays()
  {
    InitializeComponent();
  }

  /// <summary>
  /// Used as a filter predicate to see if the person should be included 
  /// </summary>
  /// <param name="o">Person object</param>
  /// <returns>True if the person should be included in the filter, otherwise false</returns>
  public static bool FilterPerson(object o)
  {
    Person p = o as Person;
    return (p.BirthDate != null);
  }

  internal static void PeopleCollectionProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
  {
    SharedBirthdays sharedBirthdays = ((SharedBirthdays)sender);

    // ListCollectionView is used for sorting and grouping
    s_lcv = new ListCollectionView((IList)args.NewValue)
    {
      // Include only those people with a birthdate
      Filter = new Predicate<object>(FilterPerson),

      // Sort by Month and Day only
      CustomSort = new MonthDayComparer()
    };

    // Group the collection by the month and day of the person's birthdate
    s_lcv.GroupDescriptions.Add(new PropertyGroupDescription("BirthMonthAndDay"));

    sharedBirthdays.GroupedItemsControl.ItemsSource = s_lcv;
  }

  internal void GroupedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    Person selected = ((ListBox)sender).SelectedItem as Person;

    if (selected != null)
    {
      RaiseEvent(new RoutedEventArgs(SharedBirthdaysSelectionChangedEvent, selected.BirthDate));
    }
  }

  internal static void Refresh()
  {
    s_lcv?.Refresh();
  }

  internal void ClearSelection()
  {
    GroupedItemsControl.UnselectAll();
  }
}

/// <summary>
/// Compare the birthdates (month and day) and first names of two people
/// </summary>
public class MonthDayComparer : IComparer
{
  #region IComparer Members

  public int Compare(object x, object y)
  {
    Person p1 = x as Person;
    Person p2 = y as Person;

    if (p1 == p2)
    {
      return 0;
    }

    DateWrapper.IsDateExact(p1.BirthDate, out IDateExact p1Birthdate);
    DateWrapper.IsDateExact(p2.BirthDate, out IDateExact p2Birthdate);

    if (p1Birthdate == null || p2Birthdate == null)
    {
      if (p1.BirthDate == p2.BirthDate)
      {
        // The days are the same so check the first name
        return (string.Compare(p1.FirstName, p2.FirstName, true, CultureInfo.CurrentCulture));
      }

      return (string.Compare(p1.BirthDate.ToGedcom(), p2.BirthDate.ToGedcom(), true, CultureInfo.CurrentCulture));
    }

    // Check the month first
    if (p1Birthdate.Month < p2Birthdate.Month)
    {
      return -1;
    }
    else if (p1Birthdate.Month > p2Birthdate.Month)
    {
      return 1;
    }
    else
    {
      // Since the months were the same, now check the day
      if (p1Birthdate.Day < p2Birthdate.Day)
      {
        return -1;
      }
      else if (p1Birthdate.Day > p2Birthdate.Day)
      {
        return 1;
      }
      else
      {
        // The days are the same so check the first name
        return (string.Compare(p1.FirstName, p2.FirstName, true, CultureInfo.CurrentCulture));
      }
    }

  }

  #endregion
}
