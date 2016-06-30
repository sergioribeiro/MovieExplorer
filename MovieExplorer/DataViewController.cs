using System;

using UIKit;

namespace MovieExplorer
{
	public partial class DataViewController : UIViewController
	{
		public string DataObject {
			get;
			set;
		}

		public DataViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			dataLabel.Text = DataObject;
		}
	}
}

