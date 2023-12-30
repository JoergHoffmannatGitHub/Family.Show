using System;
using System.Linq;

using Microsoft.VisualBasic.ApplicationServices;

namespace Microsoft.FamilyShow
{
  /// <summary>
  /// Method based on Single Instance Detection Sample http://msdn.microsoft.com/en-us/library/ms771662.aspx
  ///  Family.Show must be single instance to prevent temporary files being overwritten.
  ///
  /// Using VB bits to detect single instances and process accordingly:
  /// * OnStartup is fired when the first instance loads
  /// * OnStartupNextInstance is fired when the application is re-run again
  /// 
  /// NOTE: it is redirected to this instance thanks to IsSingleInstance 
  /// </summary>
  internal class SingleInstanceManager : WindowsFormsApplicationBase
  {
    private App _app;

    public SingleInstanceManager()
    {
      IsSingleInstance = true;
    }

    protected override bool OnStartup(StartupEventArgs eventArgs)
    {
      // First time app is launched.
      _app = new App();
      _app.InitializeComponent();
      _app.ProcessArgs(eventArgs.CommandLine.ToArray());
      _app.Run();

      return false;
    }

    protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
    {
      // Subsequent launches.
      base.OnStartupNextInstance(eventArgs);

      _app.Activate();
      _app.ProcessArgs(eventArgs.CommandLine.ToArray());
    }

    [STAThread]
    static void Main(string[] args)
    {
      SingleInstanceManager manager = new();
      manager.Run(args);
    }
  }
}
