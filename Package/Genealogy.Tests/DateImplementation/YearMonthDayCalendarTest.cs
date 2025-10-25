// based on NodaTime.Test.YearMonthDayCalendarTest (see: https://github.com/nodatime/nodatime)

using Genealogy.DateImplementation;

namespace Genealogy.Tests.DateImplementation;

public class YearMonthDayCalendarTest
{
  [Theory]
  [InlineData(2023, 12, 31)]
  [InlineData(1, 1, 1)]
  [InlineData(9999, 12, 31)]
  public void Constructor_ShouldStoreYearMonthDayCalendarCorrectly(int year, int month, int day)
  {
    // Act
    YearMonthDayCalendar ymdc = new(year, month, day, 0);

    // Assert
    Assert.Equal(year, ymdc.Year);
    Assert.Equal(month, ymdc.Month);
    Assert.Equal(day, ymdc.Day);
    Assert.Equal(CalendarOrdinal.Gregorian, ymdc.CalendarOrdinal);
  }

  [Fact]
  public void YearMonthDayCalendar_ShouldHandleAllYears()
  {
    // Range of years we actually care about. We support more, but that's okay.
    for (int year = -9999; year <= 9999; year++)
    {
      // Act
      YearMonthDayCalendar ymdc = new(year, 5, 20, CalendarOrdinal.Julian);

      // Assert
      Assert.Equal(year, ymdc.Year);
      Assert.Equal(5, ymdc.Month);
      Assert.Equal(20, ymdc.Day);
      Assert.Equal(CalendarOrdinal.Julian, ymdc.CalendarOrdinal);
    }
  }

  [Fact]
  public void YearMonthDayCalendar_ShouldHandleAllMonths()
  {
    // We'll never actually need 32 months, but we support that many...
    int monthCount = (int)Math.Pow(2, YearMonthDayCalendar.MonthBits);
    for (int month = 0; month < monthCount; month++)
    {
      // Act
      YearMonthDayCalendar ymdc = new(-123, month, 20, CalendarOrdinal.Hebrew);

      // Assert
      Assert.Equal(-123, ymdc.Year);
      Assert.Equal(month, ymdc.Month);
      Assert.Equal(20, ymdc.Day);
      Assert.Equal(CalendarOrdinal.Hebrew, ymdc.CalendarOrdinal);
    }
  }

  [Fact]
  public void YearMonthDayCalendar_ShouldHandleAllDays()
  {
    // We'll never actually need 64 days, but we support that many...
    int dayCount = (int)Math.Pow(2, YearMonthDayCalendar.DayBits);
    for (int day = 0; day < dayCount; day++)
    {
      // Act
      YearMonthDayCalendar ymdc = new(-123, 30, day, CalendarOrdinal.FrenchRepublican);

      // Assert
      Assert.Equal(-123, ymdc.Year);
      Assert.Equal(30, ymdc.Month);
      Assert.Equal(day, ymdc.Day);
      Assert.Equal(CalendarOrdinal.FrenchRepublican, ymdc.CalendarOrdinal);
    }
  }

  [Fact]
  public void YearMonthDayCalendar_ShouldHandleAllCalendars()
  {
    int calendarCount = (int)Math.Pow(2, YearMonthDayCalendar.CalendarBits);
    for (int ordinal = 0; ordinal < calendarCount; ordinal++)
    {
      // Arrange
      CalendarOrdinal calendar = (CalendarOrdinal)ordinal;

      // Act
      YearMonthDayCalendar ymdc = new(-123, 30, 63, calendar);

      // Assert
      Assert.Equal(-123, ymdc.Year);
      Assert.Equal(30, ymdc.Month);
      Assert.Equal(63, ymdc.Day);
      Assert.Equal(calendar, ymdc.CalendarOrdinal);
    }
  }

  [Fact]
  public void YearMonthDayCalendar_ShouldHandleEqualityCorrectly()
  {
    // Arrange
    YearMonthDayCalendar ymdc = new(1000, 12, 20, CalendarOrdinal.FrenchRepublican);
    YearMonthDayCalendar[] unequalValues = [
      new(ymdc.Year + 1, ymdc.Month, ymdc.Day, ymdc.CalendarOrdinal),
      new(ymdc.Year, ymdc.Month + 1, ymdc.Day, ymdc.CalendarOrdinal),
      new(ymdc.Year, ymdc.Month, ymdc.Day + 1, ymdc.CalendarOrdinal),
      new(ymdc.Year, ymdc.Month, ymdc.Day, CalendarOrdinal.Gregorian)
    ];

    // Act & Assert
    TestHelper.TestOperatorComparisonEquality(ymdc, new(1000, 12, 20, CalendarOrdinal.FrenchRepublican), unequalValues);
    TestHelper.TestEqualsStruct(ymdc,
      new(1000, 12, 20, CalendarOrdinal.FrenchRepublican),
      unequalValues);
    TestHelper.TestOperatorEquality(ymdc, ymdc, new YearMonthDayCalendar(ymdc.Year + 1, ymdc.Month, ymdc.Day, ymdc.CalendarOrdinal));
  }
}
