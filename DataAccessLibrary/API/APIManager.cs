using DataAccessLibrary.Entitites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace DataAccessLibrary.API
{
    public static class APIManager
    {
        private const string ConnectionString = @"https://neomirapi3.azurewebsites.net/api/";

        #region FONCTIONS

        // Recevoir l'identifiant du miroir
        public static async Task<string> GetMirorId()
        {
            string result = string.Empty;
            try
            {
                var http = new HttpClient();
                var url = String.Format("http://www.martinbaud.com/V1/generateId.php");
                var response = await http.GetAsync(url);
                result = await response.Content.ReadAsStringAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return result;
        }

        //// Recevoir si l'identifiant est déjà lié ou non
        //public static async Task<Tuple<bool, string>> GetIsLinked(string id)
        //{
        //    try
        //    {
        //        var http = new HttpClient();
        //        var url = String.Format("http://martinbaud.com/V1/isLinked.php?id=" + id);
        //        var response = await http.GetAsync(url);
        //        var responJsonText = await response.Content.ReadAsStringAsync();
        //        Dictionary<string, string> output = JsonConvert.DeserializeObject<Dictionary<string, string>>(responJsonText);
        //        return new Tuple<bool, string>(output["isLinked"] == "0" ? false : true, output["email"]);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        return null;
        //    }
        //}

        //// Recevoir les profils 
        //public static async Task<List<string>> GetUserProfiles(string userMail)
        //{
        //    List<string> ret = new List<string>();
        //    try
        //    {
        //        string result = string.Empty;
        //        var http = new HttpClient();
        //        var url = String.Format("http://martinbaud.com/V1/get_profile.php?email=" + userMail);
        //        var response = await http.GetAsync(url);
        //        result = await response.Content.ReadAsStringAsync();
        //        var root = JsonObject.Parse(result);
        //        JsonObject obj = root.GetObject();
        //        JsonArray array = root.GetNamedArray("profil");


        //        for (int i = 0; i < array.Count; i++)
        //        {
        //            JsonObject array2 = array.GetObjectAt((uint)i);
        //            ret.Add(array2["profil"].GetString());
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    return ret;
        //}

        //// Recevoir les applications d'un profil
        //public static async Task<List<UserApp>> GetProfileApps(string userMail, int profilId)
        //{
        //    List<UserApp> ret = new List<UserApp>();

        //    try
        //    {
        //        string result = string.Empty;
        //        var http = new HttpClient();
        //        var url = String.Format("http://martinbaud.com/V1/getAppListFromProfil.php?email={0}&id_profil={1}", userMail, profilId);
        //        var response = await http.GetAsync(url);
        //        result = await response.Content.ReadAsStringAsync();
        //        var root = JsonArray.Parse(result);
        //       // JsonObject obj = root.GetObject();
        //        JsonArray array = root.GetArray();

        //        for (int i = 0; i < array.Count; i++)
        //        {
        //            UserApp app = new UserApp();
        //            JsonObject array2 = array.GetObjectAt((uint)i);
        //            app.AppName = array2["APP_NAME"].GetString();
        //            app.AppLink = array2["ENTRY_LINK"].GetString();
        //            app.AppIconLink = string.Format("ms-appx:///Assets/AppsPage/{0}.png", app.AppName);
        //            ret.Add(app);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    return ret;
        //}

        public static async Task<List<UserApp>> GetProfileApps(long profilId)
        {
            string result = string.Empty;
            HttpResponseMessage response = null;
            try
            {
                var http = new HttpClient();
                var url = String.Format("{0}ProfileApps/GetAppsFromProfileId/{1}", ConnectionString, profilId);
                response = await http.GetAsync(url);
                result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<UserApp>>(result);
            }
            catch (Exception e)
            {
                throw new APIException(response) { Inner = e };
            }
        }

        public static async Task<List<Profile>> GetUserProfiles(long userId)
        {
            string result = string.Empty;
            HttpResponseMessage response = null;
            try
            {
                var http = new HttpClient();
                var url = String.Format("{0}Profiles/GetProfileFromUserId?id={1}", ConnectionString, userId);
                response = await http.GetAsync(url);
                result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Profile>>(result);
            }
            catch (Exception e)
            {
                throw new APIException(response) { Inner = e };
            }
        }

        public static async Task<Miror> PostMiror(Miror miror)
        {
            string result = string.Empty;
            HttpResponseMessage response = null;
            try
            {
                var json = JsonConvert.SerializeObject(miror);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var http = new HttpClient();
                var url = String.Format("{0}Mirors", ConnectionString);
                response = await http.PostAsync(url, content);
                result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Miror>(result);
            }
            catch (Exception e)
            {
                throw new APIException(response) { Inner = e };
            }
        }


        public static async Task<Miror> GetIsLinked(long mirorId)
        {
            string result = string.Empty;
            HttpResponseMessage response = null;
            try
            {
                var http = new HttpClient();
                var url = String.Format("{0}Mirors/{1}", ConnectionString, mirorId);
                response = await http.GetAsync(url);
                result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Miror>(result);
            }
            catch (Exception e)
            {
                throw new APIException(response) { Inner = e };
            }
        }


        public static async Task<Miror> GetMirorToken(long mirorId)
        {
            string result = string.Empty;
            HttpResponseMessage response = null;
            try
            {
                var http = new HttpClient();
                var url = String.Format("{0}Mirors/GetToken/{1}", ConnectionString, mirorId);
                response = await http.GetAsync(url);
                result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Miror>(result);
            }
            catch (Exception e)
            {
                throw new APIException(response) { Inner = e };
            }
        }

        #endregion
    }
}
