/*
 * A connector consists of two nodes and a connection type. A connection has a
 * filtered state. The opacity is reduced when drawing a connection that is 
 * filtered. An animation is applied to the brush when the filtered state changes.
*/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using FamilyShowLib;

namespace FamilyShow;

/// <summary>
/// One of the nodes in a connection.
/// </summary>
public class DiagramConnectorNode(DiagramNode node, DiagramGroup group, DiagramRow row)
{
  #region fields

  // Node location in the diagram.
  private readonly DiagramRow _row = row;
  private readonly DiagramGroup _group = group;
  public bool displayDates = true;

  #endregion

  #region properties

  /// <summary>
  /// Node for this connection point.
  /// </summary>
  public DiagramNode Node { get; } = node;

  /// <summary>
  /// Center of the node relative to the diagram.
  /// </summary>
  public Point Center
  {
    get { return GetPoint(Node.Center); }
  }

  /// <summary>
  /// LeftCenter of the node relative to the diagram.
  /// </summary>
  public Point LeftCenter
  {
    get { return GetPoint(Node.LeftCenter); }
  }

  /// <summary>
  /// RightCenter of the node relative to the diagram.
  /// </summary>
  public Point RightCenter
  {
    get { return GetPoint(Node.RightCenter); }
  }

  /// <summary>
  /// TopCenter of the node relative to the diagram.
  /// </summary>
  public Point TopCenter
  {
    get { return GetPoint(Node.TopCenter); }
  }

  /// <summary>
  /// TopRight of the node relative to the diagram.
  /// </summary>
  public Point TopRight
  {
    get { return GetPoint(Node.TopRight); }
  }

  /// <summary>
  /// TopLeft of the node relative to the diagram.
  /// </summary>
  public Point TopLeft
  {
    get { return GetPoint(Node.TopLeft); }
  }

  #endregion
  /// <summary>
  /// Return the point shifted by the row and group location.
  /// </summary>
  private Point GetPoint(Point point)
  {
    point.Offset(
        _row.Location.X + _group.Location.X,
        _row.Location.Y + _group.Location.Y);

    return point;
  }
}

/// <summary>
/// Base class for child and married diagram connectors.
/// </summary>
public abstract class DiagramConnector
{
  private static class Const
  {
    // Filtered settings.
    public static double OpacityFiltered = 0.15;  //hide connectors according to time slider
    public static double OpacityNormal = 1.0;
    public static double AnimationDuration = 300;
  }

  #region fields

  // Animation if the filtered state has changed.
  private DoubleAnimation _animation;

  #endregion

  /// <summary>
  /// Return true if this is a child connector.
  /// </summary>
  public virtual bool IsChildConnector
  {
    get { return true; }
  }

  public bool ShowDate = true;

  /// <summary>
  /// Gets the married date for the connector. Can be null.
  /// </summary>
  public virtual DateTime? MarriedDate
  {
    get { return null; }
  }

  /// <summary>
  /// Get the previous married date for the connector. Can be null.
  /// </summary>
  public virtual DateTime? PreviousMarriedDate
  {
    get { return null; }
  }

  /// <summary>
  /// Get the starting node.
  /// </summary>
  protected DiagramConnectorNode StartNode { get; }

  /// <summary>
  /// Get the ending node.
  /// </summary>
  protected DiagramConnectorNode EndNode { get; }

  /// <summary>
  /// Get or set the pen that specifies the connector line.
  /// </summary>
  protected Pen ResourcePen { get; set; }

  /// <summary>
  /// Create the connector line pen. The opacity is set based on
  /// the current filtered state. The pen contains an animation
  /// if the filtered state has changed.
  /// </summary>
  protected Pen Pen
  {
    get
    {
      // Make a copy of the resource pen so it can 
      // be modified, the resource pen is frozen.
      Pen connectorPen = ResourcePen.Clone();

      // Set opacity based on the filtered state.
      connectorPen.Brush.Opacity = (IsFiltered) ? Const.OpacityFiltered : Const.OpacityNormal;

      // Create animation if the filtered state has changed.
      if (_animation != null)
      {
        connectorPen.Brush.BeginAnimation(Brush.OpacityProperty, _animation);
      }

      return connectorPen;
    }
  }

