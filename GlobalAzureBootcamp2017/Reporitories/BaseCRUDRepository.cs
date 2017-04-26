using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace GlobalAzureBootcamp2017
{
	/// <summary>
	/// Base Repository that supports the basic read/write (CRUD) operations as well as table syncing
	/// </summary>
	public class BaseCRUDRepository<T> : BaseRRepository<T> where T : BaseModel
	{
		protected BaseCRUDRepository(IService service, IMobileServiceSyncTable<T> table) : base(service, table)
		{
		}

		/// <summary>
		/// Saves the item asynchronously.
		/// If the item already exist then Update it, otherwise Create it.
		/// </summary>
		/// <param name="item">Item.</param>
		public async Task<bool> SaveAsync(T item)
		{
			try
			{
				if (item.Id == null)
				{
					return await InsertAsync(item);
				}
				else
				{
					return await UpdateAsync(item);
				}
			}
			catch (Exception e)
			{
				UserDialogs.Instance.Toast(Messages.SomethingWentWrong);

				Debug.WriteLine($"{Identifier}.SaveAsync({item}): {e}");

				return false;
			}
		}

		/// <summary>
		/// Inserts the item asynchronously.
		/// </summary>
		/// <param name="item">Item.</param>
		public async Task<bool> InsertAsync(T item)
		{
			try
			{
				await _table.InsertAsync(item).ConfigureAwait(false);
				var push = await SyncAsync().ConfigureAwait(false);

				if (push)
				{
					var updated = await GetByIdAsync(item.Id).ConfigureAwait(false);
					item.Version = updated.Version;
					item.UpdatedAt = updated.UpdatedAt;
				}

				return push;
			}
			catch (Exception e)
			{
				UserDialogs.Instance.Toast(Messages.SomethingWentWrong);

				Debug.WriteLine($"{Identifier}.InsertAsync({item}): {e}");

				return false;
			}
		}

		/// <summary>
		/// Updates the item asynchronously.
		/// </summary>
		/// <param name="item">Item.</param>
		public async Task<bool> UpdateAsync(T item)
		{
			try
			{
				await _table.UpdateAsync(item).ConfigureAwait(false);
				var push = await SyncAsync().ConfigureAwait(false);
				var updated = await GetByIdAsync(item.Id).ConfigureAwait(false);

				item.Version = updated.Version;
				item.UpdatedAt = updated.UpdatedAt;

				return push;
			}
			catch (MobileServicePreconditionFailedException<T> e)
			{
				// update version
				item.Version = e.Item.Version;
				return await UpdateAsync(item);
			}
			catch (Exception e)
			{
				UserDialogs.Instance.Toast(Messages.SomethingWentWrong);

				Debug.WriteLine($"{Identifier}.UpdateAsync({item}): {e}");

				return false;
			}
		}

		/// <summary>
		/// Deletes the item asynchronously.
		/// </summary>
		/// <param name="item">Item.</param>
		public async Task<bool> DeleteAsync(T item)
		{
			try
			{
				await _table.DeleteAsync(item).ConfigureAwait(false);
				var push = await SyncAsync().ConfigureAwait(false);
				return push;
			}
			catch (Exception e)
			{
				UserDialogs.Instance.Toast(Messages.SomethingWentWrong);

				Debug.WriteLine($"{Identifier}.DeleteAsync({item}): {e}");

				return false;
			}
		}

	}
}
