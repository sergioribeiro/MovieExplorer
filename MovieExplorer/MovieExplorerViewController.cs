// Copyright Sergio Schirmer Almenara Ribeiro
// http://github.com/sergioribeiro
// sergio@sisnet.com.br

using System;

using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using Newtonsoft.Json;
using System.Net.Http;

namespace MovieExplorer
{
	public class MovieJson
	{
		[JsonProperty("results")]
		public List<Movie> Movies { get; set; }
	}

	public interface IVideo
	{
		string Id { get; }
		string Name { get; }
		string PosterPath{ get; }
		float VoteAverage{ get;}
		int VoteCount{ get;}
		DateTime ReleaseDate{ get;}
		string Overview{ get;}
	}

	public class Movie : IVideo
	{
		public Movie ()
		{
		}

		[JsonProperty("id")]
		public string Id{ get; set; }

		[JsonProperty("title")]
		public string Name { get; set;}

		[JsonProperty("poster_path")]
		public string PosterPath{ get; set;}

		[JsonProperty("vote_average")]
		public float VoteAverage{ get; set;}

		[JsonProperty("vote_count")]
		public int VoteCount{ get; set;}

		[JsonProperty("release_date")]
		public DateTime ReleaseDate { get; set;}

		[JsonProperty("overview")]
		public string Overview{ get; set;}

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

	}

	public partial class MovieExplorerViewController : UICollectionViewController
	{
		static NSString videoCellId = new NSString ("VideoCell");
		List<IVideo> videos;
		List<Movie> movies;
		string apiUrl;

		public MovieExplorerViewController (UICollectionViewLayout layout,string url) : base (layout)
		{
			apiUrl = url;

			videos = new List<IVideo> ();
			movies = new List<Movie> ();		
		}

		async void LoadVideos(string url){
			using (var client = new HttpClient())
			{
				var result = await client.GetStringAsync(url);
				movies = JsonConvert.DeserializeObject<MovieJson>(result).Movies;
				videos.AddRange (movies);
			}

			CollectionView.ReloadData ();
		}
			
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			AddPullToRefresh ();

			LoadVideos (apiUrl);

			CollectionView.RegisterClassForCell (typeof(VideoCell), videoCellId);
			CollectionView.AlwaysBounceHorizontal = true;
			CollectionView.BackgroundColor = UIColor.Clear;
		}

		void AddPullToRefresh(){
			var refreshControl = new UIRefreshControl ();
			refreshControl.ValueChanged += HandleValueChanged;
			refreshControl.AddTarget (Self,new Selector("startRefresh"),UIControlEvent.ValueChanged);
			refreshControl.TintColor = UIColor.Clear;


			var loadingView = new UIView (refreshControl.Bounds);
			loadingView.BackgroundColor = UIColor.Clear;
			var backgroundImage = new UIImageView (UIImage.FromBundle ("loading.png"));
			loadingView.AddSubview (backgroundImage);
			loadingView.ClipsToBounds = true;

			var refreshColorView = new UIView (refreshControl.Bounds);
			refreshColorView.BackgroundColor = UIColor.Clear;
			refreshColorView.Alpha = 0.3f;

			refreshControl.AddSubview (loadingView);
			refreshControl.AddSubview (refreshColorView);

			CollectionView.AddSubview (refreshControl);
		}

		void HandleValueChanged (object sender, EventArgs e)
		{
			Console.WriteLine ("Refresh data from server");
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
		}
			
		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return videos.Count;
			//return movies.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var videoCell = (VideoCell)collectionView.DequeueReusableCell (videoCellId, indexPath);

			//var video = videos [indexPath.Row];
			var video = movies [indexPath.Row];

			UIImage img=FromUrl("https://image.tmdb.org/t/p/w500/"+video.PosterPath);
			videoCell.Image = img;
			videoCell.Id = video.Id;

			//videoCell.Image = video.Image;

			return videoCell;
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

		public override void ItemHighlighted (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (VideoCell)collectionView.CellForItem (indexPath);
			cell.ContentView.BackgroundColor = UIColor.Yellow;

			NavigationController.PushViewController(new VideoDetailViewController(cell.Id), true);
		}

		public override void ItemUnhighlighted (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem (indexPath);
			cell.ContentView.BackgroundColor = UIColor.White;
		}

		public override bool ShouldHighlightItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

		public override bool ShouldShowMenu (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

		public override bool CanPerformAction (UICollectionView collectionView, Selector action, NSIndexPath indexPath, NSObject sender)
		{
			// Selector should be the same as what's in the custom UIMenuItem
			if (action == new Selector ("custom"))
				return true;
			else
				return false;
		}

		public override void PerformAction (UICollectionView collectionView, Selector action, NSIndexPath indexPath, NSObject sender)
		{
			System.Diagnostics.Debug.WriteLine ("code to perform action");
		}

		// CanBecomeFirstResponder and CanPerform are needed for a custom menu item to appear
		public override bool CanBecomeFirstResponder {
			get {
				return true;
			}
		}
	}

	public class VideoCell : UICollectionViewCell
	{
		UIImageView imageView;
		string id;

		[Export ("initWithFrame:")]
		public VideoCell (CGRect frame) : base (frame)
		{
			BackgroundView = new UIView{BackgroundColor = UIColor.Orange};

			SelectedBackgroundView = new UIView{BackgroundColor = UIColor.Green};

			ContentView.Layer.BorderColor = UIColor.White.CGColor;
			ContentView.Layer.BorderWidth = 1.0f;
			ContentView.BackgroundColor = UIColor.White;

			imageView = new UIImageView (UIImage.FromBundle ("0.jpg"));

			ContentView.AddSubview (imageView);
		}

		public string Id {
			set {
				id = value;
			}
			get{
				return id;
			}
		}

		public UIImage Image {
			set {
				imageView.Image = value;
			}
		}

		[Export("custom")]
		void Custom()
		{
			// Put all your custom menu behavior code here
			Console.WriteLine ("custom in the cell");
		}


		public override bool CanPerform (Selector action, NSObject withSender)
		{
			if (action == new Selector ("custom"))
				return true;
			else
				return false;
		}
	}
}

