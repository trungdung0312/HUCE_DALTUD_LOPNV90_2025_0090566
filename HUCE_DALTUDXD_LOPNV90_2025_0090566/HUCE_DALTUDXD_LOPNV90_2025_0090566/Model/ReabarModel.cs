using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUCE_DALTUDXD_LOPNV90_2025_0090566.Model
{
    public class Rebar
    {
        public int Diameter { get; set; }   // Đường kính mm
        public int Quantity { get; set; }   // Số lượng – n
        public double Area => Quantity * Math.PI * Math.Pow(Diameter / 2.0, 2);  // mm2

        public Rebar(int diameter, int quantity)
        {
            Diameter = diameter;
            Quantity = quantity;
        }
    }
}