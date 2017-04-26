using Newtonsoft.Json;

namespace GlobalAzureBootcamp2017
{
	public class ActivityDto : Activity
	{
		[JsonProperty("speaker")]
		public SpeakerDto Speaker { get; set; }
	}
}
