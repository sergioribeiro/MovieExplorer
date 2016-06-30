using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Queries;

namespace MovieExplorer.UITests
{
	[TestFixture]
	public class Tests
	{
		iOSApp app;

		[SetUp]
		public void BeforeEachTest ()
		{
			app = ConfigureApp.iOS.StartApp ();
		}

		[Test]
		public void SwipeLeftOpensNextPage ()
		{
			app.SwipeLeft ();
			AppResult[] results = app.WaitForElement (c => c.Marked ("February"));
			app.Screenshot ("Next page is opened.");

			Assert.IsTrue (results.Any ());
		}
	}
}


