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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Конструктор главного окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Переход в окно полной информации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickMeBtn_Click(object sender, RoutedEventArgs e)
        {
            Button Btn = (Button)sender;
            FullInfoWindow FIW = new FullInfoWindow(1, 1);
            FIW.Show();
            this.Close();
        }
    }
}
