using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using FamilyShowLib;

namespace FamilyShow.Controls.FamilyData;

/// <summary>
/// Interaction logic for IndividualEventDate.xaml
/// A reusable custom control for editing dates with descriptors
/// </summary>
public partial class IndividualEventDate : UserControl
{
    public IndividualEventDate()
    {
        InitializeComponent();
    }

    #region Dependency Properties

    /// <summary>
    /// The date value being edited
    /// </summary>
    public static readonly DependencyProperty DateProperty =
        DependencyProperty.Register("Date", typeof(DateWrapper), typeof(IndividualEventDate),
            new PropertyMetadata(null));

    public DateWrapper Date
    {
        get { return (DateWrapper)GetValue(DateProperty); }
        set { SetValue(DateProperty, value); }
    }

    /// <summary>
    /// The date descriptor (e.g., "abt", "bef", "aft")
    /// </summary>
    public static readonly DependencyProperty DateDescriptorProperty =
        DependencyProperty.Register("DateDescriptor", typeof(string), typeof(IndividualEventDate),
            new PropertyMetadata(string.Empty));

    public string DateDescriptor
    {
        get { return (string)GetValue(DateDescriptorProperty); }
        set { SetValue(DateDescriptorProperty, value); }
    }

    /// <summary>
    /// The label text for the date field (e.g., "Date of Birth")
    /// </summary>
    public static readonly DependencyProperty DateLabelTextProperty =
        DependencyProperty.Register("DateLabelText", typeof(string), typeof(IndividualEventDate),
            new PropertyMetadata("Date"));

    public string DateLabelText
    {
        get { return (string)GetValue(DateLabelTextProperty); }
        set { SetValue(DateLabelTextProperty, value); }
    }

    /// <summary>
    /// Whether the control is enabled
    /// </summary>
    public static readonly DependencyProperty IsControlEnabledProperty =
        DependencyProperty.Register("IsControlEnabled", typeof(bool), typeof(IndividualEventDate),
            new PropertyMetadata(true));

    public bool IsControlEnabled
    {
        get { return !(bool)GetValue(IsControlEnabledProperty); }
        set { SetValue(IsControlEnabledProperty, !value); }
    }

    /// <summary>
    /// Whether to show the checkbox (e.g., for "Living" checkbox on birth dates)
    /// </summary>
    public static readonly DependencyProperty ShowCheckBoxProperty =
        DependencyProperty.Register("ShowCheckBox", typeof(bool), typeof(IndividualEventDate),
            new PropertyMetadata(false));

    public bool ShowCheckBox
    {
        get { return (bool)GetValue(ShowCheckBoxProperty); }
        set { SetValue(ShowCheckBoxProperty, value); }
    }

    /// <summary>
    /// The text for the checkbox
    /// </summary>
    public static readonly DependencyProperty CheckBoxTextProperty =
        DependencyProperty.Register("CheckBoxText", typeof(string), typeof(IndividualEventDate),
            new PropertyMetadata("Living"));

    public string CheckBoxText
    {
        get { return (string)GetValue(CheckBoxTextProperty); }
        set { SetValue(CheckBoxTextProperty, value); }
    }

    /// <summary>
    /// The value of the checkbox
    /// </summary>
    public static readonly DependencyProperty CheckBoxValueProperty =
        DependencyProperty.Register("CheckBoxValue", typeof(bool?), typeof(IndividualEventDate),
            new PropertyMetadata(true));

    public bool? CheckBoxValue
    {
        get { return (bool?)GetValue(CheckBoxValueProperty); }
        set { SetValue(CheckBoxValueProperty, value); }
    }

    #endregion

    #region Routed Events

    /// <summary>
    /// Routed event for when the date descriptor changes
    /// </summary>
    public static readonly RoutedEvent DateDescriptorChangedEvent =
        EventManager.RegisterRoutedEvent("DateDescriptorChanged", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(IndividualEventDate));

    public event RoutedEventHandler DateDescriptorChanged
    {
        add { AddHandler(DateDescriptorChangedEvent, value); }
        remove { RemoveHandler(DateDescriptorChangedEvent, value); }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle mouse enter for the label to show it's interactive
    /// </summary>
    private void Label_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Label label && label.IsEnabled)
        {
            label.Foreground = System.Windows.Media.Brushes.LightSteelBlue;
            Mouse.OverrideCursor = Cursors.Hand;
        }
    }

    /// <summary>
    /// Handle mouse leave for the label
    /// </summary>
    private void Label_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Label label && label.IsEnabled)
        {
            label.Foreground = System.Windows.Media.Brushes.White;
        }
        Mouse.OverrideCursor = null;
    }

    /// <summary>
    /// Handle left-click to change descriptor forward
    /// </summary>
    private void ChangeDateDescriptorForward(object sender, RoutedEventArgs e)
    {
        if (sender is Label label && label.IsEnabled)
        {
            DateDescriptor = ForwardDateDescriptor(DateDescriptor);
            RaiseEvent(new RoutedEventArgs(DateDescriptorChangedEvent));
        }
    }

    /// <summary>
    /// Handle right-click to change descriptor backward
    /// </summary>
    private void ChangeDateDescriptorBackward(object sender, RoutedEventArgs e)
    {
        if (sender is Label label && label.IsEnabled)
        {
            DateDescriptor = BackwardDateDescriptor(DateDescriptor);
            RaiseEvent(new RoutedEventArgs(DateDescriptorChangedEvent));
        }
    }

    /// <summary>
    /// Handle tooltip opening for the date textbox
    /// </summary>
    private void ToolTip_DateTextBox(object sender, ToolTipEventArgs e)
    {
        // Find the parent Details control and call its ToolTip_All method
        Details detailsControl = FindParentOfType<Details>(this);
        if (detailsControl != null)
        {
            detailsControl.ToolTip_All(sender, e);
        }
        else
        {
            // Fallback if Details control not found
            if (sender is TextBox textBox)
            {
                textBox.ToolTip = "Enter date in format: MM/DD/YYYY, DD MMM YYYY, or YYYY\nRight-click label to change descriptor (abt, bef, aft, etc.)";
            }
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Helper method to find a parent control of a specific type
    /// </summary>
    private static T FindParentOfType<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parentObject = VisualTreeHelper.GetParent(child);
        if (parentObject == null)
        {
            return null;
        }

        if (parentObject is T parent)
        {
            return parent;
        }

        return FindParentOfType<T>(parentObject);
    }

    /// <summary>
    /// Move to the next date descriptor in sequence
    /// </summary>
    private static string ForwardDateDescriptor(string currentDescriptor)
    {
        return (currentDescriptor != null ? currentDescriptor.ToUpper() : string.Empty) switch
        {
            "" => "ABT ",
            "ABT " => "AFT ",
            "AFT " => "BEF ",
            "BEF " => "BET ",
            "BET " => "CAL ",
            "CAL " => "EST ",
            _ => "",
        };
    }

    /// <summary>
    /// Move to the previous date descriptor in sequence
    /// </summary>
    private static string BackwardDateDescriptor(string currentDescriptor)
    {
        return (currentDescriptor != null ? currentDescriptor.ToUpper() : string.Empty) switch
        {
            "" => "EST ",
            "EST " => "CAL ",
            "CAL " => "BET ",
            "BET " => "BEF ",
            "BEF " => "AFT ",
            "AFT " => "ABT ",
            _ => ""
        };
    }

    #endregion
}
