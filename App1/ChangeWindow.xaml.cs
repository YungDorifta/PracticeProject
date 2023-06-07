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
        //тип изменяемого фото
        string type;

        //ссылка на главное окно
        MainWindow MW;

        /// <summary>
        /// Конструктор окна для изменения информации о снимке
        /// </summary>
        public ChangeWindow(MainWindow MW)
        {
            InitializeComponent();
            type = "original";
            this.Title = "Изменить информацию об оригинальном снимке";
            this.MW = MW;
        }

        /// <summary>
        /// Конструктор окна для изменения информации о снимке
        /// </summary>
        public ChangeWindow(int ID, string type, MainWindow MW)
        {
            InitializeComponent();
            this.MW = MW;

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
            //проверка загрузки главного окна, восстановление если окно закрыто
            if (this.MW.IsLoaded) MW.Focus();
            else
            {
                this.MW = new MainWindow();
                this.MW.Show();
            }
            this.Close();

            //сброс информации об открытом окне изменения
            this.MW.CW = null;
        }

        /// <summary>
        /// Вернуться в главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToMain(object sender, RoutedEventArgs e)
        {
            //проверка загрузки главного окна, восстановление если окно закрыто
            if (this.MW.IsLoaded) MW.Focus();
            else
            {
                this.MW = new MainWindow();
                this.MW.Show();
            }
            this.Close();

            //сброс информации об открытом окне изменения
            this.MW.CW = null;
        }
    }
}
