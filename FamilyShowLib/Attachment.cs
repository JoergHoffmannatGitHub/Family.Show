using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace FamilyShowLib;

/// <summary>
/// Simple representation of a serializable attachment associated with the Person class
/// </summary>
[Serializable]
public class Attachment : INotifyPropertyChanged
{
  #region Fields and Constants

  public const string AttachmentsFolderName = "Attachments";
  private string _relativePath;

  #endregion

  #region Properties

  /// <summary>
  /// The relative path to the attachment.
  /// </summary>
  public string RelativePath
  {
    get { return _relativePath; }
    set
    {
      if (_relativePath != value)
      {
        _relativePath = value;
        OnPropertyChanged("relativePath");
      }
    }
  }

  /// <summary>
  /// The file name of the attachment.
  /// </summary>
  public string FileName
  {
    get { return Path.GetFileName(_relativePath); }
    set { }
  }


  /// <summary>
  /// The fully qualified path to the attachment.
  /// </summary>
  [XmlIgnore]
  public string FullyQualifiedPath
  {
    get
    {

      string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
      App.ApplicationFolderName);
      tempFolder = Path.Combine(tempFolder, App.AppDataFolderName);

      return Path.Combine(tempFolder, _relativePath);
    }
    set
    {
      // This empty setter is needed for serialization.
    }
  }

  #endregion

  #region Constructors

  /// <summary>
  /// Empty constructor is needed for serialization
  /// </summary>
  public Attachment() { }

  /// <summary>
  /// Constructor for Attachment. Copies the attachmentPath to the attachments folder
  /// </summary>
  public Attachment(string attachmentPath)
  {

    if (!string.IsNullOrEmpty(attachmentPath))
    {
      // Copy the attachment to the attachments folder
      _relativePath = Copy(attachmentPath);
    }
  }

  #endregion

  #region Methods

  public override string ToString()
  {
    return FullyQualifiedPath;
  }

  /// <summary>
  /// Copies the attachment file to the application attachments folder. 
  /// Returns the relative path to the copied attachment.
  /// </summary>
  private static string Copy(string fileName)
  {

    // The attachment file being copied
    FileInfo fi = new(fileName);

    // Absolute path to the application folder
    string appLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        App.ApplicationFolderName);
    appLocation = Path.Combine(appLocation, App.AppDataFolderName);

    // Absolute path to the attachments folder
    string attachmentLocation = Path.Combine(appLocation, AttachmentsFolderName);

    // Fully qualified path to the new attachment file
    string attachmentFullPath = Path.Combine(attachmentLocation, App.ReplaceEncodedCharacters(fi.Name));

    // Relative path to the new attachment file

    string attachmentRelLocation = Path.Combine(AttachmentsFolderName, App.ReplaceEncodedCharacters(fi.Name));

    // Create the appLocation directory if it doesn't exist
    if (!Directory.Exists(appLocation))
    {
      Directory.CreateDirectory(appLocation);
    }

    // Create the attachments directory if it doesn't exist
    if (!Directory.Exists(attachmentLocation))
    {
      Directory.CreateDirectory(attachmentLocation);
    }

    // Copy the attachment.
    try
    {
      string attachmentName = Path.GetFileName(attachmentFullPath);
      string attachmentNameNoExt = Path.GetFileNameWithoutExtension(attachmentFullPath);
      string attachmentNameExt = Path.GetExtension(attachmentFullPath);

      int i = 1;

      if (File.Exists(attachmentFullPath))
      {
        do
        {
          attachmentName = attachmentNameNoExt + "(" + i + ")" + attachmentNameExt;  //don't overwrite existing files, append (#) to file if exists.
          attachmentRelLocation = Path.Combine(AttachmentsFolderName, attachmentName);
          attachmentFullPath = Path.Combine(attachmentLocation, attachmentName);
          i++;
        }
        while (File.Exists(attachmentFullPath));

      }
      fi.CopyTo(attachmentFullPath, true);

    }
    catch
    {
      // Could not copy the photo. Handle all exceptions 
      // the same, ignore and continue.
      // inform the developper
      Debug.Assert(false);
    }

    return attachmentRelLocation;
  }

  #endregion

  #region INotifyPropertyChanged Members

  public event PropertyChangedEventHandler PropertyChanged;

  protected virtual void OnPropertyChanged(string propertyName)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  #endregion
}

/// <summary>
/// Collection for attachments.
/// </summary>
[Serializable]
public class AttachmentCollection : ObservableCollection<Attachment>
{
}
