using System;
using ATALogger;
using NUnit.Framework;

namespace ATALoggerFileLibTests
{
	[TestFixture]
    public class FileLogReaderTests
    {
		[Test]
	    public void Test1()
	    {
		    var s = GetType().Assembly.GetManifestResourceStream(GetType(), "data.logp");
		    var l = FileLogReader.Read(s);

			Assert.AreEqual(1528,l.Items.Length);

		    var dt1 = DateTime.Parse("2017-08-07 19:17:33");
		    var dt2 = DateTime.Parse("2017-08-23 17:06:52");
			Assert.AreEqual(dt1,l.Items[0].TimeStamp());
			Assert.AreEqual(dt2,l.Items[1527].TimeStamp());
	    }
    }
}
