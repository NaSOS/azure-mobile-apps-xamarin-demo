using System;
using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
	public static class Colors
	{
		public static Color PrimaryColor => (Color)Application.Current.Resources["PrimaryColor"];
		public static Color PrimaryDarkColor => (Color)Application.Current.Resources["PrimaryDarkColor"];
		public static Color PrimaryLightTextColor => (Color)Application.Current.Resources["PrimaryLightTextColor"];
	}
}
