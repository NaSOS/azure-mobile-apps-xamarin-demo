using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Gcm.Client;
using ImageCircle.Forms.Plugin.Droid;
using Microsoft.WindowsAzure.MobileServices;

namespace GlobalAzureBootcamp2017.Droid
{
	[Activity(Label = "GlobalAzureBootcamp2017", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			CurrentPlatform.Init();
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(savedInstanceState);

			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

			// init external frameworks
			ImageCircleRenderer.Init();
			UserDialogs.Init(this);

			// load authenticator
			AuthenticationHelper.Instance.Init(this);

			LoadApplication(new App());

			// register for push
			RegisterForNotifications();
		}

		#region Authentication
		public async Task<AuthenticationResult> AuthenticateAsync()
		{
			MobileServiceUser user = null;
			string errorMessage = null;
			try
			{
				// Sign in with using a server-managed flow.
				user = await AzureService.Instance.Client.LoginAsync(this, MobileServiceAuthenticationProvider.MicrosoftAccount);
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			return new AuthenticationResult
			{
				User = user,
				IsSuccess = user != null,
				ErrorMessage = errorMessage
			};
		}
		#endregion

		#region PushNotifications
		private void RegisterForNotifications()
		{
			try
			{
				// Check to ensure everything's set up right
				GcmClient.CheckDevice(this);
				GcmClient.CheckManifest(this);

				//Call to Register the device for Push Notifications
				Log.Info("PushNotifications", "Registering...");
				GcmClient.Register(this, GcmBroadcastReceiver.SENDER_IDS);
			}
			catch (Exception e)
			{
				Log.Error("PushNotifications", "RegisterForNotifications: " + e);
			}
		}
		#endregion
	}
}
