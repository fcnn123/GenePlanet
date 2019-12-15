using Microsoft.VisualStudio.TestTools.UnitTesting;
using GP_RestService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using GP_Dal.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace GP_RestService.Tests
{
    [TestClass()]
    public class GP_serviceTests
    {
        [TestMethod()]
        public async Task GetAggDataTest()
        {
            Uri baseAddress = new Uri("http://localhost:5080/");
            string username = "7YQRY6";
            string mode = "all";

            using (WebServiceHost host = new WebServiceHost(typeof(GP_service), baseAddress))
            {
                host.Open();

                try
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(string.Format("{0}AggData/{1}/{2}", baseAddress, username, mode));
                        response.EnsureSuccessStatusCode();

                        var jsonString = response.Content.ReadAsStringAsync();
                        AggData aggData = JsonConvert.DeserializeObject<AggData>(jsonString.Result);

                        Assert.IsNotNull(aggData);
                    }

                }
                catch (HttpRequestException eReq)
                {
                    Assert.Fail(eReq.Message);
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }

                host.Close();
            }
        }
    }
}