using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FutbolPlay.WebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FutbolPlay.WebApi.Services
{
    public class RestService
    {
        #region Vars

        HttpClient _client;
        readonly string _uriAuthBase = "", _uriBase = "";
        readonly string _clientIdPlay = "";
        readonly string _clientIdPlayAdmin = "";
        static RestService _instance;
        static bool _addAuth;
        readonly string _uriBaseImg = "http://coach.sportsplay-app.com/resources/drawable/";

        #endregion

        #region Properties

        public string ErrorCode { get; set; }
        public string DefaultImgUri { get { return _uriBaseImg; } }

        #endregion

        #region Constructor

        private RestService() { _client = new HttpClient(); _addAuth = true; }
        public static RestService Instance
        {
            get
            {
                if (_instance == null) { _instance = new RestService(); }
                return _instance;
            }
        }

        public void AddAuth()
        {
            if (_addAuth)
            {
                if (SessionCustomerService.Account == null) { _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", SessionService.Account.Token); }
                else { _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", SessionCustomerService.Account.Token); }
                _addAuth = false;
            }
        }

        #endregion

        #region User

        public async Task<bool> RegisterAsync(UserModel model)
        {
            ErrorCode = string.Empty;

            var uri = new Uri(string.Concat(_uriBase, "api/users"));
            var json = JsonConvert.SerializeObject
                (new { name = model.Name, password = model.Password, phone = model.Phone, email = model.Mail, id_usersocialred = model.IdSocialNetwork, id_users_type = 3 });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            var result = await response.Content.ReadAsStringAsync();
            JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);

            if (response.StatusCode == HttpStatusCode.Created) { return await DoLoginAsync(model); }
            else
            {
                ErrorCode = (string)responseModel["message"];
                return false;
            }
        }

        public async Task<bool> DoLoginAsync(UserModel model)
        {
            ErrorCode = string.Empty;

            var uri = new Uri(string.Concat(_uriAuthBase, "oauth2/token"));
            StringContent data = null;

            if (string.IsNullOrWhiteSpace(model.IdSocialNetwork))
            {
                data = new StringContent
                    (string.Format("client_id={0}&grant_type=password&username={1}&password={2}", _clientIdPlay, model.Mail, model.Password), Encoding.UTF8, "application/x-www-form-urlencoded");
            }
            else
            {
                data = new StringContent
                    (string.Format("client_id={0}&grant_type=password&username={1}", _clientIdPlay, model.IdSocialNetwork), Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, data);

            var result = await response.Content.ReadAsStringAsync();
            JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var auth = new AuthModel
                {
                    Token = (string)responseModel["access_token"],
                    IdUser = (string)responseModel["id_user"],
                    UserName = (string)responseModel["user_name"],
                    UserPhone = (string)responseModel["user_phone"],
                    ExpiresIn = (string)responseModel["expires_in"],
                    IdSocialNetwork = model.IdSocialNetwork,
                    AuthType = AuthType.FutbolPlayApp
                };

                SessionService.SaveAccount(auth);
                return true;
            }
            else
            {
                ErrorCode = (string)responseModel["error"];
                return false;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var uri = new Uri(string.Concat(_uriBase, "validatetoken"));
            StringContent data = new StringContent(string.Format("name={0}", token), Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, data);

            var result = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK) { return true; }
            else { return false; }
        }

        public async Task<bool> RecoverPasswordAsync(string mail)
        {
            ErrorCode = string.Empty;

            var uri = new Uri(string.Format("{0}api/users/forgotpassword", _uriBase));

            var json = JsonConvert.SerializeObject(new { email = mail });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            var result = await response.Content.ReadAsStringAsync();
            JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);

            if (response.IsSuccessStatusCode) { return true; }
            else
            {
                ErrorCode = (string)responseModel["Message"];
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            ErrorCode = string.Empty;

            var uri = new Uri(string.Format("{0}api/users/access/{1}", _uriBase, SessionService.Account.IdUser));
            AddAuth();

            var json = JsonConvert.SerializeObject(new { password_old = oldPassword, password_new = newPassword });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            var result = await response.Content.ReadAsStringAsync();
            JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);

            if (response.IsSuccessStatusCode) { return true; }
            else
            {
                ErrorCode = (string)responseModel["Message"];
                return false;
            }
        }

        public async Task<UserModel> GetProfileAsync()
        {
            var uri = new Uri(string.Format("{0}api/users/{1}", _uriBase, SessionService.Account.IdUser));
            AddAuth();

            HttpResponseMessage response = null;
            response = await _client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);

                UserModel user = new UserModel
                {
                    Name = (string)responseModel["name"],
                    Mail = (string)responseModel["email"],
                    Phone = (string)responseModel["phone"],
                    IdSocialNetwork = (string)responseModel["id_usersocialred"]
                };

                return user;
            }
            else { return null; }
        }

        public async Task<bool> UpdateProfileAsync(UserModel model)
        {
            ErrorCode = string.Empty;

            var uri = new Uri(string.Format("{0}api/users/update/{1}", _uriBase, SessionService.Account.IdUser));
            AddAuth();

            var json = JsonConvert.SerializeObject
                (new { name = model.Name, phone = model.Phone, email = model.Mail });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            var result = await response.Content.ReadAsStringAsync();
            JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);

            if (response.IsSuccessStatusCode)
            {
                SessionService.UpdateAccount(model);
                return true;
            }
            else
            {
                ErrorCode = (string)responseModel["message"];
                return false;
            }
        }

        public async Task<int> CreateUserOfflineAsync(UserModel model)
        {
            ErrorCode = string.Empty;

            var uri = new Uri(string.Concat(_uriBase, "api/users_offline"));
            AddAuth();

            var json = JsonConvert.SerializeObject(new { name = model.Name, phone = model.Phone, email = model.Mail, id_place = SessionCustomerService.Account.IdPlace });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);

                return (int)responseModel["id_users_offline"];
            }
            else { return -1; }
        }

        public async Task<List<UserModel>> SearchUsersAsync(string name)
        {
            var uri = new Uri(string.Concat(_uriBase, "api/users/bynameandidplace"));
            StringContent data = new StringContent(string.Format("name={0}&id_users_type={1}", name, SessionCustomerService.Place.Id), Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, data);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();

                List<UserModel> userModel = JArray.Parse(result).Select(x => new UserModel
                {
                    IdUser = (int)x["id"],
                    Name = ((string)x["name"]).Trim(),
                    Mail = x["email"] != null ? ((string)x["email"]).Trim() : string.Empty,
                    Phone = x["phone"] != null ? ((string)x["phone"]).Trim() : string.Empty,
                    IdUserType = (int)x["id_user_type"]
                }).ToList();

                return userModel;
            }
            else { return null; }
        }

        #endregion

        #region Customer

        public async Task<bool> DoLoginCustomerAsync(CustomerModel model)
        {
            ErrorCode = string.Empty;

            var uri = new Uri(string.Concat(_uriAuthBase, "oauth2/token"));
            StringContent data = null;

            data = new StringContent
                (string.Format("client_id={0}&grant_type=password&username={1}&password={2}", _clientIdPlayAdmin, model.Mail, model.Password), Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, data);

            var result = await response.Content.ReadAsStringAsync();
            JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);

            if (response.StatusCode == HttpStatusCode.OK)
            {

                var auth = new AuthCustomerModel
                {
                    Token = (string)responseModel["access_token"],
                    ExpiresIn = (string)responseModel["expires_in"],
                    IdUser = (string)responseModel["id_user"],
                    UserName = (string)responseModel["name_customer"],
                    IdPlace = (string)responseModel["id_place"]
                };

                SessionCustomerService.SaveAccount(auth);
                SessionCustomerService.Place = await GetPlaceAsync(Convert.ToInt32((string)responseModel["id_place"]));
                if (SessionCustomerService.Place == null)
                {
                    SessionCustomerService.DeleteAccount();
                    return false;
                }
                else
                {
                    SessionCustomerService.Place.HasMutiple = await ValidateIfPlaceHasMultiples(SessionCustomerService.Place.Id);
                }

                return true;
            }
            else
            {
                ErrorCode = (string)responseModel["error"];
                return false;
            }
        }

        public async Task<bool> RecoverPasswordCustomerAsync(string mail)
        {
            ErrorCode = string.Empty;

            var uri = new Uri(string.Format("{0}api/customers/forgotpassword", _uriBase));

            var json = JsonConvert.SerializeObject(new { email = mail });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            var result = await response.Content.ReadAsStringAsync();
            JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);

            if (response.IsSuccessStatusCode) { return true; }
            else
            {
                ErrorCode = (string)responseModel["Message"];
                return false;
            }
        }

        public async Task<CustomerModel> GetProfileCustomerAsync()
        {
            var uri = new Uri(string.Format("{0}api/customers/{1}", _uriBase, SessionCustomerService.Account.IdUser));
            AddAuth();

            HttpResponseMessage response = null;
            response = await _client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                response.Dispose();

                JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);

                CustomerModel user = new CustomerModel
                {
                    Name = (string)responseModel["full_name"],
                    Mail = (string)responseModel["email"],
                    Phone = (string)responseModel["phone"]
                };

                return user;
            }
            else { return null; }
        }

        public async Task<bool> UpdateProfileCustomerAsync(CustomerModel model)
        {
            ErrorCode = string.Empty;

            var uri = new Uri(string.Format("{0}api/customers/update/{1}", _uriBase, SessionCustomerService.Account.IdUser));
            AddAuth();

            var json = JsonConvert.SerializeObject
                (new { full_name = model.Name, phone = model.Phone, email = model.Mail });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            var result = await response.Content.ReadAsStringAsync();
            JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);

            if (response.IsSuccessStatusCode) { return true; }
            else
            {
                ErrorCode = (string)responseModel["message"];
                return false;
            }
        }

        public async Task<bool> ChangePasswordCustomerAsync(string oldPassword, string newPassword)
        {
            ErrorCode = string.Empty;

            var uri = new Uri(string.Format("{0}api/customers/access/{1}", _uriBase, SessionCustomerService.Account.IdUser));
            AddAuth();

            var json = JsonConvert.SerializeObject(new { password_old = oldPassword, password_new = newPassword });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            var result = await response.Content.ReadAsStringAsync();
            JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);

            if (response.IsSuccessStatusCode) { return true; }
            else
            {
                ErrorCode = (string)responseModel["Message"];
                return false;
            }
        }

        public async Task<bool> ValidateTokenCustomerAsync(string token, string idPlace)
        {
            var uri = new Uri(string.Concat(_uriBase, "api/customers/validatetoken"));
            StringContent data = new StringContent(string.Format("name={0}", token), Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, data);

            var result = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK) { return true; }
            else { return false; }
        }

        #endregion

        #region Places

        public async Task<List<PlaceModel>> GetPlacesAsync()
        {
            if (SessionService.Places != null) { return SessionService.Places; }

            var uri = new Uri(string.Concat(_uriBase, "api/places"));

            HttpResponseMessage response = null;
            response = await _client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                List<PlaceModel> placeModels = JArray.Parse(result).Select(x => new PlaceModel
                {
                    Id = (int)x["id_place"],
                    Name = (string)x["name"],
                    Phone = (string)x["phone"],
                    Description = (string)x["description"],
                    Address = (string)x["address"],
                    Longitude = (string)x["longitude"],
                    Latitude = (string)x["latitude"],
                    MaxDaysReservation = (int)x["max_days_reservation"],
                    FormatHour = (int)x["format_hour"],
                    ProfileImgUrl = (string)x["profile_img"],
                    Hours = GetHours(x["hours"])
                }).ToList();

                SessionService.Places = placeModels;
                return placeModels;
            }
            else { return null; }
        }

        List<HourModel> GetHours(JToken json)
        {
            List<HourModel> model = JArray.Parse(json.ToString()).Select(x => new HourModel
            {
                NumberDay = (int)x["id_day"],
                HourStart = FunctionsService.GetHour((string)x["hour_start"]),
                HourEnd = FunctionsService.GetHour((string)x["hour_end"])
            }).ToList();

            return model;
        }

        public async Task<PlaceModel> GetPlaceAsync(int id)
        {
            var uri = new Uri(string.Format("{0}api/places/{1}", _uriBase, id));
            AddAuth();

            HttpResponseMessage response = null;
            response = await _client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                JObject responseModel = JsonConvert.DeserializeObject<JObject>(result.Trim('[', ']'));

                PlaceModel place = new PlaceModel
                {
                    Id = (int)responseModel["id_place"],
                    Name = (string)responseModel["name"],
                    Phone = (string)responseModel["phone"],
                    Description = (string)responseModel["description"],
                    Address = (string)responseModel["address"],
                    Longitude = (string)responseModel["longitude"],
                    Latitude = (string)responseModel["latitude"],
                    MaxDaysReservation = (int)responseModel["max_days_reservation"],
                    FormatHour = (int)responseModel["format_hour"],
                    Hours = GetHours(responseModel["hours"])
                };

                return place;
            }
            else { return null; }
        }

        public async Task<bool> ValidateIfPlaceHasMultiples(int idPlace)
        {
            var uri = new Uri(string.Format("{0}api/places/getplacesismultiple/{1}", _uriBase, idPlace));
            AddAuth();

            HttpResponseMessage response = null;
            response = await _client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                if (result.Contains("True")) { return true; }
                else { return false; }
            }
            else { return false; }
        }

        #endregion

        #region Pitches

        public async Task<List<PitchModel>> GetPitchesSingleAsync(int idPlace)
        {
            var uri = new Uri(string.Format("{0}api/pitches/single/{1}", _uriBase, idPlace));
            AddAuth();

            HttpResponseMessage response = null;
            response = await _client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                List<PitchModel> placeModels = JArray.Parse(result).Select(x => new PitchModel
                {
                    Id = (int)x["id_pitch"],
                    Description = (string)x["description"]
                }).ToList();

                return placeModels;
            }
            else { return null; }
        }

        public async Task<decimal> GetPitchPriceAsync(int id, DateTime date)
        {
            var uri = new Uri(string.Format("{0}api/schedulers/price", _uriBase));
            AddAuth();

            var json = JsonConvert.SerializeObject(new { id_pitch = id, date_insert = date.ToString("yyyy-MM-dd"), hour = date.ToString("HH:mm") });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                if (result != "[]")
                {
                    JObject responseModel = JsonConvert.DeserializeObject<JObject>(result.Trim('[', ']'));
                    return (decimal)responseModel["value"];
                }
            }

            return 0;
        }

        public async Task<List<PitchModel>> GetPitchesMultiplesAsync(int idPlace)
        {
            var uri = new Uri(string.Format("{0}api/pitches/multiple/{1}", _uriBase, idPlace));
            AddAuth();

            HttpResponseMessage response = null;
            response = await _client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                List<PitchModel> placeModels = JArray.Parse(result).Select(x => new PitchModel
                {
                    Id = (int)x["id_pitch"],
                    Description = (string)x["description"]
                }).ToList();

                return placeModels;
            }
            else { return null; }
        }

        #region User

        public async Task<List<PitchModel>> GetPitchesBusySingleAsync(int id, DateTime date)
        {
            var uri = new Uri(string.Format("{0}api/reservations/busy/single", _uriBase));
            AddAuth();

            var json = JsonConvert.SerializeObject(new { id_place = id, date = date.ToString("yyyy-MM-dd") });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                List<PitchModel> placeModels = JArray.Parse(result).Select(x => new PitchModel
                {
                    Id = (int)x["id_pitch"],
                    Hour = FunctionsService.GetHour((string)x["hour"])
                }).ToList();

                return placeModels;
            }
            else { return null; }
        }

        public async Task<List<PitchModel>> GetPitchesMultiplesBusyAsync(int id, DateTime date)
        {
            var uri = new Uri(string.Format("{0}api/reservations/busy/multiple", _uriBase));
            AddAuth();

            var json = JsonConvert.SerializeObject(new { id_place = id, date = date.ToString("yyyy-MM-dd") });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                List<PitchModel> placeModels = JArray.Parse(result).Select(x => new PitchModel
                {
                    Id = (int)x["id_pitch"],
                    Hour = FunctionsService.GetHour((string)x["hour"])
                }).ToList();

                return placeModels;
            }
            else { return null; }
        }

        #endregion

        #region Customer

        public async Task<List<ReservationModel>> GetPitchesSingleBusyCustomerAsync(int id, DateTime date)
        {
            var uri = new Uri(string.Format("{0}api/reservations/customer/busy/single", _uriBase));
            AddAuth();

            var json = JsonConvert.SerializeObject(new { id_place = id, date = date.ToString("yyyy-MM-dd") });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                List<ReservationModel> placeModels = JArray.Parse(result).Select(x => new ReservationModel
                {
                    Pitch = new PitchModel { Id = (int)x["id_pitch"] },
                    Date = FunctionsService.GetHour((string)x["hour"]),
                    Status = FunctionsService.GetStatus((int)x["status"]),
                    IdReservation = (int)x["id_reservation"],
                    Source = (int)x["source"]
                }).ToList();

                return placeModels;
            }
            else { return null; }
        }

        public async Task<List<ReservationModel>> GetPitchesMultiplesBusyCustomerAsync(int id, DateTime date)
        {
            var uri = new Uri(string.Format("{0}api/reservations/customer/busy/multiple", _uriBase));
            AddAuth();

            var json = JsonConvert.SerializeObject(new { id_place = id, date = date.ToString("yyyy-MM-dd") });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                List<ReservationModel> placeModels = JArray.Parse(result).Select(x => new ReservationModel
                {
                    Pitch = new PitchModel { Id = (int)x["id_pitch"] },
                    Date = FunctionsService.GetHour((string)x["hour"]),
                    Status = FunctionsService.GetStatus((int)x["status"]),
                    IdReservation = (int)x["id_reservation"],
                    Source = (int)x["source"]
                }).ToList();

                return placeModels;
            }
            else { return null; }
        }

        #endregion

        #endregion

        #region Reservations

        public async Task<List<ReservationModel>> FindReservationByPlaceAsync(DateTime date, int idPlace)
        {
            var uri = new Uri(string.Format("{0}api/reservations/findfreepitchbyplace", _uriBase));
            AddAuth();

            var json = JsonConvert.SerializeObject
                (new
                {
                    id_place = idPlace,
                    date = date.ToString("yyyy-MM-dd"),
                    hour = date.ToString("HH:mm"),
                });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                List<ReservationModel> reservationModels = JArray.Parse(result).Select(x => new ReservationModel
                {
                    Place = new PlaceModel
                    {
                        Id = (int)x["id_place"],
                        Name = (string)x["name"],
                        Address = (string)x["address"],
                        ProfileImgUrl = (string)x["profile_img"]
                    },
                    Pitch = new PitchModel
                    {
                        Id = (int)x["id_pitch"],
                        Description = (string)x["description"],
                        PitchType = FunctionsService.GetPitchType((int)x["id_pitch_type"])
                    },
                    Price = (decimal)x["value"],
                    Date = FunctionsService.GetDateTime((string)x["date"], (string)x["hour"])
                }).ToList();

                return reservationModels;
            }
            else { return null; }
        }

        public async Task<List<ReservationModel>> FindReservationAsync(DateTime date)
        {
            var uri = new Uri(string.Format("{0}api/reservations/findfreepitch", _uriBase));
            AddAuth();

            var json = JsonConvert.SerializeObject
                (new
                {
                    date = date.ToString("yyyy-MM-dd"),
                    hour = date.ToString("HH:mm"),
                });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                List<ReservationModel> reservationModels = JArray.Parse(result).Select(x => new ReservationModel
                {
                    Place = new PlaceModel
                    {
                        Id = (int)x["id_place"],
                        Name = (string)x["name"],
                        Address = (string)x["address"],
                        ProfileImgUrl = (string)x["profile_img"]
                    },
                    Pitch = new PitchModel
                    {
                        Id = (int)x["id_pitch"],
                        Description = (string)x["description"],
                        PitchType = FunctionsService.GetPitchType((int)x["id_pitch_type"])
                    },
                    Price = (decimal)x["value"],
                    Date = FunctionsService.GetDateTime((string)x["date"], (string)x["hour"])
                }).ToList();

                return reservationModels;
            }
            else { return null; }
        }

        public async Task<bool> CancelReservationAsync(int idReservation)
        {
            var uri = new Uri(string.Format("{0}api/reservations/cancel/{1}", _uriBase, idReservation));
            AddAuth();

            var json = JsonConvert.SerializeObject(new { });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode) { return true; }
            else { return false; }
        }

        public async Task<bool> ReservationUpdateAsync(ReservationModel model)
        {
            var uri = new Uri(string.Format("{0}api/reservations/updateReservation", _uriBase));
            AddAuth();

            var json = JsonConvert.SerializeObject(new { id_reservation = model.IdReservation, status = model.Status, value = model.Price, description = model.Description });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode) { return true; }
            else { return false; }
        }

        public async Task<ReservationModel> GetReservationDetailAsync(int idReservation)
        {
            var uri = new Uri(string.Format("{0}api/reservations/detail/{1}", _uriBase, idReservation));
            AddAuth();

            HttpResponseMessage response = null;
            response = await _client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                JObject responseModel = JsonConvert.DeserializeObject<JObject>(result.Trim('[', ']'));

                ReservationModel reservation = new ReservationModel
                {
                    IdReservation = (int)responseModel["id_reservation"],
                    Date = FunctionsService.GetDateTime((string)responseModel["date"], (string)responseModel["hour"]),
                    Price = (decimal)responseModel["value"],
                    Description = (string)responseModel["reservation_description"],
                    Status = FunctionsService.GetStatus((int)responseModel["status"]),
                    User = new UserModel
                    {
                        Name = (string)responseModel["name"],
                        Phone = (string)responseModel["phone"]
                    },
                    Pitch = new PitchModel
                    {
                        Description = (string)responseModel["description"]
                    }
                };

                return reservation;
            }
            else { return null; }
        }

        #region User

        public async Task<ReservationStatus> DoReservationSingleAsync(ReservationModel model)
        {
            var uri = new Uri(string.Format("{0}api/reservations/single", _uriBase));
            AddAuth();

            var json = JsonConvert.SerializeObject
                (new
                {
                    id_place = model.Place.Id,
                    date = model.Date.ToString("yyyy-MM-dd"),
                    hour = model.Date.ToString("HH:mm"),
                    id_users = SessionService.Account.IdUser,
                    id_pitch = model.Pitch.Id
                });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);
                int value = (int)responseModel["status"];
                if (value == 1) { return ReservationStatus.Pending; }
                else { return ReservationStatus.Ok; }
            }
            else if (response.StatusCode == HttpStatusCode.Conflict) { return ReservationStatus.Conflict; }

            return ReservationStatus.Error;
        }

        public async Task<ReservationStatus> DoReservationMultipleAsync(ReservationModel model)
        {
            var uri = new Uri(string.Format("{0}api/reservations/multiple", _uriBase));
            AddAuth();

            var json = JsonConvert.SerializeObject
                (new
                {
                    id_place = model.Place.Id,
                    date = model.Date.ToString("yyyy-MM-dd"),
                    hour = model.Date.ToString("HH:mm"),
                    id_users = SessionService.Account.IdUser,
                    id_pitch = model.Pitch.Id
                });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                JObject responseModel = JsonConvert.DeserializeObject<JObject>(result);
                int value = (int)responseModel["status"];
                if (value == 1) { return ReservationStatus.Pending; }
                else { return ReservationStatus.Ok; }
            }
            else if (response.StatusCode == HttpStatusCode.Conflict) { return ReservationStatus.Conflict; }

            return ReservationStatus.Error;
        }

        public async Task<List<ReservationModel>> GetMyReservationsAsync()
        {
            var uri = new Uri(string.Format("{0}api/reservations/myreservations/{1}", _uriBase, SessionService.Account.IdUser));
            AddAuth();

            HttpResponseMessage response = null;
            response = await _client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                List<ReservationModel> reservationModels = JArray.Parse(result).Select(x => new ReservationModel
                {
                    IdReservation = (int)x["id_reservation"],
                    Place = new PlaceModel
                    {
                        Id = (int)x["id_place"],
                        Name = (string)x["name_place"],
                        Address = (string)x["address"],
                        ProfileImgUrl = (string)x["profile_img"]
                    },
                    Price = (decimal)x["value"],
                    Status = FunctionsService.GetStatus((int)x["status"]),
                    Date = FunctionsService.GetDateTime((string)x["date"], (string)x["hour"]),
                    Pitch = new PitchModel
                    {
                        Description = (string)x["pitch_description"]
                    }
                }).ToList();

                return reservationModels;
            }
            else { return null; }
        }

        #endregion

        #region Customer

        public async Task<int> DoReservationSingleCustomerAsync(ReservationModel model, int idUserType)
        {
            var uri = new Uri(string.Format("{0}api/reservations/customer/single", _uriBase));
            AddAuth();

            string json = string.Empty;

            #region Create Object

            if (idUserType == 1)
            {
                json = JsonConvert.SerializeObject(new
                {
                    id_place = model.Place.Id,
                    date = model.Date.ToString("yyyy-MM-dd"),
                    hour = model.Date.ToString("HH:mm"),
                    id_users = model.User.IdUser,
                    id_pitch = model.Pitch.Id,
                    value = model.Price,
                    description = model.Description
                });
            }
            else
            {
                json = JsonConvert.SerializeObject(new
                {
                    id_place = model.Place.Id,
                    date = model.Date.ToString("yyyy-MM-dd"),
                    hour = model.Date.ToString("HH:mm"),
                    id_users_offline = model.User.IdUser,
                    id_pitch = model.Pitch.Id,
                    value = model.Price,
                    description = model.Description
                });
            }

            #endregion

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Convert.ToInt32(result);
            }
            return -1;
        }

        public async Task<int> DoReservationMultipleCustomerAsync(ReservationModel model, int idUserType)
        {
            var uri = new Uri(string.Format("{0}api/reservations/customer/multiple", _uriBase));
            AddAuth();

            string json = string.Empty;

            #region Create Object

            if (idUserType == 1)
            {
                json = JsonConvert.SerializeObject(new
                {
                    id_place = model.Place.Id,
                    date = model.Date.ToString("yyyy-MM-dd"),
                    hour = model.Date.ToString("HH:mm"),
                    id_users = model.User.IdUser,
                    id_pitch = model.Pitch.Id,
                    value = model.Price,
                    description = model.Description
                });
            }
            else
            {
                json = JsonConvert.SerializeObject(new
                {
                    id_place = model.Place.Id,
                    date = model.Date.ToString("yyyy-MM-dd"),
                    hour = model.Date.ToString("HH:mm"),
                    id_users_offline = model.User.IdUser,
                    id_pitch = model.Pitch.Id,
                    value = model.Price,
                    description = model.Description
                });
            }

            #endregion

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Convert.ToInt32(result);
            }
            return -1;
        }

        public async Task<List<ReservationModel>> GetPendingReservationsAsync()
        {
            var uri = new Uri(string.Format("{0}api/reservations/customer/myreservations/{1}", _uriBase, SessionCustomerService.Account.IdPlace));
            AddAuth();

            HttpResponseMessage response = null;
            response = await _client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                List<ReservationModel> reservationModels = JArray.Parse(result).Select(x => new ReservationModel
                {
                    IdReservation = (int)x["id_reservation"],
                    User = new UserModel
                    {
                        IdUser = (int)x["id_user"],
                        Name = (string)x["name_user"],
                        Phone = (string)x["phone_user"],
                    },
                    Price = (decimal)x["value"],
                    Status = FunctionsService.GetStatus((int)x["status"]),
                    Date = FunctionsService.GetDateTime((string)x["date"], (string)x["hour"]),
                    Pitch = new PitchModel
                    {
                        Description = (string)x["pitch_description"]
                    }
                }).ToList();

                return reservationModels;
            }
            else { return null; }
        }

        #endregion

        #endregion
    }
}
