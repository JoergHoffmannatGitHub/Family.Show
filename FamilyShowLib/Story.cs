using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace FamilyShowLib;

[Serializable]
public class Story : INotifyPropertyChanged
{
    #region Fields and Constants

    public const string StoriesFolderName = "Stories";
    private string _relativePath;

    #endregion

    #region Properties

    /// <summary>
    /// The relative path to the story file.
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
    /// The fully qualified path to the story.
    /// </summary>
    [XmlIgnore]
    public string AbsolutePath
    {
        get
        {
            //LocalApplicationData
            string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            App.ApplicationFolderName);
            tempFolder = Path.Combine(tempFolder, App.AppDataFolderName);

            if (_relativePath != null)
            {
                return Path.Combine(tempFolder, _relativePath);
            }
            else
            {
                return string.Empty;
            }
        }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Empty constructor is needed for serialization
    /// </summary>
    public Story() { }

    #endregion

    #region Methods

    /// <summary>
    /// Save the story to the file system.
    /// </summary>
    public void Save(TextRange storyText, string storyFileName)
    {
        // Data format for the story file.
        string dataFormat = DataFormats.Rtf;

        string tempFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            App.ApplicationFolderName);
        tempFolder = Path.Combine(tempFolder, App.AppDataFolderName);

        // Absolute path to the stories folder
        string storiesLocation = Path.Combine(tempFolder, StoriesFolderName);

        // Fully qualified path to the new story file
        storyFileName = GetSafeFileName(storyFileName);
        string storyAbsolutePath = Path.Combine(storiesLocation, storyFileName);

        try
        {
            // Create the stories directory if it doesn't exist
            if (!Directory.Exists(storiesLocation))
            {
                Directory.CreateDirectory(storiesLocation);
            }

            // Open the file for writing the richtext
            using (FileStream stream = File.Create(storyAbsolutePath))
            {
                // Store the relative path to the story
                _relativePath = Path.Combine(StoriesFolderName, storyFileName);

                // Save the story to disk
                if (storyText.CanSave(dataFormat))
                {
                    storyText.Save(stream, dataFormat);
                }
            }
        }
        catch
        {
            // Could not save the story. Handle all exceptions
            // the same, ignore and continue.
            // inform the developer
            Debug.Assert(false);
        }
    }

    /// <summary>
    /// Load the History from file to the textrange.
    /// </summary>
    public void Load(TextRange storyText)
    {
        // Data format for the person's story file.
        string dataFormat = DataFormats.Rtf;

        if (File.Exists(AbsolutePath))
        {
            try
            {
                // Open the file for reading
                using (FileStream stream = File.OpenRead(AbsolutePath))
                {
                    if (storyText.CanLoad(dataFormat))
                    {
                        storyText.Load(stream, dataFormat);
                    }
                }
            }
            catch
            {
                // Could not load the story. Handle all exceptions
                // the same, ignore and continue.
                // inform the developer
                Debug.Assert(false);
            }
        }
    }

    /// <summary>
    /// Save the person's story on the file system.  Accepts plain text for Gedcom support
    /// </summary>
    public void Save(string storyText, string storyFileName)
    {
        // Convert the text into a TextRange.  This will allow saving the story to disk as RTF.
        TextBlock block = new()
        {
            Text = storyText
        };

        TextRange range = new(block.ContentStart, block.ContentEnd);

        Save(range, storyFileName);
    }

    public void Delete()
    {
        // Delete the person's story if it exists
        if (File.Exists(AbsolutePath))
        {
            try
            {
                File.Delete(AbsolutePath);
            }
            catch
            {
                // Could not delete the file. Handle all exceptions
                // the same, ignore and continue.
                // inform the developer
                Debug.Assert(false);
            }
        }
    }

    /// <summary>
    /// Return a string that is a safe file name.
    /// </summary>
    private static string GetSafeFileName(string fileName)
    {
        // List of invalid characters.
        char[] invalid = Path.GetInvalidFileNameChars();

        // Remove all invalid characters.
        int pos;
        while ((pos = fileName.IndexOfAny(invalid)) != -1)
        {
            fileName = fileName.Remove(pos, 1);
        }

        return fileName;
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
