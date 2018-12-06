using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace ClassLibrary
{
    public class Cell
    {
        public Cell() { }
        public Cell(double M, double P, double rho, double T) 
        {
            this.M = M;
            this.P = P;
            this.rho = rho;
            this.T = T;
        }

        public Rectangle rectangle;
        double M, P, rho, T;
        double theta = 5.532;

        public void calcularRectangle(double H,double M)
        {
            double mu = Math.Pow(Math.Sin(1 / M), -1);
            rectangle.Width = Math.Max(((H/40)/Math.Tan(theta+mu)),((H/40)/Math.Tan(theta-mu)));
            rectangle.Height = H / 40;
        }
    }

    
}
