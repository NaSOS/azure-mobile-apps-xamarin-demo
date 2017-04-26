using System.Threading.Tasks;
using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
	public partial class EventUpdatesPage : ContentPage
	{
        public EventUpdatesPage()
        {
			InitializeComponent();

			Title = Messages.Updates;
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await this.GetViewModel<EventUpdatesViewModel>().LoadDataAsync();
        }

        void EventUpdatesList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
			if (EventUpdatesList.SelectedItem == null) return;

			// disable list selection highlight
			EventUpdatesList.SelectedItem = null;
        }

    }
}
