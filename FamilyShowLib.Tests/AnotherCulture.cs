using System.Globalization;

/// <summary>
/// Makes it easy to execute test code under a specific culture.
/// </summary>
/// <example>
/// using (AnotherCulture.UnitedStates())
/// {
///     // Execute code in United States's culture.
/// }
/// // Back to the original culture.
/// </example>
/// <author>Joshua Poehls</author>
internal sealed class AnotherCulture : IDisposable
{
  private bool _disposed;
  private readonly CultureInfo _originalCurrentCulture;
  private readonly CultureInfo _originalCurrentUICulture;
  private readonly CultureInfo? _originalDefaultThreadCurrentCulture;
  private readonly CultureInfo? _originalDefaultThreadCurrentUICulture;

  public AnotherCulture(string cultureName)
  {
    _originalCurrentCulture = Thread.CurrentThread.CurrentCulture;
    _originalCurrentUICulture = Thread.CurrentThread.CurrentUICulture;
    _originalDefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
    _originalDefaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture;

    var otherCulture = CultureInfo.CreateSpecificCulture(cultureName);

    // Change the current thread's culture.
    Thread.CurrentThread.CurrentCulture = otherCulture;
    Thread.CurrentThread.CurrentUICulture = otherCulture;

    // Change the default culture of any new threads created by the application domain.
    // These properties are only available as of .NET 4.5.
    CultureInfo.DefaultThreadCurrentCulture = otherCulture;
    CultureInfo.DefaultThreadCurrentUICulture = otherCulture;

    CultureInfo.CurrentCulture.ClearCachedData();
    CultureInfo.CurrentUICulture.ClearCachedData();
  }

  #region - IDisposable Implementation -

  public void Dispose()
  {
    Dispose(true);
    // This object will be cleaned up by the Dispose method.
    // Therefore, you should call GC.SupressFinalize to
    // take this object off the finalization queue
    // and prevent finalization code for this object
    // from executing a second time.
    GC.SuppressFinalize(this);
  }

  private void Dispose(bool disposing)
  {
    // Check to see if Dispose has already been called.
    if (!_disposed)
    {
      // If disposing equals true, dispose all managed
      // and unmanaged resources.
      if (disposing)
      {
        // Dispose managed resources.
        Thread.CurrentThread.CurrentCulture = _originalCurrentCulture;
        Thread.CurrentThread.CurrentUICulture = _originalCurrentUICulture;
        CultureInfo.DefaultThreadCurrentCulture = _originalDefaultThreadCurrentCulture;
        CultureInfo.DefaultThreadCurrentUICulture = _originalDefaultThreadCurrentUICulture;

        CultureInfo.CurrentCulture.ClearCachedData();
        CultureInfo.CurrentUICulture.ClearCachedData();
      }

      // Clean up unmanaged resources here.

      // Note disposing has been done.
      _disposed = true;
    }
  }

  ~AnotherCulture()
  {
    Dispose(false);
  }

  #endregion

  public static AnotherCulture UnitedStates()
  {
    return new AnotherCulture("en-US");
  }
}
