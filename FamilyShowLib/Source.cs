/*
 * A clone of the photo class which creates a serializable source.
 * 
 * The fields contained in the source are comparable to the GEDCOM 
 * format. Not all fields are currently used.
 * 
*/

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FamilyShowLib;

/// <summary>
/// Describes a source
/// </summary>
[Serializable]
public class Source : INotifyPropertyChanged, IEquatable<Source>
{

  #region Fields and Constants

  private string _id;
  private string _sourceName;
  private string _sourceAuthor;
  private string _sourcePublisher;
  private string _sourceNote;
  private string _sourceRepository;

  #endregion

  #region Properties

  [XmlAttribute]
  public string Id
  {
    get { return _id; }
    set
    {
      if (_id != value)
      {
        _id = value;
        OnPropertyChanged(nameof(Id));
      }
    }
  }

  public string SourceName
  {
    get { return _sourceName; }
    set
    {
      if (_sourceName != value)
      {
        _sourceName = value;
        OnPropertyChanged(nameof(SourceName));
      }
    }
  }

  public string SourceAuthor
  {
    get { return _sourceAuthor; }
    set
    {
      if (_sourceAuthor != value)
      {
        _sourceAuthor = value;
        OnPropertyChanged(nameof(SourceAuthor));
      }
    }
  }

  public string SourcePublisher
  {
    get { return _sourcePublisher; }
    set
    {
      if (_sourcePublisher != value)
      {
        _sourcePublisher = value;
        OnPropertyChanged(nameof(SourcePublisher));
      }
    }
  }

  public string SourceNote
  {
    get { return _sourceNote; }
    set
    {
      if (_sourceNote != value)
      {
        _sourceNote = value;
        OnPropertyChanged(nameof(SourceNote));
      }
    }
  }

  public string SourceRepository
  {
    get { return _sourceRepository; }
    set
    {
      if (_sourceRepository != value)
      {
        _sourceRepository = value;
        OnPropertyChanged(nameof(SourceRepository));
      }
    }
  }

  [XmlIgnore]
  public string SourceNameAndId
  {
    get { return _id + " " + _sourceName; }
    set { }
  }

  #endregion

  #region Constructors

  /// <summary>
  /// Creates a new instance of a sourceobject.
  /// This parameterless constructor is also required for serialization.
  /// </summary>
  public Source()
  {
    _sourceName = Properties.Resources.Unknown;
  }

  /// <summary>
  /// Creates a new instance of the source class with the id, name, author, publisher, note and repository of the source.  
  /// The calling method must ensure that there are no duplicated ids.
  /// </summary>
  public Source(string sourceId, string sourceName, string sourceAuthor, string sourcePublisher, string sourceNote, string sourceRepository)
      : this()
  {
    if (!string.IsNullOrEmpty(sourceId))
    {
      _id = sourceId;
    }

    if (!string.IsNullOrEmpty(sourceName))
    {
      _sourceName = sourceName;
    }

    if (!string.IsNullOrEmpty(sourceAuthor))
    {
      _sourceAuthor = sourceAuthor;
    }

    if (!string.IsNullOrEmpty(sourcePublisher))
    {
      _sourcePublisher = sourcePublisher;
    }

    if (!string.IsNullOrEmpty(sourceNote))
    {
      _sourceNote = sourceNote;
    }

    if (!string.IsNullOrEmpty(sourceRepository))
    {
      _sourceRepository = sourceRepository;
    }
  }

  #endregion

  #region IEquatable Members

  /// <summary>
  /// Determine equality between two source classes
  /// </summary>
  public bool Equals(Source other)
  {
    if (other is null)
    {
      return false;
    }
    // Optimization for a common success case.
    if (ReferenceEquals(this, other))
    {
      return true;
    }
    // If run-time types are not exactly the same, return false.
    if (GetType() != other.GetType())
    {
      return false;
    }
    // Return true if the Id match.
    return (Id == other.Id);
  }

  #endregion

  #region Equals Methods

  /// <summary>
  /// Determines whether the specified object is equal to the current source.
  /// </summary>
  public override bool Equals(object obj) => Equals(obj as Source);

  /// <summary>
  /// Returns the hash code of this source.
  /// </summary>
  public override int GetHashCode()
  {
    return Id.GetHashCode();
  }

  public static bool operator ==(Source lhs, Source rhs)
  {
    return lhs is null ? rhs is null : lhs.Equals(rhs);
  }

  public static bool operator !=(Source lhs, Source rhs) => !(lhs == rhs);

  #endregion

  #region INotifyPropertyChanged Members

  /// <summary>
  /// INotifyPropertyChanged requires a property called PropertyChanged.
  /// </summary>
  public event PropertyChangedEventHandler PropertyChanged;

  /// <summary>
  /// Fires the event for the property when it changes.
  /// </summary>
  public virtual void OnPropertyChanged(string propertyName)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  #endregion

}
