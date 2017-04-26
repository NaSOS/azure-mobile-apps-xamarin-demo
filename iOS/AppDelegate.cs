using Acr.UserDialogs;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using UIKit;

namespace GlobalAzureBootcamp2017.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
			global::Xamarin.Forms.Forms.Init();

			ImageCircleRenderer.Init();

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
