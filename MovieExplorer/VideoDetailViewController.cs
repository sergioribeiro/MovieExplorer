using System;

using UIKit;
using Newtonsoft.Json;
using System.Net.Http;
using CoreGraphics;
using Foundation;
using System.Threading.Tasks;
using CoreImage;
using System.Drawing;

namespace MovieExplorer
{
	public partial class VideoDetailViewController : UIViewController
	{
		Movie movie;

		public VideoDetailViewController (string videoId) : base ("VideoDetailViewController", null)
		{
			movie = new Movie();
			movie.Id = videoId;
			View.BackgroundColor = UIColor.Gray;
			LoadVideoDetails ();
			AddSimilarMoviesSection ();
		}

		public static UIImage FromUrl (string uri)
		{
			using (var url = new NSUrl (uri))
			using (var data = NSData.FromUrl (url))
				if (data!=null) {
					return UIImage.LoadFromData (data);
				} else {
					return UIImage.FromBundle("0.jpg");
				}
		}

		private async Task LoadVideoDetails(){
			//UIImage img=FromUrl("https://image.tmdb.org/t/p/w500/"+video.posterPath);
			using (var client = new HttpClient())
			{
				//https://api.themoviedb.org/3/movie/47933?external_source=imdb_id&api_key=ab41356b33d100ec61e6c098ecc92140
				var result = await client.GetStringAsync("https://api.themoviedb.org/3/movie/" + movie.Id + "?external_source=imdb_id&api_key=ab41356b33d100ec61e6c098ecc92140");

				movie = JsonConvert.DeserializeObject<Movie>(result);

				var image = FromUrl ("https://image.tmdb.org/t/p/w500/" + movie.PosterPath);
				View.BackgroundColor = UIColor.FromPatternImage(image);
				//UILabel backgroundImgLayer = new UILabel (new CGRect(0,0,View.Bounds.Width,470));
				//UILabel backgroundImgLayer = new UILabel (View.Bounds);
				//backgroundImgLayer.BackgroundColor = UIColor.FromPatternImage(FromUrl("https://image.tmdb.org/t/p/w500/"+movie.PosterPath));
				//View.AddSubview (backgroundImgLayer);

				UILabel backgroundLayer = new UILabel (new CGRect(0,0,View.Bounds.Width,470));
				backgroundLayer.BackgroundColor = UIColor.FromWhiteAlpha (.5f, .9f);
				View.AddSubview (backgroundLayer);

				UIImageView imgMovie = new UIImageView (new CGRect(10,80,150,225));
				imgMovie.Image = image;
				imgMovie.Layer.BorderColor = new CGColor(255,255,255);
				imgMovie.Layer.BorderWidth = 1f;
				View.AddSubview (imgMovie);

				AddLabel (movie.Name, 16, 2, 170, 80, (int)View.Bounds.Width-180, 60, UIColor.Clear);
				AddLabel ("Release Date: " + movie.ReleaseDate.ToShortDateString(), 12, 1, 170, 140, (int)View.Bounds.Width-180, 30, UIColor.Clear);
				AddLabel ("Votes Average: " + movie.VoteAverage.ToString(), 12, 1, 170, 170, (int)View.Bounds.Width-180, 30, UIColor.Clear);
				AddLabel ("(from " + movie.VoteCount.ToString() + " votes)", 12, 1, 170, 190, (int)View.Bounds.Width-180, 30, UIColor.Clear);

				AddLabel ("Play Video", 16, 1, 170, 235, (int)View.Bounds.Width-180, 30, UIColor.FromRGB(163,202,23));
				AddLabel ("Save to Favorites", 16, 1, 170, 275, (int)View.Bounds.Width-180, 30, UIColor.FromRGB(218,177,0));

				AddLabel (movie.Overview, 14, 10, 10, 310, (int)View.Bounds.Width-20, 160, UIColor.Clear);
			}
		}

		private void AddLabel(string text, int size, int lines, int x, int y, int width, int height, UIColor backgroundColor){
			UILabel titleLabel = new UILabel {
				BackgroundColor = backgroundColor,
				Frame = new CoreGraphics.CGRect (x,y,width,height),
				TextAlignment = UITextAlignment.Left,
				LineBreakMode = UILineBreakMode.WordWrap,
				Lines = lines,
				TextColor = UIColor.White,
				Font = UIFont.BoldSystemFontOfSize(size),
				Text = text
			};

			View.AddSubview (titleLabel);
		}

		private void AddSimilarMoviesSection(){
			UILabel backgroundLayer = new UILabel (new CGRect(0,470,View.Bounds.Width,View.Bounds.Height));
			backgroundLayer.BackgroundColor = UIColor.FromWhiteAlpha (.5f, .9f);
			View.AddSubview (backgroundLayer);

			AddSectionHeader("Similar Movies",10,470,400,30);
			AddSection (495,"http://api.themoviedb.org/3/movie/" + movie.Id + "/similar?api_key=ab41356b33d100ec61e6c098ecc92140");
		}

		private void AddSectionHeader(string title, int x, int y, int width, int height){
			UILabel titleLabel = new UILabel {
				Frame = new CoreGraphics.CGRect (x,y,width,height),
				TextAlignment = UITextAlignment.Left,
				TextColor = UIColor.White,
				Font = UIFont.BoldSystemFontOfSize(16),
				Text = title
			};

			View.AddSubview (titleLabel);
		}

		private void AddSection(int y, string url){
			var layout = new UICollectionViewFlowLayout (){
				EstimatedItemSize = new CGSize (100, 150),
				ScrollDirection = UICollectionViewScrollDirection.Horizontal
			};

			var collection = new MovieExplorerViewController (layout,url);
			this.AddChildViewController (collection);
			collection.View.Frame = new CGRect (10, y, 580, 98);
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


