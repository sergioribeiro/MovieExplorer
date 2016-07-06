// Copyright Sergio Schirmer Almenara Ribeiro
// http://github.com/sergioribeiro
// sergio@sisnet.com.br

using UIKit;

namespace MovieExplorer
{
	public partial class HomeViewController : UIViewController
	{
		public HomeViewController () : base ("HomeViewController", null)
		{
			View.BackgroundColor = UIColor.Gray;

			Title = "Movie Explorer";
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes {
				TextColor = UIColor.FromRGB(221,156,46)
			});

			AddTopRatedSection ();
			AddPopularSection ();
			AddNowPlayingSection ();
		}

		void AddTopRatedSection(){
			AddSectionHeader("Top Rated",10,90,400,30);
			AddSection (120,"http://api.themoviedb.org/3/movie/top_rated?api_key=ab41356b33d100ec61e6c098ecc92140&sort_by=popularity.des");
		}

		void AddPopularSection(){
			AddSectionHeader("Popular",10,285,400,30);
			AddSection (310,"http://api.themoviedb.org/3/movie/popular?api_key=ab41356b33d100ec61e6c098ecc92140&sort_by=popularity.des");
		}

		void AddNowPlayingSection(){
			AddSectionHeader("Now Playing",10,470,400,30);
			AddSection (495,"http://api.themoviedb.org/3/movie/now_playing?api_key=ab41356b33d100ec61e6c098ecc92140&sort_by=popularity.des");

		}

		void AddSectionHeader(string title, int x, int y, int width, int height){
			var titleLabel = new UILabel {
				Frame = new CoreGraphics.CGRect (x,y,width,height),
				TextAlignment = UITextAlignment.Left,
				TextColor = UIColor.White,
				Font = UIFont.BoldSystemFontOfSize(18),
				Text = title
			};

			View.AddSubview (titleLabel);
		}

		void AddSection(int y, string url){
			var layout = new UICollectionViewFlowLayout{
				EstimatedItemSize = new CoreGraphics.CGSize (100, 150),
				ScrollDirection = UICollectionViewScrollDirection.Horizontal
			};

			var collection = new MovieExplorerViewController (layout,url);
			AddChildViewController (collection);
			collection.View.Frame = new CoreGraphics.CGRect (10, y, 580, 98);
			View.AddSubview (collection.View);
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
	}
}


