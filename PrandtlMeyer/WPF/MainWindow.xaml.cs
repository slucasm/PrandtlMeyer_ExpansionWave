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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibreriaClases;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Matriz matrix = new Matriz();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            label_textbody.Content = "The aim of this project is to study and obtain a numerical solution for a \n flow over a Prandtl Meyer expansion corner. At this problem, the flow is a \n two-dimensional, supersonic and inviscid flow moving over a surface. Giving \n the initial conditions, we will have to study how this flow evolves along \n all the surface, obtaining its properties at different points of the surface.";
            //Load text for main label explaining the project
        }

        private void button_simulation_Click(object sender, RoutedEventArgs e)
        {
            //open simulator form
            Simulation simulation = new Simulation(matrix);
            simulation.Show();
            this.Hide();
        }

        private void button_tableresults_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("First you have to create the problem");
        }

        private void button_close_Click(object sender, RoutedEventArgs e)
        {
            //Close program
            this.Close();
        }

        private void button_video_Click(object sender, RoutedEventArgs e)
        {
            //open videotutorial form
            Video videoForm = new Video();
            videoForm.Show();
        }

        private void button_credits_Click(object sender, RoutedEventArgs e)
        {
            //open credits form
            Credits creditsForm = new Credits();
            creditsForm.Show();
        }

        
    }
}
