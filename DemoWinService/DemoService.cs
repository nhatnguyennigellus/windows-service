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

namespace DemoWinService
{
    public class MyWebRequest
    {
        private WebRequest request;
        private Stream dataStream;

        private string status;

        public String Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        public MyWebRequest(string url)
        {
            // Create a request using a URL that can receive a post.

            request = WebRequest.Create(url);
        }

        public MyWebRequest(string url, string method)
            : this(url)
        {

            if (method.Equals("GET") || method.Equals("POST"))
            {
                // Set the Method property of the request to POST.
                request.Method = method;
            }
            else
            {
                throw new Exception("Invalid Method Type");
            }
        }

        public MyWebRequest(string url, string method, string data)
            : this(url, method)
        {

            // Create POST data and convert it to a byte array.
            string postData = data;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            dataStream = request.GetRequestStream();

            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);

            // Close the Stream object.
            dataStream.Close();

        }

        public string GetResponse()
        {
            // Get the original response.
            WebResponse response = request.GetResponse();

            this.Status = ((HttpWebResponse)response).StatusDescription;

            // Get the stream containing all content returned by the requested server.
            dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);

            // Read the content fully up to the end.
            string responseFromServer = reader.ReadToEnd();

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }
    }
    public partial class DemoService : ServiceBase
    {
        public DemoService()
        {
            InitializeComponent();
            
        }

        System.IO.StreamWriter writer;
        StringBuilder builder;
        string path = @"D:\nKid";
        WebRequest request;
        protected override void OnStart(string[] args)
        {
            writer = new System.IO.StreamWriter("D:\\log.txt");
            builder = new StringBuilder();
            timer1.Enabled = true;
            timer1.Start();

            // Send request to server
            //request = WebRequest.Create("http://192.168.12.250:8889/ping");
            //request.Credentials = CredentialCache.DefaultCredentials;
            //((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";
            //request.Method = "GET";
            ////string postData = "Test.";
            ////byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            ////request.ContentType = "application/x-www-form-urlencoded";
            ////request.ContentLength = byteArray.Length;
            //Stream dataStream = request.GetRequestStream();
            ////dataStream.Write(byteArray, 0, byteArray.Length);
            //dataStream.Close();
            //WebResponse response = request.GetResponse();
            //dataStream = response.GetResponseStream();
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            //StreamReader reader = new StreamReader(dataStream);
            //string responseFromServer = reader.ReadToEnd();
            //builder.AppendLine(responseFromServer);
            //reader.Close();
            //dataStream.Close();
            //response.Close();
            MyWebRequest request = new MyWebRequest("http://192.168.12.250:8889/ping", "GET");

            builder.AppendLine("Connect to server - " + request.GetResponse());
            builder.AppendLine("* Scanning " + path + "...");
            scanFiles();
            using (writer)
            {
                writer.Write(builder.ToString());
            }

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
            timer1.Enabled = true;
            timer1.Stop();
            writer = new System.IO.StreamWriter("D:\\log.txt");

            scanFiles();

            using (writer)
            {
                writer.Write(builder.ToString());
            }
            timer1.Start();
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
    }
}