  /// <summary>
  /// Return true if the connection is currently filtered.
  /// </summary>
  private bool IsFiltered { set; get; }

  /// <summary>
  /// Get the new filtered state of the connection. This depends
  /// on the connection nodes, marriage date and previous marriage date.
  /// </summary>
  protected virtual bool NewFilteredState
  {
    get
    {
      // Connection is filtered if any of the nodes are filtered.
      if (StartNode.Node.IsFiltered || EndNode.Node.IsFiltered)
      {
        return true;
      }

      // Connection is not filtered.
      return false;
    }
  }

  /// <summary>
  /// Consturctor that specifies the two nodes that are connected.
  /// </summary>
  protected DiagramConnector(DiagramConnectorNode startConnector,
      DiagramConnectorNode endConnector)
  {
    StartNode = startConnector;
    EndNode = endConnector;
  }

  /// <summary>
  /// Return true if should continue drawing, otherwise false.
  /// </summary>
  public virtual bool Draw(DrawingContext drawingContext)
  {
    // Don't draw if either of the nodes are filtered.
    if (StartNode.Node.Visibility != Visibility.Visible ||
        EndNode.Node.Visibility != Visibility.Visible)
    {
      return false;
    }

    // First check if the filtered state has changed, an animation
    // if created if the state has changed which is used for all 
    // connection drawing.
    CheckIfFilteredChanged();

    return true;
  }

  /// <summary>
  /// Create the specified brush. The opacity is set based on the 
  /// current filtered state. The brush contains an animation if 
  /// the filtered state has changed.
  /// </summary>
  protected SolidColorBrush GetBrush(Color color)
  {
    // Create the brush.
    SolidColorBrush brush = new(color)
    {
      // Set the opacity based on the filtered state.
      Opacity = (IsFiltered) ? Const.OpacityFiltered : Const.OpacityNormal
    };

    // Create animation if the filtered state has changed.
    if (_animation != null)
    {
      brush.BeginAnimation(Brush.OpacityProperty, _animation);
    }

    return brush;
  }

  /// <summary>
  /// Determine if the filtered state has changed, and create
  /// the animation that is used to draw the connection.
  /// </summary>
  protected void CheckIfFilteredChanged()
  {
    // See if the filtered state has changed.
    bool newFiltered = NewFilteredState;
    if (newFiltered != IsFiltered)
    {
      // Filtered state did change, create the animation.
      IsFiltered = newFiltered;
      _animation = new DoubleAnimation
      {
        From = IsFiltered ? Const.OpacityNormal : Const.OpacityFiltered,
        To = IsFiltered ? Const.OpacityFiltered : Const.OpacityNormal,
        Duration = App.GetAnimationDuration(Const.AnimationDuration)
      };
    }
    else
    {
      // Filtered state did not change, clear the animation.
      _animation = null;
    }
  }
}

/// <summary>
/// Connector between a parent and child.
/// </summary>
public class ChildDiagramConnector : DiagramConnector
{
  public ChildDiagramConnector(DiagramConnectorNode startConnector,
      DiagramConnectorNode endConnector) : base(startConnector, endConnector)
  {
    // Get the pen that is used to draw the connection line.

    ChildRelationship rel = StartNode.Node.Person.GetParentChildRelationship(EndNode.Node.Person);
    if (rel != null)
    {
      if (rel.ParentChildModifier == ParentChildModifier.Adopted || rel.ParentChildModifier == ParentChildModifier.Foster)
      {
        if (rel.ParentChildModifier == ParentChildModifier.Adopted)
        {
          ResourcePen = (Pen)Application.Current.TryFindResource("AdoptedChildConnectionPen");
        }

        if (rel.ParentChildModifier == ParentChildModifier.Foster)
        {
          ResourcePen = (Pen)Application.Current.TryFindResource("FosteredChildConnectionPen");
        }
      }
      else
      {
        if ((StartNode.Node.Type == NodeType.Related && EndNode.Node.Type == NodeType.Related ||
            StartNode.Node.Type == NodeType.Related && EndNode.Node.Type == NodeType.Primary ||
            StartNode.Node.Type == NodeType.Primary && EndNode.Node.Type == NodeType.Related) && Diagram.showBloodlines)
        {
          ResourcePen = (Pen)Application.Current.TryFindResource("ChildPrimaryConnectionPen");
        }
        else
        {
          ResourcePen = (Pen)Application.Current.TryFindResource("ChildConnectionPen");
        }
      }
    }

  }

