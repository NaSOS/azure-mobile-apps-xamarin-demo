using System.Collections.Generic;
using Newtonsoft.Json;

namespace GlobalAzureBootcamp2017
{
	public class SpeakerDto : Speaker
	{
		[JsonProperty("socials")]
		public new IEnumerable<string> Socials { get; set; }
	}
}
