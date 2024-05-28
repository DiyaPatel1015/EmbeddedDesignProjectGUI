using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmbeddedDesignProjectGUI
{
    public partial class Form1 : Form
    {
        static Appboard appBoard;
        private Timer DigitalTimer;
        private Timer PotsAndLightsTimer;
        private Timer TempTimer;

        public Form1()
        {
            // Initialize the AppBoard object
            appBoard = new Appboard();

            InitializeComponent();
            InitializeSerialPorts();
            InitializeBaudrate();


            connectBtn.Enabled = true;
            disconnectBtn.Enabled = false;

            databaseConnectBtn.Enabled = true;
            databaseDisconnectBtn.Enabled = false;

            passwordTxt.PasswordChar = '*';


            // Subscribe all checkboxes to the CheckBox_CheckedChanged event handler
            PC0.CheckedChanged += CheckBox_CheckedChanged;
            PC1.CheckedChanged += CheckBox_CheckedChanged;
            PC2.CheckedChanged += CheckBox_CheckedChanged;
            PC3.CheckedChanged += CheckBox_CheckedChanged;
            PC4.CheckedChanged += CheckBox_CheckedChanged;
            PC5.CheckedChanged += CheckBox_CheckedChanged;
            PC6.CheckedChanged += CheckBox_CheckedChanged;
            PC7.CheckedChanged += CheckBox_CheckedChanged;


            DigitalTimer = new Timer();
            PotsAndLightsTimer = new Timer();
            TempTimer = new Timer();

            // Set timer intervals
            DigitalTimer.Interval = 100;
            PotsAndLightsTimer.Interval = 100;
            TempTimer.Interval = 100;

            // Attach event handlers
            DigitalTimer.Tick += DigitalTimer_Tick;
            PotsAndLightsTimer.Tick += PotsAndLightsTimer_Tick;
            TempTimer.Tick += TempTimer_Tick;

            // Set up TabControl event handler
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Stop timers first
            DigitalTimer.Stop();
            PotsAndLightsTimer.Stop();
            TempTimer.Stop();

            // Switch between tabs
            switch (tabControl1.SelectedIndex)
            {
                case 0: // Setup Tab
                    // No timer needed
                    break;
                case 1: // Digital I/O Tab
                    DigitalTimer.Start();
                    break;
                case 2: // Ports/Lights Tab
                    PotsAndLightsTimer.Start();
                    break;
                case 3: // Temp Tab
                    TempTimer.Start();
                    break;
                default:
                    break;
            }
        }

        private void DigitalTimer_Tick(object sender, EventArgs e)
        {
        }

        private void PotsAndLightsTimer_Tick(object sender, EventArgs e)
        {
        }

        private void TempTimer_Tick(object sender, EventArgs e)
        {
        }
    }
}
