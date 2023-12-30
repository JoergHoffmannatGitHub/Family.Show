using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

using FamilyShow.Properties;
using FamilyShowLib;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace FamilyShow;

internal partial class App : Application
{
  #region fields

  // The name of the application folder.  This folder is used to save the files 
  // for this application such as the photos, stories and family data.
  internal const string ApplicationFolderName = "Family.Show";

  internal const string AppDataFolderName = "Family Data";

  internal const string SampleFilesFolderName = "Sample Files";

  // The main list of family members that is shared for the entire application.
  // The FamilyCollection and Family fields are accessed from the same thread.
  public static People FamilyCollection = new();
  public static PeopleCollection Family = FamilyCollection.PeopleCollection;
  public static SourceCollection Sources = FamilyCollection.SourceCollection;
  public static RepositoryCollection Repositories = FamilyCollection.RepositoryCollection;

  // The number of recent files to keep track of.
  private const int NumberOfRecentFiles = 5;

  // The path to the recent files file.
  private static readonly string s_recentFilesFilePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Path.Combine(ApplicationFolderName, "RecentFiles.xml"));
  public static string args;
  public static bool canExecuteJumpList = true;

  #endregion

  #region Properties

  internal MainWindow Window
  {
    get { return MainWindow as MainWindow; }
  }

  #endregion

  // A method to handle command line arguments.
  public void ProcessArgs(string[] args)
  {
    if (args.Length > 0)
    {
      App.args = Convert.ToString(args[0]);
    }
    else
    {
      App.args = "/x";
    }

    Window?.ProcessCommandLines();
  }

  protected override void OnStartup(StartupEventArgs e)
  {
    LoadLanguageResources();
    InstallSampleFiles();
    CleanUpTempDirectory();
    CreateWorkingDirectory();

    // Load the collection of recent files.
    LoadRecentFiles();

    InitializeDefaultTheme();

    // Create and show the application's main window            
    MainWindow window = new();
    window.Show();

    // In Windows 7 make use of new TaskBar and JumpList features.
    InitializeTaskBar(window);

    base.OnStartup(e);
  }

  private static void InitializeDefaultTheme()
  {
    Settings appSettings = Settings.Default;

    if (!string.IsNullOrEmpty(appSettings.Theme))
    {
      try
      {
        ResourceDictionary rd = [];
        rd.MergedDictionaries.Add(LoadComponent(new Uri(appSettings.Theme, UriKind.Relative)) as ResourceDictionary);
        Current.Resources = rd;
      }
      catch { }
    }
  }

  private static void InitializeTaskBar(Window window)
  {
    string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
    string applicationFilePath = Assembly.GetExecutingAssembly().Location;
    _ = TaskBar.Create(window, Settings.Default.AppId, new JumpListLink[]
    {
              new(applicationFilePath, FamilyShow.Properties.Resources.StartANewFamilyTree)
              {
                  Arguments = "/n",
                  IconReference = new IconReference(Path.Combine(systemFolder, "shell32.dll"), 0),
              },
              new(applicationFilePath, FamilyShow.Properties.Resources.OpenMenu)
              {
                  Arguments = "/o",
                  IconReference = new IconReference(Path.Combine(systemFolder, "shell32.dll"), 4),
              },
              new(applicationFilePath, FamilyShow.Properties.Resources.GedcomMenu)
              {
                  Arguments = "/i",
                  IconReference = new IconReference(Path.Combine(systemFolder, "shell32.dll"), 4),
              }
    });
  }

  protected override void OnExit(ExitEventArgs e)
  {
    SaveRecentFiles();
    CleanUpTempDirectory();
    base.OnExit(e);
  }

  public void Activate()
  {
    // Reactivate application's main window
    MainWindow.Activate();
    MainWindow.Focus();
  }

  private static void LoadLanguageResources()
  {
    // Load language resources and use en-US incase of errors.
    FamilyShow.Properties.Resources.Culture = new CultureInfo("en-US");
    FamilyShowLib.Properties.Resources.Culture = new CultureInfo("en-US");

    if (File.Exists(Settings.Default.LanguagesFileName))
    {
      try
      {
        FamilyShow.Properties.Resources.Culture = new CultureInfo(Settings.Default.Language);
        FamilyShowLib.Properties.Resources.Culture = new CultureInfo(Settings.Default.Language);
      }
      catch
      {
        // Error with selected language, reset to default.
        Settings.Default.Language = "en-US";
        Settings.Default.Save();
      }
    }
    else
    {
      Settings.Default.Language = "en-US";
      Settings.Default.Save();
    }
  }

  #region methods

  /// <summary>
  /// Gets the list of recent files.
  /// </summary>
  public static StringCollection RecentFiles { get; private set; } = [];

  /// <summary>
  /// Gets the collection of themes
  /// </summary>
  public static NameValueCollection Themes
  {
    get
    {
      NameValueCollection themes = [];

      foreach (string folder in Directory.GetDirectories("Themes"))
      {
        foreach (string file in Directory.GetFiles(folder))
        {
          FileInfo fileInfo = new(file);
          if (string.Compare(fileInfo.Extension, FamilyShow.Properties.Resources.XamlExtension,
              true, CultureInfo.InvariantCulture) == 0)
          {
            // Use the first part of the resource file name for the menu item name.
            themes.Add(fileInfo.Name.Remove(fileInfo.Name.IndexOf("Resources")),
                 Path.Combine(folder, fileInfo.Name));
          }
        }
      }

      return themes;
    }
  }

  /// <summary>
  /// Return the animation duration. The duration is extended
  /// if special keys are currently pressed (for demo purposes)  
  /// otherwise the specified duration is returned. 
  /// </summary>
  public static TimeSpan GetAnimationDuration(double milliseconds)
  {
    return TimeSpan.FromMilliseconds(
        Keyboard.IsKeyDown(Key.F12) ?
        milliseconds * 5 : milliseconds);
  }

  /// <summary>
  /// Load the list of recent files from disk.
  /// </summary>
  public static void LoadRecentFiles()
  {
    if (File.Exists(s_recentFilesFilePath))
    {
      // Load the Recent Files from disk
      XmlSerializer ser = new(typeof(StringCollection));
      using (TextReader reader = new StreamReader(s_recentFilesFilePath))
      {
        RecentFiles = (StringCollection)ser.Deserialize(reader);
      }

      // Remove files from the Recent Files list that no longer exists.
      for (int i = 0; i < RecentFiles.Count; i++)
      {
        if (!File.Exists(RecentFiles[i]))
        {
          RecentFiles.RemoveAt(i);
        }
      }

      // Only keep the 5 most recent files, trim the rest.
      while (RecentFiles.Count > NumberOfRecentFiles)
      {
        RecentFiles.RemoveAt(NumberOfRecentFiles);
      }
    }
  }

  /// <summary>
  /// Save the list of recent files to disk.
  /// </summary>
  public static void SaveRecentFiles()
  {
    XmlSerializer ser = new(typeof(StringCollection));
    using (TextWriter writer = new StreamWriter(s_recentFilesFilePath))
    {
      ser.Serialize(writer, RecentFiles);
    }
  }

  /// <summary>
  /// Create the working directory the first time the program runs
  /// </summary>
  private static void CreateWorkingDirectory()
  {
    // Full path to the document file location.
    string location = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.MyDocuments), ApplicationFolderName);

    // Return right away if the data file already exist.
    if (Directory.Exists(location))
    {
      return;
    }

    try
    {
      // Creates the working directory
      Directory.CreateDirectory(location);
    }
    catch
    {
      // Could not create the working directory
    }
  }

  /// <summary>
  /// Clean up all temp files when program terminates or starts.
  /// </summary>
  private static void CleanUpTempDirectory()
  {
    string appLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ApplicationFolderName);
    appLocation = Path.Combine(appLocation, AppDataFolderName);

    try
    {
      // Creates the working directory
      if (Directory.Exists(appLocation))
      {
        Directory.Delete(appLocation, true);
      }

      Directory.CreateDirectory(appLocation);
    }
    catch
    {
      // Could not create the working directory
    }
  }

  /// <summary>
  /// Install sample files the first time the application runs.
  /// </summary>
  private static void InstallSampleFiles()
  {
    // Full path to the document file location.
    string location = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.MyDocuments), ApplicationFolderName);

    location = Path.Combine(location, SampleFilesFolderName);

    // Return right away if the data file already exist.
    if (Directory.Exists(location))
    {
      return;
    }

    try
    {
      // Sample data files.
      Directory.CreateDirectory(location);
      CreateSampleFile(location, "Windsor.familyx", FamilyShow.Properties.Resources.WindsorSampleFile);
      CreateSampleFile(location, "Kennedy.ged", FamilyShow.Properties.Resources.KennedySampleFile);
    }

    catch
    {
      // Could not install the sample files, handle all exceptions the
      // same, ignore and continue without installing the sample files.
    }
  }

  /// <summary>
  /// Extract the sample family files from the executable and write it to the file system.
  /// </summary>
  private static void CreateSampleFile(string location, string fileName, byte[] fileContent)
  {
    // Full path to the sample file.
    string path = Path.Combine(location, fileName);

    // Return right away if the file already exists.
    if (File.Exists(path))
    {
      return;
    }

    // Create the file.
    using (BinaryWriter writer = new(File.Open(path, FileMode.Create)))
    {
      writer.Write(fileContent);
    }
  }

  /// <summary>
  /// Converts string to date time object using DateTime.TryParse.  
  /// Also accepts just the year for dates. 1977 = 1/1/1977.
  /// </summary>
  internal static DateTime StringToDate(string dateString)
  {

    //Append first month and day if just the year was entered.
    if (dateString.Length == 4)
    {
      dateString = "1/1/" + dateString;
    }

    _ = DateTime.TryParse(dateString, out DateTime date);

    return date;
  }

  /// <summary>
  /// Converts a DateTime to a short string.  If DateTime is null, returns an empty string.
  /// </summary>
  /// <param name="date"></param>
  /// <returns></returns>
  internal static string DateToString(DateTime? date)
  {

    if (date == null)
    {
      return string.Empty;
    }
    else
    {
      return date.Value.ToShortDateString();
    }
  }

  /// <summary>
  /// Replaces spaces and { } in file names which break relative paths
  /// </summary>
  /// <param name="fileName"></param>
  /// <returns></returns>
  internal static string ReplaceEncodedCharacters(string fileName)
  {
    fileName = fileName.Replace(" ", string.Empty);
    fileName = fileName.Replace("{", string.Empty);
    fileName = fileName.Replace("}", string.Empty);
    return fileName;
  }

  /// <summary>
  /// Determines if an image file is supported based on its extension.
  /// </summary>
  /// <param name="fileName"></param>
  /// <returns></returns>
  internal static bool IsPhotoFileSupported(string fileName)
  {
    string extension = Path.GetExtension(fileName);

    if (string.Compare(extension, ".jpg", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".jpeg", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".png", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".gif", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".tiff", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".tif", true, CultureInfo.InvariantCulture) == 0)
    {
      return true;
    }

    return false;
  }

  /// <summary>
  /// Determines if an attachment file is supported based on its extension.
  /// </summary>
  /// <param name="fileName"></param>
  /// <returns></returns>
  internal static bool IsAttachmentFileSupported(string fileName)
  {

    string extension = Path.GetExtension(fileName);

    // Only allow certain file types
    if (string.Compare(extension, ".docx", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".xlsx", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".pptx", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".odt", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".ods", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".odp", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".doc", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".xls", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".ppt", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".txt", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".htm", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".html", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".pdf", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".xps", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".rtf", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".kml", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".kmz", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".jpg", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".jpeg", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".png", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".gif", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".tiff", true, CultureInfo.InvariantCulture) == 0 ||
        string.Compare(extension, ".tif", true, CultureInfo.InvariantCulture) == 0)
    {
      return true;
    }

    return false;
  }

}

#endregion
