using System;
using Newtonsoft.Json;

namespace GlobalAzureBootcamp2017
{
	public class Activity : BaseModel
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("info")]
		public string Info { get; set; }

		[JsonProperty("from")]
		public DateTime From { get; set; }

		[JsonProperty("to")]
		public DateTime? To { get; set; }

		[JsonProperty("type")]
		public ActivityType Type { get; set; }

		[JsonProperty("place")]
		public string Place { get; set; }

		[JsonProperty("eventId")]
		public string EventId { get; set; }

		[JsonProperty("speakerId")]
		public string SpeakerId { get; set; }
	}
}
