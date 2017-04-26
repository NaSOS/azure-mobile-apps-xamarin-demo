using System.Threading.Tasks;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
	public partial class ActivitiesTabbedPage : TabbedPage
	{
		public ActivitiesTabbedPage()
		{
			InitializeComponent();

			Title = Messages.AppName;
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(Children.Count!=0) return;
            RefreshAsync();
        }

		async Task RefreshAsync()
		{
		    using (UserDialogs.Instance.Loading())
		    {
		    	var vm = this.GetViewModel<ActivitiesTabbedViewModel>();
                await vm.LoadDataAsync();
		    
		    	Children.Clear();
		    	if (vm.Collection.Count > 0)
		    	{
		    		foreach (var page in vm.Collection)
		    		{
		    			Children.Add(page);
		    		}
		    	}
		    	else
		    	{
		    		Children.Add(new ActivitiesPage());
		    	}
		    
		                 // add rest
		                 Children.Add(new MyActivitiesPage());
		    }
		}

        async void UpdatesItem_Clicked(object sender, System.EventArgs e)
        {
            await NavigationHelper.NavigateToEventUpdatesPage(Navigation);
        }
    }
}
