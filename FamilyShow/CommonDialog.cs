/*
 * Displays the common Open and SaveAs dialogs using the dialogs.
*/

using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.FamilyShow;

/// <summary>
/// One item in the common dialog filter.
/// </summary>
public class FilterEntry(string display, string extension)
{
  public string Display { get; } = display;

  public string Extension { get; } = extension;
}

/// <summary>
/// Displays the common Open and SaveAs dialogs using the Vista-style dialogs.
/// </summary>
internal class CommonDialog
{
  #region properties

  public List<FilterEntry> Filter { get; } = [];

  public string Title { private get; set; }

  public string InitialDirectory { private get; set; }

  public string DefaultExtension { private get; set; }

  public string FileName { get; set; }

  #endregion

  /// <summary>
  /// Displays a dialog box from which the user can select a file.
  /// </summary>
  public bool ShowOpen()
  {
    using OpenFileDialog openFileDialog = new();
    openFileDialog.Filter = SetFilter();
    openFileDialog.DefaultExt = DefaultExtension;
    if (openFileDialog.ShowDialog() == DialogResult.OK)
    {
      FileName = openFileDialog.FileName;
      return true;
    }

    return false;
  }


  /// <summary>
  /// Prompts the user to select a location for saving a file. This class cannot be inherited.
  /// </summary>
  public bool ShowSave()
  {
    using SaveFileDialog saveFileDialog = new();
    saveFileDialog.Filter = SetFilter();
    saveFileDialog.CheckPathExists = true;
    saveFileDialog.OverwritePrompt = true;
    saveFileDialog.DefaultExt = DefaultExtension;
    if (saveFileDialog.ShowDialog() == DialogResult.OK)
    {
      FileName = saveFileDialog.FileName;
      return true;
    }

    return false;
  }

  /// <summary>
  /// Set the low level filter with the filter collection.
  /// </summary>
  private string SetFilter()
  {
    StringBuilder sb = new();
    foreach (FilterEntry entry in Filter)
    {
      sb.AppendFormat("{0}|{1}|", entry.Display, entry.Extension);
    }
    sb.Length--;
    return sb.ToString();
  }


}
