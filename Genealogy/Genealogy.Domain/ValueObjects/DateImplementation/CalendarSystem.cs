// based on NodaTime.CalendarSystem (see: https://github.com/nodatime/nodatime)

using System;
using System.Collections.Generic;

namespace Genealogy.Domain.ValueObjects.DateImplementation;

/// <summary>
/// A calendar system maps the non-calendar-specific "local time line" to human concepts
/// such as years, months and days.
/// </summary>
/// <remarks>
/// <para>
/// Many developers will never need to touch this class, other than to potentially ask a calendar
/// how many days are in a particular year/month and the like. Genealogy defaults to using the Gregorian
/// calendar anywhere that a calendar system is required but hasn't been explicitly specified.
/// </para>
/// <para>
/// If you need to obtain a <see cref="CalendarSystem" /> instance, use one of the static properties or methods in this
/// class, such as the <see cref="Gregorian" /> property.
/// </para>
/// </remarks>
/// <threadsafety>
/// All calendar implementations are immutable and thread-safe.
/// </threadsafety>
public sealed class CalendarSystem
{
    private const string GregorianId = "Gregorian";
    private const string JulianId = "Julian";
    private const string HebrewId = "Hebrew";
    private const string FrenchRepublicanId = "French R";
    private const string RomanId = "Roman";
    private const string UnknownId = "Unknown";

    /// <summary>
    /// Abbreviated month names for the Gregorian and Julian calendars.
    /// </summary>
    private static readonly string[] s_months =
        ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    /// <summary>
    /// Abbreviated month names for the French Republican calendar.
    /// </summary>
    private static readonly string[] s_monthsFrench =
        ["Vend", "Brum", "Frim", "Nivo", "Pluv", "Vent", "Germ", "Flor", "Prai", "Mess", "Ther", "Fruc", "Comp"];

    /// <summary>
    /// Abbreviated month names for the Hebrew calendar.
    /// </summary>
    private static readonly string[] s_monthsHebr =
        ["Tsh", "Csh", "Ksl", "Tvt", "Shv", "Adr", "Ads", "Nsn", "Iyr", "Svn", "Tmz", "Aav", "Ell"];

    #region Public factory members for calendars

    /// <summary>
    /// Fetches a calendar system by its unique identifier. This provides full round-tripping of a calendar
    /// system. This method will always return the same reference for the same ID.
    /// </summary>
    /// <param name="id">The ID of the calendar system. This is case-sensitive.</param>
    /// <returns>The calendar system with the given ID.</returns>
    /// <seealso cref="Id"/>
    /// <exception cref="KeyNotFoundException">No calendar system for the specified ID can be found.</exception>
    /// <exception cref="NotSupportedException">The calendar system with the specified ID is known, but not supported on this platform.</exception>
    internal static CalendarSystem ForId(string id)
    {
        if (!s_idToFactoryMap.TryGetValue(id, out Func<CalendarSystem> factory))
        {
            throw new KeyNotFoundException($"No calendar system for ID {id} exists");
        }
        return factory();
    }

    /// <summary>
    /// Fetches a calendar system by its ordinal value, constructing it if necessary.
    /// </summary>
    internal static CalendarSystem ForOrdinal(CalendarOrdinal ordinal)
    {
        CalendarSystem result = ordinal switch
        {
            CalendarOrdinal.Gregorian => Gregorian,
            CalendarOrdinal.Julian => Julian,
            CalendarOrdinal.Hebrew => Hebrew,
            CalendarOrdinal.FrenchRepublican => FrenchRepublican,
            CalendarOrdinal.Roman => Roman,
            CalendarOrdinal.Unknown => Unknown,
            _ => throw new InvalidOperationException(
                $"Bug in Genealogy.CalendarSystem: calendar ordinal {ordinal} " +
                "missing from switch in CalendarSystem.ForOrdinal."),
        };
        return result;
    }

    /// <summary>
    /// Returns the IDs of all calendar systems available within Noda Time. The order of the keys is not guaranteed.
    /// </summary>
    /// <value>The IDs of all calendar systems available within Noda Time.</value>
    public static IEnumerable<string> Ids => s_idToFactoryMap.Keys;

    /// <summary>
    /// Returns the abbreviated month names for the specified calendar ordinal.
    /// </summary>
    /// <param name="ordinal">The calendar ordinal value.</param>
    /// <returns>An array of abbreviated month names for the calendar system.</returns>
    internal static string[] MonthNames(CalendarOrdinal ordinal) => ordinal switch
    {
        CalendarOrdinal.Gregorian or CalendarOrdinal.Julian => s_months,
        CalendarOrdinal.FrenchRepublican => s_monthsFrench,
        CalendarOrdinal.Hebrew => s_monthsHebr,
        _ => [],
    };

    // Note: each factory method must return the same reference on every invocation.
    // If the delegate calls a method, that method must have the same guarantee.
    private static readonly Dictionary<string, Func<CalendarSystem>> s_idToFactoryMap = new()
    {
        {GregorianId, () => Gregorian},
        {JulianId, () => Julian},
        {HebrewId, () => Hebrew},
        {FrenchRepublicanId, () => FrenchRepublican},
        {RomanId, () => Roman},
        {UnknownId, () => Unknown},
    };

    #endregion

    private CalendarSystem(CalendarOrdinal ordinal, string id)
    {
        Ordinal = ordinal;
        Id = id;
    }

