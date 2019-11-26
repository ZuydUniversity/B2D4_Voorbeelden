using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WindowsFormsAppExamplesRWA
{
    class ThreadingClass
    {
        static int NumberOfThreads;
        private int WaitTime;
        public bool IsCancelled;

        public ThreadingClass(int numberOfThreads)
        {
            NumberOfThreads = 0;
        }

        public void SetWaitTime(int waitTime)
        {
            WaitTime = waitTime;
        }

        public void WaitThread5()
        {
            int numberOfSeconds = 0;
            while (numberOfSeconds < 5)
            {
                Thread.Sleep(1000);
                numberOfSeconds++;
            }

            MessageBox.Show("I ran for 5 seconds");
        }

        public void WaitThread10()
        {
            int waitTime = WaitTime;
            int numberOfSeconds = 0;
            while (numberOfSeconds < 10)
            {
                Thread.Sleep(1000);
                numberOfSeconds++;
            }

            MessageBox.Show("I ran for 10 seconds");
        }

        public void WaitThreadX()
        {
            int VarWaitTime = WaitTime;
            // de bovenstaande aanpak lijkt niet te werken
            // bij 2 aanroepen wordt alleen laatste waarde gebruikt door beide Threads
            int numberOfSeconds = 0;
            while (numberOfSeconds < VarWaitTime)
            {
                Thread.Sleep(1000);
                numberOfSeconds++;
            }

            MessageBox.Show("I ran for " + VarWaitTime + " seconds");
        }

        public void WaitThreadVar(object data)
        {
            int VarWaitTime = (int) data;
            int numberOfSeconds = 0;
            while (numberOfSeconds < VarWaitTime)
            {
                Thread.Sleep(1000);
                numberOfSeconds++;
            }

            MessageBox.Show("I ran for " + VarWaitTime + " seconds");
        }

        public void DomThread1()
        {
            HttpWebRequest request =
                WebRequest.Create(
                        "http://127.0.0.1:8080/json.htm?type=command&param=switchlight&idx=1&switchcmd=On") as
                    HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            MessageBox.Show(text);
        }

        public void DomThread2()
        {
            string StatusString;
            string URLstring;
            
            // BEPAAL LAMP STATUS
            URLstring = "http://127.0.0.1:8080/json.htm?type=devices&rid=9";
            HttpWebRequest request = WebRequest.Create(URLstring) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            DomWriteToDb(text); // Write To DB
            if (text.Contains(@"""Status"" : ""On"""))
            {
                StatusString = "Aan";
                //MessageBox.Show("Is Aan, gaat Uit");
            }
            else
            {
                StatusString = "Uit";
                //MessageBox.Show("Is Uit, gaat Aan");
            }

            // Bepaal URL afhankelijk van of LAMP AAN of UIT moet
            if (StatusString == "Aan")
            {
                URLstring = "http://127.0.0.1:8080/json.htm?type=command&param=switchlight&idx=9&switchcmd=Off";
            }
            else
            {
                URLstring = "http://127.0.0.1:8080/json.htm?type=command&param=switchlight&idx=9&switchcmd=On";
            }

            // ZET LAMP AAN/UIT
            request = WebRequest.Create(URLstring) as HttpWebRequest;
            response = request.GetResponse() as HttpWebResponse;
            stream = response.GetResponseStream();
            reader = new StreamReader(stream);
            text = reader.ReadToEnd();

            DomWriteToDb(text); // Write To DB
            //MessageBox.Show(text);

        }

        public string DomThread4()
        {
            string StatusString;
            string URLstring;
            // BEPAAL LAMP STATUS
            URLstring = "http://127.0.0.1:8080/json.htm?type=devices&rid=9";
            HttpWebRequest request = WebRequest.Create(URLstring) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            DomWriteToDb(text); // Write To DB
            if (text.Contains(@"""Status"" : ""On"""))
            {
                StatusString = "Aan";
            }
            else
            {
                StatusString = "Uit";
            }
            return StatusString;
        }

        public string DomThreadJason1()
        {
            string StatusString;
            string URLstring;
            // BEPAAL LAMP STATUS
            URLstring = "http://127.0.0.1:8080/json.htm?type=command&param=switchlight&idx=9&switchcmd=Off";
            HttpWebRequest request = WebRequest.Create(URLstring) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string bericht = reader.ReadToEnd();

            //JSON - Deserialise object van bekende Class
            DomCommandMessage m = JsonConvert.DeserializeObject<DomCommandMessage>(bericht);
            StatusString = m.status;

            return StatusString;
        }

        public string DomThreadJason2()
        {
            string StatusString;
            string URLstring;
            // BEPAAL LAMP STATUS
            URLstring = "http://127.0.0.1:8080/json.htm?type=command&param=switchlight&idx=9&switchcmd=Off";
            HttpWebRequest request = WebRequest.Create(URLstring) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string bericht = reader.ReadToEnd();

            //JSON - Deserialise object van bericht van (onbekende) dictionary (name/value) structuur
            Dictionary<string, string> htmlAttributes = JsonConvert.DeserializeObject<Dictionary<string, string>>(bericht);
            StatusString = htmlAttributes["title"];

            return StatusString;
        }

        public string DomThreadJason3()
        {
            string OutputString;
            string URLstring;
            // BEPAAL LAMP STATUS
            URLstring = "http://127.0.0.1:8080/json.htm?type=devices&rid=9";
            HttpWebRequest request = WebRequest.Create(URLstring) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string bericht = reader.ReadToEnd();

            //JSON - Deserialise object van bericht van (onbekende) complexe structuur
            JObject o = JObject.Parse(bericht);
            string sunset = (string) o["Sunset"];
            string hardwareName = (string)o["result"][0]["HardwareName"];

            OutputString = sunset + " " + hardwareName;

            return OutputString;
        }

        public void DomThread3()
        {
            int loopCount = 0;
            while (IsCancelled == false && loopCount < 5)
            {
                Thread.Sleep(5000);
                DomThread2();
                loopCount++;
            }
        }

        public void DomWriteToDb(string message)
        {
            string connectionString = "Data Source=LB1908061;Initial Catalog=IoTdata;Integrated Security=True";
            SqlConnection cnn = new SqlConnection();
            try
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                string sql = "INSERT INTO tblLogging (Message) VALUES (@Message)";
                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    cmd.Parameters.AddWithValue("@Message", message);
                    cmd.ExecuteNonQuery();
                }

                cnn.Close();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                cnn.Dispose();
            }
        }

        public void DomWriteToDb2(string message)
        {
            string connectionString = "Data Source=LB1908061;Initial Catalog=IoTdata;Integrated Security=True";
            SqlConnection cnn = new SqlConnection();
            cnn.ConnectionString = connectionString;
            cnn.Open();
            string sql = "INSERT INTO tblLogging (Message) VALUES (@Message)";
            using (SqlCommand cmd = new SqlCommand(sql, cnn))
            {
                cmd.Parameters.AddWithValue("@Message", message);
                cmd.ExecuteNonQuery();
            }
            cnn.Close();
        }

    }
}