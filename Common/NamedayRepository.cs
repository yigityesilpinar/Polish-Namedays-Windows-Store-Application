using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    // Access to server and cache the data 
    public static class NamedayRepository
    {
        private static List<NamedayModel> allNamedaysCache;

        // download json from server and serialize it to List<NamedayModel>
        // ASYNC METHOD, Return value is wrapped inside a task object to enable async

        public static async Task<List<NamedayModel>> GetAllNamedaysAsync()
        {
            // to ensure the not download the data more than once per session
            if (allNamedaysCache != null)
                return allNamedaysCache;

            var client = new HttpClient();
            // await says to method to wait until all the chunks of the stream is gotten
            var stream = await client.GetStreamAsync("http://yigityesilpinar.com/namedays.json");


            // once the stream is ready
            // Json serializer
            var serializer = new DataContractJsonSerializer(typeof(List<NamedayModel>));
            // Perform serialization 
            allNamedaysCache = (List<NamedayModel>)serializer.ReadObject(stream);

            return allNamedaysCache;
        }

        public static async Task<string> GetTodaysNamesAsStringAsync() {
            var allNamedays = await GetAllNamedaysAsync();
            var todaysNamedays = allNamedays.Find
                (d => d.Day == DateTime.Now.Day && d.Month == DateTime.Now.Month);

            return todaysNamedays?.NamesAsString;
        }
    }
}
