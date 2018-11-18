using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemBotOmega
{
    class State
    {
        public Tuple<double, double> StartPoint { get; set; } = Tuple.Create(Double.NaN, Double.NaN);
        public Tuple<double, double> EndPoint { get; set; } = Tuple.Create(Double.NaN, Double.NaN);
        public Tuple<double, double> ZPoint { get; set; } = Tuple.Create(Double.NaN, Double.NaN);

        public string Operation { get; set; } = null;
        public string GCode { get; set; } = null;
    }
}
