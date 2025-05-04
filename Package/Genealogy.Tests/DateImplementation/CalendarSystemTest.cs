// based on NodaTime.Test.CalendarSystemTest (see: https://github.com/nodatime/nodatime)

using System.Globalization;

using Genealogy.DateImplementation;

namespace Genealogy.Tests.DateImplementation;

public class CalendarSystemTest
{
  public static readonly TheoryData<string> SupportedIds = [.. CalendarSystem.Ids];

  [Theory, MemberData(nameof(SupportedIds))]
  public void CalendarSystem_Id_ShouldReturnSameInstance(string id)
  {
    CalendarSystem calendar1 = CalendarSystem.ForId(id);
    CalendarSystem calendar2 = CalendarSystem.ForId(id);
    Assert.Same(calendar1, calendar2);
  }

  [Theory, MemberData(nameof(SupportedIds))]
  public void CalendarSystem_ForId_ShouldReturnCorrectCalendarSystem_ForValidId(string id)
  {
    // Act & Assert
    Assert.IsType<CalendarSystem>(CalendarSystem.ForId(id));
  }

  [Theory, MemberData(nameof(SupportedIds))]
  public void CalendarSystem_ForId_ShouldThrowKeyNotFoundException_ForCaseInsitiveId(string id)
  {
    // Act & Assert
    Assert.Throws<KeyNotFoundException>(() => CalendarSystem.ForId(id.ToLowerInvariant()));
  }

  [Fact]
  public void CalendarSystem_ForId_ShouldReturnOnlyOneCalenderSytemForEachIdGivenCalendar()
  {
    // Arrange
    IEnumerable<string> supportedIds = [.. CalendarSystem.Ids];

    // Act
    IEnumerable<CalendarSystem> allCalendars = supportedIds.Select(CalendarSystem.ForId);

    // Assert
    Assert.Equal(supportedIds.Count(), allCalendars.Distinct().Count());
  }

  [Fact]
  public void CalendarSystem_ForId_ShouldThrowKeyNotFoundException_ForInvalidId()
  {
    // Act & Assert
    Assert.Throws<KeyNotFoundException>(() => CalendarSystem.ForId("InvalidId"));
  }

  [Fact]
  public void CalendarSystem_Ids_NoSubstrings()
  {
    // Arrange
    CompareInfo comparison = CultureInfo.InvariantCulture.CompareInfo;

    // Act
    foreach (string firstId in CalendarSystem.Ids)
    {
      foreach (string secondId in CalendarSystem.Ids)
      {
        // We're looking for firstId being a substring of secondId, which can only
        // happen if firstId is shorter...
        if (firstId.Length >= secondId.Length)
        {
          continue;
        }

        // Assert
        Assert.NotEqual(0, comparison.Compare(firstId, 0, firstId.Length, secondId, 0, firstId.Length, CompareOptions.IgnoreCase));
      }
    }
  }

  [Theory, MemberData(nameof(SupportedIds))]
  public void CalendarSystem_ForOrdinal_ShouldReturnCorrectCalendarSystem_ForValidOrdinal(string id)
  {
    // Arrange
    CalendarSystem calendar = CalendarSystem.ForId(id);

    // Act & Assert
    Assert.Equal(calendar, CalendarSystem.ForOrdinal(calendar.Ordinal));
  }

  [Fact]
  public void CalendarSystem_ForOrdinal_ShouldThrowInvalidOperationException_ForInvalidOrdinal()
  {
    // Act & Assert
    Assert.Throws<InvalidOperationException>(() => CalendarSystem.ForOrdinal((CalendarOrdinal)9999));
  }
}
