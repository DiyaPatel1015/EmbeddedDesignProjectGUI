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

            // Set properties of the vertical scrollbar
            lightsScrollBar.Value = 100;  // Start with the scrollbar at the top (inverse 0)

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
            // Digital I/O tick logic
            byte PinA = appBoard.ReadPINA();
            Color[] colors = new Color[] { Color.Red, Color.Black };
            // Check if the first bit of PinA is set (1). If it is, set the color of correspond led to Red, 
            // otherwise set it to Black based on the colors array.
            PA0led.Color = (PinA & (1 << 0)) > 0 ? colors[0] : colors[1];
            PA1led.Color = (PinA & (1 << 1)) > 0 ? colors[0] : colors[1];
            PA2led.Color = (PinA & (1 << 2)) > 0 ? colors[0] : colors[1];
            PA3led.Color = (PinA & (1 << 3)) > 0 ? colors[0] : colors[1];
            PA4led.Color = (PinA & (1 << 4)) > 0 ? colors[0] : colors[1];
            PA5led.Color = (PinA & (1 << 5)) > 0 ? colors[0] : colors[1];
            PA6led.Color = (PinA & (1 << 6)) > 0 ? colors[0] : colors[1];
            PA7led.Color = (PinA & (1 << 7)) > 0 ? colors[0] : colors[1];

            //portC send to microcontroller
            byte lightValue = 0; // Determine the light value to be sent to the microcontroller
            // Iterate through all checkboxes (PC0 to PC7) and set corresponding bits in lightValue
            for (int i = 0; i < 8; i++)
            {
                CheckBox checkbox = Controls.Find("PC" + i, true).FirstOrDefault() as CheckBox;
                if (checkbox != null && checkbox.Checked)
                {
                    lightValue |= (byte)(1 << i); // Set the corresponding bit in lightValue
                }
            }
            // Send the updated light value to the microcontroller
            appBoard.WritePortC(lightValue);
        }

        private void PotsAndLightsTimer_Tick(object sender, EventArgs e)
        {
            //Pot1 voltage
            double potAValue = appBoard.ReadPot1Value();
            double scaledValueA = potAValue / 255.0 * 5.0; // Scale the potA value to fit within the range of 0 to 5
            pot1Gauge.Value = (float)scaledValueA;
            pot1Gauge.DialText = scaledValueA.ToString();

            //Pot2 voltage
            double potBValue = appBoard.ReadPot2Value();
            double scaledValueB = potBValue / 255.0 * 5.0; // Scale the potB value to fit within the range of 0 to 5
            pot2Gauge.Value = (float)scaledValueB;
            pot2Gauge.DialText = scaledValueB.ToString();

            // Read and display Light level
            int lightsensor = appBoard.ReadLight();
            lightGauge.Value = lightsensor;
            lightGauge.DialText = lightsensor.ToString();

            // Adjust lamp brightness based on scrollbar value
            // Calculate the inverse value
            int lightPercentage = lightsScrollBar.Maximum - lightsScrollBar.Value;
            // Update the TextBox text with the inverse value
            lightsTxt.Text = lightPercentage.ToString();

            ushort light = (ushort)(lightPercentage * 4);
            appBoard.WriteLight(light);
        }

        double integralError = 0;
        private void TempTimer_Tick(object sender, EventArgs e)
        { 
            // Reading the temperature sensor
            int tempReceived = appBoard.ReadTemp();
            double setTemp = (double)setpointNumericUpDown.Value;

            // Convert the received temperature data to voltage and then to temperature in degrees Celsius
            double voltage = (tempReceived / 255.0) * 5.0;
            double temp = voltage / 0.05;

            // Display the actual temperature in degrees Celsius
            actualTempTxt.Text = temp.ToString("0.00") + " °C";

            // Calculate the error between the actual temperature and the setpoint
            double error = temp - setTemp;
            integralError += (error * 0.1); // Accumulate the integral error with a fixed time step of 0.1s

            // Retrieve the tuning parameters for the PI controller
            double kp = Convert.ToDouble(piTuningKP);
            double ki = Convert.ToDouble(piTuningKI);

            // Calculate the control output using the PI control law
            ushort result = (ushort)((kp * error) + (ki * integralError));

            // Constrain the result to the range 0 to 100
            if (result > 100)
            {
                result = 100;
            }
            if (result < 0)
            {
                result = 0;
            }

            // Apply the control output to the motor (or other actuators as required)
            appBoard.WriteMotor(result);
        }
    }
}

