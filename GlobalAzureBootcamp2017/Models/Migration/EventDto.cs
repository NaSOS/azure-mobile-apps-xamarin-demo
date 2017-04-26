using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GlobalAzureBootcamp2017
{
	public class EventDto : Event
	{
		[JsonProperty("activities")]
		public IEnumerable<ActivityDto> Activities { get; set; }
	}
}
