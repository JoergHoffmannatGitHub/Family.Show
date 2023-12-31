/*
 * GedcomConverter class
 * Converts a GEDCOM file to an XML file.
 * 
 * GedcomLine class
 * Parses one line in a GEDCOM file.
*/

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace FamilyShowLib;

/// <summary>
/// Converts a GEDCOM file to an XML file.
/// </summary>
internal static class GedcomConverter
{

  /// Create an xml file that contains the same hierarchy specified in 
  /// the GEDCOM file. GEDCOM lines are limited to 255 characters, 
  /// combineSplitValues indicates if the split lines should be combined 
  /// into a single XML element.
  public static void ConvertToXml(string gedcomFilePath, string xmlFilePath, bool combineSplitValues, bool disableCharacterCheck)
  {
    // Store the previous level so can determine when need
    // to close xml element tags.
    int prevLevel = -1;

    // Used to create the .xml file, XmlWriterSettings. Indent is 
    // specified if you want to examine the xml file, otherwise
    // it should be removed.
    XmlWriterSettings settings = new()
    {
      Indent = true
    };
    using (XmlWriter writer = XmlWriter.Create(xmlFilePath, settings))
    {
      // Root element that contains all of the other elements.
      writer.WriteStartElement("root");

      // Convert each line of the gedcom file to an xml element.
      using (StreamReader sr = new(gedcomFilePath))
      {
        string text;
        GedcomLine line = new();
        while ((text = sr.ReadLine()) != null)
        {
          // Some GEDCOM files indent each line with whitespace, delete any
          // whitespace from the beginning and end of the line.
          text = text.Trim();

          // Parse gedcome line into Level, Tag and Value fields.
          if (line.Parse(text, disableCharacterCheck))
          {
            // See if need to close previous xml elements.
            if (line.Level <= prevLevel)
            {
              // Determine how many elements to close.
              int count = prevLevel - line.Level + 1;
              for (int i = 0; i < count; i++)
              {
                writer.WriteEndElement();
              }
            }

            // Create new xml element.
            writer.WriteStartElement(line.Tag);
            writer.WriteAttributeString("Value", line.Data);

            prevLevel = line.Level;
          }
        }
      }

      // Close the last element.
      writer.WriteEndElement();

      // Close the root element.
      writer.WriteEndElement();

      // Write to the file system.
      writer.Flush();
      writer.Close();

      if (combineSplitValues)
      {
        CombineSplitValues(xmlFilePath);
      }
    }
  }

  /// <summary>
  /// GEDCOM lines have a max length of 255 characters, this goes through
  /// the XML files and combines all of the split lines which makes the
  /// XML file easier to process.
  /// </summary>
  private static void CombineSplitValues(string xmlFilePath)
  {
    XmlDocument doc = new();
    doc.Load(xmlFilePath);

    // Get all nodes that contain child continue nodes.


    XmlNodeList list = doc.SelectNodes("//CONT/.. | //CONC/..");
    // Go through each node and append child continue nodes.
    foreach (XmlNode node in list)
    {
      AppendValues(node);
    }

    // Write the updates to the file system.
    doc.Save(xmlFilePath);
  }

  /// <summary>
  /// Append child continue nodes to the parent.
  /// </summary>
  private static void AppendValues(XmlNode node)
  {
    // Get the value for the parent node.
    StringBuilder sb = new StringBuilder(node.Attributes["Value"].Value).AppendLine();

    // Find all of the child continue nodes.
    XmlNodeList list = node.SelectNodes("CONT | CONC");

    foreach (XmlNode childNode in list)
    {
      switch (childNode.Name)
      {
        //Concatenate.
        case "CONC":
          sb.AppendFormat("{0}", " ");  //may add spaces between words which are split over two lines
          sb.Append(childNode.Attributes["Value"].Value);
          break;

        // Continue, add line return and then the text.
        case "CONT":
          sb.AppendFormat("\r{0}", childNode.Attributes["Value"].Value).AppendLine(); // \r is not respected
          break;
      }

      // Remove all of the child continue nodes.
      node.RemoveChild(childNode);
    }

    // Update the parent node value.
    node.Attributes["Value"].Value = sb.ToString();
  }
}

/// <summary>
/// Parses one line in a GEDCOM file.
/// </summary>
internal partial class GedcomLine
{
  #region regex

  // Expression pattern used to parse the GEDCOM line.
  [GeneratedRegex(@"(?<level>\d+)\s+(?<tag>[\S]+)(\s+(?<data>.+))?")]
  private static partial Regex RegexToSplit();

  // Expression pattern used to clean up the GEDCOM line.
  // Only allow viewable characters.
  [GeneratedRegex(@"[^\x20-\x7e]")]
  private static partial Regex RegexToClean();

  // Expression pattern used to clean up the GEDCOM tag.
  // Tag can contain alphanumeric characters, _, ., or -.
  [GeneratedRegex(@"[^\w.-]")]
  private static partial Regex RegexForTag();

  #endregion

  #region properties

  /// <summary>
  /// Level of the tag.
  /// </summary>
  public int Level { get; set; }

  /// <summary>
  /// Line tag.
  /// </summary>
  public string Tag { get; set; }

  /// <summary>
  /// Data of the tag.
  /// </summary>
  public string Data { get; set; }

  #endregion

  /// <summary>
  /// Parse the Level, Tag, and Data fields from the GEDCOM line.
  /// The following is a sample GEDCOM line:
  /// 
  ///    2 NAME Personal Ancestral File
  ///    
  /// The Level = 2, Tag = NAME, and Data = Personal Ancestral File.
  /// </summary>
  public bool Parse(string text, bool disableCharacterCheck)
  {
    try
    {
      // Init values.
      Clear();

      // Return right away is nothing to parse.
      if (string.IsNullOrEmpty(text))
      {
        return false;
      }

      //Clean up the line by only allowing viewable characters.
      //Allow override for if UTF-8 encoded GEDCOM file is being used.
      if (!disableCharacterCheck)
      {
        text = RegexToClean().Replace(text, "");
      }

      text = text.Replace("@@", "@");  // in GEDCOM @ is encoded as @ for content.

      // Get the parts of the line.
      Match match = RegexToSplit().Match(text);
      Level = Convert.ToInt32(match.Groups["level"].Value, CultureInfo.InvariantCulture);
      Tag = match.Groups["tag"].Value.Trim();
      Data = match.Groups["data"].Value.Trim();

      // The pointer reference is specified in the tag, and the tag in the data,
      // swap these two values to make it more consistent, the tag contains the 
      // tag and data contains the pointer reference.
      if (Tag[0] == '@')
      {
        (Data, Tag) = (Tag, Data);
        int pos = Tag.IndexOf(' ');

        // Some GEDCOM files have additional info, 
        // we only handle the tag info.
        if (pos != -1)
        {
          Tag = Tag.Substring(0, pos);
        }
      }

      // Make sure there are not any invalid characters in the tag.
      Tag = RegexForTag().Replace(Tag, "");

      return true;
    }
    catch
    {
      // This line is invalid, clear all values.
      Clear();
      return false;
    }
  }

  /// <summary>
  /// Reset all values.
  /// </summary>
  private void Clear()
  {
    Level = 0;
    Tag = "";
    Data = "";
  }
}
