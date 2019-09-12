using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace DataAccessLibrary.API
{
    public static class APIManager
    {
        public static async Task<string> GetMirorId()
        {
            string result = string.Empty;
            var http = new HttpClient();
            var url = String.Format("http://www.martinbaud.com/V1/generateId.php");
            var response = await http.GetAsync(url);
            result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public static async Task<Tuple<bool, string>> GetIsLinked(string id)
        {
            var http = new HttpClient();
            var url = String.Format("http://martinbaud.com/V1/isLinked.php?id=" + id);
            var response = await http.GetAsync(url);
            var responJsonText = await response.Content.ReadAsStringAsync();
            Dictionary<string, string> output = JsonConvert.DeserializeObject<Dictionary<string, string>>(responJsonText);
            return new Tuple<bool, string>(output["isLinked"] == "0" ? false : true, output["email"]);
        }

        public static async Task<List<string>> GetUserProfiles(string userMail)
        {
            List<string> ret = new List<string>();
            string result = string.Empty;
            var http = new HttpClient();
            var url = String.Format("http://martinbaud.com/V1/get_profile.php?email=" + userMail);
            var response = await http.GetAsync(url);
            result = await response.Content.ReadAsStringAsync();
            var root = JsonObject.Parse(result);
            JsonObject obj = root.GetObject();
            JsonArray array =  root.GetNamedArray("profil");
           

            for (int i = 0; i < array.Count; i++)
            {
                JsonObject array2 = array.GetObjectAt((uint)i);
                ret.Add(array2["profil"].GetString());
            }
            return ret;
        }
    }
}
