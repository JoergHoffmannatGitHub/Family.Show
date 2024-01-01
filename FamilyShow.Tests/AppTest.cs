using System.Globalization;

namespace FamilyShow.Tests;

public class AppTest
{
  public static readonly IEnumerable<object[]> StringToDateCases =
  [
    ["26061965", new CultureInfo("en-GB"), new DateTime(1, 1, 1)],
    ["08/05/1950", new CultureInfo("en-GB"), new DateTime(1950, 5, 8)],
    ["08/05/1950", new CultureInfo("en-US"), new DateTime(1950, 8, 5)],
    ["", new CultureInfo("en-GB"), new DateTime(1, 1, 1)],
    ["08-05-1950", new CultureInfo("en-GB"), new DateTime(1950, 5, 8)],
    ["08-05-1950", new CultureInfo("en-US"), new DateTime(1950, 8, 5)],
    ["18-05-1950", new CultureInfo("en-GB"), new DateTime(1950, 5, 18)],
    ["18-05-1950", new CultureInfo("en-US"), new DateTime(1, 1, 1)],
    ["01-01-0001", new CultureInfo("en-GB"), new DateTime(1, 1, 1)],
  ];

  [Theory, MemberData(nameof(StringToDateCases))]
  public void StringToDate(string source, CultureInfo cultureInfo, DateTime expected)
  {
    // Arrange
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    // Act
    DateTime result = App.StringToDate(source);
    // Assert
    Assert.Equal(expected, result);
  }

  public static readonly object[][] DateToStringCases =
  [
    [null!, new CultureInfo("en-GB"), string.Empty],
    [new DateTime(1950, 5, 8), new CultureInfo("en-US"), "5/8/1950"],
    [new DateTime(1950, 5, 8), new CultureInfo("en-GB"), "08/05/1950"],
    [new DateTime(1950, 5, 8), new CultureInfo("es-ES"), "8/5/1950"],
    [new DateTime(1950, 5, 8), new CultureInfo("de-DE"), "08.05.1950"],
    [new DateTime(1950, 5, 8), new CultureInfo("it-IT"), "08/05/1950"],
  ];

  [Theory, MemberData(nameof(DateToStringCases))]
  public void DateToString(DateTime? date, CultureInfo cultureInfo, string expected)
  {
    // Arrange
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    // Act
    string result = App.DateToString(date);
    // Assert
    Assert.Equal(expected, result);
  }
}
