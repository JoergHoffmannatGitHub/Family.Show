/*
 * A clone of the photo class which creates a serializable repository.
 * 
 * The fields contained in the repository are comparable to the GEDCOM 
 * format.
 * 
*/

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.FamilyShowLib
{
  /// <summary>
  /// Describes a repository
  /// </summary>
  [Serializable]
  public class Repository : INotifyPropertyChanged, IEquatable<Repository>
  {

    #region Fields and Constants

    private string id;
    private string repositoryName;
    private string repositoryAddress;

    #endregion

    #region Properties

    [XmlAttribute]
    public string Id
    {
      get { return id; }
      set
      {
        if (id != value)
        {
          id = value;
          OnPropertyChanged(nameof(Id));
        }
      }
    }

    public string RepositoryName
    {
      get { return repositoryName; }
      set
      {
        if (repositoryName != value)
        {
          repositoryName = value;
          OnPropertyChanged("RepositoryeName");
        }
      }
    }

    public string RepositoryAddress
    {
      get { return repositoryAddress; }
      set
      {
        if (repositoryAddress != value)
        {
          repositoryAddress = value;
          OnPropertyChanged(nameof(RepositoryAddress));
        }
      }
    }

    [XmlIgnore]
    public string RepositoryNameAndId
    {
      get { return id + " " + repositoryName; }
      set { }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new instance of a repository object.
    /// This parameterless constructor is also required for serialization.
    /// </summary>
    public Repository()
    {
      repositoryName = Properties.Resources.Unknown;
    }

    /// <summary>
    /// Creates a new instance of the repository class with the id, name and address of the repository.  
    /// The calling method must ensure that there are no duplicated ids.
    /// </summary>
    public Repository(string repositoryId, string repositoryName, string repositoryAddress) : this()
    {
      if (!string.IsNullOrEmpty(repositoryId))
        id = repositoryId;
      if (!string.IsNullOrEmpty(repositoryName))
        this.repositoryName = repositoryName;
      if (!string.IsNullOrEmpty(repositoryAddress))
        this.repositoryAddress = repositoryAddress;
    }

    #endregion

    #region IEquatable Members

    /// <summary>
    /// Determine equality between two repository classes
    /// </summary>
    public bool Equals(Repository other)
    {
      if (other is null) return false;
      // Optimization for a common success case.
      if (ReferenceEquals(this, other)) return true;
      // If run-time types are not exactly the same, return false.
      if (GetType() != other.GetType()) return false;
      // Return true if the Id match.
      return Id == other.Id;
    }

    #endregion

    #region Equals Methods

    /// <summary>
    /// Determines whether the specified object is equal to the current repository.
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as Repository);

    /// <summary>
    /// Returns the hash code of this repository.
    /// </summary>
    public override int GetHashCode()
    {
      return Id.GetHashCode();
    }

    public static bool operator ==(Repository lhs, Repository rhs)
    {
      return lhs is null ? rhs is null : lhs.Equals(rhs);
    }

    public static bool operator !=(Repository lhs, Repository rhs) => !(lhs == rhs);

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
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

  }

}
