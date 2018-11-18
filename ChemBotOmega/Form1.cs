﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChemBotOmega
{
    public partial class ChemBotForm : Form
    {
        SerialPort port;
        BindingList<State> bs = new BindingList<State>();

        double CoordX { get; set; }
        double CoordY { get; set; }
        double CoordZ { get; set; }

        double LineWidth;       
        double Multiplier;   
        double PrintSpeed;   
        double TravelSpeed;  
        double ZHeight;       
        double DotSize;
        double DotSpeed;

        double PrimeExtrusion;
        double PrimeExtrusion2;
        double PrimeSpeed;
        double PrimeSpeed2;
        double Retract;
        double RetractSpeed;

        string StartCode;
        string EndCode;

        public ChemBotForm()
        {
            InitializeComponent();

            MessageBox.Show(Regex.Replace(ConfigurationManager.AppSettings["EndCode"].ToString(), "@", Environment.NewLine));

            
        }

        public ChemBotForm(SerialPort port)
        {
            InitializeComponent();
            this.port = port;
            port.DataReceived += new SerialDataReceivedEventHandler(DataReturnFromMachine);
            this.port.Open();

            LineWidth = Convert.ToDouble(ConfigurationManager.AppSettings["LineWidth"]);
            Multiplier = Convert.ToDouble(ConfigurationManager.AppSettings["Multiplier"]);
            PrintSpeed = Convert.ToDouble(ConfigurationManager.AppSettings["PrintSpeed"]);
            TravelSpeed = Convert.ToDouble(ConfigurationManager.AppSettings["TravelSpeed"]);
            ZHeight = Convert.ToDouble(ConfigurationManager.AppSettings["ZHeight"]);
            DotSize = Convert.ToDouble(ConfigurationManager.AppSettings["DotSize"]);
            DotSpeed = Convert.ToDouble(ConfigurationManager.AppSettings["DotSpeed"]);

            PrimeExtrusion = Convert.ToDouble(ConfigurationManager.AppSettings["PrimeExtrusion"]);
            PrimeExtrusion2 = Convert.ToDouble(ConfigurationManager.AppSettings["PrimeExtrusion2"]);
            PrimeSpeed = Convert.ToDouble(ConfigurationManager.AppSettings["PrimeSpeed"]);
            PrimeSpeed2 = Convert.ToDouble(ConfigurationManager.AppSettings["PrimeSpeed2"]);
            Retract = Convert.ToDouble(ConfigurationManager.AppSettings["Retract"]);
            RetractSpeed = Convert.ToDouble(ConfigurationManager.AppSettings["RetractSpeed"]);

            StartCode = ConfigurationManager.AppSettings["StartCode"];
            EndCode = ConfigurationManager.AppSettings["EndCode"];
        }

        private void ChemBotForm_Load(object sender, EventArgs e)
        {
            XScaleComboBox.Items.Add("0.1"); XScaleComboBox.Items.Add("1"); XScaleComboBox.Items.Add("10");
            XScaleComboBox.Text = "1";

            YScaleComboBox.Items.Add("0.1"); YScaleComboBox.Items.Add("1"); YScaleComboBox.Items.Add("10");
            YScaleComboBox.Text = "1";

            ZScaleComboBox.Items.Add("0.1"); ZScaleComboBox.Items.Add("1"); ZScaleComboBox.Items.Add("10");
            ZScaleComboBox.Text = "1";

            StateDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            StateDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            StateDataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            bs.AllowEdit = false;
            bs.AllowRemove = true;
            bs.AllowNew = false;

            State state = new State
            {
                StartPoint = Tuple.Create(0.00, 0.00),
            };

            bs.Add(state);

            StateDataGridView.DataSource = bs;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Exit();
        }

        private void DataReturnFromMachine(object sender, SerialDataReceivedEventArgs e)
        {
            OutputToUser(port.ReadExisting().ToString());
        }

        delegate void SetTextCallback(string text);

        private void OutputToUser(string text)
        {
            OutPutTextBox.AppendText(text + Environment.NewLine);

            Regex Location = new Regex(@"X:\d+\.\d+ Y:\d+\.\d+ Z:\d+\.\d+ E:\d+\.\d+");
            Match match = Location.Match(text);

            if (match.Success)
            {
                Regex number = new Regex(@"\d+\.\d+");

                var coor = number.Matches(text);

                CoordX = Convert.ToDouble(coor[0].Value);
                CoordY = Convert.ToDouble(coor[1].Value);
                CoordZ = Convert.ToDouble(coor[2].Value);

                if (this.XValLabel.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(OutputToUser);
                    this.Invoke(d, new object[] { text });
                }
                else
                {
                    this.XValLabel.Text = coor[0].Value.ToString();
                }

                if (this.YValLabel.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(OutputToUser);
                    this.Invoke(d, new object[] { text });
                }
                else
                {
                    this.YValLabel.Text = coor[1].Value.ToString();
                }

                if (this.ZValLabel.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(OutputToUser);
                    this.Invoke(d, new object[] { text });
                }
                else
                {
                    this.ZValLabel.Text = coor[2].Value.ToString();
                }
            }
        }

        private void SendGCode(string cmd)
        {
            if (port != null && port.IsOpen)
            {
                port.WriteLine(cmd);
            }
            else
            {
                MessageBox.Show("Serial communication is not established.");
            }
        }

        private void Move(int dir, double scale)
        {
            switch (dir)
            {
                case 1:
                        CoordX = CoordX + scale;
                        SendGCode("G01 " + "X" + CoordX + " F" + TravelSpeed);
                    break;

                case 2:
                        CoordY = CoordY + scale;
                        SendGCode("G01 " + "Y" + CoordY + " F" + TravelSpeed);                    
                    break;

                case 3:
                        CoordZ = CoordZ + scale;
                        SendGCode("G01 " + "Z" + CoordZ + " F" + TravelSpeed);                    
                    break;
            }
        }

        private void LeftButton_Click(object sender, EventArgs e)
        {
            double x;
            if (double.TryParse(XScaleComboBox.Text, out x))
            {
                Move(1, Math.Abs(x) * -1);
            }
            else
            {
                MessageBox.Show("Invalid X value");
                XScaleComboBox.Text = "1";
            }
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            double x;
            if (double.TryParse(XScaleComboBox.Text, out x))
            {
                Move(1, Math.Abs(x));
            }
            else
            {
                MessageBox.Show("Invalid X value");
                XScaleComboBox.Text = "1";
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            double y;
            if (double.TryParse(YScaleComboBox.Text, out y))
            {
                Move(2, Math.Abs(y) * -1);
            }
            else
            {
                MessageBox.Show("Invalid Y value");
                YScaleComboBox.Text = "1";
            }
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {
            double y;
            if (double.TryParse(YScaleComboBox.Text, out y))
            {
                Move(2, Math.Abs(y));
            }
            else
            {
                MessageBox.Show("Invalid Y value");
                YScaleComboBox.Text = "1";
            }
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            double z;
            if (double.TryParse(ZScaleComboBox.Text, out z))
            {
                Move(3, Math.Abs(z) * -1);
            }
            else
            {
                MessageBox.Show("Invalid Z value");
                ZScaleComboBox.Text = "1";
            }
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            double z;
            if (double.TryParse(ZScaleComboBox.Text, out z))
            {
                Move(3, Math.Abs(z));
            }
            else
            {
                MessageBox.Show("Invalid Z value");
                ZScaleComboBox.Text = "1";
            }
        }

        private void GoXButton_Click(object sender, EventArgs e)
        {
            double x;
            if (double.TryParse(XPositionTextBox.Text, out x))
            {
                SendGCode("G01 X" + x);
            }
            else
            {
                MessageBox.Show("Invalid GoX value");
            }
        }

        private void GoYButton_Click(object sender, EventArgs e)
        {
            double y;
            if (double.TryParse(YPositionTextBox.Text, out y))
            {
                SendGCode("G01 Y" + y);
            }
            else
            {
                MessageBox.Show("Invalid GoY value");
            }
        }

        private void GoZButton_Click(object sender, EventArgs e)
        {
            double z;
            if (double.TryParse(ZPositionTextBox.Text, out z))
            {
                SendGCode("G01 Z" + z);
            }
            else
            {
                MessageBox.Show("Invalid GoZ value");
            }
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            if (port != null && port.IsOpen)
            {
                SendGCode("G28");
            }
        }

        private void TravelButton_Click(object sender, EventArgs e)
        {
            bs.Last().EndPoint = Tuple.Create(CoordX, CoordY);
            bs.Last().ZPoint = Tuple.Create(bs.Last().ZPoint.Item1, CoordZ);
            bs.Last().Operation = "TRAVEL";
            double distX = bs.Last().EndPoint.Item1 - bs.Last().StartPoint.Item1;
            double distY = bs.Last().EndPoint.Item2 - bs.Last().StartPoint.Item2;

            string gcode = "";
        
            gcode = gcode + "G91" + Environment.NewLine;
            if (bs.Count() != 1)
            {
                gcode = gcode + "G01 E" + Retract + " F" + RetractSpeed + Environment.NewLine;
            }
            gcode = gcode + "G01 Z" + ZHeight + " F" + TravelSpeed + Environment.NewLine;

            if (bs.Last().ZPoint.Item1 != bs.Last().ZPoint.Item2)
            {
                double variation = bs.Last().ZPoint.Item2 - bs.Last().ZPoint.Item1;
                gcode = gcode + "G91" + Environment.NewLine;
                gcode = gcode + "G01 Z" + variation + " F" + TravelSpeed + Environment.NewLine;
            }
            gcode = gcode + "G90" + Environment.NewLine;
            gcode = gcode + "G01 X" + bs.Last().EndPoint.Item1 + " Y" + bs.Last().EndPoint.Item2 + " F" + TravelSpeed + Environment.NewLine;
            gcode = gcode + "G91" + Environment.NewLine;
            gcode = gcode + "G01 Z-" + ZHeight + " F" + TravelSpeed + Environment.NewLine;
            gcode = gcode + "G01 E" + PrimeExtrusion + " F" + PrimeSpeed + Environment.NewLine;

            if (bs.Count() == 1)
            {
                gcode = gcode + "G01 E" + PrimeExtrusion2 + " F" + PrimeSpeed2 + Environment.NewLine;
            }

            gcode = gcode + "G90" + Environment.NewLine;
            bs.Last().GCode = gcode;
            ConcatNewPoint();
            StateDataGridView.Refresh();
        }

        private void ConcatNewPoint()
        {
            State state = new State
            {
                StartPoint = bs.Last().EndPoint,
                ZPoint = Tuple.Create(CoordZ, Double.NaN)
            };
            bs.Add(state);
        }

        private void DotButton_Click(object sender, EventArgs e)
        {
            if (!Double.IsNaN(bs.Last().EndPoint.Item1))
            {
                MessageBox.Show("Invalid dot operation. Only start point is needed.");
            }
            else
            {
                string gcode = "G90" + Environment.NewLine;
                gcode = gcode + "G01 X" + bs.Last().StartPoint.Item1 + " Y" + bs.Last().StartPoint.Item2 + Environment.NewLine;
                gcode = gcode + "G91" + Environment.NewLine;
                gcode = gcode + "G01 E" + DotSize + " F" + DotSpeed + Environment.NewLine;
                bs.Last().Operation = "DOT";
                bs.Last().GCode = gcode;
                ConcatNewPoint();
                StateDataGridView.Refresh();
            }
        }

        private void LineButton_Click(object sender, EventArgs e)
        {
            bs.Last().EndPoint = Tuple.Create(CoordX, CoordY);
            bs.Last().ZPoint = Tuple.Create(bs.Last().ZPoint.Item1, CoordZ);

            bs.Last().Operation = "LINE";
            double distX = bs.Last().EndPoint.Item1 - bs.Last().StartPoint.Item1;
            double distY = bs.Last().EndPoint.Item2 - bs.Last().StartPoint.Item2;

            string gcode = "";

            gcode = gcode + "G90" + Environment.NewLine;
            gcode = gcode + "G01 X" + bs.Last().StartPoint.Item1 + " Y" + bs.Last().StartPoint.Item2 + Environment.NewLine;
            gcode = gcode + "G91" + Environment.NewLine;
            gcode = gcode + "G01 X" + distX + " Y" + distY + " E" + (((distX == 0.00) ? Math.Abs(distY) : Math.Abs(distX)) / Multiplier) + " F" + PrintSpeed;

            bs.Last().GCode = gcode;
            ConcatNewPoint();
            StateDataGridView.Refresh();
        }

        private void CoilButton_Click(object sender, EventArgs e)
        {
            bs.Last().EndPoint = Tuple.Create(CoordX, CoordY);
            bs.Last().ZPoint = Tuple.Create(bs.Last().ZPoint.Item1, CoordZ);

            double distX = Math.Abs(bs.Last().EndPoint.Item1 - bs.Last().StartPoint.Item1) - LineWidth;
            double distY = Math.Abs(bs.Last().EndPoint.Item2 - bs.Last().StartPoint.Item2) - LineWidth;
            string gcode = "";
            gcode = gcode + "G90" + Environment.NewLine;
            gcode = gcode + "G01 X" + bs.Last().StartPoint.Item1 + " Y" + bs.Last().StartPoint.Item2 + Environment.NewLine;
            gcode = gcode + "G91" + Environment.NewLine;
            Boolean loop = true;

            if (distX < LineWidth && distY < LineWidth)
            {
                MessageBox.Show("Invalid input. PLease choose another point");
            }
            else if (distY < LineWidth)
            {
                gcode = gcode + "G01 X" + distX + " E" + (distX / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
            }
            else if (distX < LineWidth)
            {
                gcode = gcode + "G01 Y" + distY + " E" + (distY / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
            }
            else
            {
                gcode = gcode + "G01 X" + distX + " E" + (distX / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                gcode = gcode + "G01 Y" + distY + " E" + (distY / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                gcode = gcode + "G01 X-" + distX + " E" + (distX / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                distY -= LineWidth;
                distX -= LineWidth;
                gcode = gcode + "G01 Y-" + distY + " E" + (distY / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                distY -= LineWidth;
            }

            if (distY < LineWidth)
            {
                gcode = gcode + "G01 X" + distX + " E" + (distX / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
            }
            else
            {
                int counter = 1;
                while (loop)
                {
                    switch (counter)
                    {
                        case 1:
                            if (distX < LineWidth)
                            {
                                counter = 4;
                            }
                            else
                            {
                                gcode = gcode + "G01 X" + distX + " E" + (distX / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                                distX -= LineWidth;
                                counter = 2;
                                MessageBox.Show(distY.ToString());
                            }
                            break;

                        case 2:
                            if (distY < LineWidth)
                            {
                                counter = 4;
                            }
                            else
                            {
                                gcode = gcode + "G01 Y" + distY + " E" + (distY / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                                distY -= LineWidth;
                                counter = 3;
                            }
                            break;

                        case 3:
                            if (distX < LineWidth)
                            {
                                counter = 4;
                            }
                            else
                            {
                                gcode = gcode + "G01 X-" + distX + " E" + (distX / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                                distX -= LineWidth;
                                counter = 0;
                            }
                            break;

                        case 0:
                            if (distY < LineWidth)
                            {
                                counter = 4;
                            }
                            else
                            {
                                gcode = gcode + "G01 Y-" + distY + " E" + (distY / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                                distY -= LineWidth;
                                counter = 1;
                            }
                            break;

                        case 4:
                            loop = false;
                            break;
                    }

                }
            }
            bs.Last().Operation = "COIL";
            bs.Last().GCode = gcode;
            ConcatNewPoint();
            StateDataGridView.Refresh();
        }

        private void ZigZagButton_Click(object sender, EventArgs e)
        {
            bs.Last().EndPoint = Tuple.Create(CoordX, CoordY);
            bs.Last().ZPoint = Tuple.Create(bs.Last().ZPoint.Item1, CoordZ);

            double distX = Math.Abs(bs.Last().EndPoint.Item1 - bs.Last().StartPoint.Item1);
            double distY = Math.Abs(bs.Last().EndPoint.Item2 - bs.Last().StartPoint.Item2);
            string gcode = "";
            gcode = gcode + "G90" + Environment.NewLine;
            gcode = gcode + "G01 X" + bs.Last().StartPoint.Item1 + " Y" + bs.Last().StartPoint.Item2 + Environment.NewLine;
            gcode = gcode + "G91" + Environment.NewLine;
            Boolean reverse = false;

            while (distY > 0)
            {
                if (distY > LineWidth)
                {
                    if (reverse)
                    {
                        gcode = gcode + "G01 X-" + distX + " E" + (distX / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                        reverse = false;
                    }
                    else
                    {
                        gcode = gcode + "G01 X" + distX + " E" + (distX / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                        reverse = true;
                    }
                    if (distY - LineWidth == LineWidth)
                    {
                        distY -= LineWidth;
                        gcode = gcode + "G01 Y" + LineWidth + " E" + (distY / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                        if (reverse)
                        {
                            gcode = gcode + "G01 X-" + distX + " E" + (distX / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                            reverse = false;
                        }
                        else
                        {
                            gcode = gcode + "G01 X" + distX + " E" + (distX / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                            reverse = true;
                        }
                    }
                    else if (distY - LineWidth < LineWidth)
                    {
                        distY -= LineWidth;
                        gcode = gcode + "G01 Y" + ((LineWidth / 2) + distY / 2) + " E" + (distY / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                    }
                    else
                    {
                        gcode = gcode + "G01 Y" + LineWidth + " E" + (distY / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                        distY -= LineWidth;
                    }
                }
                else
                {
                    if (distY > 0 && distY < LineWidth)
                    {
                        if (reverse)
                        {
                            gcode = gcode + "G01 X-" + distX + " E" + (distX / Multiplier) + " F" + PrintSpeed;
                            MessageBox.Show(gcode);
                            reverse = false;
                        }
                        else
                        {
                            gcode = gcode + "G01 X" + distX + " E" + (distX / Multiplier) + " F" + PrintSpeed;
                            reverse = true;
                        }
                    }
                    break;
                }
            }
            bs.Last().Operation = "ZIGZAG";
            bs.Last().GCode = gcode;
            ConcatNewPoint();
            StateDataGridView.Refresh();
        }

        private void CircleButton_Click(object sender, EventArgs e)
        {
            bs.Last().EndPoint = Tuple.Create(CoordX, CoordY);
            bs.Last().ZPoint = Tuple.Create(bs.Last().ZPoint.Item1, CoordZ);

            double radius = (bs.Last().EndPoint.Item1 - bs.Last().StartPoint.Item1) / 2;
            List<double> LineWidthList = new List<double>();

            while (radius >= LineWidth)
            {
                LineWidthList.Add(LineWidth);
                radius -= LineWidth;
            }

            LineWidthList.Remove(0);

            if (radius > 0)
            {
                LineWidthList.Add(radius);
            }

            string gcode = "";
            radius = (bs.Last().EndPoint.Item1 - bs.Last().StartPoint.Item1) / 2;

            gcode = gcode + "G90" + Environment.NewLine;
            gcode = gcode + "G01 X" + (bs.Last().StartPoint.Item1 + (LineWidthList.First() / 2)) + " Y" + bs.Last().StartPoint.Item2 + " F" + TravelSpeed + Environment.NewLine;
            gcode = gcode + "G91" + Environment.NewLine;

            foreach (double x in LineWidthList)
            {
                gcode = gcode + "G01 X" + (x) + " F" + TravelSpeed + Environment.NewLine;

                if (x == LineWidth)
                {
                    gcode = gcode + "G02 I" + (radius - x / 2) + " E" + (Math.Round((2 * Math.PI * radius), 4) / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                }
                else
                {
                    gcode = gcode + "G01 E" + (radius - x / 2) + " E" + (Math.Round((2 * Math.PI * radius), 4) / Multiplier) + " F" + PrintSpeed + Environment.NewLine;
                }
                radius -= x;
            }
            bs.Last().Operation = "CIRCLE";
            bs.Last().GCode = gcode;
            ConcatNewPoint();
            StateDataGridView.Refresh();
        }

        private void ArcButton_Click(object sender, EventArgs e)
        {
            bs.Last().EndPoint = Tuple.Create(CoordX, CoordY);
            bs.Last().ZPoint = Tuple.Create(bs.Last().ZPoint.Item1, CoordZ);

            double OffsetX = (bs.Last().EndPoint.Item1 - bs.Last().StartPoint.Item1) / 2;
            double OffsetY = (bs.Last().EndPoint.Item2 - bs.Last().StartPoint.Item2) / 2;
            string gcode = "";

            gcode = gcode + "G90" + Environment.NewLine;
            gcode = gcode + "G01 X" + bs.Last().StartPoint.Item1 + " Y" + bs.Last().StartPoint.Item2 + Environment.NewLine;
            gcode = gcode + "G91" + Environment.NewLine;

            gcode = gcode + "G02 X" + (bs.Last().EndPoint.Item1 - bs.Last().StartPoint.Item1) + " Y" + (bs.Last().EndPoint.Item2 - bs.Last().StartPoint.Item2);
            gcode = gcode + " I" + OffsetX + " J" + OffsetY + " E" + (Math.Round((Math.PI * OffsetX), 4) / Multiplier) + " F" + PrintSpeed + Environment.NewLine;

            bs.Last().Operation = "ARC";
            bs.Last().GCode = gcode;
            ConcatNewPoint();
            StateDataGridView.Refresh();
        }

        private void CArcButton_Click(object sender, EventArgs e)
        {
            bs.Last().EndPoint = Tuple.Create(CoordX, CoordY);
            bs.Last().ZPoint = Tuple.Create(bs.Last().ZPoint.Item1, CoordZ);
            double OffsetX = (bs.Last().EndPoint.Item1 - bs.Last().StartPoint.Item1) / 2;
            double OffsetY = (bs.Last().EndPoint.Item2 - bs.Last().StartPoint.Item2) / 2;
            string gcode = "";

            gcode = gcode + "G90" + Environment.NewLine;
            gcode = gcode + "G01 X" + bs.Last().StartPoint.Item1 + " Y" + bs.Last().StartPoint.Item2 + Environment.NewLine;
            gcode = gcode + "G91" + Environment.NewLine;

            gcode = gcode + "G03 X" + (bs.Last().EndPoint.Item1 - bs.Last().StartPoint.Item1) + " Y" + (bs.Last().EndPoint.Item2 - bs.Last().StartPoint.Item2);
            gcode = gcode + " I" + OffsetX + " J" + OffsetY + " E" + (Math.Round((Math.PI * OffsetX), 4) / Multiplier) + " F" + PrintSpeed + Environment.NewLine;

            bs.Last().Operation = "ARC";
            bs.Last().GCode = gcode;
            ConcatNewPoint();
            StateDataGridView.Refresh();
        }
        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (ExportSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (ExportSaveFileDialog.FileName != "")
                {
                    try
                    {
                        //Pass the filepath and filename to the StreamWriter Constructor
                        StreamWriter sw = new StreamWriter(ExportSaveFileDialog.FileName);

                        sw.Write(StartCode + Environment.NewLine);
                        for (int rows = 0; rows < StateDataGridView.RowCount; rows++)
                        {
                            if (StateDataGridView.Rows[rows].Cells[3].Value != null)
                            {
                                sw.Write(";" + StateDataGridView.Rows[rows].Cells[3].Value.ToString() + Environment.NewLine);
                                sw.Write(StateDataGridView.Rows[rows].Cells[4].Value.ToString());
                            }

                        }

                        sw.Write(EndCode + Environment.NewLine);

                        //Close the file
                        sw.Close();

                        MessageBox.Show("File save successfully.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception: " + ex.Message);
                    }
                }
            }
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            if (port.IsOpen)
            {
                port.Close();
            }
            
            port = null;
            new StartScreen().Show();
            Close();
        }

        private void PreheatButton_Click(object sender, EventArgs e)
        {
            if (port != null && port.IsOpen)
            {
                SendGCode("M104 S80");
                SendGCode("M140 S60");
            }            
        }

        private void SettingButton_Click_1(object sender, EventArgs e)
        {
            using (Setting fm = new Setting(
                Multiplier,
                PrintSpeed,
                Convert.ToInt32(LineWidth),
                ZHeight,
                PrimeExtrusion,
                PrimeExtrusion2,
                PrimeSpeed,
                PrimeSpeed2,
                Retract,
                RetractSpeed,
                TravelSpeed,
                DotSpeed,
                DotSize,
                StartCode,
                EndCode))
            {
                fm.ShowDialog();
                Multiplier = fm.GetMultiplier();
                PrintSpeed = fm.GetSpeed();
                LineWidth = Convert.ToDouble(fm.GetLineWidth());
                ZHeight = fm.GetZHeight();
                PrimeExtrusion = fm.GetPrimeExtrusion();
                PrimeExtrusion2 = fm.GetPrimeExtrusion2();
                PrimeSpeed = fm.GetPrimeSpeed();
                PrimeSpeed2 = fm.GetPrimeSpeed2();
                Retract = fm.GetRetract();
                RetractSpeed = fm.GetRetractSpeed();
                TravelSpeed = fm.GetTravelSpeed();
                DotSpeed = fm.GetDotSpeed();
                DotSize = fm.GetDotSize();
                StartCode = fm.GetStartCode();
                EndCode = fm.GetEndCode();
            }
        }

        private void StateDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (StateDataGridView.RowCount == 1)
            {
                State state = new State
                {
                    StartPoint = Tuple.Create(0.00, 0.00)
                };
                bs.Add(state);
                StateDataGridView.Refresh();
            }
        }
    }
}