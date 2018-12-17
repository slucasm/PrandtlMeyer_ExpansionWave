using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Data;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace LibreriaClases
{
    public class Matriz
    {
        public Matriz() { }

        //public int rows = 41;
        //public int columns = 89;


        public int rows, columns;
        public double u_in, a_in, v_in, Rho_in, M_in, P_in, T_in, Gamma, E, theta, R_air;
        public Cell[,] matrix;

        //public Cell[,] matrix = new Cell[rows,columns];

        static double C = 0.5; //Courant number
        static double H = 40;
        //public double theta = 5.352;
        static double L = 65;
        //public double E = 10;
        //public double M_in = 2;
        //public double P_in = 1.01*Math.Pow(10,5);
        //public double Rho_in = 1.23; 
        //public double R_air = 287; 
        //public double Gamma = 1.4;
        //public double T_in = 286.1;
        //double a_in = Math.Sqrt(Gamma * R_air * T_in);
        //double v_in = 0;
        //double u_in = M_in * a_in;

        double delta_y_t = 0.025;

        double delta_x;
        double delta_y;

        double delta_xi;

        DataTable table_u = new DataTable();
        DataTable table_v = new DataTable();
        DataTable table_rho = new DataTable();
        DataTable table_P = new DataTable();
        DataTable table_T = new DataTable();
        DataTable table_M = new DataTable();

        List<Cell> listCells = new List<Cell>();

        public void setParameters(int rows, int columns, double rho, double P, double T, double M, double R, double gamma, double E, double theta)
        {
            this.rows = rows;
            this.columns = columns;
            this.Rho_in = rho;
            this.P_in = P;
            this.T_in = T;
            this.M_in = M;
            this.R_air = R;
            this.Gamma = gamma;
            this.E = E;
            this.theta = theta;
            this.a_in = Math.Sqrt(Gamma * R_air * T_in);
            this.v_in = 0;
            this.u_in = M_in * a_in;

            this.matrix = new Cell[rows, columns];

        }
        

        public void Initialize() //We write the initial values for the first column
        {
            for (int a = 0; a < rows; a++)
            {
                for (int b = 0; b < columns; b++)
                {
                    matrix[a, b] = new Cell();
                    matrix[a, b].Gamma = Gamma;
                }
            }
            for (int i = 0; i < rows; i++)
            {
                matrix[i, 0].Gamma = Gamma;
                matrix[i, 0].R = R_air;
                matrix[i, 0].M = M_in;
                matrix[i, 0].u = u_in;
                matrix[i, 0].v = v_in;
                matrix[i, 0].T = T_in;
                matrix[i, 0].P = P_in;
                matrix[i, 0].Rho = Rho_in;    
                matrix[i, 0].a = a_in;
                matrix[i, 0].M_angle = Math.Asin(1 / M_in);
                matrix[i, 0].calculateOnlyForFirstColumn();//Calculate the values of F1,F2,F3,F4,G1,G2,G3,G4 for the first column
                
            }
            //Now we calculate the Eta for every cell in our matrix
            for (int x = 1; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    matrix[x, y].calculateETA(matrix[x - 1, y].y_t, delta_y_t);
                }
            }
        }

        public void calculate()
        {
            //for (int j = 0; j<columns-1; j++)
            for (int j = 0; matrix[0,j].x <= L; j++)
            {
                for (int i = 0; i < rows; i++)//First que calculate values for transformation grid
                {
                    matrix[i, j].calculateTransform(H, E, theta);
                }

                double[] max_tan_Array = new double[rows];//

                for (int y = 0; y < rows; y++)//Now we calculate the delta_x, that is the step size
                {
                    delta_y = matrix[2, j].y - matrix[1, j].y;
                    //double[] max_tan_Array = new double[rows];
                    max_tan_Array[y] = matrix[y, j].calculateTanMax(theta);
                    //double max_tan = max_tan_Array.Max();
                    //delta_x = matrix[y, j].calculateStep(C, delta_y, max_tan);
                }

                double max_tan = max_tan_Array.Max();//
                delta_x = C*(delta_y/max_tan);//
                delta_xi = delta_x;
                for (int e = 0; e < rows; e++)
                {
                    matrix[e, j + 1].x = matrix[e, j].x + delta_x;
                }

                for (int b = 0; b < rows; b++)
                {
                    double[] arrayF_p;
                    if (b == 0)
                    {
                        arrayF_p = matrix[b, j].predictorStepBoundaryDown(matrix[b + 1, j].F1, matrix[b + 1, j].F2, matrix[b + 1, j].F3, matrix[b + 1, j].F4, matrix[b + 1, j].G1, matrix[b + 1, j].G2, matrix[b + 1, j].G3, matrix[b + 1, j].G4, delta_xi, delta_y_t);

                    }
                    else if (b == rows-1)
                    {
                        arrayF_p = matrix[b, j].predictorStepBoundaryUp(matrix[b - 1, j].F1, matrix[b - 1, j].F2, matrix[b - 1, j].F3, matrix[b - 1, j].F4, matrix[b - 1, j].G1, matrix[b - 1, j].G2, matrix[b - 1, j].G3, matrix[b - 1, j].G4, delta_xi, delta_y_t);
                    }
                    else
                    {
                        arrayF_p = matrix[b, j].predictorStepBody(matrix[b + 1, j].F1, matrix[b + 1, j].F2, matrix[b + 1, j].F3, matrix[b + 1, j].F4, matrix[b + 1, j].G1, matrix[b + 1, j].G2, matrix[b + 1, j].G3, matrix[b + 1, j].G4, matrix[b - 1, j].F1, matrix[b - 1, j].F2, matrix[b - 1, j].F3, matrix[b - 1, j].F4, matrix[b + 1, j].P, matrix[b - 1, j].P, delta_xi, delta_y_t);
                    }
                    matrix[b, j + 1].F1_p = arrayF_p[0];
                    matrix[b, j + 1].F2_p = arrayF_p[1];
                    matrix[b, j + 1].F3_p = arrayF_p[2];
                    matrix[b, j + 1].F4_p = arrayF_p[3];
                }

                for (int s = 0; s < rows; s++)
                {
                    double[] arrayG_p = matrix[s, j].calculateGPredicted(matrix[s, j + 1].F1_p, matrix[s, j + 1].F2_p, matrix[s, j + 1].F3_p, matrix[s, j + 1].F4_p);
                    matrix[s, j + 1].G1_p = arrayG_p[0];
                    matrix[s, j + 1].G2_p = arrayG_p[1];
                    matrix[s, j + 1].G3_p = arrayG_p[2];
                    matrix[s, j + 1].G4_p = arrayG_p[3];
                    matrix[s, j + 1].Rho_p = arrayG_p[4];
                    matrix[s, j + 1].P_p = arrayG_p[5];
                }

                for (int c = 0; c < rows; c++)
                {
                    double[] arrayF;
                    if (c == 0)
                    {
                        arrayF = matrix[c, j].calculateCorrectorBoundaryDown(matrix[c, j + 1].F1_p, matrix[c + 1, j + 1].F1_p, matrix[c, j + 1].F2_p, matrix[c + 1, j + 1].F2_p, matrix[c, j + 1].F3_p, matrix[c + 1, j + 1].F3_p, matrix[c, j + 1].F4_p, matrix[c + 1, j + 1].F4_p, matrix[c, j + 1].G1_p, matrix[c + 1, j + 1].G1_p, matrix[c, j + 1].G2_p, matrix[c + 1, j + 1].G2_p, matrix[c, j + 1].G3_p, matrix[c + 1, j + 1].G3_p, matrix[c, j + 1].G4_p, matrix[c + 1, j + 1].G4_p, delta_xi, delta_y_t);
                    }
                    else if (c == rows-1)
                    {
                        arrayF = matrix[c, j].calculateCorrectorBoundaryUp(matrix[c, j + 1].F1_p, matrix[c - 1, j + 1].F1_p, matrix[c, j + 1].F2_p, matrix[c - 1, j + 1].F2_p, matrix[c, j + 1].F3_p, matrix[c - 1, j + 1].F3_p, matrix[c, j + 1].F4_p, matrix[c - 1, j + 1].F4_p, matrix[c, j + 1].G1_p, matrix[c - 1, j + 1].G1_p, matrix[c, j + 1].G2_p, matrix[c - 1, j + 1].G2_p, matrix[c, j + 1].G3_p, matrix[c - 1, j + 1].G3_p, matrix[c, j + 1].G4_p, matrix[c - 1, j + 1].G4_p, delta_xi, delta_y_t);
                    }
                    else
                    {
                        arrayF = matrix[c, j].calculateCorrectorBody(matrix[c, j + 1].F1_p, matrix[c + 1, j + 1].F1_p, matrix[c - 1, j + 1].F1_p, matrix[c, j + 1].F2_p, matrix[c + 1, j + 1].F2_p, matrix[c - 1, j + 1].F2_p, matrix[c, j + 1].F3_p, matrix[c + 1, j + 1].F3_p, matrix[c - 1, j + 1].F3_p, matrix[c, j + 1].F4_p, matrix[c + 1, j + 1].F4_p, matrix[c - 1, j + 1].F4_p, matrix[c, j + 1].G1_p, matrix[c - 1, j + 1].G1_p, matrix[c, j + 1].G2_p, matrix[c - 1, j + 1].G2_p, matrix[c, j + 1].G3_p, matrix[c - 1, j + 1].G3_p, matrix[c, j + 1].G4_p, matrix[c - 1, j + 1].G4_p,matrix[c,j+1].P_p, matrix[c + 1, j+1].P_p, matrix[c - 1, j+1].P_p, delta_xi, delta_y_t);
                    }

                    matrix[c, j + 1].F1 = arrayF[0];
                    matrix[c, j + 1].F2 = arrayF[1];
                    matrix[c, j + 1].F3 = arrayF[2];
                    matrix[c, j + 1].F4 = arrayF[3];

                }

                for (int d = 0; d < rows; d++)
                {
                    if (d == 0)
                    {
                        matrix[d, j + 1].calculatePrimitivesBoundary(R_air, E, theta);
                    }
                    else
                    {
                        matrix[d, j + 1].calculatePrimitivesAndGBody(R_air);
                    }

                }

                

            }
        }


        public void calculatePoligons()
        {
            double[] arrayDeltaY = new double[columns];
            for (int j = 0; j < columns; j++)
            {
                arrayDeltaY[j] = matrix[2, j].y - matrix[1, j].y;
            }
            

            double up_left_X, up_left_Y, up_right_X, up_right_Y,down_left_X,down_left_Y,down_right_X,down_right_Y;

            up_left_X = 0;
            down_left_X = 0;
            for (int j = 0; j < columns-1; j++)
            {
                up_right_X = up_left_X + delta_x;
                down_right_X = up_right_X;
                up_left_Y = 0;
                up_right_Y = 0;
                for (int i = rows-1; i > 0; i--)
                {
                    down_left_Y = up_left_Y + arrayDeltaY[j];
                    down_right_Y = up_right_Y + arrayDeltaY[j + 1];
                    matrix[i, j].calculatePoligon_u(up_left_X,up_left_Y,up_right_X,up_right_Y,down_left_X,down_left_Y,down_right_X,down_right_Y);
                    matrix[i, j].calculatePoligon_v(up_left_X, up_left_Y, up_right_X, up_right_Y, down_left_X, down_left_Y, down_right_X, down_right_Y);
                    matrix[i, j].calculatePoligon_rho(up_left_X, up_left_Y, up_right_X, up_right_Y, down_left_X, down_left_Y, down_right_X, down_right_Y);
                    matrix[i, j].calculatePoligon_P(up_left_X, up_left_Y, up_right_X, up_right_Y, down_left_X, down_left_Y, down_right_X, down_right_Y);
                    matrix[i, j].calculatePoligon_T(up_left_X, up_left_Y, up_right_X, up_right_Y, down_left_X, down_left_Y, down_right_X, down_right_Y);
                    matrix[i, j].calculatePoligon_M(up_left_X, up_left_Y, up_right_X, up_right_Y, down_left_X, down_left_Y, down_right_X, down_right_Y);
                    up_left_Y = down_left_Y;
                    up_right_Y = down_right_Y;
                }
                up_left_X = up_right_X;
                down_left_X = down_right_X;
            }
        }


        public void colorPolygons()
        {
            List<Cell> listaCells = new List<Cell>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    listaCells.Add(matrix[i, j]);
                }
            }

            double max_u = double.MinValue;
            double max_v = double.MinValue;
            double max_rho = double.MinValue;
            double max_P = double.MinValue;
            double max_T = double.MinValue;
            double max_M = double.MinValue;
            double min_u = double.MaxValue;
            double min_v = double.MaxValue;
            double min_rho = double.MaxValue;
            double min_P = double.MaxValue;
            double min_T = double.MaxValue;
            double min_M = double.MaxValue;

            foreach (Cell cell in listaCells)
            {
                if (cell.u > max_u)
                {
                    max_u = cell.u;
                }
                if (cell.v > max_v)
                {
                    max_v = cell.v;
                }
                if (cell.Rho > max_rho)
                {
                    max_rho = cell.Rho;
                }
                if (cell.P > max_P)
                {
                    max_P = cell.P;
                }
                if (cell.T > max_T)
                {
                    max_T = cell.T;
                }
                if (cell.M > max_M)
                {
                    max_M = cell.M;
                }

                if (cell.u < min_u)
                {
                    min_u = cell.u;
                }
                if (cell.v < min_v)
                {
                    min_v = cell.v;
                }
                if (cell.Rho < min_rho)
                {
                    min_rho = cell.Rho;
                }
                if (cell.P < min_P)
                {
                    min_P = cell.P;
                }
                if (cell.T < min_T)
                {
                    min_T = cell.T;
                }
                if (cell.M < min_M)
                {
                    min_M = cell.M;
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    matrix[i, j].colorPolygon_u(min_u, max_u);
                    matrix[i, j].colorPolygon_v(min_v, max_v);
                    matrix[i, j].colorPolygon_rho(min_rho, max_rho);
                    matrix[i, j].colorPolygon_P(min_P, max_P);
                    matrix[i, j].colorPolygon_T(min_T, max_T);
                    matrix[i, j].colorPolygon_M(min_M, max_M);
                }
            }

            

        }


        public List<List<Polygon>> getListPolygons()
        {
            List<List<Polygon>> listPolygons = new List<List<Polygon>>();
            List<Polygon> listPolygons_u = new List<Polygon>();
            List<Polygon> listPolygons_v = new List<Polygon>();
            List<Polygon> listPolygons_rho = new List<Polygon>();
            List<Polygon> listPolygons_P = new List<Polygon>();
            List<Polygon> listPolygons_T = new List<Polygon>();
            List<Polygon> listPolygons_M = new List<Polygon>();
            int z = 0;

            for (int j = 0; j < columns - 1; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    listCells.Add(matrix[i, j]);
                    matrix[i, j].polygon_u.DataContext = z;
                    listPolygons_u.Add(matrix[i, j].polygon_u);
                    matrix[i, j].polygon_v.DataContext = z;
                    listPolygons_v.Add(matrix[i, j].polygon_v);
                    matrix[i, j].polygon_rho.DataContext = z;
                    listPolygons_rho.Add(matrix[i, j].polygon_rho);
                    matrix[i, j].polygon_P.DataContext = z;
                    listPolygons_P.Add(matrix[i, j].polygon_P);
                    matrix[i, j].polygon_T.DataContext = z;
                    listPolygons_T.Add(matrix[i, j].polygon_T);
                    matrix[i, j].polygon_M.DataContext = z;
                    listPolygons_M.Add(matrix[i, j].polygon_M);
                    z++;
                }
            }

            listPolygons.Add(listPolygons_u);
            listPolygons.Add(listPolygons_v);
            listPolygons.Add(listPolygons_rho);
            listPolygons.Add(listPolygons_P);
            listPolygons.Add(listPolygons_T);
            listPolygons.Add(listPolygons_M);
            return listPolygons;
        }

        public List<Cell> getListCells()
        {
            return listCells;
        }

        public List<DataTable> createTables()
        {

            for (int i = 0; i < columns; i++)
            {
               
                DataColumn index_u = new DataColumn(Convert.ToString(i), typeof(double));
                DataColumn index_v = new DataColumn(Convert.ToString(i), typeof(double));
                DataColumn index_rho = new DataColumn(Convert.ToString(i), typeof(double));
                DataColumn index_P = new DataColumn(Convert.ToString(i), typeof(double));
                DataColumn index_T = new DataColumn(Convert.ToString(i), typeof(double));
                DataColumn index_M = new DataColumn(Convert.ToString(i), typeof(double));

                table_u.Columns.Add(index_u);
                table_v.Columns.Add(index_v);
                table_rho.Columns.Add(index_rho);
                table_P.Columns.Add(index_P);
                table_T.Columns.Add(index_T);
                table_M.Columns.Add(index_M);

            }
            for (int i = 0; i < rows; i++)
            {
                DataRow row_u = table_u.NewRow();
                DataRow row_v = table_v.NewRow();
                DataRow row_rho = table_rho.NewRow();
                DataRow row_P = table_P.NewRow();
                DataRow row_T = table_T.NewRow();
                DataRow row_M = table_M.NewRow();

                

                for (int j = 0; j < columns; j++)
                {
                    
                    row_u[j] = Math.Round(matrix[i, j].u,3);
                    row_v[j] = Math.Round(matrix[i, j].v,3);
                    row_rho[j] = Math.Round(matrix[i, j].Rho,3);
                    row_P[j] = Math.Round(matrix[i, j].P,3);
                    row_T[j] = Math.Round(matrix[i, j].T,3);
                    row_M[j] = Math.Round(matrix[i, j].M,3);
                }
                table_u.Rows.Add(row_u);
                table_v.Rows.Add(row_v);
                table_rho.Rows.Add(row_rho);
                table_P.Rows.Add(row_P);
                table_T.Rows.Add(row_T);
                table_M.Rows.Add(row_M);
            }

            List<DataTable> listTables = new List<DataTable>();
            listTables.Add(table_u);
            listTables.Add(table_v);
            listTables.Add(table_rho);
            listTables.Add(table_P);
            listTables.Add(table_T);
            listTables.Add(table_M);

            return listTables;

        }

        public void clearTables()
        {
            table_u.Clear();
            table_v.Clear();
            table_rho.Clear();
            table_P.Clear();
            table_T.Clear();
            table_M.Clear();

        }



        public void Save()
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.DefaultExt = "txt";
            ofd.Filter = "Archivos txt(*.txt)|*.txt";
            ofd.Title = "Guarda los datos";
            ofd.ShowDialog();
            string nombre = ofd.FileName;
            StreamWriter fichero = new StreamWriter(nombre);
            fichero.Write(rows);
            fichero.Write("\r\n");
            fichero.Write(columns);
            fichero.Write("\r\n");
            fichero.Write(Rho_in);
            fichero.Write("\r\n");
            fichero.Write(P_in);
            fichero.Write("\r\n");
            fichero.Write(T_in);
            fichero.Write("\r\n");
            fichero.Write(M_in);
            fichero.Write("\r\n");
            fichero.Write(R_air);
            fichero.Write("\r\n");
            fichero.Write(Gamma);
            fichero.Write("\r\n");
            fichero.Write(E);
            fichero.Write("\r\n");
            fichero.Write(theta);
            fichero.Write("\r\n");
            fichero.Close();

        }


        public void Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "txt";
            ofd.Filter = "Text|*.txt";
            ofd.Title = "Abrir archivos";
            ofd.ShowDialog();
            string nombre = ofd.FileName;
            StreamReader fichero = new StreamReader(nombre);

            rows = Convert.ToInt32(fichero.ReadLine());
            columns = Convert.ToInt32(fichero.ReadLine());
            Rho_in = Convert.ToDouble(fichero.ReadLine());
            P_in = Convert.ToDouble(fichero.ReadLine());
            T_in = Convert.ToDouble(fichero.ReadLine());
            M_in = Convert.ToDouble(fichero.ReadLine());
            R_air = Convert.ToDouble(fichero.ReadLine());
            Gamma = Convert.ToDouble(fichero.ReadLine());
            E = Convert.ToDouble(fichero.ReadLine());
            theta = Convert.ToDouble(fichero.ReadLine());

            a_in = Math.Sqrt(Gamma * R_air * T_in);
            v_in = 0;
            u_in = M_in * a_in;

            matrix = new Cell[rows, columns];
        }

        public List<List<Point>> calculateGraph()
        {

            List<Point> listPoints_body_u = new List<Point>();
            List<Point> listPoints_bnd_u = new List<Point>();
            List<Point> listPoints_body_v = new List<Point>();
            List<Point> listPoints_bnd_v = new List<Point>();
            List<Point> listPoints_body_rho = new List<Point>();
            List<Point> listPoints_bnd_rho = new List<Point>();
            List<Point> listPoints_body_P = new List<Point>();
            List<Point> listPoints_bnd_P = new List<Point>();
            List<Point> listPoints_body_T = new List<Point>();
            List<Point> listPoints_bnd_T = new List<Point>();
            List<Point> listPoints_body_M = new List<Point>();
            List<Point> listPoints_bnd_M = new List<Point>();

            List<List<Point>> listPoints = new List<List<Point>>();


            for (int j = 0; j < columns; j++)
            {
                double u_tot = 0;
                double v_tot = 0;
                double rho_tot = 0;
                double P_tot = 0;
                double T_tot = 0;
                double M_tot = 0;

                for (int i = 1; i < rows; i++) //calculate avg value in body (except row == 0)
                {
                    u_tot += matrix[i, j].u;
                    v_tot += matrix[i, j].v;
                    rho_tot += matrix[i, j].Rho;
                    P_tot += matrix[i, j].P;
                    T_tot += matrix[i, j].T;
                    M_tot += matrix[i, j].M;
                }

                double u_avg = u_tot / (rows - 1);
                double v_avg = v_tot / (rows - 1);
                double rho_avg = rho_tot / (rows - 1);
                double P_avg = P_tot / (rows - 1);
                double T_avg = T_tot / (rows - 1);
                double M_avg = M_tot / (rows - 1);

                listPoints_body_u.Add(new Point(matrix[1,j].x, u_avg));
                listPoints_body_v.Add(new Point(matrix[1, j].x, v_avg));
                listPoints_body_rho.Add(new Point(matrix[1, j].x, rho_avg));
                listPoints_body_P.Add(new Point(matrix[1, j].x, P_avg));
                listPoints_body_T.Add(new Point(matrix[1, j].x, T_avg));
                listPoints_body_M.Add(new Point(matrix[1, j].x, M_avg));

                listPoints_bnd_u.Add(new Point(matrix[1, j].x, matrix[0, j].u));
                listPoints_bnd_v.Add(new Point(matrix[1, j].x, matrix[0, j].v));
                listPoints_bnd_rho.Add(new Point(matrix[1, j].x, matrix[0, j].Rho));
                listPoints_bnd_P.Add(new Point(matrix[1, j].x, matrix[0, j].P));
                listPoints_bnd_T.Add(new Point(matrix[1, j].x, matrix[0, j].T));
                listPoints_bnd_M.Add(new Point(matrix[1, j].x, matrix[0, j].M));
            }

            listPoints.Add(listPoints_body_u);
            listPoints.Add(listPoints_body_v);
            listPoints.Add(listPoints_body_rho);
            listPoints.Add(listPoints_body_P);
            listPoints.Add(listPoints_body_T);
            listPoints.Add(listPoints_body_M);

            listPoints.Add(listPoints_bnd_u);
            listPoints.Add(listPoints_bnd_v);
            listPoints.Add(listPoints_bnd_rho);
            listPoints.Add(listPoints_bnd_P);
            listPoints.Add(listPoints_bnd_T);
            listPoints.Add(listPoints_bnd_M);


            return listPoints;

        }






        /**
           public List<Polygon> getListPolygon_u()
        {
            List<Polygon> listPolygons_u = new List<Polygon>();
            for (int j = 0; j < columns - 1; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    listPolygons_u.Add(matrix[i, j].polygon_u);
                }
            }
            return listPolygons_u;
        }
        public List<Polygon> getListPolygon_v()
        {
            List<Polygon> listPolygons_v = new List<Polygon>();
            for (int j = 0; j < columns - 1; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    listPolygons_v.Add(matrix[i, j].polygon_v);
                }
            }
            return listPolygons_v;
        }
        public List<Polygon> getListPolygon_rho()
        {
            List<Polygon> listPolygons_rho = new List<Polygon>();
            for (int j = 0; j < columns - 1; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    listPolygons_rho.Add(matrix[i, j].polygon_rho);
                }
            }
            return listPolygons_rho;
        }
        public List<Polygon> getListPolygon_P()
        {
            List<Polygon> listPolygons_P = new List<Polygon>();
            for (int j = 0; j < columns - 1; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    listPolygons_P.Add(matrix[i, j].polygon_P);
                }
            }
            return listPolygons_P;
        }
        public List<Polygon> getListPolygon_T()
        {
            List<Polygon> listPolygons_T = new List<Polygon>();
            for (int j = 0; j < columns - 1; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    listPolygons_T.Add(matrix[i, j].polygon_T);
                }
            }
            return listPolygons_T;
        }
        public List<Polygon> getListPolygon_M()
        {
            List<Polygon> listPolygons_M = new List<Polygon>();
            for (int j = 0; j < columns - 1; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    listPolygons_M.Add(matrix[i, j].polygon_M);
                }
            }
            return listPolygons_M;
        }*/

        //double AX = 0;

        //public void actualizarRectanglesMatrix()
        //{
            

        //    for (int i = 0; AX < 10; i++)
        //    {
        //        for (int j = 0; j < 40; j++)
        //        {
        //            matrix[i, j] = new Cell();
        //            matrix[i, j].calcularRectangle(40, 2);
        //        }
        //        AX += matrix[i, 1].rectangle.Width;
        //    }

        //}
    }
}
