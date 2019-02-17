using ATALogger;
using NUnit.Framework;

namespace ATALoggerFileLibTests
{
	[TestFixture]
    public class Class1
    {
		[Test]
	    public void Test1()
	    {
		    var s = GetType().Assembly.GetManifestResourceStream(GetType(), "data.logp");
		    var l = FileLogReader.Read(s);

			Assert.AreEqual(1528,l.Items.Length);
	    }
    }
}
