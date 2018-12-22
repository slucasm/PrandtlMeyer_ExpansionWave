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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.IO;
//using ClosedXML;

using ExcelLibrary.CompoundDocumentFormat;
using ExcelLibrary.SpreadSheet;


namespace WPF
{
    /// <summary>
    /// Interaction logic for Tables.xaml
    /// </summary>
    public partial class Tables : Window
    {
        public Tables(Matriz matrix, Simulation originalForm, List<DataTable> listTables)
        {
            InitializeComponent();
            this.matrix = matrix;
            this.originalForm = originalForm;
            this.listTables = listTables;
        }
        Simulation originalForm;
        Matriz matrix;

        int index;

        List<DataTable> listTables = new List<DataTable>();

        private void button_introduction_Click(object sender, RoutedEventArgs e)
        {
            try //goes to introduction form
            {
                MainWindow intro = new MainWindow();
                intro.Show();
                this.Hide();
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }
        }

        private void button_simulation_Click(object sender, RoutedEventArgs e)
        {
            try //returns to simulator form
            {
                originalForm.Show();
                this.Hide();
            }
            catch (Exception b)
            {
                MessageBox.Show(b.Message);
            }
        }

        private void button_close_Click(object sender, RoutedEventArgs e)
        {
            try //close the program
            {
                this.Close();
            }
            catch (Exception c)
            {
                MessageBox.Show(c.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try 
            {
                comboBox_selectgrid.Items.Add("U grid");
                comboBox_selectgrid.Items.Add("V grid");
                comboBox_selectgrid.Items.Add("ρ grid");
                comboBox_selectgrid.Items.Add("P grid");
                comboBox_selectgrid.Items.Add("T grid");
                comboBox_selectgrid.Items.Add("M grid");

                comboBox_selectgrid.SelectedIndex = 0;

                index = 0;

                for (int i = 0; i < Convert.ToDouble(originalForm.textBox_columns.Text); i++)
                {
                    double x = originalForm.matrix.matrix[1, i].x;

                    if (x >= 12.928)
                    {
                        index = i;
                        break;
                    }

                }

                label_clickindex.Content = String.Format("Click on any cell in the {0} column or the last column to compare with the results of the Anderson tables", index);


                grid_u.ItemsSource = listTables[0].DefaultView;
                grid_v.ItemsSource = listTables[1].DefaultView;
                grid_rho.ItemsSource = listTables[2].DefaultView;
                grid_P.ItemsSource = listTables[3].DefaultView;
                grid_T.ItemsSource = listTables[4].DefaultView;
                grid_M.ItemsSource = listTables[5].DefaultView;

                List<double> listu = new List<double>();
                List<double> listv = new List<double>();
                List<double> listP = new List<double>();
                List<double> listRho = new List<double>();
                List<double> listT = new List<double>();
                List<double> listM = new List<double>();

                DataColumn columnu = listTables[0].Columns[listTables[0].Columns.Count - 1];
                DataColumn columnv = listTables[1].Columns[listTables[1].Columns.Count - 1];
                DataColumn columnRho = listTables[2].Columns[listTables[2].Columns.Count - 1];
                DataColumn columnP = listTables[3].Columns[listTables[3].Columns.Count - 1];
                DataColumn columnT = listTables[4].Columns[listTables[4].Columns.Count - 1];
                DataColumn columnM = listTables[5].Columns[listTables[5].Columns.Count - 1];

                for (int i = 1; i <= 22; i++)
                {
                    listu.Add(Convert.ToDouble(listTables[0].Rows[i][columnu.ColumnName]));
                    listv.Add(Convert.ToDouble(listTables[1].Rows[i][columnv.ColumnName]));
                    listRho.Add(Convert.ToDouble(listTables[2].Rows[i][columnRho.ColumnName]));
                    listP.Add(Convert.ToDouble(listTables[3].Rows[i][columnP.ColumnName]));
                    listT.Add(Convert.ToDouble(listTables[4].Rows[i][columnT.ColumnName]));
                    listM.Add(Convert.ToDouble(listTables[5].Rows[i][columnM.ColumnName]));
                }

                double mean_u = listu.Average();
                double mean_v = listv.Average();
                double mean_Rho = listRho.Average();
                double mean_P = listP.Average();
                double mean_T = listT.Average();
                double mean_M = listM.Average();


                double error_u = Math.Round((Math.Abs(mean_u - 710) / 710) * 100, 3);
                double error_v = Math.Round((Math.Abs(mean_v + 66.5) / 66.5) * 100, 3);
                double error_Rho = Math.Round((Math.Abs(mean_Rho - 0.984) / 0.984) * 100, 3);
                double error_P = Math.Round((Math.Abs(mean_P - 73900) / 73900) * 100, 3);
                double error_T = Math.Round((Math.Abs(mean_T - 262) / 262) * 100, 3);
                double error_M = Math.Round((Math.Abs(mean_M - 2.2) / 2.2) * 100, 3);

                label_u.Content = "u: " + error_u.ToString();
                label_v.Content = "v: " + error_v.ToString();
                label_Rho.Content = "ρ: " + error_Rho.ToString();
                label_P.Content = "P: " + error_P.ToString();
                label_T.Content = "T: " + error_T.ToString();
                label_M.Content = "M: " + error_M.ToString();

            }
            catch (Exception d)
            {
                MessageBox.Show(d.Message);
            }
        }

        private void comboBox_selectgrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try //when combobox change index the tables change
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

                image_inter_u.Visibility = Visibility.Hidden;
                image_inter_v.Visibility = Visibility.Hidden;
                image_inter_rho.Visibility = Visibility.Hidden;
                image_inter_P.Visibility = Visibility.Hidden;
                image_inter_T.Visibility = Visibility.Hidden;
                image_inter_M.Visibility = Visibility.Hidden;
                image_final_u.Visibility = Visibility.Hidden;
                image_final_v.Visibility = Visibility.Hidden;
                image_final_rho.Visibility = Visibility.Hidden;
                image_final_P.Visibility = Visibility.Hidden;
                image_final_T.Visibility = Visibility.Hidden;
                image_final_M.Visibility = Visibility.Hidden;
            }
            catch (Exception y)
            {
                MessageBox.Show(y.Message);
            }
        }

