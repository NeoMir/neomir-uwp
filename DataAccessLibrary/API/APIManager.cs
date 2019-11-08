using DataAccessLibrary.Entitites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace DataAccessLibrary.API
{
    public static class APIManager
    {
        /// <summary>
        /// Recevoir l'identifiant du miroir
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Recevoir si l'identifiant est déjà lié ou non
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Tuple<bool, string>> GetIsLinked(string id)
        {
            try
            {
                var http = new HttpClient();
                var url = String.Format("http://martinbaud.com/V1/isLinked.php?id=" + id);
                var response = await http.GetAsync(url);
                var responJsonText = await response.Content.ReadAsStringAsync();
                Dictionary<string, string> output = JsonConvert.DeserializeObject<Dictionary<string, string>>(responJsonText);
                return new Tuple<bool, string>(output["isLinked"] == "0" ? false : true, output["email"]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Recevoir les profils 
        /// </summary>
        /// <param name="userMail"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetUserProfiles(string userMail)
        {
            List<string> ret = new List<string>();
            try
            {
                string result = string.Empty;
                var http = new HttpClient();
                var url = String.Format("http://martinbaud.com/V1/get_profile.php?email=" + userMail);
                var response = await http.GetAsync(url);
                result = await response.Content.ReadAsStringAsync();
                var root = JsonObject.Parse(result);
                JsonObject obj = root.GetObject();
                JsonArray array = root.GetNamedArray("profil");


                for (int i = 0; i < array.Count; i++)
                {
                    JsonObject array2 = array.GetObjectAt((uint)i);
                    ret.Add(array2["profil"].GetString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ret;
        }

        /// <summary>
        /// Recevoir les applications d'un profil
        /// </summary>
        /// <param name="userMail"></param>
        /// <param name="profilId"></param>
        /// <returns></returns>
        public static async Task<List<UserApp>> GetProfileApps(string userMail, int profilId)
        {
            List<UserApp> ret = new List<UserApp>();

            try
            {
                string result = string.Empty;
                var http = new HttpClient();
                var url = String.Format("http://martinbaud.com/V1/getAppListFromProfil.php?email={0}&id_profil={1}", userMail, profilId);
                var response = await http.GetAsync(url);
                result = await response.Content.ReadAsStringAsync();
                var root = JsonArray.Parse(result);
               // JsonObject obj = root.GetObject();
                JsonArray array = root.GetArray();

                for (int i = 0; i < array.Count; i++)
                {
                    UserApp app = new UserApp();
                    JsonObject array2 = array.GetObjectAt((uint)i);
                    app.AppName = array2["APP_NAME"].GetString();
                    app.AppLink = array2["ENTRY_LINK"].GetString();
                    app.AppIconLink = string.Format("ms-appx:///Assets/AppsPage/{0}.png", app.AppName);
                    ret.Add(app);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ret;
        }
    }
}
