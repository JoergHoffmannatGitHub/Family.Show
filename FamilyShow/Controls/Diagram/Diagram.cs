/*
 * Arranges nodes based on relationships. Contains a series or rows (DiagramRow), 
 * each row contains a series of groups (DiagramGroup), and each group contains a 
 * series of nodes (DiagramNode).
 * 
 * Contains a list of connections. Each connection describes where the node
 * is located in the diagram and type of connection. The lines are draw
 * during OnRender.
 * 
 * Diagram is responsible for managing the rows. The logic that populates the rows
 * and understand all of the relationships is contained in DiagramLogic.
 *
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

using FamilyShowLib;

namespace FamilyShow;

/// <summary>
/// Diagram that lays out and displays the nodes.
/// </summary>
internal class Diagram : FrameworkElement
{
  #region fields

  private static class Const
  {
    // Duration to pause before displaying new nodes.
    public static double AnimationPauseDuration = 600;

    // Duration for nodes to fade in when the diagram is repopulated.
    public static double NodeFadeInDuration = 500;

    // Duration for the new person animation.
    public static double NewPersonAnimationDuration = 250;

    // Group space.
    public static double PrimaryRowGroupSpace = 20;
    public static double ChildRowGroupSpace = 20;
    public static double ParentRowGroupSpace = 40;

    // Amount of space between each row.
    public static double RowSpace = 40;

    // Scale multiplier for spouse and siblings.
    public static double RelatedMultiplier = 0.8;

    // Scale multiplier for each future generation row.
    public static double GenerationMultiplier = 0.9;
  }

  // List of rows in the diagram. Each row contains groups, and each group contains nodes.
  private readonly List<DiagramRow> _rows = [];

  // Populates the rows with nodes.
  private readonly DiagramLogic _logic;

  // Size of the diagram. Used to layout all of the nodes before the
  // control gets an actual size.
  private Size _totalSize = new(0, 0);

  // Zoom level of the diagram.
  private double _scale;

  // Bounding area of the selected node, the selected node is the 
  // non-primary node that is selected, and will become the primary node.
  private Rect _selectedNodeBounds = Rect.Empty;

  // Flag if currently populating or not. Necessary since diagram populate 
  // contains several parts and animations, request to update the diagram
  // are ignored when this flag is set.
  private bool _populating;

  // The person that has been added to the diagram.
  private Person _newPerson;

  // Timer used with the repopulating animation.
  private readonly DispatcherTimer _animationTimer = new();

  // Flag if the row and group borders should be drawn.
  public static bool displayBorder = Properties.Settings.Default.ShowOutline;

  // Flags for filtering what is displayed.  Use values from the users settings file.
  public static bool hideInLaws = Properties.Settings.Default.ShowInLaws;
  public static bool hideAncestors = Properties.Settings.Default.ShowAncestors;
  public static bool hideDescendants = Properties.Settings.Default.ShowDescendants;
  public static bool hideSiblings = Properties.Settings.Default.ShowSiblings;
  public static bool hideAuntsUncles = Properties.Settings.Default.ShowAuntsUncles;
  public static bool hideSpouses = Properties.Settings.Default.ShowCurrentSpouse;
  public static bool hidePreviousSpouses = Properties.Settings.Default.ShowPreviousSpouse;
  public static bool showImmediateFamily = Properties.Settings.Default.ShowImmediateFamily;
  public static bool showPhotos = Properties.Settings.Default.ShowPhotos;
  public static bool showGenerations = Properties.Settings.Default.ShowGenerations;
  public static bool showBloodlines = Properties.Settings.Default.ShowBloodlines;

  //Flag for displaying date information.
  public static bool displayDates = Properties.Settings.Default.ShowDates;
  public static bool displayFiltered = Properties.Settings.Default.ShowFiltered;

  #endregion

  #region events

  public event EventHandler DiagramUpdated;

  private void OnDiagramUpdated()
  {
    DiagramUpdated?.Invoke(this, EventArgs.Empty);
  }

  public event EventHandler DiagramPopulated;
  private void OnDiagramPopulated()
  {
    DiagramPopulated?.Invoke(this, EventArgs.Empty);
  }

  #endregion

  #region properties

  /// <summary>
  /// Gets or sets the zoom level of the diagram.
  /// </summary>
  public double Scale
  {
    get { return _scale; }
    set
    {
      if (_scale != value)
      {
        _scale = value;
        LayoutTransform = new ScaleTransform(_scale, _scale);
      }
    }
  }

  /// <summary>
  /// Sets the display year filter.
  /// </summary>
  public double DisplayYear
  {
    set
    {
      // Filter nodes and connections based on the year.
      _logic.DisplayYear = value;
      InvalidateVisual();
    }
  }

  /// <summary>
  /// Gets the minimum year specified in the nodes and connections.
  /// </summary>
  public double MinimumYear
  {
    get { return _logic.MinimumYear; }
  }

  /// <summary>
  /// Gets the bounding area (relative to the diagram) of the primary node.
  /// </summary>
  public Rect PrimaryNodeBounds
  {
    get { return _logic.GetNodeBounds(_logic.Family.Current); }
  }

  /// <summary>
  /// Gets the bounding area (relative to the diagram) of the selected node.
  /// The selected node is the non-primary node that was previously selected
  /// to be the primary node.
  /// </summary>
  public Rect SelectedNodeBounds
  {
    get { return _selectedNodeBounds; }
  }

  /// <summary>
  /// Gets the number of nodes in the diagram.
  /// </summary>
  public int NodeCount
  {
    get { return _logic.PersonLookup.Count; }
  }

  #endregion

  public Diagram()
  {
    // Init the diagram logic, which handles all of the layout logic.
    _logic = new DiagramLogic(VisualTreeHelper.GetDpi(this))
    {
      NodeClickHandler = new RoutedEventHandler(OnNodeClick)
    };

    // Can have an empty People collection when in design tools such as Blend.
    if (_logic.Family != null)
    {
      _logic.Family.ContentChanged += new EventHandler<ContentChangedEventArgs>(OnFamilyContentChanged);
      _logic.Family.CurrentChanged += new EventHandler(OnFamilyCurrentChanged);
    }
  }

  #region layout

  protected override void OnInitialized(EventArgs e)
  {
    // Context menu so can display row and group borders.
    ContextMenu = new ContextMenu();

    MenuItem outline = new()
    {
      Header = Properties.Resources.ShowGenerationOutline
    };
    outline.Click += new RoutedEventHandler(OnToggleBorderClick);
    outline.Foreground = SystemColors.MenuTextBrush;

    MenuItem generations = new()
    {
      Header = Properties.Resources.ShowAllGenerations
    };
    generations.Click += new RoutedEventHandler(OnToggleNodeCount);
    generations.Foreground = SystemColors.MenuTextBrush;

    MenuItem dates = new()
    {
      Header = Properties.Resources.HideDates
    };
    dates.Click += new RoutedEventHandler(OnToggleDate);
    dates.Foreground = SystemColors.MenuTextBrush;

    MenuItem ancestors = new()
    {
      Header = Properties.Resources.HideAncestors
    };
    ancestors.Click += new RoutedEventHandler(OnToggleAncestors);
    ancestors.Foreground = SystemColors.MenuTextBrush;

    MenuItem descendants = new()
    {
      Header = Properties.Resources.HideDescendants
    };
    descendants.Click += new RoutedEventHandler(OnToggleDescendants);
    descendants.Foreground = SystemColors.MenuTextBrush;

    MenuItem siblings = new()
    {
      Header = Properties.Resources.HideSiblings
    };
    siblings.Click += new RoutedEventHandler(OnToggleSiblings);
    siblings.Foreground = SystemColors.MenuTextBrush;

    MenuItem auntsUncles = new()
    {
      Header = Properties.Resources.HideAuntsAndUncles
    };
    auntsUncles.Click += new RoutedEventHandler(OnToggleAuntsUncles);
    auntsUncles.Foreground = SystemColors.MenuTextBrush;

    MenuItem inLaws = new()
    {
      Header = Properties.Resources.HideInLaws
    };
    inLaws.Click += new RoutedEventHandler(OnToggleInLaws);
    inLaws.Foreground = SystemColors.MenuTextBrush;

    MenuItem currentSpouse = new()
    {
      Header = Properties.Resources.HideCurrentSpouse
    };
    currentSpouse.Click += new RoutedEventHandler(OnToggleSpouses);
    currentSpouse.Foreground = SystemColors.MenuTextBrush;

    MenuItem previousSpouse = new()
    {
      Header = Properties.Resources.HidePreviousSpouses
    };
    previousSpouse.Click += new RoutedEventHandler(OnTogglePreviousSpouses);
    previousSpouse.Foreground = SystemColors.MenuTextBrush;

    MenuItem immediate = new()
    {
      Header = Properties.Resources.ShowImmediateFamilyOnly
    };
    immediate.Click += new RoutedEventHandler(OnToggleImmediateFamily);
    immediate.Foreground = SystemColors.MenuTextBrush;

    MenuItem photos = new()
    {
      Header = Properties.Resources.ShowPhotos
    };
    photos.Click += new RoutedEventHandler(OnTogglePhotos);
    photos.Foreground = SystemColors.MenuTextBrush;

    MenuItem bloodlines = new()
    {
      Header = Properties.Resources.ShowBloodlines
    };
    bloodlines.Click += new RoutedEventHandler(OnToggleBloodlines);
    bloodlines.Foreground = SystemColors.MenuTextBrush;

    MenuItem filtered = new()
    {
      Header = Properties.Resources.HideFiltered
    };
    filtered.Click += new RoutedEventHandler(OnToggleFiltered);
    filtered.Foreground = SystemColors.MenuTextBrush;

    ContextMenu.Items.Add(immediate); //1
    ContextMenu.Items.Add(generations);//2
    ContextMenu.Items.Add(descendants);//3
    ContextMenu.Items.Add(ancestors); //4

    ContextMenu.Items.Add(siblings);//5
    ContextMenu.Items.Add(auntsUncles);//6

    ContextMenu.Items.Add(inLaws);//7
    ContextMenu.Items.Add(currentSpouse);//8
    ContextMenu.Items.Add(previousSpouse);//9

    ContextMenu.Items.Add(new Separator());//10

    ContextMenu.Items.Add(dates);//11
    ContextMenu.Items.Add(photos);//12
    ContextMenu.Items.Add(outline);//13
    ContextMenu.Items.Add(bloodlines);//14
    ContextMenu.Items.Add(filtered);//15

    outline.IsChecked = displayBorder;
    photos.IsChecked = showPhotos;
    dates.IsChecked = !displayDates;
    previousSpouse.IsChecked = hidePreviousSpouses;
    currentSpouse.IsChecked = hideSpouses;
    inLaws.IsChecked = hideInLaws;
    auntsUncles.IsChecked = hideAuntsUncles;
    siblings.IsChecked = hideSiblings;
    ancestors.IsChecked = hideAncestors;
    descendants.IsChecked = hideDescendants;
    generations.IsChecked = showGenerations;
    immediate.IsChecked = showImmediateFamily;
    bloodlines.IsChecked = showBloodlines;
    filtered.IsChecked = !displayFiltered;


    UpdateDiagram();
    base.OnInitialized(e);
  }

  protected override int VisualChildrenCount
  {
    // Return the number of rows.
    get { return _rows.Count; }
  }

  protected override Visual GetVisualChild(int index)
  {
    // Return the requested row.
    return _rows[index];
  }

  protected override Size MeasureOverride(Size availableSize)
  {
    // Let each row determine how large they want to be.
    Size size = new(double.PositiveInfinity, double.PositiveInfinity);
    foreach (DiagramRow row in _rows)
    {
      row.Measure(size);
    }

    // Return the total size of the diagram.
    return ArrangeRows(false);
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    // Arrange the rows in the diagram, return the total size.
    return ArrangeRows(true);
  }

  /// <summary>
  /// Arrange the rows in the diagram, return the total size.
  /// </summary>
  private Size ArrangeRows(bool arrange)
  {
    // Location of the row.
    double pos = 0;

    // Bounding area of the row.
    Rect bounds = new();

    // Total size of the diagram.
    Size size = new(0, 0);

    foreach (DiagramRow row in _rows)
    {
      // Row location, center the row horizontaly.
      bounds.Y = pos;
      bounds.X = (_totalSize.Width == 0) ? 0 :
          bounds.X = (_totalSize.Width - row.DesiredSize.Width) / 2;

      // Row Size.
      bounds.Width = row.DesiredSize.Width;
      bounds.Height = row.DesiredSize.Height;

      // Arrange the row, save the location.
      if (arrange)
      {
        row.Arrange(bounds);
        row.Location = bounds.TopLeft;
      }

      // Update the size of the diagram.
      size.Width = Math.Max(size.Width, bounds.Width);
      size.Height = pos + row.DesiredSize.Height;

      pos += bounds.Height;
    }

    // Store the size, this is necessary so the diagram
    // can be laid out without a valid Width property.
    _totalSize = size;
    return size;
  }

  #endregion

  /// <summary>
  /// Draw the connector lines at a lower level (OnRender) instead
  /// of creating visual tree objects.
  /// </summary>
  protected override void OnRender(DrawingContext drawingContext)
  {

    if (displayBorder)
    {

      double connectionTextSize = (double)Application.Current.TryFindResource("ConnectionTextSize");
      FontFamily connectionTextFont = (FontFamily)Application.Current.TryFindResource("ConnectionTextFont");

      int rownumber = 1;
      int labelnumber = 1;
      // Draws borders around the rows and groups.
      foreach (DiagramRow row in _rows)
      {
        // Display row border.

        int generationNumber = labelnumber;
        FormattedText format = new(" " + generationNumber,
        System.Globalization.CultureInfo.CurrentUICulture,
        FlowDirection.LeftToRight, new Typeface(connectionTextFont,
        FontStyles.Normal, FontWeights.Normal, FontStretches.Normal,
        connectionTextFont), connectionTextSize, Brushes.PaleVioletRed,
        VisualTreeHelper.GetDpi(this).PixelsPerDip);

        if (rownumber != _rows.Count)
        {
          Vector size = new(row.DesiredSize.Width + 10, row.DesiredSize.Height - 5);
          Rect bounds = new(row.Location, size);
          bounds.Offset(-5, -30);

          if (size.Y > 50)
          {
            drawingContext.DrawRectangle(null, new Pen(Brushes.PaleVioletRed, 0.5), bounds);
            drawingContext.DrawText(format, bounds.TopLeft);
            labelnumber++;
          }
        }
        else
        {
          Vector size = new(row.DesiredSize.Width + 10, row.DesiredSize.Height + 35);
          Rect bounds = new(row.Location, size);
          bounds.Offset(-5, -30);

          if (size.Y > 50)
          {
            drawingContext.DrawRectangle(null, new Pen(Brushes.PaleVioletRed, 0.5), bounds);
            drawingContext.DrawText(format, bounds.TopLeft);
            labelnumber++;
          }

        }

        rownumber++;
      }
    }

    if (displayDates == false)
    {
      foreach (DiagramConnectorNode connector in _logic.PersonLookup.Values)
      {
        connector.Node.HideBottomLabel();
      }

      foreach (DiagramConnector connector in _logic.Connections)
      {
        connector.ShowDate = false;
      }
    }
    else
    {
      foreach (DiagramConnectorNode connector in _logic.PersonLookup.Values)
      {
        connector.Node.UpdateBottomLabel();
      }

      foreach (DiagramConnector connector in _logic.Connections)
      {
        connector.ShowDate = true;
      }
    }

    // Draw child connectors first, so marriage information appears on top.
    foreach (DiagramConnector connector in _logic.Connections)
    {
      if (connector.IsChildConnector)
      {
        connector.Draw(drawingContext);
      }
    }

    // Draw all other non-child connectors.
    foreach (DiagramConnector connector in _logic.Connections)
    {
      if (!connector.IsChildConnector)
      {
        connector.Draw(drawingContext);
      }
    }
  }

  private void OnToggleBorderClick(object sender, RoutedEventArgs e)
  {
    // Display or hide the row and group borders.
    displayBorder = !displayBorder;
    Properties.Settings.Default.ShowOutline = displayBorder;
    Properties.Settings.Default.Save();

    // Update check on menu.
    MenuItem menuItem = ContextMenu.Items[12] as MenuItem;
    menuItem.IsChecked = displayBorder;

    InvalidateVisual();
  }

  private void OnToggleBloodlines(object sender, RoutedEventArgs e)
  {
    // Display or hide the Bloodlines.
    showBloodlines = !showBloodlines;
    Properties.Settings.Default.ShowBloodlines = showBloodlines;
    Properties.Settings.Default.Save();

    // Update check on menu.
    MenuItem menuItem = ContextMenu.Items[13] as MenuItem;
    menuItem.IsChecked = showBloodlines;

    InvalidateVisual();
    UpdateDiagram();
  }

  private void OnToggleNodeCount(object sender, RoutedEventArgs e)
  {
    // Display or hide additional generations
    showGenerations = !showGenerations;
    Properties.Settings.Default.ShowGenerations = showGenerations;
    Properties.Settings.Default.Save();

    // Update check on menu.
    MenuItem menuItem = ContextMenu.Items[1] as MenuItem;
    menuItem.IsChecked = showGenerations;

    InvalidateVisual();
    OnDiagramUpdated();
    UpdateDiagram();

  }

  private void OnToggleDate(object sender, RoutedEventArgs e)
  {

    // Display or hide the dates
    displayDates = !displayDates;
    Properties.Settings.Default.ShowDates = displayDates;
    Properties.Settings.Default.Save();

    // Update check on menu.
    MenuItem menuItem = ContextMenu.Items[10] as MenuItem;
    menuItem.IsChecked = !displayDates;

    InvalidateVisual();
    UpdateDiagram();
  }

  private void OnToggleFiltered(object sender, RoutedEventArgs e)
  {

    // Display or hide filtered people
    displayFiltered = !displayFiltered;
    Properties.Settings.Default.ShowFiltered = displayFiltered;
    Properties.Settings.Default.Save();

    // Update check on menu.
    MenuItem menuItem = ContextMenu.Items[14] as MenuItem;
    menuItem.IsChecked = !displayFiltered;

    InvalidateVisual();
    OnDiagramUpdated();
    UpdateDiagram();
  }

  private void OnToggleInLaws(object sender, RoutedEventArgs e)
  {

    // Display or hide the In laws
    hideInLaws = !hideInLaws;
    Properties.Settings.Default.ShowInLaws = hideInLaws;
    Properties.Settings.Default.Save();

    // Update check on menu.
    MenuItem menuItem = ContextMenu.Items[6] as MenuItem;
    menuItem.IsChecked = hideInLaws;

    InvalidateVisual();
    OnDiagramUpdated();
    UpdateDiagram();
  }

  private void OnToggleAncestors(object sender, RoutedEventArgs e)
  {

    // Display or hide ancestors
    hideAncestors = !hideAncestors;
    Properties.Settings.Default.ShowAncestors = hideAncestors;
    Properties.Settings.Default.Save();

    // Update check on menu.
    MenuItem menuItem = ContextMenu.Items[3] as MenuItem;
    menuItem.IsChecked = hideAncestors;

    InvalidateVisual();
    OnDiagramUpdated();
    UpdateDiagram();
  }

  private void OnToggleImmediateFamily(object sender, RoutedEventArgs e)
  {

    // Display or hide immediate family
    showImmediateFamily = !showImmediateFamily;
    Properties.Settings.Default.ShowImmediateFamily = showImmediateFamily;
    Properties.Settings.Default.Save();

    // Update check on menu.
    MenuItem menuItem = ContextMenu.Items[0] as MenuItem;
    menuItem.IsChecked = showImmediateFamily;

    InvalidateVisual();
    OnDiagramUpdated();
    UpdateDiagram();
  }

  private void OnToggleDescendants(object sender, RoutedEventArgs e)
  {

    // Display or hide the descendants
    hideDescendants = !hideDescendants;
    Properties.Settings.Default.ShowDescendants = hideDescendants;
    Properties.Settings.Default.Save();

    //// Update check on menu.
    MenuItem menuItem = ContextMenu.Items[2] as MenuItem;
    menuItem.IsChecked = hideDescendants;

    InvalidateVisual();
    OnDiagramUpdated();
    UpdateDiagram();
  }

  private void OnToggleSiblings(object sender, RoutedEventArgs e)
  {

    // Display or hide the siblings
    hideSiblings = !hideSiblings;
    Properties.Settings.Default.ShowSiblings = hideSiblings;
    Properties.Settings.Default.Save();

    //// Update check on menu.
    MenuItem menuItem = ContextMenu.Items[4] as MenuItem;
    menuItem.IsChecked = hideSiblings;

    InvalidateVisual();
    OnDiagramUpdated();
    UpdateDiagram();
  }

  private void OnToggleAuntsUncles(object sender, RoutedEventArgs e)
  {

    // Display or hide the aunts and uncles
    hideAuntsUncles = !hideAuntsUncles;
    Properties.Settings.Default.ShowAuntsUncles = hideAuntsUncles;
    Properties.Settings.Default.Save();

    //// Update check on menu.
    MenuItem menuItem = ContextMenu.Items[5] as MenuItem;
    menuItem.IsChecked = hideAuntsUncles;

    InvalidateVisual();
    OnDiagramUpdated();
    UpdateDiagram();

  }

  private void OnTogglePreviousSpouses(object sender, RoutedEventArgs e)
  {

    // Display or hide the previous spouses
    hidePreviousSpouses = !hidePreviousSpouses;
    Properties.Settings.Default.ShowPreviousSpouse = hidePreviousSpouses;
    Properties.Settings.Default.Save();

    //// Update check on menu.
    MenuItem menuItem = ContextMenu.Items[8] as MenuItem;
    menuItem.IsChecked = hidePreviousSpouses;

    InvalidateVisual();
    OnDiagramUpdated();
    UpdateDiagram();
  }

  private void OnToggleSpouses(object sender, RoutedEventArgs e)
  {

    // Display or hide the spouses
    hideSpouses = !hideSpouses;
    Properties.Settings.Default.ShowCurrentSpouse = hideSpouses;
    Properties.Settings.Default.Save();

    //// Update check on menu.
    MenuItem menuItem = ContextMenu.Items[7] as MenuItem;
    menuItem.IsChecked = hideSpouses;

    InvalidateVisual();
    OnDiagramUpdated();
    UpdateDiagram();
  }

  private void OnTogglePhotos(object sender, RoutedEventArgs e)
  {

    // Display or hide the photos
    showPhotos = !showPhotos;
    Properties.Settings.Default.ShowPhotos = showPhotos;
    Properties.Settings.Default.Save();

    //// Update check on menu.
    MenuItem menuItem = ContextMenu.Items[11] as MenuItem;
    menuItem.IsChecked = showPhotos;

    InvalidateVisual();
    UpdateDiagram();

  }

  #region diagram updates

  /// <summary>
  /// Reset all of the data associated with the diagram.
  /// </summary>
  private void Clear()
  {
    foreach (DiagramRow row in _rows)
    {
      row.Clear();
      RemoveVisualChild(row);
    }

    _rows.Clear();
    _logic.Clear();
  }

  /// <summary>
  /// Populate the diagram. Update the diagram and hide all non-primary nodes.
  /// Then pause, and finish the populate by fading in the new nodes.
  /// </summary>
  private void Populate()
  {
    // Set flag to ignore future updates until complete.
    _populating = true;

    // Update the nodes in the diagram.
    UpdateDiagram();

    // First hide all of the nodes except the primary node.
    foreach (DiagramConnectorNode connector in _logic.PersonLookup.Values)
    {
      if (connector.Node.Person != _logic.Family.Current)
      {
        connector.Node.Visibility = Visibility.Hidden;
      }
    }

    // Required to update (hide) the connector lines.            
    InvalidateVisual();
    InvalidateArrange();
    InvalidateMeasure();

    // Pause before displaying the new nodes.
    _animationTimer.Interval = App.GetAnimationDuration(Const.AnimationPauseDuration);
    _animationTimer.Tick += new EventHandler(OnAnimationTimer);
    _animationTimer.IsEnabled = true;

    // Let other controls know the diagram has been repopulated.
    OnDiagramPopulated();
  }

  /// <summary>
  /// The animation pause timer is complete, finish populating the diagram.
  /// </summary>
  private void OnAnimationTimer(object sender, EventArgs e)
  {
    // Turn off the timer.
    _animationTimer.IsEnabled = false;

    // Fade each node into view.
    foreach (DiagramConnectorNode connector in _logic.PersonLookup.Values)
    {
      if (connector.Node.Visibility != Visibility.Visible)
      {
        connector.Node.Visibility = Visibility.Visible;
        connector.Node.BeginAnimation(OpacityProperty,
            new DoubleAnimation(0, 1,
            App.GetAnimationDuration(Const.NodeFadeInDuration)));
      }
    }

    // Redraw connector lines.
    InvalidateVisual();

    _populating = false;
  }

  /// <summary>
  /// Reset the diagram with the nodes. This is accomplished by creating a series of rows.
  /// Each row contains a series of groups, and each group contains the nodes. The elements 
  /// are not laid out at this time. Also creates the connections between the nodes.
  /// </summary>
  private void UpdateDiagram()
  {
    int MaximumNodes = _logic.Family.Count;

    if (!showGenerations)
    {
      MaximumNodes = 50;
    }


    // Necessary for Blend.
    if (_logic.Family == null)
    {
      return;
    }

    // First reset everything.
    Clear();

    // Nothing to draw if there is not a primary person.
    if (_logic.Family.Current == null)
    {
      return;
    }

    // Primary row.
    Person primaryPerson = _logic.Family.Current;
    DiagramRow primaryRow = _logic.CreatePrimaryRow(primaryPerson, 1.0, Const.RelatedMultiplier, hideSiblings, hideSpouses, hidePreviousSpouses);
    primaryRow.GroupSpace = Const.PrimaryRowGroupSpace;
    AddRow(primaryRow);

    // Create as many rows as possible until exceed the max node limit.
    // Switch between child and parent rows to prevent only creating
    // child or parents rows (want to create as many of each as possible).
    int nodeCount = NodeCount;

    // The scale values of future generations, this makes the nodes
    // in each row slightly smaller.
    double nodeScale = 1.0;

    DiagramRow childRow = primaryRow;
    DiagramRow parentRow = primaryRow;

    while (nodeCount < MaximumNodes && parentRow != null && hideDescendants == true && hideAncestors == false)  //Const.
    {

      // Parent row.
      if (parentRow != null)
      {
        nodeScale *= Const.GenerationMultiplier;
        parentRow = AddParentRow(parentRow, nodeScale);
      }

      if (showImmediateFamily == true)
      {
        break;
      }

      // See if reached node limit yet.                                       
      nodeCount = NodeCount;
    }

    while (nodeCount < MaximumNodes && childRow != null && hideAncestors == true && hideDescendants == false)  //Const.
    {
      // Child Row.
      if (childRow != null)
      {
        childRow = AddChildRow(childRow);
      }

      if (showImmediateFamily == true)
      {
        break;
      }

      // See if reached node limit yet.                                       
      nodeCount = NodeCount;
    }


    while (nodeCount < MaximumNodes && (childRow != null || parentRow != null) && hideDescendants == false && hideAncestors == false)  //Const.
    {
      // Child Row.
      if (childRow != null)
      {
        childRow = AddChildRow(childRow);
      }

      // Parent row.
      if (parentRow != null)
      {
        nodeScale *= Const.GenerationMultiplier;
        parentRow = AddParentRow(parentRow, nodeScale);
      }

      if (showImmediateFamily == true)
      {
        break;
      }

      // See if reached node limit yet.                                       
      nodeCount = NodeCount;
    }

    // Raise event so others know the diagram was updated.
    OnDiagramUpdated();

    // Animate the new person (optional, might not be any new people).
    AnimateNewPerson();


  }

  /// <summary>
  /// Add a child row to the diagram.
  /// </summary>
  private DiagramRow AddChildRow(DiagramRow row)
  {
    // Get list of children for the current row.
    List<Person> children = DiagramLogic.GetChildren(row);
    if (children.Count == 0)
    {
      return null;
    }

    // Add bottom space to existing row.
    row.Margin = new Thickness(0, 0, 0, Const.RowSpace);

    // Add another row.
    DiagramRow childRow = _logic.CreateChildrenRow(children, 1.0, Const.RelatedMultiplier, hideInLaws);
    childRow.GroupSpace = Const.ChildRowGroupSpace;
    AddRow(childRow);
    return childRow;
  }

  /// <summary>
  /// Add a parent row to the diagram.
  /// </summary>
  private DiagramRow AddParentRow(DiagramRow row, double nodeScale)
  {
    // Get list of parents for the current row.
    Collection<Person> parents = DiagramLogic.GetParents(row);
    if (parents.Count == 0)
    {
      return null;
    }

    // Add another row.
    DiagramRow parentRow = _logic.CreateParentRow(parents, nodeScale, nodeScale * Const.RelatedMultiplier, hideAuntsUncles);
    parentRow.Margin = new Thickness(0, 0, 0, Const.RowSpace);
    parentRow.GroupSpace = Const.ParentRowGroupSpace;
    InsertRow(parentRow);
    return parentRow;
  }

  /// <summary>
  /// Add a row to the visual tree.
  /// </summary>
  private void AddRow(DiagramRow row)
  {
    if (row != null && row.NodeCount > 0)
    {
      AddVisualChild(row);
      _rows.Add(row);
    }
  }

  /// <summary>
  /// Insert a row in the visual tree.
  /// </summary>
  private void InsertRow(DiagramRow row)
  {
    if (row != null && row.NodeCount > 0)
    {
      AddVisualChild(row);
      _rows.Insert(0, row);
    }
  }

  #endregion

  /// <summary>
  /// Called when the current person in the main People collection changes.
  /// This means the diagram should be updated based on the new selected person.
  /// </summary>
  private void OnFamilyCurrentChanged(object sender, EventArgs e)
  {
    // Save the bounds for the current primary person, this 
    // is required later when animating the diagram.
    _selectedNodeBounds = _logic.GetNodeBounds(_logic.Family.Current);

    // Repopulate the diagram.
    Populate();
  }

  /// <summary>
  /// Called when data changed in the main People collection. This can be
  /// a new node added to the collection, updated Person details, and 
  /// updated relationship data.
  /// </summary>
  private void OnFamilyContentChanged(object sender, ContentChangedEventArgs e)
  {
    // Ignore if currently repopulating the diagram.
    if (_populating)
    {
      return;
    }

    // Save the person that is being added to the diagram.
    // This is optional and can be null.
    _newPerson = e.NewPerson;

    // Redraw the diagram.
    UpdateDiagram();
    InvalidateMeasure();
    InvalidateArrange();
    InvalidateVisual();
  }

  /// <summary>
  /// A node was clicked, make that node the primary node. 
  /// </summary>
  private void OnNodeClick(object sender, RoutedEventArgs e)
  {
    // Get the node that was clicked.
    if (sender is DiagramNode node)
    {
      // Make it the primary node. This raises the CurrentChanged
      // event, which repopulates the diagram.
      _logic.Family.Current = node.Person;
    }
  }

  /// <summary>
  /// Animate the new person that was added to the diagram.
  /// </summary>
  private void AnimateNewPerson()
  {
    // The new person is optional, can be null.
    if (_newPerson == null)
    {
      return;
    }

    // Get the UI element to animate.                
    DiagramNode node = _logic.GetDiagramNode(_newPerson);
    if (node != null)
    {
      // Create the new person animation.
      DoubleAnimation anim = new(0, 1,
          App.GetAnimationDuration(Const.NewPersonAnimationDuration));

      // Animate the node.
      ScaleTransform transform = new();
      transform.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
      transform.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
      node.RenderTransform = transform;

    }

    _newPerson = null;
  }
}
