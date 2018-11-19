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
            ComPortComboBox.Items.Clear();

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
            if (ComPortComboBox.Text == "")
            {
                MessageBox.Show("Please select a COM PORT.");
            }
            else
            {
                new ChemBotForm(ComPortComboBox.Text).Show();
                Visible = false;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Exit();
        }
    }
}
