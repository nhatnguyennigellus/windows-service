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

        System.IO.StreamWriter writer;
        StringBuilder builder;
        string path = @"D:\nKid\upload";
        string accessToken;
        string[] filePaths;
        string cardID = "200000000850";
        protected override void OnStart(string[] args)
        {
            writer = new System.IO.StreamWriter("D:\\log.txt");
            builder = new StringBuilder();
            

            timer1.Enabled = true;
            timer1.Start();
            
            //builder.AppendLine("* Scanning " + path + "...");
            //  scanFiles();
            //using (writer)
            //{
            //    writer.Write(builder.ToString());
            //}

        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            timer1.Stop();

            builder.AppendLine("* Stop scanning " + path + "...");

            using (writer)
            {
                writer.Write(builder.ToString());
            }
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            getAccessToken();
            timer1.Enabled = true;
            timer1.Stop();
            writer = new System.IO.StreamWriter("D:\\log.txt");

            //scanFiles();

            builder.AppendLine(makeRequest());
            using (writer)
            {
                writer.Write(builder.ToString());
            }



            timer1.Start();
        }

        private void scanFiles()
        {
            filePaths = Directory.GetFiles(path, "*.jpg");
            if (Directory.Exists(path))
            {
                //builder.AppendLine("----- Scan result at " + DateTime.Now + " -----");
                foreach (string file in filePaths)
                {
                    string fileName = file.Substring(path.Length + 1);
                  //  builder.AppendLine(fileName);

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

        private string makeRequest()
        {
            var client = new RestClient();
            var request =
                new RestRequest("http://sandbox.tinizen.com/api/cards/" + cardID + "?populate=owner",
                    Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + accessToken);

            RestResponse<Owner> response = (RestResponse<Owner>)client.Execute(request);
            RestResponse res = (RestResponse)client.Execute(request);
            if (response != null &&
                    ((response.StatusCode == HttpStatusCode.OK) &&
                    (response.ResponseStatus == ResponseStatus.Completed)))
            {
                //JObject obj = JObject.Parse(response.Content);
               // this.accessToken = (string)obj["access_token"];
                JObject obj = JObject.Parse(res.Content);
                string status = (string)obj["status"];
                if (status != null && status == "404")
                {
                    return "Not found";
                }
                obj = JObject.Parse(response.Content);
                var firstName = response.Data.first_name;
                return firstName;
            }
            return null;
        }
    }
}