    /// <summary>
    /// Returns the unique identifier for this calendar system. This is provides full round-trip capability
    /// using <see cref="ForId" /> to retrieve the calendar system from the identifier.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A unique ID for a calendar is required when serializing types which include a <see cref="CalendarSystem"/>.
    /// As of 2 Nov 2012 (ISO calendar) there are no ISO or RFC standards for naming a calendar system. As such,
    /// the identifiers provided here are specific to Noda Time, and are not guaranteed to interoperate with any other
    /// date and time API.
    /// </para>
    /// <list type="table">
    ///   <listheader>
    ///     <term>Calendar ID</term>
    ///     <description>Equivalent factory method or property</description>
    ///   </listheader>
    ///   <item><term>Gregorian</term><description><see cref="Gregorian"/></description></item>
    ///   <item><term>Julian</term><description><see cref="Julian"/></description></item>
    ///   <item><term>Hebrew</term><description><see cref="Hebrew"/></description></item>
    ///   <item><term>French Republican</term><description><see cref="FrenchRepublican"/></description></item>
    ///   <item><term>Roman</term><description><see cref="Roman"/></description></item>
    ///   <item><term>Unknown</term><description><see cref="Unknown"/></description></item>
    /// </list>
    /// <para>
    /// The ID "Persian Algorithmic" for the Persian Astronomical calendar is an unfortunate error. The ID has been
    /// incorrect in Noda Time for so long that "fixing" it now would cause compatibility issues between systems
    /// storing or exchanging Noda Time data.
    /// </para>
    /// </remarks>
    /// <value>The unique identifier for this calendar system.</value>
    public string Id { get; }

    /// <summary>
    /// Returns the ordinal value of this calendar.
    /// </summary>
    internal CalendarOrdinal Ordinal { get; }

    /// <summary>
    /// Determines the date interpretation by signifying which <see cref="CalendarSystem" /> to use.
    /// </summary>
    internal string Escape { get => $"@#D{Id.ToUpper()}@"; }

    /// <summary>
    /// Returns a Gregorian calendar system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Gregorian calendar system defines every
    /// fourth year as leap, unless the year is divisible by 100 and not by 400.
    /// This improves upon the Julian calendar leap year rule.
    /// </para>
    /// <para>
    /// Although the Gregorian calendar did not exist before 1582 CE, this
    /// calendar system assumes it did, thus it is proleptic. This implementation also
    /// fixes the start of the year at January 1.
    /// </para>
    /// </remarks>
    /// <value>A Gregorian calendar system.</value>
    public static CalendarSystem Gregorian => GregorianJulianCalendars.Gregorian;

    /// <summary>
    /// Returns a pure proleptic Julian calendar system, which defines every
    /// fourth year as a leap year. This implementation follows the leap year rule
    /// strictly, even for dates before 8 CE, where leap years were actually
    /// irregular.
    /// </summary>
    /// <remarks>
    /// Although the Julian calendar did not exist before 45 BCE, this calendar
    /// assumes it did, thus it is proleptic. This implementation also fixes the
    /// start of the year at January 1.
    /// </remarks>
    /// <value>A suitable Julian calendar reference; the same reference may be returned by several
    /// calls as the object is immutable and thread-safe.</value>
    public static CalendarSystem Julian => GregorianJulianCalendars.Julian;

    /// <summary>
    /// Returns a Hebrew calendar system using the civil month numbering,
    /// equivalent to the one used by the BCL HebrewCalendar.
    /// The numbering system where month 1 is Tishri.
    /// </summary>
    /// <value>A Hebrew calendar system using the civil month numbering, equivalent to the one used by the
    /// BCL.</value>
    public static CalendarSystem Hebrew => HebrewCalendars.Hebrew;

    /// <summary>
    /// Returns a French Republican calendar system.
    /// </summary>
    /// <value>A French Republican calendar system.</value>
    public static CalendarSystem FrenchRepublican => FrenchCalendars.FrenchRepublican;

    /// <summary>
    /// Returns a Roman calendar system.
    /// </summary>
    /// <remarks>
    /// This is a placeholder for future definition of a calendar system and
    /// does not implement validation or parsing.
    /// </remarks>
    /// <value>A Roman calendar system.</value>
    public static CalendarSystem Roman => MiscellaneousCalendars.Roman;

    /// <summary>
    /// Returns a Unknown calendar system.
    /// </summary>
    /// <remarks>
    /// This is a placeholder for a unknown calendar system and does not
    /// implement validation or parsing.
    /// </remarks>
    /// <value>A Unknown calendar system.</value>
    public static CalendarSystem Unknown => MiscellaneousCalendars.Unknown;

    private static class GregorianJulianCalendars
    {
        internal static CalendarSystem Gregorian { get; }
        internal static CalendarSystem Julian { get; }

        static GregorianJulianCalendars()
        {
            Julian = new CalendarSystem(CalendarOrdinal.Julian, JulianId);
            Gregorian = new CalendarSystem(CalendarOrdinal.Gregorian, GregorianId);
        }
    }

    private static class HebrewCalendars
    {
        internal static CalendarSystem Hebrew { get; }

        static HebrewCalendars()
        {
            Hebrew = new CalendarSystem(CalendarOrdinal.Hebrew, HebrewId);
        }
    }

    private static class FrenchCalendars
    {
        internal static CalendarSystem FrenchRepublican { get; }

        static FrenchCalendars()
        {
            FrenchRepublican = new CalendarSystem(CalendarOrdinal.FrenchRepublican, FrenchRepublicanId);
        }
    }

    private static class MiscellaneousCalendars
    {
        internal static CalendarSystem Roman { get; } =
          new CalendarSystem(CalendarOrdinal.Roman, RomanId);
        internal static CalendarSystem Unknown { get; } =
          new CalendarSystem(CalendarOrdinal.Unknown, UnknownId);

        // Static constructor to enforce laziness. This used to be important to avoid a Heisenbug.
        static MiscellaneousCalendars()
        {
        }
    }
}
