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
using KinectDrawing.Sounds;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KinectDrawing
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>

    public partial class Menu : Window
    {
        private Sounds.Sounds s;
        private bool isRightHand;
        public Menu()
        {
            InitializeComponent();

            s = new Sounds.Sounds();
            s.playOpeningSound();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Menu menuWindow = new Menu();
            MainWindow mainWindow = new MainWindow(isRightHand);
            mainWindow.Show();
            menuWindow.Hide();
            menuWindow.Close();
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

       

        private void right_hand_check(object sender, RoutedEventArgs e)
        {
            isRightHand = true;
        }

        private void left_hand_check(object sender, RoutedEventArgs e)
        {
            isRightHand = false;
        }
    }
}