        //when click on the correct columns, appear the associated anderson column
        private void grid_u_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (grid_u.CurrentColumn.DisplayIndex == index)
                {
                    image_inter_u.Visibility = Visibility.Visible;
                }
                if (grid_u.CurrentColumn.DisplayIndex == listTables[0].Columns.Count - 1)
                {
                    image_final_u.Visibility = Visibility.Visible;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.Message);
            }

        }
        private void grid_v_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (grid_v.CurrentColumn.DisplayIndex == index)
                {
                    image_inter_v.Visibility = Visibility.Visible;
                }
                if (grid_v.CurrentColumn.DisplayIndex == listTables[0].Columns.Count - 1)
                {
                    image_final_v.Visibility = Visibility.Visible;
                }
            }
            catch (Exception g)
            {
                MessageBox.Show(g.Message);
            }
        }
        private void grid_rho_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (grid_rho.CurrentColumn.DisplayIndex == index)
                {
                    image_inter_rho.Visibility = Visibility.Visible;
                }
                if (grid_rho.CurrentColumn.DisplayIndex == listTables[0].Columns.Count - 1)
                {
                    image_final_rho.Visibility = Visibility.Visible;
                }
            }
            catch (Exception h)
            {
                MessageBox.Show(h.Message);
            }
        }
        private void grid_P_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (grid_P.CurrentColumn.DisplayIndex == index)
                {
                    image_inter_P.Visibility = Visibility.Visible;
                }
                if (grid_P.CurrentColumn.DisplayIndex == listTables[0].Columns.Count - 1)
                {
                    image_final_P.Visibility = Visibility.Visible;
                }
            }
            catch (Exception j)
            {
                MessageBox.Show(j.Message);
            }
        }
        private void grid_T_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (grid_T.CurrentColumn.DisplayIndex == index)
                {
                    image_inter_T.Visibility = Visibility.Visible;
                }
                if (grid_T.CurrentColumn.DisplayIndex == listTables[0].Columns.Count - 1)
                {
                    image_final_T.Visibility = Visibility.Visible;
                }
            }
            catch (Exception k)
            {
                MessageBox.Show(k.Message);
            }
        }
        private void grid_M_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (grid_M.CurrentColumn.DisplayIndex == index)
                {
                    image_inter_M.Visibility = Visibility.Visible;
                }
                if (grid_M.CurrentColumn.DisplayIndex == listTables[0].Columns.Count - 1)
                {
                    image_final_M.Visibility = Visibility.Visible;
                }
            }
            catch (Exception l)
            {
                MessageBox.Show(l.Message);
            }
        }

        //if selection of column change, the column of anderson also change
        private void grid_u_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (grid_u.CurrentColumn.DisplayIndex == index)
                {
                    image_inter_u.Visibility = Visibility.Visible;
                    image_inter_v.Visibility = Visibility.Hidden;
                    image_inter_rho.Visibility = Visibility.Hidden;
                    image_inter_P.Visibility = Visibility.Hidden;
                    image_inter_T.Visibility = Visibility.Hidden;
                    image_inter_M.Visibility = Visibility.Hidden;
                    image_final_u.Visibility = Visibility.Hidden;
                    image_final_v.Visibility = Visibility.Hidden;
                    image_final_rho.Visibility = Visibility.Hidden;
                    image_final_P.Visibility = Visibility.Hidden;
                    image_final_T.Visibility = Visibility.Hidden;
                    image_final_M.Visibility = Visibility.Hidden;
                }
                if (grid_u.CurrentColumn.DisplayIndex == listTables[0].Columns.Count - 1)
                {
                    image_inter_u.Visibility = Visibility.Hidden;
                    image_inter_v.Visibility = Visibility.Hidden;
                    image_inter_rho.Visibility = Visibility.Hidden;
                    image_inter_P.Visibility = Visibility.Hidden;
                    image_inter_T.Visibility = Visibility.Hidden;
                    image_inter_M.Visibility = Visibility.Hidden;
                    image_final_u.Visibility = Visibility.Visible;
                    image_final_v.Visibility = Visibility.Hidden;
                    image_final_rho.Visibility = Visibility.Hidden;
                    image_final_P.Visibility = Visibility.Hidden;
                    image_final_T.Visibility = Visibility.Hidden;
                    image_final_M.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception m)
            {
                MessageBox.Show(m.Message);
            }
        }
        private void grid_v_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (grid_v.CurrentColumn.DisplayIndex == index)
                {
                    image_inter_u.Visibility = Visibility.Hidden;
                    image_inter_v.Visibility = Visibility.Visible;
                    image_inter_rho.Visibility = Visibility.Hidden;
                    image_inter_P.Visibility = Visibility.Hidden;
                    image_inter_T.Visibility = Visibility.Hidden;
                    image_inter_M.Visibility = Visibility.Hidden;
                    image_final_u.Visibility = Visibility.Hidden;
                    image_final_v.Visibility = Visibility.Hidden;
                    image_final_rho.Visibility = Visibility.Hidden;
                    image_final_P.Visibility = Visibility.Hidden;
                    image_final_T.Visibility = Visibility.Hidden;
                    image_final_M.Visibility = Visibility.Hidden;
                }
                if (grid_v.CurrentColumn.DisplayIndex == listTables[0].Columns.Count - 1)
                {
                    image_inter_u.Visibility = Visibility.Hidden;
                    image_inter_v.Visibility = Visibility.Hidden;
                    image_inter_rho.Visibility = Visibility.Hidden;
                    image_inter_P.Visibility = Visibility.Hidden;
                    image_inter_T.Visibility = Visibility.Hidden;
                    image_inter_M.Visibility = Visibility.Hidden;
                    image_final_u.Visibility = Visibility.Hidden;
                    image_final_v.Visibility = Visibility.Visible;
                    image_final_rho.Visibility = Visibility.Hidden;
                    image_final_P.Visibility = Visibility.Hidden;
                    image_final_T.Visibility = Visibility.Hidden;
                    image_final_M.Visibility = Visibility.Hidden;
                }

            }
            catch (Exception n)
            {
                MessageBox.Show(n.Message);
            }

        }
        private void grid_rho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                if (grid_rho.CurrentColumn.DisplayIndex == index)
                {
                    image_inter_u.Visibility = Visibility.Hidden;
                    image_inter_v.Visibility = Visibility.Hidden;
                    image_inter_rho.Visibility = Visibility.Visible;
                    image_inter_P.Visibility = Visibility.Hidden;
                    image_inter_T.Visibility = Visibility.Hidden;
                    image_inter_M.Visibility = Visibility.Hidden;
                    image_final_u.Visibility = Visibility.Hidden;
                    image_final_v.Visibility = Visibility.Hidden;
                    image_final_rho.Visibility = Visibility.Hidden;
                    image_final_P.Visibility = Visibility.Hidden;
                    image_final_T.Visibility = Visibility.Hidden;
                    image_final_M.Visibility = Visibility.Hidden;
                }
                if (grid_rho.CurrentColumn.DisplayIndex == listTables[0].Columns.Count - 1)
                {
                    image_inter_u.Visibility = Visibility.Hidden;
                    image_inter_v.Visibility = Visibility.Hidden;
                    image_inter_rho.Visibility = Visibility.Hidden;
                    image_inter_P.Visibility = Visibility.Hidden;
                    image_inter_T.Visibility = Visibility.Hidden;
                    image_inter_M.Visibility = Visibility.Hidden;
                    image_final_u.Visibility = Visibility.Hidden;
                    image_final_v.Visibility = Visibility.Hidden;
                    image_final_rho.Visibility = Visibility.Visible;
                    image_final_P.Visibility = Visibility.Hidden;
                    image_final_T.Visibility = Visibility.Hidden;
                    image_final_M.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception o)
            {
                MessageBox.Show(o.Message);
            }

        }
        private void grid_P_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (grid_P.CurrentColumn.DisplayIndex == index)
                {
                    image_inter_u.Visibility = Visibility.Hidden;
                    image_inter_v.Visibility = Visibility.Hidden;
                    image_inter_rho.Visibility = Visibility.Hidden;
                    image_inter_P.Visibility = Visibility.Visible;
                    image_inter_T.Visibility = Visibility.Hidden;
                    image_inter_M.Visibility = Visibility.Hidden;
                    image_final_u.Visibility = Visibility.Hidden;
                    image_final_v.Visibility = Visibility.Hidden;
                    image_final_rho.Visibility = Visibility.Hidden;
                    image_final_P.Visibility = Visibility.Hidden;
                    image_final_T.Visibility = Visibility.Hidden;
                    image_final_M.Visibility = Visibility.Hidden;
                }
                if (grid_P.CurrentColumn.DisplayIndex == listTables[0].Columns.Count - 1)
                {
                    image_inter_u.Visibility = Visibility.Hidden;
                    image_inter_v.Visibility = Visibility.Hidden;
                    image_inter_rho.Visibility = Visibility.Hidden;
                    image_inter_P.Visibility = Visibility.Hidden;
                    image_inter_T.Visibility = Visibility.Hidden;
                    image_inter_M.Visibility = Visibility.Hidden;
                    image_final_u.Visibility = Visibility.Hidden;
                    image_final_v.Visibility = Visibility.Hidden;
                    image_final_rho.Visibility = Visibility.Hidden;
                    image_final_P.Visibility = Visibility.Visible;
                    image_final_T.Visibility = Visibility.Hidden;
                    image_final_M.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception p)
            {
                MessageBox.Show(p.Message);
            }
        }
        private void grid_T_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                if (grid_T.CurrentColumn.DisplayIndex == index)
                {
                    image_inter_u.Visibility = Visibility.Hidden;
                    image_inter_v.Visibility = Visibility.Hidden;
                    image_inter_rho.Visibility = Visibility.Hidden;
                    image_inter_P.Visibility = Visibility.Hidden;
                    image_inter_T.Visibility = Visibility.Visible;
                    image_inter_M.Visibility = Visibility.Hidden;
                    image_final_u.Visibility = Visibility.Hidden;
                    image_final_v.Visibility = Visibility.Hidden;
                    image_final_rho.Visibility = Visibility.Hidden;
                    image_final_P.Visibility = Visibility.Hidden;
                    image_final_T.Visibility = Visibility.Hidden;
                    image_final_M.Visibility = Visibility.Hidden;
                }
                if (grid_T.CurrentColumn.DisplayIndex == listTables[0].Columns.Count - 1)
                {
                    image_inter_u.Visibility = Visibility.Hidden;
                    image_inter_v.Visibility = Visibility.Hidden;
                    image_inter_rho.Visibility = Visibility.Hidden;
                    image_inter_P.Visibility = Visibility.Hidden;
                    image_inter_T.Visibility = Visibility.Hidden;
                    image_inter_M.Visibility = Visibility.Hidden;
                    image_final_u.Visibility = Visibility.Hidden;
                    image_final_v.Visibility = Visibility.Hidden;
                    image_final_rho.Visibility = Visibility.Hidden;
                    image_final_P.Visibility = Visibility.Hidden;
                    image_final_T.Visibility = Visibility.Visible;
                    image_final_M.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception q)
            {
                MessageBox.Show(q.Message);
            }
        }
        private void grid_M_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (grid_M.CurrentColumn.DisplayIndex == index)
                {
                    image_inter_u.Visibility = Visibility.Hidden;
                    image_inter_v.Visibility = Visibility.Hidden;
                    image_inter_rho.Visibility = Visibility.Hidden;
                    image_inter_P.Visibility = Visibility.Hidden;
                    image_inter_T.Visibility = Visibility.Hidden;
                    image_inter_M.Visibility = Visibility.Visible;
                    image_final_u.Visibility = Visibility.Hidden;
                    image_final_v.Visibility = Visibility.Hidden;
                    image_final_rho.Visibility = Visibility.Hidden;
                    image_final_P.Visibility = Visibility.Hidden;
                    image_final_T.Visibility = Visibility.Hidden;
                    image_final_M.Visibility = Visibility.Hidden;
                }
                if (grid_M.CurrentColumn.DisplayIndex == listTables[0].Columns.Count - 1)
                {
                    image_inter_u.Visibility = Visibility.Hidden;
                    image_inter_v.Visibility = Visibility.Hidden;
                    image_inter_rho.Visibility = Visibility.Hidden;
                    image_inter_P.Visibility = Visibility.Hidden;
                    image_inter_T.Visibility = Visibility.Hidden;
                    image_inter_M.Visibility = Visibility.Hidden;
                    image_final_u.Visibility = Visibility.Hidden;
                    image_final_v.Visibility = Visibility.Hidden;
                    image_final_rho.Visibility = Visibility.Hidden;
                    image_final_P.Visibility = Visibility.Hidden;
                    image_final_T.Visibility = Visibility.Hidden;
                    image_final_M.Visibility = Visibility.Visible;
                }
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message);
            }
        }

        //save the tables in a excel file
        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                listTables[0].TableName = "u";

                listTables[1].TableName = "v";

                listTables[2].TableName = "rho";

                listTables[3].TableName = "P";

                listTables[4].TableName = "T";

                listTables[5].TableName = "M";


                SaveFileDialog ofd = new SaveFileDialog();
                ofd.DefaultExt = "xlsx";
                ofd.Filter = "Excel files|*.xls";
                ofd.Title = "Save the tables";
                ofd.ShowDialog();
                string nombre = ofd.FileName;

                Workbook workbook = new Workbook();
                for (int x = 0; x < listTables.Count; x++)
                {
                    Worksheet worksheet = new Worksheet(listTables[x].TableName);
                    for (int j = 0; j < listTables[x].Rows.Count; j++)
                    {
                        for (int i = 0; i < listTables[x].Columns.Count; i++)
                        {
                            worksheet.Cells[j, i] = new ExcelLibrary.SpreadSheet.Cell(Convert.ToDouble(listTables[x].Rows[j][i].ToString()));
                        }
                    }
                    workbook.Worksheets.Add(worksheet);
                }
                workbook.Save(nombre);

                MessageBox.Show("File saved correctly");
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }

        }


    }
}
