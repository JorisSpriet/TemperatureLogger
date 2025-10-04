using System.IO;
using NUnit.Framework;
using TemperatuurLogger.Model;

[SetUpFixture]
public class TestsSetup
{
    private static readonly List<string> testDirs = new List<string>();

    internal static void ResetTestDir()
    {
        var td = Path.Combine(TestContext.CurrentContext.TestDirectory, "Testrun", DateTime.Now.ToString("yyyyMMddTHHmmss"));
        testDirs.Add(td);
        Utils.TestDir = td;
    }

    [OneTimeSetUp]
    public void SetUp()
    {
        Utils.IsTest = true;
        ResetTestDir();        
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        foreach (var dir in testDirs)
            try {  Directory.Delete(dir, true); } catch { }
    }
}
