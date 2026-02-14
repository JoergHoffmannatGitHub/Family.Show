using FamilyShowLib;

namespace FamilyShow.Tests.Controls.Diagram;

public class DiagramLogicTest
{
    // I have not found a way to set up DiagramLogic so that I can fill
    // MinimumYear.get with values for PersonLookup and Connections. Until then,
    // MinimumYear.get has to be tested on the complete system by loading
    // different data sets. This ensures that each path is run through at least
    // once.
    // The following data records provide the following result:
    // Windsor.familyx	1841
    // Kennedy.ged		  1821

    public static readonly TheoryData<int, int, int, double> DateMinimumYearCases =
      new()
      {
      { 0, 0, 0, double.MaxValue },
      { 1950, 5, 8, 1950 },
      };

    [Theory, MemberData(nameof(DateMinimumYearCases))]
    public void GetMinimumYearFromDateTest(int year, int month, int day, double expected)
    {
        // Arrange
        double minimumYear = double.MaxValue;
        DateWrapper? date = (year == 0 ? null : new DateWrapper(year, month, day));

        // Act
        minimumYear = DiagramLogic.GetMinimumYearFromDate(minimumYear, date);

        // Assert
        Assert.Equal(expected, minimumYear);
    }
}
