using PhotoViewer;
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

namespace PhotoViewerPRCVI
{
    /// <summary>
    /// Логика взаимодействия для ChangeWindow.xaml
    /// </summary>
    public partial class ChangeWindow : Window
    {
        string type;

        /// <summary>
        /// Конструктор окна для изменения информации о снимке
        /// </summary>
        public ChangeWindow()
        {
            InitializeComponent();
            type = "original";
            this.Title = "Изменить информацию об оригинальном снимке";
        }

        /// <summary>
        /// Конструктор окна для изменения информации о снимке
        /// </summary>
        public ChangeWindow(int ID, string type)
        {
            InitializeComponent();

            if (type == "markup")
            {
                this.type = type;
                this.Title = "Изменить информацию о размеченном снимке";
            }
            else
            {
                this.type = "original";
                this.Title = "Изменить информацию об оригинальном снимке";
            }
        }

        /// <summary>
        /// Сохранить изменения и вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAndBackToMain(object sender, RoutedEventArgs e)
        {
            MainWindow MW = new MainWindow();
            MW.Show();
            this.Close();
        }

        /// <summary>
        /// Вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToMain(object sender, RoutedEventArgs e)
        {
            MainWindow MW = new MainWindow();
            MW.Show();
            this.Close();
        }
    }
}
