using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;

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

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            sc.Start();
            //MessageBox.Show("Service is running...");
            sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running);
            sttService.Text = "Service is running...";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped);

            sc.Stop();
            //MessageBox.Show("Service is closing...");
            sttService.Text = "Service is closing...";
        }
    }
}