  /// <summary>
  /// Draw the connection between the two nodes.
  /// </summary>
  public override bool Draw(DrawingContext drawingContext)
  {
    if (!base.Draw(drawingContext))
    {
      return false;
    }

    drawingContext.DrawLine(Pen, StartNode.Center, EndNode.Center);
    return true;
  }
}

/// <summary>
/// Connector between spouses. Handles current and former spouses.
/// </summary>
public class MarriedDiagramConnector : DiagramConnector
{
  #region fields

  // Connector line text.
  private readonly double _connectionTextSize;
  private Color _connectionTextColor;
  private readonly FontFamily _connectionTextFont;

  // Flag if currently married or former.
  private readonly bool _married;

  // The Pixels Per Density Independent Pixel value.
  private readonly double _pixelsPerDip;

  #endregion

  #region properties

  /// <summary>
  /// Return true if this is a child connector.
  /// </summary>
  public override bool IsChildConnector
  {
    get { return false; }
  }

  /// <summary>
  /// Gets the married date for the connector. Can be null.
  /// </summary>
  public override DateTime? MarriedDate
  {
    get
    {
      if (_married)
      {
        SpouseRelationship rel = StartNode.Node.Person.GetSpouseRelationship(EndNode.Node.Person);
        if (rel != null)
        {
          return rel.MarriageDate;
        }
      }
      return null;
    }
  }

  /// <summary>
  /// Get the previous married date for the connector. Can be null.
  /// </summary>
  public override DateTime? PreviousMarriedDate
  {
    get
    {
      if (!_married)
      {
        SpouseRelationship rel = StartNode.Node.Person.GetSpouseRelationship(EndNode.Node.Person);
        if (rel != null)
        {
          return rel.DivorceDate;
        }
      }
      return null;
    }
  }

  /// <summary>
  /// Get the new filtered state of the connection. This depends
  /// on the connection nodes, marriage date and previous marriage date.
  /// Return true if the connection should be filtered.
  /// </summary>
  protected override bool NewFilteredState
  {
    get
    {
      // Check the two connected nodes.
      if (base.NewFilteredState)
      {
        return true;
      }

      // Check the married date for current and former spouses.
      SpouseRelationship rel = StartNode.Node.Person.GetSpouseRelationship(EndNode.Node.Person);
      if (rel != null && rel.MarriageDate != null &&
          (StartNode.Node.DisplayYear < rel.MarriageDate.Value.Year))
      {
        return true;
      }

      // Check the divorce date for former spouses.
      if (!_married && rel != null && rel.DivorceDate != null &&
          (StartNode.Node.DisplayYear < rel.DivorceDate.Value.Year))
      {
        return true;
      }

      // Connection is not filtered.
      return false;
    }
  }

  #endregion

  public MarriedDiagramConnector(bool isMarried,
      DiagramConnectorNode startConnector, DiagramConnectorNode endConnector,
      DpiScale dpiScale) :
      base(startConnector, endConnector)
  {
    // Store if currently married or former.
    _married = isMarried;

    // Get resources used to draw text.
    _connectionTextSize = (double)Application.Current.TryFindResource("ConnectionTextSize");
    _connectionTextColor = (Color)Application.Current.TryFindResource("ConnectionTextColor");
    _connectionTextFont = (FontFamily)Application.Current.TryFindResource("ConnectionTextFont");

    // Gets the DPI information at which this Visual is measured and rendered.
    _pixelsPerDip = dpiScale.PixelsPerDip;

    // Get resourced used to draw the connection line.
    ResourcePen = (Pen)Application.Current.TryFindResource(
        _married ? "MarriedConnectionPen" : "FormerConnectionPen");
  }

