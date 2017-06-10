using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HSEInformer.DTO;

namespace HSEInformer
{
    class ApiManager
    {
        private string _host;
        const string UriLogin = "{0}/login";
        public ApiManager(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException("Host");
            }

            this._host = host;
        }

        public async Task<string> Login(string username, string password)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string> {
                    {"grant_type" , "password"},
                    { "username", username },
                    { "password", password }
                };
                var content = new FormUrlEncodedContent(values);

                string uri = string.Format(UriLogin, _host);
                
                HttpResponseMessage response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var res = JsonConvert.DeserializeObject<Token>(responseString);

                    if (res != null && res.AcessToken != null)
                    {
                        return res.AcessToken;
                    }
                    else
                    {
                        throw new UnauthorizedAccessException("Неверное имя пользователя или пароль");
                    }
                }
                else
                {
                    throw new UnauthorizedAccessException("Неверное имя пользовтеля или пароль");
                }
            }
        }


    }
}