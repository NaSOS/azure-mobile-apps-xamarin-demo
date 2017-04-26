using Microsoft.WindowsAzure.MobileServices.Sync;

namespace GlobalAzureBootcamp2017
{
	public class EventRepository : BaseRRepository<Event>
	{
		public override string Identifier => nameof(Event);

        public EventRepository(IService service, IMobileServiceSyncTable<Event> table) : base(service, table)
		{
		}
	}
}
