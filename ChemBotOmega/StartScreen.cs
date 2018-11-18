using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChemBotOmega
{
    public partial class StartScreen : Form
    {
        string[] ComPorts;
        SerialPort port;

        public StartScreen()
        {
            InitializeComponent();
            GetComPort();
        }

        private void StartScreen_Load(object sender, EventArgs e)
        {
            
        }

        private void GetComPort()
        {
            ComPorts = SerialPort.GetPortNames();

            foreach (string cp in ComPorts)
            {
                ComPortComboBox.Items.Add(cp);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            GetComPort();
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (ComPortComboBox.Text == null)
            {
                MessageBox.Show("Please select a COM PORT.");
            }
            else
            {
                try
                {
                    port = new SerialPort(ComPortComboBox.Text, 250000, Parity.None, 8, StopBits.One)
                    {
                        Handshake = Handshake.None
                    };
                    new ChemBotForm(port).Show();
                    Close();
                }
                catch (Exception ex)
                {
                    if (port != null)
                    {
                        port.Dispose();
                    }

                    MessageBox.Show("Error Message: " + Environment.NewLine + ex.Message);
                }
            }            
        }

        private void ComPortComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }        
    }
}
