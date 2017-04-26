using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
    public class EventUpdatesViewModel : BaseCollectionViewModel<EventUpdateViewModel>
    {
		public ICommand RefreshCommand
		{
			get
			{
				return new Command(async () => await LoadDataAsync(true));
			}
		}

		public async Task LoadDataAsync(bool forceRefresh = false)
		{
			IsBusy = true;
			IsEmptyMessageVisible = false;
			try
			{
				Collection.Clear();

                var updates = await AzureService.Instance.EventUpdateRepository.GetByEventIdAsync(Constants.EventId, forceRefresh);
				// force refresh data if nothing was returned
                if (!forceRefresh && (updates==null || updates.Count==0))
				{
                    updates = await AzureService.Instance.EventUpdateRepository.GetByEventIdAsync(Constants.EventId, true);
				}
				if (updates?.Count > 0)
				{
					foreach (var update in updates)
					{
                        Collection.Add(new EventUpdateViewModel{EventUpdate=update});
					}
					IsEmptyMessageVisible = false;
				}
				else
				{
					IsEmptyMessageVisible = true;
				}
			}
			catch (Exception e)
			{
				UserDialogs.Instance.Toast(Messages.SomethingWentWrong);

				Debug.WriteLine($"{GetType()}.LoadData(): {e}");
			}
			IsBusy = false;
		}
    }
}

