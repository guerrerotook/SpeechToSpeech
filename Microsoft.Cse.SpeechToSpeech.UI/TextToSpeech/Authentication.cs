namespace Microsoft.Cse.SpeechToSpeech.UI.TextToSpeech
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class demonstrates how to get a valid O-auth token
    /// </summary>
    public class Authentication
    {
        // Issue token uri for new unified SpeechService API "https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken". 
        // Note: new unified SpeechService API key and issue token uri is per region
        public static readonly string AccessUri = "https://westeurope.api.cognitive.microsoft.com/sts/v1.0/issuetoken";
        private string apiKey;
        private string accessToken;
        private Timer accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public Authentication(string apiKey)
        {
            this.apiKey = apiKey;

            
        }

        public async Task RenewAuthenticationToken()
        {
            this.accessToken = await HttpPost(AccessUri, this.apiKey);
            
            // renew the token every specfied minutes
            accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback),
                                           this,
                                           TimeSpan.FromMinutes(RefreshTokenDuration),
                                           TimeSpan.FromMilliseconds(-1));
        }

        public string GetAccessToken()
        {
            return this.accessToken;
        }

        private async Task RenewAccessToken()
        {
            string newAccessToken = await HttpPost(AccessUri, this.apiKey);
            //swap the new token with old one
            //Note: the swap is thread unsafe
            this.accessToken = newAccessToken;
        }

        private async void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                await RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }

        private async Task<string> HttpPost(string accessUri, string apiKey)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, accessUri))
                {
                    request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        using (response.Content)
                        {
                            return await response.Content.ReadAsStringAsync();
                        }
                    }
                }

            }
        }
    }
}
