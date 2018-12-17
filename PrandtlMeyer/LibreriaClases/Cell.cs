using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Resources;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using Microsoft.Win32;
using System.IO;

namespace LibreriaClases
{
    public class Cell
    {
        public Cell() { }


        public double Gamma, R;
        public double M, u, v, T, P, Rho, a, M_angle;
        public double F1, F2, F3, F4, G1, G2, G3, G4;
        public double x,y,y_t,y_s,h,dETA_x,dETA_y; //variables of grid
        public double F1_p,F2_p,F3_p,F4_p,G1_p,G2_p,G3_p,G4_p;
        public double dF1_x,dF2_x,dF3_x,dF4_x;
        public double dF1_p_x, dF2_p_x, dF3_p_x, dF4_p_x;
        //public double A_p, B_p, C_p, Rho_p, P_p;
        public double P_p,Rho_p;
        public double dF1_x_av, dF2_x_av, dF3_x_av, dF4_x_av;

        public Polygon polygon_u = new Polygon();
        public Polygon polygon_v = new Polygon();
        public Polygon polygon_rho = new Polygon();
        public Polygon polygon_P = new Polygon();
        public Polygon polygon_T = new Polygon();
        public Polygon polygon_M = new Polygon();



        public void calculateOnlyForFirstColumn()
        {
            F1 = Rho * u;
            G1 = Rho * v;
            F2 = Rho * Math.Pow(u, 2) + P;
            G2 = Rho * v * u;
            F3 = G2;
            G3 = Rho * Math.Pow(v, 2) + P;
            F4 = ((Gamma / (Gamma - 1)) * P * u) + (Rho * u * ((Math.Pow(u, 2) + Math.Pow(v, 2)) / 2));
            G4 = ((Gamma / (Gamma - 1)) * P * v) + (Rho * v * ((Math.Pow(u, 2) + Math.Pow(v, 2)) / 2));

        }
        /*
        public void calculatePrimitives() //Calculate the values of u,v,T,P,rho from F's and G's
        {
            double A, B, C;

            A = (Math.Pow(F3, 2) / (2 * F1)) - F4;
            B = (Gamma / (Gamma - 1)) * F1 * F2;
            C = -(((Gamma + 1) / (2 * (Gamma - 1))) * Math.Pow(F1, 3));

            Rho = (-(B) + Math.Sqrt(Math.Pow(B, 2) - (4 * A * C))) / (2 * A);
            u = F1 / Rho;
            v = F3 / F1;
            P = F2 - (F1 * u);
            T = (P) / (Rho * R);
        }*/

        public void calculateETA(double y_t_down,double delta_y_t) //calculate the ETA of our cell (vertical component of the grid in the computational plane)
        {
            y_t = y_t_down + delta_y_t;
        }

        public void calculateTransform(double H,double E,double Theta) //Checked
        {
            Theta = Theta*(Math.PI / 180);
            if (x < E)
            {
                y_s = 0;
                h = H;
            }
            else
            {
                y_s = -(x-E)*Math.Tan(Theta);
                //y_s = (-x * Math.Tan(Theta)) + (E * Math.Tan(Theta));
                h = H + (x-E)*Math.Tan(Theta);
                //h = H + (x * Math.Tan(Theta)) - (E * Math.Tan(Theta));
            }

            if (x < E)
            {
                dETA_x = 0;
            }
            else
            {
                double eta = (y - y_s) / h;
                dETA_x = (Math.Tan(Theta) / h) * (1 - eta);
                //dETA_x = (Math.Tan(Theta) / h) - (y_t * (Math.Tan(Theta) / h));//OJO REVISAR
            }
            y = (y_t * h) + y_s;
            dETA_y = 1 / h;
        }

        public double calculateTanMax(double Theta)//calculate max between Tan(theta+mu) and Tan(theta-mu)
        {
            double tan_max_1 = Math.Abs(Math.Tan((Theta * (Math.PI / 180) + M_angle)));
            double tan_max_2 = Math.Abs(Math.Tan((Theta * (Math.PI / 180) - M_angle)));
            double tan_max = Math.Max(tan_max_1,tan_max_2);
            return tan_max;
        }

        public double calculateStep(double C,double delta_y,double tan_max) //Calculate the value of delta_x for the calculation of step size, delta_y is calculated in class Matriz because is constant 
        {
            double delta_x = C * (delta_y / tan_max);
            return delta_x;
        }

        public double[] predictorStepBody(double F1_up,double F2_up,double F3_up,double F4_up,double G1_up,double G2_up,double G3_up,double G4_up,double F1_down,double F2_down,double F3_down,double F4_down,double P_up,double P_down,double delta_x,double delta_y_t)
        {
            dF1_x = ((dETA_x) * ((F1 - F1_up) / delta_y_t)) + ((dETA_y) * ((G1 - G1_up) / delta_y_t));
            dF2_x = ((dETA_x) * ((F2 - F2_up) / delta_y_t)) + ((dETA_y) * ((G2 - G2_up) / delta_y_t));
            dF3_x = ((dETA_x) * ((F3 - F3_up) / delta_y_t)) + ((dETA_y) * ((G3 - G3_up) / delta_y_t));
            dF4_x = ((dETA_x) * ((F4 - F4_up) / delta_y_t)) + ((dETA_y) * ((G4 - G4_up) / delta_y_t));
            double SF1 = ((0.6 * (Math.Abs((P_up) - (2 * P) + P_down))) / (P_up + 2 * P + P_down)) * (F1_up - (2 * F1) + F1_down); //P_next? and F's next????!!!!
            double SF2 = ((0.6 * (Math.Abs((P_up) - (2 * P) + P_down))) / (P_up + 2 * P + P_down)) * (F2_up - (2 * F2) + F2_down);
            double SF3 = ((0.6 * (Math.Abs((P_up) - (2 * P) + P_down))) / (P_up + 2 * P + P_down)) * (F3_up - (2 * F3) + F3_down);
            double SF4 = ((0.6 * (Math.Abs((P_up) - (2 * P) + P_down))) / (P_up + 2 * P + P_down)) * (F4_up - (2 * F4) + F4_down);
            double F1_p_next = F1 + (dF1_x * delta_x) + SF1;
            double F2_p_next = F2 + (dF2_x * delta_x) + SF2;
            double F3_p_next = F3 + (dF3_x * delta_x) + SF3;
            double F4_p_next = F4 + (dF4_x * delta_x) + SF4;
            double[] arrayF_p = { F1_p_next, F2_p_next, F3_p_next, F4_p_next };
            return arrayF_p;
        }

