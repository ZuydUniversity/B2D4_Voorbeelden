using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WindowsFormsAppExamplesRWA
{
    public partial class FormIoT : Form
    {

        public FormIoT()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpWebRequest request = WebRequest.Create("http://192.168.78.21:8080/json.htm?type=command&param=switchlight&idx=1&switchcmd=On") as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            string text = reader.ReadToEnd();
            textBox1.Text = text;

            // json (de)serialiser
            // System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Message?>(text?);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HttpWebRequest request 
                = WebRequest.Create("http://192.168.78.21:8080/json.htm?type=command&param=switchlight&idx=1&switchcmd=Off") 
                    as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            string text = reader.ReadToEnd();
            textBox2.Text = text;
        }

        // -----------------------------------
        // Threading
        // -----------------------------------

        ThreadingClass tc = new ThreadingClass(0);

        private void button3_Click(object sender, EventArgs e)
        {
            tc.WaitThread5();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(tc.WaitThread5);
            t1.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tc.WaitThread5();
            tc.WaitThread5();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Middels Thread
            Thread t1 = new Thread(tc.WaitThread10);
            t1.Start();
            Thread t2 = new Thread(tc.WaitThread5);
            t2.Start();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Middels ThreadStart
            ThreadStart threadDelegate = new ThreadStart(tc.WaitThread10);
            Thread t1 = new Thread(threadDelegate);
            t1.Start();
            threadDelegate = new ThreadStart(tc.WaitThread5);
            Thread t2 = new Thread(threadDelegate);
            t2.Start();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Middels ThreadStart
            tc.SetWaitTime(10);
            ThreadStart threadDelegate = new ThreadStart(tc.WaitThreadX);
            Thread t1 = new Thread(threadDelegate);
            t1.Start();
            tc.SetWaitTime(3);
            ThreadStart threadDelegate2 = new ThreadStart(tc.WaitThreadX);
            Thread t2 = new Thread(threadDelegate2);
            t2.Start();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(tc.WaitThreadVar);
            t1.Start(10);
            Thread t2 = new Thread(tc.WaitThreadVar);
            t2.Start(3);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(tc.WaitThreadVar);
            t1.Start(12);
            Thread t2 = new Thread(tc.WaitThreadVar);
            t2.Start(9);
            Thread t3 = new Thread(tc.WaitThreadVar);
            t3.Start(6);
            Thread t4 = new Thread(tc.WaitThreadVar);
            t4.Start(3);
            Thread t5 = new Thread(tc.WaitThreadVar);
            t5.Start(1);
            
            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();
            t5.Join();
            
            MessageBox.Show("Alle 5 Threads zijn klaar");

        }

        private void button10_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(tc.DomThread1);
            t1.Start();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // Zet lamp aan of uit en sla dit op in DB
            Thread t1 = new Thread(tc.DomThread2);
            t1.Start();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            tc.IsCancelled = false;
            Thread t1 = new Thread(tc.DomThread3);
            t1.Start();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            tc.IsCancelled = true;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            DateTime startTime = DateTime.Now;
            tc.IsCancelled = false;

            Thread t = new Thread(() =>
            {
                while (tc.IsCancelled == false)
                {
                    Thread.Sleep(1000);
                    string timeElapsedInstring = (DateTime.Now - startTime).ToString(@"hh\:mm\:ss");
                    textBox3.Invoke(new UpdateLabel(UpdateUI), timeElapsedInstring);
                }
            });
            t.Start();
        }

        public delegate void UpdateLabel(string label);

        public void UpdateUI(string labelText)
        {
            textBox3.Text = labelText;
        }

        public void UpdateUI2(string labelText)
        {
            listBox1.Items.Add(labelText);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(() =>
            {
                string displayOutput;
                int loopCount = 0;
                while (loopCount < 5)
                {
                    Thread.Sleep(5000);
                    displayOutput = tc.DomThread4();
                    loopCount++;

                    textBox3.Invoke(new UpdateLabel(UpdateUI), loopCount.ToString() + " : " + displayOutput);
                    listBox1.Invoke(new UpdateLabel(UpdateUI2), loopCount.ToString() + " : " + displayOutput);
                    

                }
            });
            t1.Start();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(() =>
            {
                string displayOutput;
                int loopCount = 0;
                while (loopCount < 1)
                {
                    //Thread.Sleep(5000);
                    displayOutput = tc.DomThreadJason1();
                    loopCount++;
                    textBox3.Invoke(new UpdateLabel(UpdateUI), loopCount.ToString() + " : " + displayOutput);
                }
            });
            t1.Start();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(() =>
            {
                string displayOutput;
                int loopCount = 0;
                while (loopCount < 1)
                {
                    //Thread.Sleep(5000);
                    displayOutput = tc.DomThreadJason2();
                    loopCount++;
                    textBox3.Invoke(new UpdateLabel(UpdateUI), loopCount.ToString() + " : " + displayOutput);
                }
            });
            t1.Start();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(() =>
            {
                string displayOutput;
                int loopCount = 0;
                while (loopCount < 1)
                {
                    //Thread.Sleep(2000);
                    displayOutput = tc.DomThreadJason3();
                    loopCount++;
                    textBox3.Invoke(new UpdateLabel(UpdateUI), loopCount.ToString() + " : " + displayOutput);
                }
            });
            t1.Start();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
