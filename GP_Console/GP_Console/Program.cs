using GP_Dal.Models;
using GP_RestService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace GP_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri(string.Format("http://localhost:{0}/", Properties.Settings.Default.localhost_port));

            using (WebServiceHost host = new WebServiceHost(typeof(GP_service), baseAddress))
            {
                host.Open();

                Console.WriteLine("The service is ready at {0}AggData/{1}/{1}", baseAddress, "{username}", "{param}");
                Console.WriteLine("Parameter {0} is unique user identifier.", "{username}");
                Console.WriteLine("Possible values for parameter {0} are \"all\",\"min\" or \"max\"", "{param}");
                Console.WriteLine("Press <Enter> to stop the service.");

                GetOnlineData("7YQRY6", "localhost:8083");

                Console.ReadLine();

                host.Close();
            }
        }

        public static async Task GetOnlineData(string username, string shimmer_host)
        {
            Console.WriteLine("");

            string[] shims = new string[] { "fitbit", "ihealth" };

            MHealth_body_weight mHealth = null;
            using (var client = new HttpClient())
            {
                DateTime t = DateTime.Today;
                foreach (var shim in shims)
                {
                    try
                    {
                        var response = await client.GetAsync(string.Format("http://{0}/data/{1}/body_weight?username={2}&dateStart={3}&dateEnd={3}&normalize=true", shimmer_host, shim, username, t.ToString("yyy-MM-dd")));
                        response.EnsureSuccessStatusCode();

                        var jsonString = response.Content.ReadAsStringAsync();
                        mHealth = JsonConvert.DeserializeObject<MHealth_body_weight>(jsonString.Result);

                        foreach (var item in mHealth.body)
                        {
                            float weight = 0;
                            if (item.body.body_weight.unit == "kg")
                                weight = item.body.body_weight.value;
                            else
                                weight = item.body.body_weight.value / 2.2046F;
                            Console.WriteLine(string.Format("Shim \"{0}\" ; Weight: {1}", shim, weight));
                        }                        
                    }
                    catch (HttpRequestException eReq)
                    {
                        Console.WriteLine(string.Format("Error on request call for shim \"{0}\": {1}", shim, eReq.Message));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(string.Format("Error on sync for shim \"{0}\": {1}", shim, e.Message));
                    }
                }
            }            
        }
    }
}
