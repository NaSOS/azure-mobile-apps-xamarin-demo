using System.Threading.Tasks;
using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
	public static class NavigationHelper
	{
		public static Page MainPage()
		{
			return new ActivitiesTabbedPage().WithinNavigationPage();
		}

		public static async Task NavigateToActivityPage(INavigation navigation, Activity activity){
			await navigation.PushAsync(new ActivityPage(activity));
		}

		public static async Task NavigateToEventUpdatesPage(INavigation navigation)
		{
            await navigation.PushAsync(new EventUpdatesPage());
		}

	}
}
