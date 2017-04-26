using System;
using Microsoft.WindowsAzure.MobileServices;

namespace GlobalAzureBootcamp2017
{
	public class AuthenticationResult
	{
		public bool IsSuccess { get; set; }

		public MobileServiceUser User { get; set; }

		public string ErrorMessage { get; set; }
	}
}
