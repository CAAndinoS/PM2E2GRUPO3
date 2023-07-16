using Java.Util;
using Newtonsoft.Json;
using PM2E2GRUPO3.Config;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PM2E2GRUPO3.Controller
{
    public class SitioController
    {
        public async static Task<Models.Msg> CreateSit(Models.Sitios sit)
        {
            Models.Msg msg = new Models.Msg();

            String jsonObject = JsonConvert.SerializeObject(sit);
            System.Net.Http.StringContent content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = null;
                response = await client.PostAsync(ConfigProcess.ApiPOST, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    msg = JsonConvert.DeserializeObject<Models.Msg>(result);
                }
            }
            return msg;
        }

        //Microservicio --Read
        public async static Task<List<Models.Sitios>> GetSitios()
        {
            List<Models.Sitios> sit = new List<Models.Sitios>();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = null;
                response = await client.GetAsync(ConfigProcess.ApiGET);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    sit = JsonConvert.DeserializeObject<List<Models.Sitios>>(result);
                }
                return sit;
            }
        }

        public async static Task<Models.Msg> DeleteSit(int id)
        {
            Models.Msg msg = new Models.Msg();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(ConfigProcess.ApiDELETE + "?id=" + id);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    msg = JsonConvert.DeserializeObject<Models.Msg>(result);
                }
            }

            return msg;
        }

        public async static Task<Models.Msg> UpdateSit(Models.Sitios sit)
        {
            Models.Msg msg = new Models.Msg();

            String jsonObject = JsonConvert.SerializeObject(sit);
            System.Net.Http.StringContent content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = null;
                response = await client.PutAsync(ConfigProcess.ApiUPDATE, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    msg = JsonConvert.DeserializeObject<Models.Msg>(result);
                }
            }
            return msg;
        }
    }
}
