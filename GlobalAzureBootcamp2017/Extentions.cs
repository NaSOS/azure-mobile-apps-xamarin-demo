using System;

using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
	public static class Extentions
	{
		#region Pages
		public static NavigationPage WithinNavigationPage(this Page page)
		{
			var nav = new NavigationPage(page)
			{
				BarBackgroundColor = Colors.PrimaryColor,
				BarTextColor = Colors.PrimaryLightTextColor
			};
			return nav;
		}

		public static T GetViewModel<T>(this Page page) where T : BaseViewModel, new()
		{			
			var vm = page.BindingContext as T;
			if (vm != null) return vm;
			vm = new T();
			page.BindingContext = vm;
			return vm;
		}
		#endregion

		public static UriType GetUriType(this Uri uri)
		{
			switch (uri.Host)
			{
				case "www.facebook.com": return UriType.Facebook;
				case "twitter.com": return UriType.Twitter;
				case "linkedin.com": return UriType.LinkedIn;
				case "github.com": return UriType.Github;
				default: return UriType.Website;
			}
		}
	}
}

