namespace FamilyShowLib.Tests;

public class GedcomConverterTest
{
    [Theory]
    [InlineData("", false, 0, "", "")]
    [InlineData("0 HEAD", true, 0, "HEAD", "")]
    [InlineData("1 SOUR EasyTree", true, 1, "SOUR", "EasyTree")]
    [InlineData("0 @I1@ INDI", true, 0, "INDI", "@I1@")]
    [InlineData("7 ADR1 Müllerstr.", true, 7, "ADR1", "Müllerstr.")]
    [InlineData("2 _ADPF Adopted by father", true, 2, "_ADPF", "Adopted by father")]
    public void ParseLine(string text, bool expectedResult, int level, string tag, string data)
    {
        var gedcomLine = new GedcomLine();
        bool result = gedcomLine.Parse(text, true);
        Assert.Equal(expectedResult, result);
        Assert.Equal(level, gedcomLine.Level);
        Assert.Equal(tag, gedcomLine.Tag);
        Assert.Equal(data, gedcomLine.Data);
    }

    [Fact]
    public void ParseLineRegexToClean()
    {
        string text = "1 SOUR EasyTree";
        var expectedGedcomLine = new GedcomLine();
        bool expectedResult = expectedGedcomLine.Parse(text, true);
        for (int i = char.MinValue; i < 32; i++)
        {
            text += Convert.ToChar(i);
        }
        text += Convert.ToChar(127);
        var gedcomLine = new GedcomLine();
        bool result = gedcomLine.Parse(text, false);
        Assert.Equal(expectedResult, result);
        Assert.Equal(expectedGedcomLine.Level, gedcomLine.Level);
        Assert.Equal(expectedGedcomLine.Tag, gedcomLine.Tag);
        Assert.Equal(expectedGedcomLine.Data, gedcomLine.Data);
    }
}
