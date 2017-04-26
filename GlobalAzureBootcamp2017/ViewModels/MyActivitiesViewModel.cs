using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
    public class MyActivitiesViewModel : BaseCollectionViewModel<ActivityViewModel>
    {
		private bool _isLoginButtonVisible = true;
		public bool IsLoginButtonVisible
		{
			get
			{
				return _isLoginButtonVisible;
			}
			set
			{
				if (_isLoginButtonVisible == value) return;
				_isLoginButtonVisible = value;
				SetPropertyChanged("IsLoginButtonVisible");
			}
		}

		public ICommand RefreshCommand
		{
			get
			{
				return new Command(async () => await LoadDataAsync(true));
			}
		}

		public async Task LoadDataAsync(bool forceRefresh = false)
		{
            if(!AuthenticationHelper.Instance.IsAuthenticated){
                IsLoginButtonVisible = true;
                IsEmptyMessageVisible = false;
                return;
            }

            IsLoginButtonVisible = false;

            IsBusy = true;
			IsEmptyMessageVisible = false;
			try
			{
				Collection.Clear();

                var userActivities = await AzureService.Instance.UserActivityRepository.GetAsync(forceRefresh);
				// force refresh data if nothing was returned
				if (!forceRefresh && (userActivities == null || userActivities.Count == 0))
				{
					userActivities = await AzureService.Instance.UserActivityRepository.GetAsync(true);
				}
				if (userActivities?.Count > 0)
				{
                    foreach (var userActivity in userActivities.OrderBy(a => a.Activity?.From))
					{
                        Collection.Add(new ActivityViewModel { Activity = userActivity.Activity });
					}
                    // sort
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
