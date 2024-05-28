using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Drawing;
using MySql.Data.MySqlClient;
using System.Data;

namespace EmbeddedDesignProjectGUI
{
    public partial class Form1 : Form
    {
        MySqlConnection myConnection;
        MySqlCommand myCommand;

        private bool passwordVisible = false; // Variable to track password visibility

        private void InitializeSerialPorts()
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            // Add each port name to the ComboBox.
            comPort.Items.AddRange(ports);

            // Optionally, you can select the first item in the ComboBox.

            if (comPort.Items.Count > 0)
            {
                comPort.SelectedIndex = 0;
            }
        }

        private void InitializeBaudrate()
        {
            // Add  items to the ComboBox.
            baudrate.Items.Add("9600");
            baudrate.Items.Add("19200");
            baudrate.Items.Add("38400");
            baudrate.Items.Add("57600");
            baudrate.Items.Add("115200");

            // default item.
            baudrate.SelectedIndex = 2;
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            appBoard.InitializeSerialPort(comPort.Text, Convert.ToInt32(baudrate.Text));
            if (appBoard.ConnectSerialPort())
            {
                // Update status indicator
                serialStatLed.On = true;
                connectBtn.Enabled = false;
                disconnectBtn.Enabled = true;
            }
            else
            {
                MessageBox.Show("Failed to connect to serial port.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void disconnectBtn_Click(object sender, EventArgs e)
        {
            appBoard.DisconnectSerialPort();
            serialStatLed.On = false;
            disconnectBtn.Enabled = false;
            connectBtn.Enabled = true;
        }

        private void passwordHiddenBtn_Click(object sender, EventArgs e)
        {
            // Toggle the visibility of the password
            if (passwordVisible)
            {
                // Hide the password
                passwordTxt.PasswordChar = '*';
            }
            else
            {
                // Show the password
                passwordTxt.PasswordChar = '\0';
            }

            // Update the state
            passwordVisible = !passwordVisible;
        }

        private void databaseConnectBtn_Click(object sender, EventArgs e)
        {
            string server = serverName.Text;
            int port = 3306; // Default MySQL port
            string username = userNameTxt.Text; //ST1234567
            string password = passwordTxt.Text; //Andm1015!
            string database = databaseTxt.Text; //temperature_record

            try
            {
                string connectionString = $"server={server};port={port};user={username};password={password};database={database}";

                using (myConnection = new MySqlConnection(connectionString))
                {
                    myConnection.Open();
                    databaseStatLed.On = true;
                    databaseConnectBtn.Enabled = false;
                    databaseDisconnectBtn.Enabled = true;
                }

                MessageBox.Show("Database connection successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Unable to connect to database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                databaseStatLed.On = false;
            }
        }

        private void databaseDisconnectBtn_Click(object sender, EventArgs e) //it not disconnecting?
        {
            if (myConnection.State == ConnectionState.Open) // Check if the connection is already open
            {
                myConnection.Close();
                databaseStatLed.On = false;
                databaseConnectBtn.Enabled = true;
                databaseDisconnectBtn.Enabled = false;

                // Clear text boxes
                serverName.Text = "";
                userNameTxt.Text = "";
                passwordTxt.Text = "";
                databaseTxt.Text = "";
            }
        }
    }
}
