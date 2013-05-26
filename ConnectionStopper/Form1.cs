using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Threading;
using System.Diagnostics;



namespace ConnectionStopper
{
    public partial class Form1 : Form
    {
        int miliseconds;
        DateTime time;
        TimeSpan toGo;
        
        public Form1()
        {
            InitializeComponent();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            miliseconds = (int)(numericUpDown1.Value) * 1000 * 60 * 60 + (int)numericUpDown2.Value * 1000 * 60 + (int)numericUpDown3.Value * 1000;
            time = DateTime.Now;
            time = time.AddMilliseconds(miliseconds);
            Thread t1 = new Thread(StartStop);
            t1.Start();
            timer1.Start();
        }

        private void StartStop()
        {
            connectionSwitch("Disable");
            Thread.Sleep(miliseconds);
            connectionSwitch("Enable");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int hours, minutes, seconds;
            if (time.CompareTo(DateTime.Now) <= 0)
            {
                timer1.Stop();
                label5.Text = "0h 0m 0s";
            }
            else
            {
                toGo = time.Subtract(DateTime.Now);
                seconds = (int)toGo.Seconds;
                minutes = (int)toGo.Minutes;
                hours = (int)toGo.Hours;
                label5.Text = hours + "h " + minutes + "m " + seconds + "s";
            }
        }

        /// <summary>
        /// Enables or disables all available connections
        /// </summary>
        /// <param name="operation">"Enable" or "Disable" available connections</param>
        private void connectionSwitch(string operation)
        {
            var wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter " +
                                              "WHERE NetConnectionId != null " +
                                              "AND Manufacturer != 'Microsoft' ");
            using (var searcher = new ManagementObjectSearcher(wmiQuery))
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    if (((String)item["NetConnectionId"]) == "Local Area Connection")
                    {
                        using (item)
                        {
                            item.InvokeMethod(operation, null);
                        }
                    }
                }
            }
        }
    }
}