        public double[] predictorStepBoundaryDown(double F1_up, double F2_up, double F3_up, double F4_up, double G1_up, double G2_up, double G3_up, double G4_up, double delta_x, double delta_y_t)
        {
            dF1_x = (dETA_x * ((F1 - F1_up) / delta_y_t)) + (dETA_y * ((G1 - G1_up) / delta_y_t));
            dF2_x = (dETA_x * ((F2 - F2_up) / delta_y_t)) + (dETA_y * ((G2 - G2_up) / delta_y_t));
            dF3_x = (dETA_x * ((F3 - F3_up) / delta_y_t)) + (dETA_y * ((G3 - G3_up) / delta_y_t));
            dF4_x = (dETA_x * ((F4 - F4_up) / delta_y_t)) + (dETA_y * ((G4 - G4_up) / delta_y_t));
            double F1_p_next = F1 + (dF1_x * delta_x);
            double F2_p_next = F2 + (dF2_x * delta_x);
            double F3_p_next = F3 + (dF3_x * delta_x);
            double F4_p_next = F4 + (dF4_x * delta_x);
            double[] arrayF_p = { F1_p_next, F2_p_next, F3_p_next, F4_p_next };
            return arrayF_p;
        }

        public double[] predictorStepBoundaryUp(double F1_down, double F2_down, double F3_down, double F4_down, double G1_down, double G2_down, double G3_down, double G4_down, double delta_x, double delta_y_t)
        {
            dF1_x = (dETA_x * ((F1_down - F1) / delta_y_t)) + (dETA_y * ((G1_down - G1) / delta_y_t));
            dF2_x = (dETA_x * ((F2_down - F2) / delta_y_t)) + (dETA_y * ((G2_down - G2) / delta_y_t));
            dF3_x = (dETA_x * ((F3_down - F3) / delta_y_t)) + (dETA_y * ((G3_down - G3) / delta_y_t));
            dF4_x = (dETA_x * ((F4_down - F4) / delta_y_t)) + (dETA_y * ((G4_down - G4) / delta_y_t));
            double F1_p_next = F1 + (dF1_x * delta_x);
            double F2_p_next = F2 + (dF2_x * delta_x);
            double F3_p_next = F3 + (dF3_x * delta_x);
            double F4_p_next = F4 + (dF4_x * delta_x);
            double[] arrayF_p = {F1_p_next,F2_p_next,F3_p_next,F4_p_next};
            return arrayF_p;
        }

        public double[] calculateGPredicted(double F1_p_next,double F2_p_next,double F3_p_next,double F4_p_next)
        {
            double A_p = ((Math.Pow(F3_p_next,2)) / (2 * F1_p_next)) - F4_p_next;
            double B_p = (Gamma / (Gamma - 1)) * F1_p_next * F2_p_next;
            double C_p = -(((Gamma + 1) / (2 * (Gamma - 1))) * (Math.Pow(F1_p_next,3)));
            double Rho_p_next = (-B_p + (Math.Sqrt((Math.Pow(B_p,2)) - (4 * A_p * C_p)))) / (2 * A_p);
            double P_p_next = F2_p_next - ((Math.Pow(F1_p_next,2)) / Rho_p_next);
            double G1_p_next = Rho_p_next * (F3_p_next / F1_p_next);
            double G2_p_next = F3_p_next;
            double G3_p_next = (Rho_p_next * (Math.Pow((F3_p_next / F1_p_next) , 2))) + F2_p_next - ((Math.Pow(F1_p_next , 2)) / Rho_p_next);
            double G4_p_next = ((Gamma / (Gamma - 1)) * ((F2_p_next) - ((Math.Pow(F1_p_next, 2)) / Rho_p_next)) * (F3_p_next / F1_p_next)) + (((Rho_p_next * F3_p_next) / (2 * F1_p_next)) * ((Math.Pow((F1_p_next / Rho_p_next) , 2)) + (Math.Pow((F3_p_next / F1_p_next) , 2))));
            double[] arrayG_p = { G1_p_next, G2_p_next, G3_p_next, G4_p_next, Rho_p_next,P_p_next };
            return arrayG_p;
        }

        public double[] calculateCorrectorBody(double F1_p_next, double F1_p_next_up, double F1_p_next_down, double F2_p_next, double F2_p_next_up, double F2_p_next_down, double F3_p_next, double F3_p_next_up, double F3_p_next_down, double F4_p_next, double F4_p_next_up, double F4_p_next_down, double G1_p_next, double G1_p_next_down, double G2_p_next, double G2_p_next_down, double G3_p_next, double G3_p_next_down, double G4_p_next, double G4_p_next_down, double P_p_next, double P_p_next_up, double P_p_next_down, double delta_x, double delta_y_t)
        {
            double dF1_p_x_next = (dETA_x * ((F1_p_next_down - F1_p_next) / delta_y_t)) + (dETA_y * ((G1_p_next_down - G1_p_next) / delta_y_t));
            double dF2_p_x_next = (dETA_x * ((F2_p_next_down - F2_p_next) / delta_y_t)) + (dETA_y * ((G2_p_next_down - G2_p_next) / delta_y_t));
            double dF3_p_x_next = (dETA_x * ((F3_p_next_down - F3_p_next) / delta_y_t)) + (dETA_y * ((G3_p_next_down - G3_p_next) / delta_y_t));
            double dF4_p_x_next = (dETA_x * ((F4_p_next_down - F4_p_next) / delta_y_t)) + (dETA_y * ((G4_p_next_down - G4_p_next) / delta_y_t));

            double SF1_p_next = (((0.6 * (Math.Abs(P_p_next_up) - (2 * P_p_next) + P_p_next_down))) / (P_p_next_up + 2 * P_p_next + P_p_next_down)) * (F1_p_next_up - (2 * F1_p_next) + F1_p_next_down);//P_p son NEXT!!
            double SF2_p_next = (((0.6 * (Math.Abs(P_p_next_up) - (2 * P_p_next) + P_p_next_down))) / (P_p_next_up + 2 * P_p_next + P_p_next_down)) * (F2_p_next_up - (2 * F2_p_next) + F2_p_next_down);
            double SF3_p_next = (((0.6 * (Math.Abs(P_p_next_up) - (2 * P_p_next) + P_p_next_down))) / (P_p_next_up + 2 * P_p_next + P_p_next_down)) * (F3_p_next_up - (2 * F3_p_next) + F3_p_next_down);
            double SF4_p_next = (((0.6 * (Math.Abs(P_p_next_up) - (2 * P_p_next) + P_p_next_down))) / (P_p_next_up + 2 * P_p_next + P_p_next_down)) * (F4_p_next_up - (2 * F4_p_next) + F4_p_next_down);

            dF1_x_av = 0.5 * (dF1_x + dF1_p_x_next);
            dF2_x_av = 0.5 * (dF2_x + dF2_p_x_next);
            dF3_x_av = 0.5 * (dF3_x + dF3_p_x_next);
            dF4_x_av = 0.5 * (dF4_x + dF4_p_x_next);

            double F1_next = F1 + (dF1_x_av * delta_x) + SF1_p_next;
            double F2_next = F2 + (dF2_x_av * delta_x) + SF2_p_next;
            double F3_next = F3 + (dF3_x_av * delta_x) + SF3_p_next;
            double F4_next = F4 + (dF4_x_av * delta_x) + SF4_p_next;

            double[] arrayF_next = { F1_next, F2_next, F3_next, F4_next };
            return arrayF_next;
        }

