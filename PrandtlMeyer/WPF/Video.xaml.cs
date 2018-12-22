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
using Microsoft.Win32;

namespace WPF
{
    /// <summary>
    /// Interaction logic for Video.xaml
    /// </summary>
    public partial class Video : Window
    {
        public Video()
        {
            InitializeComponent();
        }

        MediaElement mediaElement;
        Boolean isPaused = true;
        Boolean firstPlay = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //load video presets
            string directory = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));

            mediaElement = new MediaElement();
            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.ScrubbingEnabled = true;
            mediaElement.Source = new Uri(directory+ "\\Videotutorial.avi");
            mediaElement.Position = TimeSpan.FromSeconds(1);
            mediaElement.Stretch = Stretch.Fill;
            grid_movie.Children.Add(mediaElement);
            mediaElement.Play();
            mediaElement.Pause();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            //play video
            mediaElement.Play();
            firstPlay = true;
            isPaused = false;
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            //pause video
            mediaElement.Pause();
            isPaused = true;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            //stop video (return to initial position)
            isPaused = true;
            mediaElement.Stop();
            slider_changeposition.Value = 0;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //close program
            mediaElement.Stop();
            this.Hide();
        }

        private void slider_changeposition_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //change position of video with slider
            if (!firstPlay)
                MessageBox.Show("First, click play once time");
            else if (isPaused)
                mediaElement.Position = TimeSpan.FromSeconds(slider_changeposition.Value);
            else
                MessageBox.Show("Please, pause the video to change position");
        }

    }
}
