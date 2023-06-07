using System;
using System.Windows;
using System.Configuration;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media.Imaging;
using PhotoViewerPRCVI;

namespace PhotoViewer
{
    /// <summary>
    /// Логика взаимодействия для FullInfoWindow.xaml
    /// </summary>
    public partial class FullInfoWindow : Window
    {
        //ссылка на главное окно
        MainWindow MW;

        //ID изображений
        int OriginalID, MarkupID;

        /// <summary>
        /// Конструктор окна доп. информации
        /// </summary>
        public FullInfoWindow(int OriginalID, int MarkupID, MainWindow MW)
        {
            InitializeComponent();
            this.MW = MW;
            this.OriginalID = OriginalID;
            this.MarkupID = MarkupID;
        }

        /// <summary>
        /// Загрузка окна доп. информации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //заполнение таблицы информацией
                string SQL = "SELECT * FROM dbo.Originals";
                PhotoViewerImage.LoadTable(FullInfoTable, SQL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        /// <summary>
        /// Возвращение на главное окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToMain(object sender, RoutedEventArgs e)
        {
            //проверка загрузки главного окна, восстановление если окно закрыто
            if (this.MW.IsLoaded) MW.Focus();
            else
            {
                this.MW = new MainWindow(OriginalID, MarkupID);
                this.MW.Show();
            }
            this.Close();

            //сброс информации об открытом окне информации
            this.MW.FIW = null;
        }
    }
}
