using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GlobalAzureBootcamp2017
{
    public class TextAnalyticsRepository
    {
        private readonly Dictionary<string, IEnumerable<string>> Cache = new Dictionary<string, IEnumerable<string>>();

        public async Task<IEnumerable<string>> GetKeyPhrasesAsync(string text)
        {
            try
            {
				var documents = new DocumentsDto();
				documents.Documents.Add(new DocumentDto
				{
					Language = "en",
					Id = "1",
					Text = text
				});

				var json = JsonConvert.SerializeObject(documents);
                IEnumerable<string> cached;
                Cache.TryGetValue(json, out cached);
               	if (cached != null) return cached;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Constants.CognitiveServicesUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Constants.CognitiveServicesKey);
                    var response = await client.PostAsync("keyPhrases", new StringContent(json, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        documents = JsonConvert.DeserializeObject<DocumentsDto>(body);
                        var document = documents.Documents.FirstOrDefault(d => d.Id == "1");
                        var data = document?.KeyPhrases ?? new List<string>();
                        Cache[json] = data;
                        return data;
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
				Debug.WriteLine($"TextAnalyticsRepository.GetKeyPhrasesAsync(): {e}");

				return null;
            }
        }

        class DocumentsDto
		{
            public DocumentsDto(){
                Documents = new List<DocumentDto>();
            }

            public IList<DocumentDto> Documents { get; set; }
		}

        class DocumentDto
        {
            public string Language { get; set; }

            public string Id { get; set; }

            public string Text { get; set; }

            public IEnumerable<string> KeyPhrases { get; set; }
        }
    }
}
