using Microsoft.WindowsAzure.MobileServices.Sync;

namespace GlobalAzureBootcamp2017
{
	public class SpeakerRepository : BaseRRepository<Speaker>
	{
		public override string Identifier => nameof(Speaker);

		public SpeakerRepository(IService service, IMobileServiceSyncTable<Speaker> table) : base(service, table)
		{
		}
	}
}
