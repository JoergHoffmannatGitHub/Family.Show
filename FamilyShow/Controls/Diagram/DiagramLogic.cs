/*
 * Contains the logic to populate the diagram. Populates rows with 
 * groups and nodes based on the node relationships. 
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using FamilyShowLib;

using Genealogy.Domain.Interfaces;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FamilyShow;
#pragma warning restore IDE0130 // Namespace does not match folder structure

internal class DiagramLogic
{
    #region fields

    // DPI information at which this Visual is measured and rendered.
    private DpiScale _dpiScale;

    // Callback when a node is clicked.
    private RoutedEventHandler _nodeClickHandler;

    // Filter year for nodes and connectors.
    private double _displayYear;

    #endregion

    #region properties

    /// <summary>
    /// Sets the callback that is called when a node is clicked.
    /// </summary>
    public RoutedEventHandler NodeClickHandler
    {
        set { _nodeClickHandler = value; }
    }

    /// <summary>
    /// Gets the list of people in the family.
    /// </summary>
    public PeopleCollection Family { get; }

    /// <summary>
    /// Gets the list of connections between nodes.
    /// </summary>
    public List<DiagramConnector> Connections { get; } = [];

    /// <summary>
    /// Gets the person lookup list. This includes all of the 
    /// people and nodes that are displayed in the diagram.
    /// </summary>
    public Dictionary<Person, DiagramConnectorNode> PersonLookup { get; } = [];

    /// <summary>
    /// Sets the year filter that filters nodes and connectors.
    /// </summary>
    public double DisplayYear
    {
        set
        {
            if (_displayYear != value)
            {
                _displayYear = value;
                foreach (DiagramConnectorNode connectorNode in PersonLookup.Values)
                {
                    connectorNode.Node.DisplayYear = _displayYear;
                }
            }
        }
    }

    /// <summary>
    /// Gets the minimum year in all nodes and connectors.
    /// </summary>
    public double MinimumYear
    {
        get
        {
            // Init to current year.
            double minimumYear = DateTime.Now.Year;

            foreach (DiagramConnectorNode connectorNode in PersonLookup.Values)
            {
                // Check birth years.
                minimumYear = DiagramLogic.GetMinimumYearFromDate(minimumYear, connectorNode.Node.Person.BirthDate);

                // Check death years.
                minimumYear = DiagramLogic.GetMinimumYearFromDate(minimumYear, connectorNode.Node.Person.DeathDate);
            }

            foreach (DiagramConnector connector in Connections)
            {
                // Marriage date.
                minimumYear = DiagramLogic.GetMinimumYearFromDate(minimumYear, connector.MarriedDate);

                // Previous marriage (divorve) date.
                minimumYear = DiagramLogic.GetMinimumYearFromDate(minimumYear, connector.PreviousMarriedDate);
            }

            return minimumYear;
        }
    }

    #endregion

    internal static double GetMinimumYearFromDate(double minimumYear, DateWrapper date)
    {
        if (DateWrapper.IsDateExact(date, out IDateExact exact))
        {
            minimumYear = Math.Min(minimumYear, exact.Year);
        }

        return minimumYear;
    }

    public DiagramLogic(DpiScale dpiScale)
    {
        // The list of people, this is a global list shared by the application.
        Family = App.Family;

        _dpiScale = dpiScale;

        Clear();
    }

    #region get people

    /// <summary>
    /// Return a list of parents for the people in the specified row.
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public static Collection<Person> GetParents(DiagramRow row)
    {
        // List that is returned.
        Collection<Person> list = [];

        // Get possible children in the row.
        List<Person> rowList = GetPrimaryAndRelatedPeople(row);

        // Add each parent to the list, make sure the parent is only added once.
        foreach (Person person in rowList)
        {
            foreach (Person parent in person.Parents)
            {
                if (!list.Contains(parent))
                {
                    list.Add(parent);
                }
            }
        }

        return list;
    }

    /// <summary>
    /// Return a list of children for the people in the specified row.
    /// </summary>
    public static List<Person> GetChildren(DiagramRow row)
    {
        // List that is returned.
        List<Person> list = [];

        // Get possible parents in the row.
        List<Person> rowList = GetPrimaryAndRelatedPeople(row);

        // Add each child to the list, make sure the child is only added once.
        foreach (Person person in rowList)
        {
            foreach (Person child in person.Children)
            {
                if (!list.Contains(child))
                {
                    list.Add(child);
                }
            }
        }

        return list;
    }

    /// <summary>
    /// Return list of people in the row that are primary or related node types.
    /// </summary>
    private static List<Person> GetPrimaryAndRelatedPeople(DiagramRow row)
    {
        List<Person> list = [];
        foreach (DiagramGroup group in row.Groups)
        {
            foreach (DiagramNode node in group.Nodes)
            {
                if (node.Type == NodeType.Related || node.Type == NodeType.Primary)
                {
                    list.Add(node.Person);
                }
            }
        }

        return list;
    }

    /// <summary>
    /// Remove any people from the 'other' list from the 'people' list.
    /// </summary>
    private static void RemoveDuplicates(Collection<Person> people, Collection<Person> other)
    {
        foreach (Person person in other)
        {
            people.Remove(person);
        }
    }

    #endregion

    #region create nodes

    /// <summary>
    /// Create a DiagramNode.
    /// </summary>
    private DiagramNode CreateNode(Person person, NodeType type, bool clickEvent, double scale)
    {
        DiagramNode node = CreateNode(person, type, clickEvent);
        node.Scale = scale;
        return node;
    }

    /// <summary>
    /// Create a DiagramNode.
    /// </summary>
    private DiagramNode CreateNode(Person person, NodeType type, bool clickEvent)
    {
        DiagramNode node = new()
        {
            Person = person,
            Type = type
        };
        if (clickEvent)
        {
            node.Click += _nodeClickHandler;
        }

        return node;
    }

    /// <summary>
    /// Add the siblings to the specified row and group.
    /// </summary>
    private void AddSiblingNodes(DiagramRow row, DiagramGroup group,
        Collection<Person> siblings, NodeType nodeType, double scale)
    {
        foreach (Person sibling in siblings)
        {
            if (!PersonLookup.ContainsKey(sibling))
            {
                // Siblings node.
                DiagramNode node = CreateNode(sibling, nodeType, true, scale);
                group.Add(node);
                PersonLookup.Add(node.Person, new DiagramConnectorNode(node, group, row));
            }
        }
    }

    /// <summary>
    /// Add the spouses to the specified row and group.
    /// </summary>
    private void AddSpouseNodes(Person person, DiagramRow row,
        DiagramGroup group, Collection<Person> spouses,
        NodeType nodeType, double scale, bool married)
    {
        foreach (Person spouse in spouses)
        {
            if (!PersonLookup.ContainsKey(spouse))
            {
                // Spouse node.
                DiagramNode node = CreateNode(spouse, nodeType, true, scale);
                group.Add(node);

                // Add connection.
                DiagramConnectorNode connectorNode = new(node, group, row);
                PersonLookup.Add(node.Person, connectorNode);
                Connections.Add(new MarriedDiagramConnector(married,
                  PersonLookup[person], connectorNode, _dpiScale));
            }
        }
    }

    #endregion

    #region create rows

    /// <summary>
    /// Creates the primary row. The row contains groups: 1) The primary-group 
    /// that only contains the primary node, and 2) The optional left-group 
    /// that contains spouses and siblings.
    /// </summary>
    public DiagramRow CreatePrimaryRow(Person person, double scale, double scaleRelated, bool hideSiblings, bool hideSpouses, bool hidePreviousSpouses)
    {
        // The primary node contains two groups, 
        DiagramGroup primaryGroup = new();
        DiagramGroup leftGroup = new();

        // Set up the row.
        DiagramRow row = new();

        // Add primary node.
        DiagramNode node = CreateNode(person, NodeType.Primary, false, scale);
        primaryGroup.Add(node);
        PersonLookup.Add(node.Person, new DiagramConnectorNode(node, primaryGroup, row));

        if (hideSpouses == false)
        {

            // Current spouses.
            Collection<Person> currentSpouses = person.CurrentSpouses;
            AddSpouseNodes(person, row, leftGroup, currentSpouses,
                NodeType.Spouse, scaleRelated, true);
        }


        if (hidePreviousSpouses == false)
        {

            // Previous spouses.
            Collection<Person> previousSpouses = person.PreviousSpouses;
            AddSpouseNodes(person, row, leftGroup, previousSpouses,
                NodeType.Spouse, scaleRelated, false);
        }

        if (hideSiblings == false)
        {

            // Siblings.
            Collection<Person> siblings = person.Siblings;
            AddSiblingNodes(row, leftGroup, siblings, NodeType.Sibling, scaleRelated);

            // Half siblings.
            Collection<Person> halfSiblings = person.HalfSiblings;
            AddSiblingNodes(row, leftGroup, halfSiblings, NodeType.SiblingLeft, scaleRelated);

        }

        if (leftGroup.Nodes.Count > 0)
        {
            leftGroup.Reverse();
            row.Add(leftGroup);
        }

        row.Add(primaryGroup);

        return row;
    }

    /// <summary>
    /// Create the child row. The row contains a group for each child. 
    /// Each group contains the child and spouses.
    /// </summary>
    public DiagramRow CreateChildrenRow(List<Person> children, double scale, double scaleRelated, bool hideInLaws)
    {
        // Setup the row.
        DiagramRow row = new();

        foreach (Person child in children)
        {
            // Each child is in their group, the group contains the child 
            // and any spouses. The groups does not contain siblings.
            DiagramGroup group = new();
            row.Add(group);

            // Child.
            if (!PersonLookup.ContainsKey(child))
            {
                DiagramNode node = CreateNode(child, NodeType.Related, true, scale);
                group.Add(node);
                PersonLookup.Add(node.Person, new DiagramConnectorNode(node, group, row));
            }


            if (hideInLaws == false)
            {
                // Current spouses.
                Collection<Person> currentSpouses = child.CurrentSpouses;
                AddSpouseNodes(child, row, group, currentSpouses,
                    NodeType.Spouse, scaleRelated, true);

                // Previous spouses.
                Collection<Person> previousSpouses = child.PreviousSpouses;
                AddSpouseNodes(child, row, group, previousSpouses,
                    NodeType.Spouse, scaleRelated, false);
            }

            // Connections.
            AddParentConnections(child);

            group.Reverse();
        }

        return row;
    }

    /// <summary>
    /// Create the parent row. The row contains a group for each parent. 
    /// Each groups contains the parent, spouses and siblings.
    /// </summary>
    public DiagramRow CreateParentRow(Collection<Person> parents, double scale, double scaleRelated, bool hideInLaws)
    {
        // Set up the row.
        DiagramRow row = new();

        int groupCount = 0;

        foreach (Person person in parents)
        {
            // Each parent is in their group, the group contains the parent,
            // spouses and siblings.
            DiagramGroup group = new();
            row.Add(group);

            // Determine if this is a left or right oriented group.
            bool left = (groupCount++ % 2 == 0);

            // Parent.
            if (!PersonLookup.ContainsKey(person))
            {
                DiagramNode node = CreateNode(person, NodeType.Related, true, scale);
                group.Add(node);
                PersonLookup.Add(node.Person, new DiagramConnectorNode(node, group, row));
            }

            // Current spouses.
            Collection<Person> currentSpouses = person.CurrentSpouses;
            RemoveDuplicates(currentSpouses, parents);
            AddSpouseNodes(person, row, group, currentSpouses,
                NodeType.Spouse, scaleRelated, true);

            // Previous spouses.
            Collection<Person> previousSpouses = person.PreviousSpouses;
            RemoveDuplicates(previousSpouses, parents);
            AddSpouseNodes(person, row, group, previousSpouses,
                NodeType.Spouse, scaleRelated, false);

            if (hideInLaws == false)
            {
                // Siblings.
                Collection<Person> siblings = person.Siblings;
                AddSiblingNodes(row, group, siblings, NodeType.Sibling, scaleRelated);

                // Half siblings.
                Collection<Person> halfSiblings = person.HalfSiblings;
                AddSiblingNodes(row, group, halfSiblings, left ?
                    NodeType.SiblingLeft : NodeType.SiblingRight, scaleRelated);
            }
            // Connections.
            AddChildConnections(person);
            AddChildConnections(currentSpouses);
            AddChildConnections(previousSpouses);

            if (left)
            {
                group.Reverse();
            }
        }

        // Add connections that span across groups.
        AddSpouseConnections(parents);

        return row;
    }

    #endregion

    #region connections

    /// <summary>
    /// Add connections for each person and the person's children in the list.
    /// </summary>
    private void AddChildConnections(Collection<Person> parents)
    {
        foreach (Person person in parents)
        {
            AddChildConnections(person);
        }
    }

    /// <summary>
    /// Add connections between the child and child's parents.
    /// </summary>
    private void AddParentConnections(Person child)
    {
        foreach (Person parent in child.Parents)
        {
            AddChildConnection(parent, child);
        }
    }

    /// <summary>
    /// Add connections between the parent and parent’s children.
    /// </summary>
    private void AddChildConnections(Person parent)
    {
        foreach (Person child in parent.Children)
        {
            AddChildConnection(parent, child);
        }
    }

    /// <summary>
    /// Add connections between parent and child.
    /// </summary>
    private void AddChildConnection(Person parent, Person child)
    {
        if (PersonLookup.TryGetValue(parent, out DiagramConnectorNode parentConnector) &&
            PersonLookup.TryGetValue(child, out DiagramConnectorNode childConnector))
        {
            Connections.Add(new ChildDiagramConnector(parentConnector, childConnector));
        }
    }

    /// <summary>
    /// Add marriage connections for the people specified in the 
    /// list. Each marriage connection is only specified once.
    /// </summary>
    private void AddSpouseConnections(Collection<Person> list)
    {
        // Iterate through the list. 
        for (int current = 0; current < list.Count; current++)
        {
            // The person to check for marriages.
            Person person = list[current];

            // Check for current / former marriages in the rest of the list.                    
            for (int i = current + 1; i < list.Count; i++)
            {
                Person spouse = list[i];
                SpouseRelationship rel = person.GetSpouseRelationship(spouse);

                // Current marriage.
                if (rel != null && rel.SpouseModifier == SpouseModifier.Current)
                {
                    AddMarriedConnection(true, person, spouse);
                }

                // Former marriage
                if (rel != null && rel.SpouseModifier == SpouseModifier.Former)
                {
                    AddMarriedConnection(false, person, spouse);
                }
            }
        }
    }

    /// <summary>
    /// Add connections between parent and child.
    /// </summary>
    private void AddMarriedConnection(bool isMarried, Person person, Person spouse)
    {
        if (PersonLookup.TryGetValue(person, out DiagramConnectorNode personConnector) &&
            PersonLookup.TryGetValue(spouse, out DiagramConnectorNode spouseConnector))
        {
            Connections.Add(new MarriedDiagramConnector(isMarried, personConnector, spouseConnector, _dpiScale));
        }
    }

    #endregion

    /// <summary>
    /// Clear 
    /// </summary>
    public void Clear()
    {
        // Remove any event handlers from the nodes. Otherwise 
        // the delegate maintains a reference to the object 
        // which can hinder garbage collection. 
        foreach (DiagramConnectorNode node in PersonLookup.Values)
        {
            node.Node.Click -= _nodeClickHandler;
        }

        // Clear the connection info.
        Connections.Clear();
        PersonLookup.Clear();

        // Time filter.
        _displayYear = DateTime.Now.Year;
    }

    /// <summary>
    /// Return the DiagramNode for the specified Person.
    /// </summary>
    public DiagramNode GetDiagramNode(Person person)
    {
        return person != null && PersonLookup.TryGetValue(person, out DiagramConnectorNode personConnector) ? personConnector.Node : null;
    }

    /// <summary>
    /// Return the bounds (relative to the diagram) for the specified person.
    /// </summary>
    public Rect GetNodeBounds(Person person)
    {
        Rect bounds = Rect.Empty;
        if (person != null && PersonLookup.TryGetValue(person, out DiagramConnectorNode personConnector))
        {
            bounds = new Rect(personConnector.TopLeft.X, personConnector.TopLeft.Y,
                personConnector.Node.ActualWidth, personConnector.Node.ActualHeight);
        }

        return bounds;
    }
}
