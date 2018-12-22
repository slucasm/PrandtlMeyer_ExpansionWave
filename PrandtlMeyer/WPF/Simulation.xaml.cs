using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LibreriaClases;
using System.Data;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;

namespace WPF
{
    /// <summary>
    /// Interaction logic for Simulation.xaml
    /// </summary>
    public partial class Simulation : Window
    {
        public Simulation(Matriz matrix)
        {
            InitializeComponent();
            this.matrix = matrix;
        }

        Boolean isCalculatePressed = false;

        List<Polygon> listPolygons_u = new List<Polygon>();
        List<Polygon> listPolygons_v = new List<Polygon>();
        List<Polygon> listPolygons_rho = new List<Polygon>();
        List<Polygon> listPolygons_P = new List<Polygon>();
        List<Polygon> listPolygons_T = new List<Polygon>();
        List<Polygon> listPolygons_M = new List<Polygon>();

        List<DataTable> listTables = new List<DataTable>();

        List<Cell> listCells = new List<Cell>();

        public Matriz matrix = new Matriz();

        ChartPlotter plot_u = new ChartPlotter();
        ChartPlotter plot_v = new ChartPlotter();
        ChartPlotter plot_rho = new ChartPlotter();
        ChartPlotter plot_P = new ChartPlotter();
        ChartPlotter plot_T = new ChartPlotter();
        ChartPlotter plot_M = new ChartPlotter();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Load window
            try
            {

                comboBox_selectgrid.Items.Add("U grid");
                comboBox_selectgrid.Items.Add("V grid");
                comboBox_selectgrid.Items.Add("Rho grid");
                comboBox_selectgrid.Items.Add("P grid");
                comboBox_selectgrid.Items.Add("T grid");
                comboBox_selectgrid.Items.Add("M grid");
                comboBox_selectgrid.SelectedIndex = 0;

                label_information.Content = ("u:        v:      rho:        P:      T:      M:      x:      y:");

                rectangle_gradient.Fill = new LinearGradientBrush(Color.FromRgb(255, 255, 255), Color.FromRgb(0, 230, 0), new Point(0, 0), new Point(1, 0));
                rectangle_gradient.Visibility = Visibility.Hidden;
                label_min.Visibility = Visibility.Hidden;
                label_max.Visibility = Visibility.Hidden;

                label_plot_title.Visibility = Visibility.Hidden;
                label_plot_xaxis.Visibility = Visibility.Hidden;
                label_plot_yaxis.Visibility = Visibility.Hidden;

            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }
            

        }

        private void button_introduction_Click(object sender, RoutedEventArgs e)
        {
            //open introduction form
            try
            {
                MainWindow intro = new MainWindow();
                intro.Show();
                this.Hide();
            }
            catch (Exception b)
            {
                MessageBox.Show(b.Message);
            }
        }

        private void button_tableresults_Click(object sender, RoutedEventArgs e)
        {
            //open tableresults form
            try
            {
                if (isCalculatePressed == true)
                {
                    Tables tables = new Tables(matrix, this, listTables);
                    MessageBox.Show("Loading Tables, wait a few moments");
                    tables.Show();
                    this.Hide();
                }

                else
                {
                    MessageBox.Show("First you have to calculate your situation");
                }
            }
            catch (Exception c)
            {
                MessageBox.Show(c.Message);
            }
            
        }

        private void button_close_Click(object sender, RoutedEventArgs e)
        {
            //close simulator
            try
            {
                this.Close();
            }
            catch (Exception d)
            {
                MessageBox.Show(d.Message);
            }
        }

