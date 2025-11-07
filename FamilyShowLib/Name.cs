using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FamilyShowLib;

/// <summary>
/// Represents a name for a person. A person can have multiple names (birth name, married name, aka, etc.)
/// </summary>
[Serializable]
public class Name : INotifyPropertyChanged, IEquatable<Name>
{
  #region Fields

  private string _firstName;
  private string _lastName;
  private string _suffix;
  private string _prefix;
  private NameType _nameType;
  private bool _isPrimary;

  #endregion

  #region Properties

  /// <summary>
  /// Gets or sets the first name (given names)
  /// </summary>
  public string FirstName
  {
    get { return _firstName; }
    set
    {
      if (_firstName != value)
      {
        _firstName = value;
        OnPropertyChanged(nameof(FirstName));
        OnPropertyChanged(nameof(FullName));
      }
    }
  }

  /// <summary>
  /// Gets or sets the last name (surname)
  /// </summary>
  public string LastName
  {
    get { return _lastName; }
    set
    {
      if (_lastName != value)
      {
        _lastName = value;
        OnPropertyChanged(nameof(LastName));
        OnPropertyChanged(nameof(FullName));
      }
    }
  }

  /// <summary>
  /// Gets or sets the suffix (Jr., Sr., III, etc.)
  /// </summary>
  public string Suffix
  {
    get { return _suffix; }
    set
    {
      if (_suffix != value)
      {
        _suffix = value;
        OnPropertyChanged(nameof(Suffix));
        OnPropertyChanged(nameof(FullName));
      }
    }
  }

  /// <summary>
  /// Gets or sets the prefix/title (Dr., Prof., etc.)
  /// </summary>
  public string Prefix
  {
    get { return _prefix; }
    set
    {
      if (_prefix != value)
      {
        _prefix = value;
        OnPropertyChanged(nameof(Prefix));
        OnPropertyChanged(nameof(FullName));
      }
    }
  }

  /// <summary>
  /// Gets or sets the type of name (birth, married, aka, etc.)
  /// </summary>
  public NameType NameType
  {
    get { return _nameType; }
    set
    {
      if (_nameType != value)
      {
        _nameType = value;
        OnPropertyChanged(nameof(NameType));
      }
    }
  }

  /// <summary>
  /// Gets or sets whether this is the primary/preferred name for the person
  /// </summary>
  public bool IsPrimary
  {
    get { return _isPrimary; }
    set
    {
      if (_isPrimary != value)
      {
        _isPrimary = value;
        OnPropertyChanged(nameof(IsPrimary));
      }
    }
  }

  /// <summary>
  /// Gets the full name including prefix, first name, last name, and suffix
  /// </summary>
  [XmlIgnore]
  public string FullName
  {
    get
    {
      string name = string.Empty;

      if (!string.IsNullOrEmpty(Prefix))
      {
        name += Prefix + " ";
      }

      if (!string.IsNullOrEmpty(FirstName))
      {
        name += FirstName;
      }

      if (!string.IsNullOrEmpty(LastName))
      {
        name += (string.IsNullOrEmpty(name) ? "" : " ") + LastName;
      }

      if (!string.IsNullOrEmpty(Suffix))
      {
        name += " " + Suffix;
      }

      return name.Trim();
    }
  }

  #endregion

  #region Constructors

  /// <summary>
  /// Empty constructor for serialization
  /// </summary>
  public Name()
  {
    _nameType = NameType.Birth;
    _isPrimary = false;
  }

  /// <summary>
  /// Constructor with first and last name
  /// </summary>
  public Name(string firstName, string lastName) : this()
  {
    _firstName = firstName;
    _lastName = lastName;
  }

  /// <summary>
  /// Constructor with all name components
  /// </summary>
  public Name(string firstName, string lastName, string suffix, string prefix, NameType nameType, bool isPrimary)
  {
    _firstName = firstName;
    _lastName = lastName;
    _suffix = suffix;
    _prefix = prefix;
    _nameType = nameType;
    _isPrimary = isPrimary;
  }

  #endregion

  #region Methods

  public override string ToString()
  {
    return FullName;
  }

  public bool Equals(Name other)
  {
    if (other == null)
    {
      return false;
    }

    return string.Equals(FirstName, other.FirstName, StringComparison.OrdinalIgnoreCase) &&
           string.Equals(LastName, other.LastName, StringComparison.OrdinalIgnoreCase) &&
           string.Equals(Suffix, other.Suffix, StringComparison.OrdinalIgnoreCase) &&
           string.Equals(Prefix, other.Prefix, StringComparison.OrdinalIgnoreCase) &&
           NameType == other.NameType;
  }

  public override bool Equals(object obj)
  {
    return Equals(obj as Name);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(
      FirstName?.ToLowerInvariant(),
      LastName?.ToLowerInvariant(),
      Suffix?.ToLowerInvariant(),
      Prefix?.ToLowerInvariant(),
      NameType
    );
  }

  #endregion

  #region INotifyPropertyChanged Members

  public event PropertyChangedEventHandler PropertyChanged;

  protected void OnPropertyChanged(string propertyName)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  #endregion
}

/// <summary>
/// Type of name (birth, married, aka, etc.)
/// Based on GEDCOM 5.5.1 NAME_TYPE values
/// </summary>
[Serializable]
public enum NameType
{
  /// <summary>
  /// Birth name (default)
  /// </summary>
  Birth,

  /// <summary>
  /// Also known as, alias
  /// </summary>
  Aka,

  /// <summary>
  /// Married name
  /// </summary>
  Married,

  /// <summary>
  /// Maiden name
  /// </summary>
  Maiden,

  /// <summary>
  /// Immigration name
  /// </summary>
  Immigration,

  /// <summary>
  /// Professional/stage name
  /// </summary>
  Professional,

  /// <summary>
  /// Other/custom name type
  /// </summary>
  Other
}

/// <summary>
/// Collection of names for a person
/// </summary>
[Serializable]
public class NameCollection : ObservableCollection<Name>
{
  /// <summary>
  /// Gets the primary name from the collection, or the first name if no primary is set
  /// </summary>
  public Name PrimaryName
  {
    get
    {
      // Find the name marked as primary
      foreach (Name name in this)
      {
        if (name.IsPrimary)
        {
          return name;
        }
      }

      // If no primary is set, return the first name
      if (Count > 0)
      {
        return this[0];
      }

      // Return empty name if collection is empty
      return new Name();
    }
  }

  /// <summary>
  /// Sets a name as the primary name, clearing IsPrimary from all other names
  /// </summary>
  public void SetPrimary(Name name)
  {
    if (name == null || !Contains(name))
    {
      return;
    }

    foreach (Name n in this)
    {
      n.IsPrimary = (n == name);
    }
  }
}
