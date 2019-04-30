using KinectDrawing.Game.LevelClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KinectDrawing
{
    /// <summary>
    /// Interaction logic for Level2.xaml
    /// </summary>
    public partial class Level2Game : Window
    {
        private Level level;
        public Level2Game()
        {
            InitializeComponent();
            level = new Level2();
            
            
        }

        private void GameLevelBase_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
