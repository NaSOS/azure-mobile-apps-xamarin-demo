using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace GlobalAzureBootcamp2017
{
	/// <summary>
	/// Base Repository that supports the basic read (R) operations as well as table syncing
	/// </summary>
	public abstract class BaseRRepository<T> where T : BaseModel
	{
		readonly IService _service;
        protected readonly IMobileServiceSyncTable<T> _table;

		protected BaseRRepository(IService service, IMobileServiceSyncTable<T> table)
		{
			_service = service;
            _table = table;
        }

		public virtual string Identifier => "Items";

		/// <summary>
		/// Get all the table data asynchronously.
		/// </summary>
		/// <param name="forseRefresh">If set to <c>true</c> forse refresh.</param>
		public async Task<IList<T>> GetAsync(bool forseRefresh = false)
		{
			try
			{
				// sync data
				if (forseRefresh)
				{
					await PullLatestAsync().ConfigureAwait(false);
				}

				var items = await _table.ToEnumerableAsync().ConfigureAwait(false);

				return items.ToList();
			}
			catch (Exception e)
			{
				UserDialogs.Instance.Toast(Messages.SomethingWentWrong);

				Debug.WriteLine($"{Identifier}.GetAsync(): {e}");

				return null;
			}
		}

		/// <summary>
		/// Get the item with the specified identifier asynchronously.
		/// </summary>
		/// <param name="forseRefresh">If set to <c>true</c> forse refresh.</param>
		public async Task<T> GetByIdAsync(string id, bool forseRefresh = false)
		{
			try
			{
				// sync data
				if (forseRefresh)
				{
					await PullLatestAsync().ConfigureAwait(false);
				}

				var item = await _table.LookupAsync(id).ConfigureAwait(false);

				return item;
			}
			catch (Exception e)
			{
				UserDialogs.Instance.Toast(Messages.SomethingWentWrong);

				Debug.WriteLine($"{Identifier}.GetAsync({id}): {e}");

				return default(T);
			}
		}

		/// <summary>
		/// Helper method to pulls the latest table data asynchronously.
		/// </summary>
		protected async Task<bool> PullLatestAsync()
		{
			try
			{
				// pull all
				await _table.PullAsync($"all{Identifier}", _table.CreateQuery()).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				UserDialogs.Instance.Toast(Messages.SomethingWentWrong);

				Debug.WriteLine($"{Identifier}.PullLatestAsync: {e}");

				return false;
			}
			return true;
		}

		/// <summary>
		/// Syncs the table asynchronously.
		/// </summary>
		public async Task<bool> SyncAsync()
		{
			try
			{
				await _service.PushAsync();

				if (!(await PullLatestAsync()))
					return false;
			}
			catch (MobileServicePushFailedException ex)
			{
				UserDialogs.Instance.Toast(Messages.SomethingWentWrong);

				if (ex.PushResult?.Errors != null)
				{
					foreach (var error in ex.PushResult.Errors)
					{
						if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
						{
							//Update failed, reverting to server's copy
							await error.CancelAndUpdateItemAsync(error.Result);
						}
						else
						{
							//Discard local change
							await error.CancelAndDiscardItemAsync();
						}

						var sb = new StringBuilder();
						foreach (var v in error.Result)
						{
							sb.AppendLine(v.Value.ToString());
						}

						Debug.WriteLine($"{Identifier}.SyncAsync: {sb}");
					}
				}

				Debug.WriteLine($"{Identifier}.SyncAsync: Unable to sync");

				return false;
			}

			return true;
		}
	}
}
