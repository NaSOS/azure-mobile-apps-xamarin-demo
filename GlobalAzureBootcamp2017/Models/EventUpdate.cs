using Newtonsoft.Json;

namespace GlobalAzureBootcamp2017
{
	public class EventUpdate : BaseModel
	{
        [JsonProperty("eventId")]
        public string EventId { get; set; }

        [JsonProperty("message")]
		public string Message { get; set; }
	}
}

