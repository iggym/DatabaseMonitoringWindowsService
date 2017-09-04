using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace DatabaseMonitoringWindowsService
{
    public partial class DbMonitorService : ServiceBase
    {
        int currentRowCount;
        public DbMonitorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            currentRowCount = getRowCount(); //set the row count when service starts
            //setup timer for polling
            SetupTimer();
        }

        private void SetupTimer()
        {
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = Int32.Parse(ConfigurationManager.AppSettings["PollingInterval"]);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }
        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            //   monitoring activities here.  
            int rowCount = getRowCount();
            if (rowCount > currentRowCount ) // if row count is more than last set row count log alert
            {
                LogAlert();
            }
           
        }

        static int getRowCount()
        {
            string connString = "YOUR_CONNECTION_STRING";
            string stmt = "SELECT COUNT(*) FROM dbo.[SensorData]";
            int count = 0;

            using (SqlConnection thisConnection = new SqlConnection(connString))
            {
                using (SqlCommand cmdCount = new SqlCommand(stmt, thisConnection))
                {
                    thisConnection.Open();
                    count = (int)cmdCount.ExecuteScalar();
                }
            }
            currentRowCount = count;
            return count;
        }
        static void LogAlert()
        {
            string connString = "YOUR_CONNECTION_STRING";
            string stmt = "SELECT TOP(1) * FROM[SensorDataDB].[dbo].[SensorData] ORDER BY[ReadingID] DESC";

            SqlConnection sqlConnection1 = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = stmt;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;

            sqlConnection1.Open();

            reader = cmd.ExecuteReader();
            // Data is accessible through the DataReader object here.
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                   // Console.WriteLine(reader["Temperature"]);
                    string id = reader["ReadingID"].ToString();
                    string temp = reader["Temperature"].ToString();
                    string pressure = reader["Pressure"].ToString();
                    string luminosity = reader["[luminosity"].ToString();
                    string timeStamp = reader["Time/stamp"].ToString();
                    //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    string logline = id + "," + temp + "," + pressure + "," + luminosity + "," + timeStamp;
                    File.AppendAllText(@"c:\alerts.txt", logline + Environment.NewLine);
                }
            }
            else
            {
                Console.WriteLine("No rows found.");
            }
            reader.Close();
            sqlConnection1.Close();
        }
        protected override void OnStop()
        {
        }
    }
}
