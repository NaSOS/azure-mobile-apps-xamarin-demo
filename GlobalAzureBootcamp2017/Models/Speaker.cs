using Newtonsoft.Json;

namespace GlobalAzureBootcamp2017
{
	public class Speaker : BaseModel
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("info")]
		public string Info { get; set; }

		[JsonProperty("socials")]
		public string Socials { get; set; }
	}
}
