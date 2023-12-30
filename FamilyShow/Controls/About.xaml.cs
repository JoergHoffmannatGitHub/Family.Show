using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Microsoft.FamilyShow;

/// <summary>
/// Interaction logic for About.xaml
/// </summary>
public partial class About : System.Windows.Controls.UserControl
{
  public About()
  {
    InitializeComponent();
    DisplayVersion();
    DisplayCopyright();
  }

  #region routed events

  public static readonly RoutedEvent CloseButtonClickEvent = EventManager.RegisterRoutedEvent(
      "CloseButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(About));

  // Expose this event for this control's container
  public event RoutedEventHandler CloseButtonClick
  {
    add { AddHandler(CloseButtonClickEvent, value); }
    remove { RemoveHandler(CloseButtonClickEvent, value); }
  }

  #endregion

  #region methods

  private void CloseButton_Click(object sender, RoutedEventArgs e)
  {
    RaiseEvent(new RoutedEventArgs(CloseButtonClickEvent));
  }

  #endregion

  #region helper methods

  /// <summary>
  /// Display the application version.
  /// </summary>
  private void DisplayVersion()
  {
    Version version = Assembly.GetExecutingAssembly().GetName().Version;
    VersionLabel.Content += string.Format(CultureInfo.CurrentCulture,
        "{0}.{1}.{2}", version.Major, version.Minor, version.Build);
  }

  /// <summary>
  /// Display the application version.
  /// </summary>
  private void DisplayCopyright()
  {
    // Get the application information.
    AssemblyCopyrightAttribute attribute = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyCopyrightAttribute>().FirstOrDefault();
    CopyrightLabel.Content = string.Format(CultureInfo.CurrentCulture, "Copyright {0}", attribute.Copyright);
  }

  private void Homepage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
  {
    // Open the GitHub website in the user's default browser
    try
    {
      Process.Start(new ProcessStartInfo
      {
        FileName = "https://github.com/fredatgithub/FamilyShow",
        UseShellExecute = true
      });
    }
    catch { }
  }

  #endregion

}
