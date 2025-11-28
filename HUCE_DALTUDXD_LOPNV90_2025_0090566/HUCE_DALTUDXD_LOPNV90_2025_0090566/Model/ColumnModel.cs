using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Model
{
    public class Column
    {
        public double Width { get; set; }      // b (mm)
        public double Height { get; set; }     // h (mm)
        public double AxialForce { get; set; } // N (kN)
        public double MomentX { get; set; }    // Mx (kNm)
        public double MomentY { get; set; }    // My (kNm)

        public Rebar Rebar { get; set; }

        public Column(double width, double height, Rebar rebar)
        {
            Width = width;
            Height = height;
            Rebar = rebar;
        }
    }
}