        public double [] calculateCorrectorBoundaryDown(double F1_p_next, double F1_p_next_up,double F2_p_next, double F2_p_next_up,double F3_p_next, double F3_p_next_up,double F4_p_next, double F4_p_next_up,double G1_p_next, double G1_p_next_up,double G2_p_next, double G2_p_next_up,double G3_p_next, double G3_p_next_up,double G4_p_next, double G4_p_next_up,double delta_x,double delta_y_t)
        {
            double dF1_p_x_next = (dETA_x * ((F1_p_next - F1_p_next_up) / delta_y_t)) + (dETA_y * ((G1_p_next - G1_p_next_up) / delta_y_t));
            double dF2_p_x_next = (dETA_x * ((F2_p_next - F2_p_next_up) / delta_y_t)) + (dETA_y * ((G2_p_next - G2_p_next_up) / delta_y_t));
            double dF3_p_x_next = (dETA_x * ((F3_p_next - F3_p_next_up) / delta_y_t)) + (dETA_y * ((G3_p_next - G3_p_next_up) / delta_y_t));
            double dF4_p_x_next = (dETA_x * ((F4_p_next - F4_p_next_up) / delta_y_t)) + (dETA_y * ((G4_p_next - G4_p_next_up) / delta_y_t));

            double dF1_x_av = 0.5 * (dF1_x + dF1_p_x_next);
            double dF2_x_av = 0.5 * (dF2_x + dF2_p_x_next);
            double dF3_x_av = 0.5 * (dF3_x + dF3_p_x_next);
            double dF4_x_av = 0.5 * (dF4_x + dF4_p_x_next);

            double F1_next = F1 + (dF1_x_av * delta_x);
            double F2_next = F2 + (dF2_x_av * delta_x);
            double F3_next = F3 + (dF3_x_av * delta_x);
            double F4_next = F4 + (dF4_x_av * delta_x);

            double[] arrayF_next = { F1_next, F2_next, F3_next, F4_next };
            return arrayF_next;
        }

        public double[] calculateCorrectorBoundaryUp(double F1_p_next, double F1_p_next_down, double F2_p_next, double F2_p_next_down, double F3_p_next, double F3_p_next_down, double F4_p_next, double F4_p_next_down, double G1_p_next, double G1_p_next_down, double G2_p_next, double G2_p_next_down, double G3_p_next, double G3_p_next_down, double G4_p_next, double G4_p_next_down, double delta_x, double delta_y_t)
        {
            double dF1_p_x_next = (dETA_x * ((F1_p_next_down - F1_p_next) / delta_y_t)) + (dETA_y * ((G1_p_next_down - G1_p_next) / delta_y_t));
            double dF2_p_x_next = (dETA_x * ((F2_p_next_down - F2_p_next) / delta_y_t)) + (dETA_y * ((G2_p_next_down - G2_p_next) / delta_y_t));
            double dF3_p_x_next = (dETA_x * ((F3_p_next_down - F3_p_next) / delta_y_t)) + (dETA_y * ((G3_p_next_down - G3_p_next) / delta_y_t));
            double dF4_p_x_next = (dETA_x * ((F4_p_next_down - F4_p_next) / delta_y_t)) + (dETA_y * ((G4_p_next_down - G4_p_next) / delta_y_t));

            dF1_x_av = 0.5 * (dF1_x + dF1_p_x_next);
            dF2_x_av = 0.5 * (dF2_x + dF2_p_x_next);
            dF3_x_av = 0.5 * (dF3_x + dF3_p_x_next);
            dF4_x_av = 0.5 * (dF4_x + dF4_p_x_next);

            double F1_next = F1 + (dF1_x_av * delta_x);
            double F2_next = F2 + (dF2_x_av * delta_x);
            double F3_next = F3 + (dF3_x_av * delta_x);
            double F4_next = F4 + (dF4_x_av * delta_x);

            double[] arrayF_next = { F1_next, F2_next, F3_next, F4_next };
            return arrayF_next;
        }

