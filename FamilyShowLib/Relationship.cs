using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

#pragma warning disable IDE0161 // Convert to file-scoped namespace - class diagram does not work without
namespace FamilyShowLib
#pragma warning restore IDE0161 // Convert to file-scoped namespace
{

  #region Relationship classes

  /// <summary>
  /// Describes the kinship between person objects
  /// </summary>
  [Serializable]
  public abstract class Relationship
  {
    private RelationshipType _relationshipType;

    private Person _relationTo;

    // The person's Id will be serialized instead of the relationTo person object to avoid
    // circular references during Xml Serialization. When family data is loaded, the corresponding
    // person object will be assigned to the relationTo property (please see app.xaml.cs).
    private string _personId;

    // Store the person's name with the Id to make the xml file more readable
    private string _personFullName;

    /// <summary>
    /// The Type of relationship.  Parent, child, sibling, or spouse
    /// </summary>
    public RelationshipType RelationshipType
    {
      get { return _relationshipType; }
      set { _relationshipType = value; }
    }

    /// <summary>
    /// The person id the relationship is to. See comment on personId above.
    /// </summary>
    [XmlIgnore]
    public Person RelationTo
    {
      get { return _relationTo; }
      set
      {
        _relationTo = value;
        _personId = value.Id;
        _personFullName = value.Name;
      }
    }

    public string PersonId
    {
      get { return _personId; }
      set { _personId = value; }
    }

    public string PersonFullName
    {
      get { return _personFullName; }
      set { _personFullName = value; }
    }

  }

  /// <summary>
  /// Describes the kinship between a child and parent.
  /// </summary>
  [Serializable]
  public class ParentRelationship : Relationship
  {
    private ParentChildModifier _parentChildModifier;

    public ParentChildModifier ParentChildModifier
    {
      get { return _parentChildModifier; }
      set { _parentChildModifier = value; }
    }

    // Paramaterless constructor required for XML serialization
    public ParentRelationship() { }

    public ParentRelationship(Person personId, ParentChildModifier parentChildType)
    {
      RelationshipType = RelationshipType.Parent;
      RelationTo = personId;
      _parentChildModifier = parentChildType;
    }
  }

  /// <summary>
  /// Describes the kinship between a parent and child.
  /// </summary>
  [Serializable]
  public class ChildRelationship : Relationship
  {
    private ParentChildModifier _parentChildModifier;

    public ParentChildModifier ParentChildModifier
    {
      get { return _parentChildModifier; }
      set { _parentChildModifier = value; }
    }

    // Paramaterless constructor required for XML serialization
    public ChildRelationship() { }

    public ChildRelationship(Person person, ParentChildModifier parentChildType)
    {
      RelationshipType = RelationshipType.Child;
      RelationTo = person;
      _parentChildModifier = parentChildType;
    }
  }

  /// <summary>
  /// Describes the kindship between a couple
  /// </summary>
  [Serializable]
  public class SpouseRelationship : Relationship
  {
    private SpouseModifier _spouseModifier;

    private DateWrapper _marriageDate;
    private string _marriageDateDescriptor;
    private string _marriagePlace;

    private string _marriageCitation;
    private string _marriageSource;
    private string _marriageLink;
    private string _marriageCitationActualText;
    private string _marriageCitationNote;

    private DateWrapper _divorceDate;
    private string _divorceDateDescriptor;

    private string _divorceCitation;
    private string _divorceSource;
    private string _divorceLink;
    private string _divorceCitationActualText;
    private string _divorceCitationNote;

    public SpouseModifier SpouseModifier
    {
      get { return _spouseModifier; }
      set { _spouseModifier = value; }
    }

    #region marriage get set methods

    public DateWrapper MarriageDate
    {
      get { return _marriageDate; }
      set { _marriageDate = value; }
    }

    public string MarriageDateDescriptor
    {
      get { return _marriageDateDescriptor; }
      set { _marriageDateDescriptor = value; }
    }

    public string MarriagePlace
    {
      get { return _marriagePlace; }
      set { _marriagePlace = value; }
    }

    public string MarriageCitation
    {
      get { return _marriageCitation; }
      set { _marriageCitation = value; }
    }

    public string MarriageSource
    {
      get { return _marriageSource; }
      set { _marriageSource = value; }
    }

    public string MarriageLink
    {
      get { return _marriageLink; }
      set { _marriageLink = value; }
    }

    public string MarriageCitationNote
    {
      get { return _marriageCitationNote; }
      set { _marriageCitationNote = value; }
    }

    public string MarriageCitationActualText
    {
      get { return _marriageCitationActualText; }
      set { _marriageCitationActualText = value; }
    }

    #endregion

    #region divorce get set methods

    public DateWrapper DivorceDate
    {
      get { return _divorceDate; }
      set { _divorceDate = value; }
    }

    public string DivorceDateDescriptor
    {
      get { return _divorceDateDescriptor; }
      set { _divorceDateDescriptor = value; }
    }

    public string DivorceCitation
    {
      get { return _divorceCitation; }
      set { _divorceCitation = value; }
    }

    public string DivorceSource
    {
      get { return _divorceSource; }
      set { _divorceSource = value; }
    }

    public string DivorceLink
    {
      get { return _divorceLink; }
      set { _divorceLink = value; }
    }

    public string DivorceCitationNote
    {
      get { return _divorceCitationNote; }
      set { _divorceCitationNote = value; }
    }

    public string DivorceCitationActualText
    {
      get { return _divorceCitationActualText; }
      set { _divorceCitationActualText = value; }
    }

    #endregion

    // Paramaterless constructor required for XML serialization
    public SpouseRelationship() { }

    public SpouseRelationship(Person person, SpouseModifier spouseType)
    {
      RelationshipType = RelationshipType.Spouse;
      _spouseModifier = spouseType;
      RelationTo = person;
    }
  }

  /// <summary>
  /// Describes the kindship between a siblings
  /// </summary>
  [Serializable]
  public class SiblingRelationship : Relationship
  {
    // Paramaterless constructor required for XML serialization
    public SiblingRelationship() { }

    public SiblingRelationship(Person person)
    {
      RelationshipType = RelationshipType.Sibling;
      RelationTo = person;
    }
  }

  #endregion

  #region Relationships collection

  /// <summary>
  /// Collection of relationship for a person object
  /// </summary>
  [Serializable]
  public class RelationshipCollection : ObservableCollection<Relationship> { }

  #endregion

  #region Relationship Type/Modifier Enums

  /// <summary>
  /// Enumeration of connection types between person objects
  /// </summary>
  public enum RelationshipType
  {
    Child,
    Parent,
    Sibling,
    Spouse
  }

  public enum SpouseModifier
  {
    Current,
    Former
  }

  public enum ParentChildModifier
  {
    Natural,
    Adopted,
    Foster
  }

  #endregion

}
