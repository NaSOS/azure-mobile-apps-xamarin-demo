using Newtonsoft.Json;

namespace GlobalAzureBootcamp2017
{
    public class UserActivity : BaseModel
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("activityId")]
        public string ActivityId { get; set; }

        [JsonIgnore]
        public Activity Activity {get; set;}
	}
}
