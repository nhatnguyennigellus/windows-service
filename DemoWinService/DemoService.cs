using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Windows.Forms;

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
        string path = @"D:\nKid";
        protected override void OnStart(string[] args)
        {
            writer = new System.IO.StreamWriter("D:\\log.txt");
            builder = new StringBuilder();
            timer1.Enabled = true;
            timer1.Start();
            builder.AppendLine("* Scanning " + path + "...");

            //MessageBox.Show("Running...");
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

            //MessageBox.Show("Stop!");
            using (writer)
            {
                writer.Write(builder.ToString());
            }
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            //MessageBox.Show("Scanning...");
            writer = new System.IO.StreamWriter("D:\\log.txt");
            timer1.Enabled = true;
            timer1.Stop();
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

            using (writer)
            {
                writer.Write(builder.ToString());
            }
            timer1.Start();
        }
    }
}
