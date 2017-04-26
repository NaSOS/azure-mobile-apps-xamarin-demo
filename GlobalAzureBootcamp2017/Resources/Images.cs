using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
	public static class Images
	{
		static readonly string ResourcesPath = "GlobalAzureBootcamp2017.Resources";

		public static readonly string Building = "building.png";
		public static readonly string User = "user.png";
		public static readonly string Coffee = "coffee.png";
		public static readonly string Cutlery = "cutlery.png";
		public static readonly string Flask = "flask.png";
		public static readonly string Gift = "gift.png";
		public static readonly string Facebook = "facebook-f.png";
		public static readonly string Github = "github-alt.png";
		public static readonly string Linkedin = "linkedin.png";
		public static readonly string Twitter = "twitter.png";
		public static readonly string Globe = "globe.png";
		public static readonly string Bookmark = "bookmark_o.png";
		public static readonly string BookmarkFilled = "bookmark.png";
        public static readonly string Bullhorn = "bullhorn.png";

		public static ImageSource BuildingImageSource => ImageSource.FromResource($"{ResourcesPath}.{Building}");
		public static ImageSource UserImageSource => ImageSource.FromResource($"{ResourcesPath}.{User}");
		public static ImageSource CoffeeImageSource => ImageSource.FromResource($"{ResourcesPath}.{Coffee}");
		public static ImageSource CutleryImageSource => ImageSource.FromResource($"{ResourcesPath}.{Cutlery}");
		public static ImageSource FlaskImageSource => ImageSource.FromResource($"{ResourcesPath}.{Flask}");
		public static ImageSource GiftImageSource => ImageSource.FromResource($"{ResourcesPath}.{Gift}");
		public static ImageSource FacebookImageSource => ImageSource.FromResource($"{ResourcesPath}.{Facebook}");
		public static ImageSource GithubImageSource => ImageSource.FromResource($"{ResourcesPath}.{Github}");
		public static ImageSource LinkedInImageSource => ImageSource.FromResource($"{ResourcesPath}.{Linkedin}");
		public static ImageSource TwitterImageSource => ImageSource.FromResource($"{ResourcesPath}.{Twitter}");
		public static ImageSource GlobeImageSource => ImageSource.FromResource($"{ResourcesPath}.{Globe}");
		public static ImageSource BookmarkImageSource => ImageSource.FromResource($"{ResourcesPath}.{Bookmark}");
		public static ImageSource BookmarkFilledImageSource => ImageSource.FromResource($"{ResourcesPath}.{BookmarkFilled}");
        public static ImageSource BullhornImageSource => ImageSource.FromResource($"{ResourcesPath}.{Bullhorn}");
	}
}
