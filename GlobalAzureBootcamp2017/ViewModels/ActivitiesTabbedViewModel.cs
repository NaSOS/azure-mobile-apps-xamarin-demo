using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
	public class ActivitiesTabbedViewModel : BaseCollectionViewModel<ActivitiesPage>
	{
		public async Task LoadDataAsync(bool forceRefresh = false)
		{
			IsBusy = true;
			IsEmptyMessageVisible = false;
			try
			{
				Collection.Clear();

                var activities = await AzureService.Instance.ActivityRepository.GetByEventIdAsync(Constants.EventId, forceRefresh);
				// force refresh data if nothing was returned
                if (!forceRefresh && (activities == null || activities.Count == 0))
				{
					await AzureService.Instance.SyncAsync();

					activities = await AzureService.Instance.ActivityRepository.GetByEventIdAsync(Constants.EventId, forceRefresh);
				}
				if (activities?.Count > 0)
				{
					var tabs = activities.GroupBy(a => a.Place).Select(g => new ActivitiesPage
					{
						Title = g.FirstOrDefault()?.Place,
						BindingContext = new ActivitiesViewModel(g.Select(a => new ActivityViewModel { Activity = a }))
					});
					foreach (var tab in tabs)
					{
						Collection.Add(tab);
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

