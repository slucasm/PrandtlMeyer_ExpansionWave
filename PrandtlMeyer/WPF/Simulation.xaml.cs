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
       
        List<Polygon> listPolygons_u = new List<Polygon>();
        List<Polygon> listPolygons_v = new List<Polygon>();
        List<Polygon> listPolygons_rho = new List<Polygon>();
        List<Polygon> listPolygons_P = new List<Polygon>();
        List<Polygon> listPolygons_T = new List<Polygon>();
        List<Polygon> listPolygons_M = new List<Polygon>();

        List<DataTable> listTables = new List<DataTable>();

        /**DataTable table_u = new DataTable();
        DataTable table_v = new DataTable();
        DataTable table_rho = new DataTable();
        DataTable table_P = new DataTable();
        DataTable table_T = new DataTable();
        DataTable table_M = new DataTable();**/

        List<Cell> listCells = new List<Cell>();

        Matriz matrix = new Matriz();



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboBox_selectgrid.Items.Add("U grid");
            comboBox_selectgrid.Items.Add("V grid");
            comboBox_selectgrid.Items.Add("Rho grid");
            comboBox_selectgrid.Items.Add("P grid");
            comboBox_selectgrid.Items.Add("T grid");
            comboBox_selectgrid.Items.Add("M grid");
            comboBox_selectgrid.SelectedIndex = 0;

            label_information.Content = ("u:        v:      rho:        P:      T:      M:");

            


            
            
            
            

            //Polygon p = new Polygon();
            //p.Points = new PointCollection() { new Point(0, 0), new Point(100, 0), new Point(100, 100), new Point(0, 100) };
            //p.Fill = Brushes.Red;
            //grid1.Children.Add(p);
        }

        private void button_introduction_Click(object sender, RoutedEventArgs e)
        {
            MainWindow intro = new MainWindow();
            intro.Show();
            this.Hide();
        }

        private void button_tableresults_Click(object sender, RoutedEventArgs e)
        {
                Tables tables = new Tables(matrix, this,listTables);
                tables.Show();
                this.Hide();
            
        }

        private void button_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void comboBox_selectgrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_selectgrid.SelectedIndex == 0)
            {
                grid_u.Visibility = Visibility.Visible;
                grid_v.Visibility = Visibility.Hidden;
                grid_rho.Visibility = Visibility.Hidden;
                grid_P.Visibility = Visibility.Hidden;
                grid_T.Visibility = Visibility.Hidden;
                grid_M.Visibility = Visibility.Hidden;

            }
            else if (comboBox_selectgrid.SelectedIndex == 1)
            {
                grid_u.Visibility = Visibility.Hidden;
                grid_v.Visibility = Visibility.Visible;
                grid_rho.Visibility = Visibility.Hidden;
                grid_P.Visibility = Visibility.Hidden;
                grid_T.Visibility = Visibility.Hidden;
                grid_M.Visibility = Visibility.Hidden;
            }
            else if (comboBox_selectgrid.SelectedIndex == 2)
            {
                grid_u.Visibility = Visibility.Hidden;
                grid_v.Visibility = Visibility.Hidden;
                grid_rho.Visibility = Visibility.Visible;
                grid_P.Visibility = Visibility.Hidden;
                grid_T.Visibility = Visibility.Hidden;
                grid_M.Visibility = Visibility.Hidden;
            }
            else if (comboBox_selectgrid.SelectedIndex == 3)
            {
                grid_u.Visibility = Visibility.Hidden;
                grid_v.Visibility = Visibility.Hidden;
                grid_rho.Visibility = Visibility.Hidden;
                grid_P.Visibility = Visibility.Visible;
                grid_T.Visibility = Visibility.Hidden;
                grid_M.Visibility = Visibility.Hidden;
            }
            else if (comboBox_selectgrid.SelectedIndex == 4)
            {
                grid_u.Visibility = Visibility.Hidden;
                grid_v.Visibility = Visibility.Hidden;
                grid_rho.Visibility = Visibility.Hidden;
                grid_P.Visibility = Visibility.Hidden;
                grid_T.Visibility = Visibility.Visible;
                grid_M.Visibility = Visibility.Hidden;
            }
            else if (comboBox_selectgrid.SelectedIndex == 5)
            {
                grid_u.Visibility = Visibility.Hidden;
                grid_v.Visibility = Visibility.Hidden;
                grid_rho.Visibility = Visibility.Hidden;
                grid_P.Visibility = Visibility.Hidden;
                grid_T.Visibility = Visibility.Hidden;
                grid_M.Visibility = Visibility.Visible;
            }
        }

        private void grid_u_MouseEnter(object sender, MouseEventArgs e)
        {
            //for (int i = 0; i < listPolygons_u.Count; i++)
            //{
            //    listPolygons_u[i].MouseEnter += Polygon_MouseEnter;
            //}
        }

        private void Polygon_MouseEnter(object sender, MouseEventArgs e)
        {
            Polygon obj = sender as Polygon;
            int i = Convert.ToInt32(obj.DataContext.ToString());
            label_information.Content = String.Format("u: " + Math.Round(listCells[i].u,3) +"   v: " + Math.Round(listCells[i].v,3) +  "    rho: " + Math.Round(listCells[i].Rho,3) +  "    P: " + Math.Round(listCells[i].P,3) +  "    T: " + Math.Round(listCells[i].T,3) +  "    M: " + Math.Round(listCells[i].M,3));
        }
        private void Polygon_MouseLeave(object sender, MouseEventArgs e)
        {
            label_information.Content = String.Format("u:        v:      rho:        P:      T:      M:");
        }

        private void button_default_Click(object sender, RoutedEventArgs e)
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
            textBox_columns.Text = "89";
        }

        private void button_calculate_Click(object sender, RoutedEventArgs e)
        {
            if (textBox_columns.Text != "" && textBox_rows.Text != "" && textBox_rho.Text != "" && textBox_P.Text != "" && textBox_T.Text != "" && textBox_M.Text != "" && textBox_R.Text != "" && textBox_E.Text != "" && textBox_gamma.Text != "" && textBox_theta.Text != "")
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
            }

            else
            {
                MessageBox.Show("First select all initial parameters");
            }
        }

        private void button_restart_Click(object sender, RoutedEventArgs e)
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

        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            matrix.Save();
        }

        private void button_open_Click(object sender, RoutedEventArgs e)
        {
            matrix.Open();

            matrix.Initialize();
            matrix.calculate();
            matrix.calculatePoligons();
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
        }
        
    }
}
