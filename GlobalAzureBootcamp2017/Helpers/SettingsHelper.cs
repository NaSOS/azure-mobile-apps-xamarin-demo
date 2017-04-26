using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace GlobalAzureBootcamp2017.Helpers
{
	public static class SettingsHelper
	{
		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		#region Setting Constants

		private const string UserIdKey = "userId";
		private const string UserTokenKey = "userTokenKey";

		#endregion

		public static string UserId
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(UserIdKey);
			}
			set
			{
				AppSettings.AddOrUpdateValue<string>(UserIdKey, value);
			}
		}

		public static string UserToken
		{
			get
			{
				return AppSettings.GetValueOrDefault<string>(UserTokenKey);
			}
			set
			{
				AppSettings.AddOrUpdateValue<string>(UserTokenKey, value);
			}
		}

	}
}