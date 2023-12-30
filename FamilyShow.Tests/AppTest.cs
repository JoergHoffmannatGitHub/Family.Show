using FamilyShow;

namespace FamilyShow.Tests;

public class AppTest
{
  public static readonly object[][] StringToDateCases =
  [
    ["26061965", new DateTime(1, 1, 1)],
    ["08/05/1950", new DateTime(1950, 5, 8)],
    ["", new DateTime(1, 1, 1)],
    ["08-05-1950", new DateTime(1950, 5, 8)],
    ["01-01-0001", new DateTime(1, 1, 1)],
  ];

  [Theory, MemberData(nameof(StringToDateCases))]
  public void StringToDate(string source, DateTime expected)
  {
    DateTime result = App.StringToDate(source);
    Assert.Equal(result, expected);
  }

  public static readonly object[][] DateToStringCases =
  [
    [null!, string.Empty],
    [new DateTime(1950, 5, 8), "08.05.1950"],
  ];

  [Theory, MemberData(nameof(DateToStringCases))]
  public void DateToString(DateTime? date, string expected)
  {
    string result = App.DateToString(date);
    Assert.Equal(result, expected);
  }
}
