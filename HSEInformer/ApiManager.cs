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
using System.Net;
using System.Net.Http.Headers;

namespace HSEInformer
{
    class ApiManager
    {
        private string _host;
        const string UriLogin = "{0}/login";
        const string UriCheckAccountIsRegistred = "{0}/checkAccountIsRegistred?email={1}";
        const string UriSendConfirmationCode = "{0}/sendConfirmationCode?email={1}";
        const string UriConfirmEmail = "{0}/confirmEmail";
        const string UriRegister = "{0}/register";
        const string UriGetGroups = "{0}/getGroups";
        const string UriCheckIfAdmin = "{0}/checkIfAdmin?id={1}";
        const string UriGetPosts = "{0}/getPosts?id={1}";
        const string UriGetGroupMembers = "{0}/getGroupMembers?id={1}";
        const string UriGetAdministrator = "{0}/getAdministrator?id={1}";
        const string UriGetPostPermissionRequests = "{0}/getPostPermissionRequests?id={1}";
        const string UriGetPostPermissions = "{0}/getPostPermissions?id={1}";
        


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

        public async Task<bool> SendConfirmationCode(string email)
        {
            var accountIsRegistred = await CheckAccountIsRegistred(email);
            if (!accountIsRegistred)
            {
                using (var client = new HttpClient())
                {

                    string requestUri = string.Format(UriSendConfirmationCode, _host, email);
                    HttpResponseMessage response = await client.GetAsync(requestUri);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<Response<bool>>(responseString);

                        if (res != null && res.Ok)
                        {
                            return res.Result;
                        }
                        else
                        {
                            throw new WebException(res.Message);
                        }
                    }
                    else
                    {
                        throw new WebException("Неполадки на сервере");
                    }
                }
            }
            else
            {
                throw new Exception("Данный аккаунт уже зарегистрирован в системе HSE Informer");
            }
        }