        public void calculatePrimitivesAndGBody(double R_air) //Calculate the values of u,v,T,P,rho from F's and G's
        {
            /**double A, B, C;

            A = (Math.Pow(F3, 2) / (2 * Math.Pow(F1,1))) - F4;
            B = (Gamma / (Gamma - 1)) * F1 * F2;
            C = -(((Gamma + 1) / (2 * (Gamma - 1))) * Math.Pow(F1, 3));

            Rho = (-(B) + Math.Sqrt(Math.Pow(B, 2) - (4 * A * C))) / (2 * A);
            u = F1 / Rho;
            v = F3 / F1;
            P = F2 - (F1 * u);
            T = (P) / (Rho * R_air);
            a = Math.Sqrt(Gamma * R_air * T);
            M = (Math.Sqrt((Math.Pow(u , 2)) + (Math.Pow(v, 2)))) / a;
            M_angle = Math.Asin(1 / M);

            G1 = Rho * (F3 / F1);
            G2 = F3;
            G3 = (Rho * (Math.Pow((F3 / F1) , 2))) + F2 - ((Math.Pow(F1 , 2)) / Rho);
            G4 = ((Gamma / (Gamma - 1)) * ((F2) - ((Math.Pow(F1, 2)) / Rho)) * (F3 / F1)) + (((Rho * F3) / (2 * F1)) * ((Math.Pow((F1 / Rho) , 2)) + (Math.Pow((F3 / F1) , 2))));
        
             */
            double A, B, C;

            A = (Math.Pow(F3, 2) / (2 * F1)) - F4;
            B = (Gamma / (Gamma - 1)) * F1 * F2;
            C = -(((Gamma + 1) / (2 * (Gamma - 1))) * Math.Pow(F1, 3));

            Rho = (-(B) + Math.Sqrt(Math.Pow(B, 2) - (4 * A * C))) / (2 * A);
            u = F1 / Rho;
            v = F3 / F1;
            P = F2 - (F1 * u);
            T = (P) / (Rho * R_air);
            a = Math.Sqrt(Gamma * R_air * T);
            M = (Math.Sqrt((Math.Pow(u, 2)) + (Math.Pow(v, 2)))) / a;
            M_angle = Math.Asin(1 / M);

            G1 = Rho * (F3 / F1);
            G2 = F3;
            G3 = (Rho * (Math.Pow((F3 / F1), 2))) + F2 - ((Math.Pow(F1, 2)) / Rho);
            G4 = ((Gamma / (Gamma - 1)) * ((F2) - ((Math.Pow(F1, 2)) / Rho)) * (F3 / F1)) + (((Rho * F3) / (2 * F1)) * ((Math.Pow((F1 / Rho), 2)) + (Math.Pow((F3 / F1), 2))));
        
        }

