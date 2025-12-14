using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using FamilyShowLib;

namespace FamilyShow.Controls;

/// <summary>
/// Interaction logic for DateEditControl.xaml
/// A reusable custom control for editing dates with descriptors
/// </summary>
public partial class DateEditControl : UserControl
{
    public DateEditControl()
    {
        InitializeComponent();
    }

    #region Dependency Properties

    /// <summary>
    /// The date value being edited
    /// </summary>
    public static readonly DependencyProperty DateProperty =
        DependencyProperty.Register("Date", typeof(DateWrapper), typeof(DateEditControl),
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
        DependencyProperty.Register("DateDescriptor", typeof(string), typeof(DateEditControl),
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
        DependencyProperty.Register("DateLabelText", typeof(string), typeof(DateEditControl),
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
        DependencyProperty.Register("IsControlEnabled", typeof(bool), typeof(DateEditControl),
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
        DependencyProperty.Register("ShowCheckBox", typeof(bool), typeof(DateEditControl),
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
        DependencyProperty.Register("CheckBoxText", typeof(string), typeof(DateEditControl),
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
        DependencyProperty.Register("CheckBoxValue", typeof(bool?), typeof(DateEditControl),
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
            typeof(RoutedEventHandler), typeof(DateEditControl));

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
            Mouse.OverrideCursor = Cursors.Hand;
        }
    }

    /// <summary>
    /// Handle mouse leave for the label
    /// </summary>
    private void Label_MouseLeave(object sender, MouseEventArgs e)
    {
        Mouse.OverrideCursor = null;
    }

    /// <summary>
    /// Handle left-click to change descriptor forward
    /// </summary>
    private void ChangeDateDescriptorForward(object sender, RoutedEventArgs e)
    {
        if (IsControlEnabled)
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
        if (IsControlEnabled)
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
        // This can be extended to show helpful date format information
        if (sender is TextBox textBox)
        {
            textBox.ToolTip = "Enter date in format: MM/DD/YYYY, DD MMM YYYY, or YYYY\nRight-click label to change descriptor (abt, bef, aft, etc.)";
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Move to the next date descriptor in sequence
    /// </summary>
    private static string ForwardDateDescriptor(string currentDescriptor)
    {
        return currentDescriptor switch
        {
            "" => "abt ",
            "abt " => "bef ",
            "bef " => "aft ",
            "aft " => "bet ",
            "bet " => "cal ",
            "cal " => "est ",
            "est " => "",
            _ => ""
        };
    }

    /// <summary>
    /// Move to the previous date descriptor in sequence
    /// </summary>
    private static string BackwardDateDescriptor(string currentDescriptor)
    {
        return currentDescriptor switch
        {
            "" => "est ",
            "abt " => "",
            "bef " => "abt ",
            "aft " => "bef ",
            "bet " => "aft ",
            "cal " => "bet ",
            "est " => "cal ",
            _ => ""
        };
    }

    #endregion
}