  /// <summary>
  /// Draw the connection between the two nodes.
  /// </summary>
  public override bool Draw(DrawingContext drawingContext)
  {
    if (!base.Draw(drawingContext))
    {
      return false;
    }

    DrawMarried(drawingContext);
    return true;
  }

  /// <summary>
  /// Draw married or previous married connector between nodes.
  /// </summary>
  private void DrawMarried(DrawingContext drawingContext)
  {
    const double TextSpace = 3;

    // Determine the start and ending points based on what node is on the left / right.
    Point startPoint = (StartNode.TopCenter.X < EndNode.TopCenter.X) ? StartNode.TopCenter : EndNode.TopCenter;
    Point endPoint = (StartNode.TopCenter.X < EndNode.TopCenter.X) ? EndNode.TopCenter : StartNode.TopCenter;

    // Use a higher arc when the nodes are further apart.
    double arcHeight = (endPoint.X - startPoint.X) / 4;

    // Use a maximum arc height to prevent arcs from obscuring rows above
    if (arcHeight > 35)
    {
      arcHeight = 35;
    }

    Point middlePoint = new(startPoint.X + ((endPoint.X - startPoint.X) / 2), startPoint.Y - arcHeight);

    // Draw the arc, get the bounds so can draw connection text.
    Rect bounds = DrawArc(drawingContext, Pen, startPoint, middlePoint, endPoint);

    // Get the relationship info so the dates can be displayed.
    SpouseRelationship rel = StartNode.Node.Person.GetSpouseRelationship(EndNode.Node.Person);
    if (rel != null)
    {
      // Marriage date.
      if (rel.MarriageDate != null && ShowDate == true)
      {
        string text = rel.MarriageDateDescriptor + rel.MarriageDate.Value.Year.ToString(CultureInfo.CurrentCulture);

        FormattedText format = new(text,
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight, new Typeface(_connectionTextFont,
            FontStyles.Normal, FontWeights.Normal, FontStretches.Normal,
            _connectionTextFont), _connectionTextSize, GetBrush(_connectionTextColor),
            _pixelsPerDip);

        drawingContext.DrawText(format, new Point(
            bounds.Left + ((bounds.Width / 2) - (format.Width / 2)),
            bounds.Top - format.Height - TextSpace));
      }

      // Previous marriage date.
      if (!_married && rel.DivorceDate != null && ShowDate == true)
      {
        string text = rel.DivorceDateDescriptor + rel.DivorceDate.Value.Year.ToString(CultureInfo.CurrentCulture);

        FormattedText format = new(text,
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight, new Typeface(_connectionTextFont,
            FontStyles.Normal, FontWeights.Normal, FontStretches.Normal,
            _connectionTextFont), _connectionTextSize, GetBrush(_connectionTextColor),
            _pixelsPerDip);

        drawingContext.DrawText(format, new Point(
            bounds.Left + ((bounds.Width / 2) - (format.Width / 2)),
            bounds.Top + TextSpace));
      }
    }
  }

  /// <summary>
  /// Draw an arc connecting the two nodes.
  /// </summary>
  private static Rect DrawArc(DrawingContext drawingContext, Pen pen,
      Point startPoint, Point middlePoint, Point endPoint)
  {
    PathGeometry geometry = new();
    PathFigure figure = new()
    {
      StartPoint = startPoint
    };
    figure.Segments.Add(new QuadraticBezierSegment(middlePoint, endPoint, true));
    geometry.Figures.Add(figure);
    drawingContext.DrawGeometry(null, pen, geometry);
    return geometry.Bounds;
  }
}
