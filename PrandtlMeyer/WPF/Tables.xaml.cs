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
    /// Interaction logic for Tables.xaml
    /// </summary>
    public partial class Tables : Window
    {
        public Tables(Matriz matrix)
        {
            InitializeComponent();
            this.matrix = matrix;
        }

        Matriz matrix;

        List<DataTable> listTables = new List<DataTable>();

        private void button_introduction_Click(object sender, RoutedEventArgs e)
        {
            MainWindow intro = new MainWindow();
            intro.Show();
            this.Hide();
        }

        private void button_simulation_Click(object sender, RoutedEventArgs e)
        {
            Simulation simulation = new Simulation(matrix);
            simulation.Show();
            this.Hide();
        }

        private void button_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboBox_selectgrid.Items.Add("U grid");
            comboBox_selectgrid.Items.Add("V grid");
            comboBox_selectgrid.Items.Add("Rho grid");
            comboBox_selectgrid.Items.Add("P grid");
            comboBox_selectgrid.Items.Add("T grid");
            comboBox_selectgrid.Items.Add("M grid");

            comboBox_selectgrid.SelectedIndex = 0;

            listTables = matrix.createTables();

            grid_u.ItemsSource = listTables[0].DefaultView;
            grid_v.ItemsSource = listTables[1].DefaultView;
            grid_rho.ItemsSource = listTables[2].DefaultView;
            grid_P.ItemsSource = listTables[3].DefaultView;
            grid_T.ItemsSource = listTables[4].DefaultView;
            grid_M.ItemsSource = listTables[5].DefaultView;

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
    }
}
