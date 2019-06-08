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
using System.Windows.Navigation;
using System.Configuration;

namespace KinectDrawing
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>

    public partial class Menu : Page
    {
        private TheGame gameInstance;
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
            NavigationService ns = NavigationService.GetNavigationService(this);

            if (gameInstance == null)
            {
                gameInstance = new TheGame(isRightHand);
            }
            gameInstance.setIsHandRight(isRightHand);
            ns.Navigate(gameInstance);
            //load TheGame dimenstions
            //Application.Current.MainWindow.Height = Double.Parse(ConfigurationManager.AppSettings["TheGameHeight"]);
            //Application.Current.MainWindow.Width = Double.Parse(ConfigurationManager.AppSettings["TheGameWidth"]);
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
