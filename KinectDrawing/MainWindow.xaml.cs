using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using KinectDrawing.Game;
using KinectDrawing.Sounds;
using KinectDrawing.Game.LevelClasses;
using Microsoft.Samples.Kinect.SpeechBasics;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;
using Path = System.IO.Path;
using WpfAnimatedGif;
using System.Windows.Media.Animation;
using System.Configuration;

namespace KinectDrawing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
           InitializeComponent();
            Menu menu= new Menu();
            Main.Content = menu;
            PythonProcess.initProcess();
            //load dimenstions of menu 
          //  Application.Current.MainWindow.Height = Double.Parse(ConfigurationManager.AppSettings["MenuHeight"]);
           // Application.Current.MainWindow.Width = Double.Parse(ConfigurationManager.AppSettings["MenuWidth"]);
        }

       
    }
}