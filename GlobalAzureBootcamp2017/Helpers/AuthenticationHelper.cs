using System.Diagnostics;
using System.Threading.Tasks;
using GlobalAzureBootcamp2017.Helpers;
using Microsoft.WindowsAzure.MobileServices;

namespace GlobalAzureBootcamp2017
{
	public interface IAuthenticate
	{
		Task<AuthenticationResult> AuthenticateAsync();
	}

	public class AuthenticationHelper
	{
		#region Singleton
		public static AuthenticationHelper Instance { get; } = new AuthenticationHelper();
		#endregion

		public bool IsAuthenticated
		{
			get
			{
				return User != null;
			}
		}

		public IAuthenticate Authenticator { get; private set; }

		public MobileServiceUser User { get; private set; }

		public void Init(IAuthenticate authenticator)
		{
			Authenticator = authenticator;

			// load session
			var userId = SettingsHelper.UserId;
			var userToken = SettingsHelper.UserToken;
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userToken)) return;
			User = new MobileServiceUser(userId)
			{
				MobileServiceAuthenticationToken = userToken
			};

			AzureService.Instance.Client.CurrentUser = User;
		}

		public async Task<bool> AuthenticateAsync()
		{
			if (Authenticator == null) return false;

			var result = await Authenticator.AuthenticateAsync();

			if (result.IsSuccess)
			{
				User = result.User;

				// cache
				SettingsHelper.UserId = User.UserId;
				SettingsHelper.UserToken = User.MobileServiceAuthenticationToken;

				Debug.WriteLine($"AuthenticationHelper.AuthenticateAsync({IsAuthenticated}): {result.User.UserId}, {result.User.MobileServiceAuthenticationToken}");
			}
			else
			{
				Debug.WriteLine($"AuthenticationHelper.AuthenticateAsync({IsAuthenticated}): {result.ErrorMessage}");
			}

			return IsAuthenticated;
		}
	}
}

