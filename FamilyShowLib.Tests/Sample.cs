namespace FamilyShowLib.Tests;

internal static partial class Sample
{
    internal static string FullName(string fileName)
    {
        string location = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
        return Path.Combine(location, "Sample Files", fileName);
    }
}
