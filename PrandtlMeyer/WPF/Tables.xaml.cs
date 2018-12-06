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


namespace WPF
{
    /// <summary>
    /// Interaction logic for Tables.xaml
    /// </summary>
    public partial class Tables : Window
    {
        public Tables()
        {
            InitializeComponent();
        }

        private void button_introduction_Click(object sender, RoutedEventArgs e)
        {
            MainWindow intro = new MainWindow();
            intro.Show();
            this.Hide();
        }

        private void button_simulation_Click(object sender, RoutedEventArgs e)
        {
            Simulation simulation = new Simulation();
            simulation.Show();
            this.Hide();
        }

        private void button_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
