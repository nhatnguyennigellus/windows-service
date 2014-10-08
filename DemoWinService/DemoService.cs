using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace DemoWinService
{
    
    public partial class DemoService : ServiceBase
    {
        public DemoService()
        {
            InitializeComponent();
        }

        StreamWriter writer;
        StringBuilder builder;
        string path = @"D:\nKid\upload";
        string accessToken = "mp41-SfOamSXYS2CApwk1PCdS3j4WgP9";
        //string[] filePaths;
        string cardID = "200000000850";
        protected override void OnStart(string[] args)
        {
            writer = new StreamWriter("D:\\log.txt");
            builder = new StringBuilder();
            timer1.Enabled = true;
            timer1.Start();

            builder.AppendLine("* Scanning...");
            //builder.AppendLine(accessToken);
            //builder.AppendLine((makeRequest() == "False") ? "Avatar unavailable!" : "Avatar AVAILABLE");
          //--  scanFiles();
            using (writer)
            {
                writer.Write(builder.ToString());
            }

        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            timer1.Stop();

            builder.AppendLine("* Stop scanning...");

            using (writer)
            {
                writer.Write(builder.ToString());
            }
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            writer = new StreamWriter("D:\\log.txt");
            timer1.Enabled = true;
            timer1.Stop();
            
          //--  scanFiles();
            
            //getAccessToken(); 

            
            builder.AppendLine("----- " + DateTime.Now + " -----");
          //  builder.AppendLine(accessToken);
         //   builder.AppendLine((makeRequest() == "False") ? "Avatar unavailable!" : "Avatar AVAILABLE");
            using (writer)
            {
                writer.Write(builder.ToString());
            }



            timer1.Start();
        }

        private string makeRequest()
        {
            var client = new RestClient();
            var request =
                new RestRequest("http://sandbox.tinizen.com/api/cards/" + cardID + "?populate=owner",
                    Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + accessToken);

            RestResponse response = (RestResponse)client.Execute(request);
            if (response != null &&
                    ((response.StatusCode == HttpStatusCode.OK) &&
                    (response.ResponseStatus == ResponseStatus.Completed)))
            {
                JObject obj = JObject.Parse(response.Content);

                return (string)obj["owner"]["profile_picture"]["user_uploaded"];
            }
            return "ID not found";
        }
        private void scanFiles()
        {
            string[] filePaths = Directory.GetFiles(path, "*.jpg");
            if (Directory.Exists(path))
            {
                builder.AppendLine("----- Scan result at " + DateTime.Now + " -----");
                foreach (string file in filePaths)
                {
                    string fileName = file.Substring(path.Length + 1);
                    builder.AppendLine(fileName);

                }
            }
        }

        private void getAccessToken()
        {
            try
            {
                var client = new RestClient();
                var request =
                    new RestRequest("http://sandbox.tinizen.com/api/oauth/token",
                        Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Accept", "application/json");
                request.AddBody(new
                {
                    grant_type = ConfigurationManager.AppSettings["grant_type"],
                    client_id = ConfigurationManager.AppSettings["client_id"],
                    client_secret = ConfigurationManager.AppSettings["client_secret"],
                    name = ConfigurationManager.AppSettings["name"],
                    scope = ConfigurationManager.AppSettings["scope"]
                });

                RestResponse response = (RestResponse)client.Execute(request);

                if (response != null &&
                    ((response.StatusCode == HttpStatusCode.OK) &&
                    (response.ResponseStatus == ResponseStatus.Completed)))
                {
                    JObject obj = JObject.Parse(response.Content);
                    this.accessToken = (string)obj["access_token"];

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        
    }
}