        public void calculatePrimitivesBoundary(double R_air,double E,double Theta)
        {
            /*Theta = Theta * (Math.PI / 180);
            double A, B, C;

            A = (Math.Pow(F3, 2) / (2 * Math.Pow(F1,1))) - F4;
            B = (Gamma / (Gamma - 1)) * F1 * F2;
            C = -(((Gamma + 1) / (2 * (Gamma - 1))) * Math.Pow(F1, 3));

            double Rho_cal, u_cal, v_cal, P_cal, T_cal, M_cal, phi, f_cal, f_act;

            Rho_cal = (-(B) + Math.Sqrt(Math.Pow(B, 2) - (4 * A * C))) / (2 * A);
            u_cal = F1 / Rho_cal;
            v_cal = F3 / F1;
            P_cal = F2 - (F1 * u_cal);
            T_cal = P_cal / (R_air * Rho_cal);
            M_cal = (Math.Sqrt((Math.Pow(u_cal, 2)) + (Math.Pow(v_cal, 2)))) / Math.Sqrt(Gamma * R_air * T_cal);

            if (x < E)
            {
                phi = Math.Atan(v_cal / u_cal);
            }
            else
            {
                phi = Theta - (Math.Atan(Math.Abs(v_cal) / u_cal));
            }
            f_cal = Math.Sqrt((Gamma + 1) / (Gamma - 1)) * (Math.Atan(Math.Sqrt(((Gamma - 1) / (Gamma + 1)) * (Math.Pow(M_cal, 2) - 1)))) - (Math.Atan(Math.Sqrt(Math.Pow(M_cal, 2) - 1)));
            f_act = f_cal + phi;

            double a_int = 1.1; 
            double b_int = 2.9; 
            double precision = 0.0000001;
            double zero_f1 = Math.Sqrt((Gamma + 1) / (Gamma - 1)) * (Math.Atan(Math.Sqrt(((Gamma - 1) / (Gamma + 1)) * (Math.Pow(a_int, 2) - 1))))  - (Math.Atan(Math.Sqrt(Math.Pow(a_int, 2) - 1))) - f_act;
            double zero_f2 = Math.Sqrt((Gamma + 1) / (Gamma - 1)) * (Math.Atan(Math.Sqrt(((Gamma - 1) / (Gamma + 1)) * (Math.Pow(((a_int + b_int) / 2), 2) - 1))))  - (Math.Atan(Math.Sqrt(Math.Pow(((a_int + b_int) / 2), 2) - 1)))  - f_act;
            while ((b_int - a_int) / 2 > precision)
            {
                if (zero_f1 * zero_f2 <= 0)
                    b_int = (a_int + b_int) / 2;
                else
                    a_int = (a_int + b_int) / 2;
                zero_f1 = Math.Sqrt((Gamma + 1) / (Gamma - 1)) * (Math.Atan(Math.Sqrt(((Gamma - 1) / (Gamma + 1)) * (Math.Pow(a_int, 2) - 1))))  - (Math.Atan(Math.Sqrt(Math.Pow(a_int, 2) - 1)))  - f_act;
                zero_f2 = Math.Sqrt((Gamma + 1) / (Gamma - 1)) * (Math.Atan(Math.Sqrt(((Gamma - 1) / (Gamma + 1)) * (Math.Pow(((a_int + b_int) / 2), 2) - 1))))  - (Math.Atan(Math.Sqrt(Math.Pow(((a_int + b_int) / 2), 2) - 1)))  - f_act;
            }

            double M_act = (a_int + b_int)/2; 
            M = M_act;
            M_angle = (Math.Asin(1 / M));// *(180 / Math.PI);
            P = P_cal*(Math.Pow(((1 + ((Gamma - 1)/2)*(Math.Pow(M_cal,2)))/(1 + ((Gamma - 1)/2)*(Math.Pow(M_act,2)))),(Gamma/(Gamma - 1))));
            T = T_cal*((1 + ((Gamma - 1)/2)*(Math.Pow(M_cal,2)))/(1 + ((Gamma - 1)/2)*(Math.Pow(M_act,2))));
            Rho = P/(R_air*T);
            u = u_cal;
            a = Math.Sqrt(Gamma*R_air*T);

            if (x > E)
                v = -(u*Math.Tan(Theta));
            else
                v = 0;

            F1 = Rho * u;
            F2 = (Rho * (Math.Pow(u , 2))) + P;
            F3 = Rho * u * v;
            F4 = ((Gamma / (Gamma - 1)) * P * u) + (Rho * u * (((Math.Pow(u , 2)) + (Math.Pow(v , 2)))) / 2);

            G1 = Rho * (F3 / F1);
            G2 = F3;
            G3 = (Rho * (Math.Pow((F3 / F1), 2))) + F2 - ((Math.Pow(F1, 2)) / Rho);
            G4 = ((Gamma / (Gamma - 1)) * ((F2) - ((Math.Pow(F1, 2)) / Rho)) * (F3 / F1)) + (((Rho * F3) / (2 * F1)) * ((Math.Pow((F1 / Rho), 2)) + (Math.Pow((F3 / F1), 2))));

            */

            Theta = Theta * (Math.PI / 180);
            double A, B, C;

            A = (Math.Pow(F3, 2) / (2 * F1)) - F4;
            B = (Gamma / (Gamma - 1)) * F1 * F2;
            C = -(((Gamma + 1) / (2 * (Gamma - 1))) * Math.Pow(F1, 3));

            double Rho_cal, u_cal, v_cal, P_cal, T_cal, M_cal, phi, f_cal, f_act;

            Rho_cal = (-(B) + Math.Sqrt(Math.Pow(B, 2) - (4 * A * C))) / (2 * A);
            u_cal = F1 / Rho_cal;
            v_cal = F3 / F1;
            P_cal = F2 - (F1 * u_cal);
            T_cal = P_cal / (R_air * Rho_cal);
            M_cal = (Math.Sqrt((Math.Pow(u_cal, 2)) + (Math.Pow(v_cal, 2)))) / Math.Sqrt(Gamma * R_air * T_cal);

            if (x < E)
            {
                phi = Math.Atan(v_cal / u_cal);
            }
            else
            {
                phi = Theta - (Math.Atan(Math.Abs(v_cal) / u_cal));
            }
            f_cal = Math.Sqrt((Gamma + 1) / (Gamma - 1)) * (Math.Atan(Math.Sqrt(((Gamma - 1) / (Gamma + 1)) * (Math.Pow(M_cal, 2) - 1)))) - (Math.Atan(Math.Sqrt(Math.Pow(M_cal, 2) - 1)));
            f_act = f_cal + phi;

            double a_int = 1.1;
            double b_int = 2.9;
            double precision = 0.0000001;
            double zero_f1 = Math.Sqrt((Gamma + 1) / (Gamma - 1)) * (Math.Atan(Math.Sqrt(((Gamma - 1) / (Gamma + 1)) * (Math.Pow(a_int, 2) - 1)))) - (Math.Atan(Math.Sqrt(Math.Pow(a_int, 2) - 1))) - f_act;
            double zero_f2 = Math.Sqrt((Gamma + 1) / (Gamma - 1)) * (Math.Atan(Math.Sqrt(((Gamma - 1) / (Gamma + 1)) * (Math.Pow(((a_int + b_int) / 2), 2) - 1)))) - (Math.Atan(Math.Sqrt(Math.Pow(((a_int + b_int) / 2), 2) - 1))) - f_act;
            while ((b_int - a_int) / 2 > precision)
            {
                if (zero_f1 * zero_f2 <= 0)
                    b_int = (a_int + b_int) / 2;
                else
                    a_int = (a_int + b_int) / 2;
                zero_f1 = Math.Sqrt((Gamma + 1) / (Gamma - 1)) * (Math.Atan(Math.Sqrt(((Gamma - 1) / (Gamma + 1)) * (Math.Pow(a_int, 2) - 1)))) - (Math.Atan(Math.Sqrt(Math.Pow(a_int, 2) - 1))) - f_act;
                zero_f2 = Math.Sqrt((Gamma + 1) / (Gamma - 1)) * (Math.Atan(Math.Sqrt(((Gamma - 1) / (Gamma + 1)) * (Math.Pow(((a_int + b_int) / 2), 2) - 1)))) - (Math.Atan(Math.Sqrt(Math.Pow(((a_int + b_int) / 2), 2) - 1))) - f_act;
            }

            double M_act = (a_int + b_int) / 2;
            M = M_act;
            M_angle = (Math.Asin(1 / M));
            P = P_cal * (Math.Pow(((1 + ((Gamma - 1) / 2) * (Math.Pow(M_cal, 2))) / (1 + ((Gamma - 1) / 2) * (Math.Pow(M_act, 2)))), (Gamma / (Gamma - 1))));
            T = T_cal * ((1 + ((Gamma - 1) / 2) * (Math.Pow(M_cal, 2))) / (1 + ((Gamma - 1) / 2) * (Math.Pow(M_act, 2))));
            Rho = P / (R_air * T);
            u = u_cal;
            a = Math.Sqrt(Gamma * R_air * T);

            if (x > E)
                v = -(u * Math.Tan(Theta));
            else
                v = 0;

            F1 = Rho * u;
            F2 = (Rho * (Math.Pow(u, 2))) + P;
            F3 = Rho * u * v;
            F4 = ((Gamma / (Gamma - 1)) * P * u) + (Rho * u * (((Math.Pow(u, 2)) + (Math.Pow(v, 2)))) / 2);

            G1 = Rho * (F3 / F1);
            G2 = F3;
            G3 = (Rho * (Math.Pow((F3 / F1), 2))) + F2 - ((Math.Pow(F1, 2)) / Rho);
            G4 = ((Gamma / (Gamma - 1)) * ((F2) - ((Math.Pow(F1, 2)) / Rho)) * (F3 / F1)) + (((Rho * F3) / (2 * F1)) * ((Math.Pow((F1 / Rho), 2)) + (Math.Pow((F3 / F1), 2))));

        }

