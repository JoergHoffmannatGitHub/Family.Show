using System;
using System.Diagnostics.CodeAnalysis;

namespace FamilyShowLib;

public static class Extensions
{
    #region DateTime

    /// <summary>
    /// Converts the value of the current <see cref="DateTime"/> object to its
    /// equivalent short date string representation.
    /// </summary>
    /// <param name="date">The date to convert.</param>
    /// <returns>A string that contains the short date string representation of
    /// the current <see cref="DateTime"/> object.</returns>
    public static string ToShortString(this DateTime date)
    {
        return date.ToShortDateString();
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a short string. If <see cref="DateTime"/>
    /// is <see langword="null"/>, returns an empty string.
    /// </summary>
    /// <param name="date">The date to convert.</param>
    /// <returns>
    /// A string that contains the short date string representation of the current <see cref="DateTime"/> object.
    /// If <see cref="DateTime"/> is <see langword="null"/>, returns an empty string.
    /// </returns>
    public static string ToShortString(this DateTime? date)
    {
        return date == null ? string.Empty : date.Value.ToShortString();
    }

    /// <summary>
    /// Indicates whether the specified <see cref="DateTime"/> is <see langword="null"/>
    /// or an empty <see cref="DateTime"/> (<see cref="DateTime.MinValue"/>).
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="date"/> parameter is
    /// <see langword="null"/> or an empty <see cref="DateTime"/> (DateTime.MinValue);
    /// otherwise, <see langword="false"/>.</returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this DateTime? date)
    {
        return date == null || date == DateTime.MinValue || date == DateTime.MaxValue;
    }

    #endregion

    #region string

    /// <summary>
    /// Converts string to date time object using <see cref="DateTime.TryParse"/>.  
    /// Also accepts just the year for dates. 1977 = 1/1/1977.
    /// </summary>
    /// <param name="dateString">The string to convert.</param>
    /// <returns><see langword="null"/> if the <paramref name="dateString"/>
    /// parameter is <see langword="null"/> or an empty <see cref="DateTime"/>
    /// (DateTime.MinValue); otherwise, <see langword="false"/>.</returns>
    public static DateTime? ToDate(this string dateString)
    {
        // Append first month and day if just the year was entered.
        if (dateString.Length == 4)
        {
            dateString = "1/1/" + dateString;
        }

        if (DateTime.TryParse(dateString, out DateTime date))
        {
            return date;
        }

        return null;
    }

    #endregion

    #region Person

    /// <summary>
    /// Converts string to date and set the person's birth date.
    /// </summary>
    /// <param name="person">The person to set the birth date for.</param>
    /// <param name="dateString">The string to convert to a date.</param>
    public static void SetBirthDate(this Person person, string dateString)
    {
        DateWrapper birthdate = new(dateString);
        if (!DateWrapper.IsNullOrEmpty(birthdate))
        {
            person.BirthDate = birthdate;
        }
    }

    #endregion
}
