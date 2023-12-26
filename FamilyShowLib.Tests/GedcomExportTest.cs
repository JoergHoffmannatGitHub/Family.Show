using System.Globalization;

using Microsoft.FamilyShowLib;

namespace FamilyShowLib.Tests;

public class GedcomExportTest
{
  [Theory]
  [InlineData(2019, 6, 21, "21 JUN 2019")]
  [InlineData(1982, 1, 9, "9 JAN 1982")]
  public void TestMethod_GEDCOM(int year, int month, int day, string expected)
  {
    DateTime date = new(year, month, day);
    string result = GedcomExport.ExportDateWrapper(date);
    Assert.Equal(expected, result);
    Assert.Equal(date, DateTime.Parse(result, CultureInfo.InvariantCulture));
  }
}
