using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

        public static async Task<bool> GetIsLinked(string id)
        {
            string result = string.Empty;
            var http = new HttpClient();
            var url = String.Format("http://martinbaud.com/V1/isLinked.php?id=" + id);
            var response = await http.GetAsync(url);
            result = await response.Content.ReadAsStringAsync();
            if (result == "1")
            {
                return true;
            }
            return false;
        }
    }
}
