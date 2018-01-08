using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Android.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace To_Do_List
{
    class ToDoServices
    {
        // APIARY Mock server test feed 
        private const string URL = "http://georgyxamarine1api.000webhostapp.com/public";
        private const string GET_POIS = URL + "/getdata";
        private const string CREATE_POI = URL + "/insert";
        private const string DELETE_POI = URL + "/delete/";

        //private const string GET_POIS = "http://<YOUR_SERVER_IP>:8080/com.packt.poiapp/api/poi/pois";
        //private const string CREATE_POI = "http://<YOUR_SERVER_IP>:8080/com.packt.poiapp/api/poi/create";
        //private const string DELETE_POI = "http://<YOUR_SERVER_IP>:8080/com.packt.poiapp/api/poi/delete/{0}";

        public async Task<List<ToDo>> GetToDoListAsync()
        {
            HttpClient httpClient = new HttpClient();

            // Adding Accept-Type as application/json header
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await httpClient.GetAsync(GET_POIS);
            if (response != null || response.IsSuccessStatusCode)
            {

                // await! control returns to the caller and the task continues 
                string content = await response.Content.ReadAsStringAsync();

                // Printing response body in console 
                Console.Out.WriteLine("Response Body: \r\n {0}", content);

                // Initialize the poi list 
                var toDoListData = new List<ToDo>();

                // Load a JObject from response string
                JArray jsonResponse = JArray.Parse(content);
                IList<JToken> results = jsonResponse.ToList();
                foreach (JToken token in results)
                {
                    ToDo toDo = token.ToObject<ToDo>();
                    toDoListData.Add(toDo);
                }

                return toDoListData;
            }
            else
            {
                Console.Out.WriteLine("Failed to fetch data. Try again later!");
                return null;
            }

        }

        public bool isConnected(Context activity)
        {
            var connectivityManager = (ConnectivityManager)activity.GetSystemService(Context.ConnectivityService);
            var activeConnection = connectivityManager.ActiveNetworkInfo;
            return (null != activeConnection && activeConnection.IsConnected);
        }

        
        public async Task<String> CreateOrUpdateToDoAsync(ToDo toDo)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new POIContractResolver();
            var toDoJson = JsonConvert.SerializeObject(toDo, Formatting.Indented, settings);

            HttpClient httpClient = new HttpClient();
            StringContent jsonContent = new StringContent(toDoJson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(CREATE_POI, jsonContent);

            if (response != null || response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine("{0} saved.", toDo.heading);

                return content;
            }
            return null;
        }
        
        /**
		 * Converts all json keys into lowercase
		 */
       public class POIContractResolver : DefaultContractResolver
       {
           protected override string ResolvePropertyName(string key)
           {
               return key.ToLower();
           }
       }
        

       public async Task<String> DeleteToDoAsync(int toDoId)
       {
           HttpClient httpClient = new HttpClient();
           String url = DELETE_POI+toDoId;
           Console.Out.WriteLine("-----------url :{0}. ID : {1}", url, toDoId);
           //HttpResponseMessage response = await httpClient.DeleteAsync(url);
           HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response != null || response.IsSuccessStatusCode)
           {
               string content = await response.Content.ReadAsStringAsync();
               Console.Out.WriteLine("One record deleted.");
               return content;
           }
           return null;
       }
       
    }
}