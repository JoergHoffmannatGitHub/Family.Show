using System;
using System.Text;

namespace FamilyShowLib;

/// <summary>
/// Provides methods for converting between the binary representation of a GEDCOM UID
/// (a 16-byte GUID) and its textual hexadecimal representation.
/// Also includes checksum calculation and verification.
/// </summary>
public static class GedcomUid  //GedcomUidConverter
{
  /// <summary>
  /// Converts a 16-byte binary UID to its text hexadecimal representation.
  /// </summary>
  /// <param name="binary">A 16-byte array representing the UID (GUID).</param>
  /// <param name="result">An output parameter containing the resulting string.</param>
  /// <param name="addChecksum">If true, a 4-character checksum is added to the string.</param>
  /// <returns>True if the UID was not empty (all zeros); otherwise, false.</returns>
  public static bool UidToString(byte[] binary, out string result, bool addChecksum = false)
  {
    if (binary == null || binary.Length != 16)
    {
      // throw new ArgumentException("The input array must contain 16 bytes.", nameof(binary));
      result = string.Empty;
      return false;
    }

    byte checkA = 0;
    byte checkB = 0;
    bool isEmpty = true;

    var sb = new StringBuilder(32 + (addChecksum ? 4 : 0)); // 32 hex chars + optional 4 checksum chars

    for (int i = 0; i < 16; i++)
    {
      byte currentByte = binary[i];
      if (currentByte != 0)
      {
        isEmpty = false;
      }

      // Checksum calculation
      checkA += currentByte;
      checkB += checkA;

      // Add a byte as a hexadecimal string (e.g. "0F")
      sb.Append(currentByte.ToString("X2"));
    }

    if (addChecksum)
    {
      sb.Append(checkA.ToString("X2"));
      sb.Append(checkB.ToString("X2"));
    }

    if (isEmpty)
    {
      result = string.Empty;
      return false;
    }

    result = sb.ToString();
    return true;
  }

  /// <summary>
  /// Converts the text hexadecimal representation of the UID to its binary 16-byte format.
  /// </summary>
  /// <param name="str">Input string containing the UID (32 or 36 hexadecimal characters).</param>
  /// <param name="binary">Output array of 16 bytes where the result will be written.</param>
  /// <returns>True if the string is valid and the checksum (if any) is correct, otherwise false.</returns>
  public static bool StringToUid(string str, byte[] binary)
  {
    if (str == null || binary == null || binary.Length != 16)
    {
      if (binary != null) Array.Clear(binary, 0, 16);
      return false;
    }

    int len = str.Length;
    if (len != 32 && len != 36)
    {
      Array.Clear(binary, 0, 16);
      return false;
    }

    byte checkA = 0;
    byte checkB = 0;
    int stringIndex = 0;

    try
    {
      for (int i = 0; i < 16; i++)
      {
        // Read two characters (one byte)
        string hexByte = str.Substring(stringIndex, 2);
        byte currentByte = byte.Parse(hexByte, System.Globalization.NumberStyles.HexNumber);
        binary[i] = currentByte;

        // Checksum calculation
        checkA += currentByte;
        checkB += checkA;
        stringIndex += 2;
      }

      // If there is a checksum in the string, we check it.
      if (len == 36)
      {
        byte checkVerifyA = byte.Parse(str.Substring(stringIndex, 2), System.Globalization.NumberStyles.HexNumber);
        byte checkVerifyB = byte.Parse(str.Substring(stringIndex + 2, 2), System.Globalization.NumberStyles.HexNumber);

        if (checkVerifyA != checkA || checkVerifyB != checkB)
        {
          Array.Clear(binary, 0, 16);
          return false;
        }
      }

      return true;
    }
    catch (FormatException)
    {
      // An error occurred while parsing the hex string (invalid characters)
      Array.Clear(binary, 0, 16);
      return false;
    }
  }

  /// <summary>
  /// Converts a System.Guid object directly to a string representation of a GEDCOM UID.
  /// </summary>
  /// <param name="guid">The Guid object to convert.</param>
  /// <param name="addChecksum">If true, a 4-character checksum is added to the string.</param>
  /// <returns>The GEDCOM UID string, or an empty string if the Guid was empty.</returns>
  public static string GuidToUid(Guid guid, bool addChecksum = true)
  {
    // Convert the Guid to its 16-byte representation
    byte[] binary = guid.ToByteArray();

    // Call the main method for conversion
    if (UidToString(binary, out string result, addChecksum))
    {
      return result;
    }

    // If UidToString returned false (for example, Guid was empty), return an empty string.
    return string.Empty;
  }



  /// <summary>
  /// Converts the string representation of a GEDCOM UID back to a System.Guid object.
  /// </summary>
  /// <param name="gedcomUid">The GEDCOM UID string (32 or 36 hexadecimal characters).</param>
  /// <param name="result">The output parameter containing the resulting Guid.</param>
  /// <returns>True if the string is valid and successfully converted; otherwise, false.</returns>
  public static bool UidToGuid(string gedcomUid, out Guid result)
  {
    // Create a buffer for 16 bytes
    byte[] binary = new byte[16];

    // Use the existing method to parse the string and verify the checksum
    if (StringToUid(gedcomUid, binary))
    {
      // If parsing was successful, create a Guid from the byte array
      result = new Guid(binary);
      return true;
    }
    else
    {
      // In case of an error (invalid string, incorrect checksum)
      //result = Guid.Empty;
      if (Guid.TryParse(gedcomUid, out  result))
      {
        return true;
      }
      else
      {
        return false;
      }

    }
  }

}
