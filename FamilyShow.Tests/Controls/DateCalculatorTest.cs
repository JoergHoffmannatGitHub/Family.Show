namespace FamilyShow.Tests.Controls;

public class DateCalculatorTest
{
  public static readonly TheoryData<string, string, string, string, string, string, string> CalculateButtonCases =
    new()
    {
      { "en-US", "22 JUL 1890", "22 JAN 1995", string.Empty, "7/22/1890", "1/22/1995", "105 years" },
      { "de-DE", "22 JUL 1890", "22 JAN 1995", string.Empty, "22.07.1890", "22.01.1995", "105 Jahre" },
      { "en-GB", "22 JUL 1890", "22 JAN 1995", string.Empty, "22/07/1890", "22/01/1995", "105 years" },
      { "es-ES", "22 JUL 1890", "22 JAN 1995", string.Empty, "22/7/1890", "22/1/1995", "105 años" },
      { "fr-FR", "22 JUL 1890", "22 JAN 1995", string.Empty, "22/07/1890", "22/01/1995", "105 années" },
      { "it-IT", "22 JUL 1890", "22 JAN 1995", string.Empty, "22/07/1890", "22/01/1995", "105 anni" },
      { "en-GB", "22 JUL 1890", string.Empty, "105", "22/07/1890", "22/07/1995", "105 years" },
      { "de-DE", string.Empty, "22 JAN 1995", "105", "22.01.1890", "22.01.1995", "105 Jahre" },
      { "en-US", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty },
      { "en-US", string.Empty, string.Empty, "105", string.Empty, string.Empty, string.Empty },
      { "en-US", string.Empty, "22 JAN 1995", string.Empty, string.Empty, string.Empty, string.Empty },
      { "en-US", "22 JUL 1890", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty },
      { "en-US", "22 JAN 1995", "22 JUL 1890", string.Empty, string.Empty, string.Empty, string.Empty },
      { "en-US", "22 JUL 1890", "22 JAN 1995", "105", string.Empty, string.Empty, string.Empty },
      { "en-US", "invalid", "22 JAN 1995", string.Empty, string.Empty, string.Empty, string.Empty },
      { "en-US", "22 JUL 1890", "invalid", string.Empty, string.Empty, string.Empty, string.Empty },
      { "en-US", "invalid", string.Empty, "105", string.Empty, string.Empty, string.Empty },
      { "en-US", "22 JUL 1890", string.Empty, "invalid", string.Empty, string.Empty, string.Empty },
      { "en-US", string.Empty, "invalid", "105", string.Empty, string.Empty, string.Empty },
      { "en-US", string.Empty, "22 JAN 1995", "invalid", string.Empty, string.Empty, string.Empty },
    };

  [StaTheory, MemberData(nameof(CalculateButtonCases))]
  public void CalculateButton_ClickTest(string culture, string date1, string date2, string age, string birthResult, string deathResult, string ageResult)
  {
    using (new AnotherCulture(culture))
    {
      // Arrange
      DateCalculator calculator = new();
      calculator.Date1TextBox.Text = date1;
      calculator.Date2TextBox.Text = date2;
      calculator.AgeTextBox.Text = age;
      // Act
      calculator.CalculateButton_Click(null, null);
      // Assert
      Assert.Equal(birthResult, calculator.BirthResult.Content);
      Assert.Equal(deathResult, calculator.DeathResult.Content);
      Assert.Equal(ageResult, calculator.AgeResult.Content);
    }
  }

  public static readonly TheoryData<string, string, bool, string, string, string, string> Calculate2ButtonCases =
    new()
    {
      { "en-US", "22 JUL 1890", true, "1", "1", "1", "8/23/1891" },
      { "en-US", "22 JUL 1890", false, "1", "1", "1", "6/21/1889" },
      { "en-US", "22 JUL 1890", true, "300", "45", "17", "2/18/1912" },
      { "en-US", "22 JUL 1890", false, "300", "45", "17", "12/25/1868" },
      { "en-US", "invalid", true, "1", "1", "1", string.Empty },
      { "en-US", "invalid", false, "1", "1", "1", string.Empty },
      { "en-US", "invalid", true, "300", "45", "17", string.Empty },
      { "en-US", "invalid", false, "300", "45", "17", string.Empty },
      { "en-US", "22 JUL 1890", true, "invalid", "1", "1", "8/22/1891" },
      { "en-US", "22 JUL 1890", false, "invalid", "1", "1", "6/22/1889" },
      { "en-US", "22 JUL 1890", true, "1", "invalid", "1", "7/23/1891" },
      { "en-US", "22 JUL 1890", false, "1", "invalid", "1", "7/21/1889" },
      { "en-US", "22 JUL 1890", true, "1", "1", "invalid", "8/23/1890" },
      { "en-US", "22 JUL 1890", false, "1", "1", "invalid", "6/21/1890" },
    };

  [StaTheory, MemberData(nameof(Calculate2ButtonCases))]
  public void Calculate2Button_ClickTest(string culture, string date, bool add, string days, string months, string years, string result)
  {
    using (new AnotherCulture(culture))
    {
      // Arrange
      DateCalculator calculator = new();
      calculator.ToBox.Text = date;
      calculator.DayBox.Text = days;
      calculator.MonthBox.Text = months;
      calculator.YearBox.Text = years;
      calculator.AddSubtractComboBox.SelectedIndex = add ? 0 : 1;
      // Act
      calculator.Calculate2Button_Click(null, null);
      // Assert
      Assert.Equal(result, calculator.Result2.Content);
    }
  }
}
