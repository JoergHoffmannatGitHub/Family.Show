// based on NodaTime.Test.TestHelper (see: https://github.com/nodatime/nodatime)

namespace Genealogy.Domain.Tests.ValueObjects.DateImplementation;

public static class TestHelper
{
  /// <summary>
  /// Tests the IEquatable.Equals method for value objects. Also tests the
  /// object equals method.
  /// </summary>
  /// <typeparam name="T">The type to test.</typeparam>
  /// <param name="value">The base value.</param>
  /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
  /// <param name="unequalValue">The value not equal to the base value.</param>
  internal static void TestEqualsStruct<T>(T value, T equalValue, params T[] unequalValues) where T : struct, IEquatable<T>
  {
    object[] unequalArray = [.. unequalValues.Cast<object>()];
    TestObjectEquals(value, equalValue, unequalArray);
    Assert.True(value.Equals(value), "value.Equals<T>(value)");
    Assert.True(value.Equals(equalValue), "value.Equals<T>(equalValue)");
    Assert.True(equalValue.Equals(value), "equalValue.Equals<T>(value)");
    foreach (T unequalValue in unequalValues)
    {
      Assert.False(value.Equals(unequalValue), "value.Equals<T>(unequalValue)");
    }
  }

  /// <summary>
  /// Tests the Object.Equals method.
  /// </summary>
  /// <remarks>
  /// It takes two equal values, and then an array of values which should not be equal to the first argument.
  /// </remarks>
  /// <param name="value">The base value.</param>
  /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
  /// <param name="unequalValue">The value not equal to the base value.</param>
  private static void TestObjectEquals(object value, object equalValue, params object[] unequalValues)
  {
    ValidateInput(value, equalValue, unequalValues, "unequalValue");
    Assert.False(value.Equals(null), "value.Equals(null)");
    Assert.True(value!.Equals(value), "value.Equals(value)");
    Assert.True(value.Equals(equalValue), "value.Equals(equalValue)");
    Assert.True(equalValue.Equals(value), "equalValue.Equals(value)");
    foreach (object unequalValue in unequalValues)
    {
      Assert.False(value.Equals(unequalValue), "value.Equals(unequalValue)");
    }
    Assert.True(value.GetHashCode() == value.GetHashCode(), "GetHashCode() twice for same object");
    Assert.True(value.GetHashCode() == equalValue.GetHashCode(), "GetHashCode() for two different but equal objects");
  }

  /// <summary>
  /// Tests the less than (&lt;) and greater than (&gt;) operators if they exist on the object.
  /// </summary>
  /// <typeparam name="T">The type to test.</typeparam>
  /// <param name="value">The base value.</param>
  /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
  /// <param name="greaterValue">The values greater than the base value, in ascending order.</param>
  private static void TestOperatorComparison<T>(T value, T equalValue, params T[] greaterValues)
      where T : struct
  {
    ValidateInput(value, equalValue, greaterValues, "greaterValue");
    Type type = typeof(T);
    System.Reflection.MethodInfo? greaterThan = type.GetMethod("op_GreaterThan", [type, type]);
    System.Reflection.MethodInfo? lessThan = type.GetMethod("op_LessThan", [type, type]);

    // Comparisons only involving equal values
    if (greaterThan != null)
    {
      if (!type.IsValueType)
      {
        Assert.True((bool)greaterThan.Invoke(null, [value, null])!, "value > null");
        Assert.False((bool)greaterThan.Invoke(null, [null, value])!, "null > value");
      }
      Assert.False((bool)greaterThan.Invoke(null, [value, value])!, "value > value");
      Assert.False((bool)greaterThan.Invoke(null, [value, equalValue])!, "value > equalValue");
      Assert.False((bool)greaterThan.Invoke(null, [equalValue, value])!, "equalValue > value");
    }

    if (lessThan != null)
    {
      if (!type.IsValueType)
      {
        Assert.False((bool)lessThan.Invoke(null, [value, null])!, "value < null");
        Assert.True((bool)lessThan.Invoke(null, [null, value])!, "null < value");
      }
      Assert.False((bool)lessThan.Invoke(null, [value, value])!, "value < value");
      Assert.False((bool)lessThan.Invoke(null, [value, equalValue])!, "value < equalValue");
      Assert.False((bool)lessThan.Invoke(null, [equalValue, value])!, "equalValue < value");
    }

    // Then comparisons involving the greater values
    foreach (T greaterValue in greaterValues)
    {
      if (greaterThan != null)
      {
        Assert.False((bool)greaterThan.Invoke(null, [value, greaterValue])!, "value > greaterValue");
        Assert.True((bool)greaterThan.Invoke(null, [greaterValue, value])!, "greaterValue > value");
      }
      if (lessThan != null)
      {
        Assert.True((bool)lessThan.Invoke(null, [value, greaterValue])!, "value < greaterValue");
        Assert.False((bool)lessThan.Invoke(null, [greaterValue, value])!, "greaterValue < value");
      }

      // Now move up to the next pair...
      value = greaterValue;
    }
  }

