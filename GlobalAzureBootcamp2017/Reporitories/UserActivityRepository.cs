using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace GlobalAzureBootcamp2017
{
    public class UserActivityRepository : BaseCRUDRepository<UserActivity>
    {
		public override string Identifier => nameof(UserActivity);

        public ActivityRepository ActivityRepository { get; }

        public UserActivityRepository(IService service, IMobileServiceSyncTable<UserActivity> table, ActivityRepository activityRepository) : base(service, table)
		{
			ActivityRepository = activityRepository;
		}

		public async new Task<IList<UserActivity>> GetAsync(bool forseRefresh = false)
        {
            try
            {
                // sync data
                if (forseRefresh)
                {
                    await PullLatestAsync().ConfigureAwait(false);
                }

                var items = await _table.Where(a => a.UserId == AuthenticationHelper.Instance.User.UserId).ToEnumerableAsync().ConfigureAwait(false);
                var list = items.ToList();
                foreach(var item in list)
                {
                    item.Activity = await ActivityRepository.GetByIdAsync(item.ActivityId);
                }
                return list;
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Toast(Messages.SomethingWentWrong);

                Debug.WriteLine($"{Identifier}.GetAsync(): {e}");

                return null;
            }
        }

        public async Task<UserActivity> GetByActivityIdAsync(string activityId, bool forseRefresh = false)
        {
            try
            {
                // sync data
                if (forseRefresh)
                {
                    await PullLatestAsync().ConfigureAwait(false);
                }

                var item = await _table.Where(a => a.UserId == AuthenticationHelper.Instance.User.UserId && a.ActivityId == activityId).ToEnumerableAsync().ConfigureAwait(false);

                return item.FirstOrDefault();
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Toast(Messages.SomethingWentWrong);

                Debug.WriteLine($"{Identifier}.GetByActivityAsync({activityId}): {e}");

                return default(UserActivity);
            }
        }
    }
}
