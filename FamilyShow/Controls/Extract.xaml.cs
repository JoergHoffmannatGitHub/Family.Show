using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using FamilyShowLib;

namespace FamilyShow;

/// <summary>
/// Interaction logic for Extract.xaml
/// </summary>
public partial class Extract : UserControl
{

  #region fields

  private readonly PeopleCollection _family = App.Family;
  private readonly People _familyCollection = App.FamilyCollection;

  #endregion

  public Extract()
  {
    InitializeComponent();
  }

  #region routed events

  public static readonly RoutedEvent ExtractButtonClickEvent = EventManager.RegisterRoutedEvent(
    "ExtractButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Extract));

  public static readonly RoutedEvent CancelButtonClickEvent = EventManager.RegisterRoutedEvent(
    "CancelButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Extract));

  // Expose this event for this control's container
  public event RoutedEventHandler CancelButtonClick
  {
    add { AddHandler(CancelButtonClickEvent, value); }
    remove { RemoveHandler(CancelButtonClickEvent, value); }
  }

  #endregion

  #region methods

  private void ExtractButton_Click(object sender, RoutedEventArgs e)
  {
    RaiseEvent(new RoutedEventArgs(CancelButtonClickEvent));
    ExtractFiles();
    Clear();
  }

  private void CancelButton_Click(object sender, RoutedEventArgs e)
  {
    RaiseEvent(new RoutedEventArgs(CancelButtonClickEvent));
    Clear();
  }

  #endregion

  #region helper methods

  private void Clear()
  {
    Option1.IsChecked = true;
    Option2.IsChecked = true;
    Option3.IsChecked = true;
    Option4.IsChecked = false;
    Option5.IsChecked = true;
  }

  private void ExtractFiles()
  {
    //default options
    bool extractPhotos = Option1.IsChecked.Value;
    bool extractStories = Option2.IsChecked.Value;
    bool extractAttachments = Option3.IsChecked.Value;
    bool openFolderAfterExtraction = Option5.IsChecked.Value;
    bool currentPersonOnly = Option4.IsChecked.Value;

    string folderName = Properties.Resources.Unknown + " (" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + ")";
    string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + App.ApplicationFolderName;

    if (!string.IsNullOrEmpty(_familyCollection.FullyQualifiedFilename))
    {
      folderName = Path.GetFileNameWithoutExtension(_familyCollection.FullyQualifiedFilename);
      folderPath = Path.GetDirectoryName(_familyCollection.FullyQualifiedFilename);
    }

    if (currentPersonOnly && _family.Current != null)
    {
      folderName = folderName + " - " + _family.Current.Name;
    }

    string contentpath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + App.ApplicationFolderName + @"\" + App.AppDataFolderName;

    DirectoryInfo extractedFileLocation;

    if (Directory.Exists(Path.Combine(folderPath, folderName)))
    {
      int i = 1;
      string newPath;
      do
      {
        newPath = Path.Combine(folderPath, folderName + " (" + i + ")");
        i++;
      } while (Directory.Exists(newPath));

      extractedFileLocation = Directory.CreateDirectory(newPath);
    }
    else
    {
      extractedFileLocation = Directory.CreateDirectory(Path.Combine(folderPath, folderName));
    }

    string[] photosToExtract = null;
    string[] storiesToExtract = null;
    string[] attachmentsToExtract = null;

    if (!currentPersonOnly)
    {

      if (Directory.Exists(Path.Combine(contentpath, Photo.PhotosFolderName)))
      {
        photosToExtract = Directory.GetFiles(Path.Combine(contentpath, Photo.PhotosFolderName), "*", SearchOption.AllDirectories);
      }

      if (Directory.Exists(Path.Combine(contentpath, Story.StoriesFolderName)))
      {
        storiesToExtract = Directory.GetFiles(Path.Combine(contentpath, Story.StoriesFolderName), "*", SearchOption.AllDirectories);
      }

      if (Directory.Exists(Path.Combine(contentpath, Attachment.AttachmentsFolderName)))
      {
        attachmentsToExtract = Directory.GetFiles(Path.Combine(contentpath, Attachment.AttachmentsFolderName), "*", SearchOption.AllDirectories);
      }

      if (extractAttachments && attachmentsToExtract != null)
      {
        Directory.CreateDirectory(Path.Combine(folderPath, folderName + @"\" + Attachment.AttachmentsFolderName));

        foreach (string file in attachmentsToExtract)
        {
          try
          {
            FileInfo f = new(file);
            f.CopyTo(Path.Combine(Path.Combine(folderPath, folderName + @"\" + Attachment.AttachmentsFolderName), Path.GetFileName(file)), true);
          }
          catch { }
        }
      }

      if (extractPhotos && photosToExtract != null)
      {
        Directory.CreateDirectory(Path.Combine(folderPath, folderName + @"\" + Photo.PhotosFolderName));

        foreach (string file in photosToExtract)
        {
          try
          {
            FileInfo f = new(file);
            f.CopyTo(Path.Combine(Path.Combine(folderPath, folderName + @"\" + Photo.PhotosFolderName), Path.GetFileName(file)), true);
          }
          catch { }
        }
      }

      if (extractStories && storiesToExtract != null)
      {
        Directory.CreateDirectory(Path.Combine(folderPath, folderName + @"\" + Story.StoriesFolderName));

        foreach (string file in storiesToExtract)
        {
          try
          {
            FileInfo f = new(file);
            f.CopyTo(Path.Combine(Path.Combine(folderPath, folderName + @"\" + Story.StoriesFolderName), Path.GetFileName(file)), true);
          }
          catch { }
        }
      }
    }
    else
    {
      if (_family.Current != null)
      {
        Directory.CreateDirectory(Path.Combine(folderPath, folderName + @"\" + Attachment.AttachmentsFolderName));
        Directory.CreateDirectory(Path.Combine(folderPath, folderName + @"\" + Photo.PhotosFolderName));
        Directory.CreateDirectory(Path.Combine(folderPath, folderName + @"\" + Story.StoriesFolderName));

        foreach (Photo p in _family.Current.Photos)
        {
          string file = p.FullyQualifiedPath;

          try
          {
            FileInfo f = new(file);
            f.CopyTo(Path.Combine(Path.Combine(folderPath, folderName + @"\" + Photo.PhotosFolderName), Path.GetFileName(file)), true);
          }
          catch { }
        }

        try
        {
          FileInfo f = new(_family.Current.Story.AbsolutePath);
          f.CopyTo(Path.Combine(Path.Combine(folderPath, folderName + @"\" + Story.StoriesFolderName), Path.GetFileName(_family.Current.Story.AbsolutePath)), true);
        }
        catch { }

        foreach (Attachment a in _family.Current.Attachments)
        {
          string file = a.FullyQualifiedPath;

          try
          {
            FileInfo f = new(file);
            f.CopyTo(Path.Combine(Path.Combine(folderPath, folderName + @"\" + Attachment.AttachmentsFolderName), Path.GetFileName(file)), true);
          }
          catch { }
        }
      }
    }

    if (openFolderAfterExtraction)
    {
      try
      {
        System.Diagnostics.Process.Start(extractedFileLocation.FullName);
      }
      catch { }
    }

    Clear();

  }

  #endregion


}
