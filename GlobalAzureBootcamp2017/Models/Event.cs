using System;
using Newtonsoft.Json;

namespace GlobalAzureBootcamp2017
{
	public class Event : BaseModel
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("date")]
		public DateTime Date { get; set; }

		[JsonProperty("info")]
		public string Info { get; set; }
	}
}