        public async Task<bool> CheckAccountIsRegistred(string email)
        {
            using (var client = new HttpClient())
            {

                string requestUri = string.Format(UriCheckAccountIsRegistred, _host, email);
                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<Response<bool>>(responseString);

                    if (res != null && res.Ok)
                    {
                        return res.Result;
                    }
                    else
                    {
                        throw new WebException(res.Message);
                    }
                }
                else
                {
                    throw new WebException("Неполадки на сервере");
                }
            }
        }

        public async Task<Model.User> ConfirmEmail(string email, string code)
        {
            using (var client = new HttpClient())
            {

                string requestUri = string.Format(UriConfirmEmail, _host);

                var jsonString = JsonConvert.SerializeObject(new { code = code, email = email });
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<Response<User>>(responseString);

                    if (res != null && res.Ok)
                    {
                        return new Model.User
                        {
                            Name = res.Result.Name,
                            Surname = res.Result.Surname,
                            Patronymic = res.Result.Patronymic,
                            Email = res.Result.Username
                        };

                    }
                    else
                    {
                        throw new WebException(res.Message);
                    }
                }
                else throw new WebException("Неполадки на сервере");
            }
        }

        public async Task<bool> Register(string username, string password, string confirmed_password, string code)
        {
            using (var client = new HttpClient())
            {

                string requestUri = string.Format(UriRegister, _host);

                var jsonString = JsonConvert.SerializeObject(new
                {
                    Code = code,
                    Email = username,
                    Password = password,
                    PasswordConfirm = confirmed_password

                });

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<Response<bool>>(responseString);

                    if (res != null && res.Ok)
                    {
                        return res.Result;

                    }
                    else
                    {
                        throw new WebException(res.Message);
                    }
                }
                else throw new WebException("Неполадки на сервере");
            }
        }


        public async Task<List<Model.Group>> GetGroups(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string requestUri = string.Format(UriGetGroups, _host);

                HttpResponseMessage response = await client.GetAsync(requestUri);



                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<Response<Group[]>>(responseString);
                    if (res != null && res.Ok)
                    {
                        var DTOgroups = res.Result;

                        var modelGroups = DTOgroups.Select(g => new Model.Group
                        {
                            Id = g.Id,
                            Name = g.Name,
                            Type = g.GroupType == 0 ? Model.GroupType.AutoCreated : Model.GroupType.Custom,
                        }).ToArray();

                        return modelGroups.ToList();
                    }
                    else
                    {
                        throw new WebException("Неполадки на сервере");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    throw new WebException("Неполадки на сервере");
                }

            }
        }

        public async Task<bool> CheckIfAdmin(string token, int id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string requestUri = string.Format(UriCheckIfAdmin, _host, id);

                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<Response<bool>>(responseString);
                    if (res != null && res.Ok)
                    {
                        return res.Result;
                    }
                    else
                    {
                        throw new WebException("Неполадки на сервере");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    throw new WebException("Неполадки на сервере");
                }

            }
        }


        public async Task<List<Model.Post>> GetPosts(string token, int id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string requestUri = string.Format(UriGetPosts, _host, id);

                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<Response<List<Post>>>(responseString);
                    if (res != null && res.Ok)
                    {
                        var modelPosts = res.Result.Select(p => new Model.Post
                        {
                            Id = p.Id,
                            Theme = p.Theme,
                            Content = p.Content,
                            Time = p.Time,
                            User = new Model.User
                            {
                                Email = p.User.Username,
                                Name = p.User.Name,
                                Surname = p.User.Surname,
                                Patronymic = p.User.Patronymic
                            }

                        }).ToList();

                        return modelPosts.OrderByDescending(p => p.Time).ToList();
                    }
                    else
                    {
                        throw new WebException("Неполадки на сервере");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    throw new WebException("Неполадки на сервере");
                }

            }
        }



        public async Task<List<Model.User>> GetGroupMembers(string token, int id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string requestUri = string.Format(UriGetGroupMembers, _host, id);

                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<Response<List<User>>>(responseString);
                    if (res != null && res.Ok)
                    {
                        var modelMembers = res.Result.Select(m => new Model.User
                        {
                            Email = m.Username,
                            Name = m.Name,
                            Surname = m.Surname,
                            Patronymic = m.Patronymic
                        }).ToList();
                        return modelMembers.OrderBy(m => m.Surname).ToList();
                    }
                    else
                    {
                        throw new WebException("Неполадки на сервере");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    throw new WebException("Неполадки на сервере");
                }

            }
        }


        public async Task<List<Model.User>> GetPostPermissions(string token, int id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string requestUri = string.Format(UriGetPostPermissions, _host, id);

                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<Response<List<User>>>(responseString);
                    if (res != null && res.Ok)
                    {
                        var modelMembers = res.Result.Select(m => new Model.User
                        {
                            Email = m.Username,
                            Name = m.Name,
                            Surname = m.Surname,
                            Patronymic = m.Patronymic
                        }).ToList();
                        return modelMembers.OrderBy(m => m.Surname).ToList();
                    }
                    else
                    {
                        throw new WebException("Неполадки на сервере");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    throw new WebException("Неполадки на сервере");
                }

            }
        }


        public async Task<List<Model.User>> GetPostPermissionRequests(string token, int id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string requestUri = string.Format(UriGetPostPermissionRequests, _host, id);

                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<Response<List<User>>>(responseString);
                    if (res != null && res.Ok)
                    {
                        var modelMembers = res.Result.Select(m => new Model.User
                        {
                            Email = m.Username,
                            Name = m.Name,
                            Surname = m.Surname,
                            Patronymic = m.Patronymic
                        }).ToList();
                        return modelMembers.OrderBy(m => m.Surname).ToList();
                    }
                    else
                    {
                        throw new WebException("Неполадки на сервере");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    throw new WebException("Неполадки на сервере");
                }

            }
        }

        public async Task<Model.User> GetAdministrator(string token, int id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string requestUri = string.Format(UriGetAdministrator, _host, id);

                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<Response<User>>(responseString);
                    if (res.Ok)
                    {
                        return res.Result != null ? new Model.User
                        {
                            Email = res.Result.Username,
                            Name = res.Result.Name,
                            Surname = res.Result.Surname,
                            Patronymic = res.Result.Patronymic
                        } : null;
                    }
                    else
                    {
                        throw new WebException("Неполадки на сервере");
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    throw new WebException("Неполадки на сервере");
                }

            }
        }

    }
}

