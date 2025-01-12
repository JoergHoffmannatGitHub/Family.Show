using System;
using System.Windows;
using System.Windows.Controls;

using FamilyShowLib;

namespace FamilyShow;

/// <summary>
/// Interaction logic for DateCalculator.xaml
/// </summary>
public partial class DateCalculator : UserControl
{

  public DateCalculator()
  {
    InitializeComponent();
    AddSubtractComboBox.SelectedIndex = 0;
  }

  #region routed events

  public static readonly RoutedEvent CancelButtonClickEvent = EventManager.RegisterRoutedEvent(
      "CancelButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DateCalculator));

  public event RoutedEventHandler CancelButtonClick
  {
    add { AddHandler(CancelButtonClickEvent, value); }
    remove { RemoveHandler(CancelButtonClickEvent, value); }
  }

  #endregion

  #region methods

  /// <summary>
  /// Handler for the age, birthdate, deathdate handler.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  internal void CalculateButton_Click(object sender, RoutedEventArgs e)
  {
    // Reset the results
    DeathResult.Content = string.Empty;
    BirthResult.Content = string.Empty;
    AgeResult.Content = string.Empty;

    // Hide errors if showing
    HideErrors123();

    try
    {
      // Get any input dates
      DateTime? s1 = Date1TextBox.Text.ToDate();
      DateTime? s2 = Date2TextBox.Text.ToDate();

      if (!string.IsNullOrEmpty(Date1TextBox.Text))
      {
        Date1TextBox.Text = s1.ToShortString();
      }
      else
      {
        Date1TextBox.Text = string.Empty;
      }

      if (!string.IsNullOrEmpty(Date2TextBox.Text))
      {
        Date2TextBox.Text = s2.ToShortString();
      }
      else
      {
        Date2TextBox.Text = string.Empty;
      }

      int age = -1;

      // Get any input age
      if (!string.IsNullOrEmpty(AgeTextBox.Text))
      {
        age = int.Parse(AgeTextBox.Text);
      }

      // If a birth date and death date are specified, calculate an age
      if (!s1.IsNullOrEmpty() && !s2.IsNullOrEmpty() && age < 0)
      {
        AgeTextBox.Text = string.Empty;

        TimeSpan span = s2.Value.Subtract(s1.Value);

        if (span >= TimeSpan.Zero)
        {
          BirthResult.Content = s1.ToShortString();
          DeathResult.Content = s2.ToShortString();
          AgeResult.Content = Math.Round(span.Days / 365.25, 0, MidpointRounding.AwayFromZero) + " " + Properties.Resources.years;
        }
      }

      // If death data and age are specified, calculate a birth date
      if (s1.IsNullOrEmpty() && !s2.IsNullOrEmpty() && age >= 0)
      {
        int year = s2.Value.Year;
        int day = s2.Value.Day;
        int month = s2.Value.Month;

        year -= age;

        BirthResult.Content = new DateTime(year, month, day).ToShortString();
        DeathResult.Content = s2.ToShortString();
        AgeResult.Content = age + " " + Properties.Resources.years;
      }

      // If birth data and age are specified, calculate a death date
      if (!s1.IsNullOrEmpty() && s2.IsNullOrEmpty() && age >= 0)
      {
        int year = s1.Value.Year;
        int month = s1.Value.Month;
        int day = s1.Value.Day;

        year += age;

        BirthResult.Content = s1.ToShortString();
        DeathResult.Content = new DateTime(year, month, day).ToShortString();
        AgeResult.Content = age + " " + Properties.Resources.years;
      }
    }
    catch
    {
      // An error with the user input.  
      // Clear all results and show error icons.
      DeathResult.Content = string.Empty;
      BirthResult.Content = string.Empty;
      AgeResult.Content = string.Empty;
      ShowErrors123();
    }
  }

  /// <summary>
  /// Handler for the add/subtract operation.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  internal void Calculate2Button_Click(object sender, RoutedEventArgs e)
  {
    // Reset the results
    Result2.Content = "";

    // Hide errors if showing
    HideErrors4();

    if (!string.IsNullOrEmpty(ToBox.Text))
    {
      //Get the date input and try to add/subtract the specified number of days, months and years.
      DateTime? s = ToBox.Text.ToDate();
      if (!string.IsNullOrEmpty(ToBox.Text))
      {
        ToBox.Text = s.ToShortString();
      }
      else
      {
        return;
      }

      int i = 1;

      if (AddSubtractComboBox.SelectedIndex == 0)
      {
        i = 1;
      }

      if ((AddSubtractComboBox.SelectedIndex == 1))
      {
        i = -1;
      }

      double days = 0;
      int months = 0;
      int years = 0;

      try { days = double.Parse(DayBox.Text); }
      catch { }

      try { months = int.Parse(MonthBox.Text); }
      catch { }

      try { years = int.Parse(YearBox.Text); }
      catch { }

      try
      {
        s = s.Value.AddDays(days * i);
        s = s.Value.AddMonths(months * i);
        s = s.Value.AddYears(years * i);
        Result2.Content = s.Value.ToShortString();
      }
      catch
      {
        ShowErrors4();
      }
    }
  }

  private void CancelButton_Click(object sender, RoutedEventArgs e)
  {
    clear();
    RaiseEvent(new RoutedEventArgs(CancelButtonClickEvent));
  }

  #endregion

  #region helper methods

  private void HideErrors123()
  {
    Error1.Visibility = Visibility.Hidden;
    Error2.Visibility = Visibility.Hidden;
    Error3.Visibility = Visibility.Hidden;
  }

  private void ShowErrors123()
  {
    Error1.Visibility = Visibility.Visible;
    Error2.Visibility = Visibility.Visible;
    Error3.Visibility = Visibility.Visible;

  }

  private void ShowErrors4()
  {
    Error4.Visibility = Visibility.Visible;
  }

  private void HideErrors4()
  {
    Error4.Visibility = Visibility.Hidden;
  }

  private void clear()
  {
    //clear all the results and text input boxes
    Date1TextBox.Text = "";
    Date2TextBox.Text = "";
    AgeTextBox.Text = "";
    YearBox.Text = "";
    MonthBox.Text = "";
    DayBox.Text = "";
    ToBox.Text = "";

    DeathResult.Content = "";
    BirthResult.Content = "";
    AgeResult.Content = "";
    Result2.Content = "";

    //reset the error icons
    HideErrors123();
    HideErrors4();

  }

  #endregion

  private void AddSubtractComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
  {
    //When the user changes the add/subtract selection, update the descriptor to/from
    if (AddSubtractComboBox.SelectedItem != null)
    {
      if (AddSubtractComboBox.SelectedIndex == 0)
      {
        DateTo.Content = Properties.Resources.To.ToLower();
      }
      else
      {
        DateTo.Content = Properties.Resources.From.ToLower();
      }
    }
  }

}
