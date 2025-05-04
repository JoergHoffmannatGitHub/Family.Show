// based on NodaTime.CalendarOrdinal (see: https://github.com/nodatime/nodatime)

namespace Genealogy.DateImplementation
{
  /// <summary>
  /// Enumeration of calendar ordinal values. Used for converting between a compact integer representation and a calendar system.
  /// </summary>
  internal enum CalendarOrdinal
  {
    Gregorian = 0,
    Julian = 1,
    Hebrew = 2,
    FrenchRepublican = 3,
    Roman = 4, // for future definition
    Unknown = 5, // calendar not known
    // Not a real ordinal; just present to keep a count. Increase this as the number increases...
    Size = 6
  }
}
