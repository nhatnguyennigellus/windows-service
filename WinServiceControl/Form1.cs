using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;
using DemoWinService;
using RestSharp;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp.Deserializers;

namespace WinServiceControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ServiceController sc = new ServiceController("DemoService", "NigellusNguyen");
        private void Form1_Load(object sender, EventArgs e)
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
                    grant_type = "client_credentials",
                    client_id = "example_id",
                    client_secret = "7cbe80709c9eab855e14ac10adc250ea",
                    name = "tiNi example clients",
                    scope = "*"
                });

                RestResponse response = (RestResponse)client.Execute(request);

                if (response != null &&
                    ((response.StatusCode == HttpStatusCode.OK) &&
                    (response.ResponseStatus == ResponseStatus.Completed)))
                {
                    JObject obj = JObject.Parse(response.Content);
                    accessToken = (string)obj["access_token"];
                    sttService.Text = (makeRequest() == "False") ? "Avatar unavailable!" : "Avatar AVAILABLE";
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            
            //var content = response.Content;
            //MessageBox.Show(response.Content); 
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
           // sc.Start();
            sttService.Text = "Connecting to server...";
            
            //MessageBox.Show("Service is running...");
            //sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running);
            //sttService.Text = "Service is running...";
            //request = new MyWebRequest("http://192.168.12.250:8889/ping", "GET");
            //sttService.Text = request.GetResponse();


        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped);

            sc.Stop();
            //MessageBox.Show("Service is closing...");
            sttService.Text = "Service is closing...";
        }

        string cardID = "200000000850";
        string accessToken;
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
    }

    
}
