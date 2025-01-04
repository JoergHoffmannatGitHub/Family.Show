using System.Globalization;
using System.Reflection;

using Xunit.Sdk;

/// <summary>
/// Apply this attribute to your test method to replace the
/// <see cref="Thread.CurrentThread" /> <see cref="CultureInfo.CurrentCulture" /> and
/// <see cref="CultureInfo.CurrentUICulture" /> with another culture.
/// </summary>
/// <remarks>
/// Replaces the culture and UI culture of the current thread with
/// <paramref name="culture" />
/// </remarks>
/// <param name="culture">The name of the culture.</param>
/// <remarks>
/// <para>
/// This constructor overload uses <paramref name="culture" /> for both
/// <see cref="Culture" /> and <see cref="UICulture" />.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class UseCultureAttribute(string culture) : BeforeAfterTestAttribute
{
  private readonly string _culture = culture;
  private AnotherCulture? _anotherCulture;

  /// <summary>
  /// Stores the current <see cref="Thread.CurrentPrincipal" />
  /// <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" />
  /// and replaces them with the new cultures defined in the constructor.
  /// </summary>
  /// <param name="methodUnderTest">The method under test</param>
  public override void Before(MethodInfo methodUnderTest)
  {
    _anotherCulture = new AnotherCulture(_culture);

    CultureInfo.CurrentCulture.ClearCachedData();
    CultureInfo.CurrentUICulture.ClearCachedData();
  }

  /// <summary>
  /// Restores the original <see cref="CultureInfo.CurrentCulture" /> and
  /// <see cref="CultureInfo.CurrentUICulture" /> to <see cref="Thread.CurrentPrincipal" />
  /// </summary>
  /// <param name="methodUnderTest">The method under test</param>
  public override void After(MethodInfo methodUnderTest)
  {
    _anotherCulture!.Dispose();

    CultureInfo.CurrentCulture.ClearCachedData();
    CultureInfo.CurrentUICulture.ClearCachedData();
  }
}
