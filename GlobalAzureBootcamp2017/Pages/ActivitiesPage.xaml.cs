using Acr.UserDialogs;
using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
	public partial class ActivitiesPage : ContentPage
	{
		public ActivitiesPage()
		{
			InitializeComponent();
		}

		private async void ActivitiesList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (ActivitiesList.SelectedItem == null) return;

			var vm = ActivitiesList.SelectedItem as ActivityViewModel;

			// disable list selection highlight
			ActivitiesList.SelectedItem = null;

			if (vm == null) return;

			switch (vm.Activity.Type)
			{
				case ActivityType.Registration:
					UserDialogs.Instance.Toast(Messages.Welcome);
					break;
				case ActivityType.Talk:
					await NavigationHelper.NavigateToActivityPage(Navigation, vm.Activity);
					break;	
				case ActivityType.Break:
					UserDialogs.Instance.Toast(Messages.CoffeeTime);
					break;
				case ActivityType.Launch:
					UserDialogs.Instance.Toast(Messages.PizzaTime);
					break;
				case ActivityType.Workshop:
					UserDialogs.Instance.Toast(Messages.DoMagic);
					break;
				case ActivityType.Closing:
					UserDialogs.Instance.Toast(Messages.Farewell);
					break;
				case ActivityType.Other:
				default:
					UserDialogs.Instance.Toast(Messages.NothingToSeeHere);
					break;
			}
		}
	}
}
