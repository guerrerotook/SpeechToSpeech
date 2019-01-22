﻿namespace Microsoft.Cse.SpeechToSpeech.UI.Translation
{
    using Microsoft.Cse.SpeechToSpeech.UI.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class TranslationClient
    {
        private static HttpClient client = new HttpClient();

        private const string baseUrl = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0";
        private string translationApiKey;

        public TranslationClient(string apiKey)
        {
            apiKey.EnsureIsNotNull(nameof(apiKey));

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

            translationApiKey = apiKey;
        }

        public async Task<List<TranslationResult>> Translate(Language from, Language to, string text)
        {
            List<TranslationResult> result = null;

            if (from != null && to != null)
            {
                List<(string, string)> queryString = new List<(string, string)>();
                queryString.Add(("from", from.Code));
                queryString.Add(("to", to.Code));
                queryString.Add(("textType", "plain"));

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, GenerateRequestUri(queryString)))
                {
                    request.Headers.Add("X-ClientTraceId", Guid.NewGuid().ToString());

                    JObject body = new JObject();
                    body["text"] = text;

                    string json = new JArray(body).ToString();
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    request.Content = content;

                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (response.Content)
                            {
                                string responseJson = await response.Content.ReadAsStringAsync();
                                result = JsonConvert.DeserializeObject<List<TranslationResult>>(responseJson);
                            }
                        }
                        else
                        {
                            using (response.Content)
                            {
                                string responseJson = await response.Content.ReadAsStringAsync();
                                result = new List<TranslationResult>();
                                result.Add(new TranslationResult()
                                {
                                    IsSuccess = false,
                                    Error = responseJson
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        private string GenerateRequestUri(List<(string, string)> items)
        {
            string result = null;

            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(baseUrl);
                sb.Append("&");
                int count = 0;
                foreach (var item in items)
                {
                    sb.AppendFormat("{0}={1}", item.Item1, item.Item2);
                    if ((count + 1) < items.Count)
                    {
                        sb.Append("&");
                    }
                    count++;
                }

                result = sb.ToString();
            }

            return result;
        }
    }
}