  /// <summary>
  /// Tests the equality (==), inequality (!=), less than (&lt;), greater than (&gt;), less than or equals (&lt;=),
  /// and greater than or equals (&gt;=) operators if they exist on the object.
  /// </summary>
  /// <typeparam name="T">The type to test.</typeparam>
  /// <param name="value">The base value.</param>
  /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
  /// <param name="greaterValue">The values greater than the base value, in ascending order.</param>
  internal static void TestOperatorComparisonEquality<T>(T value, T equalValue, params T[] greaterValues)
      where T : struct
  {
    foreach (T greaterValue in greaterValues)
    {
      TestOperatorEquality(value, equalValue, greaterValue);
    }

    TestOperatorComparison(value, equalValue, greaterValues);
    Type type = typeof(T);
    System.Reflection.MethodInfo? greaterThanOrEqual = type.GetMethod("op_GreaterThanOrEqual", [type, type]);
    System.Reflection.MethodInfo? lessThanOrEqual = type.GetMethod("op_LessThanOrEqual", [type, type]);

    // First the comparisons with equal values
    if (greaterThanOrEqual != null)
    {
      if (!type.IsValueType)
      {
        Assert.True((bool)greaterThanOrEqual.Invoke(null, [value, null])!, "value >= null");
        Assert.False((bool)greaterThanOrEqual.Invoke(null, [null, value])!, "null >= value");
      }

      Assert.True((bool)greaterThanOrEqual.Invoke(null, [value, value])!, "value >= value");
      Assert.True((bool)greaterThanOrEqual.Invoke(null, [value, equalValue])!, "value >= equalValue");
      Assert.True((bool)greaterThanOrEqual.Invoke(null, [equalValue, value])!, "equalValue >= value");
    }
    if (lessThanOrEqual != null)
    {
      if (!type.IsValueType)
      {
        Assert.False((bool)lessThanOrEqual.Invoke(null, [value, null])!, "value <= null");
        Assert.True((bool)lessThanOrEqual.Invoke(null, [null, value])!, "null <= value");
      }

      Assert.True((bool)lessThanOrEqual.Invoke(null, [value, value])!, "value <= value");
      Assert.True((bool)lessThanOrEqual.Invoke(null, [value, equalValue])!, "value <= equalValue");
      Assert.True((bool)lessThanOrEqual.Invoke(null, [equalValue, value])!, "equalValue <= value");
    }
  }

  /// <summary>
  /// Tests the equality and inequality operators (==, !=) if they exist on the object.
  /// </summary>
  /// <typeparam name="T">The type to test.</typeparam>
  /// <param name="value">The base value.</param>
  /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
  /// <param name="unequalValue">The value not equal to the base value.</param>
  internal static void TestOperatorEquality<T>(T value, T equalValue, T unequalValue) where T : struct
  {
    ValidateInput(value, equalValue, unequalValue, "unequalValue");
    Type type = typeof(T);
    System.Reflection.MethodInfo? equality = type.GetMethod("op_Equality", [type, type]);
    if (equality != null)
    {
      if (!type.IsValueType)
      {
        Assert.True((bool)equality.Invoke(null, [null, null])!, "null == null");
        Assert.False((bool)equality.Invoke(null, [value, null])!, "value == null");
        Assert.False((bool)equality.Invoke(null, [null, value])!, "null == value");
      }

      Assert.True((bool)equality.Invoke(null, [value, value])!, "value == value");
      Assert.True((bool)equality.Invoke(null, [value, equalValue])!, "value == equalValue");
      Assert.True((bool)equality.Invoke(null, [equalValue, value])!, "equalValue == value");
      Assert.False((bool)equality.Invoke(null, [value, unequalValue])!, "value == unequalValue");
    }

    System.Reflection.MethodInfo? inequality = type.GetMethod("op_Inequality", [type, type]);
    if (inequality != null)
    {
      if (!type.IsValueType)
      {
        Assert.False((bool)inequality.Invoke(null, [null, null])!, "null != null");
        Assert.True((bool)inequality.Invoke(null, [value, null])!, "value != null");
        Assert.True((bool)inequality.Invoke(null, [null, value])!, "null != value");
      }

      Assert.False((bool)inequality.Invoke(null, [value, value])!, "value != value");
      Assert.False((bool)inequality.Invoke(null, [value, equalValue])!, "value != equalValue");
      Assert.False((bool)inequality.Invoke(null, [equalValue, value])!, "equalValue != value");
      Assert.True((bool)inequality.Invoke(null, [value, unequalValue])!, "value != unequalValue");
    }
  }

  /// <summary>
  /// Validates that the input parameters to the test methods are valid.
  /// </summary>
  /// <param name="value">The base value.</param>
  /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
  /// <param name="unequalValues">The values not equal to the base value.</param>
  /// <param name="unequalName">The name to use in "not equal value" error messages.</param>
  private static void ValidateInput(object value, object equalValue, object[] unequalValues, string unequalName)
  {
    Assert.True(value != null, "value cannot be null in TestObjectEquals() method");
    Assert.True(equalValue != null, "equalValue cannot be null in TestObjectEquals() method");
    Assert.True(value != equalValue, "value and equalValue MUST be different objects");
    foreach (object unequalValue in unequalValues)
    {
      Assert.True(unequalValue != null, unequalName + " cannot be null in TestObjectEquals() method");
      Assert.True(value != unequalValue, unequalName + " and value MUST be different objects");
    }
  }

  /// <summary>
  /// Validates that the input parameters to the test methods are valid.
  /// </summary>
  /// <param name="value">The base value.</param>
  /// <param name="equalValue">The value equal to but not the same object as the base value.</param>
  /// <param name="unequalValue">The value not equal to the base value.</param>
  /// <param name="unequalName">The name to use in "not equal value" error messages.</param>
  private static void ValidateInput(object value, object equalValue, object unequalValue, string unequalName)
  {
    ValidateInput(value, equalValue, [unequalValue], unequalName);
  }
}
