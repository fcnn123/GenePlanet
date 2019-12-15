﻿using GP_Dal.Models;
using GP_RestService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GP_Console
{
    public class Program
    {
        private static Logger _logger;

        static void Main(string[] args)
        {            
            if (Properties.Settings.Default.log_mode == "eventlog")
            {
                _logger = new Logger(Logger.LogMode.eventlog);
            }
            else
            {
                _logger = new Logger(Logger.LogMode.console);
            }

            Uri baseAddress = new Uri(string.Format("http://localhost:{0}/", Properties.Settings.Default.localhost_port));

            using (WebServiceHost host = new WebServiceHost(typeof(GP_service), baseAddress))
            {
                host.Open();

                Console.WriteLine("The service is ready at {0}AggData/{1}/{1}", baseAddress, "{username}", "{param}");
                Console.WriteLine("Parameter {0} is unique user identifier.", "{username}");
                Console.WriteLine("Possible values for parameter {0} are \"all\",\"min\" or \"max\"", "{param}");
                Console.WriteLine("Press <Enter> to stop the service.");

                using (Timer t = new Timer(5000))
                {
                    t.Elapsed += DoMetricSyncHandler;
                    t.Start();
                    //DoMetricSync();

                    Console.ReadLine();
                }
                host.Close();
            }
        }

        private static void DoMetricSyncHandler(object sender, ElapsedEventArgs e)
        {
            Task.Run(async () => await DoMetricSync());
        }

        private static async Task DoMetricSync()
        {
            if (_logger.Mode == Logger.LogMode.console)
                Console.WriteLine(""); //For more readable console log

            string username = Properties.Settings.Default.username;
            string shimmer_host = Properties.Settings.Default.shimmer_host;

            _logger.Log(string.Format("Sync started for username \"{0}\"", username));            

            float[] minmax = await GetOnlineData(username, shimmer_host, _logger);

            _logger.Log(string.Format("Sync completed for username \"{0}\"", username));
        }

        public static async Task<float[]> GetOnlineData(string username, string shimmer_host, Logger logger)
        {
            string[] shims = new string[] { "fitbit", "ihealth" };

            MHealth_body_weight mHealth;
            float min = float.MaxValue;
            float max = float.MinValue;

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

                            if (min > weight)
                                min = weight;
                            if (max < weight)
                                max = weight;
                        }

                        logger.Log(string.Format("Sync for shim \"{0}\" completed.", shim));
                    }
                    catch (HttpRequestException eReq)
                    {
                        logger.Log(string.Format("Error on request call for shim \"{0}\": {1}", shim, eReq.Message), EventLogEntryType.Warning);
                    }
                    catch (Exception e)
                    {
                        logger.Log(string.Format("Error on sync for shim \"{0}\": {1}", shim, e.Message), EventLogEntryType.Error);
                    }
                }
            }

            return new float[] { min, max };
        }
    }

    public class Logger
    {
        private readonly EventLog _log;
        public LogMode Mode { get; }

        public enum LogMode
        {
            console,
            eventlog
        }

        public Logger(LogMode logMode)
        {
            Mode = logMode;
            if (Mode == LogMode.eventlog)
            {
                if (!EventLog.SourceExists("GP_Service"))
                {
                    EventLog.CreateEventSource("GP_Service", "GP_MetricSync");
                    _log = new EventLog { Source = "GP_Service" };
                }
                else
                    _log = new EventLog { Source = "GP_Service" };
            }
        }

        public void Log(string message, EventLogEntryType type = EventLogEntryType.Information)
        {
            if (Mode == LogMode.eventlog)
                _log.WriteEntry(message, type);
            else
                Console.WriteLine("{0} {1}", type.ToString(), message);
        }
    }
}