        public void calculatePoligon_u(double up_left_X, double up_left_Y, double up_right_X, double up_right_Y, double down_left_X, double down_left_Y, double down_right_X, double down_right_Y)
        {
            polygon_u.Stroke = Brushes.Transparent;
            //polygon_u.Fill = Brushes.Blue;
            polygon_u.StrokeThickness = 0;
            PointCollection pointCollection = new PointCollection();
            Point point1 = new Point(up_left_X * 7.5, up_left_Y * 7.5);
            Point point2 = new Point(up_right_X * 7.5, up_right_Y * 7.5);
            Point point3 = new Point(down_left_X * 7.5, down_left_Y * 7.5);
            Point point4 = new Point(down_right_X * 7.5, down_right_Y * 7.5);
            //polygon.Points = new PointCollection() { new Point(up_left_X, up_left_Y), new Point(up_right_X, up_right_Y), new Point(down_left_X, down_left_Y), new Point(down_right_X, down_right_Y) };
            //poligon.Fill = Brushes.AliceBlue;

            pointCollection.Add(point1);
            pointCollection.Add(point2);
            pointCollection.Add(point4);
            pointCollection.Add(point3);

            polygon_u.Points = pointCollection;
        }
        public void calculatePoligon_v(double up_left_X, double up_left_Y, double up_right_X, double up_right_Y, double down_left_X, double down_left_Y, double down_right_X, double down_right_Y)
        {
            polygon_v.Stroke = Brushes.Transparent;
            //polygon_v.Fill = Brushes.White;
            polygon_v.StrokeThickness = 0;
            PointCollection pointCollection = new PointCollection();
            Point point1 = new Point(up_left_X * 7.5, up_left_Y * 7.5);
            Point point2 = new Point(up_right_X * 7.5, up_right_Y * 7.5);
            Point point3 = new Point(down_left_X * 7.5, down_left_Y * 7.5);
            Point point4 = new Point(down_right_X * 7.5, down_right_Y * 7.5);
            //polygon.Points = new PointCollection() { new Point(up_left_X, up_left_Y), new Point(up_right_X, up_right_Y), new Point(down_left_X, down_left_Y), new Point(down_right_X, down_right_Y) };
            //poligon.Fill = Brushes.AliceBlue;

            pointCollection.Add(point1);
            pointCollection.Add(point2);
            pointCollection.Add(point4);
            pointCollection.Add(point3);

            polygon_v.Points = pointCollection;
        }
        public void calculatePoligon_rho(double up_left_X, double up_left_Y, double up_right_X, double up_right_Y, double down_left_X, double down_left_Y, double down_right_X, double down_right_Y)
        {
            polygon_rho.Stroke = Brushes.Transparent;
            //polygon_rho.Fill = Brushes.Black;
            polygon_rho.StrokeThickness = 0;
            PointCollection pointCollection = new PointCollection();
            Point point1 = new Point(up_left_X * 7.5, up_left_Y * 7.5);
            Point point2 = new Point(up_right_X * 7.5, up_right_Y * 7.5);
            Point point3 = new Point(down_left_X * 7.5, down_left_Y * 7.5);
            Point point4 = new Point(down_right_X * 7.5, down_right_Y * 7.5);
            //polygon.Points = new PointCollection() { new Point(up_left_X, up_left_Y), new Point(up_right_X, up_right_Y), new Point(down_left_X, down_left_Y), new Point(down_right_X, down_right_Y) };
            //poligon.Fill = Brushes.AliceBlue;

            pointCollection.Add(point1);
            pointCollection.Add(point2);
            pointCollection.Add(point4);
            pointCollection.Add(point3);

            polygon_rho.Points = pointCollection;

        }
        public void calculatePoligon_P(double up_left_X, double up_left_Y, double up_right_X, double up_right_Y, double down_left_X, double down_left_Y, double down_right_X, double down_right_Y)
        {
            polygon_P.Stroke = Brushes.Transparent;
            //polygon_P.Fill = Brushes.Pink;
            polygon_P.StrokeThickness = 0;
            PointCollection pointCollection = new PointCollection();
            Point point1 = new Point(up_left_X * 7.5, up_left_Y * 7.5);
            Point point2 = new Point(up_right_X * 7.5, up_right_Y * 7.5);
            Point point3 = new Point(down_left_X * 7.5, down_left_Y * 7.5);
            Point point4 = new Point(down_right_X * 7.5, down_right_Y * 7.5);
            //polygon.Points = new PointCollection() { new Point(up_left_X, up_left_Y), new Point(up_right_X, up_right_Y), new Point(down_left_X, down_left_Y), new Point(down_right_X, down_right_Y) };
            //poligon.Fill = Brushes.AliceBlue;

            pointCollection.Add(point1);
            pointCollection.Add(point2);
            pointCollection.Add(point4);
            pointCollection.Add(point3);

            polygon_P.Points = pointCollection;

        }
        public void calculatePoligon_T(double up_left_X, double up_left_Y, double up_right_X, double up_right_Y, double down_left_X, double down_left_Y, double down_right_X, double down_right_Y)
        {
            polygon_T.Stroke = Brushes.Transparent;
            //polygon_T.Fill = Brushes.Green;
            polygon_T.StrokeThickness = 0;
            PointCollection pointCollection = new PointCollection();
            Point point1 = new Point(up_left_X * 7.5, up_left_Y * 7.5);
            Point point2 = new Point(up_right_X * 7.5, up_right_Y * 7.5);
            Point point3 = new Point(down_left_X * 7.5, down_left_Y * 7.5);
            Point point4 = new Point(down_right_X * 7.5, down_right_Y * 7.5);
            //polygon.Points = new PointCollection() { new Point(up_left_X, up_left_Y), new Point(up_right_X, up_right_Y), new Point(down_left_X, down_left_Y), new Point(down_right_X, down_right_Y) };
            //poligon.Fill = Brushes.AliceBlue;

            pointCollection.Add(point1);
            pointCollection.Add(point2);
            pointCollection.Add(point4);
            pointCollection.Add(point3);

            polygon_T.Points = pointCollection;

        }
        public void calculatePoligon_M(double up_left_X, double up_left_Y, double up_right_X, double up_right_Y, double down_left_X, double down_left_Y, double down_right_X, double down_right_Y)
        {
            polygon_M.Stroke = Brushes.Transparent;
            //polygon_M.Fill = Brushes.Yellow;
            polygon_M.StrokeThickness = 0;
            PointCollection pointCollection = new PointCollection();
            Point point1 = new Point(up_left_X * 7.5, up_left_Y * 7.5);
            Point point2 = new Point(up_right_X * 7.5, up_right_Y * 7.5);
            Point point3 = new Point(down_left_X * 7.5, down_left_Y * 7.5);
            Point point4 = new Point(down_right_X * 7.5, down_right_Y * 7.5);
            //polygon.Points = new PointCollection() { new Point(up_left_X, up_left_Y), new Point(up_right_X, up_right_Y), new Point(down_left_X, down_left_Y), new Point(down_right_X, down_right_Y) };
            //poligon.Fill = Brushes.AliceBlue;

            pointCollection.Add(point1);
            pointCollection.Add(point2);
            pointCollection.Add(point4);
            pointCollection.Add(point3);

            polygon_M.Points = pointCollection;
        }


