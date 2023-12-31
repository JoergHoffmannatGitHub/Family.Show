using System;
using System.Windows;
using System.Windows.Controls;

using FamilyShowLib;

namespace FamilyShow;

/// <summary>
/// Interaction logic for NewUserControl.xaml
/// </summary>

public partial class NewUserControl : UserControl
{
  #region fields

  // The list of people, this is a global list shared by the application.
  private readonly PeopleCollection _family = App.Family;

  #endregion

  public NewUserControl()
  {
    InitializeComponent();

    SetDefaultFocus();
  }

  #region routed events

  public static readonly RoutedEvent AddButtonClickEvent = EventManager.RegisterRoutedEvent(
    "AddButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewUserControl));

  // Expose this event for this control's container
  public event RoutedEventHandler AddButtonClick
  {
    add { AddHandler(AddButtonClickEvent, value); }
    remove { RemoveHandler(AddButtonClickEvent, value); }
  }

  #endregion

  #region event handlers

  private void AddButton_Click(object sender, RoutedEventArgs e)
  {
    // Create a new person with the specified inputs
    Person newPerson = new(NamesInputTextBox.Text, SurnameInputTextBox.Text)
    {
      // Setup the properties based on the input
      Gender = ((bool)MaleRadioButton.IsChecked) ? Gender.Male : Gender.Female,
      BirthPlace = BirthPlaceInputTextBox.Text,
      IsLiving = true
    };

    DateTime birthdate = App.StringToDate(BirthDateInputTextBox.Text);
    if (birthdate != DateTime.MinValue)
    {
      newPerson.BirthDate = birthdate;
    }

    _family.Current = newPerson;
    _family.Add(newPerson);
    _family.OnContentChanged();

    RaiseEvent(new RoutedEventArgs(AddButtonClickEvent));
  }

  #endregion

  #region helper methods

  public void SetDefaultFocus()
  {
    // Set the focus to the first name textbox for quick entry
    NamesInputTextBox.Focus();
  }

  /// <summary>
  /// Clear the input fields
  /// </summary>
  public void ClearInputFields()
  {
    NamesInputTextBox.Clear();
    SurnameInputTextBox.Clear();
    BirthDateInputTextBox.Clear();
    BirthPlaceInputTextBox.Clear();
    MaleRadioButton.IsChecked = true;
  }

  #endregion
}
