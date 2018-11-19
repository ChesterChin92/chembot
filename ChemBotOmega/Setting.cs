using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChemBotOmega
{
    public partial class Setting : Form
    {
        double multiplier;
        double speed;
        int linewidth;
        double zheight;
        double primeextrusion;
        double primeextrusion2;
        double primespeed;
        double primespeed2;
        double retract;
        double retractspeed;
        double travelspeed;
        double dotspeed;
        double dotsize;

        string startcode;
        string endcode;

        public Setting()
        {
            InitializeComponent();            
        }

        public Setting(double mul,
            double speed,
            int lw,
            double zheight,
            double pex,
            double pex2,
            double ps,
            double ps2,
            double rt,
            double rts,
            double ts,
            double dtsp,
            double dtsz, 
            string startcode,
            string endcode)
        {
            InitializeComponent();
            multiplier = mul;
            this.speed = speed;
            linewidth = lw;
            this.zheight = zheight;
            primeextrusion = pex;
            primeextrusion2 = pex2;
            primespeed = ps;
            primespeed2 = ps2;
            retract = rt;
            retractspeed = rts;
            travelspeed = ts;
            dotspeed = dtsp;
            dotsize = dtsz;
            this.startcode = startcode;
            this.endcode = endcode;

            LineWidthComboBox.Items.Add("2");
            LineWidthComboBox.Items.Add("3");

            LineWidthComboBox.SelectedText = lw.ToString();
        }

        public double GetMultiplier()
        {
            return multiplier;
        }

        public double GetSpeed()
        {
            return speed;
        }

        public double GetLineWidth()
        {
            return linewidth;
        }

        public double GetZHeight()
        {
            return zheight;
        }

        public double GetPrimeExtrusion()
        {
            return primeextrusion;
        }

        public double GetPrimeExtrusion2()
        {
            return primeextrusion2;
        }

        public double GetPrimeSpeed()
        {
            return primespeed;
        }

        public double GetPrimeSpeed2()
        {
            return primespeed2;
        }

        public double GetRetract()
        {
            return retract;
        }

        public double GetRetractSpeed()
        {
            return retractspeed;
        }

        public double GetTravelSpeed()
        {
            return travelspeed;
        }

        public double GetDotSpeed()
        {
            return dotspeed;
        }

        public double GetDotSize()
        {
            return dotsize;
        }

        public string GetStartCode()
        {
            return startcode;
        }

        public string GetEndCode()
        {
            return endcode;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            double s, zh, pe, pe2, ps, ps2, rt, rts, ts, dtsz, dtsp;
            int lw;
            Boolean isError = false;

            if (Double.TryParse(PrintSpeedTextBox.Text, out s))
            {
                speed = s;
            }
            else
            {
                MessageBox.Show("Invalid SPEED value");
                isError = true;
            }

            if (Int32.TryParse(LineWidthComboBox.Text, out lw))
            {
                linewidth = lw;

                switch (lw)
                {
                    case 2:
                        multiplier = 200.00;
                        break;

                    case 3:
                        multiplier = 150.00;
                        break;
                }
            }
            else
            {
                MessageBox.Show("Invalid LINEWIDTH value");
                isError = true;
            }

            if (Double.TryParse(ZLiftTextBox.Text, out zh))
            {
                this.zheight = zh;
            }
            else
            {
                MessageBox.Show("Invalid ZHEIGHT value");
                isError = true;
            }

            if (Double.TryParse(PrimeExtrusionTextBox.Text, out pe))
            {
                this.primeextrusion = pe;
            }
            else
            {
                MessageBox.Show("Invalid PRIME EXTRUSION value");
                isError = true;
            }

            if (Double.TryParse(PrimeExtrusion2TextBox.Text, out pe2))
            {
                this.primeextrusion2 = pe2;
            }
            else
            {
                MessageBox.Show("Invalid PRIME EXTRUSION 2 value");
                isError = true;
            }

            if (Double.TryParse(PrimeSpeedTextBox.Text, out ps))
            {
                this.primespeed = ps;
            }
            else
            {
                MessageBox.Show("Invalid PRIME SPEED value");
                isError = true;
            }

            if (Double.TryParse(PrimeSpeed2TextBox.Text, out ps2))
            {
                this.primespeed2 = ps2;
            }
            else
            {
                MessageBox.Show("Invalid PRIME SPEED 2 value");
                isError = true;
            }

            if (Double.TryParse(RetractTextBox.Text, out rt))
            {
                this.retract = rt;
            }
            else
            {
                MessageBox.Show("Invalid RETRACT value");
                isError = true;
            }

            if (Double.TryParse(RetractSpeedTextBox.Text, out rts))
            {
                this.retractspeed = rts;
            }
            else
            {
                MessageBox.Show("Invalid RETRACT SPEED value");
                isError = true;
            }

            if (Double.TryParse(TravelSpeedTextBox.Text, out ts))
            {
                this.travelspeed = ts;
            }
            else
            {
                MessageBox.Show("Invalid TRAVEL SPEED value");
                isError = true;
            }

            if (Double.TryParse(DotSpeedTextBox.Text, out dtsp))
            {
                this.dotspeed = dtsp;
            }
            else
            {
                MessageBox.Show("Invalid DOT SPEED value");
                isError = true;
            }

            if (Double.TryParse(DotSizeTextBox.Text, out dtsz))
            {
                this.dotsize = dtsz;
            }
            else
            {
                MessageBox.Show("Invalid DOT SIZE value");
                isError = true;
            }

            if (!isError)

            {
                Close();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Setting_Load_1(object sender, EventArgs e)
        {
            PrintSpeedTextBox.Text = speed.ToString();
            LineWidthComboBox.Text = linewidth.ToString();
            ZLiftTextBox.Text = zheight.ToString();
            PrimeExtrusionTextBox.Text = primeextrusion.ToString();
            PrimeExtrusion2TextBox.Text = primeextrusion2.ToString();
            PrimeSpeedTextBox.Text = primespeed.ToString();
            PrimeSpeed2TextBox.Text = primespeed2.ToString();
            RetractTextBox.Text = retract.ToString();
            RetractSpeedTextBox.Text = retractspeed.ToString();
            TravelSpeedTextBox.Text = travelspeed.ToString();
            DotSizeTextBox.Text = dotsize.ToString();
            DotSpeedTextBox.Text = dotspeed.ToString();

            StartCodeTextBox.Text = startcode;
            EndCodeTextBox.Text = endcode;
        }

        private void StartCodeOKButton_Click(object sender, EventArgs e)
        {
            this.startcode = StartCodeTextBox.Text;
            Close();
        }

        private void StartCodeCancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void EndCodeOKButton_Click(object sender, EventArgs e)
        {
            this.endcode = EndCodeTextBox.Text;
            Close();
        }

        private void EndCodeCancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
