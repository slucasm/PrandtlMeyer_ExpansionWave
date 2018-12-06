﻿using System;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            //Rectangle[,] matrixrectangle = new Rectangle[100, 100];
            //Rectangle[,] computationalmatrix;
            //double C = 0.5;
            //double Incremento_ETA = 0.1;
            //double theta = 5.352;
            //double M_1 = 2;
            //double mu = Math.Pow(Math.Sin(1 / M_1),-1);
            //double Incremento_XI_op1 = (C*Incremento_ETA)/(Math.Tan(theta+mu));
            //double Incremento_XI_op2 = (C * Incremento_ETA) / (Math.Tan(theta - mu));
            //double Incremento_XI = Math.Max(Incremento_XI_op1,Incremento_XI_op2);
            //double L = 65;
            //double H = 40;
            //double E = 10;

            //for (int incrementoX = 0; incrementoX < 10; incrementoX += (incrementoX / 10))
            //{
            //    for (int incrementoY = 0; incrementoY < 40; incrementoY += (incrementoY / 40))
            //    {

            //    }

            //}

            
            //for (double AX = 0; AX < L; AX++)
            //{
            //    if (AX < E)
            //    {
            //        int j = 0;
            //        while (AX < E)
            //        {
            //            for (int i = 0; i < 40; i++)
            //            {
            //                Rectangle rect = new Rectangle();
            //                rect.Height = 0.025;
            //                //rect.Width = ;
                            
            //            }
            //        }
            //    }
            //    else
            //    {

            //    }
            //}


            //Matriz matriz = new Matriz();
            //matriz.actualizarRectanglesMatrix();

            //double uno = matriz.matrix[1, 1].rectangle.Width;
            //double dos = matriz.matrix[1, 1].rectangle.Height;




        }

        private void button_simulation_Click(object sender, RoutedEventArgs e)
        {
            Simulation simulation = new Simulation();
            simulation.Show();
            this.Hide();
        }

        private void button_tableresults_Click(object sender, RoutedEventArgs e)
        {
            Tables tables = new Tables();
            tables.Show();
            this.Hide();
        }

        private void button_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
    }
}
