using Acr.UserDialogs;
using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
    public partial class MyActivitiesPage : ContentPage
    {
        bool? loginResult;

        public MyActivitiesPage()
        {
            InitializeComponent();

            Title = Messages.MySchedule;
        }

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			// show login alert
			if (loginResult.HasValue)
			{
				UserDialogs.Instance.Toast(loginResult.Value ? Messages.Welcome : Messages.SomethingWentWrong);

				// reset
				loginResult = null;
			}

            await this.GetViewModel<MyActivitiesViewModel>().LoadDataAsync();
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

        async void LoginButton_Clicked(object sender, System.EventArgs e)
        {
            loginResult = await AuthenticationHelper.Instance.AuthenticateAsync();
        }
    }
}
