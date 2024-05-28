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

        }
    }
}