        public void colorPolygon_u(double min, double max)
        {
            double incremento = Math.Abs(max - min);
            double porcentage = ((u-min) / incremento) * 100;
            if (porcentage == 0)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
                polygon_u.Fill = brush;
            }
            else if (porcentage > 0 && porcentage < 10)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e6ffe6"));
                polygon_u.Fill = brush;
            }
            else if (porcentage > 10 && porcentage < 20)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ccffcc"));
                polygon_u.Fill = brush;
            }
            else if (porcentage > 20 && porcentage < 30)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b3ffb3"));
                polygon_u.Fill = brush;
            }
            else if (porcentage > 30 && porcentage < 40)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#99ff99"));
                polygon_u.Fill = brush;
            }
            else if (porcentage > 40 && porcentage < 50)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#80ff80"));
                polygon_u.Fill = brush;
            }
            else if (porcentage > 50 && porcentage < 60)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66ff66"));
                polygon_u.Fill = brush;
            }
            else if (porcentage > 60 && porcentage < 70)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4dff4d"));
                polygon_u.Fill = brush;
            }
            else if (porcentage > 70 && porcentage < 80)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33ff33"));
                polygon_u.Fill = brush;
            }
            else if (porcentage > 80 && porcentage < 90)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1aff1a"));
                polygon_u.Fill = brush;
            }
            else if (porcentage > 90 && porcentage < 100)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00ff00"));
                polygon_u.Fill = brush;
            }
            else if (porcentage == 100)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00e600"));
                polygon_u.Fill = brush;
            }

        }
        public void colorPolygon_v(double min, double max)
        {
            double incremento = Math.Abs(max - min);
            double porcentage = ((v-min) / incremento) * 100;
            if (porcentage == 0)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
                polygon_v.Fill = brush;
            }
            else if (porcentage > 0 && porcentage < 10)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e6f0ff"));
                polygon_v.Fill = brush;
            }
            else if (porcentage > 10 && porcentage < 20)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cce0ff"));
                polygon_v.Fill = brush;
            }
            else if (porcentage > 20 && porcentage < 30)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b3d1ff"));
                polygon_v.Fill = brush;
            }
            else if (porcentage > 30 && porcentage < 40)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b3d1ff"));
                polygon_v.Fill = brush;
            }
            else if (porcentage > 40 && porcentage < 50)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#80b3ff"));
                polygon_v.Fill = brush;
            }
            else if (porcentage > 50 && porcentage < 60)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66a3ff"));
                polygon_v.Fill = brush;
            }
            else if (porcentage > 60 && porcentage < 70)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4d94ff"));
                polygon_v.Fill = brush;
            }
            else if (porcentage > 70 && porcentage < 80)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3385ff"));
                polygon_v.Fill = brush;
            }
            else if (porcentage > 80 && porcentage < 90)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1a75ff"));
                polygon_v.Fill = brush;
            }
            else if (porcentage > 90 && porcentage < 100)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0066ff"));
                polygon_v.Fill = brush;
            }
            else if (porcentage == 100)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005ce6"));
                polygon_v.Fill = brush;
            }
        }
        public void colorPolygon_rho(double min, double max)
        {
            double incremento = Math.Abs(max - min);
            double porcentage = ((Rho-min) / incremento) * 100;
            if (porcentage == 0)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
                polygon_rho.Fill = brush;
            }
            else if (porcentage > 0 && porcentage < 10)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fffae6"));
                polygon_rho.Fill = brush;
            }
            else if (porcentage > 10 && porcentage < 20)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff5cc"));
                polygon_rho.Fill = brush;
            }
            else if (porcentage > 20 && porcentage < 30)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff0b3"));
                polygon_rho.Fill = brush;
            }
            else if (porcentage > 30 && porcentage < 40)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffeb99"));
                polygon_rho.Fill = brush;
            }
            else if (porcentage > 40 && porcentage < 50)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffe680"));
                polygon_rho.Fill = brush;
            }
            else if (porcentage > 50 && porcentage < 60)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffe066"));
                polygon_rho.Fill = brush;
            }
            else if (porcentage > 60 && porcentage < 70)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffdb4d"));
                polygon_rho.Fill = brush;
            }
            else if (porcentage > 70 && porcentage < 80)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffd633"));
                polygon_rho.Fill = brush;
            }
            else if (porcentage > 80 && porcentage < 90)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffd11a"));
                polygon_rho.Fill = brush;
            }
            else if (porcentage > 90 && porcentage < 100)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffcc00"));
                polygon_rho.Fill = brush;
            }
            else if (porcentage == 100)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e6b800"));
                polygon_rho.Fill = brush;
            }
        }
        public void colorPolygon_P(double min, double max)
        {
            double incremento = Math.Abs(max - min);
            double porcentage = ((P-min) / incremento) * 100;
            if (porcentage == 0)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
                polygon_P.Fill = brush;
            }
            else if (porcentage > 0 && porcentage < 10)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffe6f0"));
                polygon_P.Fill = brush;
            }
            else if (porcentage > 10 && porcentage < 20)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffcce0"));
                polygon_P.Fill = brush;
            }
            else if (porcentage > 20 && porcentage < 30)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffb3d1"));
                polygon_P.Fill = brush;
            }
            else if (porcentage > 30 && porcentage < 40)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff99c2"));
                polygon_P.Fill = brush;
            }
            else if (porcentage > 40 && porcentage < 50)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff80b3"));
                polygon_P.Fill = brush;
            }
            else if (porcentage > 50 && porcentage < 60)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff66a3"));
                polygon_P.Fill = brush;
            }
            else if (porcentage > 60 && porcentage < 70)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff4d94"));
                polygon_P.Fill = brush;
            }
            else if (porcentage > 70 && porcentage < 80)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff3385"));
                polygon_P.Fill = brush;
            }
            else if (porcentage > 80 && porcentage < 90)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff1a75"));
                polygon_P.Fill = brush;
            }
            else if (porcentage > 90 && porcentage < 100)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff0066"));
                polygon_P.Fill = brush;
            }
            else if (porcentage == 100)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e6005c"));
                polygon_P.Fill = brush;
            }
        }
        public void colorPolygon_T(double min, double max)
        {
            double incremento = Math.Abs(max - min);
            double porcentage = ((T-min) / incremento) * 100;
            if (porcentage == 0)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
                polygon_T.Fill = brush;
            }
            else if (porcentage > 0 && porcentage < 10)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffe6e6"));
                polygon_T.Fill = brush;
            }
            else if (porcentage > 10 && porcentage < 20)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffcccc"));
                polygon_T.Fill = brush;
            }
            else if (porcentage > 20 && porcentage < 30)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffb3b3"));
                polygon_T.Fill = brush;
            }
            else if (porcentage > 30 && porcentage < 40)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff9999"));
                polygon_T.Fill = brush;
            }
            else if (porcentage > 40 && porcentage < 50)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff8080"));
                polygon_T.Fill = brush;
            }
            else if (porcentage > 50 && porcentage < 60)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff6666"));
                polygon_T.Fill = brush;
            }
            else if (porcentage > 60 && porcentage < 70)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff4d4d"));
                polygon_T.Fill = brush;
            }
            else if (porcentage > 70 && porcentage < 80)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff3333"));
                polygon_T.Fill = brush;
            }
            else if (porcentage > 80 && porcentage < 90)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff1a1a"));
                polygon_T.Fill = brush;
            }
            else if (porcentage > 90 && porcentage < 100)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff0000"));
                polygon_T.Fill = brush;
            }
            else if (porcentage == 100)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e60000"));
                polygon_T.Fill = brush;
            }
        }
        public void colorPolygon_M(double min, double max)
        {
            double incremento = Math.Abs(max - min);
            double porcentage = ((M-min) / incremento) * 100;
            if (porcentage == 0)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
                polygon_M.Fill = brush;
            }
            else if (porcentage > 0 && porcentage < 10)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f9f2ec"));
                polygon_M.Fill = brush;
            }
            else if (porcentage > 10 && porcentage < 20)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f2e6d9"));
                polygon_M.Fill = brush;
            }
            else if (porcentage > 20 && porcentage < 30)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ecd9c6"));
                polygon_M.Fill = brush;
            }
            else if (porcentage > 30 && porcentage < 40)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e6ccb3"));
                polygon_M.Fill = brush;
            }
            else if (porcentage > 40 && porcentage < 50)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dfbf9f"));
                polygon_M.Fill = brush;
            }
            else if (porcentage > 50 && porcentage < 60)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d9b38c"));
                polygon_M.Fill = brush;
            }
            else if (porcentage > 60 && porcentage < 70)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d2a679"));
                polygon_M.Fill = brush;
            }
            else if (porcentage > 70 && porcentage < 80)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cc9966"));
                polygon_M.Fill = brush;
            }
            else if (porcentage > 80 && porcentage < 90)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c68c53"));
                polygon_M.Fill = brush;
            }
            else if (porcentage > 90 && porcentage < 100)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bf8040"));
                polygon_M.Fill = brush;
            }
            else if (porcentage == 100)
            {
                SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ac7339"));
                polygon_M.Fill = brush;
            }
        }



        /*public Cell(double M, double P, double rho, double T,double gamma)
        {
            this.M = M;
            this.P = P;
            this.rho = rho;
            this.T = T;
            this.gamma = gamma;
        }

        public Rectangle rectangle = new Rectangle();
        double M, P, rho, T, gamma,u;
        double theta = 5.532;
        double R = 8.314;
        double molar_mass = 0.0289;
        double v = 0;
        double F1, F2, F3, F4, G1, G2, G3, G4;

        public void calcularRectangle(double H, double M)
        {
            double mu = Math.Asin(1 / M)*(180/Math.PI);
            rectangle.Width = Math.Max(((H / 40) / Math.Tan((theta + mu)* (Math.PI/180))), ((H / 40) / Math.Tan((theta - mu) * (Math.PI/180))));
            rectangle.Height = H / 40;
        }

        public void Initialize() //only for first column
        {
            u = M*(Math.Sqrt((gamma*R*T)/molar_mass));
            F1 = rho*u;
            F2 = (rho * Math.Pow(u, 2)) + P;
            F3 = rho * u * v;
            F4 = ((gamma / (gamma - 1)) * (rho * u)) + ((rho * u) * ((Math.Pow(u, 2) + Math.Pow(v, 2)) / 2));

            double A = (Math.Pow(F3, 2) / (2 * F1)) - F4;
            double B = (gamma / (gamma - 1)) * F1 * F2;
            double C = -(((gamma + 1) / (2 * (gamma - 1))) * Math.Pow(F1, 3));

            rho = (-B + Math.Sqrt(Math.Pow(B,2)-(4*A*C))) / (2 * A);
            u = F1 / rho;
            v = F3 / F1;
            P = F2 - F1 * u;
            T = P / (rho * 287);

            G1 = rho * (F3 / F1);
            G2 = F3;
            G3 = rho * Math.Pow((F3 / F1), 2) + F2 - (Math.Pow(F1, 2) / rho);
            G4 = ((gamma / (gamma - 1)) * (F2 - (Math.Pow(F1, 2) / rho)) * (F3 / F1)) + ((rho / 2) * (F3 / F1) * (Math.Pow((F1 / rho), 2) + Math.Pow((F3 / F1), 2)));
        }

        public void calculate()
        {

        }
          */
    }


}
