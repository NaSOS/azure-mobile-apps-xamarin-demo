using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;
using Android.Util;
using Gcm.Client;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]

namespace GlobalAzureBootcamp2017.Droid
{
	[Service] //Must use the service tag
	public class GcmService : GcmServiceBase
	{
		public GcmService() : base(GcmBroadcastReceiver.SENDER_IDS) { }

		protected async override void OnRegistered(Context context, string registrationId)
		{
			//Receive registration Id for sending GCM Push Notifications to
			try
			{
				const string templateBodyGCM = "{\"data\":{\"message\":\"$(messageParam)\"}}";

				JObject templates = new JObject();
				templates["genericMessage"] = new JObject 
				{
					{"body", templateBodyGCM}
				};

				var push = AzureService.Instance.Client.GetPush();
				await push.RegisterAsync(registrationId, templates);

				Log.Info("Push Installation Id", push.InstallationId);
			}
			catch (Exception e)
			{
				Log.Error("PushNotifications", "OnRegistered: " + e);
			}
		}

		protected override void OnUnRegistered(Context context, string registrationId)
		{
			Log.Error("PushHandlerBroadcastReceiver", "Unregistered RegisterationToken: " + registrationId);
		}

		protected override void OnMessage(Context context, Intent intent)
		{
			Log.Info("PushNotifications", "GCM Message Received!");

			var msg = new StringBuilder();
			if (intent != null && intent.Extras != null)
			{
				foreach (var key in intent.Extras.KeySet())
					msg.AppendLine(key + "=" + intent.Extras.Get(key));
			}

			//Push Notification arrived - print out the keys/values
			if (intent == null || intent.Extras == null)
				foreach (var key in intent.Extras.KeySet())
					Console.WriteLine("Key: {0}, Value: {1}");

			string message = intent.Extras.GetString("message");
			if (!string.IsNullOrEmpty(message))
			{
				CreateNotification(ApplicationInfo.LoadLabel(PackageManager), message);

                AzureService.Instance.EventRepository.SyncAsync();
			}
		}

		//protected override bool OnRecoverableError(Context context, string errorId)
		//{
		//	//Some recoverable error happened
		//}

		protected override void OnError(Context context, string errorId)
		{
			Log.Error("PushNotifications", "GCM Error: " + errorId);
		}

		void CreateNotification(string title, string msg)
		{
			//Create notification
			var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

			//Create an intent to show ui
			var uiIntent = new Intent(this, typeof(MainActivity));

			//Use Notification Builder
			NotificationCompat.Builder builder = new NotificationCompat.Builder(this);

			//Create the notification
			//we use the pending intent, passing our ui intent over which will get called
			//when the notification is tapped.
			var notification = builder.SetContentIntent(PendingIntent.GetActivity(this, 0, uiIntent, 0))
			                          .SetSmallIcon(Resource.Drawable.icon_notification)
					.SetTicker(title)
					.SetContentTitle(title)
					.SetContentText(msg)

					//Set the notification sound
					.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))

					//Auto cancel will remove the notification once the user touches it
					.SetAutoCancel(true).Build();

			//Show the notification
			notificationManager.Notify(1, notification);
		}
	}
}
