using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = NavigationHelper.MainPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
            MobileCenter.Start($"android={Constants.MobileCenterAndroidKey};ios={Constants.MobileCenterIosKey}", typeof(Analytics), typeof(Crashes));
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
