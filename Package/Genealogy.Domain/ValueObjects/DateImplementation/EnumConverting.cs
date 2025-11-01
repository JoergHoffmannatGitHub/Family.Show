using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Genealogy.Domain.ValueObjects.DateImplementation;

public class EnumConverting
{
    /// <summary>
    /// Gets the GEDCOM string value for the specified enum value.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="enumValue">The enum value.</param>
    /// <returns>The GEDCOM string value.</returns>
    public static string GetEnumValue<T>(T enumValue) where T : Enum
    {
        Type type = enumValue.GetType();
        MemberInfo[] memInfo = type.GetMember(enumValue.ToString());
        object[] attributes = memInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);
        return ((EnumMemberAttribute)attributes[0]).Value;
    }

    /// <summary>
    /// Gets the enum value from the GEDCOM string prefix.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="value">The GEDCOM string prefix.</param>
    /// <returns>The enum value.</returns>
    /// <exception cref="NotImplementedException">Thrown if the prefix is not recognized.</exception>
    public static T GetEnumFromString<T>(string value) where T : Enum
    {
        Type type = typeof(T);
        foreach (FieldInfo field in type.GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute)) is EnumMemberAttribute attribute &&
              attribute.Value.Trim().Equals(value, StringComparison.CurrentCultureIgnoreCase))
            {
                return (T)field.GetValue(null);
            }
        }

        throw new ArgumentException($"Unknown value: {value}");
    }
}
