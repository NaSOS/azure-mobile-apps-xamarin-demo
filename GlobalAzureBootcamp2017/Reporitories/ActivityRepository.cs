using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace GlobalAzureBootcamp2017
{
	public class ActivityRepository : BaseRRepository<Activity>
	{
		public override string Identifier => nameof(Activity);

        public ActivityRepository(IService service, IMobileServiceSyncTable<Activity> table) : base(service, table)
		{
		}

		public async Task<IList<Activity>> GetByEventIdAsync(string eventId, bool forseRefresh = false)
		{
			try
			{
				// sync data
				if (forseRefresh)
				{
					await PullLatestAsync().ConfigureAwait(false);
				}

                var items = await _table.Where(e => e.EventId==eventId).ToEnumerableAsync().ConfigureAwait(false);

				return items.ToList();
			}
			catch (Exception e)
			{
				UserDialogs.Instance.Toast(Messages.SomethingWentWrong);

				Debug.WriteLine($"{Identifier}.GetAsync(): {e}");
				return null;
			}
		}
	}
}
