using System.Globalization;

namespace FamilyShowLib.Tests;

public class GedcomExportTest
{
  [Theory]
  [InlineData(2019, 6, 21, "21 JUN 2019")]
  [InlineData(1982, 1, 9, "9 JAN 1982")]
  public void ExportDate(int year, int month, int day, string expected)
  {
    DateTime date = new(year, month, day);
    string result = GedcomExport.ExportDateWrapper(date);
    Assert.Equal(expected, result);
    Assert.Equal(date, DateTime.Parse(result, CultureInfo.InvariantCulture));
  }
}

public class GedcomIdMapTest
{
  [Fact]
  public void Get()
  {
    GedcomIdMap sut = new();
    string personId = Guid.NewGuid().ToString();
    Assert.Equal("I0", sut.Get(Guid.NewGuid().ToString()));
    Assert.Equal("I1", sut.Get(Guid.NewGuid().ToString()));
    Assert.Equal("I2", sut.Get(Guid.NewGuid().ToString()));
    Assert.Equal("I3", sut.Get(personId));
    Assert.Equal("I4", sut.Get(Guid.NewGuid().ToString()));
    Assert.Equal("I5", sut.Get(Guid.NewGuid().ToString()));
    Assert.Equal("I3", sut.Get(personId));
  }
}

public class FamilyTest
{
  [Fact]
  public void Family()
  {
    Family sut = new(new(), new())!;

    Assert.NotNull(sut);
  }
}
