using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace GlobalAzureBootcamp2017
{
	public interface IService
	{
		Task<bool> SyncAsync();

		Task PushAsync();
	}
}