        private void comboBox_selectgrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //shows the correct grid when the combobox is changed
            try 
            {
                if (comboBox_selectgrid.SelectedIndex == 0)
                {
                    grid_u.Visibility = Visibility.Visible;
                    grid_v.Visibility = Visibility.Hidden;
                    grid_rho.Visibility = Visibility.Hidden;
                    grid_P.Visibility = Visibility.Hidden;
                    grid_T.Visibility = Visibility.Hidden;
                    grid_M.Visibility = Visibility.Hidden;
                    rectangle_gradient.Fill = new LinearGradientBrush(Color.FromRgb(255, 255, 255), Color.FromRgb(0, 230, 0), new Point(0, 0), new Point(1, 0));

                    grid_plot_u.Visibility = Visibility.Visible;
                    grid_plot_v.Visibility = Visibility.Hidden;
                    grid_plot_rho.Visibility = Visibility.Hidden;
                    grid_plot_P.Visibility = Visibility.Hidden;
                    grid_plot_T.Visibility = Visibility.Hidden;
                    grid_plot_M.Visibility = Visibility.Hidden;

                    label_plot_yaxis.Content = "U (m/s)";
                }
                else if (comboBox_selectgrid.SelectedIndex == 1)
                {
                    grid_u.Visibility = Visibility.Hidden;
                    grid_v.Visibility = Visibility.Visible;
                    grid_rho.Visibility = Visibility.Hidden;
                    grid_P.Visibility = Visibility.Hidden;
                    grid_T.Visibility = Visibility.Hidden;
                    grid_M.Visibility = Visibility.Hidden;
                    rectangle_gradient.Fill = new LinearGradientBrush(Color.FromRgb(255, 255, 255), Color.FromRgb(0, 92, 230), new Point(0, 0), new Point(1, 0));

                    grid_plot_u.Visibility = Visibility.Hidden;
                    grid_plot_v.Visibility = Visibility.Visible;
                    grid_plot_rho.Visibility = Visibility.Hidden;
                    grid_plot_P.Visibility = Visibility.Hidden;
                    grid_plot_T.Visibility = Visibility.Hidden;
                    grid_plot_M.Visibility = Visibility.Hidden;

                    label_plot_yaxis.Content = "V (m/s)";
                }
                else if (comboBox_selectgrid.SelectedIndex == 2)
                {
                    grid_u.Visibility = Visibility.Hidden;
                    grid_v.Visibility = Visibility.Hidden;
                    grid_rho.Visibility = Visibility.Visible;
                    grid_P.Visibility = Visibility.Hidden;
                    grid_T.Visibility = Visibility.Hidden;
                    grid_M.Visibility = Visibility.Hidden;
                    rectangle_gradient.Fill = new LinearGradientBrush(Color.FromRgb(255, 255, 255), Color.FromRgb(230, 184, 0), new Point(0, 0), new Point(1, 0));

                    grid_plot_u.Visibility = Visibility.Hidden;
                    grid_plot_v.Visibility = Visibility.Hidden;
                    grid_plot_rho.Visibility = Visibility.Visible;
                    grid_plot_P.Visibility = Visibility.Hidden;
                    grid_plot_T.Visibility = Visibility.Hidden;
                    grid_plot_M.Visibility = Visibility.Hidden;

                    label_plot_yaxis.Content = "ρ (kg/m³)";
                }
                else if (comboBox_selectgrid.SelectedIndex == 3)
                {
                    grid_u.Visibility = Visibility.Hidden;
                    grid_v.Visibility = Visibility.Hidden;
                    grid_rho.Visibility = Visibility.Hidden;
                    grid_P.Visibility = Visibility.Visible;
                    grid_T.Visibility = Visibility.Hidden;
                    grid_M.Visibility = Visibility.Hidden;
                    rectangle_gradient.Fill = new LinearGradientBrush(Color.FromRgb(255, 255, 255), Color.FromRgb(230, 0, 92), new Point(0, 0), new Point(1, 0));

                    grid_plot_u.Visibility = Visibility.Hidden;
                    grid_plot_v.Visibility = Visibility.Hidden;
                    grid_plot_rho.Visibility = Visibility.Hidden;
                    grid_plot_P.Visibility = Visibility.Visible;
                    grid_plot_T.Visibility = Visibility.Hidden;
                    grid_plot_M.Visibility = Visibility.Hidden;

                    label_plot_yaxis.Content = "P (N/m²)";

                }
                else if (comboBox_selectgrid.SelectedIndex == 4)
                {
                    grid_u.Visibility = Visibility.Hidden;
                    grid_v.Visibility = Visibility.Hidden;
                    grid_rho.Visibility = Visibility.Hidden;
                    grid_P.Visibility = Visibility.Hidden;
                    grid_T.Visibility = Visibility.Visible;
                    grid_M.Visibility = Visibility.Hidden;
                    rectangle_gradient.Fill = new LinearGradientBrush(Color.FromRgb(255, 255, 255), Color.FromRgb(230, 0, 0), new Point(0, 0), new Point(1, 0));

                    grid_plot_u.Visibility = Visibility.Hidden;
                    grid_plot_v.Visibility = Visibility.Hidden;
                    grid_plot_rho.Visibility = Visibility.Hidden;
                    grid_plot_P.Visibility = Visibility.Hidden;
                    grid_plot_T.Visibility = Visibility.Visible;
                    grid_plot_M.Visibility = Visibility.Hidden;
                    label_plot_yaxis.Content = "T (K)";
                }
                else if (comboBox_selectgrid.SelectedIndex == 5)
                {
                    grid_u.Visibility = Visibility.Hidden;
                    grid_v.Visibility = Visibility.Hidden;
                    grid_rho.Visibility = Visibility.Hidden;
                    grid_P.Visibility = Visibility.Hidden;
                    grid_T.Visibility = Visibility.Hidden;
                    grid_M.Visibility = Visibility.Visible;
                    rectangle_gradient.Fill = new LinearGradientBrush(Color.FromRgb(255, 255, 255), Color.FromRgb(172, 115, 57), new Point(0, 0), new Point(1, 0));

                    grid_plot_u.Visibility = Visibility.Hidden;
                    grid_plot_v.Visibility = Visibility.Hidden;
                    grid_plot_rho.Visibility = Visibility.Hidden;
                    grid_plot_P.Visibility = Visibility.Hidden;
                    grid_plot_T.Visibility = Visibility.Hidden;
                    grid_plot_M.Visibility = Visibility.Visible;
                    label_plot_yaxis.Content = "Mach";
                }
            }
            catch (Exception y)
            {
                MessageBox.Show(y.Message);
            }
        }

        private void Polygon_MouseEnter(object sender, MouseEventArgs e)
        {
            //shows the values of variables in the labels when mouse enter to grid
            try
            {
                Polygon obj = sender as Polygon;
                int i = Convert.ToInt32(obj.DataContext.ToString());
                label_information.Content = String.Format("u: " + Math.Round(listCells[i].u, 3) + "   v: " + Math.Round(listCells[i].v, 3) + "    rho: " + Math.Round(listCells[i].Rho, 3) + "    P: " + Math.Round(listCells[i].P, 3) + "    T: " + Math.Round(listCells[i].T, 3) + "    M: " + Math.Round(listCells[i].M, 3) + "  x: " + Math.Round(listCells[i].x, 3) + "    y: " + Math.Round(listCells[i].y, 3));
            }
            catch (Exception f)
            {
                MessageBox.Show(f.Message);
            }
        }

        private void Polygon_MouseLeave(object sender, MouseEventArgs e)
        {
            //restart the label with values of variables when mouse leave the grid
            try
            {
                label_information.Content = String.Format("u:        v:      rho:        P:      T:      M:     x:      y:");
            }
            catch (Exception g)
            {
                MessageBox.Show(g.Message);
            }
        }

        private void button_default_Click(object sender, RoutedEventArgs e)
        { 
            //introduce default parameters for simulator (parameters of Anderson)
            try
            {
                textBox_rho.Text = "1,23";
                textBox_P.Text = "101000";
                textBox_T.Text = "286,1";
                textBox_M.Text = "2";
                textBox_R.Text = "287";
                textBox_gamma.Text = "1,4";
                textBox_E.Text = "10";
                textBox_theta.Text = "5,352";
                textBox_rows.Text = "41";
                textBox_columns.Text = "120";
            }
            catch (Exception h)
            {
                MessageBox.Show(h.Message);
            }
        }

        private void button_calculate_Click(object sender, RoutedEventArgs e)
        {
            // Calculate the simulation! 
            try
            {
                if (isCalculatePressed == true)
                {
                    MessageBox.Show("To calculate something, first restart the situation");
                }
                else if (textBox_columns.Text != "" && textBox_rows.Text != "" && textBox_rho.Text != "" && textBox_P.Text != "" && textBox_T.Text != "" && textBox_M.Text != "" && textBox_R.Text != "" && textBox_E.Text != "" && textBox_gamma.Text != "" && textBox_theta.Text != "" && isCalculatePressed == false)
                {


                    int rows = Convert.ToInt32(textBox_rows.Text);
                    int columns = Convert.ToInt32(textBox_columns.Text);
                    double rho = Convert.ToDouble(textBox_rho.Text);
                    double P = Convert.ToDouble(textBox_P.Text);
                    double T = Convert.ToDouble(textBox_T.Text);
                    double M = Convert.ToDouble(textBox_M.Text);
                    double R = Convert.ToDouble(textBox_R.Text);
                    double Gamma = Convert.ToDouble(textBox_gamma.Text);
                    double E = Convert.ToDouble(textBox_E.Text);
                    double theta = Convert.ToDouble(textBox_theta.Text);

                    matrix.setParameters(rows, columns, rho, P, T, M, R, Gamma, E, theta);

                    matrix.Initialize();
                    matrix.calculate();
                    matrix.calculatePoligons();
                    matrix.colorPolygons();
                    List<List<Polygon>> listPolygons = matrix.getListPolygons();
                    listPolygons_u = listPolygons[0];
                    listPolygons_v = listPolygons[1];
                    listPolygons_rho = listPolygons[2];
                    listPolygons_P = listPolygons[3];
                    listPolygons_T = listPolygons[4];
                    listPolygons_M = listPolygons[5];

                    listCells = matrix.getListCells();

                    for (int i = 0; i < listPolygons_u.Count; i++)
                    {
                        listPolygons_u[i].MouseEnter += Polygon_MouseEnter;
                        listPolygons_u[i].MouseLeave += Polygon_MouseLeave;

                        grid_u.Children.Add(listPolygons_u[i]);

                        listPolygons_v[i].MouseEnter += Polygon_MouseEnter;
                        listPolygons_v[i].MouseLeave += Polygon_MouseLeave;

                        grid_v.Children.Add(listPolygons_v[i]);

                        listPolygons_rho[i].MouseEnter += Polygon_MouseEnter;
                        listPolygons_rho[i].MouseLeave += Polygon_MouseLeave;

                        grid_rho.Children.Add(listPolygons_rho[i]);

                        listPolygons_P[i].MouseEnter += Polygon_MouseEnter;
                        listPolygons_P[i].MouseLeave += Polygon_MouseLeave;

                        grid_P.Children.Add(listPolygons_P[i]);

                        listPolygons_T[i].MouseEnter += Polygon_MouseEnter;
                        listPolygons_T[i].MouseLeave += Polygon_MouseLeave;

                        grid_T.Children.Add(listPolygons_T[i]);

                        listPolygons_M[i].MouseEnter += Polygon_MouseEnter;
                        listPolygons_M[i].MouseLeave += Polygon_MouseLeave;

                        grid_M.Children.Add(listPolygons_M[i]);


                    }

                    listTables = matrix.createTables();

                    rectangle_gradient.Visibility = Visibility.Visible;

                    label_min.Visibility = Visibility.Visible;
                    label_max.Visibility = Visibility.Visible;


                    List<List<Point>> listPoints = matrix.calculateGraph();
                    RawDataSource u_body = new RawDataSource(listPoints[0]);
                    RawDataSource v_body = new RawDataSource(listPoints[1]);
                    RawDataSource rho_body = new RawDataSource(listPoints[2]);
                    RawDataSource P_body = new RawDataSource(listPoints[3]);
                    RawDataSource T_body = new RawDataSource(listPoints[4]);
                    RawDataSource M_body = new RawDataSource(listPoints[5]);

                    RawDataSource u_bnd = new RawDataSource(listPoints[6]);
                    RawDataSource v_bnd = new RawDataSource(listPoints[7]);
                    RawDataSource rho_bnd = new RawDataSource(listPoints[8]);
                    RawDataSource P_bnd = new RawDataSource(listPoints[9]);
                    RawDataSource T_bnd = new RawDataSource(listPoints[10]);
                    RawDataSource M_bnd = new RawDataSource(listPoints[11]);

                    plot_u.AddLineGraph(u_body, Colors.Blue, 2, "Body");
                    plot_u.AddLineGraph(u_bnd, Colors.Red, 2, "Boundary");

                    plot_v.AddLineGraph(v_body, Colors.Blue, 2, "Body");
                    plot_v.AddLineGraph(v_bnd, Colors.Red, 2, "Boundary");

                    plot_rho.AddLineGraph(rho_body, Colors.Blue, 2, "Body");
                    plot_rho.AddLineGraph(rho_bnd, Colors.Red, 2, "Boundary");

                    plot_P.AddLineGraph(P_body, Colors.Blue, 2, "Body");
                    plot_P.AddLineGraph(P_bnd, Colors.Red, 2, "Boundary");

                    plot_T.AddLineGraph(T_body, Colors.Blue, 2, "Body");
                    plot_T.AddLineGraph(T_bnd, Colors.Red, 2, "Boundary");

                    plot_M.AddLineGraph(M_body, Colors.Blue, 2, "Body");
                    plot_M.AddLineGraph(M_bnd, Colors.Red, 2, "Boundary");

                    grid_plot_u.Children.Add(plot_u);
                    grid_plot_v.Children.Add(plot_v);
                    grid_plot_rho.Children.Add(plot_rho);
                    grid_plot_P.Children.Add(plot_P);
                    grid_plot_T.Children.Add(plot_T);
                    grid_plot_M.Children.Add(plot_M);

                    plot_u.Background = Brushes.Transparent;
                    plot_v.Background = Brushes.Transparent;
                    plot_rho.Background = Brushes.Transparent;
                    plot_P.Background = Brushes.Transparent;
                    plot_T.Background = Brushes.Transparent;
                    plot_M.Background = Brushes.Transparent;

                    plot_u.Foreground = Brushes.White;
                    plot_v.Foreground = Brushes.White;
                    plot_rho.Foreground = Brushes.White;
                    plot_P.Foreground = Brushes.White;
                    plot_T.Foreground = Brushes.White;
                    plot_M.Foreground = Brushes.White;

                    plot_u.Legend.Foreground = Brushes.Black;
                    plot_v.Legend.Foreground = Brushes.Black;
                    plot_rho.Legend.Foreground = Brushes.Black;
                    plot_P.Legend.Foreground = Brushes.Black;
                    plot_T.Legend.Foreground = Brushes.Black;

                    label_plot_yaxis.Content = "U (m/s)";
                    grid_plot_u.Visibility = Visibility.Visible;

                    label_plot_title.Visibility = Visibility.Visible;
                    label_plot_xaxis.Visibility = Visibility.Visible;
                    label_plot_yaxis.Visibility = Visibility.Visible;

                    isCalculatePressed = true;
                }
                else
                {
                    MessageBox.Show("First select all initial parameters");
                }
            }
            catch (Exception j)
            {
                MessageBox.Show(j.Message);
            }
        }

        private void button_restart_Click(object sender, RoutedEventArgs e)
        {
            //restart all the simulation
            try
            {
                this.matrix = new Matriz();
                listPolygons_M.Clear();
                listPolygons_P.Clear();
                listPolygons_rho.Clear();
                listPolygons_T.Clear();
                listPolygons_u.Clear();
                listPolygons_v.Clear();

                grid_M.Children.Clear();
                grid_P.Children.Clear();
                grid_rho.Children.Clear();
                grid_T.Children.Clear();
                grid_u.Children.Clear();
                grid_v.Children.Clear();

                listTables.Clear();

                textBox_columns.Text = "";
                textBox_rows.Text = "";
                textBox_P.Text = "";
                textBox_T.Text = "";
                textBox_M.Text = "";
                textBox_gamma.Text = "";
                textBox_R.Text = "";
                textBox_rho.Text = "";
                textBox_E.Text = "";
                textBox_theta.Text = "";

                rectangle_gradient.Visibility = Visibility.Hidden;
                label_min.Visibility = Visibility.Hidden;
                label_max.Visibility = Visibility.Hidden;
                isCalculatePressed = false;

                grid_plot_u.Visibility = Visibility.Hidden;
                grid_plot_v.Visibility = Visibility.Hidden;
                grid_plot_rho.Visibility = Visibility.Hidden;
                grid_plot_P.Visibility = Visibility.Hidden;
                grid_plot_T.Visibility = Visibility.Hidden;
                grid_plot_M.Visibility = Visibility.Hidden;

                plot_u = new ChartPlotter();
                plot_v = new ChartPlotter();
                plot_rho = new ChartPlotter();
                plot_P = new ChartPlotter();
                plot_T = new ChartPlotter();
                plot_M = new ChartPlotter();

                grid_plot_u.Children.Clear();
                grid_plot_v.Children.Clear();
                grid_plot_rho.Children.Clear();
                grid_plot_P.Children.Clear();
                grid_plot_T.Children.Clear();
                grid_plot_M.Children.Clear();

                label_min.Visibility = Visibility.Hidden;
                label_max.Visibility = Visibility.Hidden;
                rectangle_gradient.Visibility = Visibility.Hidden;

                label_plot_title.Visibility = Visibility.Hidden;
                label_plot_xaxis.Visibility = Visibility.Hidden;
                label_plot_yaxis.Visibility = Visibility.Hidden;

            }
            catch (Exception k)
            {
                MessageBox.Show(k.Message);
            }
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            //save the parameters in a file (.txt)
            try
            {
                if (isCalculatePressed == true)
                {
                    matrix.Save();
                }
                else
                {
                    MessageBox.Show("First you have to calculate your situation");
                }
            }
            catch (Exception l)
            {
                MessageBox.Show(l.Message);
            }
        }

        
        private void button_open_Click(object sender, RoutedEventArgs e)
        {
            //open .txt with the initial parameters and calculate the simulation
            try
            {
                matrix.Open();

                matrix.Initialize();
                matrix.calculate();
                matrix.calculatePoligons();
                matrix.colorPolygons();
                List<List<Polygon>> listPolygons = matrix.getListPolygons();
                listPolygons_u = listPolygons[0];
                listPolygons_v = listPolygons[1];
                listPolygons_rho = listPolygons[2];
                listPolygons_P = listPolygons[3];
                listPolygons_T = listPolygons[4];
                listPolygons_M = listPolygons[5];

                textBox_columns.Text = matrix.columns.ToString();
                textBox_E.Text = matrix.E.ToString();
                textBox_theta.Text = matrix.theta.ToString();
                textBox_M.Text = matrix.matrix[0, 0].M.ToString();
                textBox_P.Text = matrix.matrix[0, 0].P.ToString();
                textBox_rho.Text = matrix.matrix[0, 0].Rho.ToString();
                textBox_T.Text = matrix.matrix[0, 0].T.ToString();
                textBox_R.Text = matrix.R_air.ToString();
                textBox_gamma.Text = matrix.Gamma.ToString();
                textBox_rows.Text = matrix.rows.ToString();



                listCells = matrix.getListCells();

                for (int i = 0; i < listPolygons_u.Count; i++)
                {
                    listPolygons_u[i].MouseEnter += Polygon_MouseEnter;
                    listPolygons_u[i].MouseLeave += Polygon_MouseLeave;

                    grid_u.Children.Add(listPolygons_u[i]);

                    listPolygons_v[i].MouseEnter += Polygon_MouseEnter;
                    listPolygons_v[i].MouseLeave += Polygon_MouseLeave;

                    grid_v.Children.Add(listPolygons_v[i]);

                    listPolygons_rho[i].MouseEnter += Polygon_MouseEnter;
                    listPolygons_rho[i].MouseLeave += Polygon_MouseLeave;

                    grid_rho.Children.Add(listPolygons_rho[i]);

                    listPolygons_P[i].MouseEnter += Polygon_MouseEnter;
                    listPolygons_P[i].MouseLeave += Polygon_MouseLeave;

                    grid_P.Children.Add(listPolygons_P[i]);

                    listPolygons_T[i].MouseEnter += Polygon_MouseEnter;
                    listPolygons_T[i].MouseLeave += Polygon_MouseLeave;

                    grid_T.Children.Add(listPolygons_T[i]);

                    listPolygons_M[i].MouseEnter += Polygon_MouseEnter;
                    listPolygons_M[i].MouseLeave += Polygon_MouseLeave;

                    grid_M.Children.Add(listPolygons_M[i]);


                }

                listTables = matrix.createTables();

                rectangle_gradient.Visibility = Visibility.Visible;

                label_min.Visibility = Visibility.Visible;
                label_max.Visibility = Visibility.Visible;


                List<List<Point>> listPoints = matrix.calculateGraph();
                RawDataSource u_body = new RawDataSource(listPoints[0]);
                RawDataSource v_body = new RawDataSource(listPoints[1]);
                RawDataSource rho_body = new RawDataSource(listPoints[2]);
                RawDataSource P_body = new RawDataSource(listPoints[3]);
                RawDataSource T_body = new RawDataSource(listPoints[4]);
                RawDataSource M_body = new RawDataSource(listPoints[5]);

                RawDataSource u_bnd = new RawDataSource(listPoints[6]);
                RawDataSource v_bnd = new RawDataSource(listPoints[7]);
                RawDataSource rho_bnd = new RawDataSource(listPoints[8]);
                RawDataSource P_bnd = new RawDataSource(listPoints[9]);
                RawDataSource T_bnd = new RawDataSource(listPoints[10]);
                RawDataSource M_bnd = new RawDataSource(listPoints[11]);

                plot_u.AddLineGraph(u_body, Colors.Blue, 2, "Body");
                plot_u.AddLineGraph(u_bnd, Colors.Red, 2, "Boundary");

                plot_v.AddLineGraph(v_body, Colors.Blue, 2, "Body");
                plot_v.AddLineGraph(v_bnd, Colors.Red, 2, "Boundary");

                plot_rho.AddLineGraph(rho_body, Colors.Blue, 2, "Body");
                plot_rho.AddLineGraph(rho_bnd, Colors.Red, 2, "Boundary");

                plot_P.AddLineGraph(P_body, Colors.Blue, 2, "Body");
                plot_P.AddLineGraph(P_bnd, Colors.Red, 2, "Boundary");

                plot_T.AddLineGraph(T_body, Colors.Blue, 2, "Body");
                plot_T.AddLineGraph(T_bnd, Colors.Red, 2, "Boundary");

                plot_M.AddLineGraph(M_body, Colors.Blue, 2, "Body");
                plot_M.AddLineGraph(M_bnd, Colors.Red, 2, "Boundary");

                grid_plot_u.Children.Add(plot_u);
                grid_plot_v.Children.Add(plot_v);
                grid_plot_rho.Children.Add(plot_rho);
                grid_plot_P.Children.Add(plot_P);
                grid_plot_T.Children.Add(plot_T);
                grid_plot_M.Children.Add(plot_M);

                plot_u.Background = Brushes.Transparent;
                plot_v.Background = Brushes.Transparent;
                plot_rho.Background = Brushes.Transparent;
                plot_P.Background = Brushes.Transparent;
                plot_T.Background = Brushes.Transparent;
                plot_M.Background = Brushes.Transparent;

                plot_u.Foreground = Brushes.White;
                plot_v.Foreground = Brushes.White;
                plot_rho.Foreground = Brushes.White;
                plot_P.Foreground = Brushes.White;
                plot_T.Foreground = Brushes.White;
                plot_M.Foreground = Brushes.White;

                plot_u.Legend.Foreground = Brushes.Black;
                plot_v.Legend.Foreground = Brushes.Black;
                plot_rho.Legend.Foreground = Brushes.Black;
                plot_P.Legend.Foreground = Brushes.Black;
                plot_T.Legend.Foreground = Brushes.Black;

                label_plot_yaxis.Content = "U (m/s)";
                grid_plot_u.Visibility = Visibility.Visible;

                label_plot_title.Visibility = Visibility.Visible;
                label_plot_xaxis.Visibility = Visibility.Visible;
                label_plot_yaxis.Visibility = Visibility.Visible;

                isCalculatePressed = true;
            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message);
            }

        }
        
    }
}
