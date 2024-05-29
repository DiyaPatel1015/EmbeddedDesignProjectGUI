using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace EmbeddedDesignProjectGUI
{
    public partial class Form1 : Form
    {
        int Check = 0;
        void Datalogging(double Temperature)
        {
            string column1 = "timeStamp"; //column 1 title
            string column2 = "temperature"; //column 2 title
            string column3 = "remark"; //column 3 title
            string Chart = "temperature"; //chart
            string now = DateTime.Now.ToString();
            string TempSend = Temperature.ToString("00.000");
            string note = "ST123456";

            string Query = "INSERT INTO " + Chart + " (`" + column1 + "`,`" + column2 + "`,`" + column3 + "`) VALUES ('" + now + "','" + TempSend + "','" + note + "')";

            try
            {
                MySqlCommand add = new MySqlCommand(Query, myConnection); //Saving insert string and location into output
                add.ExecuteNonQuery(); //executing output

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error"); //showing if incorrect
            }

        }

        private void insertDataBtn_Click(object sender, EventArgs e)
        {
            double temp = (Convert.ToDouble(manualDataTxt.Text));
            Datalogging(temp);
        }

        private void dataLoggingBtn_Click(object sender, EventArgs e)
        {
            dataLoggingBtn.Enabled = false;
            Check = 1;
            dataLoggingBtn.Enabled = true;
        }

        private void stopDataLoggingBtn_Click(object sender, EventArgs e)
        {
            stopDataLoggingBtn.Enabled = false;
            Check = 0;
            stopDataLoggingBtn.Enabled = true;
        }
    }
}

