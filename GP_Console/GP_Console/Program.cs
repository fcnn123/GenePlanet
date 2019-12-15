using GP_RestService;
using System;
using System.Collections.Generic;
using System.Linq;
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

                Console.ReadLine();

                host.Close();
            }
        }
    }
}